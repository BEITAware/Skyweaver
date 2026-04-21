using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Xml;
using System.Xml.Linq;
using Skyweaver.Commands;
using Skyweaver.Infrastructure.Mvvm;
using Skyweaver.Services.SkyweaverTools;

namespace Skyweaver.Controls.ToolConfigurationControl.ViewModels
{
    public sealed class ToolConfigurationControlViewModel : ObservableObject
    {
        public sealed class ToolParameterItemViewModel
        {
            public ToolParameterItemViewModel(SkyweaverToolParameterDefinition definition)
            {
                Name = definition.Name;
                Description = string.IsNullOrWhiteSpace(definition.Description)
                    ? "未提供参数说明。"
                    : definition.Description;
                ParameterType = definition.ParameterType;
                IsRequired = definition.IsRequired;
                DefaultValue = definition.DefaultValue;
                TypeDisplayName = GetTypeDisplayName(definition.ParameterType);
                RequirementText = definition.IsRequired ? "必填" : "可选";
                DefaultValueText = string.IsNullOrWhiteSpace(definition.DefaultValue)
                    ? "无默认值"
                    : $"默认值：{definition.DefaultValue}";
                ConversionHint = GetConversionHint(definition.ParameterType);
                ExampleRawValue = GetExampleRawValue(definition);
            }

            public string Name { get; }

            public string Description { get; }

            public SkyweaverToolParameterType ParameterType { get; }

            public bool IsRequired { get; }

            public string? DefaultValue { get; }

            public string TypeDisplayName { get; }

            public string RequirementText { get; }

            public string DefaultValueText { get; }

            public string ConversionHint { get; }

            public string ExampleRawValue { get; }

            private static string GetTypeDisplayName(SkyweaverToolParameterType parameterType)
            {
                return parameterType switch
                {
                    SkyweaverToolParameterType.Boolean => "布尔",
                    SkyweaverToolParameterType.Integer => "整数",
                    SkyweaverToolParameterType.Number => "数字",
                    SkyweaverToolParameterType.Json => "JSON",
                    _ => "字符串"
                };
            }

            private static string GetConversionHint(SkyweaverToolParameterType parameterType)
            {
                return parameterType switch
                {
                    SkyweaverToolParameterType.Boolean => "调用参数会先按字符串接收，再规范化为 true/false、1/0、yes/no 或 on/off。",
                    SkyweaverToolParameterType.Integer => "调用参数会先按字符串接收，再解析为整数。",
                    SkyweaverToolParameterType.Number => "调用参数会先按字符串接收，再解析为小数。",
                    SkyweaverToolParameterType.Json => "调用参数会先按字符串接收，再解析为 JSON，适合数组或对象参数。",
                    _ => "调用参数会原样作为字符串透传。"
                };
            }

            private static string GetExampleRawValue(SkyweaverToolParameterDefinition definition)
            {
                if (!string.IsNullOrWhiteSpace(definition.DefaultValue))
                {
                    return definition.DefaultValue!;
                }

                return definition.ParameterType switch
                {
                    SkyweaverToolParameterType.Boolean => "false",
                    SkyweaverToolParameterType.Integer => "0",
                    SkyweaverToolParameterType.Number => "0",
                    SkyweaverToolParameterType.Json => "{}",
                    _ => "<文本>"
                };
            }
        }

        public sealed class ToolItemViewModel : ObservableObject, IDisposable
        {
            private readonly SkyweaverToolConfigurationPresenter? _configurationPresenter;
            private bool _isEnabled;

            public ToolItemViewModel(SkyweaverToolRegistration registration)
            {
                Name = registration.Definition.Name;
                Description = string.IsNullOrWhiteSpace(registration.Definition.Description)
                    ? "未提供工具说明。"
                    : registration.Definition.Description;
                IconName = string.IsNullOrWhiteSpace(registration.Definition.IconName)
                    ? "默认图标"
                    : registration.Definition.IconName!;
                IconPath = registration.IconPath;
                ImplementationTypeName = registration.ImplementationType.FullName ?? registration.ImplementationType.Name;
                _isEnabled = registration.IsEnabled;
                Parameters = new ObservableCollection<ToolParameterItemViewModel>(
                    registration.Definition.Parameters.Select(parameter => new ToolParameterItemViewModel(parameter)));
                DynamicDefinitionHint = registration.HasCustomConfiguration
                    ? "运行时工具定义会基于已保存配置动态计算；修改自定义配置后，可重新加载此页以刷新这里的参数与默认值展示。"
                    : "当前展示的是工具的静态定义。";

                _configurationPresenter = registration.CreateConfigurationPresenter();
                if (_configurationPresenter != null)
                {
                    _configurationPresenter.ConfigurationChanged += OnConfigurationPresenterChanged;
                    CustomConfigurationView = _configurationPresenter.View;
                    CustomConfigurationSummaryText = "该工具提供独立配置面板。布局、控件和交互由工具自己决定，宿主只负责承载与持久化。";
                }
                else
                {
                    CustomConfigurationSummaryText = "该工具未提供自定义配置面板。";
                }
            }

