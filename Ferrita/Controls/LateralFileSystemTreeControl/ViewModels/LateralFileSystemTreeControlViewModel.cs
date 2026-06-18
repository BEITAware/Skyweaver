using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Ferrita.Commands;
using Ferrita.Infrastructure.Mvvm;
using Ferrita.Models.LateralFileSystem;
using Ferrita.Services.Localization;
using Ferrita.Services.LateralFileSystem;
using Ferrita.Windows;

namespace Ferrita.Controls.LateralFileSystemTreeControl.ViewModels
{
    public sealed class LateralFileSystemTreeControlViewModel : ObservableObject
    {
        private readonly LateralFileSystemRuntime _runtime;
        private LateralFileSystemNodeViewModel? _selectedNode;
        private string _currentWorkingRootDirectory = string.Empty;
        private bool _isBackendConfiguredEnabled;
        private bool _isVirtualizationBackendAvailable;
        private string _statusMessage = L("LateralFileSystemTree.Status.Connecting", "正在连接侧向文件系统后端…");
        private string _selectedNodeHydratedSizeText = L("LateralFileSystemTree.Selection.None", "未选择侧向文件夹");
        private string _selectedNodeFileSummaryText = string.Empty;
        private string _selectedNodeStorageNote = string.Empty;
        private string _selectedNodeInspectorStatusText = string.Empty;
        private bool _isLoadingSelectedNodeInspector;
        private int _selectedNodeRefreshVersion;
        private readonly int _instanceNumber;

        public LateralFileSystemTreeControlViewModel(int instanceNumber)
        {
            _instanceNumber = instanceNumber;
            var stopwatch = Stopwatch.StartNew();
            LateralFileSystemDebugConsole.Write("TreeVM", $"Constructor start; instanceNumber={instanceNumber}.");
            _runtime = LateralFileSystemRuntime.Instance;

            CreateProjectionFolderCommand = new RelayCommand(ExecuteCreateProjectionFolder, () => CanManageFolders);
            CreateInheritanceFolderCommand = new RelayCommand(ExecuteCreateInheritanceFolder, () => CanManageFolders && SelectedNode != null);
            MergeFolderCommand = new AsyncRelayCommand(ExecuteMergeFolderAsync, () => CanManageFolders && SelectedNode != null);
            DeleteFolderCommand = new RelayCommand(ExecuteDeleteFolder, () => CanManageFolders && SelectedNode != null);
            RefreshTreeCommand = new RelayCommand(() => RefreshFromBackend(preserveSelection: true));
            RefreshSelectedNodeFilesCommand = new AsyncRelayCommand(
                () => RefreshSelectedNodeInspectorAsync(syncSelectedNode: true, refreshStorageSummary: true),
                () => SelectedNode != null);

            SelectedNodeFileTreeRoots.CollectionChanged += (_, _) => OnPropertyChanged(nameof(HasSelectedNodeFiles));

            WeakEventManager<LateralFileSystemRuntime, EventArgs>.AddHandler(_runtime, nameof(LateralFileSystemRuntime.ConfigurationChanged), OnRuntimeConfigurationChanged);
            WeakEventManager<LateralFileSystemRuntime, EventArgs>.AddHandler(_runtime, nameof(LateralFileSystemRuntime.TreeChanged), OnRuntimeTreeChanged);
            WeakEventManager<LateralFileSystemRuntime, LateralFileSystemSourceChangedEventArgs>.AddHandler(_runtime, nameof(LateralFileSystemRuntime.SourceChanged), OnRuntimeSourceChanged);
            LocalizationRuntime.Instance.LanguageChanged += (_, _) => RefreshLocalizedText();

            RefreshFromBackend(preserveSelection: false);
            LateralFileSystemDebugConsole.Write("TreeVM", $"Constructor end; instanceNumber={instanceNumber}; elapsedMs={stopwatch.ElapsedMilliseconds}.");
        }

        public string Title => _instanceNumber > 1
            ? LF("LateralFileSystemTree.Title.NumberedFormat", "侧向文件系统树 {0}", _instanceNumber)
            : L("LateralFileSystemTree.Title", "侧向文件系统树");

        public string Description => L("LateralFileSystemTree.Description", "连接本应用程序的 LateralFS 后端，管理侧向文件夹、继承关系和实际文件占用。");

        public ObservableCollection<LateralFileSystemNodeViewModel> Nodes { get; } = new();

        public ObservableCollection<LateralFileSystemLinkViewModel> Links { get; } = new();

        public ObservableCollection<LateralFileSystemFileTreeNodeViewModel> SelectedNodeFileTreeRoots { get; } = new();

        public LateralFileSystemNodeViewModel? SelectedNode
        {
            get => _selectedNode;
            set
            {
                if (_selectedNode != null)
                {
                    _selectedNode.IsSelected = false;
                }

                if (!SetProperty(ref _selectedNode, value))
                {
                    return;
                }

                if (_selectedNode != null)
                {
                    _selectedNode.IsSelected = true;
                }

                LateralFileSystemDebugConsole.Write("TreeVM", $"SelectedNode changed to '{_selectedNode?.Name}' ({_selectedNode?.Id ?? "<null>"}).");

                OnPropertyChanged(nameof(HasSelectedNode));
                OnPropertyChanged(nameof(SelectedNodeParentName));
                OnPropertyChanged(nameof(SelectedNodeKindText));
                OnPropertyChanged(nameof(SelectedNodeOwnerText));
                OnPropertyChanged(nameof(SelectedNodeParentText));
                OnPropertyChanged(nameof(SelectedNodeVirtualRootText));
                OnPropertyChanged(nameof(SelectedNodeProjectionSourceText));
                OnPropertyChanged(nameof(SelectionPlaceholderText));
                CommandManager.InvalidateRequerySuggested();
                _ = RefreshSelectedNodeInspectorAsync(syncSelectedNode: false, refreshStorageSummary: false);
            }
        }

