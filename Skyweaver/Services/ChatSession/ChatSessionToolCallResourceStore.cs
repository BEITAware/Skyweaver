using System.Globalization;
using System.IO;
using System.Xml.Linq;
using Skyweaver.Models.ChatSession;

namespace Skyweaver.Services.ChatSession
{
    public sealed class ChatSessionToolCallResourceStore
    {
        public string SaveInvocation(
            ChatSessionModel session,
            string toolCallId,
            string? callerAgentId,
            string? invocationXml)
        {
            return SaveToolCallContent(
                session,
                toolCallId,
                callerAgentId,
                "InvocationXml",
                invocationXml);
        }

        public string SaveOutput(
            ChatSessionModel session,
            string toolCallId,
            string? callerAgentId,
            string? outputXml)
        {
            return SaveToolCallContent(
                session,
                toolCallId,
                callerAgentId,
                "OutputXml",
                outputXml);
        }

        public string LoadInvocation(ChatSessionModel session, string? toolCallId)
        {
            return LoadToolCallContent(session, toolCallId, "InvocationXml");
        }

        public string LoadOutput(ChatSessionModel session, string? toolCallId)
        {
            return LoadToolCallContent(session, toolCallId, "OutputXml");
        }

        private static string SaveToolCallContent(
            ChatSessionModel session,
            string toolCallId,
            string? callerAgentId,
            string elementName,
            string? content)
        {
            ArgumentNullException.ThrowIfNull(session);

            var normalizedToolCallId = ChatSessionToolCallIdGenerator.Normalize(toolCallId);
            if (normalizedToolCallId.Length == 0)
            {
                throw new ArgumentException("Tool call id cannot be empty.", nameof(toolCallId));
            }

            ChatSessionResourceLayout.EnsureResources(session);
            var filePath = ChatSessionResourceLayout.GetToolCallFilePath(session, normalizedToolCallId);
            var document = LoadOrCreateDocument(filePath, normalizedToolCallId);
            var root = document.Root!;
            root.SetAttributeValue("ToolCallID", normalizedToolCallId);
            if (!string.IsNullOrWhiteSpace(callerAgentId))
            {
                root.SetAttributeValue("CallerAgentID", callerAgentId.Trim());
            }

            root.SetElementValue(elementName, content ?? string.Empty);
            root.SetElementValue(
                "UpdatedAtUtc",
                DateTime.UtcNow.ToString("O", CultureInfo.InvariantCulture));

            document.Save(filePath);
            return filePath;
        }

        private static string LoadToolCallContent(
            ChatSessionModel session,
            string? toolCallId,
            string elementName)
        {
            ArgumentNullException.ThrowIfNull(session);

            var normalizedToolCallId = ChatSessionToolCallIdGenerator.Normalize(toolCallId);
            if (normalizedToolCallId.Length == 0)
            {
                return string.Empty;
            }

            var filePath = ChatSessionResourceLayout.GetToolCallFilePath(session, normalizedToolCallId);
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
