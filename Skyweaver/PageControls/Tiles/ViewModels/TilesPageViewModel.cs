using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Skyweaver.Commands;
using Skyweaver.Infrastructure.Mvvm;

namespace Skyweaver.PageControls.Tiles.ViewModels
{
    public sealed class TileItemViewModel : ObservableObject
    {
        private string _name = string.Empty;
        private string _icon = string.Empty;
        private string _size = "1x1";
        private string? _customImageSource;
        private int _column;
        private int _row;
        private int _columnSpan = 1;
        private int _rowSpan = 1;
        private bool _isDragging;

        public TileItemViewModel()
        {
            RunCommand = new RelayCommand(() =>
            {
                MessageBox.Show($"Running {Name}...", "Tile system", MessageBoxButton.OK, MessageBoxImage.Information);
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
                    RequestLayoutUpdate?.Invoke(this, EventArgs.Empty);
                }
            });

            CustomImageCommand = new RelayCommand(() =>
            {
                var openFileDialog = new Microsoft.Win32.OpenFileDialog
                {
                    Filter = "Image files (*.png;*.jpg;*.jpeg;*.gif;*.bmp)|*.png;*.jpg;*.jpeg;*.gif;*.bmp",
                    Title = "Choose tile image"
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    CustomImageSource = openFileDialog.FileName;
                }
            });
        }

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public string Icon
        {
            get => _icon;
            set => SetProperty(ref _icon, value);
        }

        public string Size
        {
            get => _size;
            set
            {
                if (SetProperty(ref _size, NormalizeSize(value)))
                {
                    ApplySize();
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

        public bool IsLarge => Size == "2x2";

        public bool HasCustomImage => IsLarge && !string.IsNullOrEmpty(CustomImageSource);

        public ICommand RunCommand { get; }

        public ICommand RemoveCommand { get; }

        public ICommand SetSizeCommand { get; }

        public ICommand CustomImageCommand { get; }

        public event EventHandler? RequestLayoutUpdate;

        public event EventHandler? RequestRemove;

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
            OnPropertyChanged(nameof(HasCustomImage));
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

        public int Index
        {
            get => _index;
            set => SetProperty(ref _index, value);
        }

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, string.IsNullOrWhiteSpace(value) ? DefaultName(Index) : value.Trim());
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
            string[] names = { "Essentials", "Productivity", "System Tools", "Entertainment", "Applications" };
            return index >= 0 && index < names.Length ? names[index] : $"Group {index + 1}";
        }
    }

    public sealed class TilesPageViewModel : ObservableObject
    {
        public const int GroupColumns = 6;
        public const int GroupRows = 4;
        public const int GroupCellCount = GroupColumns * GroupRows;

        private readonly List<string> _rememberedGroupNames = new();
        private bool _isPacking;

        public TilesPageViewModel()
        {
            InitializeDefaultTiles();
        }

        public ObservableCollection<TileItemViewModel> Tiles { get; } = new ObservableCollection<TileItemViewModel>();

        public ObservableCollection<TileGroupViewModel> TileGroups { get; } = new ObservableCollection<TileGroupViewModel>();

        public event EventHandler<TileLayoutTransitionEventArgs>? TileLayoutChanging;

        public event EventHandler<TileLayoutTransitionEventArgs>? TileLayoutChanged;

        public void MoveTileToCell(TileItemViewModel tile, int targetGroupIndex, int targetColumn, int targetRow)
        {
            if (!Tiles.Contains(tile))
            {
                return;
            }

            targetGroupIndex = Math.Max(0, targetGroupIndex);
            targetColumn = Math.Clamp(targetColumn, 0, GroupColumns - tile.ColumnSpan);
            targetRow = Math.Clamp(targetRow, 0, GroupRows - tile.RowSpan);

            var orderedWithoutTile = Tiles.Where(candidate => !ReferenceEquals(candidate, tile)).ToList();
            var placementsWithoutTile = ComputeLayout(orderedWithoutTile);
            int targetSlot = targetRow * GroupColumns + targetColumn;
            int insertIndex = orderedWithoutTile.Count;

            for (int i = 0; i < placementsWithoutTile.Count; i++)
            {
                var placement = placementsWithoutTile[i];
                int placementSlot = placement.Row * GroupColumns + placement.Column;
                if (placement.GroupIndex > targetGroupIndex ||
                    placement.GroupIndex == targetGroupIndex && placementSlot >= targetSlot)
                {
                    insertIndex = i;
                    break;
                }
            }

            Tiles.Remove(tile);
            Tiles.Insert(Math.Clamp(insertIndex, 0, Tiles.Count), tile);
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
                    TileGroups.Clear();
                    return;
                }