            public event EventHandler? CustomConfigurationChanged;

            public string Name { get; }

            public string Description { get; }

            public string IconName { get; }

            public string IconPath { get; }

            public string ImplementationTypeName { get; }

            public ObservableCollection<ToolParameterItemViewModel> Parameters { get; }

            public FrameworkElement? CustomConfigurationView { get; }

            public string DynamicDefinitionHint { get; }

            public string CustomConfigurationSummaryText { get; }

            public bool HasCustomConfiguration => CustomConfigurationView != null;

            public bool IsEnabled
            {
                get => _isEnabled;
                set
                {
                    if (SetProperty(ref _isEnabled, value))
                    {
                        OnPropertyChanged(nameof(AvailabilityText));
                    }
                }
            }

            public string AvailabilityText => IsEnabled ? "已启用" : "已禁用";

            public string ConfigurationModeText => HasCustomConfiguration ? "带自定义配置" : "仅基础配置";

            public string ParameterCountText => Parameters.Count == 0 ? "无参数" : $"{Parameters.Count} 个参数";

            public string RequirementSummaryText
            {
                get
                {
                    if (Parameters.Count == 0)
                    {
                        return "无需输入";
                    }

                    var requiredCount = Parameters.Count(parameter => parameter.IsRequired);
                    var optionalCount = Parameters.Count - requiredCount;
                    return $"{requiredCount} 必填 / {optionalCount} 可选";
                }
            }

            public string IconSummaryText => string.Equals(IconName, "默认图标", StringComparison.Ordinal)
                ? "工具未声明显式图标，当前使用默认图标。"
                : $"图标名称：{IconName}";

            public string RawInvocationTemplate => BuildRawInvocationTemplate();

            public SkyweaverToolPersistedState CreatePersistedState()
            {
                if (_configurationPresenter == null)
                {
                    return new SkyweaverToolPersistedState(Name, IsEnabled, configuration: null);
                }

                if (!_configurationPresenter.TryCaptureConfiguration(out var configuration, out var errorMessage))
                {
                    throw new InvalidOperationException(
                        string.IsNullOrWhiteSpace(errorMessage)
                            ? $"工具“{Name}”的自定义配置无法保存。"
                            : errorMessage);
                }

                return new SkyweaverToolPersistedState(Name, IsEnabled, configuration);
            }

            public void Dispose()
            {
                if (_configurationPresenter == null)
                {
                    return;
                }

                _configurationPresenter.ConfigurationChanged -= OnConfigurationPresenterChanged;
                _configurationPresenter.Dispose();
            }

            private string BuildRawInvocationTemplate()
            {
                var toolElement = new XElement("Tool", new XAttribute("ToolName", Name));
                foreach (var parameter in Parameters)
                {
                    toolElement.Add(CreateParameterElement(parameter));
                }

                var document = new XDocument(new XElement("Tools", toolElement));
                return document.ToString();
            }

            private void OnConfigurationPresenterChanged(object? sender, EventArgs e)
            {
                CustomConfigurationChanged?.Invoke(this, EventArgs.Empty);
            }

            private static XElement CreateParameterElement(ToolParameterItemViewModel parameter)
            {
                if (IsValidXmlElementName(parameter.Name))
                {
                    return new XElement(parameter.Name, parameter.ExampleRawValue);
                }

                return new XElement(
                    "Parameter",
                    new XAttribute("Name", parameter.Name),
                    parameter.ExampleRawValue);
            }

            private static bool IsValidXmlElementName(string name)
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    return false;
                }

