using Skyweaver.Controls.ChatSessionControl.Models;

namespace Skyweaver.Models.ChatSession
{
    public sealed class ChatSessionMessageRecordModel
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N");

        public ChatMessageRole Role { get; set; } = ChatMessageRole.User;

        public string DisplayName { get; set; } = string.Empty;

        public string AvatarPath { get; set; } = string.Empty;

        public DateTime TimestampUtc { get; set; } = DateTime.UtcNow;

        public List<ChatSessionContentBlockModel> Blocks { get; } = new();
    }
}
