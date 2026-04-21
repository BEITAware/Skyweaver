namespace Skyweaver.Controls.WorkflowEditorControl.Models
{
    public sealed class SessionFlowAgentOption
    {
        public string AgentId { get; init; } = string.Empty;

        public string DisplayName { get; init; } = string.Empty;

        public bool CanCreate { get; init; } = true;

        public bool IsStructuredXmlIO { get; init; }

        public SessionFlowPortType InputPortType { get; init; } = SessionFlowPortType.NaturalLanguage;

        public SessionFlowPortType OutputPortType { get; init; } = SessionFlowPortType.NaturalLanguage;

        public IReadOnlyList<string> InputFieldPaths { get; init; } = Array.Empty<string>();

        public IReadOnlyList<string> OutputFieldPaths { get; init; } = Array.Empty<string>();

        public string DisplayText
        {
            get
            {
                if (string.IsNullOrWhiteSpace(DisplayName))
                {
                    return AgentId;
                }

                return string.IsNullOrWhiteSpace(AgentId)
                    ? DisplayName
                    : $"{DisplayName} ({AgentId})";
            }
        }

        public string PortTypeSummaryText => CanCreate
            ? IsStructuredXmlIO
                ? $"XML {GetFieldCountText(InputFieldPaths.Count, OutputFieldPaths.Count)}"
                : $"{GetPortTypeText(InputPortType)} / {GetPortTypeText(OutputPortType)}"
            : string.Empty;

        public string MenuDescription => CanCreate
            ? IsStructuredXmlIO
                ? $"Input fields {Math.Max(1, InputFieldPaths.Count)}, output fields {Math.Max(1, OutputFieldPaths.Count)}"
                : $"Input: {GetPortTypeText(InputPortType)}, Output: {GetPortTypeText(OutputPortType)}"
            : "Create an agent in Agent Configuration first.";

        public string InputPortName => InputPortType == SessionFlowPortType.XmlField ? "XML Input" : "Text Input";

        public string OutputPortName => OutputPortType == SessionFlowPortType.XmlField ? "XML Output" : "Text Output";

        private static string GetPortTypeText(SessionFlowPortType portType)
        {
            return portType == SessionFlowPortType.XmlField ? "XML" : "Text";
        }

        private static string GetFieldCountText(int inputCount, int outputCount)
        {
            return $"{Math.Max(1, inputCount)} in / {Math.Max(1, outputCount)} out";
        }
    }
}
