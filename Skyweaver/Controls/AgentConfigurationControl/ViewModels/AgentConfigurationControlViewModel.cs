using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Input;
using System.Xml;
using Microsoft.Win32;
using Skyweaver.Commands;
using Skyweaver.Controls.AgentConfigurationControl.Models;
using Skyweaver.Controls.AgentConfigurationControl.Services;
using Skyweaver.Controls.LanguageModelConfigurationControl.Models;
using Skyweaver.Controls.LanguageModelConfigurationControl.Services;
using Skyweaver.Infrastructure.Mvvm;
using Skyweaver.Services;
using Skyweaver.Services.Localization;
using Skyweaver.Services.SkyweaverTools;

namespace Skyweaver.Controls.AgentConfigurationControl.ViewModels
{
    public sealed class AgentConfigurationControlViewModel : ObservableObject
    {
        private static readonly Regex s_agentIdPattern = new("^[A-Za-z0-9]+$", RegexOptions.Compiled);

        private readonly AgentConfigurationPathProvider _pathProvider;
        private readonly AgentConfigurationRepository _configurationRepository;
        private readonly LanguageModelConfigurationRepository _languageModelRepository;
        private readonly CapabilityLayerConfigurationRepository _capabilityLayerRepository;
        private readonly AgentSystemPromptBuilder _agentSystemPromptBuilder = new();
        private readonly SkyweaverToolManager _toolManager = new();
        private readonly SkyweaverToolKitService _toolKitService = new();
        private readonly Dictionary<string, ToolRegistrationSnapshot> _toolRegistrationMap = new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, LanguageModelDefinition> _languageModelMap = new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, CapabilityLayerDefinition> _capabilityLayerMap = new(StringComparer.OrdinalIgnoreCase);
        private int _suspendPersistenceCounter;
        private AgentDefinition? _selectedAgent;
        private XmlElementNodeDefinition? _selectedInputNode;
        private XmlElementNodeDefinition? _selectedOutputNode;
        private string _statusMessage = string.Empty;
        private bool _hasValidationError;
        private string _languageModelCatalogErrorMessage = string.Empty;
        private string _promptBuildResult = string.Empty;
        private string _promptBuildStatusMessage = L("AgentConfiguration.Status.InitialPrompt", "请选择代理后点击“构建提示词”。");
        private bool _hasPromptBuildError;

        private sealed class ToolRegistrationSnapshot
        {
            public ToolRegistrationSnapshot(string name, string description, bool isEnabled)
            {
                Name = name;
                Description = description;
                IsEnabled = isEnabled;
            }

            public string Name { get; }

            public string Description { get; }

            public bool IsEnabled { get; }
        }

        private sealed class DelegateDisposable : IDisposable
        {
            private readonly Action _disposeAction;
            private bool _isDisposed;

            public DelegateDisposable(Action disposeAction)
            {
                _disposeAction = disposeAction;
            }

            public void Dispose()
            {
                if (_isDisposed)
                {
                    return;
                }

                _isDisposed = true;
                _disposeAction();
            }
        }

        public AgentConfigurationControlViewModel()
        {
            _pathProvider = new AgentConfigurationPathProvider();
            _configurationRepository = new AgentConfigurationRepository(_pathProvider);
            var languageModelPathProvider = new LanguageModelConfigurationPathProvider();
            _languageModelRepository = new LanguageModelConfigurationRepository(languageModelPathProvider);
            _capabilityLayerRepository = new CapabilityLayerConfigurationRepository(languageModelPathProvider);

            AddAgentCommand = new RelayCommand(AddAgent);
            DuplicateAgentCommand = new RelayCommand(DuplicateSelectedAgent, () => SelectedAgent != null);
            RemoveAgentCommand = new RelayCommand(RemoveSelectedAgent, () => SelectedAgent != null);
            BrowseAvatarCommand = new RelayCommand(BrowseAvatar, () => SelectedAgent != null);
            OpenConfigurationDirectoryCommand = new RelayCommand(OpenConfigurationDirectory);
            ReloadToolsCommand = new RelayCommand(ReloadTools);
            AllowAllToolPermissionsCommand = new RelayCommand(
                () => SetAllToolPermissions(
                    AgentToolPermissionMode.Allow,
                    L("AgentConfiguration.Status.AllToolsAllowed", "已将当前代理的全部工具权限设为允许。")),
                () => SelectedAgent != null);
            DisableAllToolPermissionsCommand = new RelayCommand(
                () => SetAllToolPermissions(
                    AgentToolPermissionMode.Disabled,
                    L("AgentConfiguration.Status.AllToolsDisabled", "已将当前代理的全部工具权限设为禁止。")),
                () => SelectedAgent != null);
            RequireConfirmationAllToolPermissionsCommand = new RelayCommand(
                () => SetAllToolPermissions(
                    AgentToolPermissionMode.RequireConfirmation,
                    L("AgentConfiguration.Status.AllToolsRequireConfirmation", "已将当前代理的全部工具权限设为需确认。")),
                () => SelectedAgent != null);
            ReloadLanguageModelCatalogCommand = new RelayCommand(ReloadLanguageModelCatalog);
            AddInputRootNodeCommand = new RelayCommand(AddInputRootNode, () => SelectedAgent != null);
            AddOutputRootNodeCommand = new RelayCommand(AddOutputRootNode, () => SelectedAgent != null);
            AddInputChildNodeCommand = new RelayCommand(AddInputChildNode, () => SelectedInputNode != null);
            AddOutputChildNodeCommand = new RelayCommand(AddOutputChildNode, () => SelectedOutputNode != null);
            RemoveInputNodeCommand = new RelayCommand(RemoveSelectedInputNode, () => SelectedInputNode != null && !SelectedInputNode.IsRoot);
            RemoveOutputNodeCommand = new RelayCommand(RemoveSelectedOutputNode, () => SelectedOutputNode != null && !SelectedOutputNode.IsRoot);
            AddDefaultToolKitCommand = new RelayCommand(AddDefaultToolKit, () => SelectedAgent != null);
            RemoveDefaultToolKitCommand = new RelayCommand<AgentToolKitSelectionDefinition>(RemoveDefaultToolKit, entry => entry != null);
            MoveDefaultToolKitUpCommand = new RelayCommand<AgentToolKitSelectionDefinition>(MoveDefaultToolKitUp, CanMoveDefaultToolKitUp);
            MoveDefaultToolKitDownCommand = new RelayCommand<AgentToolKitSelectionDefinition>(MoveDefaultToolKitDown, CanMoveDefaultToolKitDown);
            BuildPromptPreviewCommand = new RelayCommand(BuildPromptPreview, () => SelectedAgent != null);
            CopyPromptPreviewCommand = new RelayCommand(CopyPromptPreviewSafe, () => !string.IsNullOrWhiteSpace(PromptBuildResult));

            Agents.CollectionChanged += OnAgentsCollectionChanged;
            RefreshLocalizedOptionLists();
            LocalizationRuntime.Instance.LanguageChanged += (_, _) => RefreshLocalizedText();

            LoadConfiguration();
        }