        public bool HasSelectedNode => SelectedNode != null;

        public bool HasSelectedNodeFiles => SelectedNodeFileTreeRoots.Count > 0;

        public string CurrentWorkingRootDirectory
        {
            get => _currentWorkingRootDirectory;
            private set
            {
                if (SetProperty(ref _currentWorkingRootDirectory, value))
                {
                    OnPropertyChanged(nameof(HasWorkingRootDirectory));
                    OnPropertyChanged(nameof(CurrentWorkingRootDirectoryDisplay));
                    OnPropertyChanged(nameof(CanManageFolders));
                    OnPropertyChanged(nameof(BackendAvailabilityText));
                    OnPropertyChanged(nameof(SelectionPlaceholderText));
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public string CurrentWorkingRootDirectoryDisplay => HasWorkingRootDirectory
            ? CurrentWorkingRootDirectory
            : L("LateralFileSystemTree.WorkingRoot.NotConfigured", "尚未设置工作根目录");

        public bool HasWorkingRootDirectory => !string.IsNullOrWhiteSpace(CurrentWorkingRootDirectory);

        public bool IsBackendConfiguredEnabled
        {
            get => _isBackendConfiguredEnabled;
            private set
            {
                if (SetProperty(ref _isBackendConfiguredEnabled, value))
                {
                    OnPropertyChanged(nameof(IsBackendEnabled));
                    OnPropertyChanged(nameof(CanManageFolders));
                    OnPropertyChanged(nameof(BackendAvailabilityText));
                    OnPropertyChanged(nameof(SelectionPlaceholderText));
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public bool IsVirtualizationBackendAvailable
        {
            get => _isVirtualizationBackendAvailable;
            private set
            {
                if (SetProperty(ref _isVirtualizationBackendAvailable, value))
                {
                    OnPropertyChanged(nameof(IsBackendEnabled));
                    OnPropertyChanged(nameof(CanManageFolders));
                    OnPropertyChanged(nameof(BackendAvailabilityText));
                    OnPropertyChanged(nameof(SelectionPlaceholderText));
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public bool IsBackendEnabled => IsBackendConfiguredEnabled && IsVirtualizationBackendAvailable;

        public bool CanManageFolders => IsBackendEnabled && HasWorkingRootDirectory;

        public string BackendAvailabilityText
        {
            get
            {
                if (!IsVirtualizationBackendAvailable)
                {
                    return _runtime.VirtualizationBackendStatusMessage;
                }

                if (!HasWorkingRootDirectory)
                {
                    return L("LateralFileSystemTree.Backend.WorkingRootMissing", "尚未设置工作根目录。请先到首选项中的“侧向文件系统”页完成连接。");
                }

                if (!IsBackendConfiguredEnabled)
                {
                    return L("LateralFileSystemTree.Backend.Disabled", "LateralFS 已配置工作根目录，但当前未启用，因此创建、继承、合并和删除操作已禁用。");
                }

                return L("LateralFileSystemTree.Backend.Connected", "已连接到 LateralFS 后端。");
            }
        }

        public string SelectionPlaceholderText
        {
            get
            {
                if (!IsVirtualizationBackendAvailable)
                {
                    return L("LateralFileSystemTree.Selection.BackendUnavailable", "当前系统无法启动侧向文件系统虚拟化后端。已保存节点仍可查看，但创建、继承、合并和删除操作已禁用。");
                }

                if (!HasWorkingRootDirectory)
                {
                    return L("LateralFileSystemTree.Selection.WorkingRootMissing", "请先在首选项中的“侧向文件系统”页设置工作根目录。");
                }

                if (!IsBackendConfiguredEnabled)
                {
                    return L("LateralFileSystemTree.Selection.BackendDisabled", "当前后端未启用。可以先查看已有节点，创建、合并和删除操作会保持禁用。");
                }

                return Nodes.Count == 0
                    ? L("LateralFileSystemTree.Selection.NoNodes", "尚未创建任何侧向文件夹。")
                    : L("LateralFileSystemTree.Selection.SelectFolder", "请先在左侧画布中选择一个侧向文件夹。");
            }
        }

        public string StatusMessage
        {
            get => _statusMessage;
            private set => SetProperty(ref _statusMessage, value);
        }

        public bool IsLoadingSelectedNodeInspector
        {
            get => _isLoadingSelectedNodeInspector;
            private set => SetProperty(ref _isLoadingSelectedNodeInspector, value);
        }

        public string SelectedNodeHydratedSizeText
        {
            get => _selectedNodeHydratedSizeText;
            private set => SetProperty(ref _selectedNodeHydratedSizeText, value);
        }

        public string SelectedNodeFileSummaryText
        {
            get => _selectedNodeFileSummaryText;
            private set => SetProperty(ref _selectedNodeFileSummaryText, value);
        }

        public string SelectedNodeStorageNote
        {
            get => _selectedNodeStorageNote;
            private set => SetProperty(ref _selectedNodeStorageNote, value);
        }

        public string SelectedNodeInspectorStatusText
        {
            get => _selectedNodeInspectorStatusText;
            private set => SetProperty(ref _selectedNodeInspectorStatusText, value);
        }

        public string SelectedNodeParentName
        {
            get
            {
                if (SelectedNode?.ParentNodeId is null)
                {
                    return L("Common.None", "无");
                }

                return Nodes.FirstOrDefault(node => string.Equals(node.Id, SelectedNode.ParentNodeId, StringComparison.OrdinalIgnoreCase))?.Name
                    ?? L("Common.None", "无");
            }
        }

        public string SelectedNodeKindText => SelectedNode is null
            ? string.Empty
            : LF("LateralFileSystemTree.SelectedNode.KindFormat", "类型：{0}", SelectedNode.Kind);

        public string SelectedNodeOwnerText => SelectedNode is null
            ? string.Empty
            : LF("LateralFileSystemTree.SelectedNode.OwnerFormat", "所有者：{0}", SelectedNode.Owner);

        public string SelectedNodeParentText => LF("LateralFileSystemTree.SelectedNode.ParentFormat", "继承来源：{0}", SelectedNodeParentName);

        public string SelectedNodeVirtualRootText => SelectedNode is null
            ? string.Empty
            : LF("LateralFileSystemTree.SelectedNode.VirtualRootFormat", "虚拟根：{0}", SelectedNode.VirtualRootPath);

        public string SelectedNodeProjectionSourceText => LF(
            "LateralFileSystemTree.SelectedNode.ProjectionSourceFormat",
            "投影源：{0}",
            string.IsNullOrWhiteSpace(SelectedNode?.ProjectionSourcePath)
                ? L("Common.NotConfigured", "未设置")
                : SelectedNode.ProjectionSourcePath);

        public ICommand CreateProjectionFolderCommand { get; }

        public ICommand CreateInheritanceFolderCommand { get; }

        public ICommand DeleteFolderCommand { get; }

        public ICommand MergeFolderCommand { get; }

        public ICommand RefreshTreeCommand { get; }

        public ICommand RefreshSelectedNodeFilesCommand { get; }

        public void UpdateLinks()
        {
            Links.Clear();

            foreach (var node in Nodes)
            {
                if (string.IsNullOrWhiteSpace(node.ParentNodeId))
                {
                    continue;
                }

                var parent = Nodes.FirstOrDefault(candidate => string.Equals(candidate.Id, node.ParentNodeId, StringComparison.OrdinalIgnoreCase));
                if (parent != null)
                {
                    Links.Add(new LateralFileSystemLinkViewModel(parent, node));
                }
            }
        }

        public void PersistNodeVisualPosition(LateralFileSystemNodeViewModel node)
        {
            _runtime.SaveNodeVisualPosition(node.Id, node.X, node.Y);
        }

        private void ExecuteCreateProjectionFolder()
        {
            if (!EnsureBackendReadyForMutation())
            {
                return;
            }

            var dialog = new LateralFileSystemFolderDialog(
                title: L("LateralFileSystemTree.CreateProjectionFolder", "新建侧向文件夹"),
                promptText: L("LateralFileSystemTree.Dialog.CreateProjectionPrompt", "创建一个新的侧向文件夹，并指定被投影的源文件夹。"),
                confirmButtonText: L("Common.Create", "创建"));
            dialog.Owner = Application.Current?.MainWindow;

            if (dialog.ShowDialog() != true)
            {
                return;
            }

            try
            {
                var createdNode = _runtime.CreateProjection(dialog.FolderDisplayName, dialog.SourceFolderPath);
                SaveInitialNodePosition(createdNode, parentNode: null);
                RefreshFromBackend(preserveSelection: false, preferredSelectedNodeId: createdNode.Id);
                StatusMessage = LF("LateralFileSystemTree.Status.CreatedFormat", "已新建侧向文件夹“{0}”。", createdNode.Name);
            }
            catch (Exception ex)
            {
                MessageBox.Show(Application.Current?.MainWindow, ex.Message, L("LateralFileSystemTree.CreateProjectionFolder", "新建侧向文件夹"), MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ExecuteCreateInheritanceFolder()
        {
            if (!EnsureBackendReadyForMutation())
            {
                return;
            }

            if (SelectedNode is null)
            {
                MessageBox.Show(Application.Current?.MainWindow, L("LateralFileSystemTree.Validation.SelectInheritanceSource", "请先选择一个作为继承源的侧向文件夹。"), L("LateralFileSystemTree.CreateInheritanceFolder", "继承侧向文件夹"), MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var parentNode = SelectedNode;
            var dialog = new LateralFileSystemFolderDialog(
                title: L("LateralFileSystemTree.CreateInheritanceFolder", "继承侧向文件夹"),
                promptText: L("LateralFileSystemTree.Dialog.CreateInheritancePrompt", "创建一个继承当前节点的侧向文件夹，内容默认投影父节点。"),
                confirmButtonText: L("LateralFileSystemTree.Inherit", "继承"),
                inheritedFromName: parentNode.Name,
                requiresSourcePath: false);
            dialog.Owner = Application.Current?.MainWindow;

            if (dialog.ShowDialog() != true)
            {
                return;
            }

            try
            {
                var createdNode = _runtime.CreateInheritance(dialog.FolderDisplayName, parentNode.Id, string.Empty);
                SaveInitialNodePosition(createdNode, parentNode);
                RefreshFromBackend(preserveSelection: false, preferredSelectedNodeId: createdNode.Id);
                StatusMessage = LF("LateralFileSystemTree.Status.InheritedFormat", "已创建继承自“{0}”的侧向文件夹“{1}”。", parentNode.Name, createdNode.Name);
            }
            catch (Exception ex)
            {
                MessageBox.Show(Application.Current?.MainWindow, ex.Message, L("LateralFileSystemTree.CreateInheritanceFolder", "继承侧向文件夹"), MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ExecuteDeleteFolder()
        {
            if (SelectedNode is null)
            {
                return;
            }

            var hasChildren = Nodes.Any(node => string.Equals(node.ParentNodeId, SelectedNode.Id, StringComparison.OrdinalIgnoreCase));
            if (hasChildren)
            {
                MessageBox.Show(
                    Application.Current?.MainWindow,
                    LF("LateralFileSystemTree.Delete.HasChildrenFormat", "侧向文件夹“{0}”存在向下的继承关系，当前不支持级联删除，请先删除其继承子项。", SelectedNode.Name),
                    L("LateralFileSystemTree.DeleteFolder", "删除侧向文件夹"),
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                Application.Current?.MainWindow,
                LF("LateralFileSystemTree.Delete.ConfirmFormat", "确定删除侧向文件夹“{0}”吗？", SelectedNode.Name),
                L("LateralFileSystemTree.DeleteFolder", "删除侧向文件夹"),
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning,
                MessageBoxResult.No);

            if (result != MessageBoxResult.Yes)
            {
                return;
            }

            var deletedName = SelectedNode.Name;

            try
            {
                _runtime.DeleteVirtualRoot(SelectedNode.Id);
                RefreshFromBackend(preserveSelection: false);
                StatusMessage = LF("LateralFileSystemTree.Status.DeletedFormat", "已删除侧向文件夹“{0}”。", deletedName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(Application.Current?.MainWindow, ex.Message, L("LateralFileSystemTree.DeleteFolder", "删除侧向文件夹"), MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private async Task ExecuteMergeFolderAsync()
        {
            if (SelectedNode is null)
            {
                return;
            }

            if (!EnsureBackendReadyForMutation())
            {
                return;
            }

            var selectedNodeId = SelectedNode.Id;
            var selectedNodeName = SelectedNode.Name;
            var sourcePath = ResolveNodeSourcePathForDisplay(SelectedNode);

            var result = MessageBox.Show(
                Application.Current?.MainWindow,
                LF("LateralFileSystemTree.Merge.ConfirmFormat", "确定将侧向文件夹“{0}”合并回源文件夹吗？\n\n源文件夹：{1}\n\n这会用当前侧向文件夹视图完整替换源文件夹内容；源文件夹中不存在于侧向文件夹的内容也会被删除。成功后会移除此 LateralFS 节点和投影文件夹。", selectedNodeName, sourcePath),
                L("LateralFileSystemTree.MergeFolder", "合并回源文件夹"),
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning,
                MessageBoxResult.No);

            if (result != MessageBoxResult.Yes)
            {
                return;
            }

            try
            {
                StatusMessage = LF("LateralFileSystemTree.Status.MergingFormat", "正在将侧向文件夹“{0}”合并回源文件夹…", selectedNodeName);
                var mergeResult = await Task.Run(() => _runtime.MergeVirtualRoot(selectedNodeId)).ConfigureAwait(true);
                RefreshFromBackend(preserveSelection: false);
                StatusMessage = LF("LateralFileSystemTree.Status.MergedFormat", "已将“{0}”合并回源文件夹，并移除对应 LateralFS 节点。快照包含 {1} 个文件、{2} 个文件夹。", mergeResult.NodeName, mergeResult.SnapshotFileCount, mergeResult.SnapshotDirectoryCount);
            }
            catch (Exception ex)
            {
                MessageBox.Show(Application.Current?.MainWindow, ex.Message, L("LateralFileSystemTree.MergeFolder", "合并回源文件夹"), MessageBoxButton.OK, MessageBoxImage.Warning);
                StatusMessage = LF("LateralFileSystemTree.Status.MergeFailedFormat", "合并侧向文件夹“{0}”失败。", selectedNodeName);
            }
        }

        private void RefreshFromBackend(bool preserveSelection, string? preferredSelectedNodeId = null)
        {
            var stopwatch = Stopwatch.StartNew();
            LateralFileSystemDebugConsole.Write("TreeVM", $"RefreshFromBackend start; preserveSelection={preserveSelection}; preferredSelectedNodeId='{preferredSelectedNodeId ?? "<null>"}'.");
            var selectedNodeId = preferredSelectedNodeId ?? (preserveSelection ? SelectedNode?.Id : null);
            var configuration = _runtime.GetConfiguration();
            LateralFileSystemDebugConsole.Write("TreeVM", $"RefreshFromBackend configuration; IsEnabled={configuration.IsEnabled}; WorkingRootDirectory='{configuration.WorkingRootDirectory}'; selectedNodeId='{selectedNodeId ?? "<null>"}'.");

            CurrentWorkingRootDirectory = configuration.WorkingRootDirectory;
            IsBackendConfiguredEnabled = configuration.IsEnabled;
            IsVirtualizationBackendAvailable = _runtime.IsVirtualizationBackendAvailable;

            var loadedNodes = _runtime.GetNodes().ToList();
            LateralFileSystemDebugConsole.Write("TreeVM", $"RefreshFromBackend loadedNodes count={loadedNodes.Count}.");
            var defaultPositions = BuildDefaultPositions(loadedNodes);

            Nodes.Clear();
            foreach (var model in loadedNodes)
            {
                var nodeViewModel = new LateralFileSystemNodeViewModel(model);
                if (!TryGetSavedVisualPosition(model, out _, out _)
                    && defaultPositions.TryGetValue(model.Id, out var defaultPosition))
                {
                    nodeViewModel.X = defaultPosition.X;
                    nodeViewModel.Y = defaultPosition.Y;
                    TryPersistVisualPosition(nodeViewModel);
                }

                Nodes.Add(nodeViewModel);
            }

            UpdateLinks();
            OnPropertyChanged(nameof(SelectedNodeParentName));
            OnPropertyChanged(nameof(SelectionPlaceholderText));

            SelectedNode = selectedNodeId is null
                ? null
                : Nodes.FirstOrDefault(node => string.Equals(node.Id, selectedNodeId, StringComparison.OrdinalIgnoreCase));

            if (!HasWorkingRootDirectory)
            {
                StatusMessage = L("LateralFileSystemTree.Selection.WorkingRootMissing", "请先在首选项中的“侧向文件系统”页设置工作根目录。");
            }
            else if (!IsVirtualizationBackendAvailable)
            {
                StatusMessage = _runtime.VirtualizationBackendStatusMessage;
            }
            else if (!IsBackendConfiguredEnabled)
            {
                StatusMessage = LF("LateralFileSystemTree.Status.LoadedButDisabledFormat", "已读取 {0} 个侧向文件夹，但后端当前未启用。", Nodes.Count);
            }
            else
            {
                StatusMessage = Nodes.Count == 0
                    ? L("LateralFileSystemTree.Status.ConnectedNoNodes", "已连接到 LateralFS 后端，但尚未创建侧向文件夹。")
                    : LF("LateralFileSystemTree.Status.LoadedFormat", "已从后端加载 {0} 个侧向文件夹。", Nodes.Count);
            }
            LateralFileSystemDebugConsole.Write("TreeVM", $"RefreshFromBackend end; nodes={Nodes.Count}; selectedNode='{SelectedNode?.Name ?? "<null>"}'; status='{StatusMessage}'; elapsedMs={stopwatch.ElapsedMilliseconds}.");
        }

        private async Task RefreshSelectedNodeInspectorAsync(bool syncSelectedNode, bool refreshStorageSummary)
        {
            var selectedNodeId = SelectedNode?.Id;
            var refreshVersion = ++_selectedNodeRefreshVersion;
            var stopwatch = Stopwatch.StartNew();
            LateralFileSystemDebugConsole.Write("TreeVM", $"RefreshSelectedNodeInspectorAsync start; syncSelectedNode={syncSelectedNode}; refreshStorageSummary={refreshStorageSummary}; selectedNodeId='{selectedNodeId ?? "<null>"}'; refreshVersion={refreshVersion}.");

            SelectedNodeFileTreeRoots.Clear();

            if (selectedNodeId is null)
            {
                SelectedNodeHydratedSizeText = L("LateralFileSystemTree.Selection.None", "未选择侧向文件夹");
                SelectedNodeFileSummaryText = string.Empty;
                SelectedNodeStorageNote = string.Empty;
                ResetSelectedNodeInspectorState();
                IsLoadingSelectedNodeInspector = false;
                LateralFileSystemDebugConsole.Write("TreeVM", $"RefreshSelectedNodeInspectorAsync end early because no node is selected; refreshVersion={refreshVersion}.");
                return;
            }

            IsLoadingSelectedNodeInspector = refreshStorageSummary;
            SelectedNodeHydratedSizeText = L("LateralFileSystemTree.Inspector.CountingSize", "正在统计真实体积…");
            SelectedNodeFileSummaryText = L("LateralFileSystemTree.Inspector.LoadingFileTree", "正在加载文件树…");
            SelectedNodeStorageNote = string.Empty;
            SelectedNodeInspectorStatusText = BuildInspectorLoadingStatusText(syncSelectedNode, refreshStorageSummary);
            PrepareSelectedNodeInspectorForRefresh(refreshStorageSummary);

            try
            {
                var summary = new LateralFileSystemNodeStorageSummary();
                Exception? storageSummaryException = null;

                if (syncSelectedNode)
                {
                    LateralFileSystemDebugConsole.Write("TreeVM", $"RefreshSelectedNodeInspectorAsync syncing node '{selectedNodeId}'.");
                    await Task.Run(() => _runtime.SyncNode(selectedNodeId)).ConfigureAwait(true);
                    if (refreshVersion != _selectedNodeRefreshVersion
                        || !string.Equals(SelectedNode?.Id, selectedNodeId, StringComparison.OrdinalIgnoreCase))
                    {
                        return;
                    }
                }

                if (refreshStorageSummary)
                {
                    try
                    {
                        LateralFileSystemDebugConsole.Write("TreeVM", $"RefreshSelectedNodeInspectorAsync requesting storage summary for node '{selectedNodeId}'.");
                        summary = await Task.Run(() => _runtime.GetNodeStorageSummary(selectedNodeId)).ConfigureAwait(true);
                        LateralFileSystemDebugConsole.Write("TreeVM", $"RefreshSelectedNodeInspectorAsync storage summary completed for node '{selectedNodeId}'.");
                    }
                    catch (Exception ex)
                    {
                        storageSummaryException = ex;
                        LateralFileSystemDebugConsole.WriteException("TreeVM", ex, $"RefreshSelectedNodeInspectorAsync storage summary failed; selectedNodeId='{selectedNodeId}'; refreshVersion={refreshVersion}");
                    }
                }

                LateralFileSystemDebugConsole.Write("TreeVM", $"RefreshSelectedNodeInspectorAsync requesting root entries for node '{selectedNodeId}'.");
                var rootEntries = await Task.Run(() => _runtime.GetNodeEntries(selectedNodeId, string.Empty)).ConfigureAwait(true);
                LateralFileSystemDebugConsole.Write("TreeVM", $"RefreshSelectedNodeInspectorAsync root entries completed for node '{selectedNodeId}'; count={rootEntries.Count}.");

                if (refreshVersion != _selectedNodeRefreshVersion
                    || !string.Equals(SelectedNode?.Id, selectedNodeId, StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }

                SelectedNodeHydratedSizeText = FormatBytes(summary.HydratedBytes);
                SelectedNodeFileSummaryText = LF("LateralFileSystemTree.Inspector.SummaryFormat", "已 Hydrate {0}/{1} 个文件，目录 {2} 个", summary.HydratedFileCount, summary.TotalFileCount, summary.TotalDirectoryCount);
                SelectedNodeStorageNote = BuildStorageNote(summary);

                if (refreshStorageSummary)
                {
                    if (storageSummaryException is null)
                    {
                        ApplySelectedNodeStorageSummary(summary);
                    }
                    else
                    {
                        ApplySelectedNodeStorageSummaryUnavailable(storageSummaryException);
                    }
                }
                else
                {
                    SetSelectedNodeStorageSummaryPending(rootEntries.Count);
                }

                foreach (var entry in rootEntries)
                {
                    SelectedNodeFileTreeRoots.Add(CreateFileTreeNode(selectedNodeId, entry));
                }

                LateralFileSystemDebugConsole.Write("TreeVM", $"RefreshSelectedNodeInspectorAsync populated inspector for node '{selectedNodeId}'; fileRoots={SelectedNodeFileTreeRoots.Count}; elapsedMs={stopwatch.ElapsedMilliseconds}.");
            }
            catch (Exception ex)
            {
                if (refreshVersion != _selectedNodeRefreshVersion)
                {
                    return;
                }

                SelectedNodeHydratedSizeText = L("Common.Unavailable", "不可用");
                SelectedNodeFileSummaryText = L("LateralFileSystemTree.Inspector.ReadFailed", "无法读取该侧向文件夹的文件视图。");
                SelectedNodeStorageNote = ex.Message;
                SelectedNodeInspectorStatusText = string.Empty;
                LateralFileSystemDebugConsole.WriteException("TreeVM", ex, $"RefreshSelectedNodeInspectorAsync failed; selectedNodeId='{selectedNodeId}'; refreshVersion={refreshVersion}");
            }
            finally
            {
                if (refreshVersion == _selectedNodeRefreshVersion)
                {
                    IsLoadingSelectedNodeInspector = false;
                    SelectedNodeInspectorStatusText = string.Empty;
                }

                LateralFileSystemDebugConsole.Write("TreeVM", $"RefreshSelectedNodeInspectorAsync end; selectedNodeId='{selectedNodeId ?? "<null>"}'; refreshVersion={refreshVersion}; elapsedMs={stopwatch.ElapsedMilliseconds}.");
            }
        }

        private void ResetSelectedNodeInspectorState()
        {
            SelectedNodeHydratedSizeText = L("LateralFileSystemTree.Selection.None", "未选择侧向文件夹");
            SelectedNodeFileSummaryText = string.Empty;
            SelectedNodeStorageNote = string.Empty;
            SelectedNodeInspectorStatusText = string.Empty;
        }

        private void PrepareSelectedNodeInspectorForRefresh(bool refreshStorageSummary)
        {
            if (refreshStorageSummary)
            {
                SelectedNodeHydratedSizeText = L("LateralFileSystemTree.Inspector.CountingSize", "正在统计真实体积…");
                SelectedNodeFileSummaryText = L("LateralFileSystemTree.Inspector.LoadingFileTree", "正在加载文件树…");
                SelectedNodeStorageNote = string.Empty;
                return;
            }

            SetSelectedNodeStorageSummaryPending();
            SelectedNodeFileSummaryText = L("LateralFileSystemTree.Inspector.LoadingFileTree", "正在加载文件树…");
        }

        private void ApplySelectedNodeStorageSummary(LateralFileSystemNodeStorageSummary summary)
        {
            SelectedNodeHydratedSizeText = FormatBytes(summary.HydratedBytes);
            SelectedNodeFileSummaryText = LF("LateralFileSystemTree.Inspector.SummaryFormat", "已 Hydrate {0}/{1} 个文件，目录 {2} 个", summary.HydratedFileCount, summary.TotalFileCount, summary.TotalDirectoryCount);
            SelectedNodeStorageNote = BuildStorageNote(summary);
        }

        private void ApplySelectedNodeStorageSummaryUnavailable(Exception? exception)
        {
            SelectedNodeHydratedSizeText = L("Common.Unavailable", "不可用");
            SelectedNodeFileSummaryText = L("LateralFileSystemTree.Inspector.SizeUnavailableButLoaded", "真实体积统计失败，但已加载文件视图。");
            SelectedNodeStorageNote = exception?.Message ?? L("LateralFileSystemTree.Inspector.SizeUnavailable", "无法统计该侧向文件夹的真实体积。");
        }

        private void SetSelectedNodeStorageSummaryPending(int? rootEntryCount = null)
        {
            SelectedNodeHydratedSizeText = L("LateralFileSystemTree.Inspector.NotCounted", "未统计");
            SelectedNodeFileSummaryText = rootEntryCount is null
                ? L("LateralFileSystemTree.Inspector.SizeNotAutoCounted", "真实体积不会自动统计。")
                : LF("LateralFileSystemTree.Inspector.RootEntriesLoadedFormat", "已加载根目录 {0} 个条目，真实体积未统计。", rootEntryCount.Value);
            SelectedNodeStorageNote = L("LateralFileSystemTree.Inspector.ManualRefreshHint", "点击“手动刷新并统计”后，才会更新 Hydrate 数量、目录数量和真实体积。");
        }

        private static string BuildInspectorLoadingStatusText(bool syncSelectedNode, bool refreshStorageSummary)
        {
            if (syncSelectedNode && refreshStorageSummary)
            {
                return L("LateralFileSystemTree.Inspector.SyncingCountingLoading", "正在同步、统计真实体积并载入文件树…");
            }

            if (syncSelectedNode)
            {
                return L("LateralFileSystemTree.Inspector.SyncingLoading", "正在同步并载入文件树…");
            }

            if (refreshStorageSummary)
            {
                return L("LateralFileSystemTree.LoadingFileTree", "正在统计真实体积并载入文件树…");
            }

            return L("LateralFileSystemTree.Inspector.LoadingFileTree", "正在加载文件树…");
        }

        private void SaveInitialNodePosition(LateralFileSystemNodeModel createdNode, LateralFileSystemNodeViewModel? parentNode)
        {
            double x;
            double y;

            if (parentNode != null)
            {
                x = parentNode.X + 260;
                y = parentNode.Y + 140;
            }
            else if (Nodes.Count == 0)
            {
                x = 60;
                y = 60;
            }
            else
            {
                x = 60;
                y = Nodes.Max(node => node.Y) + 160;
            }

            _runtime.SaveNodeVisualPosition(createdNode.Id, x, y);
        }

        private static bool TryGetSavedVisualPosition(LateralFileSystemNodeModel node, out double x, out double y)
        {
            x = 0;
            y = 0;

            var xProperty = node.Properties.FirstOrDefault(property => string.Equals(property.Key, "VisualX", StringComparison.OrdinalIgnoreCase));
            var yProperty = node.Properties.FirstOrDefault(property => string.Equals(property.Key, "VisualY", StringComparison.OrdinalIgnoreCase));

            if (xProperty is null || yProperty is null)
            {
                return false;
            }

            return double.TryParse(xProperty.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out x)
                && double.TryParse(yProperty.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out y);
        }

        private Dictionary<string, (double X, double Y)> BuildDefaultPositions(IReadOnlyList<LateralFileSystemNodeModel> nodes)
        {
            var positions = new Dictionary<string, (double X, double Y)>(StringComparer.OrdinalIgnoreCase);
            var validIds = nodes.Select(node => node.Id).ToHashSet(StringComparer.OrdinalIgnoreCase);
            var childrenByParentId = nodes
                .Where(node => !string.IsNullOrWhiteSpace(node.ParentNodeId) && validIds.Contains(node.ParentNodeId!))
                .GroupBy(node => node.ParentNodeId!, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(group => group.Key, group => group.OrderBy(node => node.Name, StringComparer.OrdinalIgnoreCase).ToList(), StringComparer.OrdinalIgnoreCase);

            var roots = nodes
                .Where(node => string.IsNullOrWhiteSpace(node.ParentNodeId) || !validIds.Contains(node.ParentNodeId!))
                .OrderBy(node => node.Name, StringComparer.OrdinalIgnoreCase)
                .ToList();

            var row = 0;
            var visited = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            void PlaceNode(LateralFileSystemNodeModel node, int depth)
            {
                if (!visited.Add(node.Id))
                {
                    return;
                }

                positions[node.Id] = (60 + (depth * 260), 60 + (row * 140));
                row++;

                if (childrenByParentId.TryGetValue(node.Id, out var children))
                {
                    foreach (var child in children)
                    {
                        PlaceNode(child, depth + 1);
                    }
                }
            }

            foreach (var root in roots)
            {
                PlaceNode(root, 0);
            }

            foreach (var node in nodes.Where(node => !visited.Contains(node.Id)).OrderBy(node => node.Name, StringComparer.OrdinalIgnoreCase))
            {
                PlaceNode(node, 0);
            }

            return positions;
        }

        private void TryPersistVisualPosition(LateralFileSystemNodeViewModel node)
        {
            try
            {
                PersistNodeVisualPosition(node);
            }
            catch
            {
            }
        }

        private bool EnsureBackendReadyForMutation()
        {
            if (!IsVirtualizationBackendAvailable)
            {
                MessageBox.Show(Application.Current?.MainWindow, _runtime.VirtualizationBackendStatusMessage, L("LateralFileSystemTree.Title", "侧向文件系统树"), MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!HasWorkingRootDirectory)
            {
                MessageBox.Show(Application.Current?.MainWindow, L("LateralFileSystemTree.Selection.WorkingRootMissing", "请先在首选项中的“侧向文件系统”页设置工作根目录。"), L("LateralFileSystemTree.Title", "侧向文件系统树"), MessageBoxButton.OK, MessageBoxImage.Information);
                return false;
            }

            if (!IsBackendConfiguredEnabled)
            {
                MessageBox.Show(Application.Current?.MainWindow, L("LateralFileSystemTree.Validation.EnableBackendFirst", "侧向文件系统当前未启用，请先在配置页面中启用后再执行创建、继承、合并或删除。"), L("LateralFileSystemTree.Title", "侧向文件系统树"), MessageBoxButton.OK, MessageBoxImage.Information);
                return false;
            }

            return true;
        }

        private string ResolveNodeSourcePathForDisplay(LateralFileSystemNodeViewModel node)
        {
            if (!string.IsNullOrWhiteSpace(node.ProjectionSourcePath))
            {
                return node.ProjectionSourcePath;
            }

            if (!string.IsNullOrWhiteSpace(node.ParentNodeId))
            {
                var parent = Nodes.FirstOrDefault(candidate => string.Equals(candidate.Id, node.ParentNodeId, StringComparison.OrdinalIgnoreCase));
                if (parent != null)
                {
                    return parent.VirtualRootPath;
                }
            }

            return L("Common.NotConfigured", "未设置");
        }

        private LateralFileSystemFileTreeNodeViewModel CreateFileTreeNode(string nodeId, LateralFileSystemFileEntryModel entry)
        {
            return new LateralFileSystemFileTreeNodeViewModel(
                entry,
                relativePath => _runtime.GetNodeEntries(nodeId, relativePath));
        }

        private static string BuildStorageNote(LateralFileSystemNodeStorageSummary summary)
        {
            var segments = new List<string>
            {
                LF("LateralFileSystemTree.StorageNote.PlaceholderFilesFormat", "占位符 {0} 个", summary.PlaceholderFileCount),
                LF("LateralFileSystemTree.StorageNote.HydratedPlaceholdersFormat", "已 Hydrate 的占位符 {0} 个", summary.HydratedPlaceholderFileCount),
                LF("LateralFileSystemTree.StorageNote.FullFilesFormat", "完整文件 {0} 个", summary.FullFileCount)
            };

            if (summary.UsedFallbackEstimation)
            {
                segments.Add(L("LateralFileSystemTree.StorageNote.Estimated", "部分状态为估算值"));
            }

            return string.Join("  ·  ", segments);
        }

        private static string FormatBytes(long value)
        {
            string[] units = ["B", "KB", "MB", "GB", "TB"];
            double size = Math.Max(0, value);
            var unitIndex = 0;

            while (size >= 1024 && unitIndex < units.Length - 1)
            {
                size /= 1024;
                unitIndex++;
            }

            return unitIndex == 0
                ? $"{size:0} {units[unitIndex]}"
                : $"{size:0.##} {units[unitIndex]}";
        }

        private void OnRuntimeConfigurationChanged(object? sender, EventArgs e)
        {
            LateralFileSystemDebugConsole.Write("TreeVM", "OnRuntimeConfigurationChanged received.");
            Application.Current?.Dispatcher.InvokeAsync(() => RefreshFromBackend(preserveSelection: true));
        }

        private void OnRuntimeTreeChanged(object? sender, EventArgs e)
        {
            LateralFileSystemDebugConsole.Write("TreeVM", "OnRuntimeTreeChanged received.");
            Application.Current?.Dispatcher.InvokeAsync(() => RefreshFromBackend(preserveSelection: true));
        }

        private void OnRuntimeSourceChanged(object? sender, LateralFileSystemSourceChangedEventArgs e)
        {
            LateralFileSystemDebugConsole.Write("TreeVM", $"OnRuntimeSourceChanged received; node='{e.Node.Name}' ({e.Node.Id}); changeType={e.ChangeType}; fullPath='{e.FullPath}'.");
            Application.Current?.Dispatcher.InvokeAsync(() =>
            {
                if (SelectedNode is null || !string.Equals(SelectedNode.Id, e.Node.Id, StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }

                StatusMessage = LF("LateralFileSystemTree.Status.SourceChangedFormat", "已检测到“{0}”的源目录变更。", SelectedNode.Name);
                _ = RefreshSelectedNodeInspectorAsync(syncSelectedNode: false, refreshStorageSummary: false);
            });
        }

        private void RefreshLocalizedText()
        {
            OnPropertyChanged(nameof(Title));
            OnPropertyChanged(nameof(Description));
            OnPropertyChanged(nameof(CurrentWorkingRootDirectoryDisplay));
            OnPropertyChanged(nameof(BackendAvailabilityText));
            OnPropertyChanged(nameof(SelectionPlaceholderText));
            OnPropertyChanged(nameof(SelectedNodeParentName));
            OnPropertyChanged(nameof(SelectedNodeKindText));
            OnPropertyChanged(nameof(SelectedNodeOwnerText));
            OnPropertyChanged(nameof(SelectedNodeParentText));
            OnPropertyChanged(nameof(SelectedNodeVirtualRootText));
            OnPropertyChanged(nameof(SelectedNodeProjectionSourceText));
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
