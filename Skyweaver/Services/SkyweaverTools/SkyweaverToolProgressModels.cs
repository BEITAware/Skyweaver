namespace Skyweaver.Services.SkyweaverTools
{
    public static class SkyweaverToolProgressMetadataKeys
    {
        public const string Phase = "ToolProgressPhase";

        public const string StatusText = "ToolProgressStatusText";

        public const string CompletedItems = "ToolProgressCompletedItems";

        public const string TotalItems = "ToolProgressTotalItems";

        public const string ProgressFraction = "ToolProgressFraction";

        public const string IsCompleted = "ToolProgressIsCompleted";

        public const string ActiveItems = "ToolProgressActiveItems";
    }

    public sealed class SkyweaverToolProgressUpdate
    {
        public string Phase { get; init; } = string.Empty;

        public string StatusText { get; init; } = string.Empty;

        public int? CompletedItems { get; init; }

        public int? TotalItems { get; init; }

        public double? ProgressFraction { get; init; }

        public bool IsCompleted { get; init; }

        public IReadOnlyList<string> ActiveItems { get; init; } = Array.Empty<string>();

        public SkyweaverToolProgressUpdate Normalize()
        {
            return new SkyweaverToolProgressUpdate
            {
                Phase = Phase?.Trim() ?? string.Empty,
                StatusText = StatusText?.Trim() ?? string.Empty,
                CompletedItems = CompletedItems,
                TotalItems = TotalItems,
                ProgressFraction = ProgressFraction is double value && !double.IsNaN(value) && !double.IsInfinity(value)
                    ? Math.Clamp(value, 0d, 1d)
                    : null,
                IsCompleted = IsCompleted,
                ActiveItems = ActiveItems
                    .Where(item => !string.IsNullOrWhiteSpace(item))
                    .Select(item => item.Trim())
                    .ToArray()
            };
        }
    }
}
