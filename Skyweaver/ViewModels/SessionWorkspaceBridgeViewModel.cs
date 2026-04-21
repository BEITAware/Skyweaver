using Skyweaver.Controls.ChatSessionControl.ViewModels;
using Skyweaver.Services.ChatSession;
using Skyweaver.Panels.DocumentWorkspace.Contracts;
using Skyweaver.Panels.DocumentWorkspace.Models;
using Skyweaver.Panels.SessionList.Models;

namespace Skyweaver.ViewModels
{
    public sealed class SessionWorkspaceBridgeViewModel
    {
        private readonly IDocumentWorkspaceHost _documentWorkspacePanel;
        private readonly ChatSessionRepository _chatSessionRepository;

        public SessionWorkspaceBridgeViewModel(IDocumentWorkspaceHost documentWorkspacePanel, ChatSessionRepository chatSessionRepository)
        {
            _documentWorkspacePanel = documentWorkspacePanel;
            _chatSessionRepository = chatSessionRepository;
        }

        public void OpenSession(SessionListItem session)
        {
            var documentKey = $"chat-session:{session.Id}";
            var sessionModel = session.Session;
            _documentWorkspacePanel.OpenOrActivateDocument(documentKey, () => new WorkspaceDocument
            {
                DocumentKey = documentKey,
                Title = session.Title,
                Subtitle = session.TimeLabel,
                IconPath = session.IconPath,
                ContentViewModel = new ChatSessionControlViewModel(session.Title, session.TimeLabel, sessionModel, _chatSessionRepository),
                PlaceholderText = $"Session '{session.Title}' content will appear here."
            });
        }
    }
}
