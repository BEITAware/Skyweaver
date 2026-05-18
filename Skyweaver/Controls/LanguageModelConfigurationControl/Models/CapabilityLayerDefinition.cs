using System.Collections.ObjectModel;
using Skyweaver.Infrastructure.Mvvm;
using Skyweaver.Services.Localization;

namespace Skyweaver.Controls.LanguageModelConfigurationControl.Models
{
    public sealed class CapabilityLayerDefinition : ObservableObject
    {
        private string _key = Guid.NewGuid().ToString("N");
        private string _name = string.Empty;
        private bool _isBuiltIn;

        public string Key
        {
            get => _key;
            set => SetProperty(ref _key, value?.Trim() ?? string.Empty);
        }

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value?.Trim() ?? string.Empty);
        }

        public bool IsBuiltIn
        {
            get => _isBuiltIn;
            set
            {
                if (SetProperty(ref _isBuiltIn, value))
                {
                    OnPropertyChanged(nameof(CanDelete));
                    OnPropertyChanged(nameof(CanRename));
                    OnPropertyChanged(nameof(IsUserSelectable));
                    OnPropertyChanged(nameof(ManagementHint));
                }
            }
        }

        public ObservableCollection<CapabilityLayerEntry> LanguageModels { get; } = new();

        public bool CanDelete => !IsBuiltIn;

        public bool CanRename => !IsBuiltIn;

        public bool IsUserSelectable => !IsBuiltIn;

        public string ManagementHint => IsBuiltIn
            ? L("LanguageModelConfiguration.CapabilityLayers.ManagementHint.BuiltIn", "系统内置功能层级。名称不可修改，也不能删除。")
            : L("LanguageModelConfiguration.CapabilityLayers.ManagementHint.User", "用户自定义功能层级。");

        public void RefreshLocalizedText()
        {
            OnPropertyChanged(nameof(ManagementHint));
        }

        private static string L(string resourceKey, string fallback)
        {
            return LocalizationRuntime.Instance.GetString(resourceKey, fallback);
        }
    }
}
