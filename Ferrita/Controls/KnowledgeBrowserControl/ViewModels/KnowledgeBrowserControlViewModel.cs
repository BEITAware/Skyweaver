using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Serialization;
using Ferrita.Commands;
using Ferrita.Infrastructure.Mvvm;
using Ferrita.Tools;
using OKXML;
using AerialCity.Retrieval;

namespace Ferrita.Controls.KnowledgeBrowserControl.ViewModels
{
    /// <summary>
    /// 知识浏览器控件的视图模型
    /// </summary>
    public sealed class KnowledgeBrowserControlViewModel : ObservableObject
    {
        private int _selectedSideTabIndex;
        private int _selectedBrowseTabIndex;
        private int _browseTabCounter = 0;
        private bool _isWikiOpened;
        private WikiItemViewModel? _selectedWiki;
        private WikiItemViewModel? _currentWiki;
        private string _searchQuery = string.Empty;
        private bool _isSearching;
        private bool _hasNoWikis;
        private BrowseTabItem? _selectedBrowseTab;

        public KnowledgeBrowserControlViewModel()
        {
            BrowseTabs = new ObservableCollection<BrowseTabItem>();
            FileTreeNodes = new ObservableCollection<FileSystemNodeViewModel>();
            Wikis = new ObservableCollection<WikiItemViewModel>();
            CosineResults = new ObservableCollection<SearchResultItemViewModel>();
            Bm25Results = new ObservableCollection<SearchResultItemViewModel>();

            AddBrowseTabCommand = new RelayCommand(AddBrowseTab);
            CloseBrowseTabCommand = new RelayCommand<BrowseTabItem>(CloseBrowseTab);
            SearchCommand = new RelayCommand(ExecuteSearch, () => !string.IsNullOrWhiteSpace(SearchQuery) && IsWikiOpened && !IsSearching);

            LoadWikis();
        }

        /// <summary>
        /// 侧面 Tab 选中索引 (0=打开..., 1=文件, 2=浏览, 3=检索)
        /// </summary>
        public int SelectedSideTabIndex
        {
            get => _selectedSideTabIndex;
            set => SetProperty(ref _selectedSideTabIndex, value);
        }

        /// <summary>
        /// 浏览页面内的标签页集合
        /// </summary>
        public ObservableCollection<BrowseTabItem> BrowseTabs { get; }

        /// <summary>
        /// 浏览页面内选中的标签页索引
        /// </summary>
        public int SelectedBrowseTabIndex
        {
            get => _selectedBrowseTabIndex;
            set
            {
                if (SetProperty(ref _selectedBrowseTabIndex, value))
                {
                    if (value >= 0 && value < BrowseTabs.Count)
                    {
                        SelectedBrowseTab = BrowseTabs[value];
                    }
                    else
                    {
                        SelectedBrowseTab = null;
                    }
                }
            }
        }

