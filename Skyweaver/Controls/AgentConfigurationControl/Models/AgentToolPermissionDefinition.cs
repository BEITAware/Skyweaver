using Skyweaver.Infrastructure.Mvvm;
using Skyweaver.Services.Localization;

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
            ? L("AgentConfiguration.GlobalAvailability.Missing", "当前工具发现结果中缺失")
            : IsGloballyEnabled
                ? L("AgentConfiguration.GlobalAvailability.Enabled", "全局：已启用")
                : L("AgentConfiguration.GlobalAvailability.Disabled", "全局：已禁用");

        public void RefreshLocalizedText()
        {
            OnPropertyChanged(nameof(GlobalAvailabilityText));
        }

        private static string L(string resourceKey, string fallback)
        {
            return LocalizationRuntime.Instance.GetString(resourceKey, fallback);
        }
    }
}
