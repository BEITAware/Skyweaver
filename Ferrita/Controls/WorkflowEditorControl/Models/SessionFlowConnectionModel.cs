using Ferrita.Infrastructure.Mvvm;

namespace Ferrita.Controls.WorkflowEditorControl.Models
{
    public sealed class SessionFlowConnectionModel : ObservableObject
    {
        private string _id = Guid.NewGuid().ToString("N");
        private string _sourceNodeId = string.Empty;
        private string _sourcePortId = string.Empty;
        private string _targetNodeId = string.Empty;
        private string _targetPortId = string.Empty;
        private string _pathData = string.Empty;
        private SessionFlowPortType _portType = SessionFlowPortType.NaturalLanguage;
        private double _sourceX;
        private double _sourceY;
        private double _targetX;
        private double _targetY;

        public string Id
        {
            get => _id;
            set => SetProperty(ref _id, string.IsNullOrWhiteSpace(value) ? Guid.NewGuid().ToString("N") : value.Trim());
        }

        public string SourceNodeId
        {
            get => _sourceNodeId;
            set => SetProperty(ref _sourceNodeId, value?.Trim() ?? string.Empty);
        }

        public string SourcePortId
        {
            get => _sourcePortId;
            set => SetProperty(ref _sourcePortId, value?.Trim() ?? string.Empty);
        }

        public string TargetNodeId
        {
            get => _targetNodeId;
            set => SetProperty(ref _targetNodeId, value?.Trim() ?? string.Empty);
        }

        public string TargetPortId
        {
            get => _targetPortId;
            set => SetProperty(ref _targetPortId, value?.Trim() ?? string.Empty);
        }

        public string PathData
        {
            get => _pathData;
            set => SetProperty(ref _pathData, value ?? string.Empty);
        }

        public SessionFlowPortType PortType
        {
            get => _portType;
            set
            {
                if (SetProperty(ref _portType, value))
                {
                    OnPropertyChanged(nameof(IsNaturalLanguageConnection));
                    OnPropertyChanged(nameof(IsXmlConnection));
                }
            }
        }

        public double SourceX
        {
            get => _sourceX;
            set => SetProperty(ref _sourceX, value);
        }

        public double SourceY
        {
            get => _sourceY;
            set => SetProperty(ref _sourceY, value);
        }

        public double TargetX
        {
            get => _targetX;
            set => SetProperty(ref _targetX, value);
        }

        public double TargetY
        {
            get => _targetY;
            set => SetProperty(ref _targetY, value);
        }

        public bool IsNaturalLanguageConnection => PortType == SessionFlowPortType.NaturalLanguage;

        public bool IsXmlConnection => PortType == SessionFlowPortType.XmlField;

        public SessionFlowConnectionModel DeepClone()
        {
            return new SessionFlowConnectionModel
            {
                Id = Id,
                SourceNodeId = SourceNodeId,
                SourcePortId = SourcePortId,
                TargetNodeId = TargetNodeId,
                TargetPortId = TargetPortId,
                PathData = PathData,
                PortType = PortType,
                SourceX = SourceX,
                SourceY = SourceY,
                TargetX = TargetX,
                TargetY = TargetY
            };
        }
    }
}
