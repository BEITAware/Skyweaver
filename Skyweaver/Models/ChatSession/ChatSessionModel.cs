using Skyweaver.Controls.ChatSessionControl.Models;
using Skyweaver.Controls.LanguageModelConfigurationControl.Services;

namespace Skyweaver.Models.ChatSession
{
    public sealed class ChatSessionModel
    {
        public string SessionId { get; set; } = Guid.NewGuid().ToString("N");

        public string Name { get; set; } = string.Empty;

        public string IconPath { get; set; } = "pack://application:,,,/Resources/NewNodeGraphAlt.png";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public string ContextSummary { get; set; } = string.Empty;

        public string MetadataNote { get; set; } = string.Empty;

        public ChatSessionFlowBinding FlowBinding { get; } = new();

        public List<ChatMessageModel> Messages { get; } = new();

        public List<LanguageModelChatMessage> ConversationHistory { get; } = new();

        public string SessionFolderPath { get; set; } = string.Empty;

        public string SessionFilePath { get; set; } = string.Empty;

        public string ResourcesFolderPath { get; set; } = string.Empty;

        public bool HasBoundFlow => FlowBinding.IsBound;

        public string BoundFlowDisplayName => string.IsNullOrWhiteSpace(FlowBinding.GraphName)
            ? "未绑定会话流"
            : FlowBinding.GraphName;
    }
}
