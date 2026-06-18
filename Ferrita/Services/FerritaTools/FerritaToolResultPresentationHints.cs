namespace Ferrita.Services.FerritaTools
{
    public static class FerritaToolResultPresentationKinds
    {
        public const string LineDiffV1 = "line-diff-v1";
    }

    public static class FerritaToolResultPresentationMetadataKeys
    {
        public const string PresentationKind = "ToolResultPresentationKind";
    }

    public sealed class FerritaToolResultPresentationHints
    {
        public static FerritaToolResultPresentationHints None { get; } = new();

        public string PresentationKind { get; init; } = string.Empty;

        public bool IsUserVisible { get; init; }

        public bool HasAnyValue =>
            !string.IsNullOrWhiteSpace(PresentationKind) ||
            IsUserVisible;

        public static FerritaToolResultPresentationHints CreateLineDiff()
        {
            return new FerritaToolResultPresentationHints
            {
                PresentationKind = FerritaToolResultPresentationKinds.LineDiffV1
            };
        }
    }
}
