using System;
using System.Collections.Generic;
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
    public sealed class DocumentPreferencesPageViewModel : ObservableObject
    {
        private readonly MultimodalRuntime _runtime;
        private readonly MultimodalConfiguration _configuration;
        private string _statusMessage;

        public DocumentPreferencesPageViewModel()
        {
            _runtime = MultimodalRuntime.Instance;
            _configuration = _runtime.GetConfiguration();
            _statusMessage = L("Document.Status.Loaded", "文档首选项已加载。");

            OpenConfigurationDirectoryCommand = new RelayCommand(OpenConfigurationDirectory);
            LocalizationRuntime.Instance.LanguageChanged += (_, _) => RefreshLocalizedText();
        }

        public string Title => L("Document.Page.Title", "文档");

        public string Description => L("Document.Page.Description", "配置文档解析和字符识别首选项，优化多模态处理能力。");

        public string Hint => L("Document.Page.Hint", "修改后会立即写入 Multimodal.xml。");

        public string ConfigurationFilePath => _runtime.ConfigurationFilePath;

        public bool EnableOcr
        {
            get => _configuration.EnableOcr;
            set
            {
                if (_configuration.EnableOcr == value)
                {
                    return;
                }

                _configuration.EnableOcr = value;
                OnPropertyChanged();
                PersistConfiguration(L("Document.Status.OcrSaved", "启用文档字符识别设置已保存。"));
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
        }

        private static string L(string resourceKey, string fallback)
        {
            return LocalizationRuntime.Instance.GetString(resourceKey, fallback);
        }
    }


}
