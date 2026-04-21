using Skyweaver.Infrastructure.Mvvm;

namespace Skyweaver.Panels.DocumentWorkspace.Models
{
    public sealed class WorkspaceDocument : ObservableObject
    {
        public const string DefaultIconPath = "pack://application:,,,/Resources/image.png";

        public string Id { get; } = Guid.NewGuid().ToString();

        public string DocumentKey { get; init; } = string.Empty;

        private string _title = string.Empty;
        private string _subtitle = string.Empty;
        private string _iconPath = DefaultIconPath;
        private string _placeholderText = "Document content here...";
        private object? _contentViewModel;
        private int? _instanceNumber;
        private string _displayTitle = string.Empty;

        public string TabTypeKey { get; init; } = string.Empty;

        public int? InstanceNumber
        {
            get => _instanceNumber;
            set => SetProperty(ref _instanceNumber, value);
        }

        public string DisplayTitle
        {
            get => string.IsNullOrEmpty(_displayTitle) ? _title : _displayTitle;
            private set => SetProperty(ref _displayTitle, value);
        }

        public void RefreshDisplayTitle(bool showNumber)
        {
            DisplayTitle = showNumber && InstanceNumber.HasValue
                ? $"{Title} {InstanceNumber.Value}"
                : Title;
        }

        public string Title
        {
            get => _title;
            set
            {
                if (SetProperty(ref _title, value))
                {
                    OnPropertyChanged(nameof(DisplayTitle));
                }
            }
        }

        public string Subtitle
        {
            get => _subtitle;
            set => SetProperty(ref _subtitle, value);
        }

        public string IconPath
        {
            get => _iconPath;
            set => SetProperty(ref _iconPath, value);
        }

        public string PlaceholderText
        {
            get => _placeholderText;
            set => SetProperty(ref _placeholderText, value);
        }

        public object? ContentViewModel
        {
            get => _contentViewModel;
            set => SetProperty(ref _contentViewModel, value);
        }
    }
}