        public string Title => L("AgentConfiguration.Title", "代理配置");

        public string Description => L(
            "AgentConfiguration.Description",
            "配置代理身份、语言模型、系统提示词、结构化输入输出架构与工具权限。");

        public ObservableCollection<AgentDefinition> Agents { get; } = new();

        public IReadOnlyList<AgentToolPermissionModeOption> PermissionModes { get; private set; } = Array.Empty<AgentToolPermissionModeOption>();

        public IReadOnlyList<AgentLanguageModelSelectionModeOption> LanguageModelSelectionModes { get; private set; } = Array.Empty<AgentLanguageModelSelectionModeOption>();

        public IReadOnlyList<AgentRuntimeRoleOption> RuntimeRoleOptions { get; private set; } = Array.Empty<AgentRuntimeRoleOption>();

        public ObservableCollection<AgentConfigurationReferenceOption> AvailableLanguageModels { get; } = new();

        public ObservableCollection<AgentConfigurationReferenceOption> AvailableCapabilityLayers { get; } = new();

        public ObservableCollection<AgentConfigurationReferenceOption> AvailableToolKits { get; } = new();

        public string ConfigurationFilePath => _configurationRepository.ConfigurationFilePath;

        public string ConfigurationDirectoryPath => _pathProvider.ConfigurationDirectoryPath;

        public string AgentSummaryText => Agents.Count == 0
            ? L("AgentConfiguration.AgentSummary.Empty", "未配置代理。")
            : LF("AgentConfiguration.AgentSummary.Format", "代理总数：{0}", Agents.Count);

