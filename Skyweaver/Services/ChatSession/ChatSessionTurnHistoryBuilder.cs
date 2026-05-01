using System.Xml.Linq;
using Skyweaver.Controls.ChatSessionControl.Models;
using Skyweaver.Controls.LanguageModelConfigurationControl.Services;
using Skyweaver.Models.ChatSession;
using Skyweaver.Services.SkyweaverTools;
using Skyweaver.Tools;

namespace Skyweaver.Services.ChatSession
{
    public static class ChatSessionTurnHistoryBuilder
    {
        private static readonly ChatSessionToolCallResourceStore s_toolCallResourceStore = new();

        public static IReadOnlyList<LanguageModelChatMessage> BuildForNextTurn(
            ChatSessionModel session,
            string? currentUserText,
            IReadOnlyList<LanguageModelChatContentBlock>? currentUserContentBlocks = null)
        {
            ArgumentNullException.ThrowIfNull(session);

            if (session.Records.Count > 0)
            {
                return BuildFromRecords(session, session.Records, currentUserText, currentUserContentBlocks);
            }

            if (session.Messages.Count > 0)
            {
                return BuildFromMessages(session.Messages, currentUserText, currentUserContentBlocks);
            }

            return BuildFromStoredHistory(session.ConversationHistory, currentUserText, currentUserContentBlocks);
        }

        public static IReadOnlyList<LanguageModelChatMessage> BuildFromRecords(
            ChatSessionModel session,
            IEnumerable<ChatSessionMessageRecordModel> records,
            string? currentUserText = null,
            IReadOnlyList<LanguageModelChatContentBlock>? currentUserContentBlocks = null)
        {
            ArgumentNullException.ThrowIfNull(session);
            ArgumentNullException.ThrowIfNull(records);

            var materializedRecords = records.ToArray();
            var effectiveRecordCount = materializedRecords.Length;
            if (effectiveRecordCount > 0 &&
                IsCurrentTurnUserMessage(
                    session,
                    materializedRecords[effectiveRecordCount - 1],
                    currentUserText,
                    currentUserContentBlocks))
            {
                effectiveRecordCount--;
            }

            var history = new List<LanguageModelChatMessage>();
            for (var index = 0; index < effectiveRecordCount; index++)
            {
                AppendProjectedRecord(session, history, materializedRecords[index]);
            }

            return history;
        }

