namespace Ferrita.Controls.WorkflowEditorControl.Models
{
    public sealed class SessionFlowGraphModel
    {
        public double CanvasWidth { get; set; } = 3200;

        public double CanvasHeight { get; set; } = 2000;

        public List<SessionFlowNodeModel> Nodes { get; } = new();

        public List<SessionFlowConnectionModel> Connections { get; } = new();
    }
}
