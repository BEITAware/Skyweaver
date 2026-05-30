using System.Diagnostics;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Skyweaver.Services.Localization;

namespace Skyweaver.Controls.ChatSessionControl.Views
{
    public sealed class MarkdownContentControl : ContentControl
    {
        private const double TableMaxHeight = 360.0;
        private const double TableMinColumnWidth = 72.0;
        private const int StreamingRefreshThrottleMilliseconds = 60;

        private static readonly Brush HeadingForegroundBrush = CreateFrozenBrush(Color.FromRgb(0xBD, 0xEB, 0xFF));
        private static readonly Brush QuoteForegroundBrush = CreateFrozenBrush(Color.FromArgb(0xE0, 0xEA, 0xFD, 0xFF));
        private static readonly Brush QuoteBorderBrush = CreateFrozenBrush(Color.FromArgb(0x88, 0x7A, 0xD8, 0xF1));
        private static readonly Brush LinkForegroundBrush = CreateFrozenBrush(Color.FromRgb(0x8F, 0xD8, 0xFF));
        private static readonly Brush InlineCodeForegroundBrush = CreateFrozenBrush(Color.FromRgb(0xFF, 0xF2, 0xCF));
        private static readonly Brush InlineCodeBackgroundBrush = CreateFrozenBrush(Color.FromArgb(0x38, 0x00, 0x00, 0x00));
        private static readonly Brush CodeBorderBrush = CreateFrozenBrush(Color.FromArgb(0x44, 0x96, 0xFC, 0xFF));
        private static readonly Brush CodeBackgroundBrush = CreateFrozenBrush(Color.FromArgb(0x24, 0x00, 0x00, 0x00));
        private static readonly Brush CodeForegroundBrush = CreateFrozenBrush(Color.FromRgb(0x9C, 0xDC, 0xFE));
        private static readonly Brush BadgeBackgroundBrush = CreateFrozenBrush(Color.FromArgb(0x22, 0x00, 0x00, 0x00));
        private static readonly Brush BadgeBorderBrush = CreateFrozenBrush(Color.FromArgb(0x33, 0x88, 0xD7, 0xE8));
        private static readonly Brush MathBorderBrush = CreateFrozenBrush(Color.FromArgb(0x44, 0xCF, 0xB9, 0x7A));
        private static readonly Brush MathBackgroundBrush = CreateFrozenBrush(Color.FromArgb(0x18, 0x22, 0x1A, 0x10));
        private static readonly Brush MathForegroundBrush = CreateFrozenBrush(Color.FromRgb(0xF4, 0xDF, 0xB5));
        private static readonly Style TableCellTextStyle = CreateTableCellTextStyle(TextWrapping.Wrap);
        private static readonly Style TableCellStreamingTextStyle = CreateTableCellTextStyle(TextWrapping.NoWrap);

        public static readonly DependencyProperty MarkdownTextProperty =
            DependencyProperty.Register(
                nameof(MarkdownText),
                typeof(string),
                typeof(MarkdownContentControl),
                new PropertyMetadata(string.Empty, OnMarkdownTextChanged));

        public static readonly DependencyProperty IsStreamingProperty =
            DependencyProperty.Register(
                nameof(IsStreaming),
                typeof(bool),
                typeof(MarkdownContentControl),
                new PropertyMetadata(false, OnIsStreamingChanged));

        public static readonly DependencyProperty IsUserMessageProperty =
            DependencyProperty.Register(
                nameof(IsUserMessage),
                typeof(bool),
                typeof(MarkdownContentControl),
                new PropertyMetadata(false, OnIsUserMessageChanged));

        private static readonly DependencyProperty CachedBlockProperty =
            DependencyProperty.RegisterAttached(
                "CachedBlock",
                typeof(MarkdownBlock),
                typeof(MarkdownContentControl),
                new PropertyMetadata(null));

        private static MarkdownBlock? GetCachedBlock(DependencyObject obj)
        {
            return (MarkdownBlock?)obj.GetValue(CachedBlockProperty);
        }

        private static void SetCachedBlock(DependencyObject obj, MarkdownBlock? value)
        {
            obj.SetValue(CachedBlockProperty, value);
        }