        public AgentDefinition? SelectedAgent
        {
            get => _selectedAgent;
            set
            {
                if (!SetProperty(ref _selectedAgent, value))
                {
                    return;
                }

                SelectedInputNode = _selectedAgent?.InputSchemaRoot;
                SelectedOutputNode = _selectedAgent?.OutputSchemaRoot;
                ResetPromptBuildPreview();
                OnPropertyChanged(nameof(HasSelectedAgent));
                OnPropertyChanged(nameof(IsStructuredEditorVisible));
                OnPropertyChanged(nameof(IsSpecificLanguageModelSelectionVisible));
                OnPropertyChanged(nameof(IsCapabilityLayerSelectionVisible));
                OnPropertyChanged(nameof(IsSubAgentIntroductionVisible));
                OnPropertyChanged(nameof(SelectedAgentLanguageModelBindingSummary));
                OnPropertyChanged(nameof(PromptBuildHintText));
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public XmlElementNodeDefinition? SelectedInputNode
        {
            get => _selectedInputNode;
            set
            {
                if (SetProperty(ref _selectedInputNode, value))
                {
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public XmlElementNodeDefinition? SelectedOutputNode
        {
            get => _selectedOutputNode;
            set
            {
                if (SetProperty(ref _selectedOutputNode, value))
                {
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public bool HasSelectedAgent => SelectedAgent != null;

        public bool IsStructuredEditorVisible => SelectedAgent?.IsStructuredXmlIO == true;

        public bool IsSpecificLanguageModelSelectionVisible => SelectedAgent?.LanguageModelSelectionMode == AgentLanguageModelSelectionMode.SpecificLanguageModel;

        public bool IsCapabilityLayerSelectionVisible => SelectedAgent?.LanguageModelSelectionMode == AgentLanguageModelSelectionMode.CapabilityLayer;

        public bool IsSubAgentIntroductionVisible => SelectedAgent?.CanRunAsSubAgent == true;

        public string SelectedAgentLanguageModelBindingSummary => BuildLanguageModelBindingSummary(SelectedAgent);

        public string PromptBuildHintText => SelectedAgent == null
            ? L("AgentConfiguration.PromptHint.NoAgent", "请选择代理后点击“构建提示词”，这里会显示最终发送给 LLM 的完整系统提示词。")
            : LF("AgentConfiguration.PromptHint.SelectedFormat", "当前目标：{0}。修改代理配置后请重新构建。", BuildPromptTargetLabel(SelectedAgent));

        public string PromptBuildResult
        {
            get => _promptBuildResult;
            private set
            {
                if (SetProperty(ref _promptBuildResult, value ?? string.Empty))
                {
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public string PromptBuildStatusMessage
        {
            get => _promptBuildStatusMessage;
            private set => SetProperty(ref _promptBuildStatusMessage, value ?? string.Empty);
        }

        public bool HasPromptBuildError
        {
            get => _hasPromptBuildError;
            private set => SetProperty(ref _hasPromptBuildError, value);
        }

        public string StatusMessage
        {
            get => _statusMessage;
            private set => SetProperty(ref _statusMessage, value ?? string.Empty);
        }

        public bool HasValidationError
        {
            get => _hasValidationError;
            private set => SetProperty(ref _hasValidationError, value);
        }

        public ICommand AddAgentCommand { get; }

        public ICommand DuplicateAgentCommand { get; }

        public ICommand RemoveAgentCommand { get; }

        public ICommand BrowseAvatarCommand { get; }

        public ICommand OpenConfigurationDirectoryCommand { get; }

        public ICommand ReloadToolsCommand { get; }

        public ICommand AllowAllToolPermissionsCommand { get; }

        public ICommand DisableAllToolPermissionsCommand { get; }

        public ICommand RequireConfirmationAllToolPermissionsCommand { get; }

        public ICommand ReloadLanguageModelCatalogCommand { get; }

        public ICommand AddInputRootNodeCommand { get; }

        public ICommand AddOutputRootNodeCommand { get; }

        public ICommand AddInputChildNodeCommand { get; }

        public ICommand AddOutputChildNodeCommand { get; }

        public ICommand RemoveInputNodeCommand { get; }

        public ICommand RemoveOutputNodeCommand { get; }

        public ICommand AddDefaultToolKitCommand { get; }

        public ICommand RemoveDefaultToolKitCommand { get; }

        public ICommand MoveDefaultToolKitUpCommand { get; }

        public ICommand MoveDefaultToolKitDownCommand { get; }

        public ICommand BuildPromptPreviewCommand { get; }

        public ICommand CopyPromptPreviewCommand { get; }

        private void LoadConfiguration()
        {
            try
            {
                using (SuspendPersistence())
                {
                    Agents.Clear();
                    RefreshRegisteredToolsInternal();
                    RefreshLanguageModelCatalogInternal();

                    var definitions = _configurationRepository.Load();
                    if (definitions.Count == 0)
                    {
                        var defaultAgent = CreateDefaultAgent();
                        SyncToolPermissions(defaultAgent);
                        Agents.Add(defaultAgent);
                    }
                    else
                    {
                        foreach (var definition in definitions)
                        {
                            SyncToolPermissions(definition);
                            Agents.Add(definition);
                        }
                    }

                    SelectedAgent = Agents.FirstOrDefault();
                }

                PersistAll(L("AgentConfiguration.Status.Loaded", "代理配置已加载。"));
            }
            catch (Exception ex)
            {
                using (SuspendPersistence())
                {
                    Agents.Clear();
                    RefreshRegisteredToolsInternal();

                    var fallbackAgent = CreateDefaultAgent();
                    SyncToolPermissions(fallbackAgent);
                    Agents.Add(fallbackAgent);
                    SelectedAgent = fallbackAgent;
                }

                HasValidationError = true;
                StatusMessage = LF("AgentConfiguration.Status.LoadFailedFormat", "加载代理配置失败：{0}", ex.Message);
            }
        }

        private void AddAgent()
        {
            using (SuspendPersistence())
            {
                var agent = CreateDefaultAgent();
                SyncToolPermissions(agent);
                Agents.Add(agent);
                SelectedAgent = agent;
            }

            PersistAll(L("AgentConfiguration.Status.Added", "已新增代理。"));
        }

        private void DuplicateSelectedAgent()
        {
            if (SelectedAgent == null)
            {
                return;
            }

            using (SuspendPersistence())
            {
                var clone = SelectedAgent.DeepClone();
                clone.AgentId = GenerateUniqueAgentId(SelectedAgent.AgentId);
                clone.DisplayName = BuildDuplicatedDisplayName(SelectedAgent.DisplayName);
                SyncToolPermissions(clone);
                Agents.Add(clone);
                SelectedAgent = clone;
            }

            PersistAll(L("AgentConfiguration.Status.Duplicated", "已复制代理。"));
        }

        private void RemoveSelectedAgent()
        {
            if (SelectedAgent == null)
            {
                return;
            }

            using (SuspendPersistence())
            {
                var selectedIndex = Agents.IndexOf(SelectedAgent);
                if (selectedIndex < 0)
                {
                    return;
                }

                Agents.RemoveAt(selectedIndex);

                if (Agents.Count == 0)
                {
                    var fallbackAgent = CreateDefaultAgent();
                    SyncToolPermissions(fallbackAgent);
                    Agents.Add(fallbackAgent);
                    SelectedAgent = fallbackAgent;
                }
                else
                {
                    var nextIndex = Math.Min(selectedIndex, Agents.Count - 1);
                    SelectedAgent = Agents[nextIndex];
                }
            }

            PersistAll(L("AgentConfiguration.Status.Removed", "已删除代理。"));
        }

        private void BrowseAvatar()
        {
            if (SelectedAgent == null)
            {
                return;
            }

            var dialog = new OpenFileDialog
            {
                Filter = L("AgentConfiguration.Dialog.ImageFilter", "图像文件|*.png;*.jpg;*.jpeg;*.bmp;*.ico|所有文件|*.*"),
                Multiselect = false,
                CheckFileExists = true,
                Title = L("AgentConfiguration.Dialog.SelectAvatarTitle", "选择头像图片")
            };

            if (dialog.ShowDialog() != true)
            {
                return;
            }

            SelectedAgent.AvatarPath = dialog.FileName;
            StatusMessage = L("AgentConfiguration.Status.AvatarUpdated", "头像已更新。");
        }

        private void OpenConfigurationDirectory()
        {
            Directory.CreateDirectory(ConfigurationDirectoryPath);
            Process.Start(new ProcessStartInfo
            {
                FileName = ConfigurationDirectoryPath,
                UseShellExecute = true
            });
        }

        private void ReloadTools()
        {
            using (SuspendPersistence())
            {
                RefreshRegisteredToolsInternal();
                foreach (var agent in Agents)
                {
                    SyncToolPermissions(agent);
                }
            }

            PersistAll(L("AgentConfiguration.Status.ToolsReloaded", "已根据当前工具注册表刷新工具权限。"));
        }

        private void SetAllToolPermissions(AgentToolPermissionMode permissionMode, string successMessage)
        {
            if (SelectedAgent == null)
            {
                return;
            }

            using (SuspendPersistence())
            {
                foreach (var permission in SelectedAgent.ToolPermissions)
                {
                    permission.Permission = permissionMode;
                }
            }

            PersistAll(successMessage);
        }

        private void ReloadLanguageModelCatalog()
        {
            RefreshLanguageModelCatalogInternal();
            StatusMessage = string.IsNullOrWhiteSpace(_languageModelCatalogErrorMessage)
                ? L("AgentConfiguration.Status.LanguageModelCatalogRefreshed", "语言模型目录已刷新。")
                : LF("AgentConfiguration.Status.LanguageModelCatalogRefreshFailedFormat", "语言模型目录刷新失败：{0}", _languageModelCatalogErrorMessage);
        }

        private void BuildPromptPreview()
        {
            if (SelectedAgent == null)
            {
                return;
            }

            try
            {
                PromptBuildResult = _agentSystemPromptBuilder.BuildCompleteSystemPrompt(SelectedAgent);
                HasPromptBuildError = false;
                PromptBuildStatusMessage = LF("AgentConfiguration.Status.PromptBuiltFormat", "已为代理“{0}”生成完整系统提示词。", BuildPromptTargetLabel(SelectedAgent));
            }
            catch (Exception ex)
            {
                PromptBuildResult = string.Empty;
                HasPromptBuildError = true;
                PromptBuildStatusMessage = LF("AgentConfiguration.Status.PromptBuildFailedFormat", "构建提示词失败：{0}", ex.Message);
            }
        }

        private void CopyPromptPreview()
        {
            CopyPromptPreviewSafe();
        }

        private void CopyPromptPreviewSafe()
        {
            if (string.IsNullOrWhiteSpace(PromptBuildResult))
            {
                return;
            }

            if (ClipboardAccessService.TrySetText(PromptBuildResult, out var errorMessage))
            {
                HasPromptBuildError = false;
                PromptBuildStatusMessage = L("AgentConfiguration.Status.PromptCopied", "提示词已复制到剪贴板。");
                return;
            }

            HasPromptBuildError = true;
            PromptBuildStatusMessage = LF("AgentConfiguration.Status.PromptCopyFailedFormat", "复制提示词失败：{0}", errorMessage ?? string.Empty);
        }

        private void AddInputRootNode()
        {
            if (SelectedAgent == null)
            {
                return;
            }

            AddNode(SelectedAgent.InputSchemaRoot, isInputTree: true, L("AgentConfiguration.Status.InputNodeAdded", "已添加输入架构节点。"));
        }

        private void AddOutputRootNode()
        {
            if (SelectedAgent == null)
            {
                return;
            }

            AddNode(SelectedAgent.OutputSchemaRoot, isInputTree: false, L("AgentConfiguration.Status.OutputNodeAdded", "已添加输出架构节点。"));
        }

        private void AddInputChildNode()
        {
            if (SelectedInputNode == null)
            {
                return;
            }

            AddNode(SelectedInputNode, isInputTree: true, L("AgentConfiguration.Status.InputChildNodeAdded", "已添加输入架构子节点。"));
        }

        private void AddOutputChildNode()
        {
            if (SelectedOutputNode == null)
            {
                return;
            }

            AddNode(SelectedOutputNode, isInputTree: false, L("AgentConfiguration.Status.OutputChildNodeAdded", "已添加输出架构子节点。"));
        }

        private void RemoveSelectedInputNode()
        {
            RemoveNode(SelectedInputNode, isInputTree: true, L("AgentConfiguration.Status.InputNodeRemoved", "已移除输入架构节点。"));
        }

        private void RemoveSelectedOutputNode()
        {
            RemoveNode(SelectedOutputNode, isInputTree: false, L("AgentConfiguration.Status.OutputNodeRemoved", "已移除输出架构节点。"));
        }

        private void AddNode(XmlElementNodeDefinition parentNode, bool isInputTree, string successMessage)
        {
            if (SelectedAgent == null)
            {
                return;
            }

            using (SuspendPersistence())
            {
                var childNode = parentNode.AddChild(CreateUniqueNodeName(parentNode, "Node"));
                AttachNodeSubtree(childNode);

                if (isInputTree)
                {
                    SelectedInputNode = childNode;
                }
                else
                {
                    SelectedOutputNode = childNode;
                }
            }

            PersistAll(successMessage);
        }

        private void RemoveNode(XmlElementNodeDefinition? node, bool isInputTree, string successMessage)
        {
            if (node == null || node.IsRoot || node.Parent == null)
            {
                return;
            }

            var parent = node.Parent;
            using (SuspendPersistence())
            {
                DetachNodeSubtree(node);
                parent.RemoveChild(node);

                if (isInputTree)
                {
                    SelectedInputNode = parent;
                }
                else
                {
                    SelectedOutputNode = parent;
                }
            }

            PersistAll(successMessage);
        }

        private void AddDefaultToolKit()
        {
            if (SelectedAgent == null || AvailableToolKits.Count == 0)
            {
                return;
            }

            var firstAvailableKey = AvailableToolKits[0].Key;
            SelectedAgent.DefaultToolKits.Add(new AgentToolKitSelectionDefinition
            {
                ToolKitKey = firstAvailableKey
            });
        }

        private void RemoveDefaultToolKit(AgentToolKitSelectionDefinition? toolKit)
        {
            if (SelectedAgent == null || toolKit == null)
            {
                return;
            }

            SelectedAgent.DefaultToolKits.Remove(toolKit);
        }

        private bool CanMoveDefaultToolKitUp(AgentToolKitSelectionDefinition? toolKit)
        {
            return SelectedAgent != null &&
                   toolKit != null &&
                   SelectedAgent.DefaultToolKits.IndexOf(toolKit) > 0;
        }

        private void MoveDefaultToolKitUp(AgentToolKitSelectionDefinition? toolKit)
        {
            if (!CanMoveDefaultToolKitUp(toolKit))
            {
                return;
            }

            var index = SelectedAgent!.DefaultToolKits.IndexOf(toolKit!);
            SelectedAgent.DefaultToolKits.Move(index, index - 1);
        }

        private bool CanMoveDefaultToolKitDown(AgentToolKitSelectionDefinition? toolKit)
        {
            return SelectedAgent != null &&
                   toolKit != null &&
                   SelectedAgent.DefaultToolKits.IndexOf(toolKit) >= 0 &&
                   SelectedAgent.DefaultToolKits.IndexOf(toolKit) < SelectedAgent.DefaultToolKits.Count - 1;
        }

        private void MoveDefaultToolKitDown(AgentToolKitSelectionDefinition? toolKit)
        {
            if (!CanMoveDefaultToolKitDown(toolKit))
            {
                return;
            }

            var index = SelectedAgent!.DefaultToolKits.IndexOf(toolKit!);
            SelectedAgent.DefaultToolKits.Move(index, index + 1);
        }

        private void OnAgentsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (var agent in e.NewItems.OfType<AgentDefinition>())
                {
                    AttachAgent(agent);
                }
            }

            if (e.OldItems != null)
            {
                foreach (var agent in e.OldItems.OfType<AgentDefinition>())
                {
                    DetachAgent(agent);
                }
            }

            OnPropertyChanged(nameof(AgentSummaryText));
            CommandManager.InvalidateRequerySuggested();
        }

        private void AttachAgent(AgentDefinition agent)
        {
            agent.PropertyChanged -= OnAgentPropertyChanged;
            agent.PropertyChanged += OnAgentPropertyChanged;

            agent.ToolPermissions.CollectionChanged -= OnToolPermissionsCollectionChanged;
            agent.ToolPermissions.CollectionChanged += OnToolPermissionsCollectionChanged;
            foreach (var permission in agent.ToolPermissions)
            {
                AttachToolPermission(permission);
            }

            agent.DefaultToolKits.CollectionChanged -= OnDefaultToolKitsCollectionChanged;
            agent.DefaultToolKits.CollectionChanged += OnDefaultToolKitsCollectionChanged;
            foreach (var toolKit in agent.DefaultToolKits)
            {
                AttachDefaultToolKit(toolKit);
            }

            AttachNodeSubtree(agent.InputSchemaRoot);
            AttachNodeSubtree(agent.OutputSchemaRoot);
        }

        private void DetachAgent(AgentDefinition agent)
        {
            agent.PropertyChanged -= OnAgentPropertyChanged;

            agent.ToolPermissions.CollectionChanged -= OnToolPermissionsCollectionChanged;
            foreach (var permission in agent.ToolPermissions)
            {
                DetachToolPermission(permission);
            }

            agent.DefaultToolKits.CollectionChanged -= OnDefaultToolKitsCollectionChanged;
            foreach (var toolKit in agent.DefaultToolKits)
            {
                DetachDefaultToolKit(toolKit);
            }

            DetachNodeSubtree(agent.InputSchemaRoot);
            DetachNodeSubtree(agent.OutputSchemaRoot);
        }

        private void AttachToolPermission(AgentToolPermissionDefinition permission)
        {
            permission.PropertyChanged -= OnToolPermissionPropertyChanged;
            permission.PropertyChanged += OnToolPermissionPropertyChanged;
        }

        private void DetachToolPermission(AgentToolPermissionDefinition permission)
        {
            permission.PropertyChanged -= OnToolPermissionPropertyChanged;
        }

        private void AttachDefaultToolKit(AgentToolKitSelectionDefinition toolKit)
        {
            toolKit.PropertyChanged -= OnDefaultToolKitPropertyChanged;
            toolKit.PropertyChanged += OnDefaultToolKitPropertyChanged;
        }

        private void DetachDefaultToolKit(AgentToolKitSelectionDefinition toolKit)
        {
            toolKit.PropertyChanged -= OnDefaultToolKitPropertyChanged;
        }

        private void AttachNodeSubtree(XmlElementNodeDefinition node)
        {
            node.PropertyChanged -= OnXmlNodePropertyChanged;
            node.PropertyChanged += OnXmlNodePropertyChanged;

            foreach (var child in node.Children)
            {
                AttachNodeSubtree(child);
            }
        }

        private void DetachNodeSubtree(XmlElementNodeDefinition node)
        {
            node.PropertyChanged -= OnXmlNodePropertyChanged;

            foreach (var child in node.Children)
            {
                DetachNodeSubtree(child);
            }
        }

        private void OnAgentPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (sender == SelectedAgent &&
                string.Equals(e.PropertyName, nameof(AgentDefinition.IsStructuredXmlIO), StringComparison.Ordinal))
            {
                OnPropertyChanged(nameof(IsStructuredEditorVisible));
            }

            if (sender == SelectedAgent &&
                (string.Equals(e.PropertyName, nameof(AgentDefinition.LanguageModelSelectionMode), StringComparison.Ordinal) ||
                 string.Equals(e.PropertyName, nameof(AgentDefinition.SelectedLanguageModelKey), StringComparison.Ordinal) ||
                 string.Equals(e.PropertyName, nameof(AgentDefinition.SelectedCapabilityLayerKey), StringComparison.Ordinal)))
            {
                OnPropertyChanged(nameof(IsSpecificLanguageModelSelectionVisible));
                OnPropertyChanged(nameof(IsCapabilityLayerSelectionVisible));
                OnPropertyChanged(nameof(SelectedAgentLanguageModelBindingSummary));
            }

            if (sender == SelectedAgent &&
                (string.Equals(e.PropertyName, nameof(AgentDefinition.RuntimeRole), StringComparison.Ordinal) ||
                 string.Equals(e.PropertyName, nameof(AgentDefinition.CanRunAsSubAgent), StringComparison.Ordinal)))
            {
                OnPropertyChanged(nameof(IsSubAgentIntroductionVisible));
            }

            if (string.Equals(e.PropertyName, nameof(AgentDefinition.StructuredModeText), StringComparison.Ordinal) ||
                string.Equals(e.PropertyName, nameof(AgentDefinition.RuntimeRoleText), StringComparison.Ordinal) ||
                string.Equals(e.PropertyName, nameof(AgentDefinition.CanRunAsMainAgent), StringComparison.Ordinal) ||
                string.Equals(e.PropertyName, nameof(AgentDefinition.CanRunAsSubAgent), StringComparison.Ordinal) ||
                string.Equals(e.PropertyName, nameof(AgentDefinition.DisplayNameOrFallback), StringComparison.Ordinal) ||
                string.Equals(e.PropertyName, nameof(AgentDefinition.AgentIdOrFallback), StringComparison.Ordinal) ||
                string.Equals(e.PropertyName, nameof(AgentDefinition.AvatarPreviewPath), StringComparison.Ordinal))
            {
                return;
            }

            PersistAll(L("AgentConfiguration.Status.Saved", "代理配置已保存。"));
        }

        private void OnToolPermissionsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (var permission in e.NewItems.OfType<AgentToolPermissionDefinition>())
                {
                    AttachToolPermission(permission);
                }
            }

            if (e.OldItems != null)
            {
                foreach (var permission in e.OldItems.OfType<AgentToolPermissionDefinition>())
                {
                    DetachToolPermission(permission);
                }
            }

            PersistAll(L("AgentConfiguration.Status.ToolPermissionListUpdated", "工具权限列表已更新。"));
        }

        private void OnToolPermissionPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (sender is not AgentToolPermissionDefinition)
            {
                return;
            }

            if (string.Equals(e.PropertyName, nameof(AgentToolPermissionDefinition.GlobalAvailabilityText), StringComparison.Ordinal))
            {
                return;
            }

            PersistAll(L("AgentConfiguration.Status.ToolPermissionUpdated", "工具权限已更新。"));
        }

        private void OnDefaultToolKitPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (sender is not AgentToolKitSelectionDefinition)
            {
                return;
            }

            PersistAll(L("AgentConfiguration.Status.DefaultToolKitUpdated", "默认工具集已更新。"));
        }

        private void OnDefaultToolKitsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (var toolKit in e.NewItems.OfType<AgentToolKitSelectionDefinition>())
                {
                    AttachDefaultToolKit(toolKit);
                }
            }

            if (e.OldItems != null)
            {
                foreach (var toolKit in e.OldItems.OfType<AgentToolKitSelectionDefinition>())
                {
                    DetachDefaultToolKit(toolKit);
                }
            }

            PersistAll(L("AgentConfiguration.Status.DefaultToolKitListUpdated", "默认工具集列表已更新。"));
        }

        private void OnXmlNodePropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (!string.Equals(e.PropertyName, nameof(XmlElementNodeDefinition.Name), StringComparison.Ordinal))
            {
                return;
            }

            PersistAll(L("AgentConfiguration.Status.XmlSchemaUpdated", "XML 架构已更新。"));
        }

        private void PersistAll(string successMessage)
        {
            if (_suspendPersistenceCounter > 0)
            {
                return;
            }

            if (!ValidateAllAgents(out var validationError))
            {
                HasValidationError = true;
                StatusMessage = validationError;
                return;
            }

            try
            {
                _configurationRepository.Save(Agents);
                HasValidationError = false;
                StatusMessage = successMessage;
            }
            catch (Exception ex)
            {
                HasValidationError = true;
                StatusMessage = LF("AgentConfiguration.Status.SaveFailedFormat", "保存代理配置失败：{0}", ex.Message);
            }
        }

        private bool ValidateAllAgents(out string errorMessage)
        {
            var knownAgentIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var agent in Agents)
            {
                if (string.IsNullOrWhiteSpace(agent.AgentId))
                {
                    errorMessage = LF("AgentConfiguration.Validation.MissingAgentIdFormat", "代理“{0}”缺少代理 ID。", agent.DisplayNameOrFallback);
                    return false;
                }

                if (!s_agentIdPattern.IsMatch(agent.AgentId))
                {
                    errorMessage = LF("AgentConfiguration.Validation.InvalidAgentIdFormat", "代理 ID “{0}”无效。仅允许字母和数字。", agent.AgentId);
                    return false;
                }

                if (!knownAgentIds.Add(agent.AgentId))
                {
                    errorMessage = LF("AgentConfiguration.Validation.DuplicateAgentIdFormat", "代理 ID “{0}”重复。", agent.AgentId);
                    return false;
                }

                if (!ValidateSchemaNodeNames(
                        agent,
                        agent.InputSchemaRoot,
                        L("AgentConfiguration.Validation.InputSchema", "输入架构"),
                        out errorMessage))
                {
                    return false;
                }

                if (!ValidateSchemaNodeNames(
                        agent,
                        agent.OutputSchemaRoot,
                        L("AgentConfiguration.Validation.OutputSchema", "输出架构"),
                        out errorMessage))
                {
                    return false;
                }
            }

            errorMessage = string.Empty;
            return true;
        }

