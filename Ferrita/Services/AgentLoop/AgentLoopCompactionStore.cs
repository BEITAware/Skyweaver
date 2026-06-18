using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Ferrita.Controls.LanguageModelConfigurationControl.Services;
using Ferrita.Services.ChatSession;

namespace Ferrita.Services.AgentLoop
{
    public sealed record AgentLoopCompactionToolCallRecord(
        string ToolCallId,
        string ToolName,
        string InvocationXml,
        string OutputXml);

    public sealed class AgentLoopCompactionStore
    {
        private static readonly object s_syncRoot = new();

        public IReadOnlyList<string> GetCompactedToolCallIds(string? compactionFilePath)
        {
            if (string.IsNullOrWhiteSpace(compactionFilePath))
            {
                return Array.Empty<string>();
            }

            lock (s_syncRoot)
            {
                var document = LoadOrCreateDocument(compactionFilePath);
                return document.Root!
                    .Element("ToolCalls")
                    ?.Elements("ToolCall")
                    .Where(element => (bool?)element.Attribute("IsCompacted") == true)
                    .Select(element => ChatSessionToolCallIdGenerator.Normalize((string?)element.Attribute("ToolCallID")))
                    .Where(id => id.Length > 0)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .OrderBy(id => id, StringComparer.OrdinalIgnoreCase)
                    .ToArray() ?? Array.Empty<string>();
            }
        }

        public void CompactToolCalls(
            string? compactionFilePath,
            IEnumerable<AgentLoopCompactionToolCallRecord> records)
        {
            if (string.IsNullOrWhiteSpace(compactionFilePath))
            {
                return;
            }

            ArgumentNullException.ThrowIfNull(records);

            lock (s_syncRoot)
            {
                var document = LoadOrCreateDocument(compactionFilePath);
                var toolCallsElement = EnsureElement(document.Root!, "ToolCalls");
                var now = DateTime.UtcNow.ToString("O", CultureInfo.InvariantCulture);

                foreach (var record in records)
                {
                    var toolCallId = ChatSessionToolCallIdGenerator.Normalize(record.ToolCallId);
                    if (toolCallId.Length == 0)
                    {
                        continue;
                    }

                    var element = toolCallsElement.Elements("ToolCall")
                        .FirstOrDefault(candidate => string.Equals(
                            ChatSessionToolCallIdGenerator.Normalize((string?)candidate.Attribute("ToolCallID")),
                            toolCallId,
                            StringComparison.OrdinalIgnoreCase));
                    if (element == null)
                    {
                        element = new XElement("ToolCall", new XAttribute("ToolCallID", toolCallId));
                        toolCallsElement.Add(element);
                    }

                    element.SetAttributeValue("ToolCallID", toolCallId);
                    element.SetAttributeValue("ToolName", string.IsNullOrWhiteSpace(record.ToolName) ? "UnknownTool" : record.ToolName.Trim());
                    element.SetAttributeValue("IsCompacted", "true");
                    element.SetAttributeValue("CompactedAtUtc", now);
                    element.SetElementValue("InvocationXml", record.InvocationXml ?? string.Empty);
                    element.SetElementValue("OutputXml", record.OutputXml ?? string.Empty);
                }

                SaveDocument(document, compactionFilePath);
            }
        }

