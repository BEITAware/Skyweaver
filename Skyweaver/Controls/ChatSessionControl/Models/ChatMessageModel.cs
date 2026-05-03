using System.Collections.ObjectModel;
using Skyweaver.Infrastructure.Mvvm;

namespace Skyweaver.Controls.ChatSessionControl.Models
{
    public class ChatMessageModel : ObservableObject
    {
        private ChatMessageRole _role;
        private string _displayName;
        private string _avatarPath;
        private DateTime _timestamp;
        private string? _sourceEntryId;

        public Guid Id { get; } = Guid.NewGuid();

        public ChatMessageRole Role
        {
            get => _role;
            set => SetProperty(ref _role, value);
        }

        public string DisplayName
        {
            get => _displayName;
            set => SetProperty(ref _displayName, value);
        }

        public string AvatarPath
        {
            get => _avatarPath;
            set => SetProperty(ref _avatarPath, value);
        }

        public DateTime Timestamp
        {
            get => _timestamp;
            set
            {
                if (SetProperty(ref _timestamp, value))
                {
                    OnPropertyChanged(nameof(TimestampText));
                }
            }
        }

        public string TimestampText => Timestamp.ToString("HH:mm");

        public ObservableCollection<ChatMessagePartModel> Parts { get; } = new();

        public ObservableCollection<string> SourceEntryIds { get; } = new();

        public string? SourceEntryId
        {
            get => _sourceEntryId;
            set => SetProperty(ref _sourceEntryId, string.IsNullOrWhiteSpace(value) ? null : value.Trim());
        }

        public ChatMessageModel(
            ChatMessageRole role,
            string displayName,
            string avatarPath,
            DateTime timestamp,
            IEnumerable<ChatMessagePartModel>? parts = null,
            string? sourceEntryId = null)
        {
            _role = role;
            _displayName = displayName;
            _avatarPath = avatarPath;
            _timestamp = timestamp;
            _sourceEntryId = string.IsNullOrWhiteSpace(sourceEntryId) ? null : sourceEntryId.Trim();
            if (_sourceEntryId != null)
            {
                SourceEntryIds.Add(_sourceEntryId);
            }

            if (parts == null)
            {
                return;
            }

            foreach (var part in parts)
            {
                Parts.Add(part);
            }
        }
    }
}
