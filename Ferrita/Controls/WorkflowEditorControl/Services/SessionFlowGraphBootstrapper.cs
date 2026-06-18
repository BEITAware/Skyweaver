using Ferrita.Controls.WorkflowEditorControl.Models;
using Ferrita.Services.Localization;

namespace Ferrita.Controls.WorkflowEditorControl.Services
{
    public static class SessionFlowGraphBootstrapper
    {
        private const double DefaultCanvasWidth = 3200;
        private const double DefaultCanvasHeight = 2000;
        private const double DefaultEndpointNodeGap = 220;

        public static SessionFlowGraphModel CreateDefaultGraph()
        {
            var graph = new SessionFlowGraphModel
            {
                CanvasWidth = DefaultCanvasWidth,
                CanvasHeight = DefaultCanvasHeight
            };

            var userInputNode = new SessionFlowNodeModel
            {
                Id = Guid.NewGuid().ToString("N"),
                Kind = SessionFlowNodeKind.UserInput,
                Title = L("WorkflowEditor.Node.UserInput", "用户输入"),
                Width = 220,
                X = 120,
                Y = 220,
                IsFixed = true
            };

            userInputNode.OutputPorts.Add(new SessionFlowPortModel
            {
                Id = "output-user-text",
                Name = L("WorkflowEditor.Port.NaturalLanguageOutput", "自然语言输出"),
                Direction = SessionFlowPortDirection.Output,
                PortType = SessionFlowPortType.NaturalLanguage
            });

            var returnNode = new SessionFlowNodeModel
            {
                Id = Guid.NewGuid().ToString("N"),
                Kind = SessionFlowNodeKind.Return,
                Title = L("WorkflowEditor.Node.Return", "返回"),
                Width = 220,
                X = userInputNode.X + userInputNode.Width + DefaultEndpointNodeGap,
                Y = 260,
                IsFixed = true
            };

            returnNode.InputPorts.Add(new SessionFlowPortModel
            {
                Id = "input-return-text",
                Name = L("WorkflowEditor.Port.NaturalLanguageInput", "自然语言输入"),
                Direction = SessionFlowPortDirection.Input,
                PortType = SessionFlowPortType.NaturalLanguage
            });

            returnNode.InputPorts.Add(new SessionFlowPortModel
            {
                Id = "input-return-xml",
                Name = L("WorkflowEditor.Port.XmlFieldInput", "XML字段输入"),
                Direction = SessionFlowPortDirection.Input,
                PortType = SessionFlowPortType.XmlField
            });

            graph.Nodes.Add(userInputNode);
            graph.Nodes.Add(returnNode);
            return graph;
        }

        private static string L(string resourceKey, string fallback)
        {
            return LocalizationRuntime.Instance.GetString(resourceKey, fallback);
        }
    }
}
