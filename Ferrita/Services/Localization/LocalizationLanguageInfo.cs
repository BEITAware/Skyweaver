namespace Ferrita.Services.Localization
{
    public sealed class LocalizationLanguageInfo
    {
        public LocalizationLanguageInfo(string languageCode, string resourceKey, string fallbackDisplayName)
        {
            LanguageCode = languageCode;
            ResourceKey = resourceKey;
            FallbackDisplayName = fallbackDisplayName;
        }

        public string LanguageCode { get; }

        public string ResourceKey { get; }

        public string FallbackDisplayName { get; }
    }
}
