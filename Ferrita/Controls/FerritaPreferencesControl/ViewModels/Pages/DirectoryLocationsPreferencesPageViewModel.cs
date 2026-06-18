using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Forms = System.Windows.Forms;
using Ferrita.Commands;
using Ferrita.Infrastructure.Mvvm;
using Ferrita.Models.Directories;
using Ferrita.Services.Directories;
using Ferrita.Services.Localization;

namespace Ferrita.Controls.FerritaPreferencesControl.ViewModels.Pages
{
    public sealed class DirectoryLocationsPreferencesPageViewModel : ObservableObject
    {
        private readonly FerritaDirectoryRuntime _runtime;
        private DirectoriesConfiguration _configuration;
        private string _statusMessage;

        public DirectoryLocationsPreferencesPageViewModel()
        {
            _runtime = FerritaDirectoryRuntime.Instance;
            _configuration = _runtime.GetConfiguration();
            _statusMessage = L("Directories.Status.Loaded", "目录配置已加载。");

            BrowseChatSessionsDirectoryCommand = new RelayCommand(
                () => BrowseDirectory(L("Directories.Browse.ChatSessions", "选择聊天会话保存目录"), ChatSessionsDirectoryPath, path => ChatSessionsDirectoryPath = path));
            BrowseConfigurationDirectoryCommand = new RelayCommand(
                () => BrowseDirectory(L("Directories.Browse.ConfigurationFiles", "选择配置文件保存目录"), ConfigurationFilesDirectoryPath, path => ConfigurationFilesDirectoryPath = path));
            BrowseDebugDirectoryCommand = new RelayCommand(
                () => BrowseDirectory(L("Directories.Browse.Debug", "选择调试文件保存目录"), DebugDirectoryPath, path => DebugDirectoryPath = path));
            BrowseSessionFlowsDirectoryCommand = new RelayCommand(
                () => BrowseDirectory(L("Directories.Browse.SessionFlows", "选择会话流保存目录"), SessionFlowsDirectoryPath, path => SessionFlowsDirectoryPath = path));
            BrowseAerialCityDirectoryCommand = new RelayCommand(
                () => BrowseDirectory(L("Directories.Browse.AerialCity", "选择 AerialCity 目录"), AerialCityDirectoryPath, path => AerialCityDirectoryPath = path));
            OpenDirectoriesConfigurationDirectoryCommand = new RelayCommand(OpenDirectoriesConfigurationDirectory);

            LocalizationRuntime.Instance.LanguageChanged += (_, _) => RefreshLocalizedText();
        }

        public string Title => L("Directories.Page.Title", "目录位置");

        public string Description => L("Directories.Page.Description", "配置 Ferrita 的会话、配置、调试和会话流文件保存位置。");

        public string Hint => L("Directories.Page.Hint", "目录设置会立即写入默认 Configuration 目录中的 Directories.xml。");

        public string DirectoriesConfigurationFilePath => _runtime.DirectoriesConfigurationFilePath;

        public string FixedConfigurationDirectoryPath => _runtime.FixedConfigurationDirectoryPath;

        public string DefaultApplicationDirectoryPath => _runtime.DefaultApplicationDirectoryPath;

        public string ChatSessionsDirectoryPath
        {
            get => _configuration.ChatSessionsDirectoryPath;
            set => SetDirectoryPath(
                value,
                _configuration.ChatSessionsDirectoryPath,
                path => _configuration.ChatSessionsDirectoryPath = path,
                L("Directories.Status.ChatSessionsSaved", "聊天会话目录已保存。"));
        }

        public string ConfigurationFilesDirectoryPath
        {
            get => _configuration.ConfigurationDirectoryPath;
            set => SetDirectoryPath(
                value,
                _configuration.ConfigurationDirectoryPath,
                path => _configuration.ConfigurationDirectoryPath = path,
                L("Directories.Status.ConfigurationFilesSaved", "配置文件目录已保存。"));
        }

        public string DebugDirectoryPath
        {
            get => _configuration.DebugDirectoryPath;
            set => SetDirectoryPath(
                value,
                _configuration.DebugDirectoryPath,
                path => _configuration.DebugDirectoryPath = path,
                L("Directories.Status.DebugSaved", "调试文件目录已保存。"));
        }

