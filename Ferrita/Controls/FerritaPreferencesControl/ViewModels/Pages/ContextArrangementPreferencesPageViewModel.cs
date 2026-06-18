using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Input;
using Ferrita.Commands;
using Ferrita.Infrastructure.Mvvm;
using Ferrita.Models.ContextManagement;
using Ferrita.Services.ContextManagement;
using Ferrita.Services.Localization;

namespace Ferrita.Controls.FerritaPreferencesControl.ViewModels.Pages
{
    public sealed class ContextArrangementPreferencesPageViewModel : ObservableObject
    {
        private readonly ContextArrangementRuntime _runtime;
        private readonly ContextArrangementConfiguration _configuration;
        private string _statusMessage;

        public ContextArrangementPreferencesPageViewModel()
        {
            _runtime = ContextArrangementRuntime.Instance;
            _configuration = _runtime.GetConfiguration();
            _statusMessage = L("ContextArrangement.Status.Loaded", "上下文编排配置已加载。");

            OpenConfigurationDirectoryCommand = new RelayCommand(OpenConfigurationDirectory);
            LocalizationRuntime.Instance.LanguageChanged += (_, _) => RefreshLocalizedText();
        }

        public string Title => L("ContextArrangement.Page.Title", "上下文编排");

        public string Description => L("ContextArrangement.Page.Description", "配置上下文编排策略，优化大语言模型工具调用与提示词结构。");

        public string Hint => L("ContextArrangement.Page.Hint", "修改后会立即写入 ContextArrangement.xml。");

        public string ConfigurationFilePath => _runtime.ConfigurationFilePath;

        public bool OptimizeToolCallPrompt
        {
            get => _configuration.OptimizeToolCallPrompt;
            set
            {
                if (_configuration.OptimizeToolCallPrompt == value)
                {
                    return;
                }

                _configuration.OptimizeToolCallPrompt = value;
                OnPropertyChanged();
                PersistConfiguration(L("ContextArrangement.Status.OptimizeToolCallPromptSaved", "工具调用优化提示词设置已保存。"));
            }
        }

        public bool ToolCallIdTable
        {
            get => _configuration.ToolCallIdTable;
            set
            {
                if (_configuration.ToolCallIdTable == value)
                {
                    return;
                }

                _configuration.ToolCallIdTable = value;
                OnPropertyChanged();
                PersistConfiguration(L("ContextArrangement.Status.ToolCallIdTableSaved", "Tool Call ID表设置已保存。"));
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
