using Ferrita.Infrastructure.Mvvm;

namespace Ferrita.Models.Localization
{
    public sealed class LocalizationConfiguration : ObservableObject
    {
        private string _languageCode = "zh-CN";

        public string LanguageCode
        {
            get => _languageCode;
            set => SetProperty(ref _languageCode, string.IsNullOrWhiteSpace(value) ? "zh-CN" : value.Trim());
        }
    }
}
