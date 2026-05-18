namespace Skyweaver.Panels.DocumentWorkspace.Models
{
    public sealed class WorkspaceTabOptions
    {
        public string DocumentKey { get; init; } = string.Empty;

        public string Title { get; init; } = string.Empty;

        public string Subtitle { get; init; } = string.Empty;

        public string IconPath { get; init; } = WorkspaceDocument.DefaultIconPath;

        public object? ContentViewModel { get; init; }

        public string PlaceholderText { get; init; } = WorkspaceDocument.DefaultPlaceholderText;

        public string TabTypeKey { get; init; } = string.Empty;

        public int? InstanceNumber { get; init; }
    }
}
