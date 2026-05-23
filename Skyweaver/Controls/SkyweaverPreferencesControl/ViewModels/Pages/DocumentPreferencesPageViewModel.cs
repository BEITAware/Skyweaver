using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Input;
using Skyweaver.Commands;
using Skyweaver.Infrastructure.Mvvm;
using Skyweaver.Models.Multimodal;
using Skyweaver.Services.Multimodal;
using Skyweaver.Services.Localization;

namespace Skyweaver.Controls.SkyweaverPreferencesControl.ViewModels.Pages
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

        public OcrHardwareOption SelectedHardwareOption
        {
            get => _configuration.HardwareOption;
            set
            {
                if (_configuration.HardwareOption == value)
                {
                    return;
                }

                _configuration.HardwareOption = value;
                OnPropertyChanged();
                PersistConfiguration(string.Format(L("Document.Status.HardwareOptionSaved", "硬件方案已设为 {0}。"), GetHardwareOptionDisplayName(value)));
            }
        }

        public IEnumerable<OcrHardwareOptionOption> HardwareOptions => new[]
        {
            new OcrHardwareOptionOption { Option = OcrHardwareOption.CPU, DisplayName = L("Document.HardwareOption.Cpu", "CPU (中央处理器)") },
            new OcrHardwareOptionOption { Option = OcrHardwareOption.GPU, DisplayName = L("Document.HardwareOption.Gpu", "GPU (图形处理器)") }
        };

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

        private string GetHardwareOptionDisplayName(OcrHardwareOption option)
        {
            return option switch
            {
                OcrHardwareOption.CPU => L("Document.HardwareOption.Cpu", "CPU"),
                OcrHardwareOption.GPU => L("Document.HardwareOption.Gpu", "GPU"),
                _ => option.ToString()
            };
        }

        private void RefreshLocalizedText()
        {
            OnPropertyChanged(nameof(Title));
            OnPropertyChanged(nameof(Description));
            OnPropertyChanged(nameof(Hint));
            OnPropertyChanged(nameof(HardwareOptions));
        }

        private static string L(string resourceKey, string fallback)
        {
            return LocalizationRuntime.Instance.GetString(resourceKey, fallback);
        }
    }

    public sealed class OcrHardwareOptionOption
    {
        public OcrHardwareOption Option { get; init; }
        public required string DisplayName { get; init; }
    }
}
