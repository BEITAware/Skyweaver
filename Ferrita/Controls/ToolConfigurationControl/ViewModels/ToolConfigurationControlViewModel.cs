using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Xml;
using System.Xml.Linq;
using Ferrita.Commands;
using Ferrita.Infrastructure.Mvvm;
using Ferrita.Services.Localization;
using Ferrita.Services.FerritaTools;

namespace Ferrita.Controls.ToolConfigurationControl.ViewModels
{
    public sealed class ToolConfigurationControlViewModel : ObservableObject
    {
        public sealed class ToolParameterItemViewModel : ObservableObject
        {
            private readonly string _toolName;
            private readonly string _description;

            public ToolParameterItemViewModel(string toolName, FerritaToolParameterDefinition definition)
            {
                _toolName = toolName;
                Name = definition.Name;
                _description = definition.Description?.Trim() ?? string.Empty;
                ParameterType = definition.ParameterType;
                IsRequired = definition.IsRequired;
                DefaultValue = definition.DefaultValue;
            }

            public string Name { get; }

            public string DisplayName => L($"Tool.{_toolName}.Param.{Name}.DisplayName", Name);

            public string Description => string.IsNullOrWhiteSpace(_description)
                ? L("ToolConfiguration.Parameter.NoDescription", "未提供参数说明。")
                : L($"Tool.{_toolName}.Param.{Name}.Description", _description);

            public FerritaToolParameterType ParameterType { get; }

            public bool IsRequired { get; }

            public string? DefaultValue { get; }

            public string TypeDisplayName => GetTypeDisplayName(ParameterType);

            public string RequirementText => IsRequired
                ? L("ToolConfiguration.Parameter.Required", "必填")
                : L("ToolConfiguration.Parameter.Optional", "可选");

            public string DefaultValueText => string.IsNullOrWhiteSpace(DefaultValue)
                ? L("ToolConfiguration.Parameter.NoDefaultValue", "无默认值")
                : LF("Common.DefaultValueFormat", "默认值：{0}", DefaultValue!);

            public string ConversionHint => GetConversionHint(ParameterType);

            public string ExampleRawValue => GetExampleRawValue();

            public string ExampleRawValueText => LF("Common.ExampleFormat", "示例：{0}", ExampleRawValue);

            public void RefreshLocalizedText()
            {
                OnPropertyChanged(nameof(DisplayName));
                OnPropertyChanged(nameof(Description));
                OnPropertyChanged(nameof(TypeDisplayName));
                OnPropertyChanged(nameof(RequirementText));
                OnPropertyChanged(nameof(DefaultValueText));
                OnPropertyChanged(nameof(ConversionHint));
                OnPropertyChanged(nameof(ExampleRawValue));
                OnPropertyChanged(nameof(ExampleRawValueText));
            }

            private static string GetTypeDisplayName(FerritaToolParameterType parameterType)
            {
                return parameterType switch
                {
                    FerritaToolParameterType.Boolean => L("ToolConfiguration.Parameter.Type.Boolean", "布尔"),
                    FerritaToolParameterType.Integer => L("ToolConfiguration.Parameter.Type.Integer", "整数"),
                    FerritaToolParameterType.Number => L("ToolConfiguration.Parameter.Type.Number", "数字"),
                    FerritaToolParameterType.Json => L("ToolConfiguration.Parameter.Type.Json", "JSON"),
                    _ => L("ToolConfiguration.Parameter.Type.String", "字符串")
                };
            }

