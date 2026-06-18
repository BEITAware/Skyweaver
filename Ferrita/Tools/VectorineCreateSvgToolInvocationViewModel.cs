using System;
using System.ComponentModel;
using System.Windows.Media;
using System.Xml.Linq;
using Ferrita.Infrastructure.Mvvm;
using Ferrita.Services.FerritaTools;

namespace Ferrita.Tools
{
    /// <summary>
    /// VectorineCreateSVG 工具调用卡片的 ViewModel
    /// </summary>
    public sealed class VectorineCreateSvgToolInvocationViewModel : ObservableObject
    {
        private readonly FerritaToolInvocationPresentationState _state;
        private readonly FerritaToolInvocationParameterPresentationState _titleParameter;
        private readonly FerritaToolInvocationParameterPresentationState _backgroundParameter;
        private readonly FerritaToolInvocationParameterPresentationState _svgParameter;

        private string _titleText = string.Empty;
        private string _backgroundPath = string.Empty;
        private string _svgText = string.Empty;
        private bool _showPreview;
        private bool _showCode = true;
        private ImageSource? _renderedImage;

        public VectorineCreateSvgToolInvocationViewModel(FerritaToolInvocationPresentationState state)
        {
            _state = state ?? throw new ArgumentNullException(nameof(state));
            _titleParameter = _state.GetOrCreateParameterState("Title");
            _backgroundParameter = _state.GetOrCreateParameterState("Background");
            _svgParameter = _state.GetOrCreateParameterState("SVG");

            _state.PropertyChanged += HandlePropertyChanged;
            _titleParameter.PropertyChanged += HandlePropertyChanged;
            _backgroundParameter.PropertyChanged += HandlePropertyChanged;
            _svgParameter.PropertyChanged += HandlePropertyChanged;

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

        public string SvgText
        {
            get => _svgText;
            private set => SetProperty(ref _svgText, value);
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

        public ImageSource? RenderedImage
        {
            get => _renderedImage;
            set => SetProperty(ref _renderedImage, value);
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

            var svg = _svgParameter.Value ?? string.Empty;
            SvgText = svg;

            bool isClosed = _state.IsInvocationClosed || _svgParameter.IsClosed;
            bool isValid = IsValidSvg(svg);

            if (!isValid && !isClosed)
            {
                ShowPreview = false;
                ShowCode = true;
                RenderedImage = null;
            }
            else
            {
                if (isValid)
                {
                    ShowPreview = true;
                    ShowCode = false;
                    try
                    {
                        // 原生同步转换 SVG 为 WPF DrawingImage，无需 WebBrowser 或临时网页
                        RenderedImage = SvgToWpfConverter.Convert(svg);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Failed to convert SVG natively: {ex}");
                        RenderedImage = null;
                    }
                }
                else
                {
                    ShowPreview = false;
                    ShowCode = true;
                    RenderedImage = null;
                }
            }
        }

        private static bool IsValidSvg(string svg)
        {
            if (string.IsNullOrWhiteSpace(svg)) return false;
            var trimmed = svg.Trim();

            // 快速验证是否符合 SVG 基本闭合 XML 格式
            if (!trimmed.StartsWith("<") || !trimmed.EndsWith(">")) return false;
            if (!trimmed.Contains("</svg>", StringComparison.OrdinalIgnoreCase)) return false;

            try
            {
                var doc = XDocument.Parse(trimmed);
                return doc.Root != null && string.Equals(doc.Root.Name.LocalName, "svg", StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
        }
    }
}
