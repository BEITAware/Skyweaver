using Ferrita.Controls.ChatSessionControl.ViewModels;
using Ferrita.Services.ChatSession;
using Ferrita.Panels.DocumentWorkspace.Contracts;
using Ferrita.Panels.DocumentWorkspace.Models;
using Ferrita.Panels.SessionList.Models;
using Ferrita.Services.Localization;

namespace Ferrita.ViewModels
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
                PlaceholderText = LF("SessionWorkspace.PlaceholderFormat", "Session '{0}' content will appear here.", session.Title)
            });
        }

        private static string L(string resourceKey, string fallback)
        {
            return LocalizationRuntime.Instance.GetString(resourceKey, fallback);
        }

        private static string LF(string resourceKey, string fallbackFormat, params object?[] args)
        {
            var format = L(resourceKey, fallbackFormat);
            return string.Format(format, args);
        }
    }
}
