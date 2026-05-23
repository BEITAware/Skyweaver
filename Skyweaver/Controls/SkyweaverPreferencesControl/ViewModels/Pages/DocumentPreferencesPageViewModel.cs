using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Input;
using Skyweaver.Commands;
using Skyweaver.Infrastructure.Mvvm;
using Skyweaver.Models.Multimodal;
using Skyweaver.Services.Directories;
using Skyweaver.Services.Localization;
using Skyweaver.Services.Multimodal;

namespace Skyweaver.Controls.SkyweaverPreferencesControl.ViewModels.Pages
{
    /// <summary>
    /// 文档首选项页面视图模型，用于管理多模态文档处理的配置。
    /// </summary>
    public sealed class DocumentPreferencesPageViewModel : ObservableObject
    {
        private readonly MultimodalConfiguration _configuration;
        private string _statusMessage;

        public DocumentPreferencesPageViewModel()
        {
            _configuration = MultimodalRuntime.Instance.GetConfiguration();
            _statusMessage = L("Multimodal.Document.Status.Loaded", "多模态配置已加载。");

            OpenConfigurationDirectoryCommand = new RelayCommand(OpenConfigurationDirectory);
            LocalizationRuntime.Instance.LanguageChanged += (_, _) => RefreshLocalizedText();
        }

        /// <summary>
        /// 页面标题。
        /// </summary>
        public string Title => L("Multimodal.Document.Page.Title", "文档");

        /// <summary>
        /// 页面描述。
        /// </summary>
        public string Description => L("Multimodal.Document.Page.Description", "配置多模态文档处理，例如文档字符识别与硬件加速方案。");

        /// <summary>
        /// 硬件计算方案选项列表。
        /// </summary>
        public IEnumerable<string> HardwareSolutions => new[] { "CPU", "GPU" };

        /// <summary>
        /// 是否启用文档字符识别。
        /// </summary>
        public bool EnableDocumentCharacterRecognition
        {
            get => _configuration.EnableDocumentCharacterRecognition;
            set
            {
                if (_configuration.EnableDocumentCharacterRecognition == value)
                {
                    return;
                }

                _configuration.EnableDocumentCharacterRecognition = value;
                OnPropertyChanged();
                PersistConfiguration(L("Multimodal.Document.Status.EnableOcrSaved", "启用文档字符识别设置已保存。"));
            }
        }

        /// <summary>
        /// 硬件方案。
        /// </summary>
        public string HardwareSolution
        {
            get => _configuration.HardwareSolution;
            set
            {
                var trimmedValue = value?.Trim() ?? "CPU";
                if (_configuration.HardwareSolution == trimmedValue)
                {
                    return;
                }

                _configuration.HardwareSolution = trimmedValue;
                OnPropertyChanged();
                PersistConfiguration(string.Format(L("Multimodal.Document.Status.HardwareSolutionSaved", "硬件方案已保存为 {0}。"), trimmedValue));
            }
        }

        /// <summary>
        /// 配置文件绝对路径。
        /// </summary>
        public string ConfigurationFilePath => MultimodalRuntime.Instance.ConfigurationFilePath;

        /// <summary>
        /// 配置状态消息。
        /// </summary>
        public string StatusMessage
        {
            get => _statusMessage;
            private set => SetProperty(ref _statusMessage, value);
        }

        /// <summary>
        /// 打开配置目录命令。
        /// </summary>
        public ICommand OpenConfigurationDirectoryCommand { get; }

        private void OpenConfigurationDirectory()
        {
            try
            {
                var directoryPath = Path.GetDirectoryName(ConfigurationFilePath) ?? SkyweaverDirectoryRuntime.Instance.ConfigurationDirectoryPath;
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
                MultimodalRuntime.Instance.SaveConfiguration(_configuration);
                StatusMessage = successMessage;
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format(L("EmbeddingModelConfiguration.Status.SaveFailedFormat", "保存失败：{0}"), ex.Message);
            }
        }

        private void RefreshLocalizedText()
        {
            OnPropertyChanged(nameof(Title));
            OnPropertyChanged(nameof(Description));
        }

        private static string L(string resourceKey, string fallback)
        {
            return LocalizationRuntime.Instance.GetString(resourceKey, fallback);
        }
    }
}