        public string SessionFlowsDirectoryPath
        {
            get => _configuration.SessionFlowsDirectoryPath;
            set => SetDirectoryPath(
                value,
                _configuration.SessionFlowsDirectoryPath,
                path => _configuration.SessionFlowsDirectoryPath = path,
                L("Directories.Status.SessionFlowsSaved", "会话流目录已保存。"));
        }

        public string AerialCityDirectoryPath
        {
            get => _configuration.AerialCityDirectoryPath;
            set => SetDirectoryPath(
                value,
                _configuration.AerialCityDirectoryPath,
                path => _configuration.AerialCityDirectoryPath = path,
                L("Directories.Status.AerialCitySaved", "AerialCity 目录已保存。"));
        }

        public string StatusMessage
        {
            get => _statusMessage;
            private set => SetProperty(ref _statusMessage, value);
        }

        public ICommand BrowseChatSessionsDirectoryCommand { get; }

        public ICommand BrowseConfigurationDirectoryCommand { get; }

        public ICommand BrowseDebugDirectoryCommand { get; }

        public ICommand BrowseSessionFlowsDirectoryCommand { get; }

        public ICommand BrowseAerialCityDirectoryCommand { get; }

        public ICommand OpenDirectoriesConfigurationDirectoryCommand { get; }

        private void SetDirectoryPath(
            string? value,
            string currentValue,
            Action<string> applyValue,
            string successMessage,
            [CallerMemberName] string? propertyName = null)
        {
            var normalizedValue = value?.Trim() ?? string.Empty;
            if (string.Equals(currentValue, normalizedValue, StringComparison.Ordinal))
            {
                return;
            }

            applyValue(normalizedValue);
            OnPropertyChanged(propertyName);
            PersistConfiguration(successMessage);
        }

        private void BrowseDirectory(string description, string currentPath, Action<string> applySelectedPath)
        {
            try
            {
                var selectedPath = Directory.Exists(currentPath)
                    ? currentPath
                    : DefaultApplicationDirectoryPath;

                using var dialog = new Forms.FolderBrowserDialog
                {
                    Description = description,
                    SelectedPath = selectedPath,
                    ShowNewFolderButton = true
                };

                if (dialog.ShowDialog() == Forms.DialogResult.OK)
                {
                    applySelectedPath(dialog.SelectedPath);
                }
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format(L("Directories.Status.BrowseFailedFormat", "选择目录失败：{0}"), ex.Message);
            }
        }

        private void OpenDirectoriesConfigurationDirectory()
        {
            try
            {
                var directoryPath = Path.GetDirectoryName(DirectoriesConfigurationFilePath) ?? FixedConfigurationDirectoryPath;
                Directory.CreateDirectory(directoryPath);
                Process.Start(new ProcessStartInfo
                {
                    FileName = directoryPath,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format(L("Directories.Status.OpenDirectoryFailedFormat", "打开目录失败：{0}"), ex.Message);
            }
        }

        private void PersistConfiguration(string successMessage)
        {
            try
            {
                _runtime.SaveConfiguration(_configuration);
                _configuration = _runtime.GetConfiguration();
                RefreshDirectoryProperties();
                StatusMessage = successMessage;
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format(L("Localization.Status.SaveFailedFormat", "保存失败：{0}"), ex.Message);
            }
        }

        private void RefreshDirectoryProperties()
        {
            OnPropertyChanged(nameof(ChatSessionsDirectoryPath));
            OnPropertyChanged(nameof(ConfigurationFilesDirectoryPath));
            OnPropertyChanged(nameof(DebugDirectoryPath));
            OnPropertyChanged(nameof(SessionFlowsDirectoryPath));
            OnPropertyChanged(nameof(AerialCityDirectoryPath));
            OnPropertyChanged(nameof(DirectoriesConfigurationFilePath));
            OnPropertyChanged(nameof(FixedConfigurationDirectoryPath));
            OnPropertyChanged(nameof(DefaultApplicationDirectoryPath));
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
