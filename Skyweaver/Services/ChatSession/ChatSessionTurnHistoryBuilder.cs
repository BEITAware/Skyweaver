using System.Xml.Linq;
using Skyweaver.Controls.ChatSessionControl.Models;
using Skyweaver.Controls.LanguageModelConfigurationControl.Services;
using Skyweaver.Models.ChatSession;
using Skyweaver.Services.SkyweaverTools;

namespace Skyweaver.Services.ChatSession
{
    public static class ChatSessionTurnHistoryBuilder
    {
        public static IReadOnlyList<LanguageModelChatMessage> BuildForNextTurn(
            ChatSessionModel session,
            string? currentUserText)
        {
            ArgumentNullException.ThrowIfNull(session);

            if (session.ConversationHistory.Count > 0)
            {
                return BuildFromStoredHistory(session.ConversationHistory, currentUserText);
            }

            return BuildFromMessages(session.Messages, currentUserText);
        }

        public static IReadOnlyList<LanguageModelChatMessage> BuildFromMessages(
            IEnumerable<ChatMessageModel> messages,
            string? currentUserText = null)
        {
            ArgumentNullException.ThrowIfNull(messages);

            var materializedMessages = messages.ToArray();
            var effectiveMessageCount = materializedMessages.Length;
            if (effectiveMessageCount > 0 &&
                IsCurrentTurnUserMessage(materializedMessages[effectiveMessageCount - 1], currentUserText))
            {
                effectiveMessageCount--;
            }

            var history = new List<LanguageModelChatMessage>();
            for (var index = 0; index < effectiveMessageCount; index++)
            {
                AppendProjectedMessage(history, materializedMessages[index]);
            }

            return history;
        }

        private static IReadOnlyList<LanguageModelChatMessage> BuildFromStoredHistory(
            IReadOnlyList<LanguageModelChatMessage> storedHistory,
            string? currentUserText)
        {
            var effectiveMessageCount = storedHistory.Count;
            if (effectiveMessageCount > 0 &&
                IsCurrentTurnUserMessage(storedHistory[effectiveMessageCount - 1], currentUserText))
            {
                effectiveMessageCount--;
            }

            var history = new List<LanguageModelChatMessage>(effectiveMessageCount);
            for (var index = 0; index < effectiveMessageCount; index++)
            {
                var projectedMessage = TryCloneStoredHistoryMessage(storedHistory, index);
                if (projectedMessage != null)
                {
                    history.Add(projectedMessage);
                }
            }

            return history;
        }

        private static void AppendProjectedMessage(
            ICollection<LanguageModelChatMessage> history,
            ChatMessageModel message)
        {
            ArgumentNullException.ThrowIfNull(history);
            ArgumentNullException.ThrowIfNull(message);

            if (!ShouldIncludeMessageInNextTurnHistory(message))
            {
                return;
            }

            var suppressAssistantText = message.Role == ChatMessageRole.Assistant &&
                                        MessageContainsToolActivity(message);
            foreach (var part in message.Parts)
            {
                if (!TryProjectPart(message, part, suppressAssistantText, out var projectedMessage) ||
                    projectedMessage == null)
                {
                    continue;
                }

                history.Add(projectedMessage);
            }
        }

        private static bool TryProjectPart(
            ChatMessageModel sourceMessage,
            ChatMessagePartModel part,
            bool suppressAssistantText,
            out LanguageModelChatMessage? projectedMessage)
        {
            ArgumentNullException.ThrowIfNull(sourceMessage);

            projectedMessage = null;
            if (part == null || !ShouldIncludePartInNextTurnHistory(part))
            {
                return false;
            }

            if (suppressAssistantText &&
                part.PartType is ChatMessagePartType.Text or ChatMessagePartType.Code or ChatMessagePartType.StructuredXml)
            {
                return false;
            }

            var content = BuildMessageContent(part);
            if (content.Length == 0)
            {
                return false;
            }

            if (sourceMessage.Role == ChatMessageRole.Assistant &&
                SkyweaverToolSyntaxInspector.ContainsInvalidPseudoToolMarkup(content))
            {
                return false;
            }

            var role = ResolveRole(sourceMessage, part);
            projectedMessage = new LanguageModelChatMessage(role, content)
            {
                AuthorName = ResolveAuthorName(sourceMessage, part, role)
            };
            return true;
        }

