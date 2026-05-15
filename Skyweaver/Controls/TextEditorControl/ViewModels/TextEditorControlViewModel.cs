using System.Globalization;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using Skyweaver.Commands;
using Skyweaver.Infrastructure.Mvvm;

namespace Skyweaver.Controls.TextEditorControl.ViewModels
{
    public enum TextEditorDocumentMode
    {
        Default,
        PrintablePages
    }

    public sealed class TextEditorControlViewModel : ObservableObject
    {
        private const string UntitledFileName = "Untitled.txt";
        private static readonly Encoding Utf8NoBom = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
        private static readonly Encoding Utf8Bom = new UTF8Encoding(encoderShouldEmitUTF8Identifier: true);
        private static readonly Encoding StrictUtf8NoBom = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true);

        private string _title;
        private string _textContent = string.Empty;
        private string _documentName = UntitledFileName;
        private string? _filePath;
        private Encoding _currentEncoding = Utf8NoBom;
        private TextEditorDocumentMode _documentMode = TextEditorDocumentMode.Default;
        private bool _hasUnsavedChanges;
        private bool _isUpdatingDocument;
        private string _operationStatus = "Ready";
        private int _lineCount = 1;
        private int _characterCount;
        private string _lineNumbersText = "1";
        private GridLength _lineNumberColumnWidth = new(42);
        private int _caretLine = 1;
        private int _caretColumn = 1;
        private string _syntaxProfileName = "Plain text";

        public TextEditorControlViewModel(int instanceNumber)
        {
            _title = instanceNumber > 1 ? $"文本编辑器 {instanceNumber}" : "文本编辑器";

            NewDocumentCommand = new AsyncRelayCommand(NewDocumentAsync);
            OpenDocumentCommand = new AsyncRelayCommand(OpenDocumentAsync);
            SaveDocumentCommand = new AsyncRelayCommand(SaveDocumentAsync);
            SaveDocumentAsCommand = new AsyncRelayCommand(SaveDocumentAsAsync);

            RefreshTextStatistics();
        }

        public string Title
        {
            get => _title;
            private set
            {
                if (SetProperty(ref _title, value))
                {
                    RefreshStatusBindings();
                }
            }
        }

        public string TextContent
        {
            get => _textContent;
            set
            {
                var normalizedValue = value ?? string.Empty;
                if (!SetProperty(ref _textContent, normalizedValue))
                {
                    return;
                }

                RefreshTextStatistics();

                if (!_isUpdatingDocument)
                {
                    HasUnsavedChanges = true;
                    SetOperationStatus("Edited");
                }
            }
        }

        public TextEditorDocumentMode DocumentMode
        {
            get => _documentMode;
            private set
            {
                if (SetProperty(ref _documentMode, value))
                {
                    OnPropertyChanged(nameof(IsDefaultMode));
                    OnPropertyChanged(nameof(IsPrintablePagesMode));
                    OnPropertyChanged(nameof(ModeDisplayName));
                    OnPropertyChanged(nameof(ModeSummaryText));
                    RefreshStatusBindings();
                }
            }
        }

        public bool IsDefaultMode => DocumentMode == TextEditorDocumentMode.Default;

        public bool IsPrintablePagesMode => DocumentMode == TextEditorDocumentMode.PrintablePages;

        public bool HasUnsavedChanges
        {
            get => _hasUnsavedChanges;
            private set
            {
                if (SetProperty(ref _hasUnsavedChanges, value))
                {
                    OnPropertyChanged(nameof(DocumentTitle));
                    RefreshStatusBindings();
                }
            }
        }

        public string DocumentTitle => HasUnsavedChanges ? $"{_documentName} *" : _documentName;

        public string DocumentSummary
        {
            get
            {
                var location = string.IsNullOrWhiteSpace(_filePath) ? "Unsaved" : _filePath;
                return $"{location} | {ModeDisplayName} | {EncodingDisplayName}";
            }
        }

        public string HeaderStatusText => $"{Title} | {DocumentTitle} | {ModeSummaryText}";

        public string StatusText
        {
            get
            {
                var dirtyState = HasUnsavedChanges ? "Modified" : "Saved";
                return $"{dirtyState} | {_operationStatus}";
            }
        }

