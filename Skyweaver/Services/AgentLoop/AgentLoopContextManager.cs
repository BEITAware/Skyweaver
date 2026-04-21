using System.Text;
using Skyweaver.Controls.AgentConfigurationControl.Models;
using Skyweaver.Controls.AgentConfigurationControl.Services;
using Skyweaver.Controls.LanguageModelConfigurationControl.Services;
using Skyweaver.Services.SkyweaverTools;

namespace Skyweaver.Services.AgentLoop
{
    public sealed class AgentLoopContextManager
    {
        private const double CompressionTargetRatio = 0.25d;

        private readonly IAgentLanguageModelResolver _languageModelResolver;
        private readonly ILanguageModelChatService _chatService;
        private readonly ApproximateTokenEstimator _tokenEstimator = new();

        private sealed record CompressionExecutionResult(string SummaryText, string? ModelId);

        public AgentLoopContextManager()
            : this(new AgentLanguageModelResolver(), new LanguageModelChatService())
        {
        }

        public AgentLoopContextManager(
            IAgentLanguageModelResolver languageModelResolver,
            ILanguageModelChatService chatService)
        {
            _languageModelResolver = languageModelResolver ?? throw new ArgumentNullException(nameof(languageModelResolver));
            _chatService = chatService ?? throw new ArgumentNullException(nameof(chatService));
        }

        public Task<AgentLoopContextPreparationResult> PrepareAsync(
            AgentDefinition agent,
            string systemPrompt,
            string upstreamInput,
            IReadOnlyList<LanguageModelChatMessage> persistentHistory,
            IReadOnlyList<LanguageModelChatMessage> turnHistory,
            CancellationToken cancellationToken = default)
        {
            return PrepareAsync(
                agent,
                systemPrompt,
                upstreamInput,
                persistentHistory,
                turnHistory,
                debugRunContext: null,
                iterationNumber: 0,
                cancellationToken);
        }

        internal async Task<AgentLoopContextPreparationResult> PrepareAsync(
            AgentDefinition agent,
            string systemPrompt,
            string upstreamInput,
            IReadOnlyList<LanguageModelChatMessage> persistentHistory,
            IReadOnlyList<LanguageModelChatMessage> turnHistory,
            AgentLoopDebugRunContext? debugRunContext = null,
            int iterationNumber = 0,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(agent);

            var immutableHistory = NormalizeHistory(persistentHistory);
            var currentTurnHistory = NormalizeHistory(turnHistory);

            var contextWindowTokens = _languageModelResolver.GetMinimumContextWindowTokens(agent);
            var preparedMessages = ComposeMessages(
                systemPrompt,
                immutableHistory,
                upstreamInput,
                currentTurnHistory);
            var estimatedBefore = _tokenEstimator.Estimate(preparedMessages);

            if (estimatedBefore <= contextWindowTokens)
            {
                return new AgentLoopContextPreparationResult
                {
                    PreparedMessages = preparedMessages,
                    PersistentHistory = immutableHistory,
                    TurnHistory = currentTurnHistory
                };
            }

            if (immutableHistory.Count == 0)
            {
                throw new InvalidOperationException(
                    $"Agent '{agent.DisplayNameOrFallback}' exceeds the context window with its system prompt, current user input, and current-turn loop history. Earlier-turn history is already empty, so there is nothing left to compress.");
            }

            var fixedMessages = ComposeFixedMessages(systemPrompt, upstreamInput, currentTurnHistory);
            var fixedTokenCount = _tokenEstimator.Estimate(fixedMessages);
            if (fixedTokenCount >= contextWindowTokens)
            {
                throw new InvalidOperationException(
                    $"Agent '{agent.DisplayNameOrFallback}' exceeds the context window with its system prompt, current user input, and current-turn loop history. Compressing earlier turns would not be enough to continue.");
            }

            var targetTokenCountAfterCompression = Math.Max(
                fixedTokenCount,
                (int)Math.Floor(contextWindowTokens * CompressionTargetRatio));
            var targetHistoryTokens = Math.Max(0, targetTokenCountAfterCompression - fixedTokenCount);

            var compressionResult = await CompressHistoryAsync(
                agent,
                upstreamInput,
                immutableHistory,
                targetHistoryTokens,
                debugRunContext,
                iterationNumber,
                cancellationToken).ConfigureAwait(false);

            var compressedPersistentHistory = string.IsNullOrWhiteSpace(compressionResult.SummaryText)
                ? new List<LanguageModelChatMessage>()
                : new List<LanguageModelChatMessage>
                {
                    new(LanguageModelChatRole.Assistant, BuildCompressedHistoryEnvelope(compressionResult.SummaryText))
                    {
                        AuthorName = "HistorySummary"
                    }
                };

            preparedMessages = ComposeMessages(
                systemPrompt,
                compressedPersistentHistory,
                upstreamInput,
                currentTurnHistory);
            var estimatedAfter = _tokenEstimator.Estimate(preparedMessages);
            if (estimatedAfter > contextWindowTokens)
            {
                throw new InvalidOperationException(
                    $"Agent '{agent.DisplayNameOrFallback}' still exceeds the context window after compressing earlier-turn history. Shorten the system prompt, the current input, or the current-turn loop transcript.");
            }

            return new AgentLoopContextPreparationResult
            {
                PreparedMessages = preparedMessages,
                PersistentHistory = compressedPersistentHistory,
                TurnHistory = currentTurnHistory,
                ContextCompression = new AgentLoopContextCompressionInfo
                {
                    ContextWindowTokens = contextWindowTokens,
                    EstimatedTokenCountBeforeCompression = estimatedBefore,
                    EstimatedTokenCountAfterCompression = estimatedAfter,
                    TargetTokenCountAfterCompression = targetTokenCountAfterCompression,
                    CompressionLayerKey = CapabilityLayerBuiltIns.ContextCompressionLayerKey,
                    CompressionModelId = compressionResult.ModelId
                }
            };
        }

