namespace Skyweaver.Models.ChatSession
{
    public sealed class ChatSessionContentBlockModel
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N");

        public ChatSessionContentBlockKind Kind { get; set; } = ChatSessionContentBlockKind.Text;

        public string Content { get; set; } = string.Empty;

        public string? Title { get; set; }

        public string? Language { get; set; }

        public string? BadgeText { get; set; }

        public bool IsStreaming { get; set; }

        public string? ToolCallId { get; set; }

        public string? CallerAgentId { get; set; }

        public string? ResourcePath { get; set; }

        public Dictionary<string, string> Metadata { get; } = new(StringComparer.OrdinalIgnoreCase);
    }
}
