using Ferrita.Infrastructure.Mvvm;
using Ferrita.Models.ChatSession;

namespace Ferrita.Panels.SessionList.Models
{
    public sealed class SessionListItem : ObservableObject
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        private string _title = string.Empty;
        private string _timeLabel = string.Empty;
        private string _iconPath = "pack://application:,,,/Resources/image.png";

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public string TimeLabel
        {
            get => _timeLabel;
            set => SetProperty(ref _timeLabel, value);
        }

        public string IconPath
        {
            get => _iconPath;
            set => SetProperty(ref _iconPath, value);
        }

        public ChatSessionModel? Session { get; set; }
    }
}
