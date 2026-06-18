using Ferrita.Infrastructure.Mvvm;
using Ferrita.Panels.DocumentWorkspace.ViewModels;
using Ferrita.Panels.LiveTasks.ViewModels;
using Ferrita.Panels.MultiFunctionArea.ViewModels;
using Ferrita.Panels.SessionList.ViewModels;

namespace Ferrita.PageControls.Session.ViewModels
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
