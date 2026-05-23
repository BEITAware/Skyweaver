using System.Globalization;
using System.IO;
using System.Text;
using System.Xml.Linq;
using AerialCity;
using AerialCity.Core.Primitives;
using AerialCity.Core.Storage;
using AerialCity.Database;
using AerialCity.Retrieval;
using Skyweaver.Controls.EmbeddingModelConfigurationControl.Models;
using Skyweaver.Controls.EmbeddingModelConfigurationControl.Services;
using Skyweaver.Controls.LanguageModelConfigurationControl.Services;
using Skyweaver.Controls.WorkflowEditorControl.Models;
using Skyweaver.Controls.WorkflowEditorControl.Services;
using Skyweaver.Models.ChatSession;
using Skyweaver.Models.ContextManagement;
using Skyweaver.Services.AerialCityRag;
using Skyweaver.Services.ChatSession;
using Skyweaver.Services.ContextManagement;
using Skyweaver.Services.Directories;
using Skyweaver.Services.Localization;
using Skyweaver.Services.Notifications;

namespace Skyweaver.Services.Memory
{
    public sealed class MemoryService
    {
        private const int RetrievalCandidateCount = 120;
        private const string MemoryCollectionName = "Skyweaver.Memory.Blocks";
        private const string VectorsDatabaseName = "Vectors";

        private static readonly Encoding Utf8NoBom = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
        private static readonly SemaphoreSlim s_vectorDatabaseGate = new(1, 1);

        private readonly ContextManagementRuntime _contextRuntime;
        private readonly AerialCityRagConfigurationRepository _ragConfigurationRepository;
        private readonly EmbeddingModelConfigurationRepository _embeddingModelRepository;
        private readonly EmbeddingModelService _embeddingModelService;
        private readonly ChatSessionFlowBindingService _flowBindingService;
        private readonly SessionFlowPayloadRouter _payloadRouter;

        public MemoryService()
            : this(
                ContextManagementRuntime.Instance,
                new AerialCityRagConfigurationRepository(),
                new EmbeddingModelConfigurationRepository(new EmbeddingModelConfigurationPathProvider()),
                new EmbeddingModelService(),
                new ChatSessionFlowBindingService(),
                new SessionFlowPayloadRouter())
        {
        }

        internal MemoryService(
            ContextManagementRuntime contextRuntime,
            AerialCityRagConfigurationRepository ragConfigurationRepository,
            EmbeddingModelConfigurationRepository embeddingModelRepository,
            EmbeddingModelService embeddingModelService,
            ChatSessionFlowBindingService flowBindingService,
            SessionFlowPayloadRouter payloadRouter)
        {
            _contextRuntime = contextRuntime ?? throw new ArgumentNullException(nameof(contextRuntime));
            _ragConfigurationRepository = ragConfigurationRepository ?? throw new ArgumentNullException(nameof(ragConfigurationRepository));
            _embeddingModelRepository = embeddingModelRepository ?? throw new ArgumentNullException(nameof(embeddingModelRepository));
            _embeddingModelService = embeddingModelService ?? throw new ArgumentNullException(nameof(embeddingModelService));
            _flowBindingService = flowBindingService ?? throw new ArgumentNullException(nameof(flowBindingService));
            _payloadRouter = payloadRouter ?? throw new ArgumentNullException(nameof(payloadRouter));
        }

        public async Task GenerateForClosedSessionAsync(
            ChatSessionModel session,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(session);

            var configuration = _contextRuntime.GetConfiguration();
            if (!configuration.MemoryEnabled)
            {
                return;
            }

            var triggerMsg = LocalizationRuntime.Instance.GetString(
                "Memory.Status.TriggeringPersistenceAndEmbedding",
                "Memory: Triggering persistence and embedding...");
            NotificationService.Instance.ShowTransient(triggerMsg);

            var notificationId = NotificationService.Instance.CreatePermanent("Memory: preparing chat session...", 0d);
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                var layout = MemoryLayout.CreateDefault();
                layout.EnsureCreated();
                var documentFileName = CreateSessionMemoryFileName(session);
                var documentPath = Path.Combine(layout.ChatSessionsPath, documentFileName);
                var blocks = BuildMemoryBlocks(session, documentFileName, documentPath);

                NotificationService.Instance.UpdatePermanent(notificationId, "Memory: writing natural-language transcript...", 0.12d);
                PrepareReplacementPath(documentPath);
                await SaveMemoryDocumentAsync(session, blocks, documentPath, cancellationToken).ConfigureAwait(false);

                if (blocks.Count == 0)
                {
                    NotificationService.Instance.UpdatePermanent(notificationId, "Memory: no natural-language blocks found.", 1d);
                    return;
                }

                if (!TryResolveSelectedEmbeddingModel(out var model, out var modelError) || model == null)
                {
                    NotificationService.Instance.ShowTransient($"Memory saved without vectors: {modelError}");
                    return;
                }

                var embeddedSegments = new List<Segment>(blocks.Count);

                for (var index = 0; index < blocks.Count; index++)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var block = blocks[index];
                    var progress = 0.2d + ((index + 1) / (double)blocks.Count) * 0.6d;
                    NotificationService.Instance.UpdatePermanent(
                        notificationId,
                        $"Memory: embedding block {index + 1} / {blocks.Count}...",
                        progress);

                    var segment = new Segment(SegmentKind.TextPassage, block.Content)
                    {
                        SourceUri = documentPath,
                        CollectionName = MemoryCollectionName,
                        Metadata = CreateSegmentMetadata(block)
                    };

                    var embedding = await _embeddingModelService
                        .EmbedTextAsync(model, block.Content, cancellationToken)
                        .ConfigureAwait(false);
                    segment.Embedding = embedding.Vector;
                    embeddedSegments.Add(segment);
                }

