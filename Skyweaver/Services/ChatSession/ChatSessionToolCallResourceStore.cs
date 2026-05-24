using System.Globalization;
using System.IO;
using System.Xml.Linq;
using Skyweaver.Models.ChatSession;

namespace Skyweaver.Services.ChatSession
{
    public sealed class ChatSessionToolCallResourceRecord
    {
        public string ToolCallId { get; init; } = string.Empty;

        public string? CallerAgentId { get; init; }

        public string InvocationXml { get; init; } = string.Empty;

        public string OutputXml { get; init; } = string.Empty;

        public bool IsOutputTranscriptDelivered { get; init; }
    }

    public sealed class ChatSessionToolCallResourceStore
    {
        private static readonly object s_syncRoot = new();

        public string SaveInvocation(
            ChatSessionModel session,
            string toolCallId,
            string? callerAgentId,
            string? invocationXml)
        {
            return SaveToolCallContent(
                GetToolCallsFolderPath(session, ensureResources: true),
                toolCallId,
                callerAgentId,
                "InvocationXml",
                invocationXml,
                outputTranscriptDelivered: null);
        }

        public string SaveInvocation(
            string toolCallsFolderPath,
            string toolCallId,
            string? callerAgentId,
            string? invocationXml)
        {
            return SaveToolCallContent(
                toolCallsFolderPath,
                toolCallId,
                callerAgentId,
                "InvocationXml",
                invocationXml,
                outputTranscriptDelivered: null);
        }

        public string SaveOutput(
            ChatSessionModel session,
            string toolCallId,
            string? callerAgentId,
            string? outputXml,
            bool transcriptDelivered = true)
        {
            return SaveToolCallContent(
                GetToolCallsFolderPath(session, ensureResources: true),
                toolCallId,
                callerAgentId,
                "OutputXml",
                outputXml,
                transcriptDelivered);
        }

        public string SaveOutput(
            string toolCallsFolderPath,
            string toolCallId,
            string? callerAgentId,
            string? outputXml,
            bool transcriptDelivered = true)
        {
            return SaveToolCallContent(
                toolCallsFolderPath,
                toolCallId,
                callerAgentId,
                "OutputXml",
                outputXml,
                transcriptDelivered);
        }

        public string LoadInvocation(ChatSessionModel session, string? toolCallId)
        {
            return LoadInvocation(GetToolCallsFolderPath(session, ensureResources: false), toolCallId);
        }

        public string LoadInvocation(string? toolCallsFolderPath, string? toolCallId)
        {
            return LoadToolCallContent(toolCallsFolderPath, toolCallId, "InvocationXml");
        }

        public string LoadOutput(ChatSessionModel session, string? toolCallId)
        {
            return LoadOutput(GetToolCallsFolderPath(session, ensureResources: false), toolCallId);
        }

        public string LoadOutput(string? toolCallsFolderPath, string? toolCallId)
        {
            return LoadToolCallContent(toolCallsFolderPath, toolCallId, "OutputXml");
        }

        public IReadOnlyList<string> EnumerateToolCallIds(ChatSessionModel session)
        {
            return EnumerateToolCallIds(GetToolCallsFolderPath(session, ensureResources: false));
        }

        public IReadOnlyList<string> EnumerateToolCallIds(string? toolCallsFolderPath)
        {
            if (string.IsNullOrWhiteSpace(toolCallsFolderPath) ||
                !Directory.Exists(toolCallsFolderPath))
            {
                return Array.Empty<string>();
            }

            try
            {
                return Directory.EnumerateFiles(toolCallsFolderPath, "*.xml")
                    .Select(path => ChatSessionToolCallIdGenerator.Normalize(Path.GetFileNameWithoutExtension(path)))
                    .Where(id => id.Length > 0)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .OrderBy(id => id, StringComparer.OrdinalIgnoreCase)
                    .ToArray();
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                return Array.Empty<string>();
            }
        }

        public IReadOnlyList<ChatSessionToolCallResourceRecord> EnumerateRecords(string? toolCallsFolderPath)
        {
            if (string.IsNullOrWhiteSpace(toolCallsFolderPath) ||
                !Directory.Exists(toolCallsFolderPath))
            {
                return Array.Empty<ChatSessionToolCallResourceRecord>();
            }

            try
            {
                return Directory.EnumerateFiles(toolCallsFolderPath, "*.xml")
                    .Select(TryLoadRecord)
                    .Where(record => record != null)
                    .Cast<ChatSessionToolCallResourceRecord>()
                    .ToArray();
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                return Array.Empty<ChatSessionToolCallResourceRecord>();
            }
        }

