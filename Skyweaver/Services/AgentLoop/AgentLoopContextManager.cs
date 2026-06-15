using Skyweaver.Controls.AgentConfigurationControl.Models;
using Skyweaver.Controls.LanguageModelConfigurationControl.Services;
using Skyweaver.Services.ContextManagement;

namespace Skyweaver.Services.AgentLoop
{
    public sealed class AgentLoopContextManager
    {
        private const string ToolProtocolTailReminder =
            """
<SystemTips>
这是系统的小贴士，不是用户消息。请将以下内容视为系统注入的上下文提醒，而不是用户提出的新请求。

再次确认工具协议：
1. 【必须】使用唯一合法的 XML 根标签格式：
   <Tool ToolName="工具名">参数内容</Tool>  或  <ToolAsync ToolName="工具名">参数内容</ToolAsync>
2. 【严禁】直接将具体工具名称作为 XML 根标签（例如：严禁使用 <SpawnSubAgent>...</SpawnSubAgent>，必须写为 <Tool ToolName="SpawnSubAgent">...</Tool>）。
3. 【严禁】使用 <tool_call>、<function_call>、CreateMessage、FinishTask、<tools>、<tool_calls> 等任何其他伪协议。
4. 工具标签必须是完整且独立的 XML；严禁夹杂在普通正文文字里。
5. 如果想要代理循环继续，必须在回复中包含有效的工具调用，否则代理循环自动终止。

正确的工具调用 One-shot 示例：
<Tool ToolName="SpawnSubAgent">
  <SubAgentID>Agent16</SubAgentID>
  <Mission>探索 V:Project2Services 目录，并列出所有文件的完整路径。</Mission>
</Tool>
</SystemTips>
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
                sessionResourcesFolderPath: null,
                runtimeToolCallNotice: null,
                forceOptimizeToolCallPrompt: false,
                cancellationToken: cancellationToken);
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
            string? sessionResourcesFolderPath = null,
            string? runtimeToolCallNotice = null,
            bool forceOptimizeToolCallPrompt = false,
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
                currentTurnHistory,
                sessionResourcesFolderPath,
                runtimeToolCallNotice,
                forceOptimizeToolCallPrompt);
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
            IReadOnlyList<LanguageModelChatMessage> turnHistory,
            string? sessionResourcesFolderPath,
            string? runtimeToolCallNotice,
            bool forceOptimizeToolCallPrompt)
        {
            var messages = new List<LanguageModelChatMessage>(persistentHistory.Count + turnHistory.Count + 4)
            {
                new(LanguageModelChatRole.System, systemPrompt ?? string.Empty)
            };

            messages.AddRange(persistentHistory.Select(message => message.Clone()));
            messages.Add(CreateInputMessage(upstreamInput, upstreamContentBlocks));
            messages.AddRange(turnHistory.Select(message => message.Clone()));
            InsertToolProtocolTailReminder(messages, sessionResourcesFolderPath, runtimeToolCallNotice, forceOptimizeToolCallPrompt);
            return messages;
        }

        private static void InsertToolProtocolTailReminder(
            List<LanguageModelChatMessage> messages,
            string? sessionResourcesFolderPath,
            string? runtimeToolCallNotice,
            bool forceOptimizeToolCallPrompt)
        {
            ArgumentNullException.ThrowIfNull(messages);

            LanguageModelChatMessage? planMessage = null;
            if (!string.IsNullOrWhiteSpace(sessionResourcesFolderPath))
            {
                var activePlans = PlanManager.LoadActivePlans(sessionResourcesFolderPath);
                if (activePlans.Count > 0)
                {
                    var planPromptText = PlanManager.BuildActivePlansPrompt(activePlans);
                    planMessage = new LanguageModelChatMessage(LanguageModelChatRole.System, planPromptText)
                    {
                        IsHostInjectedTail = true
                    };
                }
            }

            // 构造运行时工具调用提示消息，作为系统注入提醒
            LanguageModelChatMessage? noticeMessage = null;
            if (!string.IsNullOrWhiteSpace(runtimeToolCallNotice))
            {
                noticeMessage = new LanguageModelChatMessage(LanguageModelChatRole.System, runtimeToolCallNotice)
                {
                    IsHostInjectedTail = true
                };
            }

            var config = ContextArrangementRuntime.Instance.GetConfiguration();
            LanguageModelChatMessage? reminder = null;
            if (config.OptimizeToolCallPrompt || forceOptimizeToolCallPrompt)
            {
                reminder = CreateToolProtocolTailReminder();
            }

            for (var index = messages.Count - 1; index > 0; index--)
            {
                if (messages[index].Role is LanguageModelChatRole.User or LanguageModelChatRole.System)
                {
                    if (reminder != null)
                    {
                        messages.Insert(index, reminder);
                    }
                    if (planMessage != null)
                    {
                        messages.Insert(index, planMessage);
                    }
                    if (noticeMessage != null)
                    {
                        messages.Insert(index, noticeMessage);
                    }
                    return;
                }
            }

            if (noticeMessage != null)
            {
                messages.Add(noticeMessage);
            }
            if (planMessage != null)
            {
                messages.Add(planMessage);
            }
            if (reminder != null)
            {
                messages.Add(reminder);
            }
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
