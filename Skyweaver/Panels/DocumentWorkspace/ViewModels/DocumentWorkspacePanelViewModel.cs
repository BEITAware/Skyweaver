using System.Collections.ObjectModel;
using System.Windows.Input;
using Skyweaver.Commands;
using Skyweaver.Infrastructure.Mvvm;
using Skyweaver.Panels.DocumentWorkspace.Contracts;
using Skyweaver.Panels.DocumentWorkspace.Models;
using Skyweaver.Services.Localization;

namespace Skyweaver.Panels.DocumentWorkspace.ViewModels
{
    public sealed class DocumentWorkspacePanelViewModel : ObservableObject, IDocumentWorkspaceHost
    {
        private WorkspaceDocument? _activeDocument;

        public event Action<WorkspaceDocument>? DocumentClosed;

        public ObservableCollection<WorkspaceDocument> OpenedDocuments { get; } = new();

        public WorkspaceDocument? ActiveDocument
        {
            get => _activeDocument;
            set => SetProperty(ref _activeDocument, value);
        }

        public ICommand CloseDocumentCommand { get; }

        public DocumentWorkspacePanelViewModel()
        {
            CloseDocumentCommand = new RelayCommand<WorkspaceDocument>(CloseDocument, document => document != null);
        }

        public WorkspaceDocument? FindByDocumentKey(string documentKey)
        {
            if (string.IsNullOrWhiteSpace(documentKey))
            {
                return null;
            }

            return OpenedDocuments.FirstOrDefault(document => document.DocumentKey == documentKey);
        }

        public WorkspaceDocument OpenDocument(WorkspaceDocument document)
        {
            var existingDocument = FindByDocumentKey(document.DocumentKey);
            if (existingDocument != null)
            {
                ActiveDocument = existingDocument;
                return existingDocument;
            }

            if (!OpenedDocuments.Contains(document))
            {
                OpenedDocuments.Add(document);
            }

            ActiveDocument = document;
            return document;
        }

        public WorkspaceDocument CreateAndOpenDocument(string title, object? contentViewModel, string? documentKey = null, string? subtitle = null, string? iconPath = null)
        {
            var document = new WorkspaceDocument
            {
                DocumentKey = documentKey ?? string.Empty,
                Title = title,
                Subtitle = subtitle ?? string.Empty,
                IconPath = iconPath ?? WorkspaceDocument.DefaultIconPath,
                ContentViewModel = contentViewModel,
                PlaceholderText = LF("DocumentWorkspace.Placeholder.NoContentViewModelFormat", "Document '{0}' has no content view model yet.", title)
            };

            return OpenDocument(document);
        }

        public WorkspaceDocument CreateAndOpenDocument(WorkspaceTabOptions options)
        {
            var document = new WorkspaceDocument
            {
                DocumentKey = options.DocumentKey,
                Title = options.Title,
                Subtitle = options.Subtitle,
                IconPath = options.IconPath,
                ContentViewModel = options.ContentViewModel,
                PlaceholderText = options.PlaceholderText,
                TabTypeKey = options.TabTypeKey,
                InstanceNumber = options.InstanceNumber
            };

            return OpenDocument(document);
        }

        public WorkspaceDocument OpenOrActivateDocument(string documentKey, Func<WorkspaceDocument> documentFactory)
        {
            var existingDocument = FindByDocumentKey(documentKey);
            if (existingDocument != null)
            {
                ActiveDocument = existingDocument;
                return existingDocument;
            }

            return OpenDocument(documentFactory());
        }

        private void CloseDocument(WorkspaceDocument? document)
        {
            if (document == null || !OpenedDocuments.Contains(document))
            {
                return;
            }

            var removedIndex = OpenedDocuments.IndexOf(document);
            OpenedDocuments.Remove(document);

            if (ReferenceEquals(ActiveDocument, document))
            {
                ActiveDocument = OpenedDocuments.Count == 0
                    ? null
                    : OpenedDocuments[Math.Min(removedIndex, OpenedDocuments.Count - 1)];
            }

            if (document.ContentViewModel is IWorkspaceDocumentCloseAware closeAware)
            {
                closeAware.OnWorkspaceDocumentClosed();
            }

            DocumentClosed?.Invoke(document);
        }

        private static string LF(string resourceKey, string fallbackFormat, params object?[] args)
        {
            var format = LocalizationRuntime.Instance.GetString(resourceKey, fallbackFormat);
            return string.Format(format, args);
        }
    }
}