                try
                {
                    XmlConvert.VerifyName(name);
                    return true;
                }
                catch (XmlException)
                {
                    return false;
                }
            }
        }

        private readonly SkyweaverToolManager _toolManager = new();
        private bool _isLoading;
        private ToolItemViewModel? _selectedTool;
        private string _statusMessage = string.Empty;

        public ToolConfigurationControlViewModel()
        {
            ReloadToolsCommand = new RelayCommand(LoadTools);
            EnableAllToolsCommand = new RelayCommand(() => SetAllToolsEnabled(true), () => Tools.Count > 0);
            DisableAllToolsCommand = new RelayCommand(() => SetAllToolsEnabled(false), () => Tools.Count > 0);
            OpenToolDirectoryCommand = new RelayCommand(() => _toolManager.OpenDirectory(ToolDirectoryPath));
            OpenIconDirectoryCommand = new RelayCommand(() => _toolManager.OpenDirectory(IconDirectoryPath));
            OpenConfigurationDirectoryCommand = new RelayCommand(() => _toolManager.OpenDirectory(ConfigurationDirectoryPath));

            LoadTools();
        }

        public string Title { get; } = "工具配置";

        public string Description { get; } = "扫描 ISkyweaverTool 实现，展示运行时工具定义，并允许工具在宿主中挂载自己的配置面板。";

        public string DiscoveryHint { get; } = "宿主统一保存启用状态与工具自定义配置；工具则可以按自己的方式提供 MVVM、动态定义和执行期配置读取。";

        public ObservableCollection<ToolItemViewModel> Tools { get; } = new();

        public ToolItemViewModel? SelectedTool
        {
            get => _selectedTool;
            set => SetProperty(ref _selectedTool, value);
        }

        public string ToolDirectoryPath => _toolManager.ToolDirectoryPath;

        public string IconDirectoryPath => _toolManager.IconDirectoryPath;

        public string ConfigurationDirectoryPath => _toolManager.ConfigurationDirectoryPath;

        public string ConfigurationFilePath => _toolManager.ConfigurationFilePath;

        public string SummaryText => Tools.Count == 0
            ? "未发现工具。"
            : $"共发现 {Tools.Count} 个工具，已启用 {Tools.Count(tool => tool.IsEnabled)} 个，其中 {Tools.Count(tool => tool.HasCustomConfiguration)} 个带自定义配置。";

        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value ?? string.Empty);
        }

        public bool HasTools => Tools.Count > 0;

        public ICommand ReloadToolsCommand { get; }

        public ICommand EnableAllToolsCommand { get; }

        public ICommand DisableAllToolsCommand { get; }

        public ICommand OpenToolDirectoryCommand { get; }

        public ICommand OpenIconDirectoryCommand { get; }

        public ICommand OpenConfigurationDirectoryCommand { get; }

        private void LoadTools()
        {
            _isLoading = true;
            var selectedToolName = SelectedTool?.Name;

            try
            {
                DisposeToolItems();
                Tools.Clear();

                foreach (var registration in _toolManager.GetRegisteredTools().Where(item => item.CanUserDisable))
                {
                    var item = new ToolItemViewModel(registration);
                    item.PropertyChanged += OnToolItemPropertyChanged;
                    item.CustomConfigurationChanged += OnToolItemCustomConfigurationChanged;
                    Tools.Add(item);
                }

                SelectedTool = Tools.FirstOrDefault(item =>
                        string.Equals(item.Name, selectedToolName, StringComparison.OrdinalIgnoreCase))
                    ?? Tools.FirstOrDefault();

                StatusMessage = Tools.Count == 0
                    ? "未找到工具。请在 Tools 目录下添加实现 ISkyweaverTool 的 .cs 文件。"
                    : $"已加载 {Tools.Count} 个工具。";
            }
            catch (Exception ex)
            {
                SelectedTool = null;
                DisposeToolItems();
                Tools.Clear();
                StatusMessage = $"工具加载失败：{ex.Message}";
            }
            finally
            {
                _isLoading = false;
                RefreshState();
            }
        }

        private void OnToolItemPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (_isLoading || sender is not ToolItemViewModel item)
            {
                return;
            }

            if (!string.Equals(e.PropertyName, nameof(ToolItemViewModel.IsEnabled), StringComparison.Ordinal))
            {
                return;
            }

            PersistConfiguration($"工具“{item.Name}”已{(item.IsEnabled ? "启用" : "禁用")}。");
        }

        private void OnToolItemCustomConfigurationChanged(object? sender, EventArgs e)
        {
            if (_isLoading || sender is not ToolItemViewModel item)
            {
                return;
            }

            PersistConfiguration($"工具“{item.Name}”的自定义配置已保存。");
        }

        private void SetAllToolsEnabled(bool isEnabled)
        {
            if (Tools.Count == 0)
            {
                return;
            }

            _isLoading = true;

            try
            {
                foreach (var tool in Tools)
                {
                    tool.IsEnabled = isEnabled;
                }
            }
            finally
            {
                _isLoading = false;
            }

            PersistConfiguration(isEnabled ? "已启用全部工具。" : "已禁用全部工具。");
        }

        private void PersistConfiguration(string successMessage)
        {
            try
            {
                _toolManager.SaveConfiguration(Tools.Select(tool => tool.CreatePersistedState()));
                StatusMessage = successMessage;
            }
            catch (Exception ex)
            {
                StatusMessage = $"保存工具配置失败：{ex.Message}";
            }
            finally
            {
                RefreshState();
            }
        }

        private void DisposeToolItems()
        {
            foreach (var existingItem in Tools)
            {
                existingItem.PropertyChanged -= OnToolItemPropertyChanged;
                existingItem.CustomConfigurationChanged -= OnToolItemCustomConfigurationChanged;
                existingItem.Dispose();
            }
        }

        private void RefreshState()
        {
            OnPropertyChanged(nameof(HasTools));
            OnPropertyChanged(nameof(SummaryText));
            CommandManager.InvalidateRequerySuggested();
        }
    }
}
