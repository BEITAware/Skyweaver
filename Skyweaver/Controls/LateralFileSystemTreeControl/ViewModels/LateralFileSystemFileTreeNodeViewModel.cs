using System.Collections.ObjectModel;
using Skyweaver.Infrastructure.Mvvm;
using Skyweaver.Models.LateralFileSystem;

namespace Skyweaver.Controls.LateralFileSystemTreeControl.ViewModels
{
    public sealed class LateralFileSystemFileTreeNodeViewModel : ObservableObject
    {
        private readonly Func<string, IReadOnlyList<LateralFileSystemFileEntryModel>> _childLoader;
        private readonly string _relativePath;
        private bool _isExpanded;
        private bool _isSelected;
        private bool _hasLoaded;

        public LateralFileSystemFileTreeNodeViewModel(
            LateralFileSystemFileEntryModel entry,
            Func<string, IReadOnlyList<LateralFileSystemFileEntryModel>> childLoader)
        {
            _childLoader = childLoader;
            _relativePath = entry.RelativePath;
            Name = entry.Name;
            IsDirectory = entry.IsDirectory;
            DetailText = BuildDetailText(entry);
            Children = new ObservableCollection<LateralFileSystemFileTreeNodeViewModel>();

            if (IsDirectory)
            {
                Children.Add(LoadingPlaceholder);
            }
        }

        public string Name { get; }

        public bool IsDirectory { get; }

        public string DetailText { get; }

        public ObservableCollection<LateralFileSystemFileTreeNodeViewModel> Children { get; }

        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                if (!SetProperty(ref _isExpanded, value) || !_isExpanded)
                {
                    return;
                }

                LoadChildren();
            }
        }

        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }

        private static LateralFileSystemFileTreeNodeViewModel LoadingPlaceholder => new("加载中...");

        private LateralFileSystemFileTreeNodeViewModel(string name)
        {
            _childLoader = _ => Array.Empty<LateralFileSystemFileEntryModel>();
            _relativePath = string.Empty;
            Name = name;
            IsDirectory = false;
            DetailText = string.Empty;
            Children = new ObservableCollection<LateralFileSystemFileTreeNodeViewModel>();
            _hasLoaded = true;
        }

        private void LoadChildren()
        {
            if (!IsDirectory || _hasLoaded)
            {
                return;
            }

            _hasLoaded = true;
            Children.Clear();

            foreach (var entry in _childLoader(_relativePath))
            {
                Children.Add(new LateralFileSystemFileTreeNodeViewModel(entry, _childLoader));
            }
        }

        private static string BuildDetailText(LateralFileSystemFileEntryModel entry)
        {
            var stateText = FormatState(entry.OnDiskState, entry.IsDirectory);
            if (entry.IsDirectory)
            {
                return stateText;
            }

            return $"逻辑 {FormatBytes(entry.LogicalSizeBytes)}  ·  实体 {FormatBytes(entry.HydratedSizeBytes)}  ·  {stateText}";
        }

        private static string FormatState(LateralFileSystemOnDiskState state, bool isDirectory)
        {
            if (state.HasFlag(LateralFileSystemOnDiskState.Full))
            {
                return isDirectory ? "完整目录" : "完整文件";
            }

            if (state.HasFlag(LateralFileSystemOnDiskState.HydratedPlaceholder))
            {
                return isDirectory ? "已 Hydrate 的目录占位符" : "已 Hydrate 的文件占位符";
            }

            if (state.HasFlag(LateralFileSystemOnDiskState.Placeholder))
            {
                return isDirectory ? "目录占位符" : "文件占位符";
            }

            if (state.HasFlag(LateralFileSystemOnDiskState.Tombstone))
            {
                return "Tombstone";
            }

            if (state.HasFlag(LateralFileSystemOnDiskState.Unknown))
            {
                return "状态未知";
            }

            return isDirectory ? "目录" : "文件";
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
    }
}
