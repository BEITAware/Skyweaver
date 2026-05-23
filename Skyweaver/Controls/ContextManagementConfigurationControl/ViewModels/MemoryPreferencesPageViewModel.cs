using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Input;
using Skyweaver.Commands;
using Skyweaver.Infrastructure.Mvvm;
using Skyweaver.Models.ContextManagement;
using Skyweaver.Services.ContextManagement;
using Skyweaver.Services.Localization;

namespace Skyweaver.Controls.ContextManagementConfigurationControl.ViewModels
{
    public sealed class MemoryPreferencesPageViewModel : ObservableObject
    {
        private readonly ContextManagementRuntime _runtime;
        private readonly ContextManagementConfiguration _configuration;
        private string _statusMessage;

        public MemoryPreferencesPageViewModel()
        {
            _runtime = ContextManagementRuntime.Instance;
            _configuration = _runtime.GetConfiguration();
            _statusMessage = L("Common.Status.ConfigurationLoaded", "配置已加载。");

            OpenConfigurationDirectoryCommand = new RelayCommand(OpenConfigurationDirectory);
            LocalizationRuntime.Instance.LanguageChanged += (_, _) => RefreshLocalizedText();
        }

        public string Title => L("Memory.Page.Title", "记忆");

        public string Description => L("Memory.Page.Description", "配置 Skyweaver 记忆机制，允许代理记忆并共享以往的会话内容。");

        public string Hint => L("Memory.Page.Hint", "修改后会立即写入 ContextManagement.xml。");

        public string ConfigurationFilePath => _runtime.ConfigurationFilePath;

        public bool MemoryEnabled
        {
            get => _configuration.MemoryEnabled;
            set
            {
                if (_configuration.MemoryEnabled == value)
                {
                    return;
                }

                _configuration.MemoryEnabled = value;
                OnPropertyChanged();
                PersistConfiguration(L("Memory.Status.MemoryEnabledSaved", "启用记忆设置已保存。"));
            }
        }

        public MemoryShareScope SelectedShareScope
        {
            get => _configuration.MemoryShareScope;
            set
            {
                if (_configuration.MemoryShareScope == value)
                {
                    return;
                }

                _configuration.MemoryShareScope = value;
                OnPropertyChanged();
                PersistConfiguration(L("Memory.Status.ShareScopeSaved", "记忆共享范围已保存。"));
            }
        }

        public IEnumerable<MemoryShareScopeOption> MemoryShareScopes => new[]
        {
            new MemoryShareScopeOption { Scope = MemoryShareScope.SessionFlow, DisplayName = L("Memory.ShareScope.SessionFlow", "会话流") },
            new MemoryShareScopeOption { Scope = MemoryShareScope.Agent, DisplayName = L("Memory.ShareScope.Agent", "代理") },
            new MemoryShareScopeOption { Scope = MemoryShareScope.Application, DisplayName = L("Memory.ShareScope.Application", "应用程序") }
        };

        public int MemoryRetrievalCount
        {
            get => _configuration.MemoryRetrievalCount;
            set
            {
                if (_configuration.MemoryRetrievalCount == value)
                {
                    return;
                }

                _configuration.MemoryRetrievalCount = value;
                OnPropertyChanged();
                PersistConfiguration(string.Format(L("Memory.Status.RetrievalCountSavedFormat", "记忆取回数量已设为 {0}。"), value));
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
            OnPropertyChanged(nameof(MemoryShareScopes));
        }

        private static string L(string resourceKey, string fallback)
        {
            return LocalizationRuntime.Instance.GetString(resourceKey, fallback);
        }
    }

    public sealed class MemoryShareScopeOption
    {
        public MemoryShareScope Scope { get; init; }
        public required string DisplayName { get; init; }
    }
}
