namespace Skyweaver.Services.SkyweaverTools
{
    public static class SkyweaverToolSyntaxInspector
    {
        private static readonly string[] s_invalidPseudoToolTagPrefixes =
        [
            "<tool_call",
            "<toolcall",
            "<function_call",
            "<functioncall"
        ];

        public const string InvalidPseudoToolMarkupErrorMessage =
            "Invalid pseudo tool syntax. Use only <Tools><Tool ToolName=\"...\">...</Tool></Tools>.";

        public static bool ContainsInvalidPseudoToolMarkup(string? text)
        {
            return IndexOfInvalidPseudoToolMarkup(text) >= 0;
        }

        public static int IndexOfInvalidPseudoToolMarkup(string? text, int startIndex = 0)
        {
            if (string.IsNullOrEmpty(text))
            {
                return -1;
            }

            var searchIndex = Math.Max(0, startIndex);
            var matchIndex = -1;

            foreach (var prefix in s_invalidPseudoToolTagPrefixes)
            {
                var candidateIndex = text.IndexOf(prefix, searchIndex, StringComparison.OrdinalIgnoreCase);
                if (candidateIndex >= 0 && (matchIndex < 0 || candidateIndex < matchIndex))
                {
                    matchIndex = candidateIndex;
                }
            }

            return matchIndex;
        }

        public static int GetInvalidPseudoToolMarkupLength(string text, bool isFinal)
        {
            if (string.IsNullOrEmpty(text))
            {
                return -1;
            }

            var startIndex = IndexOfInvalidPseudoToolMarkup(text);
            if (startIndex < 0)
            {
                return -1;
            }

            var openTagEndIndex = text.IndexOf('>', startIndex);
            if (openTagEndIndex < 0)
            {
                return isFinal ? text.Length : -1;
            }

            if (openTagEndIndex > startIndex && text[openTagEndIndex - 1] == '/')
            {
                return openTagEndIndex + 1;
            }

            var closingTagStartIndex = text.IndexOf("</", openTagEndIndex + 1, StringComparison.OrdinalIgnoreCase);
            if (closingTagStartIndex < 0)
            {
                return isFinal ? text.Length : -1;
            }

            var closingTagEndIndex = text.IndexOf('>', closingTagStartIndex + 2);
            if (closingTagEndIndex < 0)
            {
                return isFinal ? text.Length : -1;
            }

            return closingTagEndIndex + 1;
        }

        public static int GetTrailingPotentialInvalidPseudoToolPrefixLength(string? text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return 0;
            }

            var trailingPrefixLength = 0;
            foreach (var prefix in s_invalidPseudoToolTagPrefixes)
            {
                var candidateLength = GetTrailingPotentialPrefixLength(text, prefix);
                if (candidateLength > trailingPrefixLength)
                {
                    trailingPrefixLength = candidateLength;
                }
            }

            return trailingPrefixLength;
        }

        private static int GetTrailingPotentialPrefixLength(string text, string prefix)
        {
            var maxLength = Math.Min(text.Length, prefix.Length - 1);
            for (var length = maxLength; length > 0; length--)
            {
                if (text.EndsWith(
                        prefix[..length],
                        StringComparison.OrdinalIgnoreCase))
                {
                    return length;
                }
            }

            return 0;
        }
    }
}
