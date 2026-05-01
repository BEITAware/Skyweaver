using System.Text;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;
using Skyweaver.Controls.AgentConfigurationControl.Models;
using Skyweaver.Controls.AgentConfigurationControl.Services;
using Skyweaver.Controls.ChatSessionControl.Services;
using Skyweaver.Controls.LanguageModelConfigurationControl.Models;
using Skyweaver.Controls.LanguageModelConfigurationControl.Services;
using Skyweaver.Services.SkyweaverTools;
using Skyweaver.Tools;

namespace Skyweaver.Services.AgentLoop
{
    public sealed class AgentLoopService
    {
        private const int MaxConsecutiveRecoverableFailures = 3;
        private const string RequestRepairToolName = "_request";
        private const int StreamingTraceRawContentTailLength = 256;
        private const int MaxAssistantToolTreeRepairCandidates = 128;
        private const int MaxRepairToolCallSummaries = 12;
        private static readonly Regex s_markdownCodeFencePattern = new(
            @"^\s*```(?:xml)?\s*(?<content>[\s\S]*?)\s*```\s*$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex s_terminalClosingTagMissingEndPattern = new(
            @"(?<tag></[A-Za-z_][\w:.-]*\s*)$",
            RegexOptions.Compiled);
        private static readonly Regex s_invalidClosingTagSelfSlashPattern = new(
            @"</(?<name>[A-Za-z_][\w:.-]*)\s*/>",
            RegexOptions.Compiled);
        private static readonly Regex s_closingTagWhitespacePattern = new(
            @"</(?<name>[A-Za-z_][\w:.-]*)\s+>",
            RegexOptions.Compiled);
        private static readonly Regex s_knownToolTagNamePattern = new(
            @"<(?<slash>/?)\s*(?<name>tools|tool)(?=[\s>/])",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex s_bareAmpersandPattern = new(
            @"&(?!#\d+;|#x[0-9a-fA-F]+;|\w+;)",
            RegexOptions.Compiled);
        private static readonly Regex s_missingAttributeEqualsPattern = new(
            @"\b(?<name>ToolName|Name|Value|ParameterName|Key)\s+(?<quote>[""'])",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex s_colonAttributePattern = new(
            @"\b(?<name>ToolName|Name|Value|ParameterName|Key)\s*:\s*(?<value>(""[^""]*""|'[^']*'|[^\s/>]+))",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex s_unquotedAttributeValuePattern = new(
            @"\b(?<name>ToolName|Name|Value|ParameterName|Key)\s*=\s*(?<value>[^\s""'<>/`]+)",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex s_parameterContentPattern = new(
            @"(?<open><Parameter\b[^>]*>)(?<content>.*?)(?<close></Parameter\s*>)",
            RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

        private sealed class InvalidAssistantResponseException : InvalidOperationException
        {
            public InvalidAssistantResponseException(
                string message,
                int attemptNumber,
                string? modelId = null,
                AgentAssistantResponse? assistantResponse = null,
                IReadOnlyList<AgentToolBackfill>? toolBackfills = null,
                AgentLoopFinalOutput? latestMessageOutput = null,
                Exception? innerException = null)
                : base(message, innerException)
            {
                AttemptNumber = attemptNumber;
                ModelId = modelId;
                AssistantResponse = assistantResponse ?? new AgentAssistantResponse(string.Empty, Array.Empty<AgentAssistantResponsePart>());
                ToolBackfills = toolBackfills ?? Array.Empty<AgentToolBackfill>();
                LatestMessageOutput = latestMessageOutput;
            }

            public int AttemptNumber { get; }

            public string? ModelId { get; }

            public AgentAssistantResponse AssistantResponse { get; }

            public IReadOnlyList<AgentToolBackfill> ToolBackfills { get; }

            public AgentLoopFinalOutput? LatestMessageOutput { get; }
        }

        private sealed record ToolExecutionAuthorization(
            bool CanExecute,
            bool HasHostConfirmation,
            string? ErrorMessage);

        private sealed record StreamedResponseResult(
            int AttemptNumber,
            string? ModelId,
            AgentAssistantResponse AssistantResponse,
            IReadOnlyList<AgentToolBackfill> ToolBackfills,
            AgentLoopFinalOutput? FinalOutput,
            AgentLoopFinalOutput? LatestMessageOutput,
            IReadOnlyList<string> NewlyLoadedToolKitKeys);

        private sealed class CreateMessageStreamingTracker
        {
            private const string ToolClosingTag = "</Tool>";
            private readonly AgentLoopOutputKind _outputKind;
            private int _emittedLength;

            public CreateMessageStreamingTracker(AgentLoopOutputKind outputKind)
            {
                _outputKind = outputKind;
            }

            public AgentLoopOutputKind OutputKind => _outputKind;

            public string ExtractDelta(string rawContent)
            {
                var visibleContent = ExtractVisibleCreateMessageContent(rawContent);
                if (visibleContent.Length <= _emittedLength)
                {
                    return string.Empty;
                }

                var delta = visibleContent[_emittedLength..];
                _emittedLength = visibleContent.Length;
                return delta;
            }

            private static string ExtractVisibleCreateMessageContent(string rawContent)
            {
                if (string.IsNullOrEmpty(rawContent))
                {
                    return string.Empty;
                }

                var builder = new StringBuilder();
                var searchIndex = 0;

                while (searchIndex < rawContent.Length)
                {
                    var toolStartIndex = IndexOfToolOpeningTag(rawContent, searchIndex);
                    if (toolStartIndex < 0)
                    {
                        break;
                    }

                    var toolOpenTagEndIndex = rawContent.IndexOf('>', toolStartIndex);
                    if (toolOpenTagEndIndex < 0)
                    {
                        break;
                    }

                    var toolOpenTag = rawContent.Substring(toolStartIndex, toolOpenTagEndIndex - toolStartIndex + 1);
                    var bodyStartIndex = toolOpenTagEndIndex + 1;
                    if (IsSelfClosingToolTag(toolOpenTag))
                    {
                        searchIndex = bodyStartIndex;
                        continue;
                    }

                    var toolCloseStartIndex = rawContent.IndexOf(
                        ToolClosingTag,
                        bodyStartIndex,
                        StringComparison.OrdinalIgnoreCase);
                    var visibleBodyEndIndex = toolCloseStartIndex >= 0
                        ? toolCloseStartIndex
                        : rawContent.Length;

                    if (IsCreateMessage(TryExtractToolName(toolOpenTag) ?? string.Empty) &&
                        visibleBodyEndIndex > bodyStartIndex)
                    {
                        builder.Append(rawContent, bodyStartIndex, visibleBodyEndIndex - bodyStartIndex);
                    }

                    if (toolCloseStartIndex < 0)
                    {
                        break;
                    }

                    searchIndex = toolCloseStartIndex + ToolClosingTag.Length;
                }

                return builder.ToString();
            }

            private static int IndexOfToolOpeningTag(string text, int startIndex)
            {
                var searchIndex = Math.Max(0, startIndex);

                while (searchIndex < text.Length)
                {
                    var matchIndex = text.IndexOf("<Tool", searchIndex, StringComparison.OrdinalIgnoreCase);
                    if (matchIndex < 0)
                    {
                        return -1;
                    }

                    var nameEndIndex = matchIndex + "<Tool".Length;
                    if (nameEndIndex >= text.Length ||
                        char.IsWhiteSpace(text[nameEndIndex]) ||
                        text[nameEndIndex] is '>' or '/')
                    {
                        return matchIndex;
                    }

                    searchIndex = matchIndex + 1;
                }

                return -1;
            }

            private static string? TryExtractToolName(string toolOpenTag)
            {
                if (string.IsNullOrWhiteSpace(toolOpenTag))
                {
                    return null;
                }

                var parseCandidate = IsSelfClosingToolTag(toolOpenTag)
                    ? toolOpenTag
                    : $"{toolOpenTag}</Tool>";

                try
                {
                    var element = XElement.Parse(parseCandidate, LoadOptions.PreserveWhitespace);
                    return element.Attributes()
                        .FirstOrDefault(attribute =>
                            string.Equals(attribute.Name.LocalName, "ToolName", StringComparison.OrdinalIgnoreCase))
                        ?.Value
                        ?.Trim();
                }
                catch
                {
                    return null;
                }
            }

            private static bool IsSelfClosingToolTag(string toolOpenTag)
            {
                return toolOpenTag.TrimEnd().EndsWith("/>", StringComparison.Ordinal);
            }
        }

        private readonly AgentSystemPromptBuilder _systemPromptBuilder;
        private readonly IAgentLanguageModelResolver _languageModelResolver;
        private readonly ILanguageModelChatService _chatService;
        private readonly SkyweaverToolManager _toolManager;
        private readonly AgentLoopContextManager _contextManager;
        private readonly SkyweaverToolKitService _toolKitService;

        public AgentLoopService()
            : this(
                new AgentSystemPromptBuilder(),
                new AgentLanguageModelResolver(),
                new LanguageModelChatService(),
                new SkyweaverToolManager(),
                new AgentLoopContextManager(),
                new SkyweaverToolKitService())
        {
        }

        public AgentLoopService(
            AgentSystemPromptBuilder systemPromptBuilder,
            IAgentLanguageModelResolver languageModelResolver,
            ILanguageModelChatService chatService,
            SkyweaverToolManager toolManager,
            AgentLoopContextManager contextManager,
            SkyweaverToolKitService? toolKitService = null)
        {
            _systemPromptBuilder = systemPromptBuilder ?? throw new ArgumentNullException(nameof(systemPromptBuilder));
            _languageModelResolver = languageModelResolver ?? throw new ArgumentNullException(nameof(languageModelResolver));
            _chatService = chatService ?? throw new ArgumentNullException(nameof(chatService));
            _toolManager = toolManager ?? throw new ArgumentNullException(nameof(toolManager));
            _contextManager = contextManager ?? throw new ArgumentNullException(nameof(contextManager));
            _toolKitService = toolKitService ?? new SkyweaverToolKitService();
        }

        public Task<AgentLoopResult> RunAsync(
            AgentLoopRequest request,
            CancellationToken cancellationToken = default)
        {
            return RunCoreAsync(request, onEventAsync: null, cancellationToken);
        }

        public Task<AgentLoopResult> RunStreamingAsync(
            AgentLoopRequest request,
            Func<AgentLoopRuntimeEvent, CancellationToken, ValueTask> onEventAsync,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(onEventAsync);
            return RunCoreAsync(request, onEventAsync, cancellationToken);
        }

        private async Task<AgentLoopResult> RunCoreAsync(
            AgentLoopRequest request,
            Func<AgentLoopRuntimeEvent, CancellationToken, ValueTask>? onEventAsync,
            CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            ValidateRequest(request);

            var availableToolKits = _toolKitService.Load();
            var toolKitMembershipMap = _toolKitService.BuildToolKitMembershipMap(availableToolKits);
            var activeToolKitKeys = new HashSet<string>(
                ExtractLoadedToolKitKeysFromHistory(request.History),
                StringComparer.OrdinalIgnoreCase);
            var runtimeToolContext = request.ToolContext
                .WithRuntimeAgent(
                    request.Agent,
                    request.ToolConfirmationCallback != null)
                .WithAvailableToolKits(availableToolKits);
            var systemPrompt = _systemPromptBuilder.BuildCompleteSystemPrompt(
                request.Agent,
                supportsHostToolConfirmation: request.ToolConfirmationCallback != null,
                availableToolKits: availableToolKits,
                activeToolKitKeys: activeToolKitKeys);
            var debugRunContext = AgentLoopDebugRecorder.TryCreateRunContext(request);
            AgentLoopDebugRecorder.RecordRunStart(debugRunContext, request, systemPrompt);

            var persistentHistory = (request.History ?? Array.Empty<LanguageModelChatMessage>())
                .Select(message => message.Clone())
                .ToList();
            var turnHistory = new List<LanguageModelChatMessage>();
            var iterations = new List<AgentLoopIteration>();
            string? lastModelId = null;
            AgentLoopFinalOutput? latestMessageOutput = null;
            var consecutiveRecoverableFailures = 0;

            for (var iterationNumber = 1; ; iterationNumber++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var preparedContext = await _contextManager.PrepareAsync(
                    request.Agent,
                    systemPrompt,
                    request.Input,
                    request.InputContentBlocks,
                    persistentHistory,
                    turnHistory,
                    debugRunContext,
                    iterationNumber,
                    cancellationToken).ConfigureAwait(false);

                persistentHistory = preparedContext.PersistentHistory
                    .Select(message => message.Clone())
                    .ToList();
                turnHistory = preparedContext.TurnHistory
                    .Select(message => message.Clone())
                    .ToList();

                var preparedSnapshot = new AgentLoopPreparedRequestDebugSnapshot
                {
                    SystemPrompt = systemPrompt,
                    Input = request.Input,
                    PersistentHistory = preparedContext.PersistentHistory
                        .Select(message => message.Clone())
                        .ToArray(),
                    TurnHistory = preparedContext.TurnHistory
                        .Select(message => message.Clone())
                        .ToArray(),
                    PreparedMessages = preparedContext.PreparedMessages
                        .Select(message => message.Clone())
                        .ToArray(),
                    ContextCompression = preparedContext.ContextCompression
                };

                if (preparedContext.ContextCompression != null)
                {
                    await PublishAsync(
                        onEventAsync,
                        new AgentLoopRuntimeEvent
                        {
                            Kind = AgentLoopRuntimeEventKind.ContextCompressionApplied,
                            IterationNumber = iterationNumber,
                            ContextCompression = preparedContext.ContextCompression
                        },
                        cancellationToken).ConfigureAwait(false);
                }

                StreamedResponseResult response;
                try
                {
                    response = await InvokeModelStreamingAsync(
                        request,
                        preparedSnapshot,
                        runtimeToolContext,
                        toolKitMembershipMap,
                        activeToolKitKeys,
                        debugRunContext,
                        iterationNumber,
                        latestMessageOutput,
                        onEventAsync,
                        cancellationToken).ConfigureAwait(false);
                }
                catch (InvalidAssistantResponseException ex)
                {
                    if (!string.IsNullOrWhiteSpace(ex.ModelId))
                    {
                        lastModelId = ex.ModelId;
                    }

                    if (ex.LatestMessageOutput != null)
                    {
                        latestMessageOutput = ex.LatestMessageOutput;
                    }

                    consecutiveRecoverableFailures++;
                    var repairMessage = BuildRepairMessage(
                        ex.Message,
                        consecutiveRecoverableFailures,
                        ex.ToolBackfills,
                        turnHistory,
                        latestMessageOutput);
                    AppendFailedIterationHistory(ex, repairMessage, turnHistory);

                    await PublishAsync(
                        onEventAsync,
                        new AgentLoopRuntimeEvent
                        {
                            Kind = AgentLoopRuntimeEventKind.RepairMessageGenerated,
                            IterationNumber = iterationNumber,
                            ModelId = lastModelId,
                            RepairMessage = repairMessage
                        },
                        cancellationToken).ConfigureAwait(false);

                    AgentLoopDebugRecorder.RecordIterationOutcome(
                        debugRunContext,
                        request.Agent,
                        iterationNumber,
                        ex.AttemptNumber,
                        lastModelId,
                        ex.AssistantResponse,
                        ex.ToolBackfills,
                        repairMessage,
                        finalOutput: null);

                    iterations.Add(new AgentLoopIteration
                    {
                        IterationNumber = iterationNumber,
                        ModelId = lastModelId,
                        AssistantResponse = ex.AssistantResponse,
                        ToolBackfills = ex.ToolBackfills,
                        RepairMessage = repairMessage,
                        FinalOutput = null,
                        ContextCompression = preparedContext.ContextCompression
                    });

                    await PublishAsync(
                        onEventAsync,
                        new AgentLoopRuntimeEvent
                        {
                            Kind = AgentLoopRuntimeEventKind.IterationCompleted,
                            IterationNumber = iterationNumber,
                            ModelId = lastModelId,
                            FinalOutput = null
                        },
                        cancellationToken).ConfigureAwait(false);

                    if (consecutiveRecoverableFailures >= MaxConsecutiveRecoverableFailures)
                    {
                        return new AgentLoopResult
                        {
                            IsCompleted = false,
                            FailureReason =
                                $"The agent loop stopped after {MaxConsecutiveRecoverableFailures} consecutive failed iterations. Last error: {ex.Message}",
                            LastModelId = lastModelId,
                            Iterations = iterations.ToArray()
                        };
                    }

                    continue;
                }

                consecutiveRecoverableFailures = 0;

                if (!string.IsNullOrWhiteSpace(response.ModelId))
                {
                    lastModelId = response.ModelId;
                }

                if (response.LatestMessageOutput != null)
                {
                    latestMessageOutput = response.LatestMessageOutput;
                }

                foreach (var toolKitKey in response.NewlyLoadedToolKitKeys)
                {
                    activeToolKitKeys.Add(toolKitKey);
                }

                AppendCurrentTurnHistory(
                    response.AssistantResponse,
                    response.ToolBackfills,
                    turnHistory);

                AgentLoopDebugRecorder.RecordIterationOutcome(
                    debugRunContext,
                    request.Agent,
                    iterationNumber,
                    response.AttemptNumber,
                    response.ModelId,
                    response.AssistantResponse,
                    response.ToolBackfills,
                    repairMessage: null,
                    response.FinalOutput);

                iterations.Add(new AgentLoopIteration
                {
                    IterationNumber = iterationNumber,
                    ModelId = response.ModelId,
                    AssistantResponse = response.AssistantResponse,
                    ToolBackfills = response.ToolBackfills,
                    FinalOutput = response.FinalOutput,
                    ContextCompression = preparedContext.ContextCompression
                });

                await PublishAsync(
                    onEventAsync,
                    new AgentLoopRuntimeEvent
                    {
                        Kind = AgentLoopRuntimeEventKind.IterationCompleted,
                        IterationNumber = iterationNumber,
                        ModelId = response.ModelId,
                        FinalOutput = response.FinalOutput
                    },
                    cancellationToken).ConfigureAwait(false);

                if (response.FinalOutput != null)
                {
                    return new AgentLoopResult
                    {
                        IsCompleted = true,
                        LastModelId = lastModelId,
                        FinalOutput = response.FinalOutput,
                        Iterations = iterations.ToArray()
                    };
                }
            }

        }

        private async Task<StreamedResponseResult> InvokeModelStreamingAsync(
            AgentLoopRequest request,
            AgentLoopPreparedRequestDebugSnapshot preparedSnapshot,
            SkyweaverToolContext runtimeToolContext,
            IReadOnlyDictionary<string, IReadOnlyList<string>> toolKitMembershipMap,
            IReadOnlyCollection<string> activeToolKitKeys,
            AgentLoopDebugRunContext? debugRunContext,
            int iterationNumber,
            AgentLoopFinalOutput? latestMessageOutput,
            Func<AgentLoopRuntimeEvent, CancellationToken, ValueTask>? onEventAsync,
            CancellationToken cancellationToken)
        {
            var candidates = _languageModelResolver.GetCandidateModels(request.Agent);
            if (candidates.Count == 0)
            {
                throw new InvalidOperationException(
                    $"Agent '{request.Agent.DisplayNameOrFallback}' has no resolved candidate language model.");
            }

            Exception? lastError = null;
            var failureMessages = new List<string>();
            var attemptNumber = 0;

            foreach (var candidate in candidates)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (!candidate.InterfaceSettings.IsFullyConfigured)
                {
                    failureMessages.Add($"{GetLanguageModelDisplayName(candidate)}: interface settings are incomplete.");
                    continue;
                }

                attemptNumber++;
                AgentLoopDebugRecorder.RecordPreparedRequest(
                    debugRunContext,
                    request,
                    preparedSnapshot,
                    candidates,
                    candidate,
                    iterationNumber,
                    attemptNumber);

                var modelId = candidate.SummaryModelId;
                var hasStartedStreaming = false;
                var rawContentBuilder = new StringBuilder();
                AgentAssistantResponse? parsedAssistantResponse = null;
                var toolInvocationStreamingParser = new SkyweaverToolInvocationStreamingParser(
                    _toolManager.GetRegisteredTools(resolveIcons: false)
                        .Select(registration => registration.Definition));
                IReadOnlyList<SkyweaverStreamingToolCallSnapshot> previousToolCallSnapshots =
                    Array.Empty<SkyweaverStreamingToolCallSnapshot>();
                var createMessageStreamingTracker = new CreateMessageStreamingTracker(
                    request.Agent.IsStructuredXmlIO
                        ? AgentLoopOutputKind.StructuredXml
                        : AgentLoopOutputKind.NaturalLanguage);
                var streamingUpdates = new List<AgentLoopStreamingUpdateDebugSnapshot>();

                try
                {
                    await PublishAsync(
                        onEventAsync,
                        new AgentLoopRuntimeEvent
                        {
                            Kind = AgentLoopRuntimeEventKind.IterationStarted,
                            IterationNumber = iterationNumber,
                            ModelId = modelId
                        },
                        cancellationToken).ConfigureAwait(false);

                    await foreach (var update in _chatService.GetStreamingResponseAsync(
                                       candidate,
                                       preparedSnapshot.PreparedMessages.Select(message => message.Clone()).ToArray(),
                                       cancellationToken).ConfigureAwait(false))
                    {
                        if (!string.IsNullOrWhiteSpace(update.ModelId))
                        {
                            modelId = update.ModelId;
                        }

                        if (!string.IsNullOrEmpty(update.ReasoningTextDelta))
                        {
                            await PublishAsync(
                                onEventAsync,
                                new AgentLoopRuntimeEvent
                                {
                                    Kind = AgentLoopRuntimeEventKind.ReasoningDelta,
                                    IterationNumber = iterationNumber,
                                    ModelId = modelId,
                                    ReasoningDelta = update.ReasoningTextDelta
                                },
                                cancellationToken).ConfigureAwait(false);
                        }

                        var rawContentLengthBeforeAppend = rawContentBuilder.Length;
                        var wasAppendedToRawContent = !string.IsNullOrEmpty(update.TextDelta);
                        if (wasAppendedToRawContent)
                        {
                            hasStartedStreaming = true;
                            rawContentBuilder.Append(update.TextDelta);
                        }

                        streamingUpdates.Add(new AgentLoopStreamingUpdateDebugSnapshot
                        {
                            SequenceNumber = streamingUpdates.Count + 1,
                            ReceivedAtLocal = DateTimeOffset.Now,
                            Update = update,
                            WasAppendedToRawContent = wasAppendedToRawContent,
                            RawContentLengthBeforeAppend = rawContentLengthBeforeAppend,
                            RawContentLengthAfterAppend = rawContentBuilder.Length,
                            RawContentTailAfterAppend = GetStringBuilderTail(rawContentBuilder, StreamingTraceRawContentTailLength)
                        });

                        if (!wasAppendedToRawContent)
                        {
                            continue;
                        }

                        var currentRawContent = rawContentBuilder.ToString();
                        var currentToolCallSnapshots = toolInvocationStreamingParser.Parse(currentRawContent);
                        await PublishStreamingToolCallUpdatesAsync(
                            onEventAsync,
                            currentToolCallSnapshots,
                            previousToolCallSnapshots,
                            iterationNumber,
                            modelId,
                            cancellationToken).ConfigureAwait(false);
                        previousToolCallSnapshots = currentToolCallSnapshots;

                        var streamedDelta = createMessageStreamingTracker.ExtractDelta(currentRawContent);
                        if (!string.IsNullOrEmpty(streamedDelta))
                        {
                            await PublishAsync(
                                onEventAsync,
                                new AgentLoopRuntimeEvent
                                {
                                    Kind = AgentLoopRuntimeEventKind.TextDelta,
                                    IterationNumber = iterationNumber,
                                    ModelId = modelId,
                                    TextDelta = streamedDelta,
                                    TextDeltaOutputKind = createMessageStreamingTracker.OutputKind
                                },
                                cancellationToken).ConfigureAwait(false);
                        }
                    }

                    var rawContent = rawContentBuilder.ToString();
                    if (!TryParseAssistantToolTree(
                            rawContent,
                            out var normalizedToolsXml,
                            out var assistantResponse,
                            out var parseError))
                    {
                        await PublishAsync(
                            onEventAsync,
                            new AgentLoopRuntimeEvent
                            {
                                Kind = AgentLoopRuntimeEventKind.MalformedToolCall,
                                IterationNumber = iterationNumber,
                                ModelId = modelId,
                                ToolXml = rawContent,
                                ErrorMessage = parseError
                            },
                            cancellationToken).ConfigureAwait(false);

                        throw new InvalidAssistantResponseException(
                            parseError,
                            attemptNumber,
                            modelId,
                            CreateRejectedAssistantResponse(rawContent, parseError));
                    }

                    parsedAssistantResponse = assistantResponse;

                    await PublishAsync(
                        onEventAsync,
                        new AgentLoopRuntimeEvent
                        {
                            Kind = AgentLoopRuntimeEventKind.AssistantToolTreeReceived,
                            IterationNumber = iterationNumber,
                            ModelId = modelId,
                            ToolXml = normalizedToolsXml
                        },
                        cancellationToken).ConfigureAwait(false);

                    var executionResult = await ExecuteAssistantToolTreeAsync(
                        request,
                        assistantResponse.GetToolCalls(),
                        runtimeToolContext,
                        toolKitMembershipMap,
                        activeToolKitKeys,
                        iterationNumber,
                        modelId,
                        latestMessageOutput,
                        onEventAsync,
                        cancellationToken).ConfigureAwait(false);

                    AgentLoopDebugRecorder.RecordStreamingTrace(
                        debugRunContext,
                        request.Agent,
                        candidate,
                        iterationNumber,
                        attemptNumber,
                        modelId,
                        streamingUpdates,
                        rawContent,
                        hasStartedStreaming,
                        completedNormally: true,
                        terminalMessage: null);

                    return new StreamedResponseResult(
                        attemptNumber,
                        modelId,
                        assistantResponse,
                        executionResult.ToolBackfills,
                        executionResult.FinalOutput,
                        executionResult.LatestMessageOutput,
                        executionResult.NewlyLoadedToolKitKeys);
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (InvalidAssistantResponseException ex)
                {
                    AgentLoopDebugRecorder.RecordStreamingTrace(
                        debugRunContext,
                        request.Agent,
                        candidate,
                        iterationNumber,
                        attemptNumber,
                        modelId,
                        streamingUpdates,
                        rawContentBuilder.ToString(),
                        hasStartedStreaming,
                        completedNormally: false,
                        terminalMessage: ex.Message);

                    AgentLoopDebugRecorder.RecordMainRequestFailure(
                        debugRunContext,
                        request.Agent,
                        candidate,
                        iterationNumber,
                        attemptNumber,
                        modelId,
                        rawContentBuilder.ToString(),
                        hasStartedStreaming,
                        ex);
                    throw;
                }
                catch (Exception ex)
                {
                    AgentLoopDebugRecorder.RecordStreamingTrace(
                        debugRunContext,
                        request.Agent,
                        candidate,
                        iterationNumber,
                        attemptNumber,
                        modelId,
                        streamingUpdates,
                        rawContentBuilder.ToString(),
                        hasStartedStreaming,
                        completedNormally: false,
                        terminalMessage: ex.Message);

                    AgentLoopDebugRecorder.RecordMainRequestFailure(
                        debugRunContext,
                        request.Agent,
                        candidate,
                        iterationNumber,
                        attemptNumber,
                        modelId,
                        rawContentBuilder.ToString(),
                        hasStartedStreaming,
                        ex);

                    if (hasStartedStreaming)
                    {
                        throw new InvalidAssistantResponseException(
                            $"Streaming response from model '{GetLanguageModelDisplayName(candidate)}' failed after partial output: {ex.Message}",
                            attemptNumber,
                            modelId,
                            parsedAssistantResponse ?? CreateRejectedAssistantResponse(rawContentBuilder.ToString(), ex.Message),
                            innerException: ex);
                    }

                    lastError = ex;
                    failureMessages.Add($"{GetLanguageModelDisplayName(candidate)}: {ex.Message}");
                }
            }

            var failureText = failureMessages.Count == 0
                ? "No callable candidate language model is available."
                : $"Tried candidates in order: {string.Join("; ", failureMessages)}";

            throw new InvalidOperationException(
                $"Agent '{request.Agent.DisplayNameOrFallback}' could not start a streaming response. {failureText}",
                lastError);
        }

        private static string GetStringBuilderTail(StringBuilder builder, int maxLength)
        {
            ArgumentNullException.ThrowIfNull(builder);

            if (builder.Length == 0 || maxLength <= 0)
            {
                return string.Empty;
            }

            var length = Math.Min(maxLength, builder.Length);
            return builder.ToString(builder.Length - length, length);
        }

        private async Task<(IReadOnlyList<AgentToolBackfill> ToolBackfills, AgentLoopFinalOutput? FinalOutput, AgentLoopFinalOutput? LatestMessageOutput, IReadOnlyList<string> NewlyLoadedToolKitKeys)>
            ExecuteAssistantToolTreeAsync(
                AgentLoopRequest request,
                IReadOnlyList<SkyweaverToolInvocation> invocations,
                SkyweaverToolContext runtimeToolContext,
                IReadOnlyDictionary<string, IReadOnlyList<string>> toolKitMembershipMap,
                IReadOnlyCollection<string> activeToolKitKeys,
                int iterationNumber,
                string? modelId,
                AgentLoopFinalOutput? latestMessageOutput,
                Func<AgentLoopRuntimeEvent, CancellationToken, ValueTask>? onEventAsync,
                CancellationToken cancellationToken)
        {
            var toolBackfills = new List<AgentToolBackfill>();
            var finishInvocations = new List<(int ToolCallIndex, SkyweaverToolInvocation Invocation)>();
            var newlyLoadedToolKitKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var currentLatestMessageOutput = latestMessageOutput;
            const int partIndex = 0;

            for (var invocationIndex = 0; invocationIndex < invocations.Count; invocationIndex++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var invocation = invocations[invocationIndex];
                var toolCallIndex = invocationIndex + 1;

                if (IsFinishTask(invocation.ToolName))
                {
                    finishInvocations.Add((toolCallIndex, invocation));
                    continue;
                }

                AgentToolBackfill backfill;
                AgentLoopFinalOutput? createdMessage = null;

                if (IsCreateMessage(invocation.ToolName))
                {
                    backfill = ExecuteCreateMessageInvocation(
                        request.Agent,
                        invocation,
                        partIndex,
                        toolCallIndex,
                        out createdMessage);
                }
                else
                {
                    var toolReturns = await ExecuteAuthorizedInvocationsAsync(
                        request,
                        [invocation],
                        runtimeToolContext,
                        toolKitMembershipMap,
                        activeToolKitKeys,
                        iterationNumber,
                        partIndex,
                        cancellationToken).ConfigureAwait(false);

                    if (IsLoadToolKits(invocation.ToolName))
                    {
                        foreach (var toolKitKey in ExtractLoadedToolKitKeys(toolReturns))
                        {
                            newlyLoadedToolKitKeys.Add(toolKitKey);
                        }
                    }

                    backfill = CreateBackfill(partIndex, toolCallIndex, toolReturns);
                }

                toolBackfills.Add(backfill);

                await PublishAsync(
                    onEventAsync,
                    new AgentLoopRuntimeEvent
                    {
                        Kind = AgentLoopRuntimeEventKind.ToolOutputReceived,
                        IterationNumber = iterationNumber,
                        ModelId = modelId,
                        PartIndex = partIndex,
                        ToolCallIndex = toolCallIndex,
                        ToolInvocation = invocation,
                        ToolOutputXml = backfill.ToolsReturnXml,
                        ToolReturns = backfill.ToolReturns
                    },
                    cancellationToken).ConfigureAwait(false);

                if (createdMessage != null)
                {
                    currentLatestMessageOutput = createdMessage;
                    await PublishAsync(
                        onEventAsync,
                        new AgentLoopRuntimeEvent
                        {
                            Kind = AgentLoopRuntimeEventKind.MessageCreated,
                            IterationNumber = iterationNumber,
                            ModelId = modelId,
                            PartIndex = partIndex,
                            ToolCallIndex = toolCallIndex,
                            ToolInvocation = invocation,
                            MessageOutput = createdMessage
                        },
                        cancellationToken).ConfigureAwait(false);
                }
            }

            if (finishInvocations.Count == 0)
            {
                return (toolBackfills, null, currentLatestMessageOutput, newlyLoadedToolKitKeys.ToArray());
            }

            if (finishInvocations.Count > 1)
            {
                foreach (var finishInvocation in finishInvocations)
                {
                    var errorBackfill = CreateBackfill(
                        partIndex,
                        finishInvocation.ToolCallIndex,
                        [_toolManager.CreateErrorToolReturnPayload(
                            FinishTaskTool.ToolName,
                            "FinishTask may only be called once in a single assistant response.")]);
                    toolBackfills.Add(errorBackfill);

                    await PublishAsync(
                        onEventAsync,
                        new AgentLoopRuntimeEvent
                        {
                            Kind = AgentLoopRuntimeEventKind.ToolOutputReceived,
                            IterationNumber = iterationNumber,
                            ModelId = modelId,
                            PartIndex = partIndex,
                            ToolCallIndex = finishInvocation.ToolCallIndex,
                            ToolInvocation = finishInvocation.Invocation,
                            ToolOutputXml = errorBackfill.ToolsReturnXml,
                            ToolReturns = errorBackfill.ToolReturns
                        },
                        cancellationToken).ConfigureAwait(false);
                }

                return (toolBackfills, null, currentLatestMessageOutput, newlyLoadedToolKitKeys.ToArray());
            }

            var finishInvocationCandidate = finishInvocations[0];
            if (!TryValidateFinishTaskInvocation(finishInvocationCandidate.Invocation, out var finishError))
            {
                var errorBackfill = CreateBackfill(
                    partIndex,
                    finishInvocationCandidate.ToolCallIndex,
                    [_toolManager.CreateErrorToolReturnPayload(
                        FinishTaskTool.ToolName,
                        finishError)]);
                toolBackfills.Add(errorBackfill);

                await PublishAsync(
                    onEventAsync,
                    new AgentLoopRuntimeEvent
                    {
                        Kind = AgentLoopRuntimeEventKind.ToolOutputReceived,
                        IterationNumber = iterationNumber,
                        ModelId = modelId,
                        PartIndex = partIndex,
                        ToolCallIndex = finishInvocationCandidate.ToolCallIndex,
                        ToolInvocation = finishInvocationCandidate.Invocation,
                        ToolOutputXml = errorBackfill.ToolsReturnXml,
                        ToolReturns = errorBackfill.ToolReturns
                    },
                    cancellationToken).ConfigureAwait(false);

                return (toolBackfills, null, currentLatestMessageOutput, newlyLoadedToolKitKeys.ToArray());
            }

            if (currentLatestMessageOutput == null)
            {
                var errorBackfill = CreateBackfill(
                    partIndex,
                    finishInvocationCandidate.ToolCallIndex,
                    [_toolManager.CreateErrorToolReturnPayload(
                        FinishTaskTool.ToolName,
                        $"FinishTask requires a successful {CreateMessageTool.ToolName} before the turn can close.")]);
                toolBackfills.Add(errorBackfill);

                await PublishAsync(
                    onEventAsync,
                    new AgentLoopRuntimeEvent
                    {
                        Kind = AgentLoopRuntimeEventKind.ToolOutputReceived,
                        IterationNumber = iterationNumber,
                        ModelId = modelId,
                        PartIndex = partIndex,
                        ToolCallIndex = finishInvocationCandidate.ToolCallIndex,
                        ToolInvocation = finishInvocationCandidate.Invocation,
                        ToolOutputXml = errorBackfill.ToolsReturnXml,
                        ToolReturns = errorBackfill.ToolReturns
                    },
                    cancellationToken).ConfigureAwait(false);

                return (toolBackfills, null, currentLatestMessageOutput, newlyLoadedToolKitKeys.ToArray());
            }

            var successBackfill = CreateBackfill(
                partIndex,
                finishInvocationCandidate.ToolCallIndex,
                [_toolManager.CreateToolReturnPayload(
                    FinishTaskTool.ToolName,
                    SkyweaverToolResult.Success("FinishTask accepted."))]);
            toolBackfills.Add(successBackfill);

            await PublishAsync(
                onEventAsync,
                new AgentLoopRuntimeEvent
                {
                    Kind = AgentLoopRuntimeEventKind.ToolOutputReceived,
                    IterationNumber = iterationNumber,
                    ModelId = modelId,
                    PartIndex = partIndex,
                    ToolCallIndex = finishInvocationCandidate.ToolCallIndex,
                    ToolInvocation = finishInvocationCandidate.Invocation,
                    ToolOutputXml = successBackfill.ToolsReturnXml,
                    ToolReturns = successBackfill.ToolReturns
                },
                cancellationToken).ConfigureAwait(false);

            await PublishAsync(
                onEventAsync,
                new AgentLoopRuntimeEvent
                {
                    Kind = AgentLoopRuntimeEventKind.FinalOutputProduced,
                    IterationNumber = iterationNumber,
                    ModelId = modelId,
                    PartIndex = partIndex,
                    ToolCallIndex = finishInvocationCandidate.ToolCallIndex,
                    FinalOutput = currentLatestMessageOutput
                },
                cancellationToken).ConfigureAwait(false);

            return (toolBackfills, currentLatestMessageOutput, currentLatestMessageOutput, newlyLoadedToolKitKeys.ToArray());
        }

        private static async Task PublishStreamingToolCallUpdatesAsync(
            Func<AgentLoopRuntimeEvent, CancellationToken, ValueTask>? onEventAsync,
            IReadOnlyList<SkyweaverStreamingToolCallSnapshot> currentSnapshots,
            IReadOnlyList<SkyweaverStreamingToolCallSnapshot> previousSnapshots,
            int iterationNumber,
            string? modelId,
            CancellationToken cancellationToken)
        {
            if (onEventAsync == null || currentSnapshots.Count == 0)
            {
                return;
            }

            var previousByToolCallIndex = previousSnapshots.ToDictionary(
                snapshot => snapshot.ToolCallIndex,
                snapshot => snapshot);

            foreach (var snapshot in currentSnapshots.OrderBy(item => item.ToolCallIndex))
            {
                var eventKind = previousByToolCallIndex.TryGetValue(snapshot.ToolCallIndex, out var previousSnapshot)
                    ? AreEquivalent(previousSnapshot, snapshot)
                        ? (AgentLoopRuntimeEventKind?)null
                        : AgentLoopRuntimeEventKind.ToolCallUpdated
                    : AgentLoopRuntimeEventKind.ToolCallStarted;

                if (eventKind == null)
                {
                    continue;
                }

                await PublishAsync(
                    onEventAsync,
                    new AgentLoopRuntimeEvent
                    {
                        Kind = eventKind.Value,
                        IterationNumber = iterationNumber,
                        ModelId = modelId,
                        PartIndex = 0,
                        ToolCallIndex = snapshot.ToolCallIndex,
                        ToolCallSnapshot = snapshot,
                        ToolXml = snapshot.ToolXmlFragment
                    },
                    cancellationToken).ConfigureAwait(false);
            }
        }

        private static bool AreEquivalent(
            SkyweaverStreamingToolCallSnapshot left,
            SkyweaverStreamingToolCallSnapshot right)
        {
            return left.ToolCallIndex == right.ToolCallIndex &&
                   string.Equals(left.ToolName, right.ToolName, StringComparison.OrdinalIgnoreCase) &&
                   left.IsInvocationClosed == right.IsInvocationClosed &&
                   string.Equals(left.ToolXmlFragment, right.ToolXmlFragment, StringComparison.Ordinal);
        }

        private AgentToolBackfill ExecuteCreateMessageInvocation(
            AgentDefinition agent,
            SkyweaverToolInvocation invocation,
            int partIndex,
            int toolCallIndex,
            out AgentLoopFinalOutput? messageOutput)
        {
            messageOutput = null;
            if (!TryBuildCreateMessageOutput(agent, invocation, out var createdOutput, out var errorMessage))
            {
                return CreateBackfill(
                    partIndex,
                    toolCallIndex,
                    [_toolManager.CreateErrorToolReturnPayload(
                        CreateMessageTool.ToolName,
                        errorMessage)]);
            }

            messageOutput = createdOutput;
            return CreateBackfill(
                partIndex,
                toolCallIndex,
                [_toolManager.CreateToolReturnPayload(
                    CreateMessageTool.ToolName,
                    SkyweaverToolResult.Success("CreateMessage accepted."))]);
        }

        private static async ValueTask PublishAsync(
            Func<AgentLoopRuntimeEvent, CancellationToken, ValueTask>? onEventAsync,
            AgentLoopRuntimeEvent update,
            CancellationToken cancellationToken)
        {
            if (onEventAsync == null)
            {
                return;
            }

            await onEventAsync(update, cancellationToken).ConfigureAwait(false);
        }

        private static void ValidateRequest(AgentLoopRequest request)
        {
            ArgumentNullException.ThrowIfNull(request.Agent);

            if (request.Agent.IsStructuredXmlIO &&
                !TryParseXmlWithRoot(
                    request.Input,
                    AgentDefinition.InputRootName,
                    out _,
                    out var inputError))
            {
                throw new InvalidOperationException(
                    $"Structured agent input must be a complete <{AgentDefinition.InputRootName}> XML document. {inputError}");
            }
        }

        private static bool TryParseAssistantToolTree(
            string rawContent,
            out string normalizedToolsXml,
            out AgentAssistantResponse assistantResponse,
            out string errorMessage)
        {
            normalizedToolsXml = string.Empty;
            assistantResponse = new AgentAssistantResponse(string.Empty, Array.Empty<AgentAssistantResponsePart>());
            errorMessage = string.Empty;

            var trimmed = rawContent?.Trim() ?? string.Empty;
            if (trimmed.Length == 0)
            {
                errorMessage = "Assistant response is empty. Every assistant response must be exactly one <Tools> XML tree.";
                return false;
            }

            if (TryParseAssistantToolTreeExact(
                    trimmed,
                    out normalizedToolsXml,
                    out assistantResponse,
                    out errorMessage))
            {
                return true;
            }

            var originalErrorMessage = errorMessage;
            if (TryRepairAssistantToolTree(trimmed, out var repairedToolsXml) &&
                TryParseAssistantToolTreeExact(
                    repairedToolsXml,
                    out normalizedToolsXml,
                    out assistantResponse,
                    out _))
            {
                return true;
            }

            if (TryWrapBareMessageAsCreateMessageToolTree(trimmed, out var wrappedMessageToolsXml) &&
                TryParseAssistantToolTreeExact(
                    wrappedMessageToolsXml,
                    out normalizedToolsXml,
                    out assistantResponse,
                    out _))
            {
                return true;
            }

            errorMessage = originalErrorMessage;
            return false;
        }

        private static bool TryParseAssistantToolTreeExact(
            string rawContent,
            out string normalizedToolsXml,
            out AgentAssistantResponse assistantResponse,
            out string errorMessage)
        {
            normalizedToolsXml = string.Empty;
            assistantResponse = new AgentAssistantResponse(string.Empty, Array.Empty<AgentAssistantResponsePart>());
            errorMessage = string.Empty;

            XDocument document;
            try
            {
                document = XDocument.Parse(rawContent, LoadOptions.PreserveWhitespace);
            }
            catch (Exception ex) when (ex is XmlException or InvalidOperationException)
            {
                errorMessage = $"Assistant response must be exactly one <Tools> XML tree with no surrounding text. {ex.Message}";
                return false;
            }

            var root = document.Root;
            if (root == null)
            {
                errorMessage = "Assistant response is missing a root element. Use a single <Tools> root.";
                return false;
            }

            if (!string.Equals(root.Name.LocalName, "Tools", StringComparison.Ordinal))
            {
                errorMessage = $"Assistant response must use <Tools> as the root element, not <{root.Name.LocalName}>.";
                return false;
            }

            if (root.Nodes().Any(node =>
                    node is XElement element &&
                    !string.Equals(element.Name.LocalName, "Tool", StringComparison.Ordinal)))
            {
                errorMessage = "Only <Tool> elements are allowed directly under <Tools>.";
                return false;
            }

            if (root.Nodes().Any(node =>
                    node is XText text &&
                    !string.IsNullOrWhiteSpace(text.Value)))
            {
                errorMessage = "Non-whitespace text is not allowed directly under <Tools>.";
                return false;
            }

            var toolElements = root.Elements()
                .Where(element => string.Equals(element.Name.LocalName, "Tool", StringComparison.Ordinal))
                .ToArray();
            if (toolElements.Length == 0)
            {
                errorMessage = "Assistant response must contain at least one <Tool> inside <Tools>.";
                return false;
            }

            normalizedToolsXml = document.ToString(SaveOptions.DisableFormatting);

            IReadOnlyList<SkyweaverToolInvocation> invocations;
            try
            {
                invocations = new SkyweaverToolManager().ParseToolsInvocationXml(normalizedToolsXml);
            }
            catch (Exception ex)
            {
                errorMessage = $"Assistant response tools could not be parsed: {ex.Message}";
                return false;
            }

            if (invocations.Count == 0)
            {
                errorMessage = "Assistant response must contain at least one <Tool> inside <Tools>.";
                return false;
            }

            assistantResponse = new AgentAssistantResponse(
                normalizedToolsXml,
                [AgentAssistantResponsePart.CreateToolCall(
                    normalizedToolsXml,
                    invocations,
                    toolCallIndex: 1)]);
            return true;
        }

        private static bool TryRepairAssistantToolTree(string rawContent, out string repairedToolsXml)
        {
            repairedToolsXml = string.Empty;

            var original = rawContent?.Trim() ?? string.Empty;
            if (original.Length == 0)
            {
                return false;
            }

            var queue = new Queue<string>();
            var seen = new HashSet<string>(StringComparer.Ordinal);

            void Enqueue(string? candidate)
            {
                if (seen.Count >= MaxAssistantToolTreeRepairCandidates ||
                    string.IsNullOrWhiteSpace(candidate))
                {
                    return;
                }

                var trimmedCandidate = candidate.Trim();
                if (trimmedCandidate.Length > 0 && seen.Add(trimmedCandidate))
                {
                    queue.Enqueue(trimmedCandidate);
                }
            }

            Enqueue(original);
            while (queue.Count > 0)
            {
                var candidate = queue.Dequeue();
                if (!string.Equals(candidate, original, StringComparison.Ordinal) &&
                    TryParseAssistantToolTreeExact(
                        candidate,
                        out var normalizedCandidate,
                        out _,
                        out _))
                {
                    repairedToolsXml = normalizedCandidate;
                    return true;
                }

                foreach (var transformedCandidate in EnumerateAssistantToolTreeRepairTransforms(candidate))
                {
                    Enqueue(transformedCandidate);
                }
            }

            return false;
        }

        private static IEnumerable<string?> EnumerateAssistantToolTreeRepairTransforms(string text)
        {
            yield return StripMarkdownCodeFence(text);
            yield return MaybeDecodeHtmlEncodedXml(text);
            yield return NormalizeCommonXmlSyntax(text);
            yield return FixTerminalMissingTagEnd(text);
            yield return NormalizeInvalidClosingTagSyntax(text);
            yield return NormalizeKnownToolTagCasing(text);
            yield return NormalizeCommonToolAttributeTypos(text);
            yield return EscapeBareAmpersands(text);
            yield return ProtectParameterTextContent(text);
            yield return ExtractSingleToolsFragment(text);
            yield return WrapStandaloneToolElements(text);
            yield return AppendMissingToolsClosingTag(text);
            yield return PrependMissingToolsOpeningTag(text);
            yield return ReplaceTerminalMistypedToolsClosingTag(text);
        }

        private static bool TryWrapBareMessageAsCreateMessageToolTree(string rawContent, out string toolsXml)
        {
            toolsXml = string.Empty;

            var normalized = rawContent?.Trim() ?? string.Empty;
            if (normalized.Length == 0 ||
                StartsWithElement(normalized, "Tools") ||
                StartsWithElement(normalized, "Tool"))
            {
                return false;
            }

            toolsXml = CreateMessageToolTreeXml(normalized);
            return toolsXml.Length > 0;
        }

        private static string CreateAssistantHistoryContent(string content)
        {
            var normalized = content?.Trim() ?? string.Empty;
            if (normalized.Length == 0)
            {
                return string.Empty;
            }

            if (StartsWithElement(normalized, "Tools"))
            {
                return normalized;
            }

            if (StartsWithElement(normalized, "Tool"))
            {
                return $"<Tools>{normalized}</Tools>";
            }

            return CreateMessageToolTreeXml(normalized);
        }

        private static string CreateMessageToolTreeXml(string content)
        {
            var normalized = content?.Trim() ?? string.Empty;
            if (normalized.Length == 0)
            {
                return string.Empty;
            }

            var document = new XDocument(
                new XElement(
                    "Tools",
                    new XElement(
                        "Tool",
                        new XAttribute("ToolName", CreateMessageTool.ToolName),
                        normalized)));
            return document.ToString(SaveOptions.DisableFormatting);
        }

        private static string StripMarkdownCodeFence(string? text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return string.Empty;
            }

            var match = s_markdownCodeFencePattern.Match(text);
            return match.Success
                ? match.Groups["content"].Value
                : text;
        }

        private static string MaybeDecodeHtmlEncodedXml(string? text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return string.Empty;
            }

            if (text.Contains('<'))
            {
                return text;
            }

            if (!text.Contains("&lt;", StringComparison.OrdinalIgnoreCase) &&
                !text.Contains("&#60;", StringComparison.OrdinalIgnoreCase) &&
                !text.Contains("&#x3c;", StringComparison.OrdinalIgnoreCase))
            {
                return text;
            }

            var decoded = WebUtility.HtmlDecode(text);
            return decoded.Contains('<') ? decoded : text;
        }

        private static string NormalizeCommonXmlSyntax(string? text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return string.Empty;
            }

            var builder = new StringBuilder(text.Length);
            foreach (var character in text)
            {
                switch (character)
                {
                    case '\uFEFF':
                    case '\u200B':
                    case '\u200C':
                    case '\u200D':
                    case '\u2060':
                        continue;
                    case '\u00A0':
                    case '\u3000':
                        builder.Append(' ');
                        break;
                    case '\uFF1C':
                        builder.Append('<');
                        break;
                    case '\uFF1E':
                        builder.Append('>');
                        break;
                    case '\uFF1D':
                        builder.Append('=');
                        break;
                    case '\uFF0F':
                        builder.Append('/');
                        break;
                    case '\u201C':
                    case '\u201D':
                    case '\u2033':
                    case '\u00AB':
                    case '\u00BB':
                    case '\u300C':
                    case '\u300D':
                    case '\u300E':
                    case '\u300F':
                    case '\uFF02':
                        builder.Append('"');
                        break;
                    case '\u2018':
                    case '\u2019':
                    case '\u2032':
                    case '\uFF07':
                        builder.Append('\'');
                        break;
                    default:
                        if (char.IsControl(character) && character is not '\r' and not '\n' and not '\t')
                        {
                            continue;
                        }

                        builder.Append(character);
                        break;
                }
            }

            return builder.ToString();
        }

        private static string FixTerminalMissingTagEnd(string? text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return string.Empty;
            }

            var trimmed = text.TrimEnd();
            return trimmed.EndsWith(">", StringComparison.Ordinal)
                ? text
                : s_terminalClosingTagMissingEndPattern.Replace(trimmed, "${tag}>");
        }

        private static string NormalizeInvalidClosingTagSyntax(string? text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return string.Empty;
            }

            var normalized = s_invalidClosingTagSelfSlashPattern.Replace(text, "</${name}>");
            return s_closingTagWhitespacePattern.Replace(normalized, "</${name}>");
        }

