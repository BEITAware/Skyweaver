using Skyweaver.Controls.LanguageModelConfigurationControl.Services;
using Skyweaver.Controls.WorkflowEditorControl.Models;
using Skyweaver.Models.ChatSession;

namespace Skyweaver.Services.ChatSession
{
    public sealed class ChatSessionConversationHistoryRecorder
    {
        private const string AssistantDisplayName = "Skyweaver Assistant";

        private readonly ChatSessionModel _session;
        private readonly object _syncRoot = new();

        public ChatSessionConversationHistoryRecorder(ChatSessionModel session, string currentUserText)
        {
            _session = session ?? throw new ArgumentNullException(nameof(session));

            var normalizedUserText = NormalizeContent(currentUserText);
            if (normalizedUserText.Length > 0)
            {
                AppendMessage(new LanguageModelChatMessage(LanguageModelChatRole.User, normalizedUserText));
            }
        }

        public void Record(ChatSessionRuntimeEvent runtimeEvent)
        {
            ArgumentNullException.ThrowIfNull(runtimeEvent);

            lock (_syncRoot)
            {
                switch (runtimeEvent.Kind)
                {
                    case ChatSessionRuntimeEventKind.AssistantToolTreeReceived:
                        AppendAssistantToolTree(runtimeEvent);
                        break;

                    case ChatSessionRuntimeEventKind.ToolOutputReceived:
                        AppendToolOutput(runtimeEvent);
                        break;
                }
            }
        }

        private void AppendAssistantToolTree(ChatSessionRuntimeEvent runtimeEvent)
        {
            if (runtimeEvent.NodeKind != SessionFlowNodeKind.Agent)
            {
                return;
            }

            var toolsXml = NormalizeContent(runtimeEvent.ToolXml);
            if (toolsXml.Length == 0)
            {
                return;
            }

            AppendMessage(new LanguageModelChatMessage(LanguageModelChatRole.Assistant, toolsXml)
            {
                AuthorName = NormalizeAuthor(runtimeEvent.NodeTitle) ?? AssistantDisplayName
            });
        }

        private void AppendToolOutput(ChatSessionRuntimeEvent runtimeEvent)
        {
            var toolOutputXml = NormalizeContent(runtimeEvent.ToolOutputXml);
            if (toolOutputXml.Length == 0)
            {
                return;
            }

            var authorName = runtimeEvent.ToolReturns.Count == 1
                ? runtimeEvent.ToolReturns[0].ToolName
                : runtimeEvent.ToolInvocation?.ToolName;

            AppendMessage(new LanguageModelChatMessage(LanguageModelChatRole.User, toolOutputXml)
            {
                AuthorName = NormalizeAuthor(authorName)
            });
        }

        private void AppendMessage(LanguageModelChatMessage message)
        {
            _session.ConversationHistory.Add(message);
        }

        private static string NormalizeContent(string? content)
        {
            return string.IsNullOrWhiteSpace(content)
                ? string.Empty
                : content.Trim();
        }

        private static string? NormalizeAuthor(string? authorName)
        {
            var normalized = NormalizeContent(authorName);
            return normalized.Length == 0 ? null : normalized;
        }
    }
}
