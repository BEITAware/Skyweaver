using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Input;
using Skyweaver.Commands;
using Skyweaver.Controls.WorkflowEditorControl.Models;
using Skyweaver.Controls.WorkflowEditorControl.Services;
using Skyweaver.Infrastructure.Mvvm;
using Skyweaver.Models.ShellIntegration;
using Skyweaver.Services.Localization;
using Skyweaver.Services.ShellIntegration;

namespace Skyweaver.Controls.SkyweaverPreferencesControl.ViewModels.Pages
{
    public sealed class ShellIntegrationPreferencesPageViewModel : ObservableObject
    {
        private readonly ShellIntegrationRuntime _runtime;
        private readonly SessionFlowRepository _sessionFlowRepository;
        private ShellIntegrationConfiguration _configuration;
        private ShellIntegrationSessionFlowOptionViewModel? _selectedSessionFlowOption;
        private string _statusMessage;
        private bool _isLoadingSessionFlowOptions;

        public ShellIntegrationPreferencesPageViewModel()
        {
            _runtime = ShellIntegrationRuntime.Instance;
            _configuration = _runtime.GetConfiguration();
            _sessionFlowRepository = new SessionFlowRepository(new SessionFlowPathProvider());
            _statusMessage = L("Common.Status.ConfigurationLoaded", "配置已加载。");

            SessionFlowOptions = new ObservableCollection<ShellIntegrationSessionFlowOptionViewModel>();
            OpenConfigurationDirectoryCommand = new RelayCommand(OpenConfigurationDirectory);
            ReloadSessionFlowsCommand = new RelayCommand(() => LoadSessionFlowOptions(updateStatus: true));

            LoadSessionFlowOptions(updateStatus: false);
            LocalizationRuntime.Instance.LanguageChanged += (_, _) => RefreshLocalizedText();
        }

        public string Title => L("ShellIntegration.Page.Title", "Shell 集成");

        public string Description => L("ShellIntegration.Page.Description", "将 Skyweaver 集成到文件资源管理器右键菜单。");

        public string Hint => L("ShellIntegration.Page.Hint", "启用后会写入当前用户的资源管理器右键菜单注册表项。");

        public string ConfigurationFilePath => _runtime.ConfigurationFilePath;

        public ObservableCollection<ShellIntegrationSessionFlowOptionViewModel> SessionFlowOptions { get; }

        public bool IsShellIntegrationEnabled
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

