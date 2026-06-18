using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Input;
using Ferrita.Commands;
using Ferrita.Infrastructure.Mvvm;
using Ferrita.Services.Localization;

namespace Ferrita.Controls.FerritaPreferencesControl.ViewModels.Pages
{
    public sealed class LocalizationPreferencesPageViewModel : ObservableObject
    {
        private readonly LocalizationRuntime _runtime;
        private LocalizationLanguageOptionViewModel? _selectedLanguage;
        private string _statusMessage;

        public LocalizationPreferencesPageViewModel()
        {
            _runtime = LocalizationRuntime.Instance;
            SupportedLanguages = new ObservableCollection<LocalizationLanguageOptionViewModel>(
                _runtime.SupportedLanguages.Select(language => new LocalizationLanguageOptionViewModel(language)));

            _selectedLanguage = FindLanguage(_runtime.CurrentLanguageCode) ?? SupportedLanguages.FirstOrDefault();
            _statusMessage = L("Localization.Status.Loaded", "本地化配置已加载。");

            OpenConfigurationDirectoryCommand = new RelayCommand(OpenConfigurationDirectory);
            _runtime.LanguageChanged += (_, _) => RefreshLocalizedText();
        }

        public string Title => L("Localization.Page.Title", "本地化");

        public string Description => L("Localization.Page.Description", "配置 Ferrita 用户界面的显示语言。");

        public string Hint => L("Localization.Page.Hint", "修改后会立即写入 Localization.xml，并即时应用到已接入资源键的界面。");

        public string ConfigurationFilePath => _runtime.ConfigurationFilePath;

        public ObservableCollection<LocalizationLanguageOptionViewModel> SupportedLanguages { get; }

        public LocalizationLanguageOptionViewModel? SelectedLanguage
        {
            get => _selectedLanguage;
            set
            {
                if (!SetProperty(ref _selectedLanguage, value) || value == null)
                {
                    return;
                }

                try
                {
                    _runtime.SetLanguage(value.LanguageCode);
                    StatusMessage = L("Localization.Status.Saved", "语言设置已保存。");
                }
                catch (Exception ex)
                {
                    StatusMessage = string.Format(
                        L("Localization.Status.SaveFailedFormat", "保存失败：{0}"),
                        ex.Message);
                }
            }
        }

        public string StatusMessage
        {
            get => _statusMessage;
            private set => SetProperty(ref _statusMessage, value);
        }

        public ICommand OpenConfigurationDirectoryCommand { get; }

        private void OpenConfigurationDirectory()
        {
            try
            {
                var directoryPath = Path.GetDirectoryName(ConfigurationFilePath) ?? string.Empty;
                if (directoryPath.Length == 0)
                {
                    StatusMessage = L("Common.Status.ConfigurationDirectoryUnavailable", "无法定位配置目录。");
                    return;
                }

                Directory.CreateDirectory(directoryPath);
                Process.Start(new ProcessStartInfo
                {
                    FileName = directoryPath,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format(
                    L("Common.Status.OpenConfigurationDirectoryFailedFormat", "打开配置目录失败：{0}"),
                    ex.Message);
            }
        }

        private void RefreshLocalizedText()
        {
            foreach (var language in SupportedLanguages)
            {
                language.RefreshDisplayName();
            }

            var currentLanguage = FindLanguage(_runtime.CurrentLanguageCode);
            if (currentLanguage != null && !ReferenceEquals(_selectedLanguage, currentLanguage))
            {
                _selectedLanguage = currentLanguage;
                OnPropertyChanged(nameof(SelectedLanguage));
            }

            OnPropertyChanged(nameof(Title));
            OnPropertyChanged(nameof(Description));
            OnPropertyChanged(nameof(Hint));
            OnPropertyChanged(nameof(ConfigurationFilePath));
        }

        private LocalizationLanguageOptionViewModel? FindLanguage(string languageCode)
        {
            return SupportedLanguages.FirstOrDefault(language =>
                string.Equals(language.LanguageCode, languageCode, StringComparison.Ordinal));
        }

        private static string L(string resourceKey, string fallback)
        {
            return LocalizationRuntime.Instance.GetString(resourceKey, fallback);
        }
    }
}