        public string WordCountText => $"{LineCount.ToString(CultureInfo.InvariantCulture)} lines | {CharacterCount.ToString(CultureInfo.InvariantCulture)} chars";

        public string LineColumnText => $"Ln {_caretLine.ToString(CultureInfo.InvariantCulture)}, Col {_caretColumn.ToString(CultureInfo.InvariantCulture)}";

        public string ModeDisplayName => DocumentMode == TextEditorDocumentMode.PrintablePages
            ? "PrintablePages"
            : "Default";

        public string ModeSummaryText => DocumentMode == TextEditorDocumentMode.PrintablePages
            ? "Markdown rich pages mode - backend pending"
            : $"Plain text/code mode - syntax profile: {SyntaxProfileName}";

        public string SyntaxProfileName
        {
            get => _syntaxProfileName;
            private set
            {
                if (SetProperty(ref _syntaxProfileName, value))
                {
                    OnPropertyChanged(nameof(ModeSummaryText));
                    RefreshStatusBindings();
                }
            }
        }

        public int LineCount
        {
            get => _lineCount;
            private set
            {
                if (SetProperty(ref _lineCount, value))
                {
                    OnPropertyChanged(nameof(WordCountText));
                }
            }
        }

        public int CharacterCount
        {
            get => _characterCount;
            private set
            {
                if (SetProperty(ref _characterCount, value))
                {
                    OnPropertyChanged(nameof(WordCountText));
                }
            }
        }

        public string LineNumbersText
        {
            get => _lineNumbersText;
            private set => SetProperty(ref _lineNumbersText, value);
        }

        public GridLength LineNumberColumnWidth
        {
            get => _lineNumberColumnWidth;
            private set => SetProperty(ref _lineNumberColumnWidth, value);
        }

        public string EncodingDisplayName => _currentEncoding.WebName.ToUpperInvariant();

        public ICommand NewDocumentCommand { get; }

        public ICommand OpenDocumentCommand { get; }

        public ICommand SaveDocumentCommand { get; }

        public ICommand SaveDocumentAsCommand { get; }

        public void UpdateCaretPosition(int line, int column)
        {
            var normalizedLine = Math.Max(1, line);
            var normalizedColumn = Math.Max(1, column);

            if (_caretLine == normalizedLine && _caretColumn == normalizedColumn)
            {
                return;
            }

            _caretLine = normalizedLine;
            _caretColumn = normalizedColumn;
            OnPropertyChanged(nameof(LineColumnText));
        }

        private async Task NewDocumentAsync()
        {
            if (!await EnsureCanReplaceDocumentAsync().ConfigureAwait(true))
            {
                return;
            }

            LoadDocumentState(
                text: string.Empty,
                filePath: null,
                encoding: Utf8NoBom,
                mode: TextEditorDocumentMode.Default,
                operationStatus: "New document");
        }

        private async Task OpenDocumentAsync()
        {
            if (!await EnsureCanReplaceDocumentAsync().ConfigureAwait(true))
            {
                return;
            }

            var dialog = new OpenFileDialog
            {
                Title = "打开文本文件",
                Filter = BuildDocumentFilter(),
                CheckFileExists = true,
                Multiselect = false
            };

            if (dialog.ShowDialog(Application.Current?.MainWindow) != true)
            {
                SetOperationStatus("Open canceled");
                return;
            }

            await LoadDocumentFromPathAsync(dialog.FileName).ConfigureAwait(true);
        }

        private async Task SaveDocumentAsync()
        {
            await SaveDocumentWithResultAsync().ConfigureAwait(true);
        }

        private async Task SaveDocumentAsAsync()
        {
            await SaveDocumentAsWithResultAsync().ConfigureAwait(true);
        }

        private async Task<bool> EnsureCanReplaceDocumentAsync()
        {
            if (!HasUnsavedChanges)
            {
                return true;
            }

            var result = MessageBox.Show(
                Application.Current?.MainWindow,
                "当前文档包含未保存的修改。是否先保存？",
                "文本编辑器",
                MessageBoxButton.YesNoCancel,
                MessageBoxImage.Warning);

            return result switch
            {
                MessageBoxResult.Yes => await SaveDocumentWithResultAsync().ConfigureAwait(true),
                MessageBoxResult.No => true,
                _ => false
            };
        }

