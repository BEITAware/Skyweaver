using Ferrita.Panels.DocumentWorkspace.Models;

namespace Ferrita.Panels.DocumentWorkspace.Contracts
{
    public interface IDocumentWorkspaceHost
    {
        WorkspaceDocument? ActiveDocument { get; }

        WorkspaceDocument? FindByDocumentKey(string documentKey);

        WorkspaceDocument OpenDocument(WorkspaceDocument document);

        WorkspaceDocument CreateAndOpenDocument(string title, object? contentViewModel, string? documentKey = null, string? subtitle = null, string? iconPath = null);

        WorkspaceDocument OpenOrActivateDocument(string documentKey, Func<WorkspaceDocument> documentFactory);
    }
}
