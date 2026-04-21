using Skyweaver.Infrastructure.Mvvm;
using Skyweaver.Services.ChatSession;
using Skyweaver.Panels.DocumentWorkspace.ViewModels;
using Skyweaver.Panels.FileExplorer.ViewModels;
using Skyweaver.Panels.Filmstrip.ViewModels;
using Skyweaver.Panels.MultiFunctionArea.ViewModels;
using Skyweaver.Panels.SessionList.ViewModels;

namespace Skyweaver.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        private readonly ChatSessionRepository _chatSessionRepository;

        public SessionListPanelViewModel SessionListPanel { get; }

        public FileExplorerPanelViewModel FileExplorerPanel { get; }

        public DocumentWorkspacePanelViewModel DocumentWorkspacePanel { get; }

        public FilmstripPanelViewModel FilmstripPanel { get; }

        public MultiFunctionAreaPanelViewModel MultiFunctionAreaPanel { get; }

        public SessionWorkspaceBridgeViewModel SessionWorkspaceBridge { get; }

        public MainViewModel()
        {
            _chatSessionRepository = new ChatSessionRepository();
            DocumentWorkspacePanel = new DocumentWorkspacePanelViewModel();
            SessionWorkspaceBridge = new SessionWorkspaceBridgeViewModel(DocumentWorkspacePanel, _chatSessionRepository);

            SessionListPanel = new SessionListPanelViewModel(SessionWorkspaceBridge.OpenSession, _chatSessionRepository);
            FileExplorerPanel = new FileExplorerPanelViewModel();
            FilmstripPanel = new FilmstripPanelViewModel();
            MultiFunctionAreaPanel = new MultiFunctionAreaPanelViewModel();
        }
    }
}