        private static bool ValidateSchemaNodeNames(
            AgentDefinition agent,
            XmlElementNodeDefinition rootNode,
            string schemaName,
            out string errorMessage)
        {
            foreach (var child in rootNode.Children)
            {
                if (!ValidateNodeAndChildren(agent, child, schemaName, out errorMessage))
                {
                    return false;
                }
            }

            errorMessage = string.Empty;
            return true;
        }

        private static bool ValidateNodeAndChildren(
            AgentDefinition agent,
            XmlElementNodeDefinition node,
            string schemaName,
            out string errorMessage)
        {
            if (string.IsNullOrWhiteSpace(node.Name))
            {
                errorMessage = LF("AgentConfiguration.Validation.EmptyNodeNameFormat", "代理“{0}”在{1}中存在空节点名称。", agent.AgentIdOrFallback, schemaName);
                return false;
            }

            try
            {
                XmlConvert.VerifyName(node.Name);
            }
            catch (XmlException)
            {
                errorMessage = LF("AgentConfiguration.Validation.InvalidNodeNameFormat", "{0}中的节点名称“{1}”不是有效的 XML 元素名。", schemaName, node.Name);
                return false;
            }

            foreach (var child in node.Children)
            {
                if (!ValidateNodeAndChildren(agent, child, schemaName, out errorMessage))
                {
                    return false;
                }
            }

            errorMessage = string.Empty;
            return true;
        }