                var placements = ComputeLayout(Tiles);
                int requiredGroupCount = placements.Count == 0
                    ? 0
                    : placements.Max(placement => placement.GroupIndex) + 1;

                EnsureGroupCount(requiredGroupCount);
                var groupedTiles = new List<TileItemViewModel>[requiredGroupCount];
                for (int i = 0; i < groupedTiles.Length; i++)
                {
                    groupedTiles[i] = new List<TileItemViewModel>();
                }

                foreach (var placement in placements)
                {
                    placement.Tile.Column = placement.Column;
                    placement.Tile.Row = placement.Row;
                    groupedTiles[placement.GroupIndex].Add(placement.Tile);
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
            }
        }

        private void InitializeDefaultTiles()
        {
            var defaultTiles = new[]
            {
                new TileItemViewModel { Name = "Mail", Size = "1x2", Icon = "M2,4h20v16H2V4z M4,6v1.5l8,4.5l8,-4.5V6H4z M4,9.5V18h16V9.5l-8,4.5L4,9.5z" },
                new TileItemViewModel { Name = "Calendar", Size = "1x1", Icon = "M19,4h-1V2h-2v2H8V2H6v2H5C3.89,4,3.01,4.9,3.01,6L3,20c0,1.1,0.89,2,2,2h14c1.1,0,2,-0.9,2,-2V6C21,4.9,20.1,4,19,4z M19,20H5V10h14V20z M19,8H5V6h14V8z" },
                new TileItemViewModel { Name = "Feeds", Size = "1x2", Icon = "M6.18,2.69C3.56,3.85,1.94,6.05,1,8.5h4c0.5-0.9,1.1-1.6,2-2.1L6.18,2.69z M2,17c0,1.66,1.34,3,3,3s3,-1.34,3,-3s-1.34,-3,-3,-3S2,15.34,2,17z M2,10v2c4.42,0,8,3.58,8,8h2C12,14.48,7.52,10,2,10z M2,5v2c7.18,0,13,5.82,13,13h2C17,11.72,10.28,5,2,5z" },
                new TileItemViewModel { Name = "IE Browser", Size = "1x1", Icon = "M12,2C6.48,2,2,6.48,2,12s4.48,10,10,10s10,-4.48,10,-10S17.52,2,12,2z M11,19.93C7.05,19.44,4,16.08,4,12c0,-0.62,0.08,-1.21,0.21,-1.79L9,15v1c0,1.1,0.9,2,2,2V19.93z M17.9,17.39c-0.26,-0.81,-1,-1.39,-1.9,-1.39h-1v-3c0,-0.55,-0.45,-1,-1,-1H8v-2h2c0.55,0,1,-0.45,1,-1V7h2c1.1,0,2,-0.9,2,-2v-0.41c2.93,1.19,5,4.06,5,7.41C20,14.08,19.2,15.97,17.9,17.39z" },
                new TileItemViewModel { Name = "Get Started", Size = "1x1", Icon = "M5,13.18V20h6.82L19,12.18L12.18,5.36L5,13.18z M12,2C6.48,2,2,6.48,2,12s4.48,10,10,10s10,-4.48,10,-10S17.52,2,12,2z M12,20c-4.41,0,-8,-3.59,-8,-8s3.59,-8,8,-8s8,3.59,8,8S16.41,20,12,20z" },
                new TileItemViewModel { Name = "Media Center", Size = "1x1", Icon = "M12,2C6.48,2,2,6.48,2,12s4.48,10,10,10s10,-4.48,10,-10S17.52,2,12,2z M10,16.5v-9l6,4.5L10,16.5z" },
                new TileItemViewModel { Name = "Gallery", Size = "1x1", Icon = "M21,19V5c0,-1.1,-0.9,-2,-2,-2H5C3.9,3,3,3.9,3,5v14c0,1.1,0.9,2,2,2h14C20.1,21,21,20.1,21,19z M8.5,13.5l2.5,3.01L14.5,12l4.5,6H5L8.5,13.5z" },
                new TileItemViewModel { Name = "Marketplace", Size = "1x1", Icon = "M20,6h-4V4c0,-1.11,-0.89,-2,-2,-2h-4C8.89,2,8,2.89,8,4v2H4C2.89,6,2,6.89,2,8v12c0,1.11,0.89,2,2,2h16c1.11,0,2,-0.89,2,-2V8C22,6.89,21.11,6,20,6z M10,4h4v2h-4V4z M20,20H4V8h16V20z M12,12c-1.66,0,-3,-1.34,-3,-3h2c0,0.55,0.45,1,1,1s1,-0.45,1,-1h2C15,10.66,13.66,12,12,12z" },
                new TileItemViewModel { Name = "Messenger", Size = "1x1", Icon = "M20,2H4C2.9,2,2,2.9,2,4v18l4,-4h14c1.1,0,2,-0.9,2,-2V4C22,2.9,21.1,2,20,2z M20,16H5.17L4,17.17V4h16V16z M6,12h12v2H6V12z M6,9h12v2H6V9z M6,6h12v2H6V6z" },
                new TileItemViewModel { Name = "Weather", Size = "2x2", Icon = "M6.05,8.05c-2.73,2.73,-2.73,7.15,0,9.88s7.15,2.73,9.88,0c2.43,-2.43,2.69,-6.2,0.77,-8.93l1.43,-1.43c2.72,3.48,2.35,8.6,-1.1,12.05s-8.58,3.82,-12.05,1.1l1.43,-1.43c1.92,1.92,4.92,2.18,7.12,0.77l-9.88,-9.88z M12,3c-0.55,0,-1,0.45,-1,1v2c0,0.55,0.45,1,1,1s1,-0.45,1,-1V4C13,3.45,12.55,3,12,3z M12,17c-0.55,0,-1,0.45,-1,1v2c0,0.55,0.45,1,1,1s1,-0.45,1,-1v-2C13,17.45,12.55,17,12,17z M20,11h-2c-0.55,0,-1,0.45,-1,1s0.45,1,1,1h2c0.55,0,1,-0.45,1,-1S20.55,11,20,11z M6,11H4c-0.55,0,-1,0.45,-1,1s0.45,1,1,1h2c0.55,0,1,-0.45,1,-1S6.55,11,6,11z M17.66,6.34L16.24,7.76c-0.39,0.39,-0.39,1.02,0,1.41s1.02,0.39,1.41,0l1.42,-1.42c0.39,-0.39,0.39,-1.02,0,-1.41S18.05,5.95,17.66,6.34z M6.34,17.66l1.42,-1.42c0.39,-0.39,0.39,-1.02,0,-1.41s-1.02,-0.39,-1.41,0l-1.42,1.42c-0.39,0.39,-0.39,1.02,0,1.41S5.95,18.05,6.34,17.66z M17.66,17.66l-1.42,-1.42c-0.39,-0.39,-1.02,-0.39,-1.41,0c-0.39,0.39,-0.39,1.02,0,1.41l1.42,1.42c0.39,0.39,1.02,0.39,1.41,0C18.05,18.68,18.05,18.05,17.66,17.66z M6.34,6.34c-0.39,0.39,-0.39,1.02,0,1.41l1.42,1.42c0.39,0.39,1.02,0.39,1.41,0s0.39,-1.02,0,-1.41L7.76,6.34C7.36,5.95,6.73,5.95,6.34,6.34z" },
                new TileItemViewModel { Name = "Desktop", Size = "1x2", Icon = "M21,2H3C1.9,2,1,2.9,1,4v12c0,1.1,0.9,2,2,2h7v2H8v2h8v-2h-2v-2h7c1.1,0,2,-0.9,2,-2V4C23,2.9,22.1,2,21,2z M21,14H3V4h18V14z" },
                new TileItemViewModel { Name = "Control Panel", Size = "1x1", Icon = "M19.14,12.94c0.04,-0.3,0.06,-0.61,0.06,-0.94c0,-0.32,-0.02,-0.64,-0.07,-0.94l2.03,-1.58c0.18,-0.14,0.23,-0.41,0.12,-0.61l-1.92,-3.32c-0.12,-0.22,-0.37,-0.29,-0.59,-0.22l-2.39,0.96c-0.5,-0.38,-1.03,-0.7,-1.62,-0.94L14.4,2.81c-0.04,-0.24,-0.24,-0.41,-0.48,-0.41h-3.84c-0.24,0,-0.43,0.17,-0.47,0.41L9.25,5.35C8.66,5.59,8.12,5.92,7.63,6.29L5.24,5.33c-0.22,-0.08,-0.47,0,-0.59,0.22L2.74,8.87C2.62,9.08,2.68,9.34,2.86,9.48l2.03,1.58C4.84,11.36,4.8,11.69,4.8,12c0,0.31,0.04,0.64,0.07,0.94l-2.03,1.58c-0.18,0.14,-0.23,0.41,-0.12,0.61l1.92,3.32c0.12,0.22,0.37,0.29,0.59,0.22l2.39,-0.96c0.5,0.38,1.03,0.7,1.62,0.94l0.36,2.54c0.05,0.24,0.24,0.41,0.48,0.41h3.84c0.24,0,0.44,-0.17,0.47,-0.41l0.36,-2.54c0.59,-0.24,1.13,-0.56,1.62,-0.94l2.39,0.96c0.22,0.08,0.47,0,0.59,-0.22l1.92,-3.32c0.12,-0.22,0.07,-0.47,-0.12,-0.61L19.14,12.94z M12,15.6c-1.98,0,-3.6,-1.62,-3.6,-3.6s1.62,-3.6,3.6,-3.6s3.6,1.62,3.6,3.6S13.98,15.6,12,15.6z" },
                new TileItemViewModel { Name = "Clock", Size = "1x1", Icon = "M11.99,2C6.47,2,2,6.48,2,12s4.47,10,9.99,10C17.52,22,22,17.52,22,12S17.52,2,11.99,2z M12,20c-4.42,0,-8,-3.58,-8,-8s3.58,-8,8,-8s8,3.58,8,8S16.42,20,12,20z M12.5,7H11v6l5.25,3.15l0.75,-1.23L12.5,12.19V7z" }
            };

            foreach (var tile in defaultTiles)
            {
                AddTile(tile);
            }

            PackTiles();
        }

