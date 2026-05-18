using System.Globalization;
using System.Threading;
using System.Windows;
using Skyweaver.Models.Localization;

namespace Skyweaver.Services.Localization
{
    public sealed class LocalizationRuntime
    {
        public const string DefaultLanguageCode = "zh-CN";
        public const string EnglishLanguageCode = "en-US";

        private static readonly IReadOnlyList<LocalizationLanguageInfo> s_supportedLanguages =
        [
            new LocalizationLanguageInfo(DefaultLanguageCode, "Localization.Language.zh-CN", "简体中文"),
            new LocalizationLanguageInfo(EnglishLanguageCode, "Localization.Language.en-US", "English")
        ];

        private readonly object _syncRoot = new();
        private readonly LocalizationConfigurationRepository _configurationRepository;
        private LocalizationConfiguration _configuration;

        private LocalizationRuntime()
        {
            var pathProvider = new LocalizationPathProvider();
            _configurationRepository = new LocalizationConfigurationRepository(pathProvider);
            _configuration = NormalizeConfiguration(_configurationRepository.Load());
        }

        public static LocalizationRuntime Instance { get; } = new();

        public IReadOnlyList<LocalizationLanguageInfo> SupportedLanguages => s_supportedLanguages;

        public string ConfigurationFilePath => _configurationRepository.ConfigurationFilePath;

        public string CurrentLanguageCode
        {
            get
            {
                lock (_syncRoot)
                {
                    return _configuration.LanguageCode;
                }
            }
        }

        public event EventHandler? LanguageChanged;

        public LocalizationConfiguration GetConfiguration()
        {
            lock (_syncRoot)
            {
                return CloneConfiguration(_configuration);
            }
        }

        public void ApplyConfiguredLanguage()
        {
            ApplyLanguageResources(CurrentLanguageCode);
        }

        public void SetLanguage(string? languageCode)
        {
            var normalizedLanguageCode = NormalizeLanguageCode(languageCode);
            var changed = false;

            lock (_syncRoot)
            {
                if (!string.Equals(_configuration.LanguageCode, normalizedLanguageCode, StringComparison.Ordinal))
                {
                    _configuration = new LocalizationConfiguration
                    {
                        LanguageCode = normalizedLanguageCode
                    };
                    _configurationRepository.Save(_configuration);
                    changed = true;
                }
            }

            ApplyLanguageResources(normalizedLanguageCode);

            if (changed)
            {
                LanguageChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public void SaveConfiguration(LocalizationConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(configuration);
            SetLanguage(configuration.LanguageCode);
        }

        public string GetString(string resourceKey, string fallback)
        {
            if (string.IsNullOrWhiteSpace(resourceKey))
            {
                return fallback;
            }

            var value = Application.Current?.TryFindResource(resourceKey);
            return value as string ?? value?.ToString() ?? fallback;
        }

        public bool IsSupportedLanguageCode(string? languageCode)
        {
            return s_supportedLanguages.Any(language =>
                string.Equals(language.LanguageCode, languageCode, StringComparison.OrdinalIgnoreCase));
        }

        public string NormalizeLanguageCode(string? languageCode)
        {
            if (string.IsNullOrWhiteSpace(languageCode))
            {
                return DefaultLanguageCode;
            }

            var trimmedLanguageCode = languageCode.Trim();
            var match = s_supportedLanguages.FirstOrDefault(language =>
                string.Equals(language.LanguageCode, trimmedLanguageCode, StringComparison.OrdinalIgnoreCase));

            return match?.LanguageCode ?? DefaultLanguageCode;
        }

        private void ApplyLanguageResources(string languageCode)
        {
            var normalizedLanguageCode = NormalizeLanguageCode(languageCode);
            ApplyCulture(normalizedLanguageCode);

            var application = Application.Current;
            if (application == null)
            {
                return;
            }

            void Apply()
            {
                var dictionaries = application.Resources.MergedDictionaries;

                for (var index = dictionaries.Count - 1; index >= 0; index--)
                {
                    if (IsLocalizationDictionary(dictionaries[index]))
                    {
                        dictionaries.RemoveAt(index);
                    }
                }

                dictionaries.Add(new ResourceDictionary
                {
                    Source = new Uri(
                        $"pack://application:,,,/Skyweaver;component/Resources/Localization/Strings.{normalizedLanguageCode}.xaml",
                        UriKind.Absolute)
                });
            }

            if (application.Dispatcher.CheckAccess())
            {
                Apply();
            }
            else
            {
                application.Dispatcher.Invoke(Apply);
            }
        }

        private static bool IsLocalizationDictionary(ResourceDictionary dictionary)
        {
            var source = dictionary.Source?.OriginalString;
            return source?.Contains("/Resources/Localization/Strings.", StringComparison.OrdinalIgnoreCase) == true;
        }

        private static void ApplyCulture(string languageCode)
        {
            var culture = CultureInfo.GetCultureInfo(languageCode);
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
        }

        private LocalizationConfiguration NormalizeConfiguration(LocalizationConfiguration configuration)
        {
            return new LocalizationConfiguration
            {
                LanguageCode = NormalizeLanguageCode(configuration.LanguageCode)
            };
        }

        private static LocalizationConfiguration CloneConfiguration(LocalizationConfiguration configuration)
        {
            return new LocalizationConfiguration
            {
                LanguageCode = configuration.LanguageCode
            };
        }
    }
}
