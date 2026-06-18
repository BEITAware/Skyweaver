using Ferrita.Infrastructure.Mvvm;
using Ferrita.Services.Localization;

namespace Ferrita.Controls.WorkflowEditorControl.Models
{
    public sealed class SessionFlowPortModel : ObservableObject
    {
        private string _id = Guid.NewGuid().ToString("N");
        private string _name = string.Empty;
        private SessionFlowPortDirection _direction;
        private SessionFlowPortType _portType = SessionFlowPortType.NaturalLanguage;
        private bool _isFlexiblePlaceholder;
        private bool _isBooleanCondition;
        private string _pairKey = string.Empty;
        private bool _isTransparentOutput;

        public string Id
        {
            get => _id;
            set => SetProperty(ref _id, string.IsNullOrWhiteSpace(value) ? Guid.NewGuid().ToString("N") : value.Trim());
        }

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value?.Trim() ?? string.Empty);
        }

        public SessionFlowPortDirection Direction
        {
            get => _direction;
            set => SetProperty(ref _direction, value);
        }

        public SessionFlowPortType PortType
        {
            get => _portType;
            set
            {
                if (SetProperty(ref _portType, value))
                {
                    OnPropertyChanged(nameof(IsNaturalLanguagePort));
                    OnPropertyChanged(nameof(IsXmlFieldPort));
                    OnPropertyChanged(nameof(PortTypeDisplayText));
                }
            }
        }

        public bool IsFlexiblePlaceholder
        {
            get => _isFlexiblePlaceholder;
            set
            {
                if (SetProperty(ref _isFlexiblePlaceholder, value))
                {
                    OnPropertyChanged(nameof(IsNaturalLanguagePort));
                    OnPropertyChanged(nameof(IsXmlFieldPort));
                    OnPropertyChanged(nameof(IsFlexiblePort));
                    OnPropertyChanged(nameof(PortTypeDisplayText));
                }
            }
        }

        public bool IsBooleanCondition
        {
            get => _isBooleanCondition;
            set => SetProperty(ref _isBooleanCondition, value);
        }

        public string PairKey
        {
            get => _pairKey;
            set => SetProperty(ref _pairKey, value?.Trim() ?? string.Empty);
        }

        public bool IsTransparentOutput
        {
            get => _isTransparentOutput;
            set => SetProperty(ref _isTransparentOutput, value);
        }

        public bool IsNaturalLanguagePort => !IsFlexiblePlaceholder && PortType == SessionFlowPortType.NaturalLanguage;

        public bool IsXmlFieldPort => !IsFlexiblePlaceholder && PortType == SessionFlowPortType.XmlField;

        public bool IsFlexiblePort => IsFlexiblePlaceholder;

        public string PortTypeDisplayText => IsFlexiblePlaceholder
            ? L("WorkflowEditor.PortType.Smart", "智能")
            : PortType == SessionFlowPortType.XmlField
                ? L("WorkflowEditor.PortType.XmlField", "XML字段")
                : L("WorkflowEditor.PortType.NaturalLanguage", "自然语言");

        public SessionFlowPortModel DeepClone()
        {
            return new SessionFlowPortModel
            {
                Id = Id,
                Name = Name,
                Direction = Direction,
                PortType = PortType,
                IsFlexiblePlaceholder = IsFlexiblePlaceholder,
                IsBooleanCondition = IsBooleanCondition,
                PairKey = PairKey,
                IsTransparentOutput = IsTransparentOutput
            };
        }

        private static string L(string resourceKey, string fallback)
        {
            return LocalizationRuntime.Instance.GetString(resourceKey, fallback);
        }
    }
}
