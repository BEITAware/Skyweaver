using Skyweaver.Controls.ChatSessionControl.Models;
using Skyweaver.Models.ChatSession;

namespace Skyweaver.Services.ChatSession
{
    public static class ChatSessionPresentationProjector
    {
        public static ChatSessionMessageRecordModel ToRecord(ChatMessageModel message)
        {
            ArgumentNullException.ThrowIfNull(message);

            var record = new ChatSessionMessageRecordModel
            {
                Id = message.Id.ToString("N"),
                Role = message.Role,
                DisplayName = message.DisplayName,
                AvatarPath = message.AvatarPath,
                TimestampUtc = message.Timestamp.ToUniversalTime()
            };

            foreach (var part in message.Parts)
            {
                record.Blocks.Add(ToRecordBlock(part));
            }

            return record;
        }

        public static ChatMessageModel ToPresentationMessage(ChatSessionMessageRecordModel record)
        {
            ArgumentNullException.ThrowIfNull(record);

            return new ChatMessageModel(
                record.Role,
                record.DisplayName,
                record.AvatarPath,
                record.TimestampUtc.ToLocalTime(),
                record.Blocks.Select(ToPresentationPart));
        }

        public static ChatSessionContentBlockModel ToRecordBlock(ChatMessagePartModel part)
        {
            ArgumentNullException.ThrowIfNull(part);

            return new ChatSessionContentBlockModel
            {
                Kind = ToBlockKind(part.PartType),
                Content = part.Content,
                Title = part.Title,
                Language = part.Language,
                BadgeText = part.BadgeText,
                IsStreaming = part.IsStreaming,
                ToolCallId = part.ToolCallId,
                CallerAgentId = part.CallerAgentId,
                ResourcePath = part.PartType is ChatMessagePartType.Image or ChatMessagePartType.Audio
                    ? part.ResourcePath ?? part.Content
                    : part.ResourcePath
            };
        }

        public static ChatMessagePartModel ToPresentationPart(ChatSessionContentBlockModel block)
        {
            ArgumentNullException.ThrowIfNull(block);

            return new ChatMessagePartModel(
                ToPartType(block.Kind),
                block.Content,
                block.Title,
                block.Language,
                block.BadgeText,
                block.IsStreaming,
                block.ToolCallId,
                block.CallerAgentId,
                block.Kind is ChatSessionContentBlockKind.Image or ChatSessionContentBlockKind.Audio
                    ? block.ResourcePath ?? block.Content
                    : block.ResourcePath,
                IsUserVisible(block));
        }

        private static bool IsUserVisible(ChatSessionContentBlockModel block)
        {
            if (block.Kind is ChatSessionContentBlockKind.ToolOutput or ChatSessionContentBlockKind.ToolReference)
            {
                return string.Equals(block.Title, "Tool Parse Error", StringComparison.Ordinal) ||
                       string.Equals(block.Title, "工具解析错误", StringComparison.Ordinal);
            }

            return true;
        }

        private static ChatSessionContentBlockKind ToBlockKind(ChatMessagePartType partType)
        {
            return partType switch
            {
                ChatMessagePartType.Code => ChatSessionContentBlockKind.Code,
                ChatMessagePartType.Status => ChatSessionContentBlockKind.Status,
                ChatMessagePartType.Placeholder => ChatSessionContentBlockKind.Placeholder,
                ChatMessagePartType.Tool or ChatMessagePartType.ToolOutput => ChatSessionContentBlockKind.ToolOutput,
                ChatMessagePartType.ToolCall => ChatSessionContentBlockKind.ToolCall,
                ChatMessagePartType.StructuredXml => ChatSessionContentBlockKind.StructuredXml,
                ChatMessagePartType.Image => ChatSessionContentBlockKind.Image,
                ChatMessagePartType.Audio => ChatSessionContentBlockKind.Audio,
                ChatMessagePartType.HostPreservedContent => ChatSessionContentBlockKind.HostPreservedContent,
                ChatMessagePartType.Reasoning => ChatSessionContentBlockKind.Reasoning,
                _ => ChatSessionContentBlockKind.Text
            };
        }

        private static ChatMessagePartType ToPartType(ChatSessionContentBlockKind blockKind)
        {
            return blockKind switch
            {
                ChatSessionContentBlockKind.Code => ChatMessagePartType.Code,
                ChatSessionContentBlockKind.Status => ChatMessagePartType.Status,
                ChatSessionContentBlockKind.Placeholder => ChatMessagePartType.Placeholder,
                ChatSessionContentBlockKind.ToolCall => ChatMessagePartType.ToolCall,
                ChatSessionContentBlockKind.ToolOutput or ChatSessionContentBlockKind.ToolReference => ChatMessagePartType.ToolOutput,
                ChatSessionContentBlockKind.StructuredXml => ChatMessagePartType.StructuredXml,
                ChatSessionContentBlockKind.Image => ChatMessagePartType.Image,
                ChatSessionContentBlockKind.Audio => ChatMessagePartType.Audio,
                ChatSessionContentBlockKind.HostPreservedContent => ChatMessagePartType.HostPreservedContent,
                ChatSessionContentBlockKind.Reasoning => ChatMessagePartType.Reasoning,
                _ => ChatMessagePartType.Text
            };
        }
    }
}
