using System.Diagnostics;
using System.IO;
using System.Windows.Input;
using Skyweaver.Commands;
using Skyweaver.Infrastructure.Mvvm;
using Skyweaver.Models.PresentationUI;
using Skyweaver.Services.Localization;
using Skyweaver.Services.PresentationUI;

namespace Skyweaver.Controls.PresentationUIConfigurationControl.ViewModels
{
    public sealed class PresentationUIConfigurationControlViewModel : ObservableObject
    {
        private readonly PresentationUIRuntime _runtime;
        private readonly PresentationUIConfiguration _configuration;
        private string _statusMessage;

        public PresentationUIConfigurationControlViewModel()
        {
            _runtime = PresentationUIRuntime.Instance;
            _configuration = _runtime.GetConfiguration();
            _statusMessage = L("Common.Status.ConfigurationLoaded", "配置已加载。");

            OpenConfigurationDirectoryCommand = new RelayCommand(OpenConfigurationDirectory);
            LocalizationRuntime.Instance.LanguageChanged += (_, _) => RefreshLocalizedText();
        }

        public string Title => L("ChatSession.Page.Title", "聊天会话");

        public string Description => L("ChatSession.Page.Description", "配置聊天会话中的思维链呈现方式。");

        public string Hint => L("ChatSession.Page.Hint", "修改后会立即写入 PresentationUI.xml。");

        public string ConfigurationFilePath => _runtime.ConfigurationFilePath;

        public bool CollapseReasoningByDefault
        {
            get => _configuration.CollapseReasoningByDefault;
            set
            {
                if (_configuration.CollapseReasoningByDefault == value)
                {
                    return;
                }

                _configuration.CollapseReasoningByDefault = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DefaultReasoningBehaviorText));
                PersistConfiguration(L("ChatSession.Status.Saved", "聊天会话呈现设置已保存。"));
            }
        }

        public string DefaultReasoningBehaviorText => CollapseReasoningByDefault
            ? L("ChatSession.DefaultReasoning.Collapsed", "新的可折叠思维链默认收起。")
            : L("ChatSession.DefaultReasoning.Expanded", "新的可折叠思维链默认展开。");

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
            OnPropertyChanged(nameof(DefaultReasoningBehaviorText));
        }

        private static string L(string resourceKey, string fallback)
        {
            return LocalizationRuntime.Instance.GetString(resourceKey, fallback);
        }
    }
}