        private static bool ShouldIncludeMessageInNextTurnHistory(ChatMessageModel message)
        {
            ArgumentNullException.ThrowIfNull(message);
            return message.Role is ChatMessageRole.User or ChatMessageRole.Assistant;
        }

        private static bool MessageContainsToolActivity(ChatMessageModel message)
        {
            ArgumentNullException.ThrowIfNull(message);

            return message.Parts.Any(part =>
                !ChatSessionFinishTaskVisibility.IsInternalToolPart(part) &&
                part.PartType is
                    ChatMessagePartType.ToolCall or
                    ChatMessagePartType.ToolOutput or
                    ChatMessagePartType.Tool);
        }

        private static bool ShouldIncludePartInNextTurnHistory(ChatMessagePartModel part)
        {
            ArgumentNullException.ThrowIfNull(part);

            if (ChatSessionFinishTaskVisibility.IsInternalToolPart(part))
            {
                return false;
            }

            return part.PartType is ChatMessagePartType.Text
                or ChatMessagePartType.Code
                or ChatMessagePartType.StructuredXml
                or ChatMessagePartType.ToolCall
                or ChatMessagePartType.ToolOutput
                or ChatMessagePartType.Tool;
        }

        private static bool IsCurrentTurnUserMessage(ChatMessageModel message, string? currentUserText)
        {
            if (message.Role != ChatMessageRole.User)
            {
                return false;
            }

            var normalizedCurrentUserText = NormalizeContent(currentUserText);
            if (normalizedCurrentUserText.Length == 0)
            {
                return false;
            }

            var normalizedMessageText = string.Join(
                    Environment.NewLine + Environment.NewLine,
                    message.Parts
                        .Where(part => part.PartType != ChatMessagePartType.ToolCall)
                        .Where(part => !ChatSessionFinishTaskVisibility.IsInternalToolPart(part))
                        .Select(BuildMessageContent)
                        .Where(content => content.Length > 0))
                .Trim();

            return string.Equals(
                normalizedMessageText,
                normalizedCurrentUserText,
                StringComparison.Ordinal);
        }

        private static bool IsCurrentTurnUserMessage(LanguageModelChatMessage message, string? currentUserText)
        {
            if (message.Role != LanguageModelChatRole.User)
            {
                return false;
            }

            return string.Equals(
                NormalizeContent(message.Content),
                NormalizeContent(currentUserText),
                StringComparison.Ordinal);
        }

        private static LanguageModelChatMessage? TryCloneStoredHistoryMessage(
            IReadOnlyList<LanguageModelChatMessage> storedHistory,
            int index)
        {
            var message = storedHistory[index];
            if (!ShouldIncludeStoredHistoryMessage(storedHistory, index))
            {
                return null;
            }

            return message.Clone();
        }

        private static bool ShouldIncludeStoredHistoryMessage(
            IReadOnlyList<LanguageModelChatMessage> storedHistory,
            int index)
        {
            var message = storedHistory[index];
            if (message.Role == LanguageModelChatRole.Assistant &&
                SkyweaverToolSyntaxInspector.ContainsInvalidPseudoToolMarkup(message.Content))
            {
                return false;
            }

            return true;
        }

        private static LanguageModelChatRole ResolveRole(
            ChatMessageModel sourceMessage,
            ChatMessagePartModel part)
        {
            if (part.PartType is ChatMessagePartType.ToolOutput or ChatMessagePartType.Tool)
            {
                return LanguageModelChatRole.User;
            }

            return sourceMessage.Role switch
            {
                ChatMessageRole.System => LanguageModelChatRole.System,
                ChatMessageRole.Assistant => LanguageModelChatRole.Assistant,
                _ => LanguageModelChatRole.User
            };
        }

