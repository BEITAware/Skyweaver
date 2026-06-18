using System.IO;
using Ferrita.Models.ChatSession;

namespace Ferrita.Services.ChatSession
{
    public static class ChatSessionResourceLayout
    {
        public const string ResourcesFolderName = "ChatSessionResources";
        public const string ToolCallsFolderName = "ToolCalls";
        public const string CompactionFileName = "Compaction.xml";

        public static string GetResourcesFolderPath(ChatSessionModel session)
        {
            ArgumentNullException.ThrowIfNull(session);

            return string.IsNullOrWhiteSpace(session.ResourcesFolderPath)
                ? Path.Combine(session.SessionFolderPath, ResourcesFolderName)
                : session.ResourcesFolderPath;
        }

        public static string GetToolCallsFolderPath(ChatSessionModel session)
        {
            return Path.Combine(GetResourcesFolderPath(session), ToolCallsFolderName);
        }

        public static string GetCompactionFilePath(ChatSessionModel session)
        {
            return Path.Combine(GetResourcesFolderPath(session), CompactionFileName);
        }

        public static string EnsureResources(ChatSessionModel session)
        {
            var resourcesFolderPath = GetResourcesFolderPath(session);
            Directory.CreateDirectory(resourcesFolderPath);
            Directory.CreateDirectory(GetToolCallsFolderPath(session));
            EnsureCompactionFile(session);
            return resourcesFolderPath;
        }

        public static string EnsureCompactionFile(ChatSessionModel session)
        {
            var compactionFilePath = GetCompactionFilePath(session);
            if (!File.Exists(compactionFilePath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(compactionFilePath) ?? GetResourcesFolderPath(session));
                File.WriteAllText(
                    compactionFilePath,
                    """
                    <?xml version="1.0" encoding="utf-8"?>
                    <Compaction SchemaVersion="1">
                      <ToolCalls />
                      <TokenCounts />
                    </Compaction>
                    """);
            }

            return compactionFilePath;
        }

        public static string GetToolCallFilePath(ChatSessionModel session, string toolCallId)
        {
            var normalizedToolCallId = ChatSessionToolCallIdGenerator.Normalize(toolCallId);
            if (normalizedToolCallId.Length == 0)
            {
                throw new ArgumentException("Tool call id cannot be empty.", nameof(toolCallId));
            }

            return Path.Combine(GetToolCallsFolderPath(session), $"{normalizedToolCallId}.xml");
        }
    }
}
