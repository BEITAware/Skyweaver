using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Input;
using Ferrita.Commands;
using Ferrita.Infrastructure.Mvvm;
using Ferrita.Models.Multimodal;
using Ferrita.Services.Multimodal;
using Ferrita.Services.Localization;

namespace Ferrita.Controls.FerritaPreferencesControl.ViewModels.Pages
{
    public sealed class ImagePreferencesPageViewModel : ObservableObject
    {
        private readonly MultimodalRuntime _runtime;
        private readonly MultimodalConfiguration _configuration;
        private string _statusMessage;

        public ImagePreferencesPageViewModel()
        {
            _runtime = MultimodalRuntime.Instance;
            _configuration = _runtime.GetConfiguration();
            _statusMessage = L("Image.Status.Loaded", "图像首选项已加载。");

            OpenConfigurationDirectoryCommand = new RelayCommand(OpenConfigurationDirectory);
            LocalizationRuntime.Instance.LanguageChanged += (_, _) => RefreshLocalizedText();
        }

        public string Title => L("Image.Page.Title", "图像");

        public string Description => L("Image.Page.Description", "配置图像处理首选项，优化长图像投影至 LLM 的效果。");

        public string Hint => L("Image.Page.Hint", "修改后会立即写入 Multimodal.xml。");

        public string ConfigurationFilePath => _runtime.ConfigurationFilePath;

        public bool EnableLongImageAutoParse
        {
            get => _configuration.EnableLongImageAutoParse;
            set
            {
                if (_configuration.EnableLongImageAutoParse == value)
                {
                    return;
                }

                _configuration.EnableLongImageAutoParse = value;
                OnPropertyChanged();
                PersistConfiguration(L("Image.Status.LongImageAutoParseSaved", "长图像自动解析设置已保存。"));
            }
        }

        public string LongImageAutoParseDescription => L(
            "Image.LongImageAutoParse.Description",
            "当图像宽高比超过 21:9 或 9:21 时，自动将其沿长边均匀切分，使每张子图比例在 16:9 到 9:16 之间（最接近 1:1），以提高 LLM 对长图内容的理解。此特性仅影响 LLM 侧的投影，不影响用户呈现和持久化。");

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
                StatusMessage = string.Format(L("Common.Status.OpenConfigurationDirectoryFailedFormat", "打开配置目录失败：{0}"), ex.Message);
            }
        }

        private void PersistConfiguration(string successMessage)
        {
            try
            {
                _runtime.SaveConfiguration(_configuration);
                StatusMessage = successMessage;
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format(L("Localization.Status.SaveFailedFormat", "保存失败：{0}"), ex.Message);
            }
        }

        private void RefreshLocalizedText()
        {
            OnPropertyChanged(nameof(Title));
            OnPropertyChanged(nameof(Description));
            OnPropertyChanged(nameof(Hint));
            OnPropertyChanged(nameof(LongImageAutoParseDescription));
        }

        private static string L(string resourceKey, string fallback)
        {
            return LocalizationRuntime.Instance.GetString(resourceKey, fallback);
        }
    }
}
