using Ferrita.Infrastructure.Mvvm;
using Ferrita.Services.Localization;

namespace Ferrita.Panels.ChatSession.ViewModels
{
    public sealed class ChatSessionPanelViewModel : ObservableObject
    {
        public string PanelTitle { get; } = L("ChatSessionPanel.Title", "会话面板");

        public string Description { get; } = L("ChatSessionPanel.Description", "ChatSession 已从右侧面板提取为独立控件，并迁移到 DocumentWorkspace 标签页中承载。");

        public string Hint { get; } = L("ChatSessionPanel.Hint", "在左侧 SessionList 中双击任意会话，即可在中间工作区打开一个新的 ChatSession 标签页。");

        private static string L(string resourceKey, string fallback)
        {
            return LocalizationRuntime.Instance.GetString(resourceKey, fallback);
        }
    }
}