        private async Task LoadDocumentFromPathAsync(string filePath)
        {
            try
            {
                var readResult = await Task.Run(() => ReadTextFile(filePath)).ConfigureAwait(true);
                LoadDocumentState(
                    readResult.Text,
                    filePath,
                    readResult.Encoding,
                    DetermineMode(filePath),
                    $"Opened {Path.GetFileName(filePath)}");
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or DecoderFallbackException or ArgumentException)
            {
                MessageBox.Show(
                    Application.Current?.MainWindow,
                    ex.Message,
                    "打开文本文件",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                SetOperationStatus("Open failed");
            }
        }

        private async Task<bool> SaveDocumentWithResultAsync()
        {
            if (string.IsNullOrWhiteSpace(_filePath))
            {
                return await SaveDocumentAsWithResultAsync().ConfigureAwait(true);
            }

            return await SaveDocumentToPathAsync(_filePath).ConfigureAwait(true);
        }

        private async Task<bool> SaveDocumentAsWithResultAsync()
        {
            var dialog = new SaveFileDialog
            {
                Title = "保存文本文件",
                Filter = BuildDocumentFilter(),
                AddExtension = true,
                DefaultExt = IsPrintablePagesMode ? ".smd" : ".txt",
                FileName = string.IsNullOrWhiteSpace(_filePath) ? _documentName : Path.GetFileName(_filePath)
            };

            var initialDirectory = SafeGetDirectoryName(_filePath);
            if (!string.IsNullOrWhiteSpace(initialDirectory) && Directory.Exists(initialDirectory))
            {
                dialog.InitialDirectory = initialDirectory;
            }

            if (dialog.ShowDialog(Application.Current?.MainWindow) != true)
            {
                SetOperationStatus("Save canceled");
                return false;
            }

            return await SaveDocumentToPathAsync(dialog.FileName).ConfigureAwait(true);
        }

        private async Task<bool> SaveDocumentToPathAsync(string filePath)
        {
            try
            {
                await File.WriteAllTextAsync(filePath, TextContent, _currentEncoding).ConfigureAwait(true);

                _filePath = filePath;
                _documentName = Path.GetFileName(filePath);
                DocumentMode = DetermineMode(filePath);
                SyntaxProfileName = DetermineSyntaxProfile(filePath);
                HasUnsavedChanges = false;

                OnPropertyChanged(nameof(DocumentTitle));
                OnPropertyChanged(nameof(DocumentSummary));
                OnPropertyChanged(nameof(EncodingDisplayName));
                SetOperationStatus($"Saved {Path.GetFileName(filePath)}");

                return true;
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or ArgumentException)
            {
                MessageBox.Show(
                    Application.Current?.MainWindow,
                    ex.Message,
                    "保存文本文件",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                SetOperationStatus("Save failed");
                return false;
            }
        }

        private void LoadDocumentState(string text, string? filePath, Encoding encoding, TextEditorDocumentMode mode, string operationStatus)
        {
            _isUpdatingDocument = true;
            try
            {
                TextContent = text;
            }
            finally
            {
                _isUpdatingDocument = false;
            }

            _filePath = filePath;
            _documentName = string.IsNullOrWhiteSpace(filePath) ? UntitledFileName : Path.GetFileName(filePath);
            _currentEncoding = encoding;
            DocumentMode = mode;
            SyntaxProfileName = DetermineSyntaxProfile(filePath);
            HasUnsavedChanges = false;

            OnPropertyChanged(nameof(DocumentTitle));
            OnPropertyChanged(nameof(DocumentSummary));
            OnPropertyChanged(nameof(EncodingDisplayName));
            SetOperationStatus(operationStatus);
            UpdateCaretPosition(1, 1);
        }

        private void RefreshTextStatistics()
        {
            var lineCount = CountLogicalLines(TextContent);
            LineCount = lineCount;
            CharacterCount = TextContent.Length;
            LineNumbersText = BuildLineNumbersText(lineCount);

            var digits = lineCount.ToString(CultureInfo.InvariantCulture).Length;
            LineNumberColumnWidth = new GridLength(Math.Max(42, 24 + digits * 8));
        }

        private void SetOperationStatus(string status)
        {
            if (_operationStatus == status)
            {
                return;
            }

            _operationStatus = status;
            RefreshStatusBindings();
        }

        private void RefreshStatusBindings()
        {
            OnPropertyChanged(nameof(HeaderStatusText));
            OnPropertyChanged(nameof(StatusText));
            OnPropertyChanged(nameof(DocumentSummary));
        }

        private static TextEditorDocumentMode DetermineMode(string? filePath)
        {
            return string.Equals(Path.GetExtension(filePath), ".smd", StringComparison.OrdinalIgnoreCase)
                ? TextEditorDocumentMode.PrintablePages
                : TextEditorDocumentMode.Default;
        }

        private static string DetermineSyntaxProfile(string? filePath)
        {
            return Path.GetExtension(filePath)?.ToLowerInvariant() switch
            {
                ".cs" => "C#",
                ".xaml" => "XAML",
                ".xml" => "XML",
                ".json" => "JSON",
                ".md" => "Markdown",
                ".ps1" => "PowerShell",
                ".js" => "JavaScript",
                ".ts" => "TypeScript",
                ".html" => "HTML",
                ".css" => "CSS",
                ".smd" => "Skyweaver Markdown",
                _ => "Plain text"
            };
        }

        private static int CountLogicalLines(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return 1;
            }

            var count = 1;
            foreach (var character in text)
            {
                if (character == '\n')
                {
                    count++;
                }
            }

            return count;
        }

        private static string BuildLineNumbersText(int lineCount)
        {
            var builder = new StringBuilder(lineCount * 4);
            for (var line = 1; line <= lineCount; line++)
            {
                if (line > 1)
                {
                    builder.AppendLine();
                }

                builder.Append(line.ToString(CultureInfo.InvariantCulture));
            }

            return builder.ToString();
        }

        private static TextReadResult ReadTextFile(string filePath)
        {
            var bytes = File.ReadAllBytes(filePath);
            if (bytes.Length == 0)
            {
                return new TextReadResult(string.Empty, Utf8NoBom);
            }

            if (HasPrefix(bytes, [0xEF, 0xBB, 0xBF]))
            {
                return DecodeWithPreamble(bytes, Utf8Bom, 3);
            }

            if (HasPrefix(bytes, [0xFF, 0xFE, 0x00, 0x00]))
            {
                return DecodeWithPreamble(bytes, Encoding.UTF32, 4);
            }

            if (HasPrefix(bytes, [0xFF, 0xFE]))
            {
                return DecodeWithPreamble(bytes, Encoding.Unicode, 2);
            }

            if (HasPrefix(bytes, [0xFE, 0xFF]))
            {
                return DecodeWithPreamble(bytes, Encoding.BigEndianUnicode, 2);
            }

            try
            {
                return new TextReadResult(StrictUtf8NoBom.GetString(bytes), Utf8NoBom);
            }
            catch (DecoderFallbackException)
            {
                return new TextReadResult(Encoding.Default.GetString(bytes), Encoding.Default);
            }
        }

        private static TextReadResult DecodeWithPreamble(byte[] bytes, Encoding encoding, int preambleLength)
        {
            return new TextReadResult(
                encoding.GetString(bytes, preambleLength, bytes.Length - preambleLength),
                encoding);
        }

        private static bool HasPrefix(byte[] bytes, byte[] prefix)
        {
            if (bytes.Length < prefix.Length)
            {
                return false;
            }

            for (var index = 0; index < prefix.Length; index++)
            {
                if (bytes[index] != prefix[index])
                {
                    return false;
                }
            }

            return true;
        }

        private static string BuildDocumentFilter()
        {
            return "Text and code files|*.txt;*.md;*.cs;*.xaml;*.xml;*.json;*.ps1;*.js;*.ts;*.html;*.css;*.ini;*.log|" +
                   "Skyweaver Markdown (*.smd)|*.smd|" +
                   "All files (*.*)|*.*";
        }

        private static string? SafeGetDirectoryName(string? filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                return null;
            }

            try
            {
                return Path.GetDirectoryName(filePath);
            }
            catch (ArgumentException)
            {
                return null;
            }
        }

        private readonly record struct TextReadResult(string Text, Encoding Encoding);
    }
}
