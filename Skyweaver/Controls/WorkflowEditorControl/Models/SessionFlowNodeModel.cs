using System.Collections.ObjectModel;
using Skyweaver.Infrastructure.Mvvm;

namespace Skyweaver.Controls.WorkflowEditorControl.Models
{
    public sealed class SessionFlowNodeModel : ObservableObject
    {
        private string _id = Guid.NewGuid().ToString("N");
        private string _title = string.Empty;
        private SessionFlowNodeKind _kind;
        private double _x;
        private double _y;
        private double _width = 240;
        private bool _isFixed;
        private bool _isSelected;
        private string _agentId = string.Empty;
        private string _agentDisplayName = string.Empty;
        private bool _isHiddenAgent;

        public SessionFlowNodeModel()
        {
            InputPorts.CollectionChanged += (_, _) => OnPropertyChanged(nameof(PortSummaryText));
            OutputPorts.CollectionChanged += (_, _) => OnPropertyChanged(nameof(PortSummaryText));
        }

        public string Id
        {
            get => _id;
            set => SetProperty(ref _id, string.IsNullOrWhiteSpace(value) ? Guid.NewGuid().ToString("N") : value.Trim());
        }

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value?.Trim() ?? string.Empty);
        }

        public SessionFlowNodeKind Kind
        {
            get => _kind;
            set
            {
                if (SetProperty(ref _kind, value))
                {
                    OnPropertyChanged(nameof(IsInputOutputNode));
                    OnPropertyChanged(nameof(IsAgentNode));
                    OnPropertyChanged(nameof(IsLogicNode));
                    OnPropertyChanged(nameof(IsLogicExecutionNode));
                    OnPropertyChanged(nameof(IsNextLogicExecutionNode));
                    OnPropertyChanged(nameof(NodeKindDisplayText));
                }
            }
        }

        public double X
        {
            get => _x;
            set => SetProperty(ref _x, value);
        }

        public double Y
        {
            get => _y;
            set => SetProperty(ref _y, value);
        }

        public double Width
        {
            get => _width;
            set => SetProperty(ref _width, value);
        }

        public bool IsFixed
        {
            get => _isFixed;
            set
            {
                if (SetProperty(ref _isFixed, value))
                {
                    OnPropertyChanged(nameof(CanDelete));
                }
            }
        }

        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }

        public string AgentId
        {
            get => _agentId;
            set => SetProperty(ref _agentId, value?.Trim() ?? string.Empty);
        }

        public string AgentDisplayName
        {
            get => _agentDisplayName;
            set => SetProperty(ref _agentDisplayName, value?.Trim() ?? string.Empty);
        }

        public bool IsHiddenAgent
        {
            get => _isHiddenAgent;
            set => SetProperty(ref _isHiddenAgent, value);
        }

        public ObservableCollection<SessionFlowPortModel> InputPorts { get; } = new();

        public ObservableCollection<SessionFlowPortModel> OutputPorts { get; } = new();

        public bool IsInputOutputNode => Kind is SessionFlowNodeKind.UserInput or SessionFlowNodeKind.Return;

        public bool IsAgentNode => Kind == SessionFlowNodeKind.Agent;

        public bool IsLogicNode => Kind is SessionFlowNodeKind.LogicAnd or SessionFlowNodeKind.LogicOr or SessionFlowNodeKind.LogicXor or SessionFlowNodeKind.LogicNot;

        public bool IsLogicExecutionNode => Kind == SessionFlowNodeKind.LogicExecution;

        public bool IsNextLogicExecutionNode => Kind == SessionFlowNodeKind.NextLogicExecution;

        public bool CanDelete => !IsFixed;

        public string NodeKindDisplayText => Kind switch
        {
            SessionFlowNodeKind.UserInput => "输入输出",
            SessionFlowNodeKind.Return => "输入输出",
            SessionFlowNodeKind.Agent => "代理",
            SessionFlowNodeKind.LogicAnd => "逻辑",
            SessionFlowNodeKind.LogicOr => "逻辑",
            SessionFlowNodeKind.LogicXor => "逻辑",
            SessionFlowNodeKind.LogicNot => "逻辑",
            SessionFlowNodeKind.LogicExecution => "逻辑执行",
            SessionFlowNodeKind.NextLogicExecution => "仅下一个逻辑执行",
            _ => "节点"
        };

        public string PortSummaryText => $"输入 {InputPorts.Count} · 输出 {OutputPorts.Count}";

        public SessionFlowNodeModel DeepClone()
        {
            var clone = new SessionFlowNodeModel
            {
                Id = Id,
                Title = Title,
                Kind = Kind,
                X = X,
                Y = Y,
                Width = Width,
                IsFixed = IsFixed,
                IsSelected = IsSelected,
                AgentId = AgentId,
                AgentDisplayName = AgentDisplayName,
                IsHiddenAgent = IsHiddenAgent
            };

            foreach (var input in InputPorts)
            {
                clone.InputPorts.Add(input.DeepClone());
            }

            foreach (var output in OutputPorts)
            {
                clone.OutputPorts.Add(output.DeepClone());
            }

            return clone;
        }
    }
}