        public string MarkdownText
        {
            get => (string)GetValue(MarkdownTextProperty);
            set => SetValue(MarkdownTextProperty, value);
        }

        public bool IsStreaming
        {
            get => (bool)GetValue(IsStreamingProperty);
            set => SetValue(IsStreamingProperty, value);
        }

        public bool IsUserMessage
        {
            get => (bool)GetValue(IsUserMessageProperty);
            set => SetValue(IsUserMessageProperty, value);
        }

        private DateTime _lastStreamingRefreshUtc = DateTime.MinValue;
        private bool _refreshPending;
        private bool _isRefreshing;

        public MarkdownContentControl()
        {
            Focusable = false;
            IsTabStop = false;
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            RefreshContent();
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == ForegroundProperty ||
                e.Property == FontSizeProperty ||
                e.Property == FontFamilyProperty ||
                e.Property == FontWeightProperty ||
                e.Property == FontStyleProperty)
            {
                Content = null; // 样式改变时清空子元素，强制重新生成以应用新样式
                RefreshContent();
            }
        }

        private static void OnMarkdownTextChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            ((MarkdownContentControl)dependencyObject).ScheduleRefresh();
        }

        private static void OnIsStreamingChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            ((MarkdownContentControl)dependencyObject).ScheduleRefresh(force: true);
        }

        private static void OnIsUserMessageChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            ((MarkdownContentControl)dependencyObject).ScheduleRefresh(force: true);
        }

        private void ScheduleRefresh(bool force = false)
        {
            if (!IsLoaded)
            {
                RefreshContent();
                return;
            }

            if (!IsStreaming)
            {
                RefreshContent();
                return;
            }

            var now = DateTime.UtcNow;
            var elapsedMilliseconds = (now - _lastStreamingRefreshUtc).TotalMilliseconds;
            if (force || elapsedMilliseconds >= StreamingRefreshThrottleMilliseconds)
            {
                _lastStreamingRefreshUtc = now;
                RefreshContent();
                return;
            }

            if (_refreshPending)
            {
                return;
            }

            _refreshPending = true;
            Dispatcher.BeginInvoke(new Action(() =>
            {
                _refreshPending = false;
                _lastStreamingRefreshUtc = DateTime.UtcNow;
                RefreshContent();
            }), System.Windows.Threading.DispatcherPriority.Background);
        }

        private void RefreshContent()
        {
            if (_isRefreshing)
            {
                return;
            }

            _isRefreshing = true;
            try
            {
                var blocks = MarkdownDocumentParser.Parse(MarkdownText, IsStreaming, IsUserMessage);
                if (blocks.Count == 0)
                {
                    Content = null;
                    return;
                }

                var root = Content as StackPanel;
                if (root == null)
                {
                    root = new StackPanel();
                    Content = root;
                }

                for (var index = 0; index < blocks.Count; index++)
                {
                    var block = blocks[index];
                    FrameworkElement? element = null;

                    if (index < root.Children.Count)
                    {
                        var existingElement = root.Children[index] as FrameworkElement;
                        if (existingElement != null && IsElementMatchingBlock(existingElement, block))
                        {
                            element = existingElement;
                            var cachedBlock = GetCachedBlock(element);
                            if (!IsBlockEquivalent(cachedBlock, block))
                            {
                                UpdateBlockElement(element, block);
                                SetCachedBlock(element, block);
                            }
                        }
                    }

                    if (element == null)
                    {
                        element = CreateBlockElement(block);
                        SetCachedBlock(element, block);
                        if (index < root.Children.Count)
                        {
                            root.Children.RemoveAt(index);
                            root.Children.Insert(index, element);
                        }
                        else
                        {
                            root.Children.Add(element);
                        }
                    }

                    var expectedMargin = new Thickness(0);
                    if (index < blocks.Count - 1)
                    {
                        expectedMargin = AppendBottomMargin(expectedMargin, 8.0);
                    }

                    if (element.Margin != expectedMargin)
                    {
                        element.Margin = expectedMargin;
                    }
                }

                while (root.Children.Count > blocks.Count)
                {
                    root.Children.RemoveAt(root.Children.Count - 1);
                }
            }
            finally
            {
                _isRefreshing = false;
            }
        }

        private static bool IsBlockEquivalent(MarkdownBlock? a, MarkdownBlock? b)
        {
            if (ReferenceEquals(a, b)) return true;
            if (a == null || b == null) return false;
            if (a.GetType() != b.GetType()) return false;

            switch (a)
            {
                case MarkdownParagraphBlock paraA:
                    var paraB = (MarkdownParagraphBlock)b;
                    return AreInlinesEquivalent(paraA.Inlines, paraB.Inlines);

                case MarkdownHeadingBlock headA:
                    var headB = (MarkdownHeadingBlock)b;
                    return headA.Level == headB.Level && AreInlinesEquivalent(headA.Inlines, headB.Inlines);

                case MarkdownCodeBlock codeA:
                    var codeB = (MarkdownCodeBlock)b;
                    return codeA.Content == codeB.Content && codeA.Language == codeB.Language;

                case MarkdownQuoteBlock quoteA:
                    var quoteB = (MarkdownQuoteBlock)b;
                    return AreInlinesEquivalent(quoteA.Inlines, quoteB.Inlines);

                case MarkdownListBlock listA:
                    var listB = (MarkdownListBlock)b;
                    if (listA.Items.Count != listB.Items.Count) return false;
                    for (int i = 0; i < listA.Items.Count; i++)
                    {
                        var itemA = listA.Items[i];
                        var itemB = listB.Items[i];
                        if (itemA.Marker != itemB.Marker || !AreInlinesEquivalent(itemA.Inlines, itemB.Inlines))
                            return false;
                    }
                    return true;

                case MarkdownMathBlock mathA:
                    var mathB = (MarkdownMathBlock)b;
                    return mathA.Content == mathB.Content;

                case MarkdownTableBlock tableA:
                    var tableB = (MarkdownTableBlock)b;
                    if (tableA.Columns.Count != tableB.Columns.Count) return false;
                    for (int i = 0; i < tableA.Columns.Count; i++)
                    {
                        if (tableA.Columns[i].Header != tableB.Columns[i].Header) return false;
                    }
                    if (tableA.Rows.Count != tableB.Rows.Count) return false;
                    for (int i = 0; i < tableA.Rows.Count; i++)
                    {
                        var rowA = tableA.Rows[i];
                        var rowB = tableB.Rows[i];
                        if (rowA.Cells.Count != rowB.Cells.Count) return false;
                        for (int j = 0; j < rowA.Cells.Count; j++)
                        {
                            if (rowA.Cells[j] != rowB.Cells[j]) return false;
                        }
                    }
                    return true;

                default:
                    return false;
            }
        }

        private static bool AreInlinesEquivalent(IReadOnlyList<MarkdownInline> a, IReadOnlyList<MarkdownInline> b)
        {
            if (ReferenceEquals(a, b)) return true;
            if (a == null || b == null) return false;
            if (a.Count != b.Count) return false;
            for (int i = 0; i < a.Count; i++)
            {
                if (!IsInlineEquivalent(a[i], b[i])) return false;
            }
            return true;
        }

        private static bool IsInlineEquivalent(MarkdownInline a, MarkdownInline b)
        {
            if (ReferenceEquals(a, b)) return true;
            if (a == null || b == null) return false;
            if (a.GetType() != b.GetType()) return false;

            switch (a)
            {
                case MarkdownTextInline textA:
                    var textB = (MarkdownTextInline)b;
                    return textA.Text == textB.Text;

                case MarkdownStrongInline strongA:
                    var strongB = (MarkdownStrongInline)b;
                    return AreInlinesEquivalent(strongA.Children, strongB.Children);

                case MarkdownEmphasisInline empA:
                    var empB = (MarkdownEmphasisInline)b;
                    return AreInlinesEquivalent(empA.Children, empB.Children);

                case MarkdownCodeInline codeA:
                    var codeB = (MarkdownCodeInline)b;
                    return codeA.Content == codeB.Content;

                case MarkdownLinkInline linkA:
                    var linkB = (MarkdownLinkInline)b;
                    return linkA.Url == linkB.Url && AreInlinesEquivalent(linkA.Label, linkB.Label);

                case MarkdownMathInline mathA:
                    var mathB = (MarkdownMathInline)b;
                    return mathA.Content == mathB.Content && mathA.IsDisplayStyle == mathB.IsDisplayStyle;

                case MarkdownLineBreakInline:
                    return true;

                default:
                    return false;
            }
        }

        private static string GetBlockTag(MarkdownBlock block)
        {
            return block switch
            {
                MarkdownHeadingBlock => "Heading",
                MarkdownParagraphBlock => "Paragraph",
                MarkdownCodeBlock => "Code",
                MarkdownQuoteBlock => "Quote",
                MarkdownListBlock => "List",
                MarkdownMathBlock => "Math",
                MarkdownTableBlock => "Table",
                _ => "Unknown"
            };
        }

        private static bool IsElementMatchingBlock(FrameworkElement element, MarkdownBlock block)
        {
            return element.Tag?.ToString() == GetBlockTag(block);
        }

        private void UpdateBlockElement(FrameworkElement element, MarkdownBlock block)
        {
            switch (block)
            {
                case MarkdownHeadingBlock heading:
                    var headingText = (TextBlock)element;
                    headingText.Inlines.Clear();
                    AddInlines(headingText.Inlines, heading.Inlines);
                    break;

                case MarkdownParagraphBlock paragraph:
                    var paraText = (TextBlock)element;
                    paraText.Inlines.Clear();
                    AddInlines(paraText.Inlines, paragraph.Inlines);
                    break;

                case MarkdownQuoteBlock quote:
                    var quoteBorder = (Border)element;
                    var quoteText = (TextBlock)quoteBorder.Child;
                    quoteText.Inlines.Clear();
                    AddInlines(quoteText.Inlines, quote.Inlines);
                    break;

                case MarkdownListBlock list:
                    UpdateListBlock((StackPanel)element, list);
                    break;

                case MarkdownCodeBlock code:
                    UpdateCodeBlock((Border)element, code);
                    break;

                case MarkdownMathBlock math:
                    var mathBorder = (Border)element;
                    var mathText = (TextBlock)mathBorder.Child;
                    mathText.Text = $"\\[{math.Content}\\]";
                    break;

                case MarkdownTableBlock table:
                    UpdateTableBlock((DataGrid)element, table);
                    break;
            }
        }

        private FrameworkElement CreateBlockElement(MarkdownBlock block)
        {
            return block switch
            {
                MarkdownHeadingBlock heading => CreateHeadingBlock(heading),
                MarkdownCodeBlock code => CreateCodeBlock(code),
                MarkdownQuoteBlock quote => CreateQuoteBlock(quote),
                MarkdownListBlock list => CreateListBlock(list),
                MarkdownMathBlock math => CreateMathBlock(math),
                MarkdownTableBlock table => CreateTableBlock(table),
                MarkdownParagraphBlock paragraph => CreateParagraphBlock(paragraph),
                _ => CreateParagraphBlock(new MarkdownParagraphBlock(Array.Empty<MarkdownInline>()))
            };
        }

        private TextBlock CreateParagraphBlock(MarkdownParagraphBlock block)
        {
            var textBlock = CreateBaseTextBlock();
            textBlock.Tag = "Paragraph";
            AddInlines(textBlock.Inlines, block.Inlines);
            return textBlock;
        }

        private TextBlock CreateHeadingBlock(MarkdownHeadingBlock block)
        {
            var textBlock = CreateBaseTextBlock();
            textBlock.Tag = "Heading";
            textBlock.Foreground = HeadingForegroundBrush;
            textBlock.FontWeight = FontWeights.SemiBold;
            textBlock.FontSize = ResolveHeadingFontSize(block.Level);
            AddInlines(textBlock.Inlines, block.Inlines);
            return textBlock;
        }

        private Border CreateQuoteBlock(MarkdownQuoteBlock block)
        {
            var textBlock = CreateBaseTextBlock();
            textBlock.Foreground = QuoteForegroundBrush;
            AddInlines(textBlock.Inlines, block.Inlines);

            return new Border
            {
                Tag = "Quote",
                BorderBrush = QuoteBorderBrush,
                BorderThickness = new Thickness(3, 0, 0, 0),
                Padding = new Thickness(10, 2, 0, 2),
                Child = textBlock
            };
        }

        private FrameworkElement CreateListBlock(MarkdownListBlock block)
        {
            var root = new StackPanel { Tag = "List" };
            UpdateListBlock(root, block);
            return root;
        }

        private void UpdateListBlock(StackPanel root, MarkdownListBlock block)
        {
            root.Children.Clear();
            for (var index = 0; index < block.Items.Count; index++)
            {
                var item = block.Items[index];
                var row = new Grid();
                row.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                if (index > 0)
                {
                    row.Margin = new Thickness(0, 4, 0, 0);
                }

                var marker = CreateBaseTextBlock();
                marker.Margin = new Thickness(0, 0, 10, 0);
                marker.FontWeight = FontWeights.SemiBold;
                marker.Text = item.Marker;

                var content = CreateBaseTextBlock();
                AddInlines(content.Inlines, item.Inlines);
                Grid.SetColumn(content, 1);

                row.Children.Add(marker);
                row.Children.Add(content);
                root.Children.Add(row);
            }
        }

        private Border CreateCodeBlock(MarkdownCodeBlock block)
        {
            var stackPanel = new StackPanel();
            var border = new Border
            {
                Tag = "Code",
                Background = CodeBackgroundBrush,
                BorderBrush = CodeBorderBrush,
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(10),
                Child = stackPanel
            };
            UpdateCodeBlock(border, block);
            return border;
        }

        private void UpdateCodeBlock(Border border, MarkdownCodeBlock block)
        {
            var stackPanel = (StackPanel)border.Child;
            stackPanel.Children.Clear();
            if (!string.IsNullOrWhiteSpace(block.Language))
            {
                var header = new DockPanel
                {
                    Margin = new Thickness(0, 0, 0, 8),
                    LastChildFill = false
                };

                var badgeBorder = new Border
                {
                    Background = BadgeBackgroundBrush,
                    BorderBrush = BadgeBorderBrush,
                    BorderThickness = new Thickness(1),
                    CornerRadius = new CornerRadius(6),
                    Padding = new Thickness(6, 2, 6, 2),
                    Child = new TextBlock
                    {
                        Text = block.Language,
                        Foreground = CodeForegroundBrush,
                        FontSize = Math.Max(11.0, ResolveBaseFontSize() - 2.0)
                    }
                };

                DockPanel.SetDock(badgeBorder, Dock.Right);
                header.Children.Add(badgeBorder);
                stackPanel.Children.Add(header);
            }

            stackPanel.Children.Add(new TextBlock
            {
                Text = block.Content,
                Foreground = CodeForegroundBrush,
                FontFamily = new FontFamily("Consolas"),
                FontSize = Math.Max(12.0, ResolveBaseFontSize() - 1.0),
                TextWrapping = TextWrapping.Wrap
            });
        }

        private Border CreateMathBlock(MarkdownMathBlock block)
        {
            var textBlock = new TextBlock
            {
                Foreground = MathForegroundBrush,
                FontFamily = new FontFamily("Consolas"),
                FontSize = Math.Max(12.0, ResolveBaseFontSize() - 0.5),
                TextWrapping = TextWrapping.Wrap,
                TextAlignment = TextAlignment.Center
            };

            var border = new Border
            {
                Tag = "Math",
                Background = MathBackgroundBrush,
                BorderBrush = MathBorderBrush,
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(10, 8, 10, 8),
                Child = textBlock
            };

            textBlock.Text = $"\\[{block.Content}\\]";
            return border;
        }

        private FrameworkElement CreateTableBlock(MarkdownTableBlock block)
        {
            var dataGrid = new DataGrid
            {
                Tag = "Table",
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Top,
                MaxHeight = TableMaxHeight,
                ColumnWidth = new DataGridLength(1.0, DataGridLengthUnitType.Star)
            };

            if (TryFindResource("TwilightBlue_DataGridStyle") is Style dataGridStyle)
            {
                dataGrid.Style = dataGridStyle;
            }

            dataGrid.Sorting += (s, e) => ApplyTableSort((DataGrid)s, e);
            UpdateTableBlock(dataGrid, block);
            return dataGrid;
        }

        private void UpdateTableBlock(DataGrid dataGrid, MarkdownTableBlock block)
        {
            bool columnsNeedRebuild = dataGrid.Columns.Count != block.Columns.Count;
            if (!columnsNeedRebuild)
            {
                for (int i = 0; i < block.Columns.Count; i++)
                {
                    var newHeader = block.Columns[i].Header;
                    if (string.IsNullOrWhiteSpace(newHeader))
                    {
                        newHeader = LF("Markdown.Table.ColumnFallbackFormat", "Column {0}", i + 1);
                    }
                    if (dataGrid.Columns[i].Header?.ToString() != newHeader)
                    {
                        columnsNeedRebuild = true;
                        break;
                    }
                }
            }

            if (columnsNeedRebuild)
            {
                dataGrid.Columns.Clear();
                for (var columnIndex = 0; columnIndex < block.Columns.Count; columnIndex++)
                {
                    var headerText = block.Columns[columnIndex].Header;
                    if (string.IsNullOrWhiteSpace(headerText))
                    {
                        headerText = LF("Markdown.Table.ColumnFallbackFormat", "Column {0}", columnIndex + 1);
                    }

                    var column = new DataGridTextColumn
                    {
                        Header = headerText,
                        Binding = new Binding($"Cells[{columnIndex}]"),
                        ClipboardContentBinding = new Binding($"Cells[{columnIndex}]"),
                        ElementStyle = IsStreaming ? TableCellStreamingTextStyle : TableCellTextStyle,
                        MinWidth = TableMinColumnWidth
                    };

                    dataGrid.Columns.Add(column);
                }
            }
            else
            {
                var expectedStyle = IsStreaming ? TableCellStreamingTextStyle : TableCellTextStyle;
                for (var columnIndex = 0; columnIndex < dataGrid.Columns.Count; columnIndex++)
                {
                    if (dataGrid.Columns[columnIndex] is DataGridTextColumn textColumn)
                    {
                        if (textColumn.ElementStyle != expectedStyle)
                        {
                            textColumn.ElementStyle = expectedStyle;
                        }
                    }
                }
            }

            dataGrid.ItemsSource = block.Rows.ToList();
        }

        private void AddInlines(InlineCollection collection, IEnumerable<MarkdownInline> inlines)
        {
            foreach (var inline in inlines)
            {
                switch (inline)
                {
                    case MarkdownTextInline text:
                        collection.Add(new Run(text.Text));
                        break;

                    case MarkdownStrongInline strong:
                    {
                        var bold = new Bold();
                        AddInlines(bold.Inlines, strong.Children);
                        collection.Add(bold);
                        break;
                    }

                    case MarkdownEmphasisInline emphasis:
                    {
                        var italic = new Italic();
                        AddInlines(italic.Inlines, emphasis.Children);
                        collection.Add(italic);
                        break;
                    }

                    case MarkdownCodeInline code:
                        collection.Add(CreateInlineCode(code.Content));
                        break;

                    case MarkdownLinkInline link:
                        collection.Add(CreateHyperlink(link));
                        break;

                    case MarkdownMathInline math:
                        collection.Add(CreateMathInline(math));
                        break;

                    case MarkdownLineBreakInline:
                        collection.Add(new LineBreak());
                        break;
                }
            }
        }

        private Inline CreateInlineCode(string code)
        {
            return new Run(code)
            {
                Background = InlineCodeBackgroundBrush,
                Foreground = InlineCodeForegroundBrush,
                FontFamily = new FontFamily("Consolas"),
                FontSize = Math.Max(12.0, ResolveBaseFontSize() - 1.0)
            };
        }

        private Hyperlink CreateHyperlink(MarkdownLinkInline link)
        {
            var hyperlink = new Hyperlink
            {
                Foreground = LinkForegroundBrush,
                TextDecorations = TextDecorations.Underline
            };

            if (Uri.TryCreate(link.Url, UriKind.Absolute, out var navigateUri))
            {
                hyperlink.NavigateUri = navigateUri;
            }

            ToolTipService.SetToolTip(hyperlink, link.Url);
            hyperlink.Click += (_, _) => TryOpenLink(link.Url);
            AddInlines(hyperlink.Inlines, link.Label);
            return hyperlink;
        }

        private Inline CreateMathInline(MarkdownMathInline math)
        {
            var delimiter = math.IsDisplayStyle ? ('[', ']') : ('(', ')');
            return new Run($"\\{delimiter.Item1}{math.Content}\\{delimiter.Item2}")
            {
                Foreground = MathForegroundBrush,
                FontFamily = new FontFamily("Consolas")
            };
        }

        private TextBlock CreateBaseTextBlock()
        {
            return new TextBlock
            {
                Foreground = ResolveForeground(),
                FontFamily = ResolveFontFamily(),
                FontSize = ResolveBaseFontSize(),
                FontWeight = FontWeight,
                FontStyle = FontStyle,
                TextWrapping = TextWrapping.Wrap
            };
        }

        private double ResolveBaseFontSize()
        {
            return FontSize > 0 ? FontSize : 14.0;
        }

        private double ResolveHeadingFontSize(int level)
        {
            var baseSize = ResolveBaseFontSize();
            return level switch
            {
                1 => baseSize + 8.0,
                2 => baseSize + 6.0,
                3 => baseSize + 4.0,
                4 => baseSize + 2.0,
                5 => baseSize + 1.0,
                _ => baseSize
            };
        }

        private Brush ResolveForeground()
        {
            return Foreground ?? Brushes.White;
        }

        private FontFamily ResolveFontFamily()
        {
            return FontFamily ?? new FontFamily("Segoe UI");
        }

        private static Thickness AppendBottomMargin(Thickness margin, double bottom)
        {
            return new Thickness(margin.Left, margin.Top, margin.Right, margin.Bottom + bottom);
        }

        private static Brush CreateFrozenBrush(Color color)
        {
            var brush = new SolidColorBrush(color);
            brush.Freeze();
            return brush;
        }

        private static void TryOpenLink(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return;
            }

            try
            {
                Process.Start(new ProcessStartInfo(url)
                {
                    UseShellExecute = true
                });
            }
            catch
            {
                // Ignore navigation failures and keep the content readable.
            }
        }

        private static void ApplyTableSort(
            DataGrid dataGrid,
            DataGridSortingEventArgs e)
        {
            e.Handled = true;
            if (dataGrid.ItemsSource is null)
            {
                return;
            }

            var columnIndex = dataGrid.Columns.IndexOf(e.Column);
            if (columnIndex < 0)
            {
                return;
            }

            var direction = e.Column.SortDirection == ListSortDirection.Ascending
                ? ListSortDirection.Descending
                : ListSortDirection.Ascending;

            foreach (var column in dataGrid.Columns)
            {
                if (!ReferenceEquals(column, e.Column))
                {
                    column.SortDirection = null;
                }
            }

            e.Column.SortDirection = direction;
            var rows = dataGrid.ItemsSource.Cast<MarkdownTableRow>();
            dataGrid.ItemsSource = direction == ListSortDirection.Ascending
                ? rows.OrderBy(row => row.GetCell(columnIndex), StringComparer.CurrentCultureIgnoreCase).ToList()
                : rows.OrderByDescending(row => row.GetCell(columnIndex), StringComparer.CurrentCultureIgnoreCase).ToList();
        }

        private static Style CreateTableCellTextStyle(TextWrapping textWrapping)
        {
            var style = new Style(typeof(TextBlock));
            style.Setters.Add(new Setter(TextBlock.TextWrappingProperty, textWrapping));
            style.Setters.Add(new Setter(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Center));
            return style;
        }

        private static string L(string resourceKey, string fallback)
        {
            return LocalizationRuntime.Instance.GetString(resourceKey, fallback);
        }

        private static string LF(string resourceKey, string fallbackFormat, params object?[] args)
        {
            return string.Format(L(resourceKey, fallbackFormat), args);
        }
    }
}
