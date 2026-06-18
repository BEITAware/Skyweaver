using Ferrita.Infrastructure.Mvvm;

namespace Ferrita.Panels.MultiFunctionArea.Models
{
    public sealed class MultiFunctionTabDefinition : ObservableObject
    {
        private string _title = string.Empty;
        private string _description = string.Empty;

        public string TypeKey { get; init; } = string.Empty;

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public string TitleResourceKey { get; init; } = string.Empty;

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        public string DescriptionResourceKey { get; init; } = string.Empty;

        public string IconPath { get; init; } = "pack://application:,,,/Resources/image.png";

        public int MaxCount { get; init; } = int.MaxValue;

        public Func<int, object?> ContentFactory { get; init; } = _ => null;
    }
}