            private static string GetConversionHint(FerritaToolParameterType parameterType)
            {
                return parameterType switch
                {
                    FerritaToolParameterType.Boolean => L("ToolConfiguration.Parameter.BooleanHint", "调用参数会先按字符串接收，再规范化为 true/false、1/0、yes/no 或 on/off。"),
                    FerritaToolParameterType.Integer => L("ToolConfiguration.Parameter.IntegerHint", "调用参数会先按字符串接收，再解析为整数。"),
                    FerritaToolParameterType.Number => L("ToolConfiguration.Parameter.NumberHint", "调用参数会先按字符串接收，再解析为小数。"),
                    FerritaToolParameterType.Json => L("ToolConfiguration.Parameter.JsonHint", "调用参数会先按字符串接收，再解析为 JSON，适合数组或对象参数。"),
                    _ => L("ToolConfiguration.Parameter.StringHint", "调用参数会原样作为字符串透传。")
                };
            }

            private string GetExampleRawValue()
            {
                if (!string.IsNullOrWhiteSpace(DefaultValue))
                {
                    return DefaultValue!;
                }

                return ParameterType switch
                {
                    FerritaToolParameterType.Boolean => "false",
                    FerritaToolParameterType.Integer => "0",
                    FerritaToolParameterType.Number => "0",
                    FerritaToolParameterType.Json => "{}",
                    _ => L("ToolConfiguration.Parameter.TextExample", "<文本>")
                };
            }
        }

        public sealed class ToolItemViewModel : ObservableObject, IDisposable
        {
            private readonly FerritaToolConfigurationPresenter? _configurationPresenter;
            private readonly string _description;
            private readonly string _iconName;
            private readonly bool _hasExplicitIcon;
            private bool _isEnabled;

            public ToolItemViewModel(FerritaToolRegistration registration)
            {
                Name = registration.Definition.Name;
                _description = registration.Definition.Description?.Trim() ?? string.Empty;
                _iconName = registration.Definition.IconName?.Trim() ?? string.Empty;
                _hasExplicitIcon = !string.IsNullOrWhiteSpace(_iconName);
                IconPath = registration.IconPath;
                ImplementationTypeName = registration.ImplementationType.FullName ?? registration.ImplementationType.Name;
                CanBelongToToolKit = registration.CanBelongToToolKit;
                _isEnabled = registration.IsEnabled;
                Parameters = new ObservableCollection<ToolParameterItemViewModel>(
                    registration.Definition.Parameters.Select(parameter => new ToolParameterItemViewModel(Name, parameter)));

                _configurationPresenter = registration.CreateConfigurationPresenter();
                if (_configurationPresenter != null)
                {
                    _configurationPresenter.ConfigurationChanged += OnConfigurationPresenterChanged;
                    CustomConfigurationView = _configurationPresenter.View;
                }
            }

            public event EventHandler? CustomConfigurationChanged;

            public string Name { get; }

            public string DisplayName => L($"Tool.{Name}.DisplayName", Name);

            public string Description => string.IsNullOrWhiteSpace(_description)
                ? L("ToolConfiguration.Tool.NoDescription", "未提供工具说明。")
                : L($"Tool.{Name}.Description", _description);

            public string IconName => _hasExplicitIcon
                ? _iconName
                : L("ToolConfiguration.Tool.DefaultIcon", "默认图标");

            public string IconPath { get; }

            public string ImplementationTypeName { get; }

            public bool CanBelongToToolKit { get; }

            public ObservableCollection<ToolParameterItemViewModel> Parameters { get; }

            public FrameworkElement? CustomConfigurationView { get; }

            public string DynamicDefinitionHint => HasCustomConfiguration
                ? L("ToolConfiguration.Tool.DynamicDefinition", "运行时工具定义会基于已保存配置动态计算；修改自定义配置后，可重新加载此页以刷新这里的参数与默认值展示。")
                : L("ToolConfiguration.Tool.StaticDefinition", "当前展示的是工具的静态定义。");

            public string CustomConfigurationSummaryText => HasCustomConfiguration
                ? L("ToolConfiguration.Tool.CustomPanel", "该工具提供独立配置面板。布局、控件和交互由工具自己决定，宿主只负责承载与持久化。")
                : L("ToolConfiguration.Tool.NoCustomPanel", "该工具未提供自定义配置面板。");

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