        private AgentDefinition CreateDefaultAgent()
        {
            return new AgentDefinition
            {
                DisplayName = LF("AgentConfiguration.NewAgentNameFormat", "代理 {0}", Agents.Count + 1),
                AgentId = GenerateUniqueAgentId("Agent1"),
                AvatarPath = AgentDefinition.DefaultAvatarPath,
                SystemPrompt = string.Empty,
                IsStructuredXmlIO = false,
                InputDescription = L("AgentConfiguration.DefaultInputDescription", "用户发送的聊天消息"),
                OutputDescription = L("AgentConfiguration.DefaultOutputDescription", "无，无需调用Passdown工具"),
                RuntimeRole = AgentRuntimeRole.MainOnly
            };
        }

        private string GenerateUniqueAgentId(string seed)
        {
            var normalizedSeed = NormalizeAgentIdSeed(seed);
            if (!ContainsAgentId(normalizedSeed))
            {
                return normalizedSeed;
            }

            var suffix = 2;
            while (ContainsAgentId($"{normalizedSeed}{suffix}"))
            {
                suffix++;
            }

            return $"{normalizedSeed}{suffix}";
        }

        private bool ContainsAgentId(string candidate)
        {
            return Agents.Any(agent => string.Equals(agent.AgentId, candidate, StringComparison.OrdinalIgnoreCase));
        }

