using System.Collections.ObjectModel;
using Skyweaver.Infrastructure.Mvvm;

namespace Skyweaver.Controls.ChatSessionControl.Models
{
    public sealed class ChatStructuredXmlNodeModel : ObservableObject
    {
        private string _name = string.Empty;
        private string _value = string.Empty;

        public ChatStructuredXmlNodeModel()
        {
            Children.CollectionChanged += (_, _) =>
            {
                OnPropertyChanged(nameof(HasChildren));
                OnPropertyChanged(nameof(DisplayText));
            };
        }

        public string Name
        {
            get => _name;
            set
            {
                if (SetProperty(ref _name, value ?? string.Empty))
                {
                    OnPropertyChanged(nameof(DisplayText));
                }
            }
        }

        public string Value
        {
            get => _value;
            set
            {
                if (SetProperty(ref _value, value ?? string.Empty))
                {
                    OnPropertyChanged(nameof(DisplayText));
                }
            }
        }

        public ObservableCollection<ChatStructuredXmlNodeModel> Children { get; } = new();

        public bool HasChildren => Children.Count > 0;

        public string DisplayText => HasChildren || string.IsNullOrWhiteSpace(Value)
            ? Name
            : $"{Name}: {Value}";
    }
}
