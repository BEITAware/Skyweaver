using System.IO;
using Skyweaver.Models.ChatSession;

namespace Skyweaver.Services.ChatSession
{
    public static class ChatSessionResourceLayout
    {
        public const string ResourcesFolderName = "ChatSessionResources";
        public const string ToolCallsFolderName = "ToolCalls";

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

        public static string EnsureResources(ChatSessionModel session)
        {
            var resourcesFolderPath = GetResourcesFolderPath(session);
            Directory.CreateDirectory(resourcesFolderPath);
            Directory.CreateDirectory(GetToolCallsFolderPath(session));
            return resourcesFolderPath;
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
