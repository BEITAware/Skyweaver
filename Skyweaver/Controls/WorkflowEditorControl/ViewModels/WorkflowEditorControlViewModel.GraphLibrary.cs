using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Skyweaver.Commands;
using Skyweaver.Controls.WorkflowEditorControl.Models;
using Skyweaver.Windows;

namespace Skyweaver.Controls.WorkflowEditorControl.ViewModels
{
    public sealed partial class WorkflowEditorControlViewModel
    {
        private const string DefaultNodeGraphBaseName = "会话流节点图";
        private readonly ObservableCollection<SessionFlowGraphListItemModel> _nodeGraphs = new();
        private SessionFlowGraphListItemModel? _selectedNodeGraph;
        private SessionFlowGraphListItemModel? _currentNodeGraph;

        public ObservableCollection<SessionFlowGraphListItemModel> NodeGraphs => _nodeGraphs;

        public SessionFlowGraphListItemModel? SelectedNodeGraph
        {
            get => _selectedNodeGraph;
            set
            {
                if (!SetProperty(ref _selectedNodeGraph, value))
                {
                    return;
                }

                OnPropertyChanged(nameof(HasSelectedNodeGraph));
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public bool HasSelectedNodeGraph => SelectedNodeGraph != null;

        public bool HasCurrentNodeGraph => _currentNodeGraph != null;

        public string CurrentNodeGraphName => _currentNodeGraph?.Name ?? "未打开节点图";

        public string NodeGraphLibrarySummaryText => _nodeGraphs.Count == 0
            ? "暂无节点图。"
            : $"已保存 {_nodeGraphs.Count} 个节点图，双击列表项即可在上方编辑器中打开。";

        public ICommand CreateNodeGraphCommand { get; private set; } = null!;

        public ICommand OpenSelectedNodeGraphCommand { get; private set; } = null!;

        public ICommand RenameSelectedNodeGraphCommand { get; private set; } = null!;

        public ICommand DeleteSelectedNodeGraphCommand { get; private set; } = null!;

        public ICommand RefreshNodeGraphLibraryCommand { get; private set; } = null!;

        private void InitializeGraphLibraryCommands()
        {
            CreateNodeGraphCommand = new RelayCommand(CreateNodeGraph);
            OpenSelectedNodeGraphCommand = new RelayCommand(OpenSelectedNodeGraph, () => SelectedNodeGraph != null);
            RenameSelectedNodeGraphCommand = new RelayCommand(RenameSelectedNodeGraph, () => SelectedNodeGraph != null);
            DeleteSelectedNodeGraphCommand = new RelayCommand(DeleteSelectedNodeGraph, () => SelectedNodeGraph != null);
            RefreshNodeGraphLibraryCommand = new RelayCommand(RefreshNodeGraphLibraryCommandExecute);
        }

        private void InitializeGraphLibrary()
        {
            RefreshNodeGraphLibrary(preferredSelectedFilePath: null, currentFilePath: null);

            if (_nodeGraphs.Count == 0)
            {
                var createdDocument = _sessionFlowRepository.Create(GetDefaultNodeGraphName());
                RefreshNodeGraphLibrary(createdDocument.FilePath, createdDocument.FilePath);

                var createdItem = _nodeGraphs.FirstOrDefault(item => PathsEqual(item.FilePath, createdDocument.FilePath));
                OpenNodeGraph(createdItem, persistAfterOpen: true, $"已创建并打开节点图“{createdDocument.Name}”。");
                return;
            }

            OpenNodeGraph(SelectedNodeGraph ?? _nodeGraphs.FirstOrDefault(), persistAfterOpen: false);
        }

        private void RefreshNodeGraphLibraryCommandExecute()
        {
            RefreshNodeGraphLibrary(SelectedNodeGraph?.FilePath, _currentNodeGraph?.FilePath);
            StatusMessage = "节点图库已刷新。";
        }

        private void RefreshNodeGraphLibrary(string? preferredSelectedFilePath, string? currentFilePath)
        {
            var documents = _sessionFlowRepository.LoadAll();

            _nodeGraphs.Clear();
            foreach (var document in documents)
            {
                var item = new SessionFlowGraphListItemModel();
                item.ApplyDocument(document);
                _nodeGraphs.Add(item);
            }

            SetCurrentNodeGraph(_nodeGraphs.FirstOrDefault(item => PathsEqual(item.FilePath, currentFilePath)));

            SelectedNodeGraph = _nodeGraphs.FirstOrDefault(item => PathsEqual(item.FilePath, preferredSelectedFilePath))
                ?? _currentNodeGraph
                ?? _nodeGraphs.FirstOrDefault();

            OnPropertyChanged(nameof(NodeGraphLibrarySummaryText));
        }

        private void OpenSelectedNodeGraph()
        {
            OpenNodeGraph(SelectedNodeGraph, persistAfterOpen: false);
        }

        private void OpenNodeGraph(SessionFlowGraphListItemModel? graphItem, bool persistAfterOpen, string? successMessage = null)
        {
            if (graphItem == null)
            {
                return;
            }

            try
            {
                var document = _sessionFlowRepository.Load(graphItem.FilePath);
                graphItem.ApplyDocument(document);

                SelectedNodeGraph = graphItem;
                SetCurrentNodeGraph(graphItem);
                ApplyGraph(document.Graph);

                if (persistAfterOpen)
                {
                    PersistGraph(successMessage ?? $"已创建并保存节点图“{document.Name}”。");
                }
                else
                {
                    StatusMessage = successMessage ?? $"已打开节点图“{document.Name}”。";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    Application.Current?.MainWindow,
                    ex.Message,
                    "打开节点图",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
        }

        private void CreateNodeGraph()
        {
            var owner = Application.Current?.MainWindow;
            var dialog = new NameInputDialog(
                "新建节点图",
                "输入新节点图名称。",
                GetDefaultNodeGraphName(),
                "创建",
                "请输入节点图名称。");

            if (owner != null && owner != dialog)
            {
                dialog.Owner = owner;
            }

            if (dialog.ShowDialog() != true)
            {
                return;
            }

            try
            {
                var document = _sessionFlowRepository.Create(dialog.InputValue);
                RefreshNodeGraphLibrary(document.FilePath, document.FilePath);

                var createdItem = _nodeGraphs.FirstOrDefault(item => PathsEqual(item.FilePath, document.FilePath));
                OpenNodeGraph(createdItem, persistAfterOpen: true, $"已创建并打开节点图“{document.Name}”。");
            }
            catch (Exception ex)
            {
                MessageBox.Show(owner, ex.Message, "新建节点图", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void RenameSelectedNodeGraph()
        {
            if (SelectedNodeGraph == null)
            {
                return;
            }

            var targetGraph = SelectedNodeGraph;
            var owner = Application.Current?.MainWindow;
            var dialog = new NameInputDialog(
                "重命名节点图",
                "输入新的节点图名称。",
                targetGraph.Name,
                "重命名",
                "请输入节点图名称。");

            if (owner != null && owner != dialog)
            {
                dialog.Owner = owner;
            }

            if (dialog.ShowDialog() != true)
            {
                return;
            }

            try
            {
                var wasCurrent = PathsEqual(_currentNodeGraph?.FilePath, targetGraph.FilePath);
                var renamedDocument = _sessionFlowRepository.Rename(targetGraph.FilePath, dialog.InputValue);

                RefreshNodeGraphLibrary(
                    preferredSelectedFilePath: renamedDocument.FilePath,
                    currentFilePath: wasCurrent ? renamedDocument.FilePath : _currentNodeGraph?.FilePath);

                StatusMessage = wasCurrent
                    ? $"已重命名当前节点图为“{renamedDocument.Name}”。"
                    : $"已重命名节点图为“{renamedDocument.Name}”。";
            }
            catch (Exception ex)
            {
                MessageBox.Show(owner, ex.Message, "重命名节点图", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void DeleteSelectedNodeGraph()
        {
            if (SelectedNodeGraph == null)
            {
                return;
            }

            var graphToDelete = SelectedNodeGraph;
            var owner = Application.Current?.MainWindow;
            var result = MessageBox.Show(
                owner,
                $"确定删除节点图“{graphToDelete.Name}”吗？",
                "删除节点图",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
            {
                return;
            }

            try
            {
                var wasCurrent = PathsEqual(_currentNodeGraph?.FilePath, graphToDelete.FilePath);
                _sessionFlowRepository.Delete(graphToDelete.FilePath);

                if (wasCurrent)
                {
                    RefreshNodeGraphLibrary(preferredSelectedFilePath: null, currentFilePath: null);

                    if (_nodeGraphs.Count == 0)
                    {
                        var createdDocument = _sessionFlowRepository.Create(GetDefaultNodeGraphName());
                        RefreshNodeGraphLibrary(createdDocument.FilePath, createdDocument.FilePath);

                        var createdItem = _nodeGraphs.FirstOrDefault(item => PathsEqual(item.FilePath, createdDocument.FilePath));
                        OpenNodeGraph(createdItem, persistAfterOpen: true, $"已删除节点图，并创建新的空白节点图“{createdDocument.Name}”。");
                        return;
                    }

                    OpenNodeGraph(SelectedNodeGraph ?? _nodeGraphs.FirstOrDefault(), persistAfterOpen: false, $"已删除节点图“{graphToDelete.Name}”。");
                    return;
                }

                RefreshNodeGraphLibrary(SelectedNodeGraph?.FilePath, _currentNodeGraph?.FilePath);
                StatusMessage = $"已删除节点图“{graphToDelete.Name}”。";
            }
            catch (Exception ex)
            {
                MessageBox.Show(owner, ex.Message, "删除节点图", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void SetCurrentNodeGraph(SessionFlowGraphListItemModel? graphItem)
        {
            _currentNodeGraph = graphItem;

            foreach (var item in _nodeGraphs)
            {
                item.IsCurrent = graphItem != null && PathsEqual(item.FilePath, graphItem.FilePath);
            }

            OnPropertyChanged(nameof(HasCurrentNodeGraph));
            OnPropertyChanged(nameof(CurrentNodeGraphName));
            OnPropertyChanged(nameof(PersistenceFilePath));
        }

        private void UpdateCurrentNodeGraph(SessionFlowGraphDocumentModel document)
        {
            ArgumentNullException.ThrowIfNull(document);

            if (_currentNodeGraph == null)
            {
                var currentItem = _nodeGraphs.FirstOrDefault(item => PathsEqual(item.FilePath, document.FilePath));
                if (currentItem != null)
                {
                    currentItem.ApplyDocument(document);
                    SetCurrentNodeGraph(currentItem);
                }

                return;
            }

            _currentNodeGraph.ApplyDocument(document);
            SetCurrentNodeGraph(_currentNodeGraph);

            if (SelectedNodeGraph != null && PathsEqual(SelectedNodeGraph.FilePath, document.FilePath))
            {
                SelectedNodeGraph.ApplyDocument(document);
            }
        }

        private SessionFlowGraphDocumentModel CreateCurrentDocument(SessionFlowGraphModel graph)
        {
            if (_currentNodeGraph == null)
            {
                throw new InvalidOperationException("当前没有打开的节点图。");
            }

            return new SessionFlowGraphDocumentModel
            {
                GraphId = string.IsNullOrWhiteSpace(_currentNodeGraph.GraphId)
                    ? Guid.NewGuid().ToString("N")
                    : _currentNodeGraph.GraphId,
                Name = _currentNodeGraph.Name,
                FilePath = _currentNodeGraph.FilePath,
                CreatedAtUtc = _currentNodeGraph.CreatedAtUtc,
                UpdatedAtUtc = _currentNodeGraph.UpdatedAtUtc,
                Graph = graph
            };
        }

        private string GetDefaultNodeGraphName()
        {
            return _sessionFlowRepository.CreateUniqueGraphName(DefaultNodeGraphBaseName);
        }

        private static bool PathsEqual(string? left, string? right)
        {
            return string.Equals(left, right, StringComparison.OrdinalIgnoreCase);
        }
    }
}