        private void AddTile(TileItemViewModel tile)
        {
            tile.RequestLayoutUpdate += OnTileRequestLayoutUpdate;
            tile.RequestRemove += OnTileRequestRemove;
            Tiles.Add(tile);
        }

        private void OnTileRequestLayoutUpdate(object? sender, EventArgs e)
        {
            if (sender is TileItemViewModel tile)
            {
                TileLayoutChanging?.Invoke(this, new TileLayoutTransitionEventArgs(tile));
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
            Tiles.Remove(tile);
            PackTiles();
        }

        private static List<TilePlacement> ComputeLayout(IReadOnlyList<TileItemViewModel> orderedTiles)
        {
            var placements = new List<TilePlacement>(orderedTiles.Count);
            var occupied = new bool[GroupColumns, GroupRows];
            int currentGroupIndex = 0;

            foreach (var tile in orderedTiles)
            {
                int width = Math.Clamp(tile.ColumnSpan, 1, GroupColumns);
                int height = Math.Clamp(tile.RowSpan, 1, GroupRows);

                if (!TryFindSlot(occupied, width, height, out int column, out int row))
                {
                    currentGroupIndex++;
                    occupied = new bool[GroupColumns, GroupRows];
                    TryFindSlot(occupied, width, height, out column, out row);
                }

                MarkOccupied(occupied, column, row, width, height);
                placements.Add(new TilePlacement(tile, currentGroupIndex, column, row));
            }

            return placements;
        }

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

        private readonly record struct TilePlacement(TileItemViewModel Tile, int GroupIndex, int Column, int Row);
    }

    public sealed class TileLayoutTransitionEventArgs : EventArgs
    {
        public TileLayoutTransitionEventArgs(TileItemViewModel tile)
        {
            Tile = tile;
        }

        public TileItemViewModel Tile { get; }
    }
}
