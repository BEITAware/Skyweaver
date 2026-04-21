namespace Skyweaver.Panels.MultiFunctionArea.Models
{
    public sealed class MultiFunctionTabDefinition
    {
        public string TypeKey { get; init; } = string.Empty;

        public string Title { get; init; } = string.Empty;

        public string Description { get; init; } = string.Empty;

        public string IconPath { get; init; } = "pack://application:,,,/Resources/image.png";

        public int MaxCount { get; init; } = int.MaxValue;

        public Func<int, object?> ContentFactory { get; init; } = _ => null;
    }
}
