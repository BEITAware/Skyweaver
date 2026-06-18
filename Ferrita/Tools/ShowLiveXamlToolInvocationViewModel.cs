using System.ComponentModel;
using System.Windows;
using Ferrita.Infrastructure.Mvvm;
using Ferrita.Services.Localization;
using Ferrita.Services.LiveXaml;
using Ferrita.Services.FerritaTools;

namespace Ferrita.Tools
{
    public sealed class ShowLiveXamlToolInvocationViewModel : ObservableObject
    {
        private readonly FerritaToolInvocationPresentationState _state;
        private readonly FerritaToolInvocationParameterPresentationState _filePathParameter;
        private string _filePathText;
        private string _statusText;
        private string _hintText;
        private string _diagnosticsText;
        private FrameworkElement? _previewElement;
        private string? _lastResolvedFilePath;
        private bool _lastAttemptWasClosed;

        public ShowLiveXamlToolInvocationViewModel(FerritaToolInvocationPresentationState state)
        {
            _state = state ?? throw new ArgumentNullException(nameof(state));
            _filePathParameter = _state.GetOrCreateParameterState("XAMLFilePath");
            _filePathText = string.Empty;
            _statusText = L("ShowLiveXaml.Status.WaitingForClose", "Waiting for the ShowLiveXAML call to finish streaming.");
            _hintText = L("ShowLiveXaml.Hint.WaitingForClose", "The preview will appear here after ShowLiveXAML closes with a full absolute .xaml path.");
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
            if (e.PropertyName is nameof(FerritaToolInvocationPresentationState.IsInvocationClosed)
                or nameof(FerritaToolInvocationParameterPresentationState.Value)
                or nameof(FerritaToolInvocationParameterPresentationState.IsClosed))
            {
                RefreshPreview();
            }
        }

        private void RefreshPreview()
        {
            var requestedPath = _filePathParameter.Value?.Trim() ?? string.Empty;
            FilePathText = requestedPath.Length == 0
                ? L("ShowLiveXaml.FilePath.Missing", "(no file path yet)")
                : requestedPath;

            if (requestedPath.Length == 0)
            {
                PreviewElement = null;
                DiagnosticsText = string.Empty;
                StatusText = L("ShowLiveXaml.Status.WaitingForPath", "Waiting for a XAML file path.");
                HintText = L("ShowLiveXaml.Hint.UseInitializePath", "Pass the full absolute .xaml path returned by InitializeLiveXAML.");
                _lastResolvedFilePath = null;
                _lastAttemptWasClosed = _state.IsInvocationClosed;
                return;
            }

            if (!_state.IsInvocationClosed && !_filePathParameter.IsClosed)
            {
                StatusText = L("ShowLiveXaml.Status.StreamingArguments", "ShowLiveXAML is still streaming its arguments.");
                HintText = L("ShowLiveXaml.Hint.RenderAfterClose", "The preview will render after the tool call closes.");
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
                    ? L("ShowLiveXaml.Status.Rendered", "Rendered a fresh LiveXAML preview instance.")
                    : previewResult.Summary;
                HintText = previewResult.IsSuccess
                    ? L("ShowLiveXaml.Hint.Rendered", "Each ShowLiveXAML call creates a new preview instance in this tool card.")
                    : L("ShowLiveXaml.Hint.FixAndRetry", "Fix the files, then call ShowLiveXAML again to render a new instance.");
                FilePathText = normalizedXamlFilePath;
            }
            catch (Exception ex)
            {
                PreviewElement = null;
                DiagnosticsText = ex.Message;
                StatusText = L("ShowLiveXaml.Status.InvalidPath", "The preview path is invalid.");
                HintText = L("ShowLiveXaml.Hint.ExactInitializePath", "Use the exact full .xaml path returned by InitializeLiveXAML.");
            }
        }

        private static string L(string resourceKey, string fallback)
        {
            return LocalizationRuntime.Instance.GetString(resourceKey, fallback);
        }
    }
}
