using Skyweaver.Infrastructure.Mvvm;

namespace Skyweaver.Panels.ChatSession.ViewModels
{
    public sealed class ChatSessionPanelViewModel : ObservableObject
    {
        public string PanelTitle { get; } = "会话面板";

        public string Description { get; } = "ChatSession 已从右侧面板提取为独立控件，并迁移到 DocumentWorkspace 标签页中承载。";

        public string Hint { get; } = "在左侧 SessionList 中双击任意会话，即可在中间工作区打开一个新的 ChatSession 标签页。";
    }
}
