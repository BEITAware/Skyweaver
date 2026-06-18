using System.Collections.ObjectModel;
using Ferrita.Infrastructure.Mvvm;
using Ferrita.Models.LateralFileSystem;
using Ferrita.Services.Localization;

namespace Ferrita.Controls.LateralFileSystemTreeControl.ViewModels
{
    public sealed class LateralFileSystemFileTreeNodeViewModel : ObservableObject
    {
        private readonly Func<string, IReadOnlyList<LateralFileSystemFileEntryModel>> _childLoader;
        private readonly string _relativePath;
        private bool _isExpanded;
        private bool _isSelected;
        private bool _hasLoaded;
        private bool _isLoading;

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

        private static LateralFileSystemFileTreeNodeViewModel LoadingPlaceholder => new(L("Common.LoadingEllipsis", "加载中..."));

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
            if (!IsDirectory || _hasLoaded || _isLoading)
            {
                return;
            }

            _ = LoadChildrenAsync();
        }

        private async Task LoadChildrenAsync()
        {
            if (!IsDirectory || _hasLoaded || _isLoading)
            {
                return;
            }

            _isLoading = true;

            try
            {
                var entries = await Task.Run(() => _childLoader(_relativePath)).ConfigureAwait(true);
                Children.Clear();

                foreach (var entry in entries)
                {
                    Children.Add(new LateralFileSystemFileTreeNodeViewModel(entry, _childLoader));
                }

                _hasLoaded = true;
            }
            catch (Exception ex)
            {
                Children.Clear();
                Children.Add(new LateralFileSystemFileTreeNodeViewModel(LF("Common.LoadFailedFormat", "加载失败: {0}", ex.Message)));
                _hasLoaded = true;
            }
            finally
            {
                _isLoading = false;
            }
        }

        private static string BuildDetailText(LateralFileSystemFileEntryModel entry)
        {
            var stateText = FormatState(entry.OnDiskState, entry.IsDirectory);
            if (entry.IsDirectory)
            {
                return stateText;
            }

            return LF("LateralFileSystemTree.FileNode.DetailFormat", "逻辑 {0}  ·  实体 {1}  ·  {2}", FormatBytes(entry.LogicalSizeBytes), FormatBytes(entry.HydratedSizeBytes), stateText);
        }

        private static string FormatState(LateralFileSystemOnDiskState state, bool isDirectory)
        {
            if (state.HasFlag(LateralFileSystemOnDiskState.Full))
            {
                return isDirectory
                    ? L("LateralFileSystemTree.FileNode.State.FullDirectory", "完整目录")
                    : L("LateralFileSystemTree.FileNode.State.FullFile", "完整文件");
            }

            if (state.HasFlag(LateralFileSystemOnDiskState.HydratedPlaceholder))
            {
                return isDirectory
                    ? L("LateralFileSystemTree.FileNode.State.HydratedDirectoryPlaceholder", "已 Hydrate 的目录占位符")
                    : L("LateralFileSystemTree.FileNode.State.HydratedFilePlaceholder", "已 Hydrate 的文件占位符");
            }

            if (state.HasFlag(LateralFileSystemOnDiskState.Placeholder))
            {
                return isDirectory
                    ? L("LateralFileSystemTree.FileNode.State.DirectoryPlaceholder", "目录占位符")
                    : L("LateralFileSystemTree.FileNode.State.FilePlaceholder", "文件占位符");
            }

            if (state.HasFlag(LateralFileSystemOnDiskState.Tombstone))
            {
                return L("LateralFileSystemTree.FileNode.State.Tombstone", "Tombstone");
            }

            if (state.HasFlag(LateralFileSystemOnDiskState.Unknown))
            {
                return L("LateralFileSystemTree.FileNode.State.Unknown", "状态未知");
            }

            return isDirectory
                ? L("LateralFileSystemTree.FileNode.State.Directory", "目录")
                : L("LateralFileSystemTree.FileNode.State.File", "文件");
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