        private static string NormalizeKnownToolTagCasing(string? text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return string.Empty;
            }

            return s_knownToolTagNamePattern.Replace(
                text,
                match =>
                {
                    var slash = match.Groups["slash"].Value;
                    var canonicalName = string.Equals(
                        match.Groups["name"].Value,
                        "Tools",
                        StringComparison.OrdinalIgnoreCase)
                            ? "Tools"
                            : "Tool";
                    return $"<{slash}{canonicalName}";
                });
        }

        private static string NormalizeCommonToolAttributeTypos(string? text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return string.Empty;
            }

            var normalized = s_missingAttributeEqualsPattern.Replace(text, "${name}=${quote}");
            normalized = s_colonAttributePattern.Replace(normalized, "${name}=${value}");
            normalized = s_unquotedAttributeValuePattern.Replace(
                normalized,
                match => $"{match.Groups["name"].Value}=\"{match.Groups["value"].Value}\"");
            return normalized;
        }

        private static string EscapeBareAmpersands(string? text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return string.Empty;
            }

            return s_bareAmpersandPattern.Replace(text, "&amp;");
        }

        private static string ProtectParameterTextContent(string? xml)
        {
            if (string.IsNullOrWhiteSpace(xml))
            {
                return string.Empty;
            }

            return s_parameterContentPattern.Replace(
                xml,
                match =>
                {
                    var content = match.Groups["content"].Value;
                    if (string.IsNullOrWhiteSpace(content) || LooksLikeXmlFragment(content))
                    {
                        return match.Value;
                    }

                    return $"{match.Groups["open"].Value}{EscapeXmlTextContent(content)}{match.Groups["close"].Value}";
                });
        }

        private static string EscapeXmlTextContent(string text)
        {
            return EscapeBareAmpersands(text)
                .Replace("<", "&lt;", StringComparison.Ordinal)
                .Replace(">", "&gt;", StringComparison.Ordinal);
        }

        private static bool LooksLikeXmlFragment(string? text)
        {
            if (string.IsNullOrWhiteSpace(text) || !text.Contains('<'))
            {
                return false;
            }

            try
            {
                _ = XDocument.Parse($"<Root>{EscapeBareAmpersands(text)}</Root>", LoadOptions.PreserveWhitespace);
                return true;
            }
            catch (Exception ex) when (ex is XmlException or InvalidOperationException)
            {
                return false;
            }
        }

        private static string? ExtractSingleToolsFragment(string? text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return null;
            }

            var trimmed = text.Trim();
            var toolsStartIndexes = EnumerateElementStartIndexes(trimmed, "Tools").Take(2).ToArray();
            if (toolsStartIndexes.Length != 1)
            {
                return null;
            }

            var toolsStartIndex = toolsStartIndexes[0];
            var toolsEndIndex = IndexOfClosingElementEnd(trimmed, "Tools", toolsStartIndex);
            return toolsEndIndex > toolsStartIndex
                ? trimmed[toolsStartIndex..toolsEndIndex]
                : trimmed[toolsStartIndex..];
        }

        private static string? WrapStandaloneToolElements(string? text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return null;
            }

            var trimmed = text.Trim();
            if (StartsWithElement(trimmed, "Tools"))
            {
                return null;
            }

            return TryCollectOnlyStandaloneToolElements(trimmed, out var toolElements)
                ? $"<Tools>{toolElements}</Tools>"
                : null;
        }

        private static string? AppendMissingToolsClosingTag(string? text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return null;
            }

            var trimmed = text.Trim();
            return StartsWithElement(trimmed, "Tools") &&
                   IndexOfClosingElementEnd(trimmed, "Tools") < 0
                ? $"{trimmed}</Tools>"
                : null;
        }

        private static string? PrependMissingToolsOpeningTag(string? text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return null;
            }

            var trimmed = text.Trim();
            if (IndexOfElementStart(trimmed, "Tools") >= 0)
            {
                return null;
            }

            var toolsCloseStartIndex = LastIndexOfClosingTagStart(trimmed, "Tools");
            if (toolsCloseStartIndex < 0 ||
                FindTagEnd(trimmed, toolsCloseStartIndex) != trimmed.Length)
            {
                return null;
            }

            var beforeClosingRoot = trimmed[..toolsCloseStartIndex].Trim();
            return TryCollectOnlyStandaloneToolElements(beforeClosingRoot, out var toolElements)
                ? $"<Tools>{toolElements}</Tools>"
                : null;
        }

        private static string? ReplaceTerminalMistypedToolsClosingTag(string? text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return null;
            }

            var trimmed = text.Trim();
            if (!StartsWithElement(trimmed, "Tools") ||
                IndexOfClosingElementEnd(trimmed, "Tools") >= 0)
            {
                return null;
            }

            var terminalToolCloseStartIndex = LastIndexOfClosingTagStart(trimmed, "Tool");
            if (terminalToolCloseStartIndex < 0 ||
                FindTagEnd(trimmed, terminalToolCloseStartIndex) != trimmed.Length)
            {
                return null;
            }

            return $"{trimmed[..terminalToolCloseStartIndex]}</Tools>";
        }

        private static bool TryCollectOnlyStandaloneToolElements(string text, out string toolElements)
        {
            toolElements = string.Empty;
            if (string.IsNullOrWhiteSpace(text))
            {
                return false;
            }

            var builder = new StringBuilder();
            var index = 0;
            var foundAny = false;

            while (index < text.Length)
            {
                index = SkipWhitespace(text, index);
                if (index >= text.Length)
                {
                    break;
                }

                if (IndexOfElementStart(text, "Tool", index) != index)
                {
                    toolElements = string.Empty;
                    return false;
                }

                var toolEndIndex = IndexOfElementEnd(text, "Tool", index);
                if (toolEndIndex < 0)
                {
                    toolElements = string.Empty;
                    return false;
                }

                builder.Append(text[index..toolEndIndex]);
                index = toolEndIndex;
                foundAny = true;
            }

            toolElements = builder.ToString();
            return foundAny;
        }

        private static IEnumerable<int> EnumerateElementStartIndexes(string text, string elementName)
        {
            var searchIndex = 0;
            while (searchIndex < text.Length)
            {
                var elementStartIndex = IndexOfElementStart(text, elementName, searchIndex);
                if (elementStartIndex < 0)
                {
                    yield break;
                }

                yield return elementStartIndex;
                searchIndex = elementStartIndex + 1;
            }
        }

        private static bool StartsWithElement(string text, string elementName)
        {
            return IndexOfElementStart(text, elementName) == 0;
        }

        private static int IndexOfElementStart(string text, string elementName, int startIndex = 0)
        {
            var needle = $"<{elementName}";
            var searchIndex = Math.Max(0, startIndex);

            while (searchIndex < text.Length)
            {
                var matchIndex = text.IndexOf(needle, searchIndex, StringComparison.OrdinalIgnoreCase);
                if (matchIndex < 0)
                {
                    return -1;
                }

                var nameEndIndex = matchIndex + needle.Length;
                if (nameEndIndex >= text.Length || IsXmlNameBoundary(text[nameEndIndex]))
                {
                    return matchIndex;
                }

                searchIndex = matchIndex + 1;
            }

            return -1;
        }

        private static int IndexOfClosingTagStart(string text, string elementName, int startIndex = 0)
        {
            var needle = $"</{elementName}";
            var searchIndex = Math.Max(0, startIndex);

            while (searchIndex < text.Length)
            {
                var matchIndex = text.IndexOf(needle, searchIndex, StringComparison.OrdinalIgnoreCase);
                if (matchIndex < 0)
                {
                    return -1;
                }

                var nameEndIndex = matchIndex + needle.Length;
                if (nameEndIndex >= text.Length || IsXmlNameBoundary(text[nameEndIndex]))
                {
                    return matchIndex;
                }

                searchIndex = matchIndex + 1;
            }

            return -1;
        }

        private static int LastIndexOfClosingTagStart(string text, string elementName)
        {
            var lastMatchIndex = -1;
            var searchIndex = 0;

            while (searchIndex < text.Length)
            {
                var matchIndex = IndexOfClosingTagStart(text, elementName, searchIndex);
                if (matchIndex < 0)
                {
                    return lastMatchIndex;
                }

                lastMatchIndex = matchIndex;
                searchIndex = matchIndex + 1;
            }

            return lastMatchIndex;
        }

        private static int IndexOfElementEnd(string text, string elementName, int startIndex)
        {
            if (IndexOfElementStart(text, elementName, startIndex) != startIndex)
            {
                return -1;
            }

            var openingTagEndIndex = FindTagEnd(text, startIndex);
            if (openingTagEndIndex < 0)
            {
                return -1;
            }

            var openingTag = text[startIndex..openingTagEndIndex];
            if (openingTag.TrimEnd().EndsWith("/>", StringComparison.Ordinal))
            {
                return openingTagEndIndex;
            }

            return IndexOfClosingElementEnd(text, elementName, openingTagEndIndex);
        }

        private static int IndexOfClosingElementEnd(string text, string elementName, int startIndex = 0)
        {
            var closingTagStartIndex = IndexOfClosingTagStart(text, elementName, startIndex);
            return closingTagStartIndex < 0
                ? -1
                : FindTagEnd(text, closingTagStartIndex);
        }

        private static int FindTagEnd(string text, int tagStartIndex)
        {
            return tagStartIndex < 0
                ? -1
                : text.IndexOf('>', tagStartIndex) is var tagEndIndex && tagEndIndex >= 0
                    ? tagEndIndex + 1
                    : -1;
        }

        private static int SkipWhitespace(string text, int startIndex)
        {
            var index = Math.Max(0, startIndex);
            while (index < text.Length && char.IsWhiteSpace(text[index]))
            {
                index++;
            }

            return index;
        }

        private static bool IsXmlNameBoundary(char character)
        {
            return char.IsWhiteSpace(character) || character is '>' or '/' or '?';
        }

        private static bool IsCreateMessage(string toolName)
        {
            return string.Equals(toolName, CreateMessageTool.ToolName, StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsFinishTask(string toolName)
        {
            return string.Equals(toolName, FinishTaskTool.ToolName, StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsLoadToolKits(string toolName)
        {
            return string.Equals(toolName, LoadToolKitsTool.ToolName, StringComparison.OrdinalIgnoreCase);
        }

        private async Task<IReadOnlyList<SkyweaverToolReturnPayload>> ExecuteAuthorizedInvocationsAsync(
            AgentLoopRequest request,
            IReadOnlyList<SkyweaverToolInvocation> invocations,
            SkyweaverToolContext runtimeToolContext,
            IReadOnlyDictionary<string, IReadOnlyList<string>> toolKitMembershipMap,
            IReadOnlyCollection<string> activeToolKitKeys,
            int iterationNumber,
            int partIndex,
            CancellationToken cancellationToken)
        {
            var toolReturns = new List<SkyweaverToolReturnPayload>(invocations.Count);

            foreach (var invocation in invocations)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var authorization = await AuthorizeToolInvocationAsync(
                    request,
                    invocation,
                    runtimeToolContext,
                    toolKitMembershipMap,
                    activeToolKitKeys,
                    iterationNumber,
                    partIndex,
                    cancellationToken).ConfigureAwait(false);

                if (!authorization.CanExecute)
                {
                    toolReturns.Add(_toolManager.CreateErrorToolReturnPayload(
                        invocation.ToolName,
                        authorization.ErrorMessage ?? $"Tool '{invocation.ToolName}' cannot be executed."));
                    continue;
                }

                try
                {
                    var result = await _toolManager.ExecuteAsync(
                        invocation.ToolName,
                        invocation.RawArguments,
                        runtimeToolContext,
                        request.Agent,
                        authorization.HasHostConfirmation,
                        cancellationToken).ConfigureAwait(false);
                    toolReturns.Add(_toolManager.CreateToolReturnPayload(invocation.ToolName, result));
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    toolReturns.Add(_toolManager.CreateErrorToolReturnPayload(
                        invocation.ToolName,
                        $"Tool execution failed: {ex.Message}"));
                }
            }

            return toolReturns;
        }

        private async Task<ToolExecutionAuthorization> AuthorizeToolInvocationAsync(
            AgentLoopRequest request,
            SkyweaverToolInvocation invocation,
            SkyweaverToolContext runtimeToolContext,
            IReadOnlyDictionary<string, IReadOnlyList<string>> toolKitMembershipMap,
            IReadOnlyCollection<string> activeToolKitKeys,
            int iterationNumber,
            int partIndex,
            CancellationToken cancellationToken)
        {
            var registration = _toolManager.GetRegisteredTools(resolveIcons: false).FirstOrDefault(item =>
                string.Equals(item.Definition.Name, invocation.ToolName, StringComparison.OrdinalIgnoreCase));

            if (registration == null)
            {
                return new ToolExecutionAuthorization(
                    false,
                    false,
                    $"Tool not found: {invocation.ToolName}");
            }

            if (!registration.IsEnabled)
            {
                return new ToolExecutionAuthorization(
                    false,
                    false,
                    $"Tool '{invocation.ToolName}' is currently disabled.");
            }

            if (registration.Definition.CanBelongToToolKit &&
                toolKitMembershipMap.TryGetValue(registration.Definition.Name, out var membershipKeys) &&
                membershipKeys.Count > 0 &&
                !membershipKeys.Any(activeToolKitKeys.Contains))
            {
                var toolKitNames = runtimeToolContext.AvailableToolKits
                    .Where(toolKit => membershipKeys.Contains(toolKit.Key, StringComparer.OrdinalIgnoreCase))
                    .Select(toolKit => string.IsNullOrWhiteSpace(toolKit.Name) ? toolKit.DisplayNameOrFallback : toolKit.Name)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .OrderBy(name => name, StringComparer.OrdinalIgnoreCase)
                    .ToArray();
                var toolKitHint = toolKitNames.Length == 0
                    ? $"Load the containing toolkit first with {LoadToolKitsTool.ToolName}."
                    : $"Load one of the containing toolkits first with {LoadToolKitsTool.ToolName}: {string.Join(", ", toolKitNames)}.";

                return new ToolExecutionAuthorization(
                    false,
                    false,
                    $"Tool '{invocation.ToolName}' is gated behind a toolkit and is not currently loaded. {toolKitHint}");
            }

            var permissionDecision = AgentToolPermissionEvaluator.Resolve(request.Agent, registration);
            if (permissionDecision == AgentToolEffectiveDecision.Allowed)
            {
                return new ToolExecutionAuthorization(true, false, null);
            }

            if (permissionDecision == AgentToolEffectiveDecision.Denied)
            {
                return new ToolExecutionAuthorization(
                    false,
                    false,
                    $"Tool '{invocation.ToolName}' is not available to agent '{request.Agent.AgentId}'.");
            }

            return await RequestToolConfirmationAsync(
                request,
                invocation,
                permissionDecision,
                iterationNumber,
                partIndex,
                cancellationToken).ConfigureAwait(false);
        }

        private async Task<ToolExecutionAuthorization> RequestToolConfirmationAsync(
            AgentLoopRequest request,
            SkyweaverToolInvocation invocation,
            AgentToolEffectiveDecision permissionDecision,
            int iterationNumber,
            int partIndex,
            CancellationToken cancellationToken)
        {
            if (request.ToolConfirmationCallback == null)
            {
                return new ToolExecutionAuthorization(
                    false,
                    false,
                    $"Tool '{invocation.ToolName}' requires host confirmation before execution, but the current runtime does not provide a confirmation callback.");
            }

            try
            {
                var confirmationResult = await request.ToolConfirmationCallback(
                    new AgentToolConfirmationRequest
                    {
                        Agent = request.Agent,
                        Invocation = invocation,
                        PermissionDecision = permissionDecision,
                        IterationNumber = iterationNumber,
                        PartIndex = partIndex
                    },
                    cancellationToken).ConfigureAwait(false);

                if (confirmationResult?.IsApproved == true)
                {
                    return new ToolExecutionAuthorization(true, true, null);
                }

                var rejectionMessage = confirmationResult == null || string.IsNullOrWhiteSpace(confirmationResult.RejectionMessage)
                    ? $"Tool '{invocation.ToolName}' was not approved by the host."
                    : confirmationResult.RejectionMessage.Trim();

                return new ToolExecutionAuthorization(false, false, rejectionMessage);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                return new ToolExecutionAuthorization(
                    false,
                    false,
                    $"Host confirmation failed for tool '{invocation.ToolName}': {ex.Message}");
            }
        }

        private static bool TryBuildCreateMessageOutput(
            AgentDefinition agent,
            SkyweaverToolInvocation invocation,
            out AgentLoopFinalOutput output,
            out string errorMessage)
        {
            output = null!;
            errorMessage = string.Empty;

            if (!TryExtractInvocationPayload(invocation, requirePayload: true, out var payload, out errorMessage))
            {
                return false;
            }

            if (agent.IsStructuredXmlIO)
            {
                if (!TryParseXmlWithRoot(
                        payload,
                        AgentDefinition.OutputRootName,
                        out var normalizedXml,
                        out var xmlError))
                {
                    errorMessage = $"{CreateMessageTool.ToolName} for this agent must contain one complete <{AgentDefinition.OutputRootName}> XML document. {xmlError}";
                    return false;
                }

                output = new AgentLoopFinalOutput
                {
                    Content = normalizedXml,
                    Kind = AgentLoopOutputKind.StructuredXml,
                    Source = AgentLoopFinalOutputSource.AssistantText
                };
                return true;
            }

            if (TryParseXmlWithRoot(
                    payload,
                    AgentDefinition.OutputRootName,
                    out _,
                    out _))
            {
                errorMessage = $"{CreateMessageTool.ToolName} for this agent must contain plain text, not a standalone <{AgentDefinition.OutputRootName}> XML block.";
                return false;
            }

            output = new AgentLoopFinalOutput
            {
                Content = payload,
                Kind = AgentLoopOutputKind.NaturalLanguage,
                Source = AgentLoopFinalOutputSource.AssistantText
            };
            return true;
        }

        private static bool TryValidateFinishTaskInvocation(
            SkyweaverToolInvocation invocation,
            out string errorMessage)
        {
            errorMessage = string.Empty;

            if (!TryExtractInvocationPayload(invocation, requirePayload: false, out var payload, out errorMessage))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(payload))
            {
                return true;
            }

            if (TryParseBooleanValue(payload, out var booleanValue))
            {
                if (booleanValue)
                {
                    return true;
                }

                errorMessage = "FinishTask cannot be false. Omit it until the turn is ready to close.";
                return false;
            }

            if (invocation.RawArguments.Count == 1 &&
                invocation.RawArguments.Values.FirstOrDefault() is string legacyValue &&
                TryParseBooleanValue(legacyValue, out var legacyBooleanValue))
            {
                if (legacyBooleanValue)
                {
                    return true;
                }

                errorMessage = "FinishTask cannot be false. Omit it until the turn is ready to close.";
                return false;
            }

            errorMessage = $"FinishTask does not accept a reply payload. Use {CreateMessageTool.ToolName} for the reply, then call FinishTask with an empty body.";
            return false;
        }

        private static bool TryExtractInvocationPayload(
            SkyweaverToolInvocation invocation,
            bool requirePayload,
            out string payload,
            out string errorMessage)
        {
            payload = string.Empty;
            errorMessage = string.Empty;

            XElement toolElement;
            try
            {
                toolElement = XElement.Parse(invocation.InvocationXml, LoadOptions.PreserveWhitespace);
            }
            catch (XmlException ex)
            {
                errorMessage = $"Tool invocation XML could not be parsed: {ex.Message}";
                return false;
            }

            payload = toolElement.HasElements
                ? string.Concat(toolElement.Nodes().Select(node => node.ToString(SaveOptions.DisableFormatting))).Trim()
                : (toolElement.Value?.Trim() ?? string.Empty);

            if (payload.Length == 0 && invocation.RawArguments.Count == 1)
            {
                payload = invocation.RawArguments.Values.FirstOrDefault()?.Trim() ?? string.Empty;
            }

            if (requirePayload && payload.Length == 0)
            {
                errorMessage = $"Tool '{invocation.ToolName}' requires a payload inside the <Tool> element.";
                return false;
            }

            return true;
        }

        private AgentToolBackfill CreateBackfill(
            int partIndex,
            int toolCallIndex,
            IReadOnlyList<SkyweaverToolReturnPayload> toolReturns)
        {
            return new AgentToolBackfill
            {
                PartIndex = partIndex,
                ToolCallIndex = toolCallIndex,
                ToolReturns = toolReturns,
                ToolsReturnXml = _toolManager.BuildToolsReturnXml(toolReturns)
            };
        }

        private static IReadOnlyList<string> ExtractLoadedToolKitKeys(
            IReadOnlyList<SkyweaverToolReturnPayload> toolReturns)
        {
            if (toolReturns.Count == 0)
            {
                return Array.Empty<string>();
            }

            return toolReturns
                .Where(toolReturn =>
                    toolReturn.IsSuccess &&
                    string.Equals(toolReturn.ToolName, LoadToolKitsTool.ToolName, StringComparison.OrdinalIgnoreCase))
                .SelectMany(toolReturn => ExtractStringValues(toolReturn.Result.Data, "loadedToolKitKeys"))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(item => item, StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        private static IReadOnlyList<string> ExtractLoadedToolKitKeysFromHistory(
            IReadOnlyList<LanguageModelChatMessage>? history)
        {
            if (history == null || history.Count == 0)
            {
                return Array.Empty<string>();
            }

            return history
                .SelectMany(message => ExtractLoadedToolKitKeysFromToolsReturnXml(message.Content))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(item => item, StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        private static IReadOnlyList<string> ExtractLoadedToolKitKeysFromToolsReturnXml(string? xml)
        {
            if (string.IsNullOrWhiteSpace(xml))
            {
                return Array.Empty<string>();
            }

            XDocument document;
            try
            {
                document = XDocument.Parse(xml, LoadOptions.PreserveWhitespace);
            }
            catch (XmlException)
            {
                return Array.Empty<string>();
            }

            var root = document.Root;
            if (root == null || !string.Equals(root.Name.LocalName, "ToolsReturn", StringComparison.OrdinalIgnoreCase))
            {
                return Array.Empty<string>();
            }

            return root.Elements()
                .Where(element => string.Equals(element.Name.LocalName, "ToolReturn", StringComparison.OrdinalIgnoreCase))
                .Where(IsSuccessfulLoadToolKitsReturn)
                .SelectMany(ExtractLoadedToolKitKeysFromToolReturnElement)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(item => item, StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        private static bool IsSuccessfulLoadToolKitsReturn(XElement toolReturn)
        {
            var toolName = toolReturn.Attributes()
                .FirstOrDefault(attribute =>
                    string.Equals(attribute.Name.LocalName, "ToolName", StringComparison.OrdinalIgnoreCase))
                ?.Value
                ?.Trim();

            return string.Equals(toolName, LoadToolKitsTool.ToolName, StringComparison.OrdinalIgnoreCase) &&
                   !toolReturn.Elements().Any(element =>
                       string.Equals(element.Name.LocalName, "ErrorMessage", StringComparison.OrdinalIgnoreCase));
        }

        private static IReadOnlyList<string> ExtractLoadedToolKitKeysFromToolReturnElement(XElement toolReturn)
        {
            const string keyPrefix = "loadedToolKitKeys=";

            return toolReturn.Elements()
                .Where(element => element.Name.LocalName.StartsWith("StringReturn", StringComparison.OrdinalIgnoreCase))
                .Select(element => element.Value?.Trim() ?? string.Empty)
                .Where(value => value.StartsWith(keyPrefix, StringComparison.OrdinalIgnoreCase))
                .SelectMany(value => ParseStringArrayValue(value[keyPrefix.Length..]))
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .Select(value => value.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(item => item, StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        private static IReadOnlyList<string> ParseStringArrayValue(string? rawValue)
        {
            if (string.IsNullOrWhiteSpace(rawValue))
            {
                return Array.Empty<string>();
            }

            var normalized = rawValue.Trim();
            try
            {
                return JToken.Parse(normalized) is JArray array
                    ? array.Values<string>()
                        .Where(item => !string.IsNullOrWhiteSpace(item))
                        .Select(item => item!.Trim())
                        .ToArray()
                    : Array.Empty<string>();
            }
            catch (Exception ex) when (ex is FormatException or Newtonsoft.Json.JsonReaderException)
            {
                return Array.Empty<string>();
            }
        }

        private static IReadOnlyList<string> ExtractStringValues(
            IReadOnlyDictionary<string, object?> data,
            string key)
        {
            if (!data.TryGetValue(key, out var value) || value == null)
            {
                return Array.Empty<string>();
            }

            return value switch
            {
                JArray array => array.Values<string>()
                    .Where(item => !string.IsNullOrWhiteSpace(item))
                    .Select(item => item!.Trim())
                    .ToArray(),
                IEnumerable<string> strings => strings
                    .Where(item => !string.IsNullOrWhiteSpace(item))
                    .Select(item => item.Trim())
                    .ToArray(),
                _ => Array.Empty<string>()
            };
        }

        private static string BuildRepairMessage(
            string errorMessage,
            int consecutiveFailures,
            IReadOnlyList<AgentToolBackfill>? rejectedResponseBackfills,
            IReadOnlyList<LanguageModelChatMessage>? turnHistory,
            AgentLoopFinalOutput? latestMessageOutput)
        {
            var normalizedError = string.IsNullOrWhiteSpace(errorMessage)
                ? "The previous assistant response could not be accepted."
                : errorMessage.Trim();
            var executionStateHint = BuildRepairExecutionStateHint(
                rejectedResponseBackfills,
                turnHistory,
                latestMessageOutput);

            return
                $"The previous assistant response was rejected and the turn is still open. Reason: {normalizedError} " +
                $"Ignore any partial text from that rejected response. {executionStateHint} " +
                $"This is consecutive failed iteration {consecutiveFailures} of {MaxConsecutiveRecoverableFailures}. " +
                $"Reply again with exactly one complete <Tools> XML tree. Put user-visible text only inside {CreateMessageTool.ToolName}, and call {FinishTaskTool.ToolName} only after a successful {CreateMessageTool.ToolName}.";
        }

        private static string BuildRepairExecutionStateHint(
            IReadOnlyList<AgentToolBackfill>? rejectedResponseBackfills,
            IReadOnlyList<LanguageModelChatMessage>? turnHistory,
            AgentLoopFinalOutput? latestMessageOutput)
        {
            var hints = new List<string>();
            var acceptedFromRejectedResponse = BuildBackfillToolCallSummaries(
                rejectedResponseBackfills,
                requireAllSuccessful: true);
            if (acceptedFromRejectedResponse.Count > 0)
            {
                hints.Add(
                    $"Tool calls already accepted from the rejected response: {string.Join(", ", acceptedFromRejectedResponse)}. Do not call these again unless you intentionally need a new side effect or need to replace the current reply.");
            }

            var failedFromRejectedResponse = BuildBackfillToolCallSummaries(
                rejectedResponseBackfills,
                requireAllSuccessful: false);
            if (failedFromRejectedResponse.Count > 0)
            {
                hints.Add(
                    $"Tool calls that returned errors from the rejected response: {string.Join(", ", failedFromRejectedResponse)}. Retry only the failed or missing work if it is still needed.");
            }

            var successfulHistoryCalls = BuildSuccessfulHistoryToolReturnSummaries(turnHistory);
            if (successfulHistoryCalls.Count > 0)
            {
                hints.Add(
                    $"Successful tool calls already recorded in this turn history: {string.Join(", ", successfulHistoryCalls)}. Use those ToolReturn messages as authoritative context; do not replay them just because the previous response was rejected.");
            }

            if (latestMessageOutput != null)
            {
                hints.Add(
                    $"A current reply candidate already exists from the latest successful {CreateMessageTool.ToolName}. Do not repeat {CreateMessageTool.ToolName} just to restate the same message; call {FinishTaskTool.ToolName} if the task is ready to close, or call {CreateMessageTool.ToolName} only if you need to update the reply.");
            }

            if (hints.Count == 0)
            {
                hints.Add("No tool calls from the rejected response were accepted. Any streamed text from that rejected response is not a successful tool call.");
            }

            return string.Join(" ", hints);
        }

        private static IReadOnlyList<string> BuildBackfillToolCallSummaries(
            IReadOnlyList<AgentToolBackfill>? toolBackfills,
            bool requireAllSuccessful)
        {
            if (toolBackfills == null || toolBackfills.Count == 0)
            {
                return Array.Empty<string>();
            }

            var summaries = toolBackfills
                .Where(backfill => backfill.ToolReturns.Count > 0)
                .Where(backfill => requireAllSuccessful
                    ? backfill.ToolReturns.All(toolReturn => toolReturn.IsSuccess)
                    : backfill.ToolReturns.Any(toolReturn => !toolReturn.IsSuccess))
                .Select(BuildBackfillToolCallSummary)
                .Where(summary => !string.IsNullOrWhiteSpace(summary))
                .ToArray();

            return LimitRepairToolCallSummaries(summaries);
        }

        private static string BuildBackfillToolCallSummary(AgentToolBackfill backfill)
        {
            var toolNames = backfill.ToolReturns
                .Select(toolReturn => toolReturn.ToolName?.Trim() ?? string.Empty)
                .Where(toolName => toolName.Length > 0)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();

            if (toolNames.Length == 0)
            {
                return string.Empty;
            }

            return backfill.ToolCallIndex > 0
                ? $"#{backfill.ToolCallIndex} {string.Join("+", toolNames)}"
                : string.Join("+", toolNames);
        }

        private static IReadOnlyList<string> BuildSuccessfulHistoryToolReturnSummaries(
            IReadOnlyList<LanguageModelChatMessage>? turnHistory)
        {
            if (turnHistory == null || turnHistory.Count == 0)
            {
                return Array.Empty<string>();
            }

            var countsByToolName = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            foreach (var message in turnHistory)
            {
                if (message.Role != LanguageModelChatRole.User)
                {
                    continue;
                }

                foreach (var toolName in ExtractSuccessfulToolReturnNames(message.Content))
                {
                    countsByToolName.TryGetValue(toolName, out var currentCount);
                    countsByToolName[toolName] = currentCount + 1;
                }
            }

            if (countsByToolName.Count == 0)
            {
                return Array.Empty<string>();
            }

            var summaries = countsByToolName
                .OrderBy(item => item.Key, StringComparer.OrdinalIgnoreCase)
                .Select(item => item.Value > 1 ? $"{item.Key} x{item.Value}" : item.Key)
                .ToArray();

            return LimitRepairToolCallSummaries(summaries);
        }

        private static IEnumerable<string> ExtractSuccessfulToolReturnNames(string? xml)
        {
            if (string.IsNullOrWhiteSpace(xml))
            {
                yield break;
            }

            XDocument document;
            try
            {
                document = XDocument.Parse(xml, LoadOptions.PreserveWhitespace);
            }
            catch (Exception ex) when (ex is XmlException or InvalidOperationException)
            {
                yield break;
            }

            var root = document.Root;
            if (root == null || !string.Equals(root.Name.LocalName, "ToolsReturn", StringComparison.OrdinalIgnoreCase))
            {
                yield break;
            }

            foreach (var toolReturn in root.Elements().Where(element =>
                         string.Equals(element.Name.LocalName, "ToolReturn", StringComparison.OrdinalIgnoreCase)))
            {
                if (toolReturn.Elements().Any(element =>
                        string.Equals(element.Name.LocalName, "ErrorMessage", StringComparison.OrdinalIgnoreCase)))
                {
                    continue;
                }

                var toolName = toolReturn.Attributes()
                    .FirstOrDefault(attribute =>
                        string.Equals(attribute.Name.LocalName, "ToolName", StringComparison.OrdinalIgnoreCase))
                    ?.Value
                    ?.Trim();

                if (!string.IsNullOrWhiteSpace(toolName))
                {
                    yield return toolName;
                }
            }
        }

        private static IReadOnlyList<string> LimitRepairToolCallSummaries(IReadOnlyList<string> summaries)
        {
            if (summaries.Count <= MaxRepairToolCallSummaries)
            {
                return summaries;
            }

            return summaries
                .Take(MaxRepairToolCallSummaries)
                .Append($"and {summaries.Count - MaxRepairToolCallSummaries} more")
                .ToArray();
        }

        private void AppendFailedIterationHistory(
            InvalidAssistantResponseException failure,
            string repairMessage,
            ICollection<LanguageModelChatMessage> turnHistory)
        {
            ArgumentNullException.ThrowIfNull(failure);
            ArgumentNullException.ThrowIfNull(turnHistory);

            if (failure.ToolBackfills.Count > 0)
            {
                AppendCurrentTurnHistory(failure.AssistantResponse, failure.ToolBackfills, turnHistory);
            }
            else if (!string.IsNullOrWhiteSpace(failure.AssistantResponse.RawContent))
            {
                var assistantHistoryContent = CreateAssistantHistoryContent(failure.AssistantResponse.RawContent);
                turnHistory.Add(new LanguageModelChatMessage(
                    LanguageModelChatRole.Assistant,
                    assistantHistoryContent));
            }

            turnHistory.Add(new LanguageModelChatMessage(
                LanguageModelChatRole.User,
                CreateRepairBackfillXml(repairMessage))
            {
                AuthorName = RequestRepairToolName
            });
        }

        private string CreateRepairBackfillXml(string repairMessage)
        {
            return _toolManager.BuildToolsReturnXml(
            [
                _toolManager.CreateErrorToolReturnPayload(RequestRepairToolName, repairMessage)
            ]);
        }

        private static void AppendCurrentTurnHistory(
            AgentAssistantResponse assistantResponse,
            IReadOnlyList<AgentToolBackfill> toolBackfills,
            ICollection<LanguageModelChatMessage> turnHistory)
        {
            ArgumentNullException.ThrowIfNull(assistantResponse);
            ArgumentNullException.ThrowIfNull(toolBackfills);
            ArgumentNullException.ThrowIfNull(turnHistory);

            var orderedBackfills = toolBackfills
                .OrderBy(item => item.PartIndex)
                .ThenBy(item => item.ToolCallIndex)
                .ToArray();

            var backfillCursor = 0;
            for (var partIndex = 0; partIndex < assistantResponse.Parts.Count; partIndex++)
            {
                var part = assistantResponse.Parts[partIndex];
                if (!part.HasParseError && !string.IsNullOrWhiteSpace(part.Content))
                {
                    var assistantHistoryContent = CreateAssistantHistoryContent(part.Content);
                    turnHistory.Add(new LanguageModelChatMessage(
                        LanguageModelChatRole.Assistant,
                        assistantHistoryContent));
                }

                while (backfillCursor < orderedBackfills.Length &&
                       orderedBackfills[backfillCursor].PartIndex == partIndex)
                {
                    turnHistory.Add(CreateToolResultMessage(orderedBackfills[backfillCursor]));
                    backfillCursor++;
                }
            }

            while (backfillCursor < orderedBackfills.Length)
            {
                turnHistory.Add(CreateToolResultMessage(orderedBackfills[backfillCursor]));
                backfillCursor++;
            }
        }

        private static LanguageModelChatMessage CreateToolResultMessage(AgentToolBackfill backfill)
        {
            var authorName = backfill.ToolReturns.Count == 1
                ? backfill.ToolReturns[0].ToolName
                : "SkyweaverTools";

            return new LanguageModelChatMessage(LanguageModelChatRole.User, backfill.ToolsReturnXml)
            {
                AuthorName = authorName
            };
        }

        private static AgentAssistantResponse CreateRejectedAssistantResponse(
            string rawContent,
            string errorMessage)
        {
            var normalizedRawContent = rawContent ?? string.Empty;
            if (normalizedRawContent.Length == 0)
            {
                return new AgentAssistantResponse(string.Empty, Array.Empty<AgentAssistantResponsePart>());
            }

            return new AgentAssistantResponse(
                normalizedRawContent,
                [
                    AgentAssistantResponsePart.CreateToolCall(
                        normalizedRawContent,
                        Array.Empty<SkyweaverToolInvocation>(),
                        parseError: string.IsNullOrWhiteSpace(errorMessage)
                            ? "The assistant response was rejected."
                            : errorMessage.Trim(),
                        toolCallIndex: 1)
                ]);
        }

        private static bool TryParseBooleanValue(string rawValue, out bool value)
        {
            switch ((rawValue ?? string.Empty).Trim().ToLowerInvariant())
            {
                case "true":
                case "1":
                case "yes":
                case "y":
                case "on":
                    value = true;
                    return true;

                case "false":
                case "0":
                case "no":
                case "n":
                case "off":
                    value = false;
                    return true;

                default:
                    value = false;
                    return false;
            }
        }

        private static bool TryParseXmlWithRoot(
            string? payload,
            string expectedRootName,
            out string normalizedXml,
            out string errorMessage)
        {
            normalizedXml = string.Empty;
            errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(payload))
            {
                errorMessage = "The payload is empty.";
                return false;
            }

            XDocument document;
            try
            {
                document = XDocument.Parse(payload, LoadOptions.PreserveWhitespace);
            }
            catch (Exception ex) when (ex is XmlException or InvalidOperationException)
            {
                errorMessage = $"The payload could not be parsed as XML: {ex.Message}";
                return false;
            }

            var root = document.Root;
            if (root == null)
            {
                errorMessage = "The payload is missing a root element.";
                return false;
            }

            if (!string.Equals(root.Name.LocalName, expectedRootName, StringComparison.Ordinal))
            {
                errorMessage = $"The root element must be <{expectedRootName}> instead of <{root.Name.LocalName}>.";
                return false;
            }

            normalizedXml = document.ToString();
            return true;
        }

        private static string GetLanguageModelDisplayName(LanguageModelDefinition model)
        {
            ArgumentNullException.ThrowIfNull(model);

            var displayName = model.DisplayName?.Trim() ?? string.Empty;
            if (displayName.Length > 0)
            {
                return displayName;
            }

            var summaryModelId = model.SummaryModelId?.Trim() ?? string.Empty;
            if (summaryModelId.Length > 0)
            {
                return summaryModelId;
            }

            return string.IsNullOrWhiteSpace(model.Key) ? "unnamed-model" : model.Key.Trim();
        }
    }
}
