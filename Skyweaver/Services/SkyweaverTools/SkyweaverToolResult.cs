namespace Skyweaver.Services.SkyweaverTools
{
    public sealed class SkyweaverToolResult
    {
        private SkyweaverToolResult(
            bool isSuccess,
            string content,
            IReadOnlyDictionary<string, object?>? data)
        {
            IsSuccess = isSuccess;
            Content = content ?? string.Empty;
            Data = data ?? new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
        }

        public bool IsSuccess { get; }

        public string Content { get; }

        public IReadOnlyDictionary<string, object?> Data { get; }

        public static SkyweaverToolResult Success(
            string content,
            IReadOnlyDictionary<string, object?>? data = null)
        {
            return new SkyweaverToolResult(true, content, data);
        }

        public static SkyweaverToolResult Failure(
            string content,
            IReadOnlyDictionary<string, object?>? data = null)
        {
            return new SkyweaverToolResult(false, content, data);
        }
    }
}