        private static string BuildMessageContent(ChatMessagePartModel part)
        {
            ArgumentNullException.ThrowIfNull(part);

            var content = NormalizeContent(part.Content);
            if (content.Length == 0)
            {
                return string.Empty;
            }

            return part.PartType switch
            {
                ChatMessagePartType.StructuredXml => content,
                ChatMessagePartType.ToolCall => EnsureToolsWrapper(content),
                ChatMessagePartType.ToolOutput or ChatMessagePartType.Tool => content,
                ChatMessagePartType.Code => BuildCodeBlock(part, content),
                _ => PrefixTitle(part.Title, content)
            };
        }

        private static string BuildCodeBlock(ChatMessagePartModel part, string content)
        {
            var fence = string.IsNullOrWhiteSpace(part.Language)
                ? "```"
                : $"```{part.Language.Trim()}";

            return PrefixTitle(
                part.Title,
                $"{fence}{Environment.NewLine}{content}{Environment.NewLine}```");
        }

        private static string PrefixTitle(string? title, string content)
        {
            var normalizedContent = NormalizeContent(content);
            if (normalizedContent.Length == 0)
            {
                return string.Empty;
            }

            var normalizedTitle = NormalizeContent(title);
            return normalizedTitle.Length == 0
                ? normalizedContent
                : $"{normalizedTitle}{Environment.NewLine}{normalizedContent}";
        }

        private static string NormalizeContent(string? content)
        {
            return string.IsNullOrWhiteSpace(content)
                ? string.Empty
                : content.Trim();
        }

        private static string EnsureToolsWrapper(string content)
        {
            var normalized = NormalizeContent(content);
            if (normalized.Length == 0)
            {
                return string.Empty;
            }

            if (normalized.StartsWith("<Tools", StringComparison.OrdinalIgnoreCase))
            {
                return normalized;
            }

            return normalized.StartsWith("<Tool", StringComparison.OrdinalIgnoreCase)
                ? $"<Tools>{normalized}</Tools>"
                : normalized;
        }

        private static string? ResolveAuthorName(
            ChatMessageModel message,
            ChatMessagePartModel part,
            LanguageModelChatRole role)
        {
            if (part.PartType is ChatMessagePartType.ToolOutput or ChatMessagePartType.Tool)
            {
                return ResolveToolAuthorName(part);
            }

            if (role != LanguageModelChatRole.Assistant)
            {
                return null;
            }

            var displayName = NormalizeContent(message.DisplayName);
            return displayName.Length == 0 ? null : displayName;
        }

        private static string? ResolveToolAuthorName(ChatMessagePartModel part)
        {
            var title = NormalizeContent(part.Title);
            if (title.Length > 0 && !title.StartsWith("Tool #", StringComparison.OrdinalIgnoreCase))
            {
                return title;
            }

            if (!TryExtractToolNameFromToolsReturn(part.Content, out var toolName))
            {
                return null;
            }

            return toolName;
        }

        private static bool TryExtractToolNameFromToolsReturn(string? xml, out string? toolName)
        {
            toolName = null;
            if (string.IsNullOrWhiteSpace(xml))
            {
                return false;
            }

            try
            {
                var document = XDocument.Parse(xml, LoadOptions.PreserveWhitespace);
                var toolReturn = document.Root?
                    .Elements()
                    .FirstOrDefault(element => string.Equals(element.Name.LocalName, "ToolReturn", StringComparison.OrdinalIgnoreCase));
                if (toolReturn == null)
                {
                    return false;
                }

                toolName = toolReturn.Attributes()
                    .FirstOrDefault(attribute => string.Equals(attribute.Name.LocalName, "ToolName", StringComparison.OrdinalIgnoreCase))
                    ?.Value
                    ?.Trim();

                return !string.IsNullOrWhiteSpace(toolName);
            }
            catch
            {
                return false;
            }
        }
    }
}