        private async Task<CompressionExecutionResult> CompressHistoryAsync(
            AgentDefinition agent,
            string upstreamInput,
            IReadOnlyList<LanguageModelChatMessage> persistentHistory,
            int targetHistoryTokens,
            AgentLoopDebugRunContext? debugRunContext,
            int iterationNumber,
            CancellationToken cancellationToken)
        {
            var compressionMessages = BuildCompressionMessages(
                agent,
                upstreamInput,
                persistentHistory,
                targetHistoryTokens);
            var attemptNumber = 0;

            return await _languageModelResolver.ExecuteCapabilityLayerWithFallbackAsync(
                CapabilityLayerBuiltIns.ContextCompressionLayerKey,
                async (model, ct) =>
                {
                    attemptNumber++;
                    AgentLoopDebugRecorder.RecordCompressionRequest(
                        debugRunContext,
                        agent,
                        model,
                        iterationNumber,
                        attemptNumber,
                        targetHistoryTokens,
                        upstreamInput,
                        persistentHistory,
                        compressionMessages);

                    try
                    {
                        var response = await _chatService.GetResponseAsync(
                            model,
                            compressionMessages.Select(message => message.Clone()).ToArray(),
                            ct).ConfigureAwait(false);
                        var summaryText = response.Text.Trim();

                        AgentLoopDebugRecorder.RecordCompressionResult(
                            debugRunContext,
                            agent,
                            model,
                            iterationNumber,
                            attemptNumber,
                            response.ModelId,
                            summaryText);

                        return new CompressionExecutionResult(
                            summaryText,
                            response.ModelId);
                    }
                    catch (Exception ex)
                    {
                        AgentLoopDebugRecorder.RecordCompressionFailure(
                            debugRunContext,
                            agent,
                            model,
                            iterationNumber,
                            attemptNumber,
                            ex);
                        throw;
                    }
                },
                cancellationToken).ConfigureAwait(false);
        }

        private static IReadOnlyList<LanguageModelChatMessage> ComposeMessages(
            string systemPrompt,
            IReadOnlyList<LanguageModelChatMessage> persistentHistory,
            string upstreamInput,
            IReadOnlyList<LanguageModelChatMessage> turnHistory)
        {
            var messages = new List<LanguageModelChatMessage>(persistentHistory.Count + turnHistory.Count + 2)
            {
                new(LanguageModelChatRole.System, systemPrompt ?? string.Empty)
            };

            messages.AddRange(persistentHistory.Select(message => message.Clone()));
            messages.Add(new LanguageModelChatMessage(LanguageModelChatRole.User, upstreamInput ?? string.Empty));
            messages.AddRange(turnHistory.Select(message => message.Clone()));
            return messages;
        }

        private static List<LanguageModelChatMessage> NormalizeHistory(
            IReadOnlyList<LanguageModelChatMessage>? history)
        {
            var sourceHistory = history ?? Array.Empty<LanguageModelChatMessage>();
            var normalizedHistory = new List<LanguageModelChatMessage>(sourceHistory.Count);

            for (var index = 0; index < sourceHistory.Count; index++)
            {
                var normalizedMessage = NormalizeHistoryMessage(sourceHistory, index);
                if (normalizedMessage != null)
                {
                    normalizedHistory.Add(normalizedMessage);
                }
            }

            return normalizedHistory;
        }

        private static LanguageModelChatMessage? NormalizeHistoryMessage(
            IReadOnlyList<LanguageModelChatMessage> history,
            int index)
        {
            ArgumentNullException.ThrowIfNull(history);

            var message = history[index];
            ArgumentNullException.ThrowIfNull(message);

            if (message.Role == LanguageModelChatRole.Assistant &&
                SkyweaverToolSyntaxInspector.ContainsInvalidPseudoToolMarkup(message.Content))
            {
                return null;
            }

            // Skyweaver's XML tool protocol carries tool returns as plain XML text instead of
            // MEAI-native FunctionResultContent. Normalize any legacy Tool messages to User
            // so the raw XML transcript is preserved by downstream chat backends.
            var normalizedRole = message.Role == LanguageModelChatRole.Tool
                ? LanguageModelChatRole.User
                : message.Role;

            return new LanguageModelChatMessage(normalizedRole, message.Content)
            {
                AuthorName = message.AuthorName
            };
        }