        public bool MarkOutputTranscriptDelivered(string? toolCallsFolderPath, string? toolCallId)
        {
            var normalizedToolCallId = ChatSessionToolCallIdGenerator.Normalize(toolCallId);
            if (string.IsNullOrWhiteSpace(toolCallsFolderPath) ||
                normalizedToolCallId.Length == 0)
            {
                return false;
            }

            var filePath = GetToolCallFilePath(toolCallsFolderPath, normalizedToolCallId);
            if (!File.Exists(filePath))
            {
                return false;
            }

            try
            {
                lock (s_syncRoot)
                {
                    var document = XDocument.Load(filePath, LoadOptions.PreserveWhitespace);
                    if (document.Root == null ||
                        !string.Equals(document.Root.Name.LocalName, "ToolCall", StringComparison.OrdinalIgnoreCase))
                    {
                        return false;
                    }

                    document.Root.SetAttributeValue("OutputTranscriptDelivered", "true");
                    document.Root.SetElementValue(
                        "UpdatedAtUtc",
                        DateTime.UtcNow.ToString("O", CultureInfo.InvariantCulture));
                    document.Save(filePath);
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        private static string SaveToolCallContent(
            string toolCallsFolderPath,
            string toolCallId,
            string? callerAgentId,
            string elementName,
            string? content,
            bool? outputTranscriptDelivered)
        {
            var normalizedToolCallId = ChatSessionToolCallIdGenerator.Normalize(toolCallId);
            if (normalizedToolCallId.Length == 0)
            {
                throw new ArgumentException("Tool call id cannot be empty.", nameof(toolCallId));
            }

            if (string.IsNullOrWhiteSpace(toolCallsFolderPath))
            {
                throw new ArgumentException("Tool call folder path cannot be empty.", nameof(toolCallsFolderPath));
            }

            lock (s_syncRoot)
            {
                Directory.CreateDirectory(toolCallsFolderPath);
                var filePath = GetToolCallFilePath(toolCallsFolderPath, normalizedToolCallId);
                var document = LoadOrCreateDocument(filePath, normalizedToolCallId);
                var root = document.Root!;
                root.SetAttributeValue("ToolCallID", normalizedToolCallId);
                if (!string.IsNullOrWhiteSpace(callerAgentId))
                {
                    root.SetAttributeValue("CallerAgentID", callerAgentId.Trim());
                }

                root.SetElementValue(elementName, content ?? string.Empty);
                if (outputTranscriptDelivered.HasValue)
                {
                    var existingDelivered =
                        bool.TryParse((string?)root.Attribute("OutputTranscriptDelivered"), out var isDelivered) &&
                        isDelivered;
                    root.SetAttributeValue(
                        "OutputTranscriptDelivered",
                        outputTranscriptDelivered.Value || existingDelivered ? "true" : "false");
                }

                root.SetElementValue(
                    "UpdatedAtUtc",
                    DateTime.UtcNow.ToString("O", CultureInfo.InvariantCulture));

                document.Save(filePath);
                return filePath;
            }
        }

        private static string LoadToolCallContent(
            string? toolCallsFolderPath,
            string? toolCallId,
            string elementName)
        {
            var normalizedToolCallId = ChatSessionToolCallIdGenerator.Normalize(toolCallId);
            if (normalizedToolCallId.Length == 0 ||
                string.IsNullOrWhiteSpace(toolCallsFolderPath))
            {
                return string.Empty;
            }

            var filePath = GetToolCallFilePath(toolCallsFolderPath, normalizedToolCallId);
            if (!File.Exists(filePath))
            {
                return string.Empty;
            }

            try
            {
                return (string?)XDocument.Load(filePath).Root?.Element(elementName) ?? string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        private static ChatSessionToolCallResourceRecord? TryLoadRecord(string filePath)
        {
            try
            {
                var document = XDocument.Load(filePath, LoadOptions.PreserveWhitespace);
                var root = document.Root;
                if (root == null ||
                    !string.Equals(root.Name.LocalName, "ToolCall", StringComparison.OrdinalIgnoreCase))
                {
                    return null;
                }

                var toolCallId = ChatSessionToolCallIdGenerator.Normalize((string?)root.Attribute("ToolCallID"));
                if (toolCallId.Length == 0)
                {
                    toolCallId = ChatSessionToolCallIdGenerator.Normalize(Path.GetFileNameWithoutExtension(filePath));
                }

                if (toolCallId.Length == 0)
                {
                    return null;
                }

                return new ChatSessionToolCallResourceRecord
                {
                    ToolCallId = toolCallId,
                    CallerAgentId = (string?)root.Attribute("CallerAgentID"),
                    InvocationXml = (string?)root.Element("InvocationXml") ?? string.Empty,
                    OutputXml = (string?)root.Element("OutputXml") ?? string.Empty,
                    IsOutputTranscriptDelivered =
                        bool.TryParse((string?)root.Attribute("OutputTranscriptDelivered"), out var isDelivered) &&
                        isDelivered
                };
            }
            catch
            {
                return null;
            }
        }

        private static string GetToolCallsFolderPath(ChatSessionModel session, bool ensureResources)
        {
            ArgumentNullException.ThrowIfNull(session);

            if (ensureResources)
            {
                ChatSessionResourceLayout.EnsureResources(session);
            }

            return ChatSessionResourceLayout.GetToolCallsFolderPath(session);
        }

        private static string GetToolCallFilePath(string toolCallsFolderPath, string toolCallId)
        {
            var normalizedToolCallId = ChatSessionToolCallIdGenerator.Normalize(toolCallId);
            if (normalizedToolCallId.Length == 0)
            {
                throw new ArgumentException("Tool call id cannot be empty.", nameof(toolCallId));
            }

            return Path.Combine(toolCallsFolderPath, $"{normalizedToolCallId}.xml");
        }

        private static XDocument LoadOrCreateDocument(string filePath, string toolCallId)
        {
            if (File.Exists(filePath))
            {
                try
                {
                    var existing = XDocument.Load(filePath, LoadOptions.PreserveWhitespace);
                    if (existing.Root != null &&
                        string.Equals(existing.Root.Name.LocalName, "ToolCall", StringComparison.OrdinalIgnoreCase))
                    {
                        return existing;
                    }
                }
                catch
                {
                    // Replace unreadable resource files with a fresh, well-formed record.
                }
            }

            return new XDocument(
                new XElement(
                    "ToolCall",
                    new XAttribute("ToolCallID", toolCallId),
                    new XElement("InvocationXml", string.Empty),
                    new XElement("OutputXml", string.Empty)));
        }
    }
}
