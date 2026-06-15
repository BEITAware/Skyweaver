using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Xml.Linq;
using Skyweaver.Commands;
using Skyweaver.Infrastructure.Mvvm;
using Skyweaver.Services.Directories;
using Skyweaver.Windows;
using Skyweaver.Services.StickyNotes;

namespace Skyweaver.PageControls.Tiles.ViewModels
{
    public sealed class TileItemViewModel : ObservableObject
    {
        private string _code = string.Empty;
        private string _name = string.Empty;
        private string _icon = string.Empty;
        private string _size = "1x1";
        private string? _customImageSource;
        private int _column;
        private int _row;
        private int _columnSpan = 1;
        private int _rowSpan = 1;
        private bool _isDragging;
        private int _groupIndex = -1;
        private bool _isLocked;
        private bool _isCompletedState;
        private bool _isManuallyTriggered;

        // 运行状态字段
        private bool _isRunning;
        private string _statusText = string.Empty;
        private string _currentNodeTitle = string.Empty;
        private string _currentAgentId = string.Empty;
        private string _modelId = string.Empty;
        private string _latestOutput = string.Empty;
        private DateTime _startedAtUtc;
        private DateTime _updatedAtUtc;
        private string _flowName = string.Empty;

        public TileItemViewModel()
        {
            Skyweaver.Services.Localization.LocalizationRuntime.Instance.LanguageChanged += OnLanguageChanged;
            StickyNotesService.ReplyAdded += OnReplyAdded;

            RunCommand = new RelayCommand(() =>
            {
                RequestRun?.Invoke(this, EventArgs.Empty);
            });

            RemoveCommand = new RelayCommand(() =>
            {
                RequestRemove?.Invoke(this, EventArgs.Empty);
            });

            SetSizeCommand = new RelayCommand<string>(size =>
            {
                if (!string.IsNullOrWhiteSpace(size))
                {
                    Size = size;
                }
            });

            CustomImageCommand = new RelayCommand(() =>
            {
                var openFileDialog = new Microsoft.Win32.OpenFileDialog
                {
                    Filter = Skyweaver.Services.Localization.LocalizationRuntime.Instance.GetString("TilesPage.Dialog.ImageFilter", "Image files (*.png;*.jpg;*.jpeg;*.gif;*.bmp)|*.png;*.jpg;*.jpeg;*.gif;*.bmp"),
                    Title = Skyweaver.Services.Localization.LocalizationRuntime.Instance.GetString("TilesPage.Dialog.ChooseImageTitle", "Choose tile image")
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    CustomImageSource = openFileDialog.FileName;
                }
            });

            ToggleLockCommand = new RelayCommand(() =>
            {
                IsLocked = !IsLocked;
            });

            SetIconCommand = new RelayCommand<string>(iconPath =>
            {
                if (!string.IsNullOrWhiteSpace(iconPath))
                {
                    Icon = iconPath;
                }
            });

            ViewRepliesCommand = new RelayCommand(() =>
            {
                if (IsStickyNote && !string.IsNullOrEmpty(Code))
                {
                    StickyNotesService.MarkRepliesAsRead(Code);
                    HasUnreadReplies = false;

                    var owner = System.Windows.Application.Current?.MainWindow;
                    var dialog = new StickyNoteRepliesWindow(ThreeDaysReplies, DisplayName + " - 回复列表");
                    if (owner != null)
                    {
                        dialog.Owner = owner;
                        dialog.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
                    }
                    dialog.ShowDialog();
                }
            });
        }

