using System.ComponentModel;
using System.Windows;
using Skyweaver.Infrastructure.Mvvm;
using Skyweaver.Services.LiveXaml;
using Skyweaver.Services.SkyweaverTools;

namespace Skyweaver.Tools
{
    public sealed class ShowLiveXamlToolInvocationViewModel : ObservableObject
    {
        private readonly SkyweaverToolInvocationPresentationState _state;
        private readonly SkyweaverToolInvocationParameterPresentationState _filePathParameter;
        private string _filePathText;
        private string _statusText;
        private string _hintText;
        private string _diagnosticsText;
        private FrameworkElement? _previewElement;
        private string? _lastResolvedFilePath;
        private bool _lastAttemptWasClosed;

        public ShowLiveXamlToolInvocationViewModel(SkyweaverToolInvocationPresentationState state)
        {
            _state = state ?? throw new ArgumentNullException(nameof(state));
            _filePathParameter = _state.GetOrCreateParameterState("XAMLFilePath");
            _filePathText = string.Empty;
            _statusText = "Waiting for the ShowLiveXAML call to finish streaming.";
            _hintText = "The preview will appear here after ShowLiveXAML closes with a full absolute .xaml path.";
            _diagnosticsText = string.Empty;

            _state.PropertyChanged += HandleSourcePropertyChanged;
            _filePathParameter.PropertyChanged += HandleSourcePropertyChanged;
            RefreshPreview();
        }

        public string FilePathText
        {
            get => _filePathText;
            private set => SetProperty(ref _filePathText, value);
        }

        public string StatusText
        {
            get => _statusText;
            private set => SetProperty(ref _statusText, value);
        }

        public string HintText
        {
            get => _hintText;
            private set => SetProperty(ref _hintText, value);
        }

        public string DiagnosticsText
        {
            get => _diagnosticsText;
            private set
            {
                if (SetProperty(ref _diagnosticsText, value))
                {
                    OnPropertyChanged(nameof(HasDiagnostics));
                }
            }
        }

        public FrameworkElement? PreviewElement
        {
            get => _previewElement;
            private set
            {
                if (SetProperty(ref _previewElement, value))
                {
                    OnPropertyChanged(nameof(HasPreview));
                }
            }
        }

        public bool HasPreview => PreviewElement != null;

        public bool HasDiagnostics => !string.IsNullOrWhiteSpace(DiagnosticsText);

        private void HandleSourcePropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName is nameof(SkyweaverToolInvocationPresentationState.IsInvocationClosed)
                or nameof(SkyweaverToolInvocationParameterPresentationState.Value)
                or nameof(SkyweaverToolInvocationParameterPresentationState.IsClosed))
            {
                RefreshPreview();
            }
        }

        private void RefreshPreview()
        {
            var requestedPath = _filePathParameter.Value?.Trim() ?? string.Empty;
            FilePathText = requestedPath.Length == 0 ? "(no file path yet)" : requestedPath;

            if (requestedPath.Length == 0)
            {
                PreviewElement = null;
                DiagnosticsText = string.Empty;
                StatusText = "Waiting for a XAML file path.";
                HintText = "Pass the full absolute .xaml path returned by InitializeLiveXAML.";
                _lastResolvedFilePath = null;
                _lastAttemptWasClosed = _state.IsInvocationClosed;
                return;
            }

            if (!_state.IsInvocationClosed && !_filePathParameter.IsClosed)
            {
                StatusText = "ShowLiveXAML is still streaming its arguments.";
                HintText = "The preview will render after the tool call closes.";
                return;
            }

            if (_lastResolvedFilePath != null &&
                string.Equals(_lastResolvedFilePath, requestedPath, StringComparison.OrdinalIgnoreCase) &&
                _lastAttemptWasClosed == _state.IsInvocationClosed)
            {
                return;
            }

            _lastResolvedFilePath = requestedPath;
            _lastAttemptWasClosed = _state.IsInvocationClosed;

            try
            {
                var normalizedXamlFilePath = LiveXamlFileSupport.NormalizeAbsoluteXamlPath(requestedPath);
                var previewResult = LiveXamlRuntime.LoadPreview(normalizedXamlFilePath);
                PreviewElement = previewResult.View;
                DiagnosticsText = previewResult.BuildDiagnosticsText();
                StatusText = previewResult.IsSuccess
                    ? "Rendered a fresh LiveXAML preview instance."
                    : previewResult.Summary;
                HintText = previewResult.IsSuccess
                    ? "Each ShowLiveXAML call creates a new preview instance in this tool card."
                    : "Fix the files, then call ShowLiveXAML again to render a new instance.";
                FilePathText = normalizedXamlFilePath;
            }
            catch (Exception ex)
            {
                PreviewElement = null;
                DiagnosticsText = ex.Message;
                StatusText = "The preview path is invalid.";
                HintText = "Use the exact full .xaml path returned by InitializeLiveXAML.";
            }
        }
    }
}
