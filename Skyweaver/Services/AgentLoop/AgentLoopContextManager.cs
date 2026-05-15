using Skyweaver.Controls.AgentConfigurationControl.Models;
using Skyweaver.Controls.LanguageModelConfigurationControl.Services;

namespace Skyweaver.Services.AgentLoop
{
    public sealed class AgentLoopContextManager
    {
        private const string ToolProtocolTailReminder =
            """
再次确认工具协议：
- 只使用 <Tool> / <ToolAsync>。
- 严禁 <tool_call>、<function_call>、CreateMessage、FinishTask、<tools>、<tool_calls> 等伪协议。
- 工具标签必须是完整 XML；不要夹在普通正文里。
""";

        private readonly AgentLoopCompactionStore _compactionStore;

        public AgentLoopContextManager()
            : this(new AgentLoopCompactionStore())
        {
        }

        public AgentLoopContextManager(AgentLoopCompactionStore compactionStore)
        {
            _compactionStore = compactionStore ?? throw new ArgumentNullException(nameof(compactionStore));
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
                upstreamContentBlocks: null,
                persistentHistory,
                turnHistory,
                compactionFilePath: null,
                debugRunContext: null,
                iterationNumber: 0,
                cancellationToken);
        }

        internal Task<AgentLoopContextPreparationResult> PrepareAsync(
            AgentDefinition agent,
            string systemPrompt,
            string upstreamInput,
            IReadOnlyList<LanguageModelChatContentBlock>? upstreamContentBlocks,
            IReadOnlyList<LanguageModelChatMessage> persistentHistory,
            IReadOnlyList<LanguageModelChatMessage> turnHistory,
            string? compactionFilePath = null,
            AgentLoopDebugRunContext? debugRunContext = null,
            int iterationNumber = 0,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(agent);
            cancellationToken.ThrowIfCancellationRequested();

            var immutableHistory = TrimTrailingCurrentInputEchoes(
                NormalizeHistory(persistentHistory),
                upstreamInput,
                upstreamContentBlocks);
            var currentTurnHistory = NormalizeHistory(turnHistory);

            var preparedMessages = ComposeMessages(
                systemPrompt,
                immutableHistory,
                upstreamInput,
                upstreamContentBlocks,
                currentTurnHistory);
            var compactedPreparedMessages = _compactionStore.ApplyCompaction(
                compactionFilePath,
                preparedMessages);

            return Task.FromResult(new AgentLoopContextPreparationResult
            {
                PreparedMessages = compactedPreparedMessages,
                PersistentHistory = immutableHistory,
                TurnHistory = currentTurnHistory
            });
        }

