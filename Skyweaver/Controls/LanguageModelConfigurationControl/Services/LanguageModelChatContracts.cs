namespace Skyweaver.Controls.LanguageModelConfigurationControl.Services
{
    public enum LanguageModelChatRole
    {
        System = 0,
        User = 1,
        Assistant = 2,
        Tool = 3
    }

    public sealed class LanguageModelChatMessage
    {
        public LanguageModelChatMessage(LanguageModelChatRole role, string content)
        {
            Role = role;
            Content = content ?? string.Empty;
        }

        public LanguageModelChatRole Role { get; }

        public string Content { get; }

        public string? AuthorName { get; init; }

        public LanguageModelChatMessage Clone()
        {
            return new LanguageModelChatMessage(Role, Content)
            {
                AuthorName = AuthorName
            };
        }
    }

    public sealed class LanguageModelChatResponse
    {
        public string Text { get; init; } = string.Empty;

        public string? ModelId { get; init; }
    }

    public sealed class LanguageModelStreamingContentDebugItem
    {
        public string ContentType { get; init; } = string.Empty;

        public string? Text { get; init; }

        public string? Summary { get; init; }

        public string? RawRepresentationType { get; init; }

        public string? RawRepresentationSummary { get; init; }

        public IReadOnlyDictionary<string, object?> AdditionalProperties { get; init; } =
            new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
    }

    public sealed class LanguageModelStreamingChatUpdate
    {
        public string TextDelta { get; init; } = string.Empty;

        public string? ModelId { get; init; }

        public string? RawText { get; init; }

        public bool WasTextSanitized { get; init; }

        public string? Role { get; init; }

        public string? AuthorName { get; init; }

        public string? FinishReason { get; init; }

        public string? ResponseId { get; init; }

        public string? MessageId { get; init; }

        public string? ConversationId { get; init; }

        public DateTimeOffset? CreatedAt { get; init; }

        public string? ContinuationToken { get; init; }

        public string? RawRepresentationType { get; init; }

        public string? RawRepresentationSummary { get; init; }

        public IReadOnlyDictionary<string, object?> AdditionalProperties { get; init; } =
            new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);

        public IReadOnlyList<LanguageModelStreamingContentDebugItem> Contents { get; init; } =
            Array.Empty<LanguageModelStreamingContentDebugItem>();
    }
}
