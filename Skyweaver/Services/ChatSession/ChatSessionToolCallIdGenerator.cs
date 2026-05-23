using System.IO;
using System.Text;
using Skyweaver.Models.ChatSession;

namespace Skyweaver.Services.ChatSession
{
    public static class ChatSessionToolCallIdGenerator
    {
        private const string Prefix = "TC";

        public static string Normalize(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            var builder = new StringBuilder(value.Length);
            foreach (var character in value.Trim())
            {
                if (char.IsAsciiLetterOrDigit(character))
                {
                    builder.Append(char.ToUpperInvariant(character));
                }
            }

            return builder.ToString();
        }

        public static string Create(
            ChatSessionModel session,
            ISet<string>? reservedIds = null)
        {
            ArgumentNullException.ThrowIfNull(session);

            var usedIds = CollectToolCallIds(session);
            if (reservedIds != null)
            {
                foreach (var reservedId in reservedIds)
                {
                    var normalized = Normalize(reservedId);
                    if (normalized.Length > 0)
                    {
                        usedIds.Add(normalized);
                    }
                }
            }

            for (var index = 1; index < int.MaxValue; index++)
            {
                var candidate = $"{Prefix}{index}";
                if (usedIds.Add(candidate))
                {
                    reservedIds?.Add(candidate);
                    return candidate;
                }
            }

            throw new InvalidOperationException("Unable to allocate a unique tool call id.");
        }

        private static HashSet<string> CollectToolCallIds(ChatSessionModel session)
        {
            var ids = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            lock (session.Transcript.SyncRoot)
            {
            foreach (var entry in session.Transcript.Entries)
            {
                Add(entry.ToolCallId);
            }
            }

            foreach (var filePath in EnumerateToolCallFiles(session))
            {
                Add(Path.GetFileNameWithoutExtension(filePath));
            }

            return ids;

            void Add(string? value)
            {
                var normalized = Normalize(value);
                if (normalized.Length > 0)
                {
                    ids.Add(normalized);
                }
            }
        }

        private static IEnumerable<string> EnumerateToolCallFiles(ChatSessionModel session)
        {
            var folderPath = ChatSessionResourceLayout.GetToolCallsFolderPath(session);
            return Directory.Exists(folderPath)
                ? Directory.EnumerateFiles(folderPath, "*.xml")
                : Array.Empty<string>();
        }
    }
}