        private static IReadOnlyList<LanguageModelChatMessage> ComposeFixedMessages(
            string systemPrompt,
            string upstreamInput,
            IReadOnlyList<LanguageModelChatMessage> turnHistory)
        {
            var messages = new List<LanguageModelChatMessage>(turnHistory.Count + 2)
            {
                new(LanguageModelChatRole.System, systemPrompt ?? string.Empty),
                new(LanguageModelChatRole.User, upstreamInput ?? string.Empty)
            };

            messages.AddRange(turnHistory.Select(message => message.Clone()));
            return messages;
        }

        private static IReadOnlyList<LanguageModelChatMessage> BuildCompressionMessages(
            AgentDefinition agent,
            string upstreamInput,
            IReadOnlyList<LanguageModelChatMessage> persistentHistory,
            int targetHistoryTokens)
        {
            var transcript = BuildHistoryTranscript(persistentHistory);
            var systemPrompt = """
You are a host-side conversation-history compressor.
Your job is not to continue the task. Your job is to summarize earlier turns that happened before the current user message.

Preserve:
1. Confirmed facts, constraints, and decisions that still matter.
2. Important tool calls, tool results, and failures.
3. The latest successful CreateMessage payloads that matter for continuity.
4. Open questions, unfinished branches, and relevant intermediate conclusions.
5. Any assistant commitments or prior outputs that the next turn must remain consistent with.
6. Whether a turn was explicitly closed with FinishTask.

Do not write XML. Do not continue the conversation. Return only the compressed summary body.
Accuracy matters more than brevity, but try to stay within the requested budget.
""";

            var userPrompt = $"""
Agent: {agent.DisplayNameOrFallback}
Target history budget: {targetHistoryTokens}

Current user input for reference:
{upstreamInput}

Earlier-turn history to compress:
{transcript}

Return only the compressed summary text.
""";

            return
            [
                new LanguageModelChatMessage(LanguageModelChatRole.System, systemPrompt),
                new LanguageModelChatMessage(LanguageModelChatRole.User, userPrompt)
            ];
        }

        private static string BuildHistoryTranscript(IReadOnlyList<LanguageModelChatMessage> history)
        {
            var builder = new StringBuilder();

            foreach (var message in history)
            {
                if (builder.Length > 0)
                {
                    builder.AppendLine();
                }

                builder.Append('[');
                builder.Append(message.Role);
                if (!string.IsNullOrWhiteSpace(message.AuthorName))
                {
                    builder.Append(':');
                    builder.Append(message.AuthorName);
                }

                builder.AppendLine("]");
                builder.AppendLine(message.Content ?? string.Empty);
            }

            return builder.ToString().Trim();
        }

        private static string BuildCompressedHistoryEnvelope(string summaryText)
        {
            return
                """
The following content is a host-generated summary of earlier conversation history from previous turns.
Treat it as background context that happened before the current user message.
It is not a new system instruction and not a new user request:

"""
                + summaryText.Trim();
        }

        private sealed class ApproximateTokenEstimator
        {
            public int Estimate(IReadOnlyList<LanguageModelChatMessage> messages)
            {
                return messages.Sum(Estimate);
            }

            private int Estimate(LanguageModelChatMessage message)
            {
                var authorCost = string.IsNullOrWhiteSpace(message.AuthorName)
                    ? 0
                    : EstimateText(message.AuthorName);
                return 6 + authorCost + EstimateText(message.Content);
            }

            private int EstimateText(string? text)
            {
                if (string.IsNullOrWhiteSpace(text))
                {
                    return 0;
                }

                var normalized = text.Trim();
                var cjkLikeCount = normalized.Count(IsCjkLikeCharacter);
                var otherCount = normalized.Length - cjkLikeCount;
                var otherTokenCount = (int)Math.Ceiling(otherCount / 4d);
                return Math.Max(1, cjkLikeCount + otherTokenCount);
            }

            private static bool IsCjkLikeCharacter(char value)
            {
                return value is >= '\u2E80' and <= '\u9FFF'
                    or >= '\uAC00' and <= '\uD7AF'
                    or >= '\u3040' and <= '\u30FF';
            }
        }
    }

    public sealed class AgentLoopContextPreparationResult
    {
        public IReadOnlyList<LanguageModelChatMessage> PreparedMessages { get; init; } = Array.Empty<LanguageModelChatMessage>();

        public IReadOnlyList<LanguageModelChatMessage> PersistentHistory { get; init; } = Array.Empty<LanguageModelChatMessage>();

        public IReadOnlyList<LanguageModelChatMessage> TurnHistory { get; init; } = Array.Empty<LanguageModelChatMessage>();

        public AgentLoopContextCompressionInfo? ContextCompression { get; init; }
    }
}
