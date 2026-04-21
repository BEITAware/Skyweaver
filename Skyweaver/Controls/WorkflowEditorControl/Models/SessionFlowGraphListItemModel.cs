using System.Globalization;
using System.IO;
using Skyweaver.Infrastructure.Mvvm;

namespace Skyweaver.Controls.WorkflowEditorControl.Models
{
    public sealed class SessionFlowGraphListItemModel : ObservableObject
    {
        private string _graphId = string.Empty;
        private string _name = string.Empty;
        private string _filePath = string.Empty;
        private DateTime _createdAtUtc;
        private DateTime _updatedAtUtc;
        private int _nodeCount;
        private int _connectionCount;
        private bool _isCurrent;

        public string GraphId
        {
            get => _graphId;
            set => SetAndRefresh(ref _graphId, value?.Trim() ?? string.Empty);
        }

        public string Name
        {
            get => _name;
            set => SetAndRefresh(ref _name, value?.Trim() ?? string.Empty);
        }

        public string FilePath
        {
            get => _filePath;
            set => SetAndRefresh(ref _filePath, value?.Trim() ?? string.Empty);
        }

        public DateTime CreatedAtUtc
        {
            get => _createdAtUtc;
            set => SetAndRefresh(ref _createdAtUtc, value);
        }

        public DateTime UpdatedAtUtc
        {
            get => _updatedAtUtc;
            set => SetAndRefresh(ref _updatedAtUtc, value);
        }

        public int NodeCount
        {
            get => _nodeCount;
            set => SetAndRefresh(ref _nodeCount, Math.Max(0, value));
        }

        public int ConnectionCount
        {
            get => _connectionCount;
            set => SetAndRefresh(ref _connectionCount, Math.Max(0, value));
        }

        public bool IsCurrent
        {
            get => _isCurrent;
            set => SetProperty(ref _isCurrent, value);
        }

        public string FileName => string.IsNullOrWhiteSpace(FilePath)
            ? string.Empty
            : Path.GetFileName(FilePath);

        public string ModifiedText => UpdatedAtUtc == default
            ? "未记录修改时间"
            : $"修改于 {UpdatedAtUtc.ToLocalTime().ToString("yyyy-MM-dd HH:mm", CultureInfo.CurrentCulture)}";

        public string CreatedText => CreatedAtUtc == default
            ? "未记录创建时间"
            : $"创建于 {CreatedAtUtc.ToLocalTime().ToString("yyyy-MM-dd HH:mm", CultureInfo.CurrentCulture)}";

        public string GraphSummaryText => $"节点 {NodeCount} · 连线 {ConnectionCount}";

        public void ApplyDocument(SessionFlowGraphDocumentModel document)
        {
            ArgumentNullException.ThrowIfNull(document);

            GraphId = document.GraphId;
            Name = document.Name;
            FilePath = document.FilePath;
            CreatedAtUtc = document.CreatedAtUtc;
            UpdatedAtUtc = document.UpdatedAtUtc;
            NodeCount = document.Graph.Nodes.Count;
            ConnectionCount = document.Graph.Connections.Count;
        }

        private void SetAndRefresh<T>(ref T field, T value, string? propertyName = null)
        {
            if (!SetProperty(ref field, value, propertyName))
            {
                return;
            }

            RaiseComputedProperties();
        }

        private void RaiseComputedProperties()
        {
            OnPropertyChanged(nameof(FileName));
            OnPropertyChanged(nameof(ModifiedText));
            OnPropertyChanged(nameof(CreatedText));
            OnPropertyChanged(nameof(GraphSummaryText));
        }
    }
}