                var successMessage = value
                    ? L("ShellIntegration.Status.Enabled", "Shell 集成已启用，右键菜单已注册。")
                    : L("ShellIntegration.Status.Disabled", "Shell 集成已禁用，右键菜单已移除。");
                PersistConfiguration(successMessage, applyRegistration: true);
            }
        }

        public ShellIntegrationSessionFlowOptionViewModel? SelectedSessionFlowOption
        {
            get => _selectedSessionFlowOption;
            set
            {
                if (!SetProperty(ref _selectedSessionFlowOption, value) || _isLoadingSessionFlowOptions)
                {
                    return;
                }

                ApplySessionFlowOption(value);
            }
        }

        public string RegistrationStatusText => _runtime.IsRegistered()
            ? L("ShellIntegration.Registration.Registered", "资源管理器右键菜单已注册。")
            : L("ShellIntegration.Registration.NotRegistered", "资源管理器右键菜单未注册。");

        public string StatusMessage
        {
            get => _statusMessage;
            private set => SetProperty(ref _statusMessage, value);
        }

        public ICommand OpenConfigurationDirectoryCommand { get; }

        public ICommand ReloadSessionFlowsCommand { get; }

        private void LoadSessionFlowOptions(bool updateStatus)
        {
            _isLoadingSessionFlowOptions = true;

            try
            {
                SessionFlowOptions.Clear();

                var noneOption = ShellIntegrationSessionFlowOptionViewModel.CreateNone(
                    L("ShellIntegration.SessionFlow.None", "未指定"),
                    L("ShellIntegration.SessionFlow.NoneDescription", "暂不为 Shell Chat 预设会话流。"));
                SessionFlowOptions.Add(noneOption);

                foreach (var document in _sessionFlowRepository.LoadAll())
                {
                    SessionFlowOptions.Add(ShellIntegrationSessionFlowOptionViewModel.FromDocument(document));
                }

                var selectedOption = FindSelectedSessionFlowOption();
                if (selectedOption == null && HasConfiguredSessionFlow())
                {
                    selectedOption = ShellIntegrationSessionFlowOptionViewModel.CreateMissing(
                        _configuration.SessionFlowGraphId,
                        _configuration.SessionFlowGraphName,
                        _configuration.SessionFlowFilePath,
                        L("ShellIntegration.SessionFlow.MissingFormat", "{0}（未找到）"));
                    SessionFlowOptions.Add(selectedOption);
                }

                SelectedSessionFlowOption = selectedOption ?? noneOption;

                if (updateStatus)
                {
                    StatusMessage = L("ShellIntegration.Status.SessionFlowsLoaded", "会话流列表已重新加载。");
                }
            }
            catch (Exception ex)
            {
                if (SessionFlowOptions.Count == 0)
                {
                    var noneOption = ShellIntegrationSessionFlowOptionViewModel.CreateNone(
                        L("ShellIntegration.SessionFlow.None", "未指定"),
                        L("ShellIntegration.SessionFlow.NoneDescription", "暂不为 Shell Chat 预设会话流。"));
                    SessionFlowOptions.Add(noneOption);
                    SelectedSessionFlowOption = noneOption;
                }

                StatusMessage = string.Format(
                    L("ShellIntegration.Status.LoadSessionFlowsFailedFormat", "加载会话流失败：{0}"),
                    ex.Message);
            }
            finally
            {
                _isLoadingSessionFlowOptions = false;
            }
        }

        private ShellIntegrationSessionFlowOptionViewModel? FindSelectedSessionFlowOption()
        {
            foreach (var option in SessionFlowOptions)
            {
                if (option.Matches(_configuration))
                {
                    return option;
                }
            }

            return null;
        }

        private bool HasConfiguredSessionFlow()
        {
            return !string.IsNullOrWhiteSpace(_configuration.SessionFlowGraphId) ||
                   !string.IsNullOrWhiteSpace(_configuration.SessionFlowGraphName) ||
                   !string.IsNullOrWhiteSpace(_configuration.SessionFlowFilePath);
        }

        private void ApplySessionFlowOption(ShellIntegrationSessionFlowOptionViewModel? option)
        {
            _configuration.SessionFlowGraphId = option?.GraphId ?? string.Empty;
            _configuration.SessionFlowGraphName = option?.GraphName ?? string.Empty;
            _configuration.SessionFlowFilePath = option?.FilePath ?? string.Empty;

            PersistConfiguration(
                L("ShellIntegration.Status.SessionFlowSaved", "Shell Chat 使用的会话流已保存。"),
                applyRegistration: false);
        }

        private void PersistConfiguration(string successMessage, bool applyRegistration)
        {
            try
            {
                _runtime.SaveConfiguration(_configuration);

                if (applyRegistration)
                {
                    var result = _runtime.ApplyConfiguredRegistration();
                    if (!result.Succeeded)
                    {
                        StatusMessage = string.Format(
                            L("ShellIntegration.Status.ApplyFailedFormat", "更新右键菜单失败：{0}"),
                            result.ErrorMessage);
                        RefreshRuntimeProperties();
                        return;
                    }
                }

                _configuration = _runtime.GetConfiguration();
                StatusMessage = successMessage;
                RefreshRuntimeProperties();
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format(L("Localization.Status.SaveFailedFormat", "保存失败：{0}"), ex.Message);
            }
        }

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
                StatusMessage = string.Format(
                    L("Common.Status.OpenConfigurationDirectoryFailedFormat", "打开配置目录失败：{0}"),
                    ex.Message);
            }
        }

        private void RefreshRuntimeProperties()
        {
            OnPropertyChanged(nameof(ConfigurationFilePath));
            OnPropertyChanged(nameof(RegistrationStatusText));
        }

        private void RefreshLocalizedText()
        {
            OnPropertyChanged(nameof(Title));
            OnPropertyChanged(nameof(Description));
            OnPropertyChanged(nameof(Hint));
            OnPropertyChanged(nameof(RegistrationStatusText));
            LoadSessionFlowOptions(updateStatus: false);
        }

        private static string L(string resourceKey, string fallback)
        {
            return LocalizationRuntime.Instance.GetString(resourceKey, fallback);
        }
    }

    public sealed class ShellIntegrationSessionFlowOptionViewModel
    {
        private ShellIntegrationSessionFlowOptionViewModel()
        {
        }

        public string GraphId { get; private init; } = string.Empty;

        public string GraphName { get; private init; } = string.Empty;

        public string FilePath { get; private init; } = string.Empty;

        public string DisplayName { get; private init; } = string.Empty;

        public string Description { get; private init; } = string.Empty;

        public bool IsNone { get; private init; }

        public static ShellIntegrationSessionFlowOptionViewModel CreateNone(string displayName, string description)
        {
            return new ShellIntegrationSessionFlowOptionViewModel
            {
                DisplayName = displayName,
                Description = description,
                IsNone = true
            };
        }

        public static ShellIntegrationSessionFlowOptionViewModel CreateMissing(
            string graphId,
            string graphName,
            string filePath,
            string displayNameFormat)
        {
            var fallbackName = string.IsNullOrWhiteSpace(graphName)
                ? Path.GetFileNameWithoutExtension(filePath)
                : graphName;
            if (string.IsNullOrWhiteSpace(fallbackName))
            {
                fallbackName = graphId;
            }

            if (string.IsNullOrWhiteSpace(fallbackName))
            {
                fallbackName = "Session Flow";
            }

            return new ShellIntegrationSessionFlowOptionViewModel
            {
                GraphId = graphId?.Trim() ?? string.Empty,
                GraphName = graphName?.Trim() ?? string.Empty,
                FilePath = filePath?.Trim() ?? string.Empty,
                DisplayName = string.Format(displayNameFormat, fallbackName),
                Description = filePath?.Trim() ?? string.Empty
            };
        }

        public static ShellIntegrationSessionFlowOptionViewModel FromDocument(SessionFlowGraphDocumentModel document)
        {
            ArgumentNullException.ThrowIfNull(document);

            return new ShellIntegrationSessionFlowOptionViewModel
            {
                GraphId = document.GraphId,
                GraphName = document.Name,
                FilePath = document.FilePath,
                DisplayName = document.Name,
                Description = Path.GetFileName(document.FilePath)
            };
        }

        public bool Matches(ShellIntegrationConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(configuration);

            if (IsNone)
            {
                return string.IsNullOrWhiteSpace(configuration.SessionFlowGraphId) &&
                       string.IsNullOrWhiteSpace(configuration.SessionFlowGraphName) &&
                       string.IsNullOrWhiteSpace(configuration.SessionFlowFilePath);
            }

            if (!string.IsNullOrWhiteSpace(GraphId) &&
                string.Equals(GraphId, configuration.SessionFlowGraphId, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (!string.IsNullOrWhiteSpace(FilePath) &&
                string.Equals(FilePath, configuration.SessionFlowFilePath, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return !string.IsNullOrWhiteSpace(GraphName) &&
                   string.Equals(GraphName, configuration.SessionFlowGraphName, StringComparison.CurrentCultureIgnoreCase);
        }
    }
}
