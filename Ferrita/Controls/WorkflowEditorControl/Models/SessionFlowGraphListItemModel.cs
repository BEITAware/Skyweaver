using System.Globalization;
using System.IO;
using Ferrita.Infrastructure.Mvvm;
using Ferrita.Services.Localization;

namespace Ferrita.Controls.WorkflowEditorControl.Models
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
            ? L("WorkflowEditor.GraphItem.ModifiedTimeMissing", "未记录修改时间")
            : LF("WorkflowEditor.GraphItem.ModifiedTimeFormat", "修改于 {0}", FormatTimestamp(UpdatedAtUtc));

        public string CreatedText => CreatedAtUtc == default
            ? L("WorkflowEditor.GraphItem.CreatedTimeMissing", "未记录创建时间")
            : LF("WorkflowEditor.GraphItem.CreatedTimeFormat", "创建于 {0}", FormatTimestamp(CreatedAtUtc));

        public string GraphSummaryText => LF("WorkflowEditor.GraphItem.SummaryFormat", "节点 {0} · 连线 {1}", NodeCount, ConnectionCount);

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

        public void RefreshLocalizedText()
        {
            RaiseComputedProperties();
        }

        private static string L(string resourceKey, string fallback)
        {
            return LocalizationRuntime.Instance.GetString(resourceKey, fallback);
        }

        private static string FormatTimestamp(DateTime utcDateTime)
        {
            var format = L("WorkflowEditor.GraphItem.TimestampFormat", "yyyy-MM-dd HH:mm");
            return utcDateTime.ToLocalTime().ToString(format, CultureInfo.CurrentCulture);
        }

        private static string LF(string resourceKey, string fallback, params object[] args)
        {
            return string.Format(L(resourceKey, fallback), args);
        }
    }
}
