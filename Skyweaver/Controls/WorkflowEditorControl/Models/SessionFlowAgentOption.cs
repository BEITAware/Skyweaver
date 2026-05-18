using Skyweaver.Services.Localization;

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
                ? LF("WorkflowEditor.AgentOption.StructuredMenuDescriptionFormat", "Input fields {0}, output fields {1}", Math.Max(1, InputFieldPaths.Count), Math.Max(1, OutputFieldPaths.Count))
                : LF("WorkflowEditor.AgentOption.MenuDescriptionFormat", "Input: {0}, Output: {1}", GetPortTypeText(InputPortType), GetPortTypeText(OutputPortType))
            : L("WorkflowEditor.AgentOption.CreateAgentFirst", "Create an agent in Agent Configuration first.");

        public string InputPortName => InputPortType == SessionFlowPortType.XmlField
            ? L("WorkflowEditor.AgentOption.Port.XmlInput", "XML Input")
            : L("WorkflowEditor.AgentOption.Port.TextInput", "Text Input");

        public string OutputPortName => OutputPortType == SessionFlowPortType.XmlField
            ? L("WorkflowEditor.AgentOption.Port.XmlOutput", "XML Output")
            : L("WorkflowEditor.AgentOption.Port.TextOutput", "Text Output");

        private static string GetPortTypeText(SessionFlowPortType portType)
        {
            return portType == SessionFlowPortType.XmlField
                ? "XML"
                : L("WorkflowEditor.AgentOption.PortType.Text", "Text");
        }

        private static string GetFieldCountText(int inputCount, int outputCount)
        {
            return LF("WorkflowEditor.AgentOption.FieldCountFormat", "{0} in / {1} out", Math.Max(1, inputCount), Math.Max(1, outputCount));
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
