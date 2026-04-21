using Skyweaver.Infrastructure.Mvvm;

namespace Skyweaver.Controls.LanguageModelConfigurationControl.Models
{
    public sealed class CapabilityLayerEntry : ObservableObject
    {
        private string _languageModelKey = string.Empty;

        public string LanguageModelKey
        {
            get => _languageModelKey;
            set => SetProperty(ref _languageModelKey, value?.Trim() ?? string.Empty);
        }
    }
}