        public string Code
        {
            get => _code;
            set
            {
                if (SetProperty(ref _code, value ?? string.Empty))
                {
                    if (IsStickyNote)
                    {
                        RefreshReplies();
                    }
                }
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                if (SetProperty(ref _name, value))
                {
                    OnPropertyChanged(nameof(DisplayName));
                    RequestLayoutUpdate?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public string DisplayName
        {
            get
            {
                if (Name == "Live Session")
                {
                    return Skyweaver.Services.Localization.LocalizationRuntime.Instance.GetString("TilesPage.Header.LiveSession", "Live Session");
                }
                return Name;
            }
        }

        private void OnLanguageChanged(object? sender, EventArgs e)
        {
            OnPropertyChanged(nameof(DisplayName));
        }

        public void Cleanup()
        {
            Skyweaver.Services.Localization.LocalizationRuntime.Instance.LanguageChanged -= OnLanguageChanged;
            StickyNotesService.ReplyAdded -= OnReplyAdded;
        }

        public string Icon
        {
            get => _icon;
            set
            {
                if (SetProperty(ref _icon, value))
                {
                    OnPropertyChanged(nameof(IsImageIcon));
                    RequestLayoutUpdate?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public string Size
        {
            get => _size;
            set
            {
                string normalizedSize = NormalizeSize(value);
                int newColSpan = 1, newRowSpan = 1;
                switch (normalizedSize)
                {
                    case "1x2":
                        newColSpan = 2;
                        newRowSpan = 1;
                        break;
                    case "2x2":
                        newColSpan = 2;
                        newRowSpan = 2;
                        break;
                }

                if (Column + newColSpan > TilesPageViewModel.GroupColumns ||
                    Row + newRowSpan > TilesPageViewModel.GroupRows)
                {
                    return;
                }

                if (SetProperty(ref _size, normalizedSize))
                {
                    ApplySize();
                    RequestLayoutUpdate?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public string? CustomImageSource
        {
            get => _customImageSource;
            set
            {
                if (SetProperty(ref _customImageSource, value))
                {
                    OnPropertyChanged(nameof(HasCustomImage));
                    RequestLayoutUpdate?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public int Column
        {
            get => _column;
            set => SetProperty(ref _column, value);
        }

        public int Row
        {
            get => _row;
            set => SetProperty(ref _row, value);
        }

        public int ColumnSpan
        {
            get => _columnSpan;
            private set => SetProperty(ref _columnSpan, value);
        }

        public int RowSpan
        {
            get => _rowSpan;
            private set => SetProperty(ref _rowSpan, value);
        }

        public bool IsDragging
        {
            get => _isDragging;
            set => SetProperty(ref _isDragging, value);
        }

        public int GroupIndex
        {
            get => _groupIndex;
            set => SetProperty(ref _groupIndex, value);
        }

        public bool IsLocked
        {
            get => _isLocked;
            set
            {
                if (SetProperty(ref _isLocked, value))
                {
                    RequestLayoutUpdate?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public bool IsLarge => Size == "2x2";

        public bool IsNon2x2 => Size != "2x2";

        public bool HasCustomImage => !string.IsNullOrEmpty(CustomImageSource);

        public bool IsImageIcon
        {
            get
            {
                if (string.IsNullOrEmpty(Icon)) return false;
                return Icon.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                       Icon.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                       Icon.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                       Icon.StartsWith("pack://", StringComparison.OrdinalIgnoreCase) ||
                       Icon.Contains("/") || Icon.Contains("\\");
            }
        }

        public ICommand RunCommand { get; }

        public ICommand RemoveCommand { get; }

        public ICommand SetSizeCommand { get; }

        public ICommand CustomImageCommand { get; }

        public ICommand ToggleLockCommand { get; }

        public ICommand SetIconCommand { get; }

        public event EventHandler? RequestLayoutUpdate;

        public event EventHandler? RequestRemove;

        public event EventHandler? RequestRun;

        public bool IsRunning
        {
            get => _isRunning;
            set => SetProperty(ref _isRunning, value);
        }

        public bool IsCompletedState
        {
            get => _isCompletedState;
            set => SetProperty(ref _isCompletedState, value);
        }

        public bool IsManuallyTriggered
        {
            get => _isManuallyTriggered;
            set => SetProperty(ref _isManuallyTriggered, value);
        }

        public string StatusText
        {
            get => _statusText;
            set => SetProperty(ref _statusText, value ?? string.Empty);
        }

        public string CurrentNodeTitle
        {
            get => _currentNodeTitle;
            set
            {
                if (SetProperty(ref _currentNodeTitle, value ?? string.Empty))
                {
                    OnPropertyChanged(nameof(AgentLoopStatus));
                }
            }
        }

        public string CurrentAgentId
        {
            get => _currentAgentId;
            set
            {
                if (SetProperty(ref _currentAgentId, value ?? string.Empty))
                {
                    OnPropertyChanged(nameof(AgentLoopStatus));
                }
            }
        }

        public string ModelId
        {
            get => _modelId;
            set
            {
                if (SetProperty(ref _modelId, value ?? string.Empty))
                {
                    OnPropertyChanged(nameof(AgentLoopStatus));
                }
            }
        }

        public string LatestOutput
        {
            get => _latestOutput;
            set => SetProperty(ref _latestOutput, value ?? string.Empty);
        }

        public DateTime StartedAtUtc
        {
            get => _startedAtUtc;
            set
            {
                if (SetProperty(ref _startedAtUtc, value))
                {
                    OnPropertyChanged(nameof(RunningDurationText));
                }
            }
        }

        public DateTime UpdatedAtUtc
        {
            get => _updatedAtUtc;
            set => SetProperty(ref _updatedAtUtc, value);
        }

        public string FlowName
        {
            get => _flowName;
            set => SetProperty(ref _flowName, value ?? string.Empty);
        }

        public string RunningDurationText
        {
            get
            {
                if (StartedAtUtc == DateTime.MinValue) return "00:00";
                var duration = DateTime.UtcNow - StartedAtUtc;
                if (duration < TimeSpan.Zero) duration = TimeSpan.Zero;
                return duration.TotalHours >= 1
                    ? $"{(int)duration.TotalHours}:{duration.Minutes:00}:{duration.Seconds:00}"
                    : $"{duration.Minutes:00}:{duration.Seconds:00}";
            }
        }

        public string AgentLoopStatus
        {
            get
            {
                var parts = new List<string>();
                if (!string.IsNullOrWhiteSpace(CurrentNodeTitle)) parts.Add($"节点: {CurrentNodeTitle}");
                if (!string.IsNullOrWhiteSpace(CurrentAgentId)) parts.Add($"代理: {CurrentAgentId}");
                if (!string.IsNullOrWhiteSpace(ModelId)) parts.Add($"模型: {ModelId}");
                return parts.Count > 0 ? string.Join(" | ", parts) : "初始化中...";
            }
        }

        public void RefreshDuration()
        {
            OnPropertyChanged(nameof(RunningDurationText));
        }

        private static string NormalizeSize(string? size)
        {
            return size is "1x2" or "2x2" ? size : "1x1";
        }

        private void ApplySize()
        {
            switch (Size)
            {
                case "1x2":
                    ColumnSpan = 2;
                    RowSpan = 1;
                    break;
                case "2x2":
                    ColumnSpan = 2;
                    RowSpan = 2;
                    break;
                default:
                    ColumnSpan = 1;
                    RowSpan = 1;
                    break;
            }

            OnPropertyChanged(nameof(IsLarge));
            OnPropertyChanged(nameof(IsNon2x2));
            OnPropertyChanged(nameof(HasCustomImage));
        }

        private bool _isStickyNote;
        private string _stickyNoteText = string.Empty;
        private string _stickyNoteMetadata = "未完成";

        public bool IsStickyNote
        {
            get => _isStickyNote;
            set
            {
                if (SetProperty(ref _isStickyNote, value))
                {
                    if (value && !string.IsNullOrEmpty(Code))
                    {
                        RefreshReplies();
                    }
                    RequestLayoutUpdate?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public string StickyNoteText
        {
            get => _stickyNoteText;
            set
            {
                if (SetProperty(ref _stickyNoteText, value ?? string.Empty))
                {
                    RequestLayoutUpdate?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public string StickyNoteMetadata
        {
            get => _stickyNoteMetadata;
            set
            {
                if (SetProperty(ref _stickyNoteMetadata, value ?? "未完成"))
                {
                    RequestLayoutUpdate?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        private string _stickyNoteColor = "Beige";
        public string StickyNoteColor
        {
            get => _stickyNoteColor;
            set
            {
                if (SetProperty(ref _stickyNoteColor, value ?? "Beige"))
                {
                    RequestLayoutUpdate?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        private bool _hasUnreadReplies;
        public bool HasUnreadReplies
        {
            get => _hasUnreadReplies;
            set => SetProperty(ref _hasUnreadReplies, value);
        }

        public ObservableCollection<StickyNoteReplyViewModel> ThreeDaysReplies { get; } = new ObservableCollection<StickyNoteReplyViewModel>();

        public ICommand ViewRepliesCommand { get; }

        private void OnReplyAdded(string tileCode)
        {
            if (string.Equals(Code, tileCode, StringComparison.OrdinalIgnoreCase))
            {
                RefreshReplies();
            }
        }

        public void RefreshReplies()
        {
            if (!IsStickyNote || string.IsNullOrEmpty(Code)) return;

            var replies = StickyNotesService.GetReplies(Code);
            var threeDaysAgo = DateTime.Now.AddDays(-3);
            
            var threeDaysRepliesList = replies
                .Where(r => r.DateTime >= threeDaysAgo)
                .OrderByDescending(r => r.DateTime)
                .Select(r => new StickyNoteReplyViewModel
                {
                    Creator = r.Creator,
                    DateTime = r.DateTime,
                    Content = r.Content
                })
                .ToList();

            System.Windows.Application.Current?.Dispatcher?.Invoke(() =>
            {
                ThreeDaysReplies.Clear();
                foreach (var reply in threeDaysRepliesList)
                {
                    ThreeDaysReplies.Add(reply);
                }
                HasUnreadReplies = replies.Any(r => !r.IsRead && r.DateTime >= threeDaysAgo);
            });
        }
    }

    public sealed class TileGroupViewModel : ObservableObject
    {
        private string _name = string.Empty;
        private int _index;
        private int _dropColumn;
        private int _dropRow;
        private int _dropColumnSpan = 1;
        private int _dropRowSpan = 1;
        private bool _isDropPreviewVisible;

        public TileGroupViewModel()
        {
            Skyweaver.Services.Localization.LocalizationRuntime.Instance.LanguageChanged += OnLanguageChanged;
        }

        private void OnLanguageChanged(object? sender, EventArgs e)
        {
            OnPropertyChanged(nameof(DisplayName));
        }

        public void Cleanup()
        {
            Skyweaver.Services.Localization.LocalizationRuntime.Instance.LanguageChanged -= OnLanguageChanged;
        }

        public int Index
        {
            get => _index;
            set => SetProperty(ref _index, value);
        }

        public string Name
        {
            get => _name;
            set
            {
                if (SetProperty(ref _name, string.IsNullOrWhiteSpace(value) ? DefaultName(Index) : value.Trim()))
                {
                    OnPropertyChanged(nameof(DisplayName));
                }
            }
        }

        public string DisplayName
        {
            get
            {
                string trimName = Name.Trim();
                int defaultIndex = GetDefaultNameIndex(trimName);
                if (defaultIndex >= 0)
                {
                    return DefaultName(defaultIndex);
                }
                return Name;
            }
            set
            {
                Name = value;
            }
        }

        private static readonly string[][] s_defaultNamesByLang = new string[][]
        {
            new string[] { "日常", "生产力", "简报", "娱乐", "应用" },
            new string[] { "Daily", "Productivity", "Briefing", "Entertainment", "Applications" },
            new string[] { "日常", "生産性", "ブリーフィング", "エンターテインメント", "アプリケーション" }
        };

        private static int GetDefaultNameIndex(string name)
        {
            for (int langIndex = 0; langIndex < s_defaultNamesByLang.Length; langIndex++)
            {
                for (int i = 0; i < s_defaultNamesByLang[langIndex].Length; i++)
                {
                    if (string.Equals(s_defaultNamesByLang[langIndex][i], name, StringComparison.OrdinalIgnoreCase))
                    {
                        return i;
                    }
                }
            }
            if (name.StartsWith("分组 ") || name.StartsWith("Group ") || name.StartsWith("グループ "))
            {
                string numStr = name.Substring(name.IndexOf(' ') + 1);
                if (int.TryParse(numStr, out int num) && num > 0)
                {
                    return num - 1 + 5;
                }
            }
            return -1;
        }

        public int DropColumn
        {
            get => _dropColumn;
            private set => SetProperty(ref _dropColumn, value);
        }

        public int DropRow
        {
            get => _dropRow;
            private set => SetProperty(ref _dropRow, value);
        }

        public int DropColumnSpan
        {
            get => _dropColumnSpan;
            private set => SetProperty(ref _dropColumnSpan, value);
        }

        public int DropRowSpan
        {
            get => _dropRowSpan;
            private set => SetProperty(ref _dropRowSpan, value);
        }

        public bool IsDropPreviewVisible
        {
            get => _isDropPreviewVisible;
            private set => SetProperty(ref _isDropPreviewVisible, value);
        }

        public ObservableCollection<TileItemViewModel> Tiles { get; } = new ObservableCollection<TileItemViewModel>();

        public void ShowDropPreview(int column, int row, int columnSpan, int rowSpan)
        {
            DropColumn = Math.Clamp(column, 0, TilesPageViewModel.GroupColumns - columnSpan);
            DropRow = Math.Clamp(row, 0, TilesPageViewModel.GroupRows - rowSpan);
            DropColumnSpan = Math.Clamp(columnSpan, 1, TilesPageViewModel.GroupColumns);
            DropRowSpan = Math.Clamp(rowSpan, 1, TilesPageViewModel.GroupRows);
            IsDropPreviewVisible = true;
        }

        public void ClearDropPreview()
        {
            IsDropPreviewVisible = false;
        }

        internal static string DefaultName(int index)
        {
            string key = $"TilesPage.Group.DefaultName.{index}";
            string fallback;
            switch (index)
            {
                case 0: fallback = "日常"; break;
                case 1: fallback = "生产力"; break;
                case 2: fallback = "简报"; break;
                case 3: fallback = "娱乐"; break;
                case 4: fallback = "应用"; break;
                default:
                    string groupFormat = Skyweaver.Services.Localization.LocalizationRuntime.Instance.GetString("TilesPage.Group.Format", "分组 {0}");
                    return string.Format(groupFormat, index + 1);
            }
            return Skyweaver.Services.Localization.LocalizationRuntime.Instance.GetString(key, fallback);
        }
    }

    public sealed class TilesPageViewModel : ObservableObject
    {
        public const int GroupColumns = 6;
        public const int GroupRows = 4;
        public const int GroupCellCount = GroupColumns * GroupRows;

        private readonly List<string> _rememberedGroupNames = new();
        private bool _isPacking;
        private bool _isAnyTileDragging;
        private readonly System.Windows.Threading.DispatcherTimer _runningTimer;

        public event EventHandler? RequestNavigateToLiveSession;

        public TilesPageViewModel()
        {
            InitializeDefaultTiles();

            Skyweaver.Services.ChatSession.ActiveChatSessionExecutionRegistry.Instance.Changed += Registry_Changed;
            Skyweaver.Services.Daemon.ScheduledTasksDaemonService.Instance.ManualTaskCompleted += DaemonService_ManualTaskCompleted;

            _runningTimer = new System.Windows.Threading.DispatcherTimer(System.Windows.Threading.DispatcherPriority.Background)
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _runningTimer.Tick += (_, _) => RefreshRunningDurations();
            _runningTimer.Start();

            UpdateTilesRunningStatus();
        }

        public ObservableCollection<TileItemViewModel> Tiles { get; } = new ObservableCollection<TileItemViewModel>();

        public ObservableCollection<TileGroupViewModel> TileGroups { get; } = new ObservableCollection<TileGroupViewModel>();

        public event EventHandler<TileLayoutTransitionEventArgs>? TileLayoutChanging;

        public event EventHandler<TileLayoutTransitionEventArgs>? TileLayoutChanged;

        public bool IsAnyTileDragging
        {
            get => _isAnyTileDragging;
            set => SetProperty(ref _isAnyTileDragging, value);
        }

        public void MoveTileToCell(TileItemViewModel tile, int targetGroupIndex, int targetColumn, int targetRow)
        {
            if (!Tiles.Contains(tile))
            {
                return;
            }

            Tiles.Remove(tile);
            Tiles.Add(tile);

            if (targetGroupIndex < 0 || targetGroupIndex >= TileGroups.Count)
            {
                targetGroupIndex = TileGroups.Count;
            }

            tile.GroupIndex = targetGroupIndex;
            tile.Column = Math.Clamp(targetColumn, 0, GroupColumns - tile.ColumnSpan);
            tile.Row = Math.Clamp(targetRow, 0, GroupRows - tile.RowSpan);

            PackTiles();
        }

        public int GetTileGroupIndex(TileItemViewModel tile)
        {
            for (int i = 0; i < TileGroups.Count; i++)
            {
                if (TileGroups[i].Tiles.Contains(tile))
                {
                    return i;
                }
            }

            return -1;
        }

        public void ShowDropPreview(TileItemViewModel tile, int targetGroupIndex, int targetColumn, int targetRow)
        {
            ClearDropPreview();

            if (targetGroupIndex < 0 || targetGroupIndex >= TileGroups.Count)
            {
                return;
            }

            TileGroups[targetGroupIndex].ShowDropPreview(
                targetColumn,
                targetRow,
                tile.ColumnSpan,
                tile.RowSpan);
        }

        public void ClearDropPreview()
        {
            foreach (var group in TileGroups)
            {
                group.ClearDropPreview();
            }
        }

        public void PackTiles()
        {
            if (_isPacking)
            {
                return;
            }

            try
            {
                _isPacking = true;
                SaveCurrentGroupNames();

                if (Tiles.Count == 0)
                {
                    foreach (var group in TileGroups)
                    {
                        group.PropertyChanged -= OnGroupPropertyChanged;
                        group.Cleanup();
                    }
                    TileGroups.Clear();
                    return;
                }

                var occupied = new Dictionary<int, bool[,]>();
                var unresolved = new List<TileItemViewModel>();
                var resolved = new List<TileItemViewModel>();

                // 第一遍遍历：处理已锁定的磁贴，确保它们保持当前位置
                for (int i = 0; i < Tiles.Count; i++)
                {
                    var tile = Tiles[i];
                    if (tile.IsLocked && tile.GroupIndex >= 0)
                    {
                        if (!occupied.ContainsKey(tile.GroupIndex))
                            occupied[tile.GroupIndex] = new bool[GroupColumns, GroupRows];

                        if (IsFree(occupied[tile.GroupIndex], tile.Column, tile.Row, tile.ColumnSpan, tile.RowSpan))
                        {
                            MarkOccupied(occupied[tile.GroupIndex], tile.Column, tile.Row, tile.ColumnSpan, tile.RowSpan);
                            resolved.Add(tile);
                        }
                        else
                        {
                            // 如果锁定的磁贴重叠，这在正常情况下不应发生。
                            // 但如果发生了，为了避免崩溃，我们将其视为未解决状态。
                            // 理想情况下，它应该保持有效的原始位置。
                            unresolved.Add(tile);
                        }
                    }
                }

                // 第二遍遍历：处理未锁定的磁贴
                for (int i = Tiles.Count - 1; i >= 0; i--)
                {
                    var tile = Tiles[i];
                    if (tile.IsLocked && resolved.Contains(tile))
                    {
                        continue;
                    }

                    if (tile.GroupIndex >= 0 && !tile.IsLocked)
                    {
                        if (!occupied.ContainsKey(tile.GroupIndex))
                            occupied[tile.GroupIndex] = new bool[GroupColumns, GroupRows];

                        if (IsFree(occupied[tile.GroupIndex], tile.Column, tile.Row, tile.ColumnSpan, tile.RowSpan))
                        {
                            MarkOccupied(occupied[tile.GroupIndex], tile.Column, tile.Row, tile.ColumnSpan, tile.RowSpan);
                            resolved.Add(tile);
                            continue;
                        }
                    }
                    if (!tile.IsLocked)
                    {
                        unresolved.Add(tile);
                    }
                }

                unresolved.Reverse();

                foreach (var tile in unresolved)
                {
                    int gIndex = Math.Max(0, tile.GroupIndex);
                    while (true)
                    {
                        if (!occupied.ContainsKey(gIndex))
                            occupied[gIndex] = new bool[GroupColumns, GroupRows];

                        if (TryFindSlot(occupied[gIndex], tile.ColumnSpan, tile.RowSpan, out int col, out int row))
                        {
                            tile.GroupIndex = gIndex;
                            tile.Column = col;
                            tile.Row = row;
                            MarkOccupied(occupied[gIndex], col, row, tile.ColumnSpan, tile.RowSpan);
                            break;
                        }
                        gIndex++;
                    }
                }

                int requiredGroupCount = 0;
                foreach (var tile in Tiles)
                {
                    if (tile.GroupIndex >= requiredGroupCount)
                    {
                        requiredGroupCount = tile.GroupIndex + 1;
                    }
                }

                EnsureGroupCount(requiredGroupCount);
                
                var groupedTiles = new List<TileItemViewModel>[requiredGroupCount];
                for (int i = 0; i < groupedTiles.Length; i++)
                {
                    groupedTiles[i] = new List<TileItemViewModel>();
                }

                foreach (var tile in Tiles)
                {
                    if (tile.GroupIndex >= 0 && tile.GroupIndex < requiredGroupCount)
                    {
                        groupedTiles[tile.GroupIndex].Add(tile);
                    }
                }

                for (int i = 0; i < requiredGroupCount; i++)
                {
                    var group = TileGroups[i];
                    group.Index = i;
                    SyncCollection(group.Tiles, groupedTiles[i]);
                }
            }
            finally
            {
                _isPacking = false;
                SaveTiles();
            }
        }

        private void InitializeDefaultTiles()
        {
            LoadTiles();
        }

        private void AddTile(TileItemViewModel tile)
        {
            tile.RequestLayoutUpdate += OnTileRequestLayoutUpdate;
            tile.RequestRemove += OnTileRequestRemove;
            tile.RequestRun += OnTileRequestRun;
            Tiles.Add(tile);
        }

        private void OnTileRequestLayoutUpdate(object? sender, EventArgs e)
        {
            if (sender is TileItemViewModel tile)
            {
                TileLayoutChanging?.Invoke(this, new TileLayoutTransitionEventArgs(tile));
                
                if (Tiles.Contains(tile))
                {
                    Tiles.Remove(tile);
                    Tiles.Add(tile);
                }
                
                PackTiles();
                TileLayoutChanged?.Invoke(this, new TileLayoutTransitionEventArgs(tile));
                return;
            }

            PackTiles();
        }

        private void OnTileRequestRemove(object? sender, EventArgs e)
        {
            if (sender is not TileItemViewModel tile)
            {
                return;
            }

            tile.RequestLayoutUpdate -= OnTileRequestLayoutUpdate;
            tile.RequestRemove -= OnTileRequestRemove;
            tile.RequestRun -= OnTileRequestRun;
            tile.Cleanup();
            Tiles.Remove(tile);
            PackTiles();
        }

        private ICommand? _resetAllCompletedCommand;
        public ICommand ResetAllCompletedCommand => _resetAllCompletedCommand ??= new RelayCommand(() =>
        {
            foreach (var tile in Tiles)
            {
                tile.IsCompletedState = false;
                tile.IsManuallyTriggered = false;
                tile.StatusText = string.Empty;
                tile.CurrentNodeTitle = string.Empty;
                tile.CurrentAgentId = string.Empty;
                tile.ModelId = string.Empty;
                tile.LatestOutput = string.Empty;
                tile.FlowName = string.Empty;
            }
        });

        private void OnTileRequestRun(object? sender, EventArgs e)
        {
            if (sender is TileItemViewModel tile)
            {
                if (tile.Name == "Live Session")
                {
                    RequestNavigateToLiveSession?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    var matchedTiles = Tiles.Where(t => string.Equals(t.Name, tile.Name, StringComparison.OrdinalIgnoreCase)).ToList();
                    if (tile.IsCompletedState)
                    {
                        foreach (var t in matchedTiles)
                        {
                            t.IsCompletedState = false;
                            t.IsManuallyTriggered = false;
                            t.StatusText = string.Empty;
                            t.CurrentNodeTitle = string.Empty;
                            t.CurrentAgentId = string.Empty;
                            t.ModelId = string.Empty;
                            t.LatestOutput = string.Empty;
                            t.FlowName = string.Empty;
                        }
                    }
                    else
                    {
                        foreach (var t in matchedTiles)
                        {
                            t.IsManuallyTriggered = true;
                        }
                        RunScheduledTask(tile.Name);
                    }
                }
            }
        }

        private void DaemonService_ManualTaskCompleted(object? sender, string taskName)
        {
            var dispatcher = Application.Current?.Dispatcher;
            if (dispatcher == null) return;

            if (dispatcher.CheckAccess())
            {
                SetTilesCompletedState(taskName);
            }
            else
            {
                dispatcher.BeginInvoke(new Action(() => SetTilesCompletedState(taskName)), System.Windows.Threading.DispatcherPriority.Background);
            }
        }

        private void SetTilesCompletedState(string taskName)
        {
            var matchedTiles = Tiles.Where(t => string.Equals(t.Name, taskName, StringComparison.OrdinalIgnoreCase)).ToList();
            foreach (var tile in matchedTiles)
            {
                tile.IsCompletedState = true;
                tile.IsManuallyTriggered = false;
                tile.StatusText = "已完成";
            }
        }

        private void RunScheduledTask(string taskName)
        {
            try
            {
                var repository = new Skyweaver.Controls.ScheduledTasksControl.Services.ScheduledTasksRepository();
                var tasks = repository.LoadAll();
                var task = tasks.FirstOrDefault(t => t.Name == taskName);
                if (task != null)
                {
                    Skyweaver.Services.Daemon.ScheduledTasksDaemonService.Instance.RunTask(task);
                }
                else
                {
                    MessageBox.Show($"未找到计划任务「{taskName}」", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"启动计划任务失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Registry_Changed(object? sender, EventArgs e)
        {
            var dispatcher = Application.Current?.Dispatcher;
            if (dispatcher == null || dispatcher.CheckAccess())
            {
                UpdateTilesRunningStatus();
                return;
            }
            dispatcher.BeginInvoke(new Action(UpdateTilesRunningStatus), System.Windows.Threading.DispatcherPriority.Background);
        }

        private void UpdateTilesRunningStatus()
        {
            var snapshots = Skyweaver.Services.ChatSession.ActiveChatSessionExecutionRegistry.Instance.GetSnapshot();
            foreach (var tile in Tiles)
            {
                if (tile.Name == "Live Session")
                {
                    continue;
                }

                var snapshot = snapshots.FirstOrDefault(s =>
                    s.SessionTitle.StartsWith($"{tile.Name}_", StringComparison.OrdinalIgnoreCase) ||
                    s.SessionTitle.Equals(tile.Name, StringComparison.OrdinalIgnoreCase));

                if (snapshot != null)
                {
                    tile.IsRunning = true;
                    tile.StatusText = snapshot.StatusText;
                    tile.CurrentNodeTitle = snapshot.CurrentNodeTitle ?? string.Empty;
                    tile.CurrentAgentId = snapshot.CurrentAgentId ?? string.Empty;
                    tile.ModelId = snapshot.ModelId ?? string.Empty;
                    tile.LatestOutput = snapshot.LatestOutput;
                    tile.StartedAtUtc = snapshot.StartedAtUtc;
                    tile.UpdatedAtUtc = snapshot.UpdatedAtUtc;
                    tile.FlowName = snapshot.FlowName;
                }
                else
                {
                    if (tile.IsRunning && tile.IsManuallyTriggered)
                    {
                        tile.IsCompletedState = true;
                        tile.IsManuallyTriggered = false;
                        tile.StatusText = "已完成";
                    }
                    else if (!tile.IsCompletedState)
                    {
                        tile.StatusText = string.Empty;
                        tile.CurrentNodeTitle = string.Empty;
                        tile.CurrentAgentId = string.Empty;
                        tile.ModelId = string.Empty;
                        tile.LatestOutput = string.Empty;
                        tile.FlowName = string.Empty;
                    }
                    tile.IsRunning = false;
                    tile.StartedAtUtc = DateTime.MinValue;
                }
            }
        }

        private void RefreshRunningDurations()
        {
            foreach (var tile in Tiles)
            {
                if (tile.IsRunning)
                {
                    tile.RefreshDuration();
                }
            }
        }

        private string GetTilesXmlPath()
        {
            var tilesDir = Path.Combine(SkyweaverDirectoryRuntime.Instance.ConfigurationDirectoryPath, "Tiles");
            return Path.Combine(tilesDir, "Tile.xml");
        }

        public void LoadTiles()
        {
            try
            {
                var filePath = GetTilesXmlPath();
                if (!File.Exists(filePath))
                {
                    foreach (var tile in Tiles)
                    {
                        tile.RequestLayoutUpdate -= OnTileRequestLayoutUpdate;
                        tile.RequestRemove -= OnTileRequestRemove;
                        tile.RequestRun -= OnTileRequestRun;
                        tile.Cleanup();
                    }
                    Tiles.Clear();
                    PackTiles();
                    return;
                }

                var doc = XDocument.Load(filePath);
                var root = doc.Root;
                if (root == null) return;

                foreach (var tile in Tiles)
                {
                    tile.RequestLayoutUpdate -= OnTileRequestLayoutUpdate;
                    tile.RequestRemove -= OnTileRequestRemove;
                    tile.RequestRun -= OnTileRequestRun;
                    tile.Cleanup();
                }
                Tiles.Clear();
                var loadedCodes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                foreach (var tileEl in root.Elements("Tile"))
                {
                    var name = (string?)tileEl.Attribute("Name") ?? string.Empty;
                    var code = (string?)tileEl.Attribute("Code");
                    var icon = (string?)tileEl.Attribute("Icon") ?? string.Empty;
                    var size = (string?)tileEl.Attribute("Size") ?? "1x1";
                    var col = int.Parse((string?)tileEl.Attribute("Column") ?? "0");
                    var row = int.Parse((string?)tileEl.Attribute("Row") ?? "0");
                    var groupIndex = int.Parse((string?)tileEl.Attribute("GroupIndex") ?? "-1");
                    var isLocked = bool.Parse((string?)tileEl.Attribute("IsLocked") ?? "false");
                    var customImageSource = (string?)tileEl.Attribute("CustomImageSource");
                    var isStickyNote = bool.Parse((string?)tileEl.Attribute("IsStickyNote") ?? "false");
                    var stickyNoteText = (string?)tileEl.Attribute("StickyNoteText") ?? string.Empty;
                    var stickyNoteMetadata = (string?)tileEl.Attribute("StickyNoteMetadata") ?? "未完成";
                    var stickyNoteColor = (string?)tileEl.Attribute("StickyNoteColor") ?? "Beige";

                    if (string.IsNullOrWhiteSpace(code) || loadedCodes.Contains(code))
                    {
                        code = GenerateUniqueTileCode(name, isStickyNote, loadedCodes);
                    }
                    loadedCodes.Add(code);

                    var tile = new TileItemViewModel
                    {
                        Code = code,
                        Name = name,
                        Icon = icon,
                        Size = size,
                        Column = col,
                        Row = row,
                        GroupIndex = groupIndex,
                        IsLocked = isLocked,
                        CustomImageSource = customImageSource,
                        IsStickyNote = isStickyNote,
                        StickyNoteText = stickyNoteText,
                        StickyNoteMetadata = stickyNoteMetadata,
                        StickyNoteColor = stickyNoteColor
                    };

                    AddTile(tile);
                }

                PackTiles();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading tiles: {ex.Message}");
            }
        }

        public void CreateStickyNoteFromAgent(string text, string size)
        {
            int targetGroupIndex = -1;
            for (int i = 0; i < TileGroups.Count; i++)
            {
                if (string.Equals(TileGroups[i].Name, "代理", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(TileGroups[i].DisplayName, "代理", StringComparison.OrdinalIgnoreCase))
                {
                    targetGroupIndex = i;
                    break;
                }
            }

            if (targetGroupIndex == -1)
            {
                targetGroupIndex = TileGroups.Count;
                RememberGroupName(targetGroupIndex, "代理");
            }

            string wpfSize = "1x1";
            if (size == "2x2") wpfSize = "2x2";
            else if (size == "2x1" || size == "1x2") wpfSize = "1x2";

            var colors = new[] { "Beige", "Green", "Purple" };
            var randomColor = colors[new Random().Next(colors.Length)];

            var newTile = new TileItemViewModel
            {
                Name = "便笺",
                Size = wpfSize,
                IsStickyNote = true,
                StickyNoteText = text,
                StickyNoteMetadata = "未完成",
                StickyNoteColor = randomColor,
                GroupIndex = targetGroupIndex
            };

            var existingCodes = Tiles.Select(t => t.Code).Where(c => !string.IsNullOrEmpty(c));
            newTile.Code = GenerateUniqueTileCode(newTile.Name, newTile.IsStickyNote, existingCodes);

            AddTile(newTile);
            PackTiles();
            SaveTiles();
        }

        private bool _isSaving;

        public void SaveTiles()
        {
            if (_isSaving) return;
            try
            {
                _isSaving = true;
                var filePath = GetTilesXmlPath();
                var tilesDir = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(tilesDir))
                {
                    Directory.CreateDirectory(tilesDir);
                }

                // 在持久化之前，如果自定义的图片不在Tiles文件夹，将其复制过去并更新路径
                if (!string.IsNullOrEmpty(tilesDir))
                {
                    foreach (var tile in Tiles)
                    {
                        if (!string.IsNullOrEmpty(tile.CustomImageSource))
                        {
                            try
                            {
                                var fileFullPath = Path.GetFullPath(tile.CustomImageSource);
                                var destFullPath = Path.GetFullPath(Path.Combine(tilesDir, Path.GetFileName(tile.CustomImageSource)));

                                if (!(Path.GetDirectoryName(fileFullPath)?.Equals(tilesDir, StringComparison.OrdinalIgnoreCase) ?? false))
                                {
                                    var baseName = Path.GetFileNameWithoutExtension(tile.CustomImageSource);
                                    var ext = Path.GetExtension(tile.CustomImageSource);
                                    var finalDest = destFullPath;
                                    int counter = 1;
                                    while (File.Exists(finalDest))
                                    {
                                        finalDest = Path.Combine(tilesDir, $"{baseName}_{counter}{ext}");
                                        counter++;
                                    }

                                    File.Copy(tile.CustomImageSource, finalDest, true);
                                    tile.CustomImageSource = finalDest;
                                }
                            }
                            catch (Exception ex)
                            {
                                System.Diagnostics.Debug.WriteLine($"Error copying image: {ex.Message}");
                            }
                        }
                    }
                }

                var doc = new XDocument(
                    new XElement("Tiles",
                        Tiles.Select(t => new XElement("Tile",
                            new XAttribute("Name", t.Name),
                            new XAttribute("Code", t.Code ?? string.Empty),
                            new XAttribute("Icon", t.Icon ?? string.Empty),
                            new XAttribute("Size", t.Size ?? "1x1"),
                            new XAttribute("Column", t.Column),
                            new XAttribute("Row", t.Row),
                            new XAttribute("GroupIndex", t.GroupIndex),
                            new XAttribute("IsLocked", t.IsLocked),
                            new XAttribute("IsStickyNote", t.IsStickyNote),
                            new XAttribute("StickyNoteText", t.StickyNoteText ?? string.Empty),
                            new XAttribute("StickyNoteMetadata", t.StickyNoteMetadata ?? "未完成"),
                            new XAttribute("StickyNoteColor", t.StickyNoteColor ?? "Beige"),
                            t.CustomImageSource != null ? new XAttribute("CustomImageSource", t.CustomImageSource) : null
                        ))
                    )
                );

                doc.Save(filePath);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving tiles: {ex.Message}");
            }
            finally
            {
                _isSaving = false;
            }
        }

        private ICommand? _addTileCommand;
        public ICommand AddTileCommand => _addTileCommand ??= new RelayCommand(() =>
        {
            var owner = Application.Current?.MainWindow;
            IReadOnlyList<Skyweaver.Controls.ScheduledTasksControl.Models.ScheduledTask> allTasks;
            try
            {
                allTasks = new Skyweaver.Controls.ScheduledTasksControl.Services.ScheduledTasksRepository().LoadAll();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to load scheduled tasks for tiles view: {ex.Message}");
                allTasks = Array.Empty<Skyweaver.Controls.ScheduledTasksControl.Models.ScheduledTask>();
            }
            var dialog = new AddTileUniversalDialog(allTasks);

            if (owner != null && owner != dialog)
            {
                dialog.Owner = owner;
            }

            if (dialog.ShowDialog() == true)
            {
                TileItemViewModel? newTile = null;
                if (dialog.IsStickyNoteSelected)
                {
                    var colors = new[] { "Beige", "Green", "Purple" };
                    var randomColor = colors[new Random().Next(colors.Length)];
                    newTile = new TileItemViewModel
                    {
                        Name = "便笺",
                        Size = "1x1",
                        IsStickyNote = true,
                        StickyNoteMetadata = "未完成",
                        StickyNoteColor = randomColor,
                        GroupIndex = -1
                    };
                }
                else if (dialog.IsLiveSessionSelected)
                {
                    newTile = new TileItemViewModel
                    {
                        Name = "Live Session",
                        Size = "1x2",
                        Icon = "pack://application:,,,/Resources/NewNodeGraphAlt.png",
                        GroupIndex = -1
                    };
                }
                else if (dialog.SelectedTask != null)
                {
                    newTile = new TileItemViewModel
                    {
                        Name = dialog.SelectedTask.Name,
                        Size = "1x1",
                        Icon = "pack://application:,,,/Resources/Default.png",
                        GroupIndex = -1
                    };
                }

                if (newTile != null)
                {
                    var existingCodes = Tiles.Select(t => t.Code).Where(c => !string.IsNullOrEmpty(c));
                    newTile.Code = GenerateUniqueTileCode(newTile.Name, newTile.IsStickyNote, existingCodes);
                    AddTile(newTile);
                    PackTiles();
                }
            }
        });



        private static bool TryFindSlot(bool[,] occupied, int width, int height, out int column, out int row)
        {
            for (int r = 0; r <= GroupRows - height; r++)
            {
                for (int c = 0; c <= GroupColumns - width; c++)
                {
                    if (IsFree(occupied, c, r, width, height))
                    {
                        column = c;
                        row = r;
                        return true;
                    }
                }
            }

            column = 0;
            row = 0;
            return false;
        }

        private static bool IsFree(bool[,] occupied, int column, int row, int width, int height)
        {
            if (column < 0 || row < 0 || column + width > GroupColumns || row + height > GroupRows)
            {
                return false;
            }

            for (int r = row; r < row + height; r++)
            {
                for (int c = column; c < column + width; c++)
                {
                    if (occupied[c, r])
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private static void MarkOccupied(bool[,] occupied, int column, int row, int width, int height)
        {
            if (column < 0 || row < 0 || column + width > GroupColumns || row + height > GroupRows)
            {
                return;
            }

            for (int r = row; r < row + height; r++)
            {
                for (int c = column; c < column + width; c++)
                {
                    occupied[c, r] = true;
                }
            }
        }

        private void SaveCurrentGroupNames()
        {
            for (int i = 0; i < TileGroups.Count; i++)
            {
                RememberGroupName(i, TileGroups[i].Name);
            }
        }

        private void EnsureGroupCount(int requiredGroupCount)
        {
            while (TileGroups.Count < requiredGroupCount)
            {
                int index = TileGroups.Count;
                var group = new TileGroupViewModel
                {
                    Index = index,
                    Name = GetRememberedGroupName(index)
                };

                group.PropertyChanged += OnGroupPropertyChanged;
                TileGroups.Add(group);
            }

            while (TileGroups.Count > requiredGroupCount)
            {
                var group = TileGroups[^1];
                RememberGroupName(TileGroups.Count - 1, group.Name);
                group.PropertyChanged -= OnGroupPropertyChanged;
                group.Cleanup();
                TileGroups.RemoveAt(TileGroups.Count - 1);
            }

            for (int i = 0; i < TileGroups.Count; i++)
            {
                TileGroups[i].Index = i;
            }
        }

        private void OnGroupPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(TileGroupViewModel.Name) && sender is TileGroupViewModel group)
            {
                RememberGroupName(group.Index, group.Name);
            }
        }

        private string GetRememberedGroupName(int index)
        {
            return index >= 0 && index < _rememberedGroupNames.Count && !string.IsNullOrWhiteSpace(_rememberedGroupNames[index])
                ? _rememberedGroupNames[index]
                : TileGroupViewModel.DefaultName(index);
        }

        private void RememberGroupName(int index, string name)
        {
            if (index < 0)
            {
                return;
            }

            while (_rememberedGroupNames.Count <= index)
            {
                _rememberedGroupNames.Add(TileGroupViewModel.DefaultName(_rememberedGroupNames.Count));
            }

            _rememberedGroupNames[index] = string.IsNullOrWhiteSpace(name)
                ? TileGroupViewModel.DefaultName(index)
                : name.Trim();
        }

        private static void SyncCollection(ObservableCollection<TileItemViewModel> collection, IReadOnlyList<TileItemViewModel> desired)
        {
            var desiredSet = new HashSet<TileItemViewModel>(desired);
            for (int i = collection.Count - 1; i >= 0; i--)
            {
                if (!desiredSet.Contains(collection[i]))
                {
                    collection.RemoveAt(i);
                }
            }

            for (int i = 0; i < desired.Count; i++)
            {
                if (i < collection.Count && ReferenceEquals(collection[i], desired[i]))
                {
                    continue;
                }

                int existingIndex = collection.IndexOf(desired[i]);
                if (existingIndex >= 0)
                {
                    collection.Move(existingIndex, i);
                }
                else if (i < collection.Count)
                {
                    collection.Insert(i, desired[i]);
                }
                else
                {
                    collection.Add(desired[i]);
                }
            }

            while (collection.Count > desired.Count)
            {
                collection.RemoveAt(collection.Count - 1);
            }
        }

        private static string GenerateUniqueTileCode(string name, bool isStickyNote, IEnumerable<string> existingCodes)
        {
            string prefix = "tile";
            if (isStickyNote)
            {
                prefix = "sticky_note";
            }
            else if (name == "Live Session")
            {
                prefix = "live_session";
            }
            else if (!string.IsNullOrWhiteSpace(name))
            {
                string cleanName = CleanNameForCode(name);
                if (!string.IsNullOrEmpty(cleanName))
                {
                    prefix = cleanName;
                }
            }

            string candidate = prefix;
            int counter = 1;
            var existingSet = new HashSet<string>(existingCodes, StringComparer.OrdinalIgnoreCase);
            while (existingSet.Contains(candidate))
            {
                candidate = $"{prefix}_{counter}";
                counter++;
            }
            return candidate;
        }

        private static string CleanNameForCode(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return "tile";

            string lowerName = name.Trim().ToLowerInvariant();
            
            var commonTranslations = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "便笺", "sticky_note" },
                { "日常", "daily" },
                { "生产力", "productivity" },
                { "简报", "briefing" },
                { "娱乐", "entertainment" },
                { "应用", "applications" }
            };

            if (commonTranslations.TryGetValue(lowerName, out var translation))
            {
                return translation;
            }

            var sb = new System.Text.StringBuilder();
            foreach (char c in name)
            {
                if (char.IsAsciiLetterOrDigit(c))
                {
                    sb.Append(char.ToLowerInvariant(c));
                }
                else if (c == '_' || c == '-' || char.IsWhiteSpace(c))
                {
                    if (sb.Length > 0 && sb[sb.Length - 1] != '_')
                    {
                        sb.Append('_');
                    }
                }
            }
            string englishPart = sb.ToString().Trim('_');
            if (englishPart.Length >= 2)
            {
                return englishPart;
            }

            return "tile";
        }

    }

    public sealed class TileLayoutTransitionEventArgs : EventArgs
    {
        public TileLayoutTransitionEventArgs(TileItemViewModel tile)
        {
            Tile = tile;
        }

        public TileItemViewModel Tile { get; }
    }

    public class StickyNoteReplyViewModel : ObservableObject
    {
        private string _creator = string.Empty;
        private DateTime _dateTime;
        private string _content = string.Empty;

        public string Creator
        {
            get => _creator;
            set => SetProperty(ref _creator, value ?? string.Empty);
        }

        public DateTime DateTime
        {
            get => _dateTime;
            set
            {
                if (SetProperty(ref _dateTime, value))
                {
                    OnPropertyChanged(nameof(DisplayDateTime));
                }
            }
        }

        public string Content
        {
            get => _content;
            set => SetProperty(ref _content, value ?? string.Empty);
        }

        public string DisplayDateTime => DateTime.ToString("yyyy-MM-dd HH:mm:ss");
    }
}