        public IReadOnlyList<string> RetrieveToolCalls(
            string? compactionFilePath,
            IEnumerable<string> requestedToolCallIds)
        {
            if (string.IsNullOrWhiteSpace(compactionFilePath))
            {
                return Array.Empty<string>();
            }

            ArgumentNullException.ThrowIfNull(requestedToolCallIds);

            var requested = requestedToolCallIds
                .Select(ChatSessionToolCallIdGenerator.Normalize)
                .Where(id => id.Length > 0)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();
            if (requested.Length == 0)
            {
                return Array.Empty<string>();
            }

            lock (s_syncRoot)
            {
                var document = LoadOrCreateDocument(compactionFilePath);
                var now = DateTime.UtcNow.ToString("O", CultureInfo.InvariantCulture);
                var retrieved = new List<string>();

                foreach (var element in document.Root!.Element("ToolCalls")?.Elements("ToolCall") ?? Enumerable.Empty<XElement>())
                {
                    var toolCallId = ChatSessionToolCallIdGenerator.Normalize((string?)element.Attribute("ToolCallID"));
                    if (toolCallId.Length == 0 || !requested.Contains(toolCallId, StringComparer.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    element.SetAttributeValue("IsCompacted", "false");
                    element.SetAttributeValue("RetrievedAtUtc", now);
                    retrieved.Add(toolCallId);
                }

                SaveDocument(document, compactionFilePath);
                return retrieved
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .OrderBy(id => id, StringComparer.OrdinalIgnoreCase)
                    .ToArray();
            }
        }

        public bool TryGetTokenCount(
            string? compactionFilePath,
            string modelKey,
            string hash,
            out int tokenCount)
        {
            tokenCount = 0;
            if (string.IsNullOrWhiteSpace(compactionFilePath) ||
                string.IsNullOrWhiteSpace(modelKey) ||
                string.IsNullOrWhiteSpace(hash))
            {
                return false;
            }

            lock (s_syncRoot)
            {
                var document = LoadOrCreateDocument(compactionFilePath);
                var element = document.Root!
                    .Element("TokenCounts")
                    ?.Elements("TokenCount")
                    .FirstOrDefault(candidate =>
                        string.Equals((string?)candidate.Attribute("ModelKey"), modelKey, StringComparison.OrdinalIgnoreCase) &&
                        string.Equals((string?)candidate.Attribute("Hash"), hash, StringComparison.OrdinalIgnoreCase));

                return int.TryParse(
                    (string?)element?.Attribute("Value"),
                    NumberStyles.Integer,
                    CultureInfo.InvariantCulture,
                    out tokenCount);
            }
        }

        public void SaveTokenCount(
            string? compactionFilePath,
            string modelKey,
            string hash,
            int tokenCount,
            string source)
        {
            if (string.IsNullOrWhiteSpace(compactionFilePath) ||
                string.IsNullOrWhiteSpace(modelKey) ||
                string.IsNullOrWhiteSpace(hash) ||
                tokenCount <= 0)
            {
                return;
            }

            lock (s_syncRoot)
            {
                var document = LoadOrCreateDocument(compactionFilePath);
                var tokenCountsElement = EnsureElement(document.Root!, "TokenCounts");
                var element = tokenCountsElement.Elements("TokenCount")
                    .FirstOrDefault(candidate =>
                        string.Equals((string?)candidate.Attribute("ModelKey"), modelKey, StringComparison.OrdinalIgnoreCase) &&
                        string.Equals((string?)candidate.Attribute("Hash"), hash, StringComparison.OrdinalIgnoreCase));
                if (element == null)
                {
                    element = new XElement("TokenCount");
                    tokenCountsElement.Add(element);
                }

                element.SetAttributeValue("ModelKey", modelKey.Trim());
                element.SetAttributeValue("Hash", hash.Trim());
                element.SetAttributeValue("Value", tokenCount.ToString(CultureInfo.InvariantCulture));
                element.SetAttributeValue("Source", string.IsNullOrWhiteSpace(source) ? "LLM API" : source.Trim());
                element.SetAttributeValue("CountedAtUtc", DateTime.UtcNow.ToString("O", CultureInfo.InvariantCulture));
                SaveDocument(document, compactionFilePath);
            }
        }

        public IReadOnlyList<LanguageModelChatMessage> ApplyCompaction(
            string? compactionFilePath,
            IReadOnlyList<LanguageModelChatMessage> messages)
        {
            ArgumentNullException.ThrowIfNull(messages);
            if (string.IsNullOrWhiteSpace(compactionFilePath))
            {
                return messages.Select(message => message.Clone()).ToArray();
            }

            var compacted = LoadCompactedRecords(compactionFilePath);
            if (compacted.Count == 0)
            {
                return messages.Select(message => message.Clone()).ToArray();
            }

            var compactedMessages = new List<LanguageModelChatMessage>(messages.Count);
            var placeholderEmittedIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var message in messages)
            {
                var replacement = TryBuildCompactedMessage(message, compacted, placeholderEmittedIds);
                if (replacement != null)
                {
                    compactedMessages.Add(replacement);
                }
            }

            return compactedMessages;
        }

        public IReadOnlyList<AgentLoopCompactionToolCallRecord> ExtractToolCallRecords(
            IReadOnlyList<LanguageModelChatMessage> messages)
        {
            ArgumentNullException.ThrowIfNull(messages);

            var recordsById = new Dictionary<string, AgentLoopCompactionToolCallRecord>(StringComparer.OrdinalIgnoreCase);
            foreach (var message in messages)
            {
                if (TryReadToolInvocation(message.Content, out var invocationRecord))
                {
                    Merge(recordsById, invocationRecord);
                    continue;
                }

                foreach (var outputRecord in ReadToolReturnRecords(message.Content))
                {
                    Merge(recordsById, outputRecord);
                }
            }

            return recordsById.Values
                .Where(record => !string.IsNullOrWhiteSpace(record.InvocationXml) ||
                                 !string.IsNullOrWhiteSpace(record.OutputXml))
                .OrderBy(record => record.ToolCallId, StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        public static string CreateTokenCountHash(IReadOnlyList<LanguageModelChatMessage> messages)
        {
            ArgumentNullException.ThrowIfNull(messages);

            var builder = new StringBuilder();
            foreach (var message in messages)
            {
                builder.Append("role=").Append(message.Role).Append('\n');
                builder.Append("author=").Append(message.AuthorName ?? string.Empty).Append('\n');
                builder.Append("tail=").Append(message.IsHostInjectedTail ? "1" : "0").Append('\n');
                foreach (var block in message.ContentBlocks)
                {
                    builder.Append("block=").Append(block.Kind).Append('\n');
                    builder.Append("media=").Append(block.MediaType ?? string.Empty).Append('\n');
                    builder.Append("path=").Append(block.ResourcePath ?? string.Empty).Append('\n');
                    builder.Append("content=").Append(block.Content ?? string.Empty).Append('\n');
                    if (block.Data is { Length: > 0 } data)
                    {
                        builder.Append("data=").Append(Convert.ToHexString(SHA256.HashData(data))).Append('\n');
                    }
                }
            }

            return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(builder.ToString())));
        }

        public static string EnsureToolCallIdInToolInvocationXml(string? xml, string? toolCallId)
        {
            var normalizedToolCallId = ChatSessionToolCallIdGenerator.Normalize(toolCallId);
            if (string.IsNullOrWhiteSpace(xml) || normalizedToolCallId.Length == 0)
            {
                return xml ?? string.Empty;
            }

            try
            {
                var document = XDocument.Parse(xml, LoadOptions.PreserveWhitespace);
                if (document.Root == null ||
                    !(IsElementNamed(document.Root, "Tool") || IsElementNamed(document.Root, "ToolAsync")))
                {
                    return xml;
                }

                document.Root.SetAttributeValue("ToolCallID", normalizedToolCallId);
                return document.Root.ToString(SaveOptions.DisableFormatting);
            }
            catch (XmlException)
            {
                return xml;
            }
        }

        public static string EnsureToolCallIdInToolsReturnXml(string? xml, string? toolCallId)
        {
            var normalizedToolCallId = ChatSessionToolCallIdGenerator.Normalize(toolCallId);
            if (string.IsNullOrWhiteSpace(xml) || normalizedToolCallId.Length == 0)
            {
                return xml ?? string.Empty;
            }

            try
            {
                var document = XDocument.Parse(xml, LoadOptions.PreserveWhitespace);
                if (document.Root == null || !IsElementNamed(document.Root, "ToolsReturn"))
                {
                    return xml;
                }

                foreach (var toolReturn in document.Root.Elements().Where(element => IsElementNamed(element, "ToolReturn")))
                {
                    toolReturn.SetAttributeValue("ToolCallId", normalizedToolCallId);
                }

                return document.ToString(SaveOptions.DisableFormatting);
            }
            catch (XmlException)
            {
                return xml;
            }
        }

        private static LanguageModelChatMessage? TryBuildCompactedMessage(
            LanguageModelChatMessage message,
            IReadOnlyDictionary<string, AgentLoopCompactionToolCallRecord> compacted,
            ISet<string> placeholderEmittedIds)
        {
            if (message.ContentBlocks.Count != 1 ||
                !message.ContentBlocks[0].IsTextLike ||
                string.IsNullOrWhiteSpace(message.Content))
            {
                return message.Clone();
            }

            var content = message.Content;
            if (content.IndexOf("[CompactedToolCall", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return message.Clone();
            }

            if (TryReadToolInvocation(content, out var invocationRecord) &&
                compacted.TryGetValue(invocationRecord.ToolCallId, out var compactedInvocation))
            {
                placeholderEmittedIds.Add(invocationRecord.ToolCallId);
                return CreateReplacementMessage(message, FormatPlaceholder(compactedInvocation));
            }

            var returnRecords = ReadToolReturnRecords(content).ToArray();
            if (returnRecords.Length == 0)
            {
                return message.Clone();
            }

            var compactedReturnIds = returnRecords
                .Select(record => record.ToolCallId)
                .Where(id => compacted.ContainsKey(id))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();
            if (compactedReturnIds.Length == 0)
            {
                return message.Clone();
            }

            var placeholders = compactedReturnIds
                .Where(id => !placeholderEmittedIds.Contains(id))
                .Select(id =>
                {
                    placeholderEmittedIds.Add(id);
                    return FormatPlaceholder(compacted[id]);
                })
                .ToArray();
            var remainingXml = RemoveCompactedToolReturns(content, compactedReturnIds);
            var replacement = string.Join(
                Environment.NewLine,
                placeholders
                    .Concat(string.IsNullOrWhiteSpace(remainingXml) ? Array.Empty<string>() : [remainingXml]));
            if (string.IsNullOrWhiteSpace(replacement))
            {
                return null;
            }

            return CreateReplacementMessage(message, replacement);
        }

        private static LanguageModelChatMessage CreateReplacementMessage(
            LanguageModelChatMessage source,
            string content)
        {
            return new LanguageModelChatMessage(source.Role, content)
            {
                AuthorName = source.AuthorName,
                IsHostInjectedTail = source.IsHostInjectedTail
            };
        }

        private static IReadOnlyDictionary<string, AgentLoopCompactionToolCallRecord> LoadCompactedRecords(string compactionFilePath)
        {
            lock (s_syncRoot)
            {
                var document = LoadOrCreateDocument(compactionFilePath);
                return document.Root!
                    .Element("ToolCalls")
                    ?.Elements("ToolCall")
                    .Where(element => (bool?)element.Attribute("IsCompacted") == true)
                    .Select(ReadStoredRecord)
                    .Where(record => record.ToolCallId.Length > 0)
                    .GroupBy(record => record.ToolCallId, StringComparer.OrdinalIgnoreCase)
                    .ToDictionary(group => group.Key, group => group.First(), StringComparer.OrdinalIgnoreCase)
                    ?? new Dictionary<string, AgentLoopCompactionToolCallRecord>(StringComparer.OrdinalIgnoreCase);
            }
        }

        private static AgentLoopCompactionToolCallRecord ReadStoredRecord(XElement element)
        {
            var toolCallId = ChatSessionToolCallIdGenerator.Normalize((string?)element.Attribute("ToolCallID"));
            var toolName = ((string?)element.Attribute("ToolName") ?? string.Empty).Trim();
            var invocationXml = (string?)element.Element("InvocationXml") ?? string.Empty;
            var outputXml = (string?)element.Element("OutputXml") ?? string.Empty;
            return new AgentLoopCompactionToolCallRecord(toolCallId, toolName, invocationXml, outputXml);
        }

        private static void Merge(
            IDictionary<string, AgentLoopCompactionToolCallRecord> recordsById,
            AgentLoopCompactionToolCallRecord incoming)
        {
            var toolCallId = ChatSessionToolCallIdGenerator.Normalize(incoming.ToolCallId);
            if (toolCallId.Length == 0)
            {
                return;
            }

            if (!recordsById.TryGetValue(toolCallId, out var existing))
            {
                recordsById[toolCallId] = incoming with { ToolCallId = toolCallId };
                return;
            }

            recordsById[toolCallId] = new AgentLoopCompactionToolCallRecord(
                toolCallId,
                FirstNonEmpty(existing.ToolName, incoming.ToolName),
                FirstNonEmpty(existing.InvocationXml, incoming.InvocationXml),
                FirstNonEmpty(existing.OutputXml, incoming.OutputXml));
        }

        private static bool TryReadToolInvocation(
            string? content,
            out AgentLoopCompactionToolCallRecord record)
        {
            record = new AgentLoopCompactionToolCallRecord(string.Empty, string.Empty, string.Empty, string.Empty);
            if (string.IsNullOrWhiteSpace(content))
            {
                return false;
            }

            try
            {
                var document = XDocument.Parse(content, LoadOptions.PreserveWhitespace);
                var root = document.Root;
                if (root == null || !(IsElementNamed(root, "Tool") || IsElementNamed(root, "ToolAsync")))
                {
                    return false;
                }

                var toolCallId = ChatSessionToolCallIdGenerator.Normalize(GetAttribute(root, "ToolCallID") ?? GetAttribute(root, "ToolCallId"));
                if (toolCallId.Length == 0)
                {
                    return false;
                }

                var toolName = GetAttribute(root, "ToolName") ?? GetAttribute(root, "Name") ?? "UnknownTool";
                record = new AgentLoopCompactionToolCallRecord(
                    toolCallId,
                    toolName,
                    root.ToString(SaveOptions.DisableFormatting),
                    string.Empty);
                return true;
            }
            catch (XmlException)
            {
                return false;
            }
        }

        private static IEnumerable<AgentLoopCompactionToolCallRecord> ReadToolReturnRecords(string? content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                yield break;
            }

            XDocument document;
            try
            {
                document = XDocument.Parse(content, LoadOptions.PreserveWhitespace);
            }
            catch (XmlException)
            {
                yield break;
            }

            var root = document.Root;
            if (root == null || !IsElementNamed(root, "ToolsReturn"))
            {
                yield break;
            }

            foreach (var toolReturn in root.Elements().Where(element => IsElementNamed(element, "ToolReturn")))
            {
                var toolCallId = ChatSessionToolCallIdGenerator.Normalize(GetAttribute(toolReturn, "ToolCallID") ?? GetAttribute(toolReturn, "ToolCallId"));
                if (toolCallId.Length == 0)
                {
                    continue;
                }

                var toolName = GetAttribute(toolReturn, "ToolName") ?? GetAttribute(toolReturn, "Name") ?? "UnknownTool";
                var outputXml = new XDocument(new XElement(
                    root.Name,
                    root.Attributes(),
                    new XElement(toolReturn))).ToString(SaveOptions.DisableFormatting);
                yield return new AgentLoopCompactionToolCallRecord(
                    toolCallId,
                    toolName,
                    string.Empty,
                    outputXml);
            }
        }

        private static string RemoveCompactedToolReturns(
            string content,
            IEnumerable<string> compactedToolCallIds)
        {
            var compacted = compactedToolCallIds
                .Select(ChatSessionToolCallIdGenerator.Normalize)
                .Where(id => id.Length > 0)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            try
            {
                var document = XDocument.Parse(content, LoadOptions.PreserveWhitespace);
                var root = document.Root;
                if (root == null || !IsElementNamed(root, "ToolsReturn"))
                {
                    return string.Empty;
                }

                foreach (var toolReturn in root.Elements().Where(element => IsElementNamed(element, "ToolReturn")).ToArray())
                {
                    var toolCallId = ChatSessionToolCallIdGenerator.Normalize(GetAttribute(toolReturn, "ToolCallID") ?? GetAttribute(toolReturn, "ToolCallId"));
                    if (toolCallId.Length > 0 && compacted.Contains(toolCallId))
                    {
                        toolReturn.Remove();
                    }
                }

                return root.HasElements ? document.ToString(SaveOptions.DisableFormatting) : string.Empty;
            }
            catch (XmlException)
            {
                return string.Empty;
            }
        }

        private static XDocument LoadOrCreateDocument(string compactionFilePath)
        {
            if (File.Exists(compactionFilePath))
            {
                try
                {
                    var document = XDocument.Load(compactionFilePath, LoadOptions.PreserveWhitespace);
                    if (document.Root != null &&
                        string.Equals(document.Root.Name.LocalName, "Compaction", StringComparison.OrdinalIgnoreCase))
                    {
                        EnsureElement(document.Root, "ToolCalls");
                        EnsureElement(document.Root, "TokenCounts");
                        return document;
                    }
                }
                catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or XmlException)
                {
                    // Replace unreadable compaction metadata with a fresh document.
                }
            }

            return new XDocument(
                new XElement(
                    "Compaction",
                    new XAttribute("SchemaVersion", "1"),
                    new XElement("ToolCalls"),
                    new XElement("TokenCounts")));
        }

