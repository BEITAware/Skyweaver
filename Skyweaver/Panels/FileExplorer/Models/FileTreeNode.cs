using System.Collections.ObjectModel;
using System.IO;
using Skyweaver.Infrastructure.Mvvm;
using Skyweaver.Services.Localization;

namespace Skyweaver.Panels.FileExplorer.Models
{
    public class FileTreeNode : ObservableObject
    {
        private bool _isExpanded;
        private bool _isSelected;
        private bool _hasLoaded;

        public string Name { get; set; }

        public string Path { get; }

        public bool IsDirectory { get; }

        public ObservableCollection<FileTreeNode> Children { get; } = new();

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

        public FileTreeNode(string path, bool isDirectory)
        {
            Path = path;
            IsDirectory = isDirectory;
            Name = ResolveNodeName(path);

            if (IsDirectory)
            {
                Children.Add(new FileTreeNode(string.Empty, false) { Name = L("FileExplorer.Loading", "Loading...") });
            }
        }

        public void LoadChildren()
        {
            if (!IsDirectory || _hasLoaded)
            {
                return;
            }

            _hasLoaded = true;
            Children.Clear();

            try
            {
                foreach (var directory in Directory.GetDirectories(Path).OrderBy(static item => item))
                {
                    Children.Add(new FileTreeNode(directory, true));
                }

                foreach (var file in Directory.GetFiles(Path).OrderBy(static item => item))
                {
                    Children.Add(new FileTreeNode(file, false));
                }
            }
            catch (UnauthorizedAccessException)
            {
            }
            catch (IOException)
            {
            }
        }

        private static string ResolveNodeName(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return L("FileExplorer.Unknown", "Unknown");
            }

            var fileName = System.IO.Path.GetFileName(path);
            return string.IsNullOrEmpty(fileName) ? path : fileName;
        }

        private static string L(string resourceKey, string fallback)
        {
            return LocalizationRuntime.Instance.GetString(resourceKey, fallback);
        }
    }
}
