using Skyweaver.Infrastructure.Mvvm;
using Skyweaver.Services.Localization;

namespace Skyweaver.Controls.SkyweaverPreferencesControl.ViewModels.Pages
{
    public sealed class LocalizationLanguageOptionViewModel : ObservableObject
    {
        private readonly LocalizationLanguageInfo _languageInfo;

        public LocalizationLanguageOptionViewModel(LocalizationLanguageInfo languageInfo)
        {
            _languageInfo = languageInfo;
        }

        public string LanguageCode => _languageInfo.LanguageCode;

        public string DisplayName => LocalizationRuntime.Instance.GetString(
            _languageInfo.ResourceKey,
            _languageInfo.FallbackDisplayName);

        public void RefreshDisplayName()
        {
            OnPropertyChanged(nameof(DisplayName));
        }
    }
}
