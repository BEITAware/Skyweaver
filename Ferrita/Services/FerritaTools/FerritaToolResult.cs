namespace Ferrita.Services.FerritaTools
{
    public sealed class FerritaToolResult
    {
        private FerritaToolResult(
            bool isSuccess,
            string content,
            IReadOnlyDictionary<string, object?>? data,
            FerritaToolResultPresentationHints? presentationHints)
        {
            IsSuccess = isSuccess;
            Content = content ?? string.Empty;
            Data = data ?? new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
            PresentationHints = presentationHints ?? FerritaToolResultPresentationHints.None;
        }

        public bool IsSuccess { get; }

        public string Content { get; }

        public IReadOnlyDictionary<string, object?> Data { get; }

        public FerritaToolResultPresentationHints PresentationHints { get; }

        public static FerritaToolResult Success(
            string content,
            IReadOnlyDictionary<string, object?>? data = null,
            FerritaToolResultPresentationHints? presentationHints = null)
        {
            return new FerritaToolResult(true, content, data, presentationHints);
        }

        public static FerritaToolResult Failure(
            string content,
            IReadOnlyDictionary<string, object?>? data = null,
            FerritaToolResultPresentationHints? presentationHints = null)
        {
            return new FerritaToolResult(false, content, data, presentationHints);
        }
    }
}
