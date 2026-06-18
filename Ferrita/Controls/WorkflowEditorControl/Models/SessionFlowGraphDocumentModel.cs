namespace Ferrita.Controls.WorkflowEditorControl.Models
{
    public sealed class SessionFlowGraphDocumentModel
    {
        public string GraphId { get; set; } = Guid.NewGuid().ToString("N");

        public string Name { get; set; } = string.Empty;

        public string FilePath { get; set; } = string.Empty;

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;

        public SessionFlowGraphModel Graph { get; set; } = new();
    }
}
