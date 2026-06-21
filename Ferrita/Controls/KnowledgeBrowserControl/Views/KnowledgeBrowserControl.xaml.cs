using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Ferrita.Controls.KnowledgeBrowserControl.ViewModels;

namespace Ferrita.Controls.KnowledgeBrowserControl.Views
{
    /// <summary>
    /// KnowledgeBrowserControl.xaml 的交互逻辑
    /// </summary>
    public partial class KnowledgeBrowserControl : UserControl
    {
        private KnowledgeBrowserControlViewModel ViewModel => (KnowledgeBrowserControlViewModel)DataContext;
        private BrowseTabItem? _lastSubscribedTab;

        public KnowledgeBrowserControl()
        {
            InitializeComponent();
            DataContext = new KnowledgeBrowserControlViewModel();

            Loaded += (_, _) =>
            {
                ViewModel.PropertyChanged += ViewModel_PropertyChanged;
                ViewModel.BrowseTabs.CollectionChanged += (s, e) => UpdateBrowseTabHighlight();
                UpdateBrowseTabHighlight();
            };
        }

        private void ViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(KnowledgeBrowserControlViewModel.SelectedBrowseTabIndex))
            {
                UpdateBrowseTabHighlight();
            }
            else if (e.PropertyName == nameof(KnowledgeBrowserControlViewModel.SelectedBrowseTab))
            {
                if (_lastSubscribedTab != null)
                {
                    _lastSubscribedTab.PropertyChanged -= ActiveTab_PropertyChanged;
                }

                _lastSubscribedTab = ViewModel.SelectedBrowseTab;

                if (_lastSubscribedTab != null)
                {
                    _lastSubscribedTab.PropertyChanged += ActiveTab_PropertyChanged;
                    ScrollActiveTabToPosition();
                }
            }
        }

        private void ActiveTab_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(BrowseTabItem.ScrollTrigger))
            {
                ScrollActiveTabToPosition();
            }
        }

        /// <summary>
        /// 双击 Wiki 列表项以打开该 Wiki
        /// </summary>
        private void WikiListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListBox listBox && listBox.SelectedItem is WikiItemViewModel wiki)
            {
                ViewModel.OpenWiki(wiki);
            }
        }

        /// <summary>
        /// 双击文件树节点以打开文档
        /// </summary>
        private void TreeView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is TreeView treeView && treeView.SelectedItem is FileSystemNodeViewModel node)
            {
                if (!node.IsDirectory)
                {
                    string ext = System.IO.Path.GetExtension(node.FullPath).ToLower();
                    if (ext == ".xml" || ext == ".md")
                    {
                        ViewModel.OpenDocumentTab(node.FullPath);
                        e.Handled = true;
                    }
                }
            }
        }

        /// <summary>
        /// 双击检索结果列表项定位并滚动到相应文档区域
        /// </summary>
        private void SearchResultListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListBox listBox && listBox.SelectedItem is SearchResultItemViewModel item)
            {
                ViewModel.OpenSearchResult(item);
                e.Handled = true;
            }
        }

        /// <summary>
        /// 在检索输入框按下 Enter 键触发搜索
        /// </summary>
        private void SearchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (ViewModel.SearchCommand.CanExecute(null))
                {
                    ViewModel.SearchCommand.Execute(null);
                    e.Handled = true;
                }
            }
        }

        /// <summary>
        /// 处理浏览标签页点击事件，切换选中标签页
        /// </summary>
        private void BrowseTab_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement fe && fe.DataContext is BrowseTabItem tab)
            {
                int index = ViewModel.BrowseTabs.IndexOf(tab);
                if (index >= 0)
                {
                    ViewModel.SelectedBrowseTabIndex = index;
                    UpdateBrowseTabHighlight();
                }
            }
        }

        /// <summary>
        /// 更新浏览标签页的高亮显示
        /// </summary>
        private void UpdateBrowseTabHighlight()
        {
            if (BrowseTabStrip == null) return;

            // 使用 Dispatcher 确保 UI 已更新
            Dispatcher.BeginInvoke(new Action(() =>
            {
                var itemsControl = BrowseTabStrip;
                for (int i = 0; i < itemsControl.Items.Count; i++)
                {
                    var container = itemsControl.ItemContainerGenerator.ContainerFromIndex(i) as ContentPresenter;
                    if (container == null) continue;

                    var border = FindChild<System.Windows.Controls.Border>(container, "TabBorder");
                    if (border != null)
                    {
                        border.Background = i == ViewModel.SelectedBrowseTabIndex
                            ? new SolidColorBrush(Color.FromArgb(0x25, 0xFF, 0xFF, 0xFF))
                            : Brushes.Transparent;
                    }
                }
            }), System.Windows.Threading.DispatcherPriority.Render);
        }

        /// <summary>
        /// 限制文本框选中以防止高亮和用户选定
        /// </summary>
        private void BrowseDocumentTextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox && textBox.SelectionLength > 0)
            {
                textBox.SelectionLength = 0;
            }
        }

        /// <summary>
        /// 将文档浏览器定位到选中搜索结果的偏移位置并滚动使其可见
        /// </summary>
        private void ScrollActiveTabToPosition()
        {
            if (ViewModel.SelectedBrowseTab == null || BrowseDocumentTextBox == null) return;

            var tab = ViewModel.SelectedBrowseTab;
            if (tab.ScrollToOffset >= 0)
            {
                BrowseDocumentTextBox.Dispatcher.BeginInvoke(new Action(() =>
                {
                    int start = Math.Min(tab.ScrollToOffset, BrowseDocumentTextBox.Text.Length);
                    int lineIndex = BrowseDocumentTextBox.GetLineIndexFromCharacterIndex(start);
                    if (lineIndex >= 0)
                    {
                        BrowseDocumentTextBox.ScrollToLine(lineIndex);
                    }
                }), System.Windows.Threading.DispatcherPriority.Background);
            }
        }

        /// <summary>
        /// 在可视化树中查找指定名称的子元素
        /// </summary>
        private static T? FindChild<T>(DependencyObject parent, string name) where T : FrameworkElement
        {
            int childCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T t && t.Name == name)
                    return t;

                var found = FindChild<T>(child, name);
                if (found != null)
                    return found;
            }
            return null;
        }
    }
}