        private static XElement EnsureElement(XElement parent, string elementName)
        {
            var element = parent.Element(elementName);
            if (element != null)
            {
                return element;
            }

            element = new XElement(elementName);
            parent.Add(element);
            return element;
        }

        private static void SaveDocument(XDocument document, string filePath)
        {
            var directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrWhiteSpace(directory))
            {
                Directory.CreateDirectory(directory);
            }

            document.Save(filePath);
        }

        private static bool IsElementNamed(XElement element, string name)
        {
            return string.Equals(element.Name.LocalName, name, StringComparison.OrdinalIgnoreCase);
        }

        private static string? GetAttribute(XElement element, string attributeName)
        {
            return element.Attributes()
                .FirstOrDefault(attribute => string.Equals(attribute.Name.LocalName, attributeName, StringComparison.OrdinalIgnoreCase))
                ?.Value
                ?.Trim();
        }

        private static string FormatPlaceholder(AgentLoopCompactionToolCallRecord record)
        {
            var toolName = XmlEscapeAttribute(string.IsNullOrWhiteSpace(record.ToolName) ? "UnknownTool" : record.ToolName.Trim());
            var toolCallId = XmlEscapeAttribute(ChatSessionToolCallIdGenerator.Normalize(record.ToolCallId));
            return $"[CompactedToolCall ToolName=\"{toolName}\" ToolCallID=\"{toolCallId}\"]";
        }

        private static string XmlEscapeAttribute(string value)
        {
            return value
                .Replace("&", "&amp;", StringComparison.Ordinal)
                .Replace("\"", "&quot;", StringComparison.Ordinal)
                .Replace("<", "&lt;", StringComparison.Ordinal)
                .Replace(">", "&gt;", StringComparison.Ordinal);
        }

        private static string FirstNonEmpty(string? first, string? second)
        {
            return string.IsNullOrWhiteSpace(first) ? second?.Trim() ?? string.Empty : first.Trim();
        }
    }
}