                NotificationService.Instance.UpdatePermanent(notificationId, "Memory: opening vector database...", 0.82d);
                await s_vectorDatabaseGate.WaitAsync(cancellationToken).ConfigureAwait(false);
                try
                {
                    using var engine = new AerialCityBuilder().Build();
                    await using var database = await OpenMemoryDatabaseAsync(engine, layout, cancellationToken).ConfigureAwait(false);
                    var insert = engine.Insert();

                    for (var index = 0; index < embeddedSegments.Count; index++)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        NotificationService.Instance.UpdatePermanent(
                            notificationId,
                            $"Memory: storing vector {index + 1} / {embeddedSegments.Count}...",
                            0.82d + ((index + 1) / (double)embeddedSegments.Count) * 0.16d);
                        await insert(database, embeddedSegments[index], cancellationToken).ConfigureAwait(false);
                    }
                }
                finally
                {
                    s_vectorDatabaseGate.Release();
                }

                NotificationService.Instance.UpdatePermanent(notificationId, "Memory: complete.", 1d);

                var completedMsg = LocalizationRuntime.Instance.GetString(
                    "Memory.Status.EmbeddingCompleted",
                    "Memory: Embedding completed.");
                NotificationService.Instance.ShowTransient(completedMsg);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                NotificationService.Instance.ShowTransient($"Memory failed: {ex.Message}");
            }
            finally
            {
                NotificationService.Instance.RemovePermanent(notificationId);
            }
        }

        public async Task<IReadOnlyList<LanguageModelChatMessage>> RetrieveBackfillMessagesAsync(
            ChatSessionModel session,
            string userText,
            IReadOnlyList<LanguageModelChatContentBlock>? userContentBlocks = null,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(session);

            var configuration = _contextRuntime.GetConfiguration();
            if (!configuration.MemoryEnabled || configuration.MemoryRetrievalCount <= 0)
            {
                return Array.Empty<LanguageModelChatMessage>();
            }

            var queryText = BuildQueryText(userText, userContentBlocks);
            if (queryText.Length == 0)
            {
                return Array.Empty<LanguageModelChatMessage>();
            }

            var currentAgent = TryResolveNextTriggeredAgent(session, userText, userContentBlocks);
            if (configuration.MemoryShareScope == MemoryShareScope.Agent && currentAgent == null)
            {
                return Array.Empty<LanguageModelChatMessage>();
            }

            if (!TryResolveSelectedEmbeddingModel(out var model, out _) || model == null)
            {
                return Array.Empty<LanguageModelChatMessage>();
            }

            try
            {
                var queryEmbedding = await _embeddingModelService
                    .EmbedTextAsync(model, queryText, cancellationToken)
                    .ConfigureAwait(false);

                var layout = MemoryLayout.CreateDefault();
                if (!Directory.Exists(layout.VectorsDatabasePath))
                {
                    return Array.Empty<LanguageModelChatMessage>();
                }

                IReadOnlyList<RetrievalResult> rawResults;
                await s_vectorDatabaseGate.WaitAsync(cancellationToken).ConfigureAwait(false);
                try
                {
                    using var engine = new AerialCityBuilder().Build();
                    await using var database = await OpenMemoryDatabaseAsync(engine, layout, cancellationToken).ConfigureAwait(false);
                    var retrieve = engine.Retrieve();
                    rawResults = await retrieve(
                        database,
                        new RetrievalQuery
                        {
                            QueryVector = queryEmbedding.Vector,
                            TopK = RetrievalCandidateCount,
                            CollectionFilter = MemoryCollectionName
                        },
                        cancellationToken).ConfigureAwait(false);
                }
                finally
                {
                    s_vectorDatabaseGate.Release();
                }

                var distinctDocumentNames = rawResults
                    .Select(result => ReadMetadata(result.Segment.Metadata, "memoryDocumentName"))
                    .Where(value => value.Length > 0)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToArray();
                _ = distinctDocumentNames;

                var filteredResults = rawResults
                    .Where(result => IsResultAllowed(result.Segment, configuration, session, currentAgent))
                    .OrderByDescending(result => result.Score)
                    .Take(configuration.MemoryRetrievalCount)
                    .ToArray();

                if (filteredResults.Length == 0)
                {
                    return Array.Empty<LanguageModelChatMessage>();
                }

                return new[]
                {
                    CreateMemoryBackfillMessage(filteredResults)
                };
            }
            catch
            {
                return Array.Empty<LanguageModelChatMessage>();
            }
        }

        private static MemoryLayout CreateLayout() => MemoryLayout.CreateDefault();

        private static IReadOnlyList<MemoryBlock> BuildMemoryBlocks(
            ChatSessionModel session,
            string documentFileName,
            string documentPath)
        {
            var entriesById = session.Transcript.Entries
                .Where(entry => !string.IsNullOrWhiteSpace(entry.EntryId))
                .ToDictionary(entry => entry.EntryId, StringComparer.OrdinalIgnoreCase);
            var blocks = new List<MemoryBlock>();
            var blockIndex = 0;

            foreach (var turn in session.Transcript.Turns.OrderBy(turn => turn.TurnNumber))
            {
                entriesById.TryGetValue(turn.UserEntryId ?? string.Empty, out var userEntry);
                var assistantEntries = session.Transcript.Entries
                    .Where(entry => string.Equals(entry.TurnId, turn.TurnId, StringComparison.OrdinalIgnoreCase))
                    .Where(IsAssistantMemoryEntry)
                    .ToArray();
                var firstAssistant = assistantEntries.FirstOrDefault();
                var counterpart = CreateAgentIdentity(firstAssistant);

                var userContent = userEntry == null ? string.Empty : BuildEntryText(userEntry);
                if (userContent.Length > 0)
                {
                    blocks.Add(new MemoryBlock
                    {
                        BlockId = $"turn-{turn.TurnNumber:D6}-user",
                        BlockIndex = ++blockIndex,
                        TurnId = turn.TurnId,
                        TurnNumber = turn.TurnNumber,
                        Content = userContent,
                        AuthorKind = "User",
                        AuthorName = "User",
                        CounterpartAgentId = counterpart?.AgentId,
                        CounterpartAgentName = counterpart?.AgentName,
                        SessionId = session.SessionId,
                        SessionName = session.Name,
                        SessionFlowId = session.FlowBinding.GraphId,
                        SessionFlowName = session.BoundFlowDisplayName,
                        DocumentFileName = documentFileName,
                        DocumentPath = documentPath,
                        CreatedAtUtc = turn.StartedAtUtc
                    });
                }

                var assistantContent = BuildAssistantTurnText(assistantEntries);
                if (assistantContent.Length > 0)
                {
                    blocks.Add(new MemoryBlock
                    {
                        BlockId = $"turn-{turn.TurnNumber:D6}-assistant",
                        BlockIndex = ++blockIndex,
                        TurnId = turn.TurnId,
                        TurnNumber = turn.TurnNumber,
                        Content = assistantContent,
                        AuthorKind = "Agent",
                        AuthorAgentId = counterpart?.AgentId,
                        AuthorName = counterpart?.AgentName ?? "Assistant",
                        SessionId = session.SessionId,
                        SessionName = session.Name,
                        SessionFlowId = session.FlowBinding.GraphId,
                        SessionFlowName = session.BoundFlowDisplayName,
                        DocumentFileName = documentFileName,
                        DocumentPath = documentPath,
                        CreatedAtUtc = firstAssistant?.TimestampUtc ?? turn.CompletedAtUtc ?? turn.StartedAtUtc
                    });
                }
            }

            return blocks;
        }

        private static bool IsAssistantMemoryEntry(ChatSessionTranscriptEntry entry)
        {
            return entry.Role == ChatSessionParticipantRole.Assistant &&
                   entry.LlmPolicy == TranscriptLlmPolicy.Include &&
                   entry.Kind is ChatSessionTranscriptEntryKind.AgentMessage
                       or ChatSessionTranscriptEntryKind.AgentFinalOutput;
        }

        private static string BuildAssistantTurnText(IReadOnlyList<ChatSessionTranscriptEntry> entries)
        {
            var seen = new HashSet<string>(StringComparer.Ordinal);
            var parts = new List<string>();
            foreach (var entry in entries)
            {
                var text = BuildEntryText(entry);
                if (text.Length == 0 || !seen.Add(text))
                {
                    continue;
                }

                var author = CreateAgentIdentity(entry)?.AgentName ?? entry.NodeTitle ?? entry.AgentName;
                parts.Add(string.IsNullOrWhiteSpace(author)
                    ? text
                    : $"{author.Trim()}:{Environment.NewLine}{text}");
            }

            return string.Join(Environment.NewLine + Environment.NewLine, parts).Trim();
        }

        private static string BuildEntryText(ChatSessionTranscriptEntry entry)
        {
            var parts = entry.Blocks
                .Where(block => block.Kind is ChatSessionTranscriptBlockKind.Text or ChatSessionTranscriptBlockKind.Code)
                .Select(block => block.Content?.Trim() ?? string.Empty)
                .Where(content => content.Length > 0)
                .ToArray();

            return string.Join(Environment.NewLine + Environment.NewLine, parts).Trim();
        }

        private static MemoryAgentIdentity? CreateAgentIdentity(ChatSessionTranscriptEntry? entry)
        {
            if (entry == null)
            {
                return null;
            }

            var agentName = FirstNonEmpty(entry.AgentName, entry.NodeTitle);
            var agentId = Normalize(entry.AgentId);
            return agentId.Length == 0 && agentName.Length == 0
                ? null
                : new MemoryAgentIdentity(agentId, agentName);
        }

        private static async Task SaveMemoryDocumentAsync(
            ChatSessionModel session,
            IReadOnlyList<MemoryBlock> blocks,
            string documentPath,
            CancellationToken cancellationToken)
        {
            var document = new XDocument(
                new XElement(
                    "MemoryChatSession",
                    new XAttribute("SchemaVersion", 1),
                    new XAttribute("SessionId", session.SessionId),
                    new XAttribute("SessionName", session.Name),
                    OptionalAttribute("SessionFlowId", session.FlowBinding.GraphId),
                    OptionalAttribute("SessionFlowName", session.BoundFlowDisplayName),
                    new XAttribute("GeneratedAtUtc", DateTime.UtcNow.ToString("O", CultureInfo.InvariantCulture)),
                    new XElement(
                        "Blocks",
                        blocks.Select(block => new XElement(
                            "Block",
                            new XAttribute("Id", block.BlockId),
                            new XAttribute("Index", block.BlockIndex),
                            new XAttribute("TurnId", block.TurnId),
                            new XAttribute("TurnNumber", block.TurnNumber),
                            new XAttribute("AuthorKind", block.AuthorKind),
                            OptionalAttribute("AuthorAgentId", block.AuthorAgentId),
                            OptionalAttribute("AuthorName", block.AuthorName),
                            OptionalAttribute("CounterpartAgentId", block.CounterpartAgentId),
                            OptionalAttribute("CounterpartAgentName", block.CounterpartAgentName),
                            new XElement("Content", block.Content))))));

            var tempPath = $"{documentPath}.tmp";
            await using (var stream = new FileStream(tempPath, FileMode.Create, FileAccess.Write, FileShare.None))
            await using (var writer = new StreamWriter(stream, Utf8NoBom))
            {
                await writer.WriteAsync(document.ToString(SaveOptions.DisableFormatting).AsMemory(), cancellationToken)
                    .ConfigureAwait(false);
            }

            if (File.Exists(documentPath))
            {
                File.Replace(tempPath, documentPath, null);
            }
            else
            {
                File.Move(tempPath, documentPath);
            }
        }

        private static Dictionary<string, object> CreateSegmentMetadata(MemoryBlock block)
        {
            var metadata = new Dictionary<string, object>(StringComparer.Ordinal)
            {
                ["memoryKind"] = "ChatSessionBlock",
                ["memoryBlockId"] = block.BlockId,
                ["memoryBlockIndex"] = block.BlockIndex,
                ["memoryTurnId"] = block.TurnId,
                ["memoryTurnNumber"] = block.TurnNumber,
                ["memoryContent"] = block.Content,
                ["sourceContent"] = block.Content,
                ["memoryAuthorKind"] = block.AuthorKind,
                ["memoryAuthorName"] = block.AuthorName,
                ["memorySessionId"] = block.SessionId,
                ["memorySessionName"] = block.SessionName,
                ["memorySessionFlowId"] = block.SessionFlowId,
                ["memorySessionFlowName"] = block.SessionFlowName,
                ["memoryDocumentName"] = block.DocumentFileName,
                ["memoryDocumentPath"] = block.DocumentPath,
                ["memoryCreatedAtUtc"] = block.CreatedAtUtc.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture)
            };

            AddIfPresent(metadata, "memoryAuthorAgentId", block.AuthorAgentId);
            AddIfPresent(metadata, "memoryCounterpartAgentId", block.CounterpartAgentId);
            AddIfPresent(metadata, "memoryCounterpartAgentName", block.CounterpartAgentName);
            return metadata;
        }

        private bool TryResolveSelectedEmbeddingModel(
            out EmbeddingModelDefinition? model,
            out string errorMessage)
        {
            model = null;
            errorMessage = string.Empty;

            try
            {
                var ragConfiguration = _ragConfigurationRepository.Load();
                if (string.IsNullOrWhiteSpace(ragConfiguration.SelectedEmbeddingModelKey))
                {
                    errorMessage = "No embedding model is selected.";
                    return false;
                }

                model = _embeddingModelRepository.Load().FirstOrDefault(item =>
                    string.Equals(item.Key, ragConfiguration.SelectedEmbeddingModelKey, StringComparison.Ordinal));
                if (model == null)
                {
                    errorMessage = "The selected embedding model was not found.";
                    return false;
                }

                if (!model.IsFullyConfigured)
                {
                    errorMessage = $"Embedding model '{model.DisplayName}' is not fully configured.";
                    model = null;
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }

        private static async Task<AerialDatabase> OpenMemoryDatabaseAsync(
            AerialCityEngine engine,
            MemoryLayout layout,
            CancellationToken cancellationToken)
        {
            var createDatabase = engine.CreateDatabase();
            return await createDatabase(
                new DatabaseOptions
                {
                    Name = VectorsDatabaseName,
                    Storage = new StorageOptions
                    {
                        BasePath = layout.RootPath,
                        EnableWal = false
                    }
                },
                cancellationToken).ConfigureAwait(false);
        }

        private static bool IsResultAllowed(
            Segment segment,
            ContextManagementConfiguration configuration,
            ChatSessionModel currentSession,
            MemoryAgentIdentity? currentAgent)
        {
            return configuration.MemoryShareScope switch
            {
                MemoryShareScope.Application => true,
                MemoryShareScope.SessionFlow => IsSameSessionFlow(segment, currentSession),
                MemoryShareScope.Agent => currentAgent != null && IsSameAgentMemory(segment, currentAgent),
                _ => false
            };
        }

        private static bool IsSameSessionFlow(Segment segment, ChatSessionModel currentSession)
        {
            var resultFlowId = ReadMetadata(segment.Metadata, "memorySessionFlowId");
            var currentFlowId = Normalize(currentSession.FlowBinding.GraphId);
            if (resultFlowId.Length > 0 && currentFlowId.Length > 0)
            {
                return string.Equals(resultFlowId, currentFlowId, StringComparison.OrdinalIgnoreCase);
            }

            var resultFlowName = ReadMetadata(segment.Metadata, "memorySessionFlowName");
            var currentFlowName = Normalize(currentSession.BoundFlowDisplayName);
            return resultFlowName.Length > 0 &&
                   currentFlowName.Length > 0 &&
                   string.Equals(resultFlowName, currentFlowName, StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsSameAgentMemory(Segment segment, MemoryAgentIdentity currentAgent)
        {
            var authorKind = ReadMetadata(segment.Metadata, "memoryAuthorKind");
            if (string.Equals(authorKind, "User", StringComparison.OrdinalIgnoreCase))
            {
                return AgentMatches(
                    ReadMetadata(segment.Metadata, "memoryCounterpartAgentId"),
                    ReadMetadata(segment.Metadata, "memoryCounterpartAgentName"),
                    currentAgent);
            }

            return AgentMatches(
                ReadMetadata(segment.Metadata, "memoryAuthorAgentId"),
                ReadMetadata(segment.Metadata, "memoryAuthorName"),
                currentAgent);
        }

        private static bool AgentMatches(
            string storedAgentId,
            string storedAgentName,
            MemoryAgentIdentity currentAgent)
        {
            return storedAgentId.Length > 0 &&
                   currentAgent.AgentId.Length > 0 &&
                   string.Equals(storedAgentId, currentAgent.AgentId, StringComparison.OrdinalIgnoreCase) ||
                   storedAgentName.Length > 0 &&
                   currentAgent.AgentName.Length > 0 &&
                   string.Equals(storedAgentName, currentAgent.AgentName, StringComparison.OrdinalIgnoreCase);
        }

        private MemoryAgentIdentity? TryResolveNextTriggeredAgent(
            ChatSessionModel session,
            string userText,
            IReadOnlyList<LanguageModelChatContentBlock>? userContentBlocks)
        {
            try
            {
                var compilation = _flowBindingService.CompileBinding(session.FlowBinding);
                if (!compilation.IsSuccess || compilation.Graph == null)
                {
                    return null;
                }

                return TryResolveNextTriggeredAgent(
                    compilation.Graph,
                    SessionFlowPayload.FromNaturalLanguage(userText?.Trim() ?? string.Empty, userContentBlocks));
            }
            catch
            {
                return null;
            }
        }

        private MemoryAgentIdentity? TryResolveNextTriggeredAgent(
            SessionFlowCompiledGraph graph,
            SessionFlowPayload initialPayload)
        {
            var runtimeStates = graph.NodesById.Values.ToDictionary(
                node => node.Node.Id,
                node =>
                {
                    var state = new MemoryFlowNodeState();
                    foreach (var connection in node.IncomingConnections)
                    {
                        state.IncomingConnections[connection.Id] = new MemoryFlowConnectionState();
                    }

                    return state;
                },
                StringComparer.OrdinalIgnoreCase);

            var readyQueue = new Queue<SessionFlowCompiledNode>();
            foreach (var node in graph.NodesById.Values
                         .Where(node => node.IncomingConnections.Count == 0)
                         .OrderBy(node => node.Node.Kind == SessionFlowNodeKind.UserInput ? 0 : 1)
                         .ThenBy(node => node.Node.Title, StringComparer.OrdinalIgnoreCase))
            {
                readyQueue.Enqueue(node);
            }

            while (readyQueue.Count > 0)
            {
                var compiledNode = readyQueue.Dequeue();
                var nodeState = runtimeStates[compiledNode.Node.Id];
                if (nodeState.IsProcessed)
                {
                    continue;
                }

                var deliveredBindings = GetDeliveredBindings(compiledNode, nodeState);
                if (compiledNode.Node.Kind == SessionFlowNodeKind.Agent && deliveredBindings.Count > 0)
                {
                    var agentId = Normalize(compiledNode.Node.AgentId);
                    var agentName = FirstNonEmpty(compiledNode.Node.AgentDisplayName, compiledNode.Node.Title, agentId);
                    return agentId.Length == 0 && agentName.Length == 0
                        ? null
                        : new MemoryAgentIdentity(agentId, agentName);
                }

                var outcome = TryEvaluateRoutingNode(compiledNode, deliveredBindings, initialPayload);
                nodeState.IsProcessed = true;
                if (outcome == null)
                {
                    continue;
                }

                ResolveOutgoingConnections(graph, compiledNode, outcome, runtimeStates, readyQueue);
            }

            return null;
        }

        private MemoryFlowOutcome? TryEvaluateRoutingNode(
            SessionFlowCompiledNode compiledNode,
            IReadOnlyList<MemoryDeliveredBinding> deliveredBindings,
            SessionFlowPayload initialPayload)
        {
            if (compiledNode.Node.Kind != SessionFlowNodeKind.UserInput && deliveredBindings.Count == 0)
            {
                return new MemoryFlowOutcome(IsSkipped: true, NodePayload: null, ExplicitOutputPayloads: new Dictionary<string, MemoryRoutedPayload>(StringComparer.OrdinalIgnoreCase));
            }

            return compiledNode.Node.Kind switch
            {
                SessionFlowNodeKind.UserInput => new MemoryFlowOutcome(false, initialPayload, new Dictionary<string, MemoryRoutedPayload>(StringComparer.OrdinalIgnoreCase)),
                SessionFlowNodeKind.LogicAnd => TryEvaluateBooleanLogic(compiledNode, deliveredBindings, static (left, right) => left && right),
                SessionFlowNodeKind.LogicOr => TryEvaluateBooleanLogic(compiledNode, deliveredBindings, static (left, right) => left || right),
                SessionFlowNodeKind.LogicXor => TryEvaluateBooleanLogic(compiledNode, deliveredBindings, static (left, right) => left ^ right),
                SessionFlowNodeKind.LogicNot => TryEvaluateBooleanNot(compiledNode, deliveredBindings),
                SessionFlowNodeKind.LogicExecution => TryEvaluateLogicExecution(compiledNode, deliveredBindings, onlyNextBranch: false),
                SessionFlowNodeKind.NextLogicExecution => TryEvaluateLogicExecution(compiledNode, deliveredBindings, onlyNextBranch: true),
                _ => null
            };
        }

        private MemoryFlowOutcome? TryEvaluateBooleanLogic(
            SessionFlowCompiledNode compiledNode,
            IReadOnlyList<MemoryDeliveredBinding> deliveredBindings,
            Func<bool, bool, bool> operation)
        {
            if (!TryGetBoolean(compiledNode, deliveredBindings, "in-a", out var left) ||
                !TryGetBoolean(compiledNode, deliveredBindings, "in-b", out var right))
            {
                return null;
            }

            return CreateBooleanOutcome("out-result", operation(left, right));
        }

        private MemoryFlowOutcome? TryEvaluateBooleanNot(
            SessionFlowCompiledNode compiledNode,
            IReadOnlyList<MemoryDeliveredBinding> deliveredBindings)
        {
            return TryGetBoolean(compiledNode, deliveredBindings, "in-a", out var value)
                ? CreateBooleanOutcome("out-result", !value)
                : null;
        }

        private MemoryFlowOutcome? TryEvaluateLogicExecution(
            SessionFlowCompiledNode compiledNode,
            IReadOnlyList<MemoryDeliveredBinding> deliveredBindings,
            bool onlyNextBranch)
        {
            if (!TryGetBoolean(compiledNode, deliveredBindings, "condition", out var condition) || !condition)
            {
                return null;
            }

            var explicitOutputs = new Dictionary<string, MemoryRoutedPayload>(StringComparer.OrdinalIgnoreCase);
            var nonConditionBindings = deliveredBindings
                .Where(binding => !binding.Port.IsBooleanCondition)
                .OrderBy(binding => compiledNode.Node.InputPorts.IndexOf(binding.Port))
                .ToArray();

            foreach (var binding in nonConditionBindings)
            {
                if (string.IsNullOrWhiteSpace(binding.Port.PairKey))
                {
                    continue;
                }

                var outputPort = compiledNode.Node.OutputPorts.FirstOrDefault(port =>
                    string.Equals(port.PairKey, binding.Port.PairKey, StringComparison.OrdinalIgnoreCase));
                if (outputPort == null)
                {
                    continue;
                }

                explicitOutputs[outputPort.Id] = new MemoryRoutedPayload(binding.Payload, binding.IsAlreadyPresented);
                if (onlyNextBranch)
                {
                    break;
                }
            }

            return explicitOutputs.Count == 0
                ? null
                : new MemoryFlowOutcome(false, null, explicitOutputs);
        }

        private MemoryFlowOutcome CreateBooleanOutcome(string outputPortId, bool value)
        {
            return new MemoryFlowOutcome(
                false,
                null,
                new Dictionary<string, MemoryRoutedPayload>(StringComparer.OrdinalIgnoreCase)
                {
                    [outputPortId] = new MemoryRoutedPayload(
                        SessionFlowPortPayload.FromXmlElement(new XElement("Boolean", value ? "true" : "false")),
                        false)
                });
        }

        private bool TryGetBoolean(
            SessionFlowCompiledNode compiledNode,
            IReadOnlyList<MemoryDeliveredBinding> deliveredBindings,
            string portId,
            out bool value)
        {
            value = false;
            var binding = deliveredBindings.FirstOrDefault(item =>
                string.Equals(item.Port.Id, portId, StringComparison.OrdinalIgnoreCase));
            return binding != null && _payloadRouter.TryNormalizeBoolean(binding.Payload, out value);
        }

        private void ResolveOutgoingConnections(
            SessionFlowCompiledGraph graph,
            SessionFlowCompiledNode compiledNode,
            MemoryFlowOutcome outcome,
            IReadOnlyDictionary<string, MemoryFlowNodeState> runtimeStates,
            Queue<SessionFlowCompiledNode> readyQueue)
        {
            foreach (var connection in compiledNode.OutgoingConnections)
            {
                if (!compiledNode.TryGetOutputPort(connection.SourcePortId, out var sourcePort) || sourcePort == null)
                {
                    continue;
                }

                var payload = TryResolveOutgoingPayload(sourcePort, outcome);
                var targetState = runtimeStates[connection.TargetNodeId];
                targetState.IncomingConnections[connection.Id].IsResolved = true;
                targetState.IncomingConnections[connection.Id].Payload = payload;

                if (graph.TryGetNode(connection.TargetNodeId, out var targetNode) &&
                    targetNode != null &&
                    targetState.IncomingConnections.Values.All(item => item.IsResolved) &&
                    !targetState.IsProcessed)
                {
                    readyQueue.Enqueue(targetNode);
                }
            }
        }

        private MemoryRoutedPayload? TryResolveOutgoingPayload(
            SessionFlowPortModel sourcePort,
            MemoryFlowOutcome outcome)
        {
            if (outcome.IsSkipped)
            {
                return null;
            }

            if (sourcePort.IsTransparentOutput)
            {
                outcome.ExplicitOutputPayloads.TryGetValue(sourcePort.Id, out var explicitPayload);
                return explicitPayload;
            }

            if (outcome.NodePayload == null)
            {
                return null;
            }

            return _payloadRouter.TryExtractPortPayload(
                outcome.NodePayload,
                sourcePort,
                out var extractedPayload,
                out _) && extractedPayload != null
                    ? new MemoryRoutedPayload(extractedPayload, false)
                    : null;
        }

        private static IReadOnlyList<MemoryDeliveredBinding> GetDeliveredBindings(
            SessionFlowCompiledNode compiledNode,
            MemoryFlowNodeState runtimeState)
        {
            return compiledNode.IncomingConnections
                .Where(connection =>
                    runtimeState.IncomingConnections.TryGetValue(connection.Id, out var resolution) &&
                    resolution.Payload != null &&
                    compiledNode.TryGetInputPort(connection.TargetPortId, out var port) &&
                    port != null)
                .Select(connection => new MemoryDeliveredBinding(
                    compiledNode.InputPortsById[connection.TargetPortId],
                    runtimeState.IncomingConnections[connection.Id].Payload!.Payload,
                    runtimeState.IncomingConnections[connection.Id].Payload!.IsAlreadyPresented))
                .ToArray();
        }

        private static LanguageModelChatMessage CreateMemoryBackfillMessage(
            IReadOnlyList<RetrievalResult> results)
        {
            var memoryElement = new XElement(
                "Memory",
                results.Select((result, index) =>
                {
                    var segment = result.Segment;
                    return new XElement(
                        "MemoryBlock",
                        new XAttribute("Rank", index + 1),
                        new XAttribute("Score", result.Score.ToString("R", CultureInfo.InvariantCulture)),
                        OptionalAttribute("SessionName", ReadMetadata(segment.Metadata, "memorySessionName")),
                        OptionalAttribute("SessionFlowName", ReadMetadata(segment.Metadata, "memorySessionFlowName")),
                        OptionalAttribute("AuthorKind", ReadMetadata(segment.Metadata, "memoryAuthorKind")),
                        OptionalAttribute("Author", ReadMetadata(segment.Metadata, "memoryAuthorName")),
                        OptionalAttribute("Counterpart", ReadMetadata(segment.Metadata, "memoryCounterpartAgentName")),
                        new XElement("Content", segment.Content));
                }));
            var root = new XElement(
                "SkyweaverPreservedContent",
                new XComment("Relevant memory from previous conversations between you and the user. This preserved content block is injected by Skyweaver and is only visible to the LLM. The user must never see this block directly."),
                memoryElement);
            var xml = root.ToString(SaveOptions.DisableFormatting);

            return new LanguageModelChatMessage(
                LanguageModelChatRole.User,
                new[] { LanguageModelChatContentBlock.CreateHostPreservedContent(xml) })
            {
                AuthorName = "Skyweaver Memory",
                IsHostInjectedTail = true
            };
        }

        private static string BuildQueryText(
            string userText,
            IReadOnlyList<LanguageModelChatContentBlock>? userContentBlocks)
        {
            var parts = new List<string>();
            var normalizedUserText = Normalize(userText);
            if (normalizedUserText.Length > 0)
            {
                parts.Add(normalizedUserText);
            }

            if (userContentBlocks != null)
            {
                parts.AddRange(userContentBlocks
                    .Where(block => block.Kind == LanguageModelChatContentBlockKind.Text)
                    .Select(block => Normalize(block.Content))
                    .Where(content => content.Length > 0));
            }

            return string.Join(Environment.NewLine + Environment.NewLine, parts).Trim();
        }

        private static string CreateSessionMemoryFileName(ChatSessionModel session)
        {
            var name = Normalize(session.Name);
            if (name.Length == 0)
            {
                name = Normalize(session.SessionId);
            }

            var invalidCharacters = Path.GetInvalidFileNameChars();
            var builder = new StringBuilder(name.Length);
            foreach (var ch in name)
            {
                builder.Append(invalidCharacters.Contains(ch) ? '_' : ch);
            }

            var fileName = builder.ToString().Trim();
            return (fileName.Length == 0 ? "ChatSession" : fileName) + ".XML";
        }

        private static void PrepareReplacementPath(string documentPath)
        {
            if (!File.Exists(documentPath))
            {
                return;
            }

            var directory = Path.GetDirectoryName(documentPath) ?? string.Empty;
            var baseName = Path.GetFileNameWithoutExtension(documentPath);
            var preferredOldPath = Path.Combine(directory, $"{baseName}.old.XML");
            if (File.Exists(preferredOldPath))
            {
                File.Move(preferredOldPath, CreateUniqueOldPath(directory, baseName));
            }

            File.Move(documentPath, preferredOldPath);
        }

        private static string CreateUniqueOldPath(string directory, string baseName)
        {
            for (var index = 1; index < int.MaxValue; index++)
            {
                var candidate = Path.Combine(directory, $"{baseName}.old.{index}.XML");
                if (!File.Exists(candidate))
                {
                    return candidate;
                }
            }

            return Path.Combine(directory, $"{baseName}.old.{Guid.NewGuid():N}.XML");
        }

        private static void AddIfPresent(IDictionary<string, object> metadata, string key, string? value)
        {
            var normalized = Normalize(value);
            if (normalized.Length > 0)
            {
                metadata[key] = normalized;
            }
        }

        private static string ReadMetadata(IReadOnlyDictionary<string, object> metadata, string key)
        {
            if (metadata.TryGetValue(key, out var raw))
            {
                return Normalize(raw?.ToString());
            }

            foreach (var (candidateKey, candidateValue) in metadata)
            {
                if (string.Equals(candidateKey, key, StringComparison.OrdinalIgnoreCase))
                {
                    return Normalize(candidateValue?.ToString());
                }
            }

            return string.Empty;
        }

        private static XAttribute? OptionalAttribute(string name, string? value)
        {
            var normalized = Normalize(value);
            return normalized.Length == 0 ? null : new XAttribute(name, normalized);
        }

        private static string FirstNonEmpty(params string?[] values)
        {
            return values.FirstOrDefault(value => !string.IsNullOrWhiteSpace(value))?.Trim() ?? string.Empty;
        }

        private static string Normalize(string? value)
        {
            return string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();
        }

        private sealed class MemoryLayout
        {
            private const string RootFolderName = "Memory";
            private const string ChatSessionsFolderName = "ChatSessions";

            private MemoryLayout(string rootPath)
            {
                RootPath = Path.GetFullPath(rootPath);
                ChatSessionsPath = Path.Combine(RootPath, ChatSessionsFolderName);
                VectorsDatabasePath = Path.Combine(RootPath, VectorsDatabaseName);
            }

            public string RootPath { get; }

            public string ChatSessionsPath { get; }

            public string VectorsDatabasePath { get; }

            public void EnsureCreated()
            {
                Directory.CreateDirectory(RootPath);
                Directory.CreateDirectory(ChatSessionsPath);
                Directory.CreateDirectory(VectorsDatabasePath);
            }

            public static MemoryLayout CreateDefault()
            {
                return new MemoryLayout(Path.Combine(
                    SkyweaverDirectoryRuntime.Instance.AerialCityDirectoryPath,
                    RootFolderName));
            }
        }

        private sealed class MemoryBlock
        {
            public string BlockId { get; init; } = string.Empty;
            public int BlockIndex { get; init; }
            public string TurnId { get; init; } = string.Empty;
            public int TurnNumber { get; init; }
            public string Content { get; init; } = string.Empty;
            public string AuthorKind { get; init; } = string.Empty;
            public string? AuthorAgentId { get; init; }
            public string AuthorName { get; init; } = string.Empty;
            public string? CounterpartAgentId { get; init; }
            public string? CounterpartAgentName { get; init; }
            public string SessionId { get; init; } = string.Empty;
            public string SessionName { get; init; } = string.Empty;
            public string SessionFlowId { get; init; } = string.Empty;
            public string SessionFlowName { get; init; } = string.Empty;
            public string DocumentFileName { get; init; } = string.Empty;
            public string DocumentPath { get; init; } = string.Empty;
            public DateTime CreatedAtUtc { get; init; }
        }

        private sealed record MemoryAgentIdentity(string AgentId, string AgentName);

        private sealed class MemoryFlowConnectionState
        {
            public bool IsResolved { get; set; }

            public MemoryRoutedPayload? Payload { get; set; }
        }

        private sealed class MemoryFlowNodeState
        {
            public bool IsProcessed { get; set; }

            public Dictionary<string, MemoryFlowConnectionState> IncomingConnections { get; } =
                new(StringComparer.OrdinalIgnoreCase);
        }

        private sealed record MemoryRoutedPayload(SessionFlowPortPayload Payload, bool IsAlreadyPresented);

        private sealed record MemoryDeliveredBinding(
            SessionFlowPortModel Port,
            SessionFlowPortPayload Payload,
            bool IsAlreadyPresented);

        private sealed record MemoryFlowOutcome(
            bool IsSkipped,
            SessionFlowPayload? NodePayload,
            IReadOnlyDictionary<string, MemoryRoutedPayload> ExplicitOutputPayloads);
    }
}