            public string AvailabilityText => IsEnabled
                ? L("ToolConfiguration.Tool.Enabled", "已启用")
                : L("ToolConfiguration.Tool.Disabled", "已禁用");

            public string ConfigurationModeText => HasCustomConfiguration
                ? L("ToolConfiguration.Tool.CustomConfigMode", "带自定义配置")
                : L("ToolConfiguration.Tool.BasicConfigMode", "仅基础配置");

            public string ParameterCountText => Parameters.Count == 0
                ? L("ToolConfiguration.Parameter.Count.None", "无参数")
                : LF("ToolConfiguration.Parameter.Count.Format", "{0} 个参数", Parameters.Count);

            public string RequirementSummaryText
            {
                get
                {
                    if (Parameters.Count == 0)
                    {
                        return L("ToolConfiguration.Parameter.RequirementSummary.None", "无需输入");
                    }

                    var requiredCount = Parameters.Count(parameter => parameter.IsRequired);
                    var optionalCount = Parameters.Count - requiredCount;
                    return LF("ToolConfiguration.Parameter.RequirementSummary.Format", "{0} 必填 / {1} 可选", requiredCount, optionalCount);
                }
            }

            public string IconSummaryText => !_hasExplicitIcon
                ? L("ToolConfiguration.Tool.IconSummary.Default", "工具未声明显式图标，当前使用默认图标。")
                : LF("ToolConfiguration.Tool.IconSummary.Format", "图标名称：{0}", IconName);

            public string RawInvocationTemplate => BuildRawInvocationTemplate();

