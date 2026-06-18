using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Markup;
using Ferrita.Infrastructure.Mvvm;
using Ferrita.Services.FerritaTools;

namespace Ferrita.Tools
{
    /// <summary>
    /// VectorineCreateXAML 工具调用卡片的 ViewModel
    /// </summary>
    public sealed class VectorineCreateXamlToolInvocationViewModel : ObservableObject
    {
        private readonly FerritaToolInvocationPresentationState _state;
        private readonly FerritaToolInvocationParameterPresentationState _titleParameter;
        private readonly FerritaToolInvocationParameterPresentationState _backgroundParameter;
        private readonly FerritaToolInvocationParameterPresentationState _xamlParameter;

        private string _titleText = string.Empty;
        private string _backgroundPath = string.Empty;
        private string _xamlText = string.Empty;
        private bool _showPreview;
        private bool _showCode = true;
        private UIElement? _renderedContent;

        public VectorineCreateXamlToolInvocationViewModel(FerritaToolInvocationPresentationState state)
        {
            _state = state ?? throw new ArgumentNullException(nameof(state));
            _titleParameter = _state.GetOrCreateParameterState("Title");
            _backgroundParameter = _state.GetOrCreateParameterState("Background");
            _xamlParameter = _state.GetOrCreateParameterState("XAML");

            _state.PropertyChanged += HandlePropertyChanged;
            _titleParameter.PropertyChanged += HandlePropertyChanged;
            _backgroundParameter.PropertyChanged += HandlePropertyChanged;
            _xamlParameter.PropertyChanged += HandlePropertyChanged;

            RefreshState();
        }

        public string TitleText
        {
            get => _titleText;
            private set
            {
                if (SetProperty(ref _titleText, value))
                {
                    OnPropertyChanged(nameof(HasTitle));
                }
            }
        }

        public bool HasTitle => !string.IsNullOrWhiteSpace(TitleText);

        public string BackgroundPath
        {
            get => _backgroundPath;
            private set
            {
                if (SetProperty(ref _backgroundPath, value))
                {
                    OnPropertyChanged(nameof(IsWhiteBackground));
                }
            }
        }

        public bool IsWhiteBackground => string.Equals(BackgroundPath, "White", StringComparison.OrdinalIgnoreCase);

        public string XamlText
        {
            get => _xamlText;
            private set => SetProperty(ref _xamlText, value);
        }

        public bool ShowPreview
        {
            get => _showPreview;
            private set => SetProperty(ref _showPreview, value);
        }

        public bool ShowCode
        {
            get => _showCode;
            private set => SetProperty(ref _showCode, value);
        }

        public UIElement? RenderedContent
        {
            get => _renderedContent;
            set => SetProperty(ref _renderedContent, value);
        }

        private void HandlePropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName is nameof(FerritaToolInvocationPresentationState.IsInvocationClosed)
                or nameof(FerritaToolInvocationParameterPresentationState.Value)
                or nameof(FerritaToolInvocationParameterPresentationState.IsClosed))
            {
                RefreshState();
            }
        }

        private void RefreshState()
        {
            var title = _titleParameter.Value?.Trim() ?? string.Empty;
            TitleText = title;

            var background = _backgroundParameter.Value?.Trim() ?? string.Empty;
            BackgroundPath = background;

            var xaml = _xamlParameter.Value ?? string.Empty;
            XamlText = xaml;

            bool isClosed = _state.IsInvocationClosed || _xamlParameter.IsClosed;
            bool isReadyToRender = isClosed && !string.IsNullOrWhiteSpace(xaml);

            if (!isReadyToRender)
            {
                ShowPreview = false;
                ShowCode = true;
                RunOnUi(() => RenderedContent = null);
            }
            else
            {
                ShowPreview = true;
                ShowCode = false;
                RunOnUi(() =>
                {
                    try
                    {
                        RenderedContent = ParseXaml(xaml);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Failed to parse XAML: {ex}");
                        RenderedContent = new TextBlock
                        {
                            Text = $"解析 XAML 失败:\n{ex.Message}",
                            Foreground = Brushes.Red,
                            TextWrapping = TextWrapping.Wrap,
                            Margin = new Thickness(10),
                            FontFamily = new FontFamily("Consolas"),
                            FontSize = 12
                        };
                    }
                });
            }
        }

        private static void RunOnUi(Action action)
        {
            var dispatcher = Application.Current?.Dispatcher;
            if (dispatcher == null)
            {
                action();
                return;
            }

            if (dispatcher.CheckAccess())
            {
                action();
            }
            else
            {
                dispatcher.Invoke(action);
            }
        }

        private static UIElement? ParseXaml(string xamlText)
        {
            var cleaned = CleanXamlText(xamlText);
            if (string.IsNullOrWhiteSpace(cleaned))
            {
                throw new InvalidOperationException("XAML 内容为空或仅包含空白字符。");
            }

            // 快速验证 XML 基础格式，避免 XamlReader 内部抛出异常触发调试器中断
            if (!cleaned.StartsWith("<") || !cleaned.EndsWith(">"))
            {
                throw new InvalidOperationException("XAML 格式不正确：必须以 '<' 开头并以 '>' 结尾。请确保去除了多余的 Markdown 标记且 XAML 结构完整。");
            }

            object? obj = XamlReader.Parse(cleaned);

            if (obj is UIElement uiElement)
            {
                return uiElement;
            }
            else if (obj is Drawing drawing)
            {
                return new Image
                {
                    Source = new DrawingImage(drawing),
                    Stretch = Stretch.Uniform,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };
            }
            else if (obj is DrawingImage drawingImage)
            {
                return new Image
                {
                    Source = drawingImage,
                    Stretch = Stretch.Uniform,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };
            }

            throw new InvalidOperationException($"解析出的对象类型 '{obj?.GetType().FullName}' 不是支持的可视化类型 (UIElement/Drawing/DrawingImage)。");
        }

        private static string CleanXamlText(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return string.Empty;

            var cleaned = input.Trim();

            // 剥除 UTF-8 BOM 字符
            if (cleaned.Length > 0 && cleaned[0] == '\uFEFF')
            {
                cleaned = cleaned.Substring(1).Trim();
            }

            // 剥除 Markdown 格式包裹 ```xml ... ``` 或者是 ```xaml ... ```
            if (cleaned.StartsWith("```"))
            {
                int firstNewLine = cleaned.IndexOf('\n');
                if (firstNewLine != -1)
                {
                    cleaned = cleaned.Substring(firstNewLine + 1).Trim();
                }
                else
                {
                    cleaned = cleaned.Substring(3).Trim();
                }

                if (cleaned.EndsWith("```"))
                {
                    cleaned = cleaned.Substring(0, cleaned.Length - 3).Trim();
                }
            }

            return cleaned;
        }
    }
}