        private static IReadOnlyList<LanguageModelChatMessage> ComposeMessages(
            string systemPrompt,
            IReadOnlyList<LanguageModelChatMessage> persistentHistory,
            string upstreamInput,
            IReadOnlyList<LanguageModelChatContentBlock>? upstreamContentBlocks,
            IReadOnlyList<LanguageModelChatMessage> turnHistory)
        {
            var messages = new List<LanguageModelChatMessage>(persistentHistory.Count + turnHistory.Count + 3)
            {
                new(LanguageModelChatRole.System, systemPrompt ?? string.Empty)
            };

            messages.AddRange(persistentHistory.Select(message => message.Clone()));
            messages.Add(CreateInputMessage(upstreamInput, upstreamContentBlocks));
            messages.AddRange(turnHistory.Select(message => message.Clone()));
            messages.Add(CreateToolProtocolTailReminder());
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

        private static List<LanguageModelChatMessage> TrimTrailingCurrentInputEchoes(
            List<LanguageModelChatMessage> history,
            string? upstreamInput,
            IReadOnlyList<LanguageModelChatContentBlock>? upstreamContentBlocks)
        {
            ArgumentNullException.ThrowIfNull(history);

            if (history.Count == 0)
            {
                return history;
            }

            var currentInputMessage = CreateInputMessage(upstreamInput, upstreamContentBlocks);
            if (currentInputMessage.ContentBlocks.Count == 0)
            {
                return history;
            }

            var endExclusive = history.Count;
            while (endExclusive > 0 &&
                   IsCurrentInputEcho(history[endExclusive - 1], currentInputMessage))
            {
                endExclusive--;
            }

            if (endExclusive == history.Count)
            {
                return history;
            }

            history.RemoveRange(endExclusive, history.Count - endExclusive);
            return history;
        }

        private static bool IsCurrentInputEcho(
            LanguageModelChatMessage candidate,
            LanguageModelChatMessage currentInputMessage)
        {
            return candidate.Role == LanguageModelChatRole.User &&
                   AreContentBlocksEquivalent(candidate.ContentBlocks, currentInputMessage.ContentBlocks);
        }

        private static bool AreContentBlocksEquivalent(
            IReadOnlyList<LanguageModelChatContentBlock> left,
            IReadOnlyList<LanguageModelChatContentBlock> right)
        {
            if (left.Count != right.Count)
            {
                return false;
            }

            for (var index = 0; index < left.Count; index++)
            {
                if (!AreContentBlocksEquivalent(left[index], right[index]))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool AreContentBlocksEquivalent(
            LanguageModelChatContentBlock left,
            LanguageModelChatContentBlock right)
        {
            return left.Kind == right.Kind &&
                   string.Equals(NormalizeComparableContent(left.Content), NormalizeComparableContent(right.Content), StringComparison.Ordinal) &&
                   string.Equals(NormalizeComparableContent(left.ResourcePath), NormalizeComparableContent(right.ResourcePath), StringComparison.Ordinal) &&
                   string.Equals(NormalizeComparableContent(left.MediaType), NormalizeComparableContent(right.MediaType), StringComparison.OrdinalIgnoreCase) &&
                   AreByteArraysEquivalent(left.Data, right.Data);
        }

        private static bool AreByteArraysEquivalent(byte[]? left, byte[]? right)
        {
            if (left == null || left.Length == 0)
            {
                return right == null || right.Length == 0;
            }

            return right != null && left.SequenceEqual(right);
        }

        private static string NormalizeComparableContent(string? content)
        {
            return string.IsNullOrWhiteSpace(content)
                ? string.Empty
                : content.Trim();
        }

        private static LanguageModelChatMessage? NormalizeHistoryMessage(
            IReadOnlyList<LanguageModelChatMessage> history,
            int index)
        {
            ArgumentNullException.ThrowIfNull(history);

            var message = history[index];
            ArgumentNullException.ThrowIfNull(message);

            var normalizedRole = message.Role == LanguageModelChatRole.Tool
                ? LanguageModelChatRole.User
                : message.Role;

            return new LanguageModelChatMessage(
                normalizedRole,
                message.ContentBlocks.Select(block => block.Clone()).ToArray())
            {
                AuthorName = message.AuthorName,
                IsHostInjectedTail = message.IsHostInjectedTail
            };
        }

        private static LanguageModelChatMessage CreateInputMessage(
            string? upstreamInput,
            IReadOnlyList<LanguageModelChatContentBlock>? upstreamContentBlocks)
        {
            var contentBlocks = new List<LanguageModelChatContentBlock>();
            if (!string.IsNullOrWhiteSpace(upstreamInput))
            {
                contentBlocks.Add(LanguageModelChatContentBlock.CreateText(upstreamInput));
            }

            if (upstreamContentBlocks != null)
            {
                contentBlocks.AddRange(upstreamContentBlocks
                    .Where(block => block != null)
                    .Select(block => block.Clone()));
            }

            return new LanguageModelChatMessage(LanguageModelChatRole.User, contentBlocks);
        }

        private static LanguageModelChatMessage CreateToolProtocolTailReminder()
        {
            return new LanguageModelChatMessage(LanguageModelChatRole.User, ToolProtocolTailReminder)
            {
                IsHostInjectedTail = true
            };
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
