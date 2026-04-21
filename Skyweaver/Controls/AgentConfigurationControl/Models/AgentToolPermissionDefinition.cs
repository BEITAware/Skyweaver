using Skyweaver.Infrastructure.Mvvm;

namespace Skyweaver.Controls.AgentConfigurationControl.Models
{
    public sealed class AgentToolPermissionDefinition : ObservableObject
    {
        private string _toolName = string.Empty;
        private AgentToolPermissionMode _permission = AgentToolPermissionMode.RequireConfirmation;
        private string _toolDescription = string.Empty;
        private bool _isMissing;
        private bool _isGloballyEnabled = true;

        public string ToolName
        {
            get => _toolName;
            set => SetProperty(ref _toolName, value?.Trim() ?? string.Empty);
        }

        public AgentToolPermissionMode Permission
        {
            get => _permission;
            set => SetProperty(ref _permission, value);
        }

        public string ToolDescription
        {
            get => _toolDescription;
            set => SetProperty(ref _toolDescription, value ?? string.Empty);
        }

        public bool IsMissing
        {
            get => _isMissing;
            set
            {
                if (SetProperty(ref _isMissing, value))
                {
                    OnPropertyChanged(nameof(GlobalAvailabilityText));
                }
            }
        }

        public bool IsGloballyEnabled
        {
            get => _isGloballyEnabled;
            set
            {
                if (SetProperty(ref _isGloballyEnabled, value))
                {
                    OnPropertyChanged(nameof(GlobalAvailabilityText));
                }
            }
        }

        public string GlobalAvailabilityText => IsMissing
            ? "当前工具发现结果中缺失"
            : IsGloballyEnabled
                ? "全局：已启用"
                : "全局：已禁用";
    }
}