        private static string NormalizeAgentIdSeed(string? seed)
        {
            var source = string.IsNullOrWhiteSpace(seed) ? "Agent" : seed.Trim();
            var filteredChars = source.Where(char.IsLetterOrDigit).ToArray();
            var normalized = new string(filteredChars);
            return normalized.Length == 0 ? "Agent" : normalized;
        }

        private static string BuildDuplicatedDisplayName(string? displayName)
        {
            return string.IsNullOrWhiteSpace(displayName)
                ? L("AgentConfiguration.AgentCopyName", "代理副本")
                : LF("AgentConfiguration.AgentCopyNameFormat", "{0} 副本", displayName.Trim());
        }

        private void ResetPromptBuildPreview()
        {
            PromptBuildResult = string.Empty;
            HasPromptBuildError = false;
            PromptBuildStatusMessage = SelectedAgent == null
                ? L("AgentConfiguration.Status.InitialPrompt", "请选择代理后点击“构建提示词”。")
                : L("AgentConfiguration.Status.CurrentResultNotGenerated", "当前结果尚未生成。点击“构建提示词”即可查看完整系统提示词。");
        }

        private void RefreshRegisteredToolsInternal()
        {
            _toolRegistrationMap.Clear();
            foreach (var registration in _toolManager.GetRegisteredTools(resolveIcons: false).Where(item => item.RequiresAgentPermission))
            {
                _toolRegistrationMap[registration.Definition.Name] = new ToolRegistrationSnapshot(
                    registration.Definition.Name,
                    string.IsNullOrWhiteSpace(registration.Definition.Description)
                        ? L("AgentConfiguration.Tool.NoDescription", "暂无工具说明。")
                        : registration.Definition.Description,
                    registration.IsEnabled);
            }
        }

        private void RefreshLanguageModelCatalogInternal()
        {
            AvailableLanguageModels.Clear();
            AvailableCapabilityLayers.Clear();
            _languageModelMap.Clear();
            _capabilityLayerMap.Clear();

            var errorMessages = new List<string>();

            try
            {
                foreach (var model in _languageModelRepository.Load()
                             .Where(item => !string.IsNullOrWhiteSpace(item.Key))
                             .OrderBy(item => GetLanguageModelDisplayName(item), StringComparer.OrdinalIgnoreCase))
                {
                    _languageModelMap[model.Key] = model;
                }
            }
            catch (Exception ex)
            {
                errorMessages.Add(LF("AgentConfiguration.Catalog.LanguageModelErrorFormat", "语言模型：{0}", ex.Message));
            }

            try
            {
                foreach (var layer in _capabilityLayerRepository.Load()
                             .Where(item => !string.IsNullOrWhiteSpace(item.Key))
                             .OrderBy(item => GetCapabilityLayerDisplayName(item), StringComparer.OrdinalIgnoreCase))
                {
                    _capabilityLayerMap[layer.Key] = layer;
                }
            }
            catch (Exception ex)
            {
                errorMessages.Add(LF("AgentConfiguration.Catalog.CapabilityLayerErrorFormat", "功能层级：{0}", ex.Message));
            }

            foreach (var model in _languageModelMap.Values.OrderBy(GetLanguageModelDisplayName, StringComparer.OrdinalIgnoreCase))
            {
                AvailableLanguageModels.Add(new AgentConfigurationReferenceOption(model.Key, GetLanguageModelDisplayName(model)));
            }

            foreach (var layer in _capabilityLayerMap.Values.OrderBy(GetCapabilityLayerDisplayName, StringComparer.OrdinalIgnoreCase))
            {
                if (!layer.IsUserSelectable)
                {
                    continue;
                }

                AvailableCapabilityLayers.Add(new AgentConfigurationReferenceOption(layer.Key, GetCapabilityLayerOptionDisplayName(layer)));
            }

            AvailableToolKits.Clear();
            foreach (var toolKit in _toolKitService.Load()
                         .Where(item => !string.IsNullOrWhiteSpace(item.Key))
                         .OrderBy(item => item.DisplayNameOrFallback, StringComparer.OrdinalIgnoreCase))
            {
                AvailableToolKits.Add(new AgentConfigurationReferenceOption(toolKit.Key, toolKit.DisplayNameOrFallback));
            }

            _languageModelCatalogErrorMessage = string.Join("；", errorMessages.Where(message => !string.IsNullOrWhiteSpace(message)));
            OnPropertyChanged(nameof(SelectedAgentLanguageModelBindingSummary));
        }