            public FerritaToolPersistedState CreatePersistedState()
            {
                if (_configurationPresenter == null)
                {
                    return new FerritaToolPersistedState(Name, IsEnabled, configuration: null);
                }

                if (!_configurationPresenter.TryCaptureConfiguration(out var configuration, out var errorMessage))
                {
                    throw new InvalidOperationException(
                        string.IsNullOrWhiteSpace(errorMessage)
                            ? LF("ToolConfiguration.Tool.CustomSaveFailedFormat", "工具“{0}”的自定义配置无法保存。", Name)
                            : errorMessage);
                }

                return new FerritaToolPersistedState(Name, IsEnabled, configuration);
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

            public void RefreshLocalizedText()
            {
                OnPropertyChanged(nameof(DisplayName));
                OnPropertyChanged(nameof(Description));
                OnPropertyChanged(nameof(IconName));
                OnPropertyChanged(nameof(DynamicDefinitionHint));
                OnPropertyChanged(nameof(CustomConfigurationSummaryText));
                OnPropertyChanged(nameof(AvailabilityText));
                OnPropertyChanged(nameof(ConfigurationModeText));
                OnPropertyChanged(nameof(ParameterCountText));
                OnPropertyChanged(nameof(RequirementSummaryText));
                OnPropertyChanged(nameof(IconSummaryText));

                foreach (var parameter in Parameters)
                {
                    parameter.RefreshLocalizedText();
                }
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

        private readonly FerritaToolManager _toolManager = new();
        private readonly FerritaToolKitConfigurationRepository _toolKitRepository = new();
        private readonly FerritaToolKitService _toolKitService = new();
        private bool _isLoading;
        private ToolItemViewModel? _selectedTool;
        private FerritaToolKitDefinition? _selectedToolKit;
        private string _statusMessage = string.Empty;

        public ToolConfigurationControlViewModel()
        {
            ReloadToolsCommand = new RelayCommand(LoadTools);
            EnableAllToolsCommand = new RelayCommand(() => SetAllToolsEnabled(true), () => Tools.Count > 0);
            DisableAllToolsCommand = new RelayCommand(() => SetAllToolsEnabled(false), () => Tools.Count > 0);
            AddToolKitCommand = new RelayCommand(AddToolKit);
            RemoveToolKitCommand = new RelayCommand(RemoveSelectedToolKit, () => SelectedToolKit != null);
            AddToolToToolKitCommand = new RelayCommand<FerritaToolKitDefinition>(AddToolToToolKit, toolKit => toolKit != null && ToolKitEligibleTools.Count > 0);
            RemoveToolFromToolKitCommand = new RelayCommand<FerritaToolKitEntry>(RemoveToolFromToolKit, entry => entry != null);
            MoveToolInToolKitUpCommand = new RelayCommand<FerritaToolKitEntry>(MoveToolInToolKitUp, CanMoveToolInToolKitUp);
            MoveToolInToolKitDownCommand = new RelayCommand<FerritaToolKitEntry>(MoveToolInToolKitDown, CanMoveToolInToolKitDown);

            Tools.CollectionChanged += OnToolsCollectionChanged;
            ToolKits.CollectionChanged += OnToolKitsCollectionChanged;
            LocalizationRuntime.Instance.LanguageChanged += (_, _) => RefreshLocalizedText();

            LoadToolKits();
            LoadTools();
        }

        public string Title => L("ToolConfiguration.Title", "工具配置");

        public string Description => L(
            "ToolConfiguration.Description",
            "扫描 IFerritaTool 实现，展示运行时工具定义，并允许工具在宿主中挂载自己的配置面板与 ToolKit 编排。");

        public string DiscoveryHint => L(
            "ToolConfiguration.DiscoveryHint",
            "宿主统一保存启用状态、工具自定义配置与 ToolKit 结构；工具则可以按自己的方式提供 MVVM、动态定义、提示词描述和执行期配置读取。");

        public ObservableCollection<ToolItemViewModel> Tools { get; } = new();

        public ObservableCollection<FerritaToolKitDefinition> ToolKits { get; } = new();

        public ToolItemViewModel? SelectedTool
        {
            get => _selectedTool;
            set => SetProperty(ref _selectedTool, value);
        }

        public FerritaToolKitDefinition? SelectedToolKit
        {
            get => _selectedToolKit;
            set
            {
                if (SetProperty(ref _selectedToolKit, value))
                {
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public IReadOnlyList<ToolItemViewModel> ToolKitEligibleTools => Tools
            .Where(tool => tool.CanBelongToToolKit)
            .OrderBy(tool => tool.Name, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        public string SummaryText => Tools.Count == 0
            ? L("ToolConfiguration.Summary.NoTools", "未发现工具。")
            : LF(
                "ToolConfiguration.Summary.Format",
                "共发现 {0} 个工具，已启用 {1} 个，其中 {2} 个带自定义配置。",
                Tools.Count,
                Tools.Count(tool => tool.IsEnabled),
                Tools.Count(tool => tool.HasCustomConfiguration));

        public string ToolKitSummaryText
        {
            get
            {
                if (ToolKits.Count == 0)
                {
                    return L(
                        "ToolConfiguration.ToolKitSummary.Empty",
                        "当前尚未配置任何 ToolKit。加入 ToolKit 的工具不会在代理循环开始时默认提供给 LLM。");
                }

                var groupedToolCount = ToolKits.Sum(toolKit => toolKit.Tools.Count(tool => !string.IsNullOrWhiteSpace(tool.ToolName)));
                return LF(
                    "ToolConfiguration.ToolKitSummary.Format",
                    "当前已配置 {0} 个 ToolKit，累计包含 {1} 条工具引用。",
                    ToolKits.Count,
                    groupedToolCount);
            }
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value ?? string.Empty);
        }

        public bool HasTools => Tools.Count > 0;

        public ICommand ReloadToolsCommand { get; }

        public ICommand EnableAllToolsCommand { get; }

        public ICommand DisableAllToolsCommand { get; }

        public ICommand AddToolKitCommand { get; }

        public ICommand RemoveToolKitCommand { get; }

        public ICommand AddToolToToolKitCommand { get; }

        public ICommand RemoveToolFromToolKitCommand { get; }

        public ICommand MoveToolInToolKitUpCommand { get; }

        public ICommand MoveToolInToolKitDownCommand { get; }

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
                    ? L("ToolConfiguration.Status.NoTools", "未找到工具。请在 Tools 目录下添加实现 IFerritaTool 的 .cs 文件。")
                    : LF("ToolConfiguration.Status.LoadedFormat", "已加载 {0} 个工具。", Tools.Count);
            }
            catch (Exception ex)
            {
                SelectedTool = null;
                DisposeToolItems();
                Tools.Clear();
                StatusMessage = LF("ToolConfiguration.Status.LoadFailedFormat", "工具加载失败：{0}", ex.Message);
            }
            finally
            {
                _isLoading = false;
                RefreshState();
            }
        }

        private void LoadToolKits()
        {
            _isLoading = true;
            var selectedToolKitKey = SelectedToolKit?.Key;

            try
            {
                DisposeToolKits();
                ToolKits.Clear();

                foreach (var toolKit in _toolKitService.Load())
                {
                    AttachToolKit(toolKit);
                    ToolKits.Add(toolKit);
                }

                SelectedToolKit = ToolKits.FirstOrDefault(toolKit =>
                        string.Equals(toolKit.Key, selectedToolKitKey, StringComparison.OrdinalIgnoreCase))
                    ?? ToolKits.FirstOrDefault();
            }
            catch (Exception ex)
            {
                DisposeToolKits();
                ToolKits.Clear();
                SelectedToolKit = null;
                StatusMessage = LF("ToolConfiguration.Status.ToolKitLoadFailedFormat", "ToolKit 加载失败：{0}", ex.Message);
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

            PersistTools(item.IsEnabled
                ? LF("ToolConfiguration.Status.ToolEnabledFormat", "工具“{0}”已启用。", item.Name)
                : LF("ToolConfiguration.Status.ToolDisabledFormat", "工具“{0}”已禁用。", item.Name));
        }

        private void OnToolItemCustomConfigurationChanged(object? sender, EventArgs e)
        {
            if (_isLoading || sender is not ToolItemViewModel item)
            {
                return;
            }

            PersistTools(LF("ToolConfiguration.Status.ToolCustomSavedFormat", "工具“{0}”的自定义配置已保存。", item.Name));
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

            PersistTools(isEnabled
                ? L("ToolConfiguration.Status.AllEnabled", "已启用全部工具。")
                : L("ToolConfiguration.Status.AllDisabled", "已禁用全部工具。"));
        }

        private void AddToolKit()
        {
            var toolKit = new FerritaToolKitDefinition
            {
                Name = LF("ToolConfiguration.NewToolKitNameFormat", "ToolKit {0}", ToolKits.Count + 1)
            };

            AttachToolKit(toolKit);
            ToolKits.Add(toolKit);
            SelectedToolKit = toolKit;
            PersistToolKits(L("ToolConfiguration.Status.ToolKitAdded", "ToolKit 已新增并保存。"));
        }

        private void RemoveSelectedToolKit()
        {
            if (SelectedToolKit == null)
            {
                return;
            }

            if (SelectedToolKit.IsDefaultToolKit)
            {
                foreach (var entry in SelectedToolKit.Tools.ToArray())
                {
                    DetachToolKitEntry(entry);
                }

                SelectedToolKit.Tools.Clear();
                PersistToolKits(L("ToolConfiguration.Status.DefaultToolKitSaved", "Default ToolKit membership override saved."));
                return;
            }

            DetachToolKit(SelectedToolKit);
            ToolKits.Remove(SelectedToolKit);
            SelectedToolKit = ToolKits.FirstOrDefault();
            PersistToolKits(L("ToolConfiguration.Status.ToolKitRemoved", "ToolKit 已删除并保存。"));
        }

        private void AddToolToToolKit(FerritaToolKitDefinition? toolKit)
        {
            if (toolKit == null || ToolKitEligibleTools.Count == 0)
            {
                return;
            }

            var entry = new FerritaToolKitEntry
            {
                ToolName = ToolKitEligibleTools[0].Name
            };

            AttachToolKitEntry(entry);
            toolKit.Tools.Add(entry);
            PersistToolKits(L("ToolConfiguration.Status.ToolKitOrderSaved", "ToolKit 中的工具顺序已保存。"));
        }

        private void RemoveToolFromToolKit(FerritaToolKitEntry? entry)
        {
            if (entry == null)
            {
                return;
            }

            var toolKit = FindParentToolKit(entry);
            if (toolKit == null)
            {
                return;
            }

            DetachToolKitEntry(entry);
            toolKit.Tools.Remove(entry);
            PersistToolKits(L("ToolConfiguration.Status.ToolKitOrderSaved", "ToolKit 中的工具顺序已保存。"));
        }

        private bool CanMoveToolInToolKitUp(FerritaToolKitEntry? entry)
        {
            var toolKit = entry == null ? null : FindParentToolKit(entry);
            return toolKit != null && toolKit.Tools.IndexOf(entry!) > 0;
        }

        private void MoveToolInToolKitUp(FerritaToolKitEntry? entry)
        {
            if (entry == null)
            {
                return;
            }

            var toolKit = FindParentToolKit(entry);
            if (toolKit == null)
            {
                return;
            }

            var index = toolKit.Tools.IndexOf(entry);
            if (index <= 0)
            {
                return;
            }

            toolKit.Tools.Move(index, index - 1);
            PersistToolKits(L("ToolConfiguration.Status.ToolKitOrderSaved", "ToolKit 中的工具顺序已保存。"));
        }

        private bool CanMoveToolInToolKitDown(FerritaToolKitEntry? entry)
        {
            var toolKit = entry == null ? null : FindParentToolKit(entry);
            return toolKit != null && toolKit.Tools.IndexOf(entry!) < toolKit.Tools.Count - 1;
        }

        private void MoveToolInToolKitDown(FerritaToolKitEntry? entry)
        {
            if (entry == null)
            {
                return;
            }

            var toolKit = FindParentToolKit(entry);
            if (toolKit == null)
            {
                return;
            }

            var index = toolKit.Tools.IndexOf(entry);
            if (index < 0 || index >= toolKit.Tools.Count - 1)
            {
                return;
            }

            toolKit.Tools.Move(index, index + 1);
            PersistToolKits(L("ToolConfiguration.Status.ToolKitOrderSaved", "ToolKit 中的工具顺序已保存。"));
        }

        private void PersistTools(string successMessage)
        {
            try
            {
                _toolManager.SaveConfiguration(Tools.Select(tool => tool.CreatePersistedState()));
                StatusMessage = successMessage;
            }
            catch (Exception ex)
            {
                StatusMessage = LF("ToolConfiguration.Status.SaveToolsFailedFormat", "保存工具配置失败：{0}", ex.Message);
            }
            finally
            {
                RefreshState();
            }
        }

        private void PersistToolKits(string successMessage)
        {
            if (_isLoading)
            {
                return;
            }

            try
            {
                _toolKitRepository.Save(ToolKits);
                StatusMessage = successMessage;
            }
            catch (Exception ex)
            {
                StatusMessage = LF("ToolConfiguration.Status.SaveToolKitFailedFormat", "保存 ToolKit 配置失败：{0}", ex.Message);
            }
            finally
            {
                RefreshState();
            }
        }

        private void AttachToolKit(FerritaToolKitDefinition toolKit)
        {
            toolKit.PropertyChanged -= OnToolKitPropertyChanged;
            toolKit.PropertyChanged += OnToolKitPropertyChanged;
            toolKit.Tools.CollectionChanged -= OnToolKitEntriesCollectionChanged;
            toolKit.Tools.CollectionChanged += OnToolKitEntriesCollectionChanged;

            foreach (var entry in toolKit.Tools)
            {
                AttachToolKitEntry(entry);
            }
        }

        private void DetachToolKit(FerritaToolKitDefinition toolKit)
        {
            toolKit.PropertyChanged -= OnToolKitPropertyChanged;
            toolKit.Tools.CollectionChanged -= OnToolKitEntriesCollectionChanged;

            foreach (var entry in toolKit.Tools)
            {
                DetachToolKitEntry(entry);
            }
        }

        private void AttachToolKitEntry(FerritaToolKitEntry entry)
        {
            entry.PropertyChanged -= OnToolKitEntryPropertyChanged;
            entry.PropertyChanged += OnToolKitEntryPropertyChanged;
        }

        private void DetachToolKitEntry(FerritaToolKitEntry entry)
        {
            entry.PropertyChanged -= OnToolKitEntryPropertyChanged;
        }

        private void OnToolKitPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            PersistToolKits(L("ToolConfiguration.Status.ToolKitSaved", "ToolKit 配置已保存。"));
        }

        private void OnToolKitEntriesCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (FerritaToolKitEntry entry in e.NewItems)
                {
                    AttachToolKitEntry(entry);
                }
            }

            if (e.OldItems != null)
            {
                foreach (FerritaToolKitEntry entry in e.OldItems)
                {
                    DetachToolKitEntry(entry);
                }
            }

            PersistToolKits(L("ToolConfiguration.Status.ToolKitSaved", "ToolKit 配置已保存。"));
            CommandManager.InvalidateRequerySuggested();
        }

        private void OnToolKitEntryPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            PersistToolKits(L("ToolConfiguration.Status.ToolKitSaved", "ToolKit 配置已保存。"));
        }