        /// <summary>
        /// 当前选中的浏览标签页
        /// </summary>
        public BrowseTabItem? SelectedBrowseTab
        {
            get => _selectedBrowseTab;
            set
            {
                if (SetProperty(ref _selectedBrowseTab, value))
                {
                    if (value != null)
                    {
                        int index = BrowseTabs.IndexOf(value);
                        if (index >= 0 && index != SelectedBrowseTabIndex)
                        {
                            SelectedBrowseTabIndex = index;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 是否已打开任何 Wiki
        /// </summary>
        public bool IsWikiOpened
        {
            get => _isWikiOpened;
            set => SetProperty(ref _isWikiOpened, value);
        }

        /// <summary>
        /// 在“打开...”列表中选中的 Wiki 项
        /// </summary>
        public WikiItemViewModel? SelectedWiki
        {
            get => _selectedWiki;
            set => SetProperty(ref _selectedWiki, value);
        }

        /// <summary>
        /// 当前已打开的 Wiki 实例
        /// </summary>
        public WikiItemViewModel? CurrentWiki
        {
            get => _currentWiki;
            set => SetProperty(ref _currentWiki, value);
        }

        /// <summary>
        /// 现有的 Wiki 知识库列表
        /// </summary>
        public ObservableCollection<WikiItemViewModel> Wikis { get; }

        /// <summary>
        /// 文件树根节点子项集合
        /// </summary>
        public ObservableCollection<FileSystemNodeViewModel> FileTreeNodes { get; }

        /// <summary>
        /// 检索框内的查询字符串
        /// </summary>
        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                if (SetProperty(ref _searchQuery, value))
                {
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        /// <summary>
        /// 标志当前是否正在执行检索
        /// </summary>
        public bool IsSearching
        {
            get => _isSearching;
            set => SetProperty(ref _isSearching, value);
        }

        /// <summary>
        /// 标志当前是否没有找到任何 Wiki
        /// </summary>
        public bool HasNoWikis
        {
            get => _hasNoWikis;
            set => SetProperty(ref _hasNoWikis, value);
        }

        /// <summary>
        /// 余弦相似度（向量）检索结果集合
        /// </summary>
        public ObservableCollection<SearchResultItemViewModel> CosineResults { get; }

        /// <summary>
        /// BM25 文本检索结果集合
        /// </summary>
        public ObservableCollection<SearchResultItemViewModel> Bm25Results { get; }

        public ICommand AddBrowseTabCommand { get; }
        public ICommand CloseBrowseTabCommand { get; }
        public ICommand SearchCommand { get; }

        /// <summary>
        /// 加载知识首选项目录下的所有 Wiki
        /// </summary>
        public void LoadWikis()
        {
            Wikis.Clear();
            string knowledgeDirPath = OKXMLWikiToolHelpers.GetKnowledgeDirectoryPath();
            if (Directory.Exists(knowledgeDirPath))
            {
                foreach (var dir in Directory.GetDirectories(knowledgeDirPath))
                {
                    string wikiName = Path.GetFileName(dir);
                    string xmlPath = Path.Combine(dir, $"{wikiName}.xml");
                    if (File.Exists(xmlPath))
                    {
                        try
                        {
                            var serializer = new XmlSerializer(typeof(OKWikiMetadata));
                            using var reader = new StreamReader(xmlPath);
                            if (serializer.Deserialize(reader) is OKWikiMetadata metadata)
                            {
                                Wikis.Add(new WikiItemViewModel
                                {
                                    WikiName = metadata.WikiName,
                                    Description = metadata.Description ?? string.Empty,
                                    Author = metadata.Author ?? string.Empty,
                                    CreatedAt = metadata.CreatedAt,
                                    WikiRootPath = dir
                                });
                            }
                        }
                        catch
                        {
                            // 发生异常时进行降级展示
                            Wikis.Add(new WikiItemViewModel
                            {
                                WikiName = wikiName,
                                Description = "元数据加载失败",
                                WikiRootPath = dir,
                                CreatedAt = Directory.GetCreationTime(dir)
                            });
                        }
                    }
                }
            }

            HasNoWikis = Wikis.Count == 0;
        }

        /// <summary>
        /// 打开选中的 Wiki 知识库
        /// </summary>
        public void OpenWiki(WikiItemViewModel wiki)
        {
            if (wiki == null) return;

            CurrentWiki = wiki;
            IsWikiOpened = true;

            // 清空现有的浏览 Tab 页并重置计数器
            BrowseTabs.Clear();
            _browseTabCounter = 0;

            // 添加一个默认的 Home 引导 Tab
            _browseTabCounter++;
            var homeTab = new BrowseTabItem
            {
                Title = "Home",
                Content = $"已打开 Wiki 知识库: {wiki.WikiName}\n\n" +
                          $"作者: {wiki.Author}\n" +
                          $"创建时间: {wiki.CreatedAt:yyyy-MM-dd HH:mm:ss}\n" +
                          $"描述: {wiki.Description}\n\n" +
                          $"使用指南:\n" +
                          $"1. 在左侧“文件”选项卡双击 XML 或 MD 格式文档可以打开并在此浏览。\n" +
                          $"2. 在“检索”选项卡并行进行语义和文本搜索，双击结果会自动定位到对应内容区域。",
                FilePath = string.Empty,
                Id = _browseTabCounter
            };
            BrowseTabs.Add(homeTab);
            SelectedBrowseTab = homeTab;

            // 重新构建文件树
            LoadFileTree();

            // 清空先前的检索内容与结果
            SearchQuery = string.Empty;
            CosineResults.Clear();
            Bm25Results.Clear();

            // 跳转到“文件”页面（索引 1）
            SelectedSideTabIndex = 1;
        }

        private void LoadFileTree()
        {
            FileTreeNodes.Clear();
            if (CurrentWiki == null || !Directory.Exists(CurrentWiki.WikiRootPath)) return;

            var tempRoot = BuildFileTree(CurrentWiki.WikiRootPath);
            foreach (var child in tempRoot.Children)
            {
                FileTreeNodes.Add(child);
            }
        }

        private FileSystemNodeViewModel BuildFileTree(string path)
        {
            var node = new FileSystemNodeViewModel
            {
                Name = Path.GetFileName(path),
                FullPath = path,
                IsDirectory = Directory.Exists(path)
            };

            if (node.IsDirectory)
            {
                try
                {
                    foreach (var dir in Directory.GetDirectories(path))
                    {
                        node.Children.Add(BuildFileTree(dir));
                    }
                    foreach (var file in Directory.GetFiles(path))
                    {
                        node.Children.Add(new FileSystemNodeViewModel
                        {
                            Name = Path.GetFileName(file),
                            FullPath = file,
                            IsDirectory = false
                        });
                    }
                }
                catch
                {
                    // 忽略无权限等无法访问的目录
                }
            }

            return node;
        }

        /// <summary>
        /// 双击打开文档并创建 Tab 页
        /// </summary>
        public void OpenDocumentTab(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath)) return;

            string fullPath = Path.GetFullPath(filePath);
            var existingTab = BrowseTabs.FirstOrDefault(t => !string.IsNullOrEmpty(t.FilePath) && Path.GetFullPath(t.FilePath) == fullPath);
            if (existingTab != null)
            {
                SelectedBrowseTab = existingTab;
            }
            else
            {
                try
                {
                    string content = File.ReadAllText(fullPath);
                    // 统一换行符为 \n，以便字符偏移量与检索结果段落的偏移匹配
                    content = content.Replace("\r\n", "\n");

                    _browseTabCounter++;
                    var newTab = new BrowseTabItem
                    {
                        Title = Path.GetFileName(fullPath),
                        FilePath = fullPath,
                        Content = content,
                        Id = _browseTabCounter
                    };
                    BrowseTabs.Add(newTab);
                    SelectedBrowseTab = newTab;
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show($"读取文件失败: {ex.Message}", "错误", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                }
            }

            SelectedSideTabIndex = 2; // 跳转到浏览页面
        }

        /// <summary>
        /// 双击检索结果并滚动到对应区域
        /// </summary>
        public void OpenSearchResult(SearchResultItemViewModel item)
        {
            if (CurrentWiki == null || string.IsNullOrEmpty(item.SourceUri)) return;

            string fullPath = Path.IsPathRooted(item.SourceUri)
                ? item.SourceUri
                : Path.GetFullPath(Path.Combine(CurrentWiki.WikiRootPath, item.SourceUri));

            if (!File.Exists(fullPath))
            {
                System.Windows.MessageBox.Show($"找不到对应的源文档文件：\n{fullPath}", "警告", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                return;
            }

            OpenDocumentTab(fullPath);

            if (SelectedBrowseTab != null)
            {
                int length = item.EndOffset - item.StartOffset;
                SelectedBrowseTab.TriggerScroll(item.StartOffset, length);
            }
        }

        private void AddBrowseTab()
        {
            _browseTabCounter++;
            var newTab = new BrowseTabItem
            {
                Title = $"新标签页 {_browseTabCounter}",
                Content = "空白内容。双击左侧“文件”中的文档进行浏览。",
                Id = _browseTabCounter
            };
            BrowseTabs.Add(newTab);
            SelectedBrowseTab = newTab;
        }

        private void CloseBrowseTab(BrowseTabItem? tab)
        {
            if (tab == null) return;

            int index = BrowseTabs.IndexOf(tab);
            BrowseTabs.Remove(tab);

            if (BrowseTabs.Count == 0)
            {
                SelectedBrowseTab = null;
                return;
            }

            // 调整选中索引
            if (index >= BrowseTabs.Count)
                SelectedBrowseTabIndex = BrowseTabs.Count - 1;
            else if (index <= SelectedBrowseTabIndex && SelectedBrowseTabIndex > 0)
                SelectedBrowseTabIndex = SelectedBrowseTabIndex - 1;
        }

        private void ExecuteSearch()
        {
            _ = ExecuteSearchAsync();
        }

        private async Task ExecuteSearchAsync()
        {
            if (string.IsNullOrWhiteSpace(SearchQuery) || CurrentWiki == null) return;

            IsSearching = true;
            CosineResults.Clear();
            Bm25Results.Clear();

            try
            {
                string wikiName = CurrentWiki.WikiName;
                string wikiRootPath = CurrentWiki.WikiRootPath;
                string query = SearchQuery.Trim();

                var configTuple = OKXMLWikiToolHelpers.GetEmbeddingConfig();
                ApiRetrievalRequest cosineRequest;
                ApiRetrievalRequest bm25Request;

                if (configTuple != null)
                {
                    var config = configTuple.Value.Config;
                    cosineRequest = new ApiRetrievalRequest
                    {
                        ApiKey = config.ApiKey,
                        BaseUrl = config.BaseUrl,
                        ApiType = config.ApiType,
                        Model = config.Model,
                        Dimensions = config.Dimensions,
                        Normalize = config.Normalize,
                        Parameters = config.Parameters ?? new(),
                        Method = RetrievalMethod.Cosine,
                        DatabasePath = Path.Combine(wikiRootPath, "Database"),
                        DatabaseName = wikiName,
                        TextQuery = query,
                        TopK = 10
                    };

                    bm25Request = new ApiRetrievalRequest
                    {
                        ApiKey = config.ApiKey,
                        BaseUrl = config.BaseUrl,
                        ApiType = config.ApiType,
                        Model = config.Model,
                        Dimensions = config.Dimensions,
                        Normalize = config.Normalize,
                        Parameters = config.Parameters ?? new(),
                        Method = RetrievalMethod.BM25,
                        DatabasePath = Path.Combine(wikiRootPath, "Database"),
                        DatabaseName = wikiName,
                        TextQuery = query,
                        TopK = 10
                    };
                }
                else
                {
                    cosineRequest = new ApiRetrievalRequest
                    {
                        Method = RetrievalMethod.Cosine,
                        DatabasePath = Path.Combine(wikiRootPath, "Database"),
                        DatabaseName = wikiName,
                        TextQuery = query,
                        TopK = 10
                    };

                    bm25Request = new ApiRetrievalRequest
                    {
                        Method = RetrievalMethod.BM25,
                        DatabasePath = Path.Combine(wikiRootPath, "Database"),
                        DatabaseName = wikiName,
                        TextQuery = query,
                        TopK = 10
                    };
                }

                var apiService = new ApiRetrievalService();

                var cosineTask = Task.Run(() => apiService.RetrieveAsync(cosineRequest));
                var bm25Task = Task.Run(() => apiService.RetrieveAsync(bm25Request));

                await Task.WhenAll(cosineTask, bm25Task);

                var rawCosine = await cosineTask;
                var rawBm25 = await bm25Task;

                var validCosine = rawCosine
                    .Where(r => !(r.Segment.Metadata.TryGetValue("Deprecated", out var dep) && dep?.ToString() == "true"))
                    .Select(r => new SearchResultItemViewModel(r))
                    .ToList();

                var validBm25 = rawBm25
                    .Where(r => !(r.Segment.Metadata.TryGetValue("Deprecated", out var dep) && dep?.ToString() == "true"))
                    .Select(r => new SearchResultItemViewModel(r))
                    .ToList();

                foreach (var res in validCosine) CosineResults.Add(res);
                foreach (var res in validBm25) Bm25Results.Add(res);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"搜索检索失败: {ex.Message}", "错误", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
            finally
            {
                IsSearching = false;
            }
        }
    }

    /// <summary>
    /// 表示一个 Wiki 项的 UI 数据
    /// </summary>
    public sealed class WikiItemViewModel : ObservableObject
    {
        public string WikiName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string WikiRootPath { get; set; } = string.Empty;
    }

    /// <summary>
    /// 文件系统节点的 UI 数据模型
    /// </summary>
    public sealed class FileSystemNodeViewModel : ObservableObject
    {
        public string Name { get; set; } = string.Empty;
        public string FullPath { get; set; } = string.Empty;
        public bool IsDirectory { get; set; }
        public ObservableCollection<FileSystemNodeViewModel> Children { get; } = new ObservableCollection<FileSystemNodeViewModel>();

        public string Icon => IsDirectory ? "📁" : "📄";
    }

    /// <summary>
    /// 检索结果项的 UI 数据模型
    /// </summary>
    public sealed class SearchResultItemViewModel : ObservableObject
    {
        private readonly RetrievalResult _raw;

        public SearchResultItemViewModel(RetrievalResult raw)
        {
            _raw = raw;
        }

        public string Content => _raw.Segment.Content;
        public float Score => _raw.Score;
        public string SourceUri => _raw.Segment.SourceUri ?? string.Empty;
        public int StartOffset => _raw.Segment.StartOffset;
        public int EndOffset => _raw.Segment.EndOffset;

        public string DisplayName => !string.IsNullOrEmpty(SourceUri) ? Path.GetFileName(SourceUri) : "未知文档";
    }

    /// <summary>
    /// 浏览标签页数据项
    /// </summary>
    public sealed class BrowseTabItem : ObservableObject
    {
        private string _title = string.Empty;
        private string _content = string.Empty;
        private string _filePath = string.Empty;
        private int _scrollToOffset = -1;
        private int _scrollToLength = 0;
        private int _scrollTrigger;

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public string Content
        {
            get => _content;
            set => SetProperty(ref _content, value);
        }

        public string FilePath
        {
            get => _filePath;
            set => SetProperty(ref _filePath, value);
        }

        public int Id { get; set; }

        public int ScrollToOffset
        {
            get => _scrollToOffset;
            set => SetProperty(ref _scrollToOffset, value);
        }

        public int ScrollToLength
        {
            get => _scrollToLength;
            set => SetProperty(ref _scrollToLength, value);
        }

        public int ScrollTrigger
        {
            get => _scrollTrigger;
            set => SetProperty(ref _scrollTrigger, value);
        }

        public void TriggerScroll(int offset, int length)
        {
            ScrollToOffset = offset;
            ScrollToLength = length;
            ScrollTrigger++;
        }
    }
}
