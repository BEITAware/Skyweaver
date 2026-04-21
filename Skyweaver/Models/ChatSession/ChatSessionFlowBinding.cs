namespace Skyweaver.Models.ChatSession
{
    public sealed class ChatSessionFlowBinding
    {
        public string GraphId { get; set; } = string.Empty;

        public string GraphName { get; set; } = string.Empty;

        public string FilePath { get; set; } = string.Empty;

        public bool IsBound =>
            !string.IsNullOrWhiteSpace(GraphId) ||
            !string.IsNullOrWhiteSpace(GraphName) ||
            !string.IsNullOrWhiteSpace(FilePath);

        public ChatSessionFlowBinding DeepClone()
        {
            return new ChatSessionFlowBinding
            {
                GraphId = GraphId,
                GraphName = GraphName,
                FilePath = FilePath
            };
        }
    }
}