        private FerritaToolKitDefinition? FindParentToolKit(FerritaToolKitEntry entry)
        {
            return ToolKits.FirstOrDefault(toolKit => toolKit.Tools.Contains(entry));
        }

        private void OnToolsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(ToolKitEligibleTools));
            RefreshState();
        }

        private void OnToolKitsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (FerritaToolKitDefinition toolKit in e.NewItems)
                {
                    AttachToolKit(toolKit);
                }
            }

            if (e.OldItems != null)
            {
                foreach (FerritaToolKitDefinition toolKit in e.OldItems)
                {
                    DetachToolKit(toolKit);
                }
            }

            OnPropertyChanged(nameof(ToolKits));
            OnPropertyChanged(nameof(ToolKitSummaryText));
            CommandManager.InvalidateRequerySuggested();
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

        private void DisposeToolKits()
        {
            foreach (var toolKit in ToolKits)
            {
                DetachToolKit(toolKit);
            }
        }

        private void RefreshState()
        {
            OnPropertyChanged(nameof(HasTools));
            OnPropertyChanged(nameof(SummaryText));
            OnPropertyChanged(nameof(ToolKitSummaryText));
            OnPropertyChanged(nameof(ToolKitEligibleTools));
            CommandManager.InvalidateRequerySuggested();
        }

        private void RefreshLocalizedText()
        {
            OnPropertyChanged(nameof(Title));
            OnPropertyChanged(nameof(Description));
            OnPropertyChanged(nameof(DiscoveryHint));

            foreach (var tool in Tools)
            {
                tool.RefreshLocalizedText();
            }

            foreach (var toolKit in ToolKits)
            {
                toolKit.RefreshLocalizedText();
            }

            RefreshState();
        }

        private static string L(string resourceKey, string fallback)
        {
            return LocalizationRuntime.Instance.GetString(resourceKey, fallback);
        }

        private static string LF(string resourceKey, string fallback, params object[] args)
        {
            return string.Format(L(resourceKey, fallback), args);
        }
    }
}
