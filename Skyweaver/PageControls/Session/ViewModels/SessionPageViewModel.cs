using Skyweaver.Infrastructure.Mvvm;
using Skyweaver.Panels.DocumentWorkspace.ViewModels;
using Skyweaver.Panels.LiveTasks.ViewModels;
using Skyweaver.Panels.MultiFunctionArea.ViewModels;
using Skyweaver.Panels.SessionList.ViewModels;

namespace Skyweaver.PageControls.Session.ViewModels
{
    /// <summary>
    /// 会话页面的 ViewModel，负责管理会话区域下的各个子面板。
    /// </summary>
    public class SessionPageViewModel : ObservableObject
    {
        public SessionListPanelViewModel SessionListPanel { get; }
        public LiveTasksPanelViewModel LiveTasksPanel { get; }
        public DocumentWorkspacePanelViewModel DocumentWorkspacePanel { get; }
        public MultiFunctionAreaPanelViewModel MultiFunctionAreaPanel { get; }

        public SessionPageViewModel(
            SessionListPanelViewModel sessionListPanel,
            LiveTasksPanelViewModel liveTasksPanel,
            DocumentWorkspacePanelViewModel documentWorkspacePanel,
            MultiFunctionAreaPanelViewModel multiFunctionAreaPanel)
        {
            SessionListPanel = sessionListPanel;
            LiveTasksPanel = liveTasksPanel;
            DocumentWorkspacePanel = documentWorkspacePanel;
            MultiFunctionAreaPanel = multiFunctionAreaPanel;
        }
    }
}
