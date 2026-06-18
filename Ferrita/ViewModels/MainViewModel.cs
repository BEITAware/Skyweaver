using Ferrita.Controls.ChatSessionControl.ViewModels;
using Ferrita.Infrastructure.Mvvm;
using Ferrita.Services.ChatSession;
using Ferrita.Panels.DocumentWorkspace.ViewModels;
using Ferrita.Panels.LiveTasks.ViewModels;
using Ferrita.Panels.Filmstrip.ViewModels;
using Ferrita.Panels.MultiFunctionArea.ViewModels;
using Ferrita.Panels.SessionList.ViewModels;
using Ferrita.Services.Daemon;

namespace Ferrita.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        private readonly ChatSessionRepository _chatSessionRepository;

        public Ferrita.Services.Notifications.NotificationService NotificationService => Ferrita.Services.Notifications.NotificationService.Instance;

        public SessionListPanelViewModel SessionListPanel { get; }

        public LiveTasksPanelViewModel LiveTasksPanel { get; }

        public DocumentWorkspacePanelViewModel DocumentWorkspacePanel { get; }

        public FilmstripPanelViewModel FilmstripPanel { get; }

        public MultiFunctionAreaPanelViewModel MultiFunctionAreaPanel { get; }

        public SessionWorkspaceBridgeViewModel SessionWorkspaceBridge { get; }

        public Ferrita.PageControls.Session.ViewModels.SessionPageViewModel SessionPage { get; }
        public Ferrita.PageControls.Desk.ViewModels.DeskPageViewModel DeskPage { get; }
        public Ferrita.PageControls.Marvelous.ViewModels.MarvelousPageViewModel MarvelousPage { get; }
        public Ferrita.PageControls.Tiles.ViewModels.TilesPageViewModel TilesPage { get; }

        private object _currentPageViewModel;
        public object CurrentPageViewModel
        {
            get => _currentPageViewModel;
            set => SetProperty(ref _currentPageViewModel, value);
        }

        private int _selectedPageIndex;
        public int SelectedPageIndex
        {
            get => _selectedPageIndex;
            set
            {
                if (SetProperty(ref _selectedPageIndex, value))
                {
                    UpdateCurrentPage();
                }
            }
        }

        public MainViewModel()
        {
            _chatSessionRepository = new ChatSessionRepository();
            DocumentWorkspacePanel = new DocumentWorkspacePanelViewModel();
            SessionWorkspaceBridge = new SessionWorkspaceBridgeViewModel(DocumentWorkspacePanel, _chatSessionRepository);

            SessionListPanel = new SessionListPanelViewModel(SessionWorkspaceBridge.OpenSession, _chatSessionRepository);
            LiveTasksPanel = new LiveTasksPanelViewModel();
            FilmstripPanel = new FilmstripPanelViewModel();
            MultiFunctionAreaPanel = new MultiFunctionAreaPanelViewModel();

            SessionPage = new Ferrita.PageControls.Session.ViewModels.SessionPageViewModel(SessionListPanel, LiveTasksPanel, DocumentWorkspacePanel, MultiFunctionAreaPanel);
            DeskPage = new Ferrita.PageControls.Desk.ViewModels.DeskPageViewModel();
            MarvelousPage = new Ferrita.PageControls.Marvelous.ViewModels.MarvelousPageViewModel();
            TilesPage = new Ferrita.PageControls.Tiles.ViewModels.TilesPageViewModel();

            _currentPageViewModel = SessionPage;
            _selectedPageIndex = 0;
        }

        private void UpdateCurrentPage()
        {
            switch (SelectedPageIndex)
            {
                case 0:
                    CurrentPageViewModel = SessionPage;
                    break;
                case 1:
                    CurrentPageViewModel = DeskPage;
                    break;
                case 2:
                    CurrentPageViewModel = MarvelousPage;
                    break;
                case 3:
                    CurrentPageViewModel = TilesPage;
                    break;
            }
        }

        public async Task HandleGuiClosingAsync()
        {
            var openSessionViewModels = DocumentWorkspacePanel.OpenedDocuments
                .Select(document => document.ContentViewModel)
                .OfType<ChatSessionControlViewModel>()
                .Where(viewModel => !string.IsNullOrWhiteSpace(viewModel.SessionId))
                .ToArray();

            if (openSessionViewModels.Length == 0)
            {
                return;
            }

            foreach (var sessionViewModel in openSessionViewModels)
            {
                sessionViewModel.SaveSessionSnapshot();
            }

            var sessionIds = openSessionViewModels
                .Select(viewModel => viewModel.SessionId)
                .Where(sessionId => !string.IsNullOrWhiteSpace(sessionId))
                .Select(sessionId => sessionId!)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();

            BackgroundMemoryQueue.Instance.Enqueue(sessionIds);
            await Task.CompletedTask;
        }
    }
}