        private void SyncToolPermissions(AgentDefinition agent)
        {
            var existingPermissions = agent.ToolPermissions
                .GroupBy(item => item.ToolName, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(group => group.Key, group => group.First(), StringComparer.OrdinalIgnoreCase);

            var mergedPermissions = new List<AgentToolPermissionDefinition>();
            foreach (var toolRegistration in _toolRegistrationMap.Values.OrderBy(item => item.Name, StringComparer.OrdinalIgnoreCase))
            {
                if (!existingPermissions.TryGetValue(toolRegistration.Name, out var permission))
                {
                    permission = new AgentToolPermissionDefinition
                    {
                        ToolName = toolRegistration.Name,
                        Permission = ResolveDefaultPermission(toolRegistration.Name)
                    };
                }
                else
                {
                    existingPermissions.Remove(toolRegistration.Name);
                }

                permission.ToolName = toolRegistration.Name;
                permission.ToolDescription = toolRegistration.Description;
                permission.IsMissing = false;
                permission.IsGloballyEnabled = toolRegistration.IsEnabled;
                mergedPermissions.Add(permission);
            }

            foreach (var missingPermission in existingPermissions.Values.OrderBy(item => item.ToolName, StringComparer.OrdinalIgnoreCase))
            {
                if (_toolManager.GetRegisteredTools(resolveIcons: false).Any(registration =>
                        registration.Definition.IsSystemTool &&
                        string.Equals(registration.Definition.Name, missingPermission.ToolName, StringComparison.OrdinalIgnoreCase)))
                {
                    continue;
                }

                missingPermission.IsMissing = true;
                missingPermission.IsGloballyEnabled = false;

                if (string.IsNullOrWhiteSpace(missingPermission.ToolDescription))
                {
                    missingPermission.ToolDescription = L("AgentConfiguration.Tool.MissingDescription", "当前工具发现结果中缺失。");
                }

                mergedPermissions.Add(missingPermission);
            }

            agent.ToolPermissions.Clear();
            foreach (var permission in mergedPermissions)
            {
                agent.ToolPermissions.Add(permission);
            }
        }

        private static AgentToolPermissionMode MapDefaultPermission(SkyweaverToolDefaultAgentPermission defaultPermission)
        {
            return defaultPermission switch
            {
                SkyweaverToolDefaultAgentPermission.Disabled => AgentToolPermissionMode.Disabled,
                SkyweaverToolDefaultAgentPermission.Allow => AgentToolPermissionMode.Allow,
                _ => AgentToolPermissionMode.RequireConfirmation
            };
        }

        private AgentToolPermissionMode ResolveDefaultPermission(string toolName)
        {
            var registration = _toolManager.GetRegisteredTools(resolveIcons: false).FirstOrDefault(item =>
                string.Equals(item.Definition.Name, toolName, StringComparison.OrdinalIgnoreCase));

            return registration == null
                ? AgentToolPermissionMode.RequireConfirmation
                : MapDefaultPermission(registration.Definition.DefaultAgentPermission);
        }

        private static string CreateUniqueNodeName(XmlElementNodeDefinition parentNode, string baseName)
        {
            if (parentNode.Children.All(child => !string.Equals(child.Name, baseName, StringComparison.OrdinalIgnoreCase)))
            {
                return baseName;
            }

            var suffix = 2;
            while (parentNode.Children.Any(child =>
                       string.Equals(child.Name, $"{baseName}{suffix}", StringComparison.OrdinalIgnoreCase)))
            {
                suffix++;
            }

            return $"{baseName}{suffix}";
        }

        private string BuildLanguageModelBindingSummary(AgentDefinition? agent)
        {
            if (!string.IsNullOrWhiteSpace(_languageModelCatalogErrorMessage))
            {
                return LF("AgentConfiguration.Binding.CatalogLoadFailedFormat", "语言模型目录读取失败：{0}", _languageModelCatalogErrorMessage);
            }

            if (agent == null)
            {
                return L("AgentConfiguration.Binding.NoAgent", "请选择代理后配置语言模型。");
            }

            return agent.LanguageModelSelectionMode == AgentLanguageModelSelectionMode.CapabilityLayer
                ? BuildCapabilityLayerBindingSummary(agent)
                : BuildSpecificLanguageModelBindingSummary(agent);
        }

        private string BuildSpecificLanguageModelBindingSummary(AgentDefinition agent)
        {
            if (_languageModelMap.Count == 0)
            {
                return L("AgentConfiguration.Binding.NoLanguageModels", "当前尚未配置任何语言模型。请先在“语言模型配置”页面新增模型。");
            }

            if (string.IsNullOrWhiteSpace(agent.SelectedLanguageModelKey))
            {
                return L("AgentConfiguration.Binding.SpecificNotSelected", "当前未选择具体语言模型。");
            }

            if (!_languageModelMap.TryGetValue(agent.SelectedLanguageModelKey, out var model))
            {
                return L("AgentConfiguration.Binding.SpecificMissing", "已选择的语言模型不存在，请重新选择。");
            }

            var displayName = GetLanguageModelDisplayName(model);
            return model.InterfaceSettings.IsFullyConfigured
                ? LF("AgentConfiguration.Binding.SpecificFixedFormat", "将固定使用语言模型“{0}”，不会自动回退到其他模型。", displayName)
                : LF("AgentConfiguration.Binding.SpecificIncompleteFormat", "已选择语言模型“{0}”，但其接口配置尚不完整。", displayName);
        }

        private string BuildCapabilityLayerBindingSummary(AgentDefinition agent)
        {
            if (_capabilityLayerMap.Count == 0)
            {
                return L("AgentConfiguration.Binding.NoCapabilityLayers", "当前尚未配置任何功能层级。请先在“语言模型配置”页面创建功能层级。");
            }

            if (string.IsNullOrWhiteSpace(agent.SelectedCapabilityLayerKey))
            {
                return L("AgentConfiguration.Binding.CapabilityNotSelected", "当前未选择功能层级。");
            }

            if (!_capabilityLayerMap.TryGetValue(agent.SelectedCapabilityLayerKey, out var layer))
            {
                return L("AgentConfiguration.Binding.CapabilityMissing", "已选择的功能层级不存在，请重新选择。");
            }

            if (!layer.IsUserSelectable)
            {
                return LF(
                    "AgentConfiguration.Binding.CapabilityBuiltInFormat",
                    "功能层级“{0}”为系统内置层级，不能直接绑定给代理。",
                    GetCapabilityLayerDisplayName(layer));
            }

            if (layer.LanguageModels.Count == 0)
            {
                return LF(
                    "AgentConfiguration.Binding.CapabilityNoCandidatesFormat",
                    "功能层级“{0}”尚未配置任何候选语言模型。",
                    GetCapabilityLayerDisplayName(layer));
            }

            var candidateNames = new List<string>();
            var unavailableCount = 0;

            foreach (var entry in layer.LanguageModels)
            {
                if (string.IsNullOrWhiteSpace(entry.LanguageModelKey) ||
                    !_languageModelMap.TryGetValue(entry.LanguageModelKey, out var model) ||
                    !model.InterfaceSettings.IsFullyConfigured)
                {
                    unavailableCount++;
                    continue;
                }

                candidateNames.Add(GetLanguageModelDisplayName(model));
            }

            if (candidateNames.Count == 0)
            {
                return LF(
                    "AgentConfiguration.Binding.CapabilityNoUsableCandidatesFormat",
                    "功能层级“{0}”下暂无可直接调用的语言模型。请检查候选模型是否存在且接口配置完整。",
                    GetCapabilityLayerDisplayName(layer));
            }

            var unavailableText = unavailableCount > 0
                ? LF("AgentConfiguration.Binding.UnavailableCandidatesFormat", " 当前另有 {0} 个候选不可用或引用缺失。", unavailableCount)
                : string.Empty;

            return LF(
                "AgentConfiguration.Binding.CapabilityOrderFormat",
                "将按功能层级“{0}”顺序尝试：{1}。遇到首个无错误可用模型后停止。{2}",
                GetCapabilityLayerDisplayName(layer),
                FormatCandidatePreview(candidateNames),
                unavailableText);
        }

        private static string FormatCandidatePreview(IReadOnlyList<string> candidateNames)
        {
            if (candidateNames.Count <= 4)
            {
                return string.Join(" -> ", candidateNames);
            }

            return LF(
                "AgentConfiguration.Binding.CandidateOverflowFormat",
                "{0} -> 等 {1} 个模型",
                string.Join(" -> ", candidateNames.Take(4)),
                candidateNames.Count);
        }

        private static string GetLanguageModelDisplayName(LanguageModelDefinition model)
        {
            var displayName = model.DisplayName?.Trim() ?? string.Empty;
            if (displayName.Length > 0)
            {
                return displayName;
            }

            var summaryModelId = model.SummaryModelId?.Trim() ?? string.Empty;
            if (summaryModelId.Length > 0)
            {
                return LF("LanguageModelConfiguration.UnnamedModelFormat", "未命名模型 ({0})", summaryModelId);
            }

            return LF("LanguageModelConfiguration.UnnamedModelFormat", "未命名模型 ({0})", GetShortKey(model.Key));
        }

        private static string GetCapabilityLayerDisplayName(CapabilityLayerDefinition layer)
        {
            var displayName = layer.Name?.Trim() ?? string.Empty;
            return displayName.Length > 0
                ? displayName
                : LF("LanguageModelConfiguration.UnnamedLayerFormat", "未命名功能层级 ({0})", GetShortKey(layer.Key));
        }

        private static string GetCapabilityLayerOptionDisplayName(CapabilityLayerDefinition layer)
        {
            var candidateCount = layer.LanguageModels.Count(entry => !string.IsNullOrWhiteSpace(entry.LanguageModelKey));
            return LF("AgentConfiguration.CapabilityLayerOptionFormat", "{0}（{1} 个候选）", GetCapabilityLayerDisplayName(layer), candidateCount);
        }

        private static string GetShortKey(string? key)
        {
            var normalizedKey = (key ?? string.Empty).Trim();
            if (normalizedKey.Length <= 8)
            {
                return normalizedKey;
            }

            return normalizedKey[..8];
        }

        private static string BuildPromptTargetLabel(AgentDefinition agent)
        {
            var displayName = string.IsNullOrWhiteSpace(agent.DisplayName)
                ? L("AgentConfiguration.UnnamedAgent", "未命名代理")
                : agent.DisplayName.Trim();
            var agentId = string.IsNullOrWhiteSpace(agent.AgentId)
                ? L("AgentConfiguration.UnsetAgentId", "未设置 ID")
                : agent.AgentId.Trim();

            return $"{displayName} ({agentId})";
        }

        private void RefreshLocalizedOptionLists()
        {
            PermissionModes = new[]
            {
                new AgentToolPermissionModeOption(
                    AgentToolPermissionMode.Disabled,
                    L("AgentConfiguration.Permission.Disabled", "禁用")),
                new AgentToolPermissionModeOption(
                    AgentToolPermissionMode.RequireConfirmation,
                    L("AgentConfiguration.Permission.RequireConfirmation", "需确认")),
                new AgentToolPermissionModeOption(
                    AgentToolPermissionMode.Allow,
                    L("AgentConfiguration.Permission.Allow", "允许"))
            };

            LanguageModelSelectionModes = new[]
            {
                new AgentLanguageModelSelectionModeOption(
                    AgentLanguageModelSelectionMode.SpecificLanguageModel,
                    L("AgentConfiguration.SelectionMode.SpecificLanguageModel", "具体语言模型")),
                new AgentLanguageModelSelectionModeOption(
                    AgentLanguageModelSelectionMode.CapabilityLayer,
                    L("AgentConfiguration.SelectionMode.CapabilityLayer", "功能层级"))
            };

            RuntimeRoleOptions = new[]
            {
                new AgentRuntimeRoleOption(
                    AgentRuntimeRole.MainOnly,
                    L("AgentConfiguration.RuntimeRole.MainOnly", "只允许作为会话流主代理")),
                new AgentRuntimeRoleOption(
                    AgentRuntimeRole.SubAgentOnly,
                    L("AgentConfiguration.RuntimeRole.SubAgentOnly", "只允许作为子代理")),
                new AgentRuntimeRoleOption(
                    AgentRuntimeRole.MainAndSubAgent,
                    L("AgentConfiguration.RuntimeRole.MainAndSubAgent", "两者皆可"))
            };
        }

        private void RefreshLocalizedText()
        {
            RefreshLocalizedOptionLists();

            OnPropertyChanged(nameof(Title));
            OnPropertyChanged(nameof(Description));
            OnPropertyChanged(nameof(PermissionModes));
            OnPropertyChanged(nameof(LanguageModelSelectionModes));
            OnPropertyChanged(nameof(RuntimeRoleOptions));
            OnPropertyChanged(nameof(AgentSummaryText));
            OnPropertyChanged(nameof(PromptBuildHintText));

            foreach (var agent in Agents)
            {
                agent.RefreshLocalizedText();
            }

            RefreshLanguageModelCatalogInternal();
        }

        private static string L(string resourceKey, string fallback)
        {
            return LocalizationRuntime.Instance.GetString(resourceKey, fallback);
        }

        private static string LF(string resourceKey, string fallback, params object[] args)
        {
            return string.Format(L(resourceKey, fallback), args);
        }

        private IDisposable SuspendPersistence()
        {
            _suspendPersistenceCounter++;
            return new DelegateDisposable(() => _suspendPersistenceCounter--);
        }
    }
}
