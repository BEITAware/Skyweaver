using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using Ferrita.Controls.AgentConfigurationControl.Models;
using Ferrita.Controls.AgentConfigurationControl.Services;
using Ferrita.Controls.LanguageModelConfigurationControl.Models;
using Ferrita.Controls.LanguageModelConfigurationControl.Services;
using Ferrita.Controls.WorkflowEditorControl.Models;
using Ferrita.Controls.WorkflowEditorControl.Services;
using Ferrita.Infrastructure.Mvvm;
using Ferrita.Models.ChatSession;
using Ferrita.Services.ChatSession;
using Ferrita.Services.Localization;

namespace Ferrita.Windows
{
    public partial class CreateChatSessionDialog : Window
    {
        private readonly CreateChatSessionDialogViewModel _viewModel;

        public string SessionName => _viewModel.SessionName.Trim();

        public ChatSessionFlowBinding? SelectedFlowBinding => _viewModel.SelectedFlowBinding?.ToBinding();

        public CreateChatSessionDialog(
            IReadOnlyList<ChatSessionFlowBindingOption>? sessionFlowOptions = null,
            string? initialSessionName = null)
        {
            _viewModel = new CreateChatSessionDialogViewModel(sessionFlowOptions, initialSessionName);
            InitializeComponent();
            DataContext = _viewModel;
            Loaded += OnLoaded;
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnLoaded;
            SessionNameTextBox.Focus();
            SessionNameTextBox.SelectAll();
            await Dispatcher.InvokeAsync(static () => { }, DispatcherPriority.Background);
            await _viewModel.InitializeAsync();
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SessionName))
            {
                MessageBox.Show(this, L("CreateChatSession.Validation.SessionNameRequired", "请输入会话名称。"), L("CreateChatSession.MessageBoxTitle", "创建会话"), MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (SelectedFlowBinding == null)
            {
                MessageBox.Show(this, L("CreateChatSession.Validation.FlowRequired", "请先为新会话选择一个会话流。"), L("CreateChatSession.MessageBoxTitle", "创建会话"), MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DialogResult = true;
        }

        private static string L(string resourceKey, string fallback)
        {
            return LocalizationRuntime.Instance.GetString(resourceKey, fallback);
        }
    }

    internal sealed class CreateChatSessionDialogViewModel : ObservableObject
    {
        private static readonly Brush InfoBackgroundBrush = CreateBrush(Color.FromArgb(0x2A, 0x68, 0x94, 0xD6));
        private static readonly Brush InfoBorderBrush = CreateBrush(Color.FromArgb(0x66, 0x9C, 0xC8, 0xFF));
        private static readonly Brush InfoForegroundBrush = CreateBrush(Color.FromArgb(0xFF, 0xE8, 0xF4, 0xFF));
        private static readonly Brush WarningBackgroundBrush = CreateBrush(Color.FromArgb(0x2E, 0xB7, 0x8A, 0x2F));
        private static readonly Brush WarningBorderBrush = CreateBrush(Color.FromArgb(0x66, 0xD8, 0xB4, 0x4E));
        private static readonly Brush WarningForegroundBrush = CreateBrush(Color.FromArgb(0xFF, 0xFF, 0xF1, 0xCC));
        private static readonly Brush ErrorBackgroundBrush = CreateBrush(Color.FromArgb(0x32, 0xB8, 0x49, 0x49));
        private static readonly Brush ErrorBorderBrush = CreateBrush(Color.FromArgb(0x66, 0xE2, 0x57, 0x57));
        private static readonly Brush ErrorForegroundBrush = CreateBrush(Color.FromArgb(0xFF, 0xFF, 0xE2, 0xE2));

        private readonly SessionFlowRepository _sessionFlowRepository;
        private readonly SessionFlowRuntimeCompiler _runtimeCompiler;
        private readonly IReadOnlyList<ChatSessionFlowBindingOption> _initialSessionFlowOptions;
        private readonly List<string> _globalStatusMessages = new();
        private Dictionary<string, AgentDefinition> _agentsById = new(StringComparer.OrdinalIgnoreCase);
        private Dictionary<string, LanguageModelDefinition> _languageModelsByKey = new(StringComparer.OrdinalIgnoreCase);
        private Dictionary<string, CapabilityLayerDefinition> _capabilityLayersByKey = new(StringComparer.OrdinalIgnoreCase);

        private string _sessionName;
        private string _flowDisplayName = L("CreateChatSession.Flow.NotSelected", "未选择会话流");
        private string _flowSubtitle = L("CreateChatSession.Flow.SelectPrompt", "请选择一个会话流以预览将要创建的 ChatSession。");
        private string _flowStatusText = L("CreateChatSession.Flow.NotSelectedStatus", "尚未选择会话流");
        private string _agentCountText = "0";
        private string _modelCountText = "0";
        private string _nodeCountText = "0";
        private string _connectionCountText = "0";
        private bool _hasRecentSessions;
        private bool _isHistoryEmpty = true;
        private bool _hasAgentPreviews;
        private bool _isAgentPreviewEmpty = true;
        private bool _hasModelPreviews;
        private bool _isModelPreviewEmpty = true;
        private CreateChatSessionFlowOptionViewModel? _selectedFlowOption;
        private CreateChatSessionHistoryItemViewModel? _selectedHistorySession;
        private bool _isInitialized;

        public CreateChatSessionDialogViewModel(
            IReadOnlyList<ChatSessionFlowBindingOption>? sessionFlowOptions,
            string? initialSessionName)
        {
            _sessionName = string.IsNullOrWhiteSpace(initialSessionName) ? L("SessionList.DefaultSessionName", "新建会话") : initialSessionName.Trim();
            _initialSessionFlowOptions = sessionFlowOptions ?? Array.Empty<ChatSessionFlowBindingOption>();
            SessionFlowOptions = new ObservableCollection<CreateChatSessionFlowOptionViewModel>();
            RecentSessions = new ObservableCollection<CreateChatSessionHistoryItemViewModel>();
            AgentPreviews = new ObservableCollection<CreateChatSessionAgentPreviewItemViewModel>();
            ModelPreviews = new ObservableCollection<CreateChatSessionModelPreviewItemViewModel>();
            FlowStatusItems = new ObservableCollection<CreateChatSessionIssueItemViewModel>();

            _sessionFlowRepository = new SessionFlowRepository(new SessionFlowPathProvider());
            _runtimeCompiler = new SessionFlowRuntimeCompiler();
        }

        public string SessionName
        {
            get => _sessionName;
            set
            {
                if (SetProperty(ref _sessionName, value ?? string.Empty))
                {
                    OnPropertyChanged(nameof(SessionNamePreview));
                    OnPropertyChanged(nameof(CanCreate));
                }
            }
        }

        public string SessionNamePreview
        {
            get
            {
                var trimmed = SessionName.Trim();
                return trimmed.Length == 0 ? L("CreateChatSession.Session.Unnamed", "未命名会话") : trimmed;
            }
        }

        public ObservableCollection<CreateChatSessionFlowOptionViewModel> SessionFlowOptions { get; }

        public CreateChatSessionFlowOptionViewModel? SelectedFlowOption
        {
            get => _selectedFlowOption;
            set
            {
                if (SetProperty(ref _selectedFlowOption, value))
                {
                    RefreshFlowPreview();
                    OnPropertyChanged(nameof(CanCreate));
                }
            }
        }

        public ChatSessionFlowBindingOption? SelectedFlowBinding => SelectedFlowOption?.BindingOption;

        public ObservableCollection<CreateChatSessionHistoryItemViewModel> RecentSessions { get; }

        public CreateChatSessionHistoryItemViewModel? SelectedHistorySession
        {
            get => _selectedHistorySession;
            set => SetProperty(ref _selectedHistorySession, value);
        }

        public bool HasRecentSessions
        {
            get => _hasRecentSessions;
            private set => SetProperty(ref _hasRecentSessions, value);
        }

        public bool IsHistoryEmpty
        {
            get => _isHistoryEmpty;
            private set => SetProperty(ref _isHistoryEmpty, value);
        }

        public string FlowDisplayName
        {
            get => _flowDisplayName;
            private set
            {
                if (SetProperty(ref _flowDisplayName, value))
                {
                    OnPropertyChanged(nameof(FlowHeaderText));
                }
            }
        }

        public string FlowHeaderText => LF("CreateChatSession.Flow.HeaderFormat", "会话流：{0}", FlowDisplayName);

        public string FlowSubtitle
        {
            get => _flowSubtitle;
            private set => SetProperty(ref _flowSubtitle, value);
        }

        public string FlowStatusText
        {
            get => _flowStatusText;
            private set => SetProperty(ref _flowStatusText, value);
        }

        public string AgentCountText
        {
            get => _agentCountText;
            private set => SetProperty(ref _agentCountText, value);
        }

        public string ModelCountText
        {
            get => _modelCountText;
            private set => SetProperty(ref _modelCountText, value);
        }

        public string NodeCountText
        {
            get => _nodeCountText;
            private set => SetProperty(ref _nodeCountText, value);
        }

        public string ConnectionCountText
        {
            get => _connectionCountText;
            private set => SetProperty(ref _connectionCountText, value);
        }

        public ObservableCollection<CreateChatSessionAgentPreviewItemViewModel> AgentPreviews { get; }

        public bool HasAgentPreviews
        {
            get => _hasAgentPreviews;
            private set => SetProperty(ref _hasAgentPreviews, value);
        }

        public bool IsAgentPreviewEmpty
        {
            get => _isAgentPreviewEmpty;
            private set => SetProperty(ref _isAgentPreviewEmpty, value);
        }

        public ObservableCollection<CreateChatSessionModelPreviewItemViewModel> ModelPreviews { get; }

        public bool HasModelPreviews
        {
            get => _hasModelPreviews;
            private set => SetProperty(ref _hasModelPreviews, value);
        }

        public bool IsModelPreviewEmpty
        {
            get => _isModelPreviewEmpty;
            private set => SetProperty(ref _isModelPreviewEmpty, value);
        }

        public ObservableCollection<CreateChatSessionIssueItemViewModel> FlowStatusItems { get; }

        public bool CanCreate => !string.IsNullOrWhiteSpace(SessionName) && SelectedFlowBinding != null;

        public async Task InitializeAsync()
        {
            if (_isInitialized)
            {
                return;
            }

            _isInitialized = true;
            LoadSessionFlowOptions(_initialSessionFlowOptions);

            await Task.Run(() =>
            {
                var languageModelPathProvider = new LanguageModelConfigurationPathProvider();
                _agentsById = LoadAgents();
                _languageModelsByKey = LoadLanguageModels(languageModelPathProvider);
                _capabilityLayersByKey = LoadCapabilityLayers(languageModelPathProvider);
            }).ConfigureAwait(true);

            LoadRecentSessions();
            RefreshFlowPreview();
        }

        private void LoadSessionFlowOptions(IReadOnlyList<ChatSessionFlowBindingOption>? sessionFlowOptions)
        {
            SessionFlowOptions.Clear();

            foreach (var option in sessionFlowOptions ?? Array.Empty<ChatSessionFlowBindingOption>())
            {
                SessionFlowOptions.Add(new CreateChatSessionFlowOptionViewModel(option));
            }

            _selectedFlowOption = SessionFlowOptions.Count > 0
                ? SessionFlowOptions[0]
                : null;
            OnPropertyChanged(nameof(SelectedFlowOption));
            OnPropertyChanged(nameof(CanCreate));
        }

        private void LoadRecentSessions()
        {
            RecentSessions.Clear();

            try
            {
                var repository = new ChatSessionRepository();
                foreach (var session in repository.LoadAll().Take(8))
                {
                    RecentSessions.Add(new CreateChatSessionHistoryItemViewModel
                    {
                        Title = session.Name,
                        FlowName = session.HasBoundFlow ? session.BoundFlowDisplayName : L("ChatSession.BoundFlow.Unbound", "未绑定会话流"),
                        UpdatedAtText = FormatTimestamp(session.UpdatedAtUtc),
                        IconPath = string.IsNullOrWhiteSpace(session.IconPath)
                            ? "pack://application:,,,/Resources/NewNodeGraphAlt.png"
                            : session.IconPath
                    });
                }
            }
            catch (Exception ex)
            {
                _globalStatusMessages.Add(LF("CreateChatSession.Status.HistoryLoadFailedFormat", "历史会话列表加载失败：{0}", ex.Message));
            }

            HasRecentSessions = RecentSessions.Count > 0;
            IsHistoryEmpty = !HasRecentSessions;
        }

        private void RefreshFlowPreview()
        {
            AgentPreviews.Clear();
            ModelPreviews.Clear();
            FlowStatusItems.Clear();

            if (SelectedFlowOption == null)
            {
                FlowDisplayName = L("CreateChatSession.Flow.NotSelected", "未选择会话流");
                FlowSubtitle = L("CreateChatSession.Flow.SelectPrompt", "请选择一个会话流以预览将要创建的 ChatSession。");
                FlowStatusText = L("CreateChatSession.Flow.NotSelectedStatus", "尚未选择会话流");
                AgentCountText = "0";
                ModelCountText = "0";
                NodeCountText = "0";
                ConnectionCountText = "0";
                ApplyEmptyFlags();
                AppendGlobalStatusItems();
                if (FlowStatusItems.Count == 0)
                {
                    FlowStatusItems.Add(CreateInfoIssue(
                        L("CreateChatSession.Issue.WaitingSelection.Title", "等待选择"),
                        L("CreateChatSession.Issue.WaitingSelection.Message", "选择会话流后，这里会展示代理、模型和流状态。")));
                }

                return;
            }

            SessionFlowGraphDocumentModel? document = null;
            SessionFlowCompilationResult? compilationResult = null;
            string? loadError = null;

            try
            {
                document = _sessionFlowRepository.Load(SelectedFlowOption.BindingOption.FilePath);
                compilationResult = _runtimeCompiler.Compile(document);
            }
            catch (Exception ex)
            {
                loadError = ex.Message;
            }

            if (document == null)
            {
                FlowDisplayName = SelectedFlowOption.DisplayName;
                FlowSubtitle = L("CreateChatSession.Flow.ReadFileFailed", "无法读取当前会话流文件。");
                FlowStatusText = L("CreateChatSession.Flow.ReadFailed", "读取失败");
                AgentCountText = "0";
                ModelCountText = "0";
                NodeCountText = "0";
                ConnectionCountText = "0";
                ApplyEmptyFlags();
                AppendGlobalStatusItems();
                FlowStatusItems.Add(CreateErrorIssue(
                    L("CreateChatSession.Issue.ReadFailed.Title", "无法读取"),
                    loadError ?? L("CreateChatSession.Issue.ReadFailed.Message", "未能读取当前会话流。")));
                return;
            }

            var graph = document.Graph ?? new SessionFlowGraphModel();
            var agentGroups = graph.Nodes
                .Where(node => node.Kind == SessionFlowNodeKind.Agent)
                .GroupBy(GetAgentGroupingKey, StringComparer.OrdinalIgnoreCase)
                .ToArray();

            var modelUsageMap = new Dictionary<string, CreateChatSessionModelUsageAccumulator>(StringComparer.OrdinalIgnoreCase);
            foreach (var group in agentGroups)
            {
                AgentPreviews.Add(BuildAgentPreview(group.ToArray(), modelUsageMap));
            }

            foreach (var preview in modelUsageMap.Values
                         .OrderBy(item => item.DisplayName, StringComparer.CurrentCultureIgnoreCase)
                         .ThenBy(item => item.ModelIdText, StringComparer.CurrentCultureIgnoreCase)
                         .Select(item => item.ToViewModel()))
            {
                ModelPreviews.Add(preview);
            }

            FlowDisplayName = string.IsNullOrWhiteSpace(document.Name) ? SelectedFlowOption.DisplayName : document.Name;
            FlowSubtitle = LF("CreateChatSession.Flow.SubtitleFormat", "文件：{0} · 更新于 {1}", Path.GetFileName(document.FilePath), FormatTimestamp(document.UpdatedAtUtc));
            FlowStatusText = BuildFlowStatusText(compilationResult);
            AgentCountText = AgentPreviews.Count.ToString(CultureInfo.InvariantCulture);
            ModelCountText = ModelPreviews.Count.ToString(CultureInfo.InvariantCulture);
            NodeCountText = graph.Nodes.Count.ToString(CultureInfo.InvariantCulture);
            ConnectionCountText = graph.Connections.Count.ToString(CultureInfo.InvariantCulture);

            AppendGlobalStatusItems();

            if (compilationResult == null)
            {
                FlowStatusItems.Add(CreateErrorIssue(
                    L("CreateChatSession.Issue.ReadFailed.Title", "读取失败"),
                    L("CreateChatSession.Issue.StatusUnavailable.Message", "未能为当前会话流生成状态信息。")));
            }
            else if (compilationResult.Issues.Count == 0)
            {
                FlowStatusItems.Add(CreateInfoIssue(
                    L("CreateChatSession.Issue.Ready.Title", "已就绪"),
                    L("CreateChatSession.Issue.Ready.Message", "当前会话流编译通过，可以直接用于创建新的 ChatSession。")));
            }
            else
            {
                var nodeMap = graph.Nodes
                    .Where(node => !string.IsNullOrWhiteSpace(node.Id))
                    .ToDictionary(node => node.Id, StringComparer.OrdinalIgnoreCase);

                foreach (var issue in compilationResult.Issues)
                {
                    FlowStatusItems.Add(CreateIssue(issue, nodeMap));
                }
            }

            ApplyEmptyFlags();
        }

        private void ApplyEmptyFlags()
        {
            HasAgentPreviews = AgentPreviews.Count > 0;
            IsAgentPreviewEmpty = !HasAgentPreviews;
            HasModelPreviews = ModelPreviews.Count > 0;
            IsModelPreviewEmpty = !HasModelPreviews;
        }

        private void AppendGlobalStatusItems()
        {
            foreach (var message in _globalStatusMessages)
            {
                FlowStatusItems.Add(CreateWarningIssue(L("CreateChatSession.Issue.AdditionalHint.Title", "附加提示"), message));
            }
        }

        private CreateChatSessionAgentPreviewItemViewModel BuildAgentPreview(
            IReadOnlyList<SessionFlowNodeModel> nodes,
            IDictionary<string, CreateChatSessionModelUsageAccumulator> modelUsageMap)
        {
            var firstNode = nodes[0];
            var agentId = firstNode.AgentId?.Trim() ?? string.Empty;
            var nodeSummary = BuildNodeSummary(nodes);

            if (agentId.Length == 0)
            {
                return CreateMissingAgentPreview(
                    string.IsNullOrWhiteSpace(firstNode.AgentDisplayName) ? firstNode.Title : firstNode.AgentDisplayName,
                    nodeSummary,
                    L("CreateChatSession.Agent.MissingDefinition", "代理节点尚未绑定 AgentDefinition。"));
            }

            if (!_agentsById.TryGetValue(agentId, out var agent))
            {
                return CreateMissingAgentPreview(
                    string.IsNullOrWhiteSpace(firstNode.AgentDisplayName) ? firstNode.Title : firstNode.AgentDisplayName,
                    nodeSummary,
                    LF("CreateChatSession.Agent.DefinitionNotFoundFormat", "未找到 AgentDefinition：{0}", agentId),
                    agentId);
            }

            var candidates = ResolveCandidateModels(agent);
            foreach (var candidate in candidates)
            {
                var usage = GetOrCreateModelUsage(modelUsageMap, candidate, agent.LanguageModelSelectionMode);
                usage.AgentNames.Add(agent.DisplayNameOrFallback);
            }

            var status = ResolveAgentStatus(agent, candidates);
            return new CreateChatSessionAgentPreviewItemViewModel
            {
                AvatarPath = string.IsNullOrWhiteSpace(agent.AvatarPreviewPath) ? AgentDefinition.DefaultAvatarPath : agent.AvatarPreviewPath,
                DisplayName = agent.DisplayNameOrFallback,
                AgentIdText = LF("CreateChatSession.Agent.IdFormat", "ID: {0}", agent.AgentIdOrFallback),
                NodeSummary = nodeSummary,
                ModeText = agent.IsStructuredXmlIO
                    ? L("CreateChatSession.Agent.Mode.StructuredXml", "结构化 XML")
                    : L("CreateChatSession.Agent.Mode.NaturalLanguage", "自然语言"),
                SelectionModeText = agent.LanguageModelSelectionMode == AgentLanguageModelSelectionMode.CapabilityLayer
                    ? L("CreateChatSession.Agent.SelectionMode.CapabilityLayer", "功能层级")
                    : L("CreateChatSession.Agent.SelectionMode.SpecificModel", "具体模型"),
                ModelSummary = BuildAgentModelSummary(agent, candidates),
                DescriptionText = BuildAgentDescription(agent),
                HasStatusBadge = status != null,
                StatusBadgeText = status?.Text ?? string.Empty,
                StatusBadgeBackground = status?.Background ?? Brushes.Transparent,
                StatusBadgeBorderBrush = status?.BorderBrush ?? Brushes.Transparent,
                StatusBadgeForeground = status?.Foreground ?? Brushes.White
            };
        }

        private CreateChatSessionAgentPreviewItemViewModel CreateMissingAgentPreview(
            string? displayName,
            string nodeSummary,
            string message,
            string? agentId = null)
        {
            return new CreateChatSessionAgentPreviewItemViewModel
            {
                AvatarPath = AgentDefinition.DefaultAvatarPath,
                DisplayName = string.IsNullOrWhiteSpace(displayName) ? L("CreateChatSession.Agent.Unbound", "未绑定代理") : displayName.Trim(),
                AgentIdText = string.IsNullOrWhiteSpace(agentId)
                    ? L("CreateChatSession.Agent.IdUnbound", "ID: 未绑定")
                    : LF("CreateChatSession.Agent.IdFormat", "ID: {0}", agentId.Trim()),
                NodeSummary = nodeSummary,
                ModeText = L("CreateChatSession.Agent.Mode.Unknown", "未知模式"),
                SelectionModeText = L("CreateChatSession.Agent.UnboundStatus", "未绑定"),
                ModelSummary = L("CreateChatSession.Agent.ModelSummary.Unresolved", "当前无法解析语言模型。"),
                DescriptionText = message,
                HasStatusBadge = true,
                StatusBadgeText = L("CreateChatSession.Agent.UnboundStatus", "未绑定"),
                StatusBadgeBackground = ErrorBackgroundBrush,
                StatusBadgeBorderBrush = ErrorBorderBrush,
                StatusBadgeForeground = ErrorForegroundBrush
            };
        }

        private IReadOnlyList<LanguageModelDefinition> ResolveCandidateModels(AgentDefinition agent)
        {
            if (agent.LanguageModelSelectionMode == AgentLanguageModelSelectionMode.CapabilityLayer)
            {
                if (string.IsNullOrWhiteSpace(agent.SelectedCapabilityLayerKey) ||
                    !_capabilityLayersByKey.TryGetValue(agent.SelectedCapabilityLayerKey, out var layer))
                {
                    return Array.Empty<LanguageModelDefinition>();
                }

                return layer.LanguageModels
                    .Select(entry => entry.LanguageModelKey?.Trim() ?? string.Empty)
                    .Where(key => key.Length > 0)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .Select(key => _languageModelsByKey.TryGetValue(key, out var model) ? model : null)
                    .Where(model => model != null)
                    .Cast<LanguageModelDefinition>()
                    .ToArray();
            }

            if (string.IsNullOrWhiteSpace(agent.SelectedLanguageModelKey) ||
                !_languageModelsByKey.TryGetValue(agent.SelectedLanguageModelKey, out var selectedModel))
            {
                return Array.Empty<LanguageModelDefinition>();
            }

            return [selectedModel];
        }

        private CreateChatSessionAgentStatus? ResolveAgentStatus(
            AgentDefinition agent,
            IReadOnlyList<LanguageModelDefinition> candidates)
        {
            if (agent.LanguageModelSelectionMode == AgentLanguageModelSelectionMode.CapabilityLayer)
            {
                if (string.IsNullOrWhiteSpace(agent.SelectedCapabilityLayerKey))
                {
                    return new CreateChatSessionAgentStatus(L("CreateChatSession.Agent.Status.ModelMissing", "模型缺失"), ErrorBackgroundBrush, ErrorBorderBrush, ErrorForegroundBrush);
                }

                if (!_capabilityLayersByKey.ContainsKey(agent.SelectedCapabilityLayerKey) || candidates.Count == 0)
                {
                    return new CreateChatSessionAgentStatus(L("CreateChatSession.Agent.Status.EmptyLayer", "层级为空"), WarningBackgroundBrush, WarningBorderBrush, WarningForegroundBrush);
                }

                if (candidates.Any(model => !model.IsFullyConfigured))
                {
                    return new CreateChatSessionAgentStatus(L("CreateChatSession.Agent.Status.IncompleteConfiguration", "配置不完整"), WarningBackgroundBrush, WarningBorderBrush, WarningForegroundBrush);
                }

                return null;
            }

            if (string.IsNullOrWhiteSpace(agent.SelectedLanguageModelKey))
            {
                return new CreateChatSessionAgentStatus(L("CreateChatSession.Agent.Status.ModelMissing", "模型缺失"), ErrorBackgroundBrush, ErrorBorderBrush, ErrorForegroundBrush);
            }

            if (candidates.Count == 0 || candidates.Any(model => !model.IsFullyConfigured))
            {
                return new CreateChatSessionAgentStatus(L("CreateChatSession.Agent.Status.IncompleteConfiguration", "配置不完整"), WarningBackgroundBrush, WarningBorderBrush, WarningForegroundBrush);
            }

            return null;
        }

        private string BuildAgentModelSummary(AgentDefinition agent, IReadOnlyList<LanguageModelDefinition> candidates)
        {
            if (agent.LanguageModelSelectionMode == AgentLanguageModelSelectionMode.CapabilityLayer)
            {
                var layerName = GetCapabilityLayerDisplayName(agent.SelectedCapabilityLayerKey);
                return candidates.Count == 0
                    ? LF("CreateChatSession.Agent.ModelSummary.LayerNoCandidatesFormat", "功能层级：{0}（未解析到候选模型）", layerName)
                    : LF("CreateChatSession.Agent.ModelSummary.LayerCandidatesFormat", "功能层级：{0}（候选 {1} 个）", layerName, candidates.Count);
            }

            if (string.IsNullOrWhiteSpace(agent.SelectedLanguageModelKey))
            {
                return L("CreateChatSession.Agent.ModelSummary.SpecificNotSelected", "具体模型：尚未选择语言模型。");
            }

            if (candidates.Count == 0)
            {
                return LF("CreateChatSession.Agent.ModelSummary.SpecificNotFoundFormat", "具体模型：未找到 {0}。", agent.SelectedLanguageModelKey);
            }

            return LF("CreateChatSession.Agent.ModelSummary.SpecificFormat", "具体模型：{0}", GetLanguageModelDisplayName(candidates[0]));
        }

        private static string BuildAgentDescription(AgentDefinition agent)
        {
            var segments = new List<string>();

            if (!string.IsNullOrWhiteSpace(agent.InputDescription))
            {
                segments.Add(LF("CreateChatSession.Agent.Description.InputFormat", "输入：{0}", agent.InputDescription.Trim()));
            }

            if (!string.IsNullOrWhiteSpace(agent.OutputDescription))
            {
                segments.Add(LF("CreateChatSession.Agent.Description.OutputFormat", "输出：{0}", agent.OutputDescription.Trim()));
            }

            if (segments.Count > 0)
            {
                return string.Join(" · ", segments);
            }

            return agent.IsStructuredXmlIO
                ? L("CreateChatSession.Agent.Description.StructuredXml", "该代理使用结构化 XML 输入输出。")
                : L("CreateChatSession.Agent.Description.NaturalLanguage", "该代理使用自然语言输入输出。");
        }

        private static string BuildNodeSummary(IReadOnlyList<SessionFlowNodeModel> nodes)
        {
            var titles = nodes
                .Select(node =>
                {
                    var title = node.Title?.Trim() ?? string.Empty;
                    return title.Length == 0 ? L("CreateChatSession.Node.Unnamed", "未命名节点") : title;
                })
                .Distinct(StringComparer.CurrentCultureIgnoreCase)
                .ToArray();

            if (titles.Length == 1)
            {
                return LF("CreateChatSession.Node.SummarySingleFormat", "节点：{0}", titles[0]);
            }

            var previewTitles = titles.Take(3).ToArray();
            var suffix = titles.Length > previewTitles.Length ? "..." : string.Empty;
            return LF("CreateChatSession.Node.SummaryMultipleFormat", "节点：{0} 个（{1}{2}）", titles.Length, string.Join(L("Common.ListSeparator.IdeographicComma", "、"), previewTitles), suffix);
        }

        private static string GetAgentGroupingKey(SessionFlowNodeModel node)
        {
            var agentId = node.AgentId?.Trim() ?? string.Empty;
            return agentId.Length > 0 ? agentId : node.Id;
        }

        private CreateChatSessionIssueItemViewModel CreateIssue(
            SessionFlowCompilationIssue issue,
            IReadOnlyDictionary<string, SessionFlowNodeModel> nodeMap)
        {
            var message = issue.Message;
            if (!string.IsNullOrWhiteSpace(issue.NodeId) && nodeMap.TryGetValue(issue.NodeId, out var node))
            {
                var nodeTitle = node.Title?.Trim();
                if (!string.IsNullOrWhiteSpace(nodeTitle))
                {
                    message = LF("CreateChatSession.Issue.NodeMessageFormat", "{0}：{1}", nodeTitle, message);
                }
            }

            return issue.Severity == SessionFlowCompilationIssueSeverity.Error
                ? CreateErrorIssue(L("CreateChatSession.Issue.Severity.Error", "错误"), message)
                : CreateWarningIssue(L("CreateChatSession.Issue.Severity.Warning", "警告"), message);
        }

        private static string BuildFlowStatusText(SessionFlowCompilationResult? compilationResult)
        {
            if (compilationResult == null)
            {
                return L("CreateChatSession.Flow.Status.Unavailable", "无法分析会话流状态");
            }

            var errorCount = compilationResult.Issues.Count(issue => issue.Severity == SessionFlowCompilationIssueSeverity.Error);
            var warningCount = compilationResult.Issues.Count(issue => issue.Severity == SessionFlowCompilationIssueSeverity.Warning);

            if (errorCount == 0 && warningCount == 0)
            {
                return L("CreateChatSession.Flow.Status.Ready", "当前会话流已就绪");
            }

            if (errorCount == 0)
            {
                return LF("CreateChatSession.Flow.Status.WarningsFormat", "存在 {0} 个警告", warningCount);
            }

            if (warningCount == 0)
            {
                return LF("CreateChatSession.Flow.Status.ErrorsFormat", "存在 {0} 个错误", errorCount);
            }

            return LF("CreateChatSession.Flow.Status.ErrorsAndWarningsFormat", "存在 {0} 个错误，{1} 个警告", errorCount, warningCount);
        }

        private static string FormatTimestamp(DateTime utcDateTime)
        {
            var localTime = utcDateTime.Kind == DateTimeKind.Utc
                ? utcDateTime.ToLocalTime()
                : utcDateTime;

            return localTime.ToString(L("CreateChatSession.TimestampFormat", "yyyy-MM-dd HH:mm"), CultureInfo.CurrentCulture);
        }

        private Dictionary<string, AgentDefinition> LoadAgents()
        {
            try
            {
                return new AgentConfigurationRepository(new AgentConfigurationPathProvider())
                    .Load()
                    .Where(agent => !string.IsNullOrWhiteSpace(agent.AgentId))
                    .ToDictionary(agent => agent.AgentId, StringComparer.OrdinalIgnoreCase);
            }
            catch (Exception ex)
            {
                _globalStatusMessages.Add(LF("CreateChatSession.Status.AgentConfigurationLoadFailedFormat", "代理配置读取失败：{0}", ex.Message));
                return new Dictionary<string, AgentDefinition>(StringComparer.OrdinalIgnoreCase);
            }
        }

        private Dictionary<string, LanguageModelDefinition> LoadLanguageModels(LanguageModelConfigurationPathProvider pathProvider)
        {
            try
            {
                return new LanguageModelConfigurationRepository(pathProvider)
                    .Load()
                    .Where(model => !string.IsNullOrWhiteSpace(model.Key))
                    .ToDictionary(model => model.Key, StringComparer.OrdinalIgnoreCase);
            }
            catch (Exception ex)
            {
                _globalStatusMessages.Add(LF("CreateChatSession.Status.LanguageModelConfigurationLoadFailedFormat", "语言模型配置读取失败：{0}", ex.Message));
                return new Dictionary<string, LanguageModelDefinition>(StringComparer.OrdinalIgnoreCase);
            }
        }

        private Dictionary<string, CapabilityLayerDefinition> LoadCapabilityLayers(LanguageModelConfigurationPathProvider pathProvider)
        {
            try
            {
                return new CapabilityLayerConfigurationRepository(pathProvider)
                    .Load()
                    .Where(layer => !string.IsNullOrWhiteSpace(layer.Key))
                    .ToDictionary(layer => layer.Key, StringComparer.OrdinalIgnoreCase);
            }
            catch (Exception ex)
            {
                _globalStatusMessages.Add(LF("CreateChatSession.Status.CapabilityLayerConfigurationLoadFailedFormat", "功能层级配置读取失败：{0}", ex.Message));
                return new Dictionary<string, CapabilityLayerDefinition>(StringComparer.OrdinalIgnoreCase);
            }
        }

        private static CreateChatSessionModelUsageAccumulator GetOrCreateModelUsage(
            IDictionary<string, CreateChatSessionModelUsageAccumulator> usageMap,
            LanguageModelDefinition model,
            AgentLanguageModelSelectionMode selectionMode)
        {
            if (usageMap.TryGetValue(model.Key, out var existing))
            {
                existing.SourceTypes.Add(selectionMode == AgentLanguageModelSelectionMode.CapabilityLayer
                    ? L("CreateChatSession.Agent.SelectionMode.CapabilityLayer", "功能层级")
                    : L("CreateChatSession.Agent.SelectionMode.SpecificModel", "具体模型"));
                return existing;
            }

            var created = new CreateChatSessionModelUsageAccumulator(model);
            created.SourceTypes.Add(selectionMode == AgentLanguageModelSelectionMode.CapabilityLayer
                ? L("CreateChatSession.Agent.SelectionMode.CapabilityLayer", "功能层级")
                : L("CreateChatSession.Agent.SelectionMode.SpecificModel", "具体模型"));
            usageMap[model.Key] = created;
            return created;
        }

        private string GetCapabilityLayerDisplayName(string? capabilityLayerKey)
        {
            if (string.IsNullOrWhiteSpace(capabilityLayerKey))
            {
                return L("CreateChatSession.CapabilityLayer.NotSelected", "未选择功能层级");
            }

            if (_capabilityLayersByKey.TryGetValue(capabilityLayerKey, out var layer))
            {
                return string.IsNullOrWhiteSpace(layer.Name)
                    ? LF("CreateChatSession.CapabilityLayer.UnnamedFormat", "未命名功能层级 ({0})", TrimKey(capabilityLayerKey))
                    : layer.Name.Trim();
            }

            return capabilityLayerKey.Trim();
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
                return LF("CreateChatSession.Model.UnnamedFormat", "未命名模型 ({0})", summaryModelId);
            }

            return LF("CreateChatSession.Model.UnnamedFormat", "未命名模型 ({0})", TrimKey(model.Key));
        }

        private static string TrimKey(string? key)
        {
            var value = key?.Trim() ?? string.Empty;
            return value.Length <= 8 ? value : value[..8];
        }

        private static string L(string resourceKey, string fallback)
        {
            return LocalizationRuntime.Instance.GetString(resourceKey, fallback);
        }

        private static string LF(string resourceKey, string fallbackFormat, params object?[] args)
        {
            var format = L(resourceKey, fallbackFormat);
            return string.Format(format, args);
        }

        private static Brush CreateBrush(Color color)
        {
            var brush = new SolidColorBrush(color);
            brush.Freeze();
            return brush;
        }

        private static CreateChatSessionIssueItemViewModel CreateInfoIssue(string title, string message)
        {
            return CreateIssueItem(title, message, InfoBackgroundBrush, InfoBorderBrush, InfoForegroundBrush);
        }

        private static CreateChatSessionIssueItemViewModel CreateWarningIssue(string title, string message)
        {
            return CreateIssueItem(title, message, WarningBackgroundBrush, WarningBorderBrush, WarningForegroundBrush);
        }

        private static CreateChatSessionIssueItemViewModel CreateErrorIssue(string title, string message)
        {
            return CreateIssueItem(title, message, ErrorBackgroundBrush, ErrorBorderBrush, ErrorForegroundBrush);
        }

        private static CreateChatSessionIssueItemViewModel CreateIssueItem(
            string title,
            string message,
            Brush background,
            Brush border,
            Brush foreground)
        {
            return new CreateChatSessionIssueItemViewModel
            {
                Title = title,
                Message = message,
                AccentBackground = background,
                AccentBorderBrush = border,
                AccentForeground = foreground
            };
        }
    }

    internal sealed class CreateChatSessionFlowOptionViewModel
    {
        public CreateChatSessionFlowOptionViewModel(ChatSessionFlowBindingOption bindingOption)
        {
            BindingOption = bindingOption ?? throw new ArgumentNullException(nameof(bindingOption));
        }

        public ChatSessionFlowBindingOption BindingOption { get; }

        public string DisplayName => BindingOption.DisplayName;
    }

    internal sealed class CreateChatSessionHistoryItemViewModel
    {
        public string Title { get; init; } = string.Empty;

        public string FlowName { get; init; } = string.Empty;

        public string UpdatedAtText { get; init; } = string.Empty;

        public string IconPath { get; init; } = "pack://application:,,,/Resources/NewNodeGraphAlt.png";
    }

    internal sealed class CreateChatSessionAgentPreviewItemViewModel
    {
        public string AvatarPath { get; init; } = AgentDefinition.DefaultAvatarPath;

        public string DisplayName { get; init; } = string.Empty;

        public string AgentIdText { get; init; } = string.Empty;

        public string NodeSummary { get; init; } = string.Empty;

        public string ModeText { get; init; } = string.Empty;

        public string SelectionModeText { get; init; } = string.Empty;

        public string ModelSummary { get; init; } = string.Empty;

        public string DescriptionText { get; init; } = string.Empty;

        public bool HasStatusBadge { get; init; }

        public string StatusBadgeText { get; init; } = string.Empty;

        public Brush StatusBadgeBackground { get; init; } = Brushes.Transparent;

        public Brush StatusBadgeBorderBrush { get; init; } = Brushes.Transparent;

        public Brush StatusBadgeForeground { get; init; } = Brushes.White;
    }

    internal sealed class CreateChatSessionModelPreviewItemViewModel
    {
        public string DisplayName { get; init; } = string.Empty;

        public string ModelIdText { get; init; } = string.Empty;

        public string InterfaceTypeText { get; init; } = string.Empty;

        public string SourceTypeText { get; init; } = string.Empty;

        public string UsedByText { get; init; } = string.Empty;
    }

    internal sealed class CreateChatSessionIssueItemViewModel
    {
        public string Title { get; init; } = string.Empty;

        public string Message { get; init; } = string.Empty;

        public Brush AccentBackground { get; init; } = Brushes.Transparent;

        public Brush AccentBorderBrush { get; init; } = Brushes.Transparent;

        public Brush AccentForeground { get; init; } = Brushes.White;
    }

    internal sealed class CreateChatSessionModelUsageAccumulator
    {
        public CreateChatSessionModelUsageAccumulator(LanguageModelDefinition model)
        {
            Model = model ?? throw new ArgumentNullException(nameof(model));
        }

        public LanguageModelDefinition Model { get; }

        public HashSet<string> AgentNames { get; } = new(StringComparer.CurrentCultureIgnoreCase);

        public HashSet<string> SourceTypes { get; } = new(StringComparer.CurrentCultureIgnoreCase);

        public string DisplayName
        {
            get
            {
                var displayName = Model.DisplayName?.Trim() ?? string.Empty;
                if (displayName.Length > 0)
                {
                    return displayName;
                }

                var summaryModelId = Model.SummaryModelId?.Trim() ?? string.Empty;
                if (summaryModelId.Length > 0)
                {
                    return LF("CreateChatSession.Model.UnnamedFormat", "未命名模型 ({0})", summaryModelId);
                }

                var key = Model.Key?.Trim() ?? string.Empty;
                return LF("CreateChatSession.Model.UnnamedFormat", "未命名模型 ({0})", key.Length <= 8 ? key : key[..8]);
            }
        }

        public string ModelIdText => string.IsNullOrWhiteSpace(Model.SummaryModelId)
            ? $"Key: {Model.Key}"
            : LF("CreateChatSession.Model.ModelIdFormat", "模型 ID: {0}", Model.SummaryModelId);

        public CreateChatSessionModelPreviewItemViewModel ToViewModel()
        {
            var sourceTypeText = string.Join(" / ", SourceTypes.OrderBy(item => item, StringComparer.CurrentCultureIgnoreCase));
            var agentList = AgentNames.OrderBy(item => item, StringComparer.CurrentCultureIgnoreCase).ToArray();
            var usedByText = agentList.Length == 0
                ? L("CreateChatSession.Model.UsedBy.None", "尚未关联代理")
                : LF("CreateChatSession.Model.UsedBy.AgentsFormat", "代理：{0}", string.Join(L("Common.ListSeparator.IdeographicComma", "、"), agentList));

            return new CreateChatSessionModelPreviewItemViewModel
            {
                DisplayName = DisplayName,
                ModelIdText = ModelIdText,
                InterfaceTypeText = string.IsNullOrWhiteSpace(Model.InterfaceType) ? L("CreateChatSession.Model.Interface.Unknown", "未知接口") : Model.InterfaceType,
                SourceTypeText = sourceTypeText.Length == 0 ? L("CreateChatSession.Model.Source.Unspecified", "未说明来源") : sourceTypeText,
                UsedByText = usedByText
            };
        }

        private static string L(string resourceKey, string fallback)
        {
            return LocalizationRuntime.Instance.GetString(resourceKey, fallback);
        }

        private static string LF(string resourceKey, string fallbackFormat, params object?[] args)
        {
            var format = L(resourceKey, fallbackFormat);
            return string.Format(format, args);
        }
    }

    internal sealed record CreateChatSessionAgentStatus(
        string Text,
        Brush Background,
        Brush BorderBrush,
        Brush Foreground);
}
