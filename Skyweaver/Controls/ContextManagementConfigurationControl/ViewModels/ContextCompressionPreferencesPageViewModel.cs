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
    public sealed class ContextCompressionPreferencesPageViewModel : ObservableObject
    {
        private readonly ContextManagementRuntime _runtime;
        private readonly ContextManagementConfiguration _configuration;
        private string _statusMessage;

        public ContextCompressionPreferencesPageViewModel()
        {
            _runtime = ContextManagementRuntime.Instance;
            _configuration = _runtime.GetConfiguration();
            _statusMessage = L("Common.Status.ConfigurationLoaded", "配置已加载。");

            OpenConfigurationDirectoryCommand = new RelayCommand(OpenConfigurationDirectory);
            LocalizationRuntime.Instance.LanguageChanged += (_, _) => RefreshLocalizedText();
        }

        public string Title => L("ContextCompression.Page.Title", "压缩");

        public string Description => L("ContextCompression.Page.Description", "配置上下文压缩与上下文生命周期相关的保留开关。");

        public string Hint => L("ContextCompression.Page.Hint", "修改后会立即写入 ContextManagement.xml。");

        public string ConfigurationFilePath => _runtime.ConfigurationFilePath;

        public bool MinCompactionEnabled
        {
            get => _configuration.MinCompactionEnabled;
            set
            {
                if (_configuration.MinCompactionEnabled == value)
                {
                    return;
                }

                _configuration.MinCompactionEnabled = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(MinCompactionBehaviorText));
                PersistConfiguration(L("ContextCompression.Status.MinCompactionSaved", "MinCompaction 设置已保存。"));
            }
        }

        public bool MaxCompactionEnabled
        {
            get => _configuration.MaxCompactionEnabled;
            set
            {
                if (_configuration.MaxCompactionEnabled == value)
                {
                    return;
                }

                _configuration.MaxCompactionEnabled = value;
                OnPropertyChanged();
                PersistConfiguration(L("ContextCompression.Status.MaxCompactionSaved", "MaxCompaction 设置已保存。"));
            }
        }

        public bool LifeCycleEnabled
        {
            get => _configuration.LifeCycleEnabled;
            set
            {
                if (_configuration.LifeCycleEnabled == value)
                {
                    return;
                }

                _configuration.LifeCycleEnabled = value;
                OnPropertyChanged();
                PersistConfiguration(L("ContextCompression.Status.LifeCycleSaved", "LifeCycle 设置已保存。"));
            }
        }

        public double LifeCycleRatioPercent
        {
            get => _configuration.LifeCycleRatioPercent;
            set
            {
                var normalized = Math.Clamp(value, 10d, 500d);
                if (Math.Abs(_configuration.LifeCycleRatioPercent - normalized) < 0.01d)
                {
                    return;
                }

                _configuration.LifeCycleRatioPercent = normalized;
                OnPropertyChanged();
                OnPropertyChanged(nameof(LifeCycleRatioDisplayText));
                PersistConfiguration(L("ContextCompression.Status.LifeCycleRatioSaved", "LifeCycle 比率已保存。"));
            }
        }

        public bool RnnOptimizedCompactionEnabled
        {
            get => _configuration.RnnOptimizedCompactionEnabled;
            set
            {
                if (_configuration.RnnOptimizedCompactionEnabled == value)
                {
                    return;
                }

                _configuration.RnnOptimizedCompactionEnabled = value;
                OnPropertyChanged();
                PersistConfiguration(L("ContextCompression.Status.RnnOptimizationSaved", "循环神经网络优化压缩设置已保存。"));
            }
        }

        public string MinCompactionBehaviorText => MinCompactionEnabled
            ? L("ContextCompression.MinCompaction.EnabledText", "上下文达到 80% 后会在后台压缩过时工具调用。")
            : L("ContextCompression.MinCompaction.DisabledText", "MinCompaction 当前关闭。");

        public string LifeCycleRatioDisplayText => $"{Math.Round(LifeCycleRatioPercent):0}%";

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
            OnPropertyChanged(nameof(MinCompactionBehaviorText));
        }

        private static string L(string resourceKey, string fallback)
        {
            return LocalizationRuntime.Instance.GetString(resourceKey, fallback);
        }
    }
}