        public static IReadOnlyList<LanguageModelChatMessage> BuildFromMessages(
            IEnumerable<ChatMessageModel> messages,
            string? currentUserText = null,
            IReadOnlyList<LanguageModelChatContentBlock>? currentUserContentBlocks = null)
        {
            ArgumentNullException.ThrowIfNull(messages);

            var materializedMessages = messages.ToArray();
            var effectiveMessageCount = materializedMessages.Length;
            if (effectiveMessageCount > 0 &&
                IsCurrentTurnUserMessage(
                    materializedMessages[effectiveMessageCount - 1],
                    currentUserText,
                    currentUserContentBlocks))
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
            string? currentUserText,
            IReadOnlyList<LanguageModelChatContentBlock>? currentUserContentBlocks)
        {
            var effectiveMessageCount = storedHistory.Count;
            if (effectiveMessageCount > 0 &&
                IsCurrentTurnUserMessage(
                    storedHistory[effectiveMessageCount - 1],
                    currentUserText,
                    currentUserContentBlocks))
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

        private static void AppendProjectedRecord(
            ChatSessionModel session,
            ICollection<LanguageModelChatMessage> history,
            ChatSessionMessageRecordModel record)
        {
            ArgumentNullException.ThrowIfNull(session);
            ArgumentNullException.ThrowIfNull(history);
            ArgumentNullException.ThrowIfNull(record);

            if (record.Role is not (ChatMessageRole.User or ChatMessageRole.Assistant))
            {
                return;
            }

            var suppressAssistantText = record.Role == ChatMessageRole.Assistant &&
                                        RecordContainsToolActivity(record);
            foreach (var block in record.Blocks)
            {
                if (!TryProjectBlock(session, record, block, suppressAssistantText, out var projectedMessage) ||
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

            var role = ResolveRole(sourceMessage, part);
            if (TryCreateLanguageModelContentBlock(part, out var contentBlock) &&
                contentBlock != null)
            {
                projectedMessage = new LanguageModelChatMessage(role, [contentBlock])
                {
                    AuthorName = ResolveAuthorName(sourceMessage, part, role)
                };
                return true;
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

            if (role == LanguageModelChatRole.Assistant &&
                !suppressAssistantText &&
                part.PartType is ChatMessagePartType.Text or ChatMessagePartType.Code or ChatMessagePartType.StructuredXml)
            {
                content = WrapInAgentToolsXml(content);
            }

            projectedMessage = new LanguageModelChatMessage(role, content)
            {
                AuthorName = ResolveAuthorName(sourceMessage, part, role)
            };
            return true;
        }

        private static bool TryProjectBlock(
            ChatSessionModel session,
            ChatSessionMessageRecordModel sourceMessage,
            ChatSessionContentBlockModel block,
            bool suppressAssistantText,
            out LanguageModelChatMessage? projectedMessage)
        {
            projectedMessage = null;
            if (!ShouldIncludeBlockInNextTurnHistory(block))
            {
                return false;
            }

            if (suppressAssistantText &&
                block.Kind is ChatSessionContentBlockKind.Text or ChatSessionContentBlockKind.Code or ChatSessionContentBlockKind.StructuredXml)
            {
                return false;
            }

            var role = ResolveRole(sourceMessage, block);
            if (TryCreateLanguageModelContentBlock(block, out var contentBlock) &&
                contentBlock != null)
            {
                projectedMessage = new LanguageModelChatMessage(role, [contentBlock])
                {
                    AuthorName = ResolveAuthorName(sourceMessage, block, role)
                };
                return true;
            }

            var content = BuildMessageContent(session, block);
            if (content.Length == 0)
            {
                return false;
            }

            if (sourceMessage.Role == ChatMessageRole.Assistant &&
                SkyweaverToolSyntaxInspector.ContainsInvalidPseudoToolMarkup(content))
            {
                return false;
            }

            if (role == LanguageModelChatRole.Assistant &&
                !suppressAssistantText &&
                block.Kind is ChatSessionContentBlockKind.Text or ChatSessionContentBlockKind.Code or ChatSessionContentBlockKind.StructuredXml)
            {
                content = WrapInAgentToolsXml(content);
            }

            projectedMessage = new LanguageModelChatMessage(role, content)
            {
                AuthorName = ResolveAuthorName(sourceMessage, block, role)
            };
            return true;
        }

        private static bool TryCreateLanguageModelContentBlock(
            ChatMessagePartModel part,
            out LanguageModelChatContentBlock? contentBlock)
        {
            contentBlock = null;
            var resourcePath = NormalizeContent(part.ResourcePath ?? part.Content);
            switch (part.PartType)
            {
                case ChatMessagePartType.Image:
                    if (resourcePath.Length == 0)
                    {
                        return false;
                    }

                    contentBlock = LanguageModelChatContentBlock.CreateImage(resourcePath);
                    return true;

                case ChatMessagePartType.Audio:
                    if (resourcePath.Length == 0)
                    {
                        return false;
                    }

                    contentBlock = LanguageModelChatContentBlock.CreateAudio(resourcePath);
                    return true;

                case ChatMessagePartType.HostPreservedContent:
                    var content = NormalizeContent(part.Content);
                    if (content.Length == 0)
                    {
                        return false;
                    }

                    contentBlock = LanguageModelChatContentBlock.CreateHostPreservedContent(content);
                    return true;

                default:
                    return false;
            }
        }

        private static bool TryCreateLanguageModelContentBlock(
            ChatSessionContentBlockModel block,
            out LanguageModelChatContentBlock? contentBlock)
        {
            contentBlock = null;
            var resourcePath = NormalizeContent(block.ResourcePath ?? block.Content);
            var mediaType = block.Metadata.TryGetValue("MediaType", out var savedMediaType)
                ? savedMediaType
                : null;

            switch (block.Kind)
            {
                case ChatSessionContentBlockKind.Image:
                    if (resourcePath.Length == 0)
                    {
                        return false;
                    }

                    contentBlock = LanguageModelChatContentBlock.CreateImage(resourcePath, mediaType);
                    return true;

                case ChatSessionContentBlockKind.Audio:
                    if (resourcePath.Length == 0)
                    {
                        return false;
                    }

                    contentBlock = LanguageModelChatContentBlock.CreateAudio(resourcePath, mediaType);
                    return true;

                case ChatSessionContentBlockKind.HostPreservedContent:
                    var content = NormalizeContent(block.Content);
                    if (content.Length == 0)
                    {
                        return false;
                    }

                    contentBlock = LanguageModelChatContentBlock.CreateHostPreservedContent(content);
                    return true;

                default:
                    return false;
            }
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

        private static bool RecordContainsToolActivity(ChatSessionMessageRecordModel record)
        {
            return record.Blocks.Any(block =>
                block.Kind is
                    ChatSessionContentBlockKind.ToolCall or
                    ChatSessionContentBlockKind.ToolOutput or
                    ChatSessionContentBlockKind.ToolReference);
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
                or ChatMessagePartType.Tool
                or ChatMessagePartType.Image
                or ChatMessagePartType.Audio
                or ChatMessagePartType.HostPreservedContent;
        }

        private static bool ShouldIncludeBlockInNextTurnHistory(ChatSessionContentBlockModel block)
        {
            ArgumentNullException.ThrowIfNull(block);

            return block.Kind is ChatSessionContentBlockKind.Text
                or ChatSessionContentBlockKind.Code
                or ChatSessionContentBlockKind.StructuredXml
                or ChatSessionContentBlockKind.ToolCall
                or ChatSessionContentBlockKind.ToolOutput
                or ChatSessionContentBlockKind.ToolReference
                or ChatSessionContentBlockKind.Image
                or ChatSessionContentBlockKind.Audio
                or ChatSessionContentBlockKind.HostPreservedContent;
        }

        private static bool IsCurrentTurnUserMessage(
            ChatMessageModel message,
            string? currentUserText,
            IReadOnlyList<LanguageModelChatContentBlock>? currentUserContentBlocks)
        {
            if (message.Role != ChatMessageRole.User)
            {
                return false;
            }

            var normalizedCurrentUserText = NormalizeContent(currentUserText);
            var currentResources = NormalizeResourceKeys(currentUserContentBlocks);
            if (normalizedCurrentUserText.Length == 0 && currentResources.Count == 0)
            {
                return false;
            }

            var normalizedMessageText = string.Join(
                    Environment.NewLine + Environment.NewLine,
                    message.Parts
                        .Where(IsTextLikeUserPart)
                        .Where(part => !ChatSessionFinishTaskVisibility.IsInternalToolPart(part))
                        .Select(BuildMessageContent)
                        .Where(content => content.Length > 0))
                .Trim();

            return string.Equals(normalizedMessageText, normalizedCurrentUserText, StringComparison.Ordinal) &&
                   ResourceKeysEqual(NormalizeResourceKeys(message.Parts), currentResources);
        }

        private static bool IsCurrentTurnUserMessage(
            LanguageModelChatMessage message,
            string? currentUserText,
            IReadOnlyList<LanguageModelChatContentBlock>? currentUserContentBlocks)
        {
            if (message.Role != LanguageModelChatRole.User)
            {
                return false;
            }

            var normalizedCurrentUserText = NormalizeContent(currentUserText);
            var currentResources = NormalizeResourceKeys(currentUserContentBlocks);
            if (normalizedCurrentUserText.Length == 0 && currentResources.Count == 0)
            {
                return false;
            }

            var normalizedMessageText = string.Join(
                    Environment.NewLine + Environment.NewLine,
                    message.ContentBlocks
                        .Where(block => block.Kind == LanguageModelChatContentBlockKind.Text)
                        .Select(block => NormalizeContent(block.Content))
                        .Where(content => content.Length > 0))
                .Trim();

            return string.Equals(normalizedMessageText, normalizedCurrentUserText, StringComparison.Ordinal) &&
                   ResourceKeysEqual(NormalizeResourceKeys(message.ContentBlocks), currentResources);
        }

        private static bool IsCurrentTurnUserMessage(
            ChatSessionModel session,
            ChatSessionMessageRecordModel record,
            string? currentUserText,
            IReadOnlyList<LanguageModelChatContentBlock>? currentUserContentBlocks)
        {
            if (record.Role != ChatMessageRole.User)
            {
                return false;
            }

            var normalizedCurrentUserText = NormalizeContent(currentUserText);
            var currentResources = NormalizeResourceKeys(currentUserContentBlocks);
            if (normalizedCurrentUserText.Length == 0 && currentResources.Count == 0)
            {
                return false;
            }

            var normalizedMessageText = string.Join(
                    Environment.NewLine + Environment.NewLine,
                    record.Blocks
                        .Where(IsTextLikeUserBlock)
                        .Select(block => BuildMessageContent(session, block))
                        .Where(content => content.Length > 0))
                .Trim();

            return string.Equals(normalizedMessageText, normalizedCurrentUserText, StringComparison.Ordinal) &&
                   ResourceKeysEqual(NormalizeResourceKeys(record.Blocks), currentResources);
        }

        private static bool IsTextLikeUserPart(ChatMessagePartModel part)
        {
            return part.PartType is ChatMessagePartType.Text
                or ChatMessagePartType.Code
                or ChatMessagePartType.StructuredXml;
        }

        private static bool IsTextLikeUserBlock(ChatSessionContentBlockModel block)
        {
            return block.Kind is ChatSessionContentBlockKind.Text
                or ChatSessionContentBlockKind.Code
                or ChatSessionContentBlockKind.StructuredXml;
        }

        private static IReadOnlyList<string> NormalizeResourceKeys(
            IEnumerable<LanguageModelChatContentBlock>? blocks)
        {
            if (blocks == null)
            {
                return Array.Empty<string>();
            }

            return blocks
                .Select(ToResourceKey)
                .Where(key => key.Length > 0)
                .OrderBy(key => key, StringComparer.Ordinal)
                .ToArray();
        }

        private static IReadOnlyList<string> NormalizeResourceKeys(
            IEnumerable<ChatMessagePartModel>? parts)
        {
            if (parts == null)
            {
                return Array.Empty<string>();
            }

            return parts
                .Where(part => part != null)
                .Select(ToResourceKey)
                .Where(key => key.Length > 0)
                .OrderBy(key => key, StringComparer.Ordinal)
                .ToArray();
        }

        private static IReadOnlyList<string> NormalizeResourceKeys(
            IEnumerable<ChatSessionContentBlockModel>? blocks)
        {
            if (blocks == null)
            {
                return Array.Empty<string>();
            }

            return blocks
                .Where(block => block != null)
                .Select(ToResourceKey)
                .Where(key => key.Length > 0)
                .OrderBy(key => key, StringComparer.Ordinal)
                .ToArray();
        }

        private static string ToResourceKey(LanguageModelChatContentBlock block)
        {
            return block.Kind switch
            {
                LanguageModelChatContentBlockKind.Image => BuildResourceKey("image", block.ResourcePath ?? block.Content),
                LanguageModelChatContentBlockKind.Audio => BuildResourceKey("audio", block.ResourcePath ?? block.Content),
                LanguageModelChatContentBlockKind.HostPreservedContent => BuildResourceKey("host", block.Content),
                _ => string.Empty
            };
        }

        private static string ToResourceKey(ChatMessagePartModel part)
        {
            return part.PartType switch
            {
                ChatMessagePartType.Image => BuildResourceKey("image", part.ResourcePath ?? part.Content),
                ChatMessagePartType.Audio => BuildResourceKey("audio", part.ResourcePath ?? part.Content),
                ChatMessagePartType.HostPreservedContent => BuildResourceKey("host", part.Content),
                _ => string.Empty
            };
        }

        private static string ToResourceKey(ChatSessionContentBlockModel block)
        {
            return block.Kind switch
            {
                ChatSessionContentBlockKind.Image => BuildResourceKey("image", block.ResourcePath ?? block.Content),
                ChatSessionContentBlockKind.Audio => BuildResourceKey("audio", block.ResourcePath ?? block.Content),
                ChatSessionContentBlockKind.HostPreservedContent => BuildResourceKey("host", block.Content),
                _ => string.Empty
            };
        }

        private static string BuildResourceKey(string kind, string? value)
        {
            var normalizedValue = NormalizeContent(value);
            return normalizedValue.Length == 0
                ? string.Empty
                : $"{kind}:{normalizedValue}";
        }

        private static bool ResourceKeysEqual(IReadOnlyList<string> left, IReadOnlyList<string> right)
        {
            return left.Count == right.Count && left.SequenceEqual(right, StringComparer.Ordinal);
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

        private static LanguageModelChatRole ResolveRole(
            ChatSessionMessageRecordModel sourceMessage,
            ChatSessionContentBlockModel block)
        {
            if (block.Kind is ChatSessionContentBlockKind.ToolOutput or ChatSessionContentBlockKind.ToolReference)
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

        private static string BuildMessageContent(
            ChatSessionModel session,
            ChatSessionContentBlockModel block)
        {
            ArgumentNullException.ThrowIfNull(block);

            var content = block.Kind switch
            {
                ChatSessionContentBlockKind.ToolCall => s_toolCallResourceStore.LoadInvocation(session, block.ToolCallId),
                ChatSessionContentBlockKind.ToolOutput or ChatSessionContentBlockKind.ToolReference => s_toolCallResourceStore.LoadOutput(session, block.ToolCallId),
                ChatSessionContentBlockKind.Image => BuildPreservedResourceXml("Image", block.ResourcePath ?? block.Content),
                ChatSessionContentBlockKind.Audio => BuildPreservedResourceXml("Audio", block.ResourcePath ?? block.Content),
                ChatSessionContentBlockKind.HostPreservedContent => EnsurePreservedContentWrapper(block.Content),
                _ => NormalizeContent(block.Content)
            };

            if (content.Length == 0)
            {
                return string.Empty;
            }

            return block.Kind switch
            {
                ChatSessionContentBlockKind.StructuredXml => content,
                ChatSessionContentBlockKind.ToolCall => EnsureToolsWrapper(content),
                ChatSessionContentBlockKind.ToolOutput or ChatSessionContentBlockKind.ToolReference => content,
                ChatSessionContentBlockKind.Code => BuildCodeBlock(block, content),
                ChatSessionContentBlockKind.Image or ChatSessionContentBlockKind.Audio or ChatSessionContentBlockKind.HostPreservedContent => content,
                _ => PrefixTitle(block.Title, content)
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

        private static string BuildCodeBlock(ChatSessionContentBlockModel block, string content)
        {
            var fence = string.IsNullOrWhiteSpace(block.Language)
                ? "```"
                : $"```{block.Language.Trim()}";

            return PrefixTitle(
                block.Title,
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

        private static string WrapInAgentToolsXml(string content)
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

            var toolsElement = new XElement("Tools",
                new XElement("Tool",
                    new XAttribute("ToolName", CreateMessageTool.ToolName),
                    normalized));
            return toolsElement.ToString(SaveOptions.DisableFormatting);
        }

        private static string BuildPreservedResourceXml(string elementName, string? path)
        {
            var normalizedPath = NormalizeContent(path);
            if (normalizedPath.Length == 0)
            {
                return string.Empty;
            }

            return $"<SkyweaverPreservedContent><{elementName} Path=\"{System.Security.SecurityElement.Escape(normalizedPath)}\" /></SkyweaverPreservedContent>";
        }

        private static string EnsurePreservedContentWrapper(string? content)
        {
            var normalized = NormalizeContent(content);
            if (normalized.Length == 0)
            {
                return string.Empty;
            }

            return normalized.StartsWith("<SkyweaverPreservedContent", StringComparison.OrdinalIgnoreCase)
                ? normalized
                : $"<SkyweaverPreservedContent>{normalized}</SkyweaverPreservedContent>";
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

        private static string? ResolveAuthorName(
            ChatSessionMessageRecordModel message,
            ChatSessionContentBlockModel block,
            LanguageModelChatRole role)
        {
            if (block.Kind is ChatSessionContentBlockKind.ToolOutput or ChatSessionContentBlockKind.ToolReference)
            {
                return ResolveToolAuthorName(block);
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

        private static string? ResolveToolAuthorName(ChatSessionContentBlockModel block)
        {
            var title = NormalizeContent(block.Title);
            if (title.Length > 0 && !title.StartsWith("Tool #", StringComparison.OrdinalIgnoreCase))
            {
                return title;
            }

            var output = NormalizeContent(block.Content);
            if (output.Length == 0)
            {
                return null;
            }

            return TryExtractToolNameFromToolsReturn(output, out var toolName)
                ? toolName
                : null;
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
