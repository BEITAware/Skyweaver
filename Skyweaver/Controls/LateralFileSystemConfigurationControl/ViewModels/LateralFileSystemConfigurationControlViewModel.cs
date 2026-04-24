using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using System.Windows.Input;
using Skyweaver.Commands;
using Skyweaver.Infrastructure.Mvvm;
using Skyweaver.Models.LateralFileSystem;
using Skyweaver.Services.LateralFileSystem;

namespace Skyweaver.Controls.LateralFileSystemConfigurationControl.ViewModels
{
    public sealed class LateralFileSystemConfigurationControlViewModel : ObservableObject
    {
        private readonly LateralFileSystemRuntime _runtime;
        private readonly LateralFileSystemConfiguration _configuration;
        private string _statusMessage = "配置已加载。";
        private bool _isLoading;

        public string Title { get; } = "侧向文件系统配置";

        public string Description { get; } = "配置侧向文件系统是否启用，以及所有虚拟化根共享的工作根目录。";

        public string Hint { get; } = "前端修改后会自动写入 XML。LateralFileSystemTree.xml 仅在建立首个虚拟文件夹时创建。";

        public string ConfigurationFilePath => _runtime.ConfigurationFilePath;

        public bool IsVirtualizationBackendAvailable => _runtime.IsVirtualizationBackendAvailable;

        public string BackendAvailabilityText => _runtime.VirtualizationBackendStatusMessage;

        public bool IsEnabled
        {
            get => _configuration.IsEnabled;
            set
            {
                if (_configuration.IsEnabled == value)
                {
                    return;
                }

                _configuration.IsEnabled = value;
                OnPropertyChanged();
                PersistConfiguration("启用状态已保存。");
            }
        }

        public string WorkingRootDirectory
        {
            get => _configuration.WorkingRootDirectory;
            set
            {
                var normalizedValue = value?.Trim() ?? string.Empty;
                if (string.Equals(_configuration.WorkingRootDirectory, normalizedValue, StringComparison.Ordinal))
                {
                    return;
                }

                _configuration.WorkingRootDirectory = normalizedValue;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasWorkingRootDirectory));
                OnPropertyChanged(nameof(WorkingRootDirectoryHint));
                PersistConfiguration("工作根目录已保存。");
            }
        }

        public bool HasWorkingRootDirectory => !string.IsNullOrWhiteSpace(WorkingRootDirectory);

        public string WorkingRootDirectoryHint => HasWorkingRootDirectory
            ? "所有侧向文件系统虚拟化根都将建立在此目录下。"
            : "请选择一个工作根目录。";

        public string StatusMessage
        {
            get => _statusMessage;
            private set => SetProperty(ref _statusMessage, value);
        }

        public ICommand BrowseWorkingRootDirectoryCommand { get; }

        public ICommand OpenConfigurationDirectoryCommand { get; }

        public LateralFileSystemConfigurationControlViewModel()
        {
            _runtime = LateralFileSystemRuntime.Instance;
            _configuration = _runtime.GetConfiguration();
            _configuration.PropertyChanged += OnConfigurationPropertyChanged;

            BrowseWorkingRootDirectoryCommand = new RelayCommand(BrowseWorkingRootDirectory);
            OpenConfigurationDirectoryCommand = new RelayCommand(OpenConfigurationDirectory);

            _isLoading = true;
            OnPropertyChanged(nameof(IsEnabled));
            OnPropertyChanged(nameof(WorkingRootDirectory));
            OnPropertyChanged(nameof(HasWorkingRootDirectory));
            OnPropertyChanged(nameof(WorkingRootDirectoryHint));
            _isLoading = false;
            StatusMessage = BuildStatusMessage("配置已加载。");
        }

        private void BrowseWorkingRootDirectory()
        {
            using var dialog = new FolderBrowserDialog
            {
                Description = "选择侧向文件系统工作根目录",
                SelectedPath = HasWorkingRootDirectory && Directory.Exists(WorkingRootDirectory)
                    ? WorkingRootDirectory
                    : Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                ShowNewFolderButton = true
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                WorkingRootDirectory = dialog.SelectedPath;
            }
        }

        private void OpenConfigurationDirectory()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(ConfigurationFilePath) ?? string.Empty);
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = Path.GetDirectoryName(ConfigurationFilePath) ?? ConfigurationFilePath,
                UseShellExecute = true
            });
        }

        private void OnConfigurationPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (_isLoading)
            {
                return;
            }

            if (string.Equals(e.PropertyName, nameof(LateralFileSystemConfiguration.IsEnabled), StringComparison.Ordinal))
            {
                OnPropertyChanged(nameof(IsEnabled));
            }

            if (string.Equals(e.PropertyName, nameof(LateralFileSystemConfiguration.WorkingRootDirectory), StringComparison.Ordinal))
            {
                OnPropertyChanged(nameof(WorkingRootDirectory));
                OnPropertyChanged(nameof(HasWorkingRootDirectory));
                OnPropertyChanged(nameof(WorkingRootDirectoryHint));
            }
        }

        private void PersistConfiguration(string successMessage)
        {
            if (_isLoading)
            {
                return;
            }

            try
            {
                _runtime.SaveConfiguration(_configuration);
                OnPropertyChanged(nameof(IsVirtualizationBackendAvailable));
                OnPropertyChanged(nameof(BackendAvailabilityText));
                StatusMessage = BuildStatusMessage(successMessage);
            }
            catch (Exception ex)
            {
                StatusMessage = $"保存失败：{ex.Message}";
            }
        }

        private string BuildStatusMessage(string successMessage)
        {
            if (!_runtime.IsVirtualizationBackendAvailable)
            {
                return _runtime.VirtualizationBackendStatusMessage;
            }

            return successMessage;
        }
    }
}
