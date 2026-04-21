using System.Collections.ObjectModel;
using System.Windows.Input;
using Skyweaver.Commands;
using Skyweaver.Infrastructure.Mvvm;
using Skyweaver.Panels.FileExplorer.Models;

namespace Skyweaver.Panels.FileExplorer.ViewModels
{
    public sealed class FileExplorerPanelViewModel : ObservableObject
    {
        private string _searchQuery = string.Empty;

        public ObservableCollection<FileTreeNode> FileTreeNodes { get; } = new();

        public string SearchQuery
        {
            get => _searchQuery;
            set => SetProperty(ref _searchQuery, value);
        }

        public ICommand RefreshCommand { get; }

        public FileExplorerPanelViewModel()
        {
            RefreshCommand = new RelayCommand(InitializeFileTree);
            InitializeFileTree();
        }

        private void InitializeFileTree()
        {
            FileTreeNodes.Clear();

            try
            {
                var userProfilePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                var rootNode = new FileTreeNode(userProfilePath, true)
                {
                    IsExpanded = true
                };

                FileTreeNodes.Add(rootNode);
            }
            catch (Exception)
            {
                var rootNode = new FileTreeNode("C:\\", true)
                {
                    IsExpanded = true
                };

                FileTreeNodes.Add(rootNode);
            }
        }
    }
}
