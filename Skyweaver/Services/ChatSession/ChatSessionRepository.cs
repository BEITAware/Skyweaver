using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Skyweaver.Controls.ChatSessionControl.Models;
using Skyweaver.Controls.LanguageModelConfigurationControl.Services;
using Skyweaver.Models.ChatSession;

namespace Skyweaver.Services.ChatSession
{
    public sealed class ChatSessionRepository
    {
        private const string DefaultIconPath = "pack://application:,,,/Resources/NewNodeGraphAlt.png";
        private const string UserAvatarPath = "pack://application:,,,/Resources/image.png";
        private const string AssistantAvatarPath = "pack://application:,,,/Resources/GuideBot.png";
        private const string SystemAvatarPath = "pack://application:,,,/Resources/QuestionBot.png";

        private readonly ChatSessionFlowBindingService _flowBindingService;

        public string RootFolderPath { get; }

        public ChatSessionRepository()
            : this(new ChatSessionFlowBindingService())
        {
        }

        public ChatSessionRepository(ChatSessionFlowBindingService flowBindingService)
        {
            _flowBindingService = flowBindingService ?? throw new ArgumentNullException(nameof(flowBindingService));
            RootFolderPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                "Skyweaver",
                "ChatSessions");
        }

        public IReadOnlyList<ChatSessionModel> LoadAll()
        {
            EnsureRootFolder();

            if (!Directory.Exists(RootFolderPath))
            {
                return Array.Empty<ChatSessionModel>();
            }

            var sessionDirectories = Directory
                .EnumerateDirectories(RootFolderPath)
                .OrderByDescending(directory => Directory.GetLastWriteTimeUtc(directory));

            var sessions = new List<ChatSessionModel>();

            foreach (var sessionDirectory in sessionDirectories)
            {
                var sessionName = Path.GetFileName(sessionDirectory);
                if (string.IsNullOrWhiteSpace(sessionName))
                {
                    continue;
                }

                var sessionFilePath = Path.Combine(sessionDirectory, $"{sessionName}.xml");
                if (!File.Exists(sessionFilePath))
                {
                    continue;
                }

                sessions.Add(LoadFromFile(sessionFilePath));
            }

            return sessions;
        }

        public ChatSessionModel Create(string sessionName, ChatSessionFlowBinding? flowBinding = null)
        {
            EnsureRootFolder();

            var validatedName = ValidateSessionName(sessionName);
            var sessionFolderPath = Path.Combine(RootFolderPath, validatedName);
            if (Directory.Exists(sessionFolderPath))
            {
                throw new InvalidOperationException($"会话“{validatedName}”已存在。");
            }

            var resourcesFolderPath = Path.Combine(sessionFolderPath, "ChatSessionResources");
            Directory.CreateDirectory(sessionFolderPath);
            Directory.CreateDirectory(resourcesFolderPath);
            Directory.CreateDirectory(Path.Combine(resourcesFolderPath, "ToolCalls"));

            var session = new ChatSessionModel
            {
                SessionId = Guid.NewGuid().ToString("N"),
                Name = validatedName,
                IconPath = DefaultIconPath,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                ContextSummary = "新建会话，等待聊天内容写入。",
                MetadataNote = "XML session storage",
                SessionFolderPath = sessionFolderPath,
                SessionFilePath = Path.Combine(sessionFolderPath, $"{validatedName}.xml"),
                ResourcesFolderPath = resourcesFolderPath
            };

            var resolvedBinding = _flowBindingService.ResolveBinding(flowBinding, ensureDefaultGraph: true);
            session.FlowBinding.GraphId = resolvedBinding.GraphId;
            session.FlowBinding.GraphName = resolvedBinding.GraphName;
            session.FlowBinding.FilePath = resolvedBinding.FilePath;

            Save(session);
            return session;
        }

        public void Save(ChatSessionModel session)
        {
            ArgumentNullException.ThrowIfNull(session);

            EnsureRootFolder();
            HydratePaths(session);
            Directory.CreateDirectory(session.SessionFolderPath);
            ChatSessionResourceLayout.EnsureResources(session);

            session.UpdatedAt = DateTime.UtcNow;
            EnsureRecordProjection(session);

            var conversationElement = new XElement(
                "Conversation",
                new XAttribute("StorageSchema", "2"),
                session.Records.Select(record => CreateRecordElement(session, record)));

            var document = new XDocument(
                new XElement("ChatSession",
                    new XAttribute("SessionID", session.SessionId),
                    new XAttribute("Name", session.Name),
                    new XElement("Metadata",
                        new XElement("CreatedAtUtc", session.CreatedAt.ToString("O", CultureInfo.InvariantCulture)),
                        new XElement("UpdatedAtUtc", session.UpdatedAt.ToString("O", CultureInfo.InvariantCulture)),
                        new XElement("IconPath", session.IconPath),
                        new XElement("SessionFolder", session.SessionFolderPath),
                        new XElement("ResourcesFolder", session.ResourcesFolderPath),
                        new XElement("BoundSessionFlow",
                            new XElement("GraphId", session.FlowBinding.GraphId),
                            new XElement("GraphName", session.FlowBinding.GraphName),
                            new XElement("FilePath", session.FlowBinding.FilePath)),
                        new XElement("ContextSummary", session.ContextSummary),
                        new XElement("Note", session.MetadataNote)),
                    conversationElement));

            document.Save(session.SessionFilePath);
        }

        public void Delete(ChatSessionModel session)
        {
            ArgumentNullException.ThrowIfNull(session);
            HydratePaths(session);

            if (Directory.Exists(session.SessionFolderPath))
            {
                Directory.Delete(session.SessionFolderPath, recursive: true);
            }
        }

        private ChatSessionModel LoadFromFile(string sessionFilePath)
        {
            var document = XDocument.Load(sessionFilePath);
            var root = document.Root ?? throw new InvalidDataException("ChatSession XML 缺少根节点。");

            var name = (string?)root.Attribute("Name") ?? Path.GetFileNameWithoutExtension(sessionFilePath);
            var session = new ChatSessionModel
            {
                SessionId = (string?)root.Attribute("SessionID") ?? Guid.NewGuid().ToString("N"),
                Name = name,
                IconPath = (string?)root.Element("Metadata")?.Element("IconPath") ?? DefaultIconPath,
                CreatedAt = ParseDateTime((string?)root.Element("Metadata")?.Element("CreatedAtUtc")),
                UpdatedAt = ParseDateTime((string?)root.Element("Metadata")?.Element("UpdatedAtUtc")),
                ContextSummary = (string?)root.Element("Metadata")?.Element("ContextSummary") ?? string.Empty,
                MetadataNote = (string?)root.Element("Metadata")?.Element("Note") ?? string.Empty,
                SessionFilePath = sessionFilePath,
                SessionFolderPath = Path.GetDirectoryName(sessionFilePath) ?? string.Empty,
                ResourcesFolderPath = Path.Combine(Path.GetDirectoryName(sessionFilePath) ?? string.Empty, "ChatSessionResources")
            };

            var boundFlowElement = root.Element("Metadata")?.Element("BoundSessionFlow");
            if (boundFlowElement != null)
            {
                session.FlowBinding.GraphId = ((string?)boundFlowElement.Element("GraphId") ?? string.Empty).Trim();
                session.FlowBinding.GraphName = ((string?)boundFlowElement.Element("GraphName") ?? string.Empty).Trim();
                session.FlowBinding.FilePath = ((string?)boundFlowElement.Element("FilePath") ?? string.Empty).Trim();
            }

            ChatSessionResourceLayout.EnsureResources(session);

            var messageElements = root.Element("Conversation")?.Elements("Message") ?? Enumerable.Empty<XElement>();
            foreach (var messageElement in messageElements)
            {
                var record = LoadRecordElement(session, messageElement);
                if (record.Blocks.Count == 0)
                {
                    continue;
                }

                session.Records.Add(record);
                session.Messages.Add(ChatSessionPresentationProjector.ToPresentationMessage(record));
            }

            // Legacy files may contain a preprojected LLM transcript. Keep it in memory
            // only as a fallback for sessions that do not have visible or persisted records.
            var historyElements = root.Element("ConversationHistory")?.Elements("Message") ?? Enumerable.Empty<XElement>();
            foreach (var historyElement in historyElements)
            {
                var role = Enum.TryParse<LanguageModelChatRole>((string?)historyElement.Attribute("Role"), true, out var parsedRole)
                    ? parsedRole
                    : LanguageModelChatRole.User;
                var content = (string?)historyElement.Element("Content") ?? string.Empty;

                session.ConversationHistory.Add(new LanguageModelChatMessage(role, content)
                {
                    AuthorName = NullIfWhiteSpace((string?)historyElement.Element("AuthorName"))
                });
            }

            return session;
        }

        private static void EnsureRecordProjection(ChatSessionModel session)
        {
            if (session.Records.Count > 0 || session.Messages.Count == 0)
            {
                return;
            }

            foreach (var message in session.Messages)
            {
                session.Records.Add(ChatSessionPresentationProjector.ToRecord(message));
            }
        }

        private static XElement CreateRecordElement(
            ChatSessionModel session,
            ChatSessionMessageRecordModel record)
        {
            return new XElement(
                "Message",
                new XAttribute("Id", record.Id),
                new XAttribute("Role", record.Role),
                new XElement("DisplayName", record.DisplayName),
                new XElement("AvatarPath", record.AvatarPath),
                new XElement("TimestampUtc", record.TimestampUtc.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture)),
                new XElement(
                    "ContentBlocks",
                    record.Blocks.Select(block => CreateBlockElement(session, block))));
        }

        private static XElement CreateBlockElement(
            ChatSessionModel session,
            ChatSessionContentBlockModel block)
        {
            var blockElement = new XElement(
                "Block",
                new XAttribute("Id", block.Id),
                new XAttribute("Kind", block.Kind),
                new XAttribute("PartType", ToLegacyPartType(block.Kind)),
                new XElement("Title", block.Title ?? string.Empty),
                new XElement("Language", block.Language ?? string.Empty),
                new XElement("BadgeText", block.BadgeText ?? string.Empty),
                new XElement("IsStreaming", block.IsStreaming));

            AddOptionalAttribute(blockElement, "ToolCallID", block.ToolCallId);
            AddOptionalAttribute(blockElement, "CallerAgentID", block.CallerAgentId);
            AddOptionalAttribute(blockElement, "ResourcePath", block.ResourcePath);

            if (IsHostPreservedBlock(block.Kind))
            {
                blockElement.Add(CreatePreservedContentElement(session, block));
            }
            else
            {
                blockElement.Add(new XElement("Content", block.Content ?? string.Empty));
            }

            if (block.Metadata.Count > 0)
            {
                blockElement.Add(new XElement(
                    "Metadata",
                    block.Metadata
                        .OrderBy(item => item.Key, StringComparer.OrdinalIgnoreCase)
                        .Select(item => new XElement(
                            "Item",
                            new XAttribute("Key", item.Key),
                            item.Value ?? string.Empty))));
            }

            return blockElement;
        }

        private static XElement CreatePreservedContentElement(
            ChatSessionModel session,
            ChatSessionContentBlockModel block)
        {
            return new XElement(
                "SkyweaverPreservedContent",
                block.Kind switch
                {
                    ChatSessionContentBlockKind.ToolCall => CreateToolReferenceElement(
                        session,
                        block,
                        isInvocation: true),
                    ChatSessionContentBlockKind.ToolOutput or ChatSessionContentBlockKind.ToolReference => CreateToolReferenceElement(
                        session,
                        block,
                        isInvocation: false),
                    ChatSessionContentBlockKind.Image => new XElement(
                        "Image",
                        new XAttribute("Path", block.ResourcePath ?? block.Content ?? string.Empty)),
                    ChatSessionContentBlockKind.Audio => new XElement(
                        "Audio",
                        new XAttribute("Path", block.ResourcePath ?? block.Content ?? string.Empty)),
                    _ => XElement.Parse("<Content />")
                });
        }

        private static XElement CreateToolReferenceElement(
            ChatSessionModel session,
            ChatSessionContentBlockModel block,
            bool isInvocation)
        {
            var toolCallId = ChatSessionToolCallIdGenerator.Normalize(block.ToolCallId);
            if (toolCallId.Length == 0)
            {
                toolCallId = ChatSessionToolCallIdGenerator.Create(session);
                block.ToolCallId = toolCallId;
            }

            var resourceStore = new ChatSessionToolCallResourceStore();
            var filePath = isInvocation
                ? resourceStore.SaveInvocation(session, toolCallId, block.CallerAgentId, block.Content)
                : resourceStore.SaveOutput(session, toolCallId, block.CallerAgentId, block.Content);
            block.ResourcePath = filePath;

            var toolElement = new XElement("Tool", new XAttribute("ToolCallID", toolCallId));
            AddOptionalAttribute(toolElement, "CallerAgentID", block.CallerAgentId);
            return toolElement;
        }

        private static ChatSessionMessageRecordModel LoadRecordElement(
            ChatSessionModel session,
            XElement messageElement)
        {
            var role = Enum.TryParse<ChatMessageRole>((string?)messageElement.Attribute("Role"), true, out var parsedRole)
                ? parsedRole
                : ChatMessageRole.User;

            var record = new ChatSessionMessageRecordModel
            {
                Id = ((string?)messageElement.Attribute("Id") ?? Guid.NewGuid().ToString("N")).Trim(),
                Role = role,
                DisplayName = (string?)messageElement.Element("DisplayName") ?? GetDisplayName(role),
                AvatarPath = (string?)messageElement.Element("AvatarPath") ?? GetAvatarPath(role),
                TimestampUtc = ParseDateTime((string?)messageElement.Element("TimestampUtc")).ToUniversalTime()
            };

            var blockElements = messageElement.Element("ContentBlocks")?.Elements("Block");
            if (blockElements != null)
            {
                foreach (var blockElement in blockElements)
                {
                    record.Blocks.Add(LoadBlockElement(session, blockElement));
                }

                BackfillPersistedToolCallBlocks(session, record);
                return record;
            }

            var legacyPartElements = messageElement.Element("Parts")?.Elements("Part") ?? Enumerable.Empty<XElement>();
            foreach (var partElement in legacyPartElements)
            {
                record.Blocks.Add(LoadLegacyPartElement(partElement));
            }

            BackfillPersistedToolCallBlocks(session, record);
            return record;
        }

        private static void BackfillPersistedToolCallBlocks(
            ChatSessionModel session,
            ChatSessionMessageRecordModel record)
        {
            if (record.Blocks.Count == 0)
            {
                return;
            }

            var resourceStore = new ChatSessionToolCallResourceStore();
            var existingToolCallIds = record.Blocks
                .Where(block => block.Kind == ChatSessionContentBlockKind.ToolCall)
                .Select(block => ChatSessionToolCallIdGenerator.Normalize(block.ToolCallId))
                .Where(toolCallId => toolCallId.Length > 0)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            for (var index = 0; index < record.Blocks.Count; index++)
            {
                var block = record.Blocks[index];
                var normalizedToolCallId = ChatSessionToolCallIdGenerator.Normalize(block.ToolCallId);

                if (normalizedToolCallId.Length == 0 ||
                    existingToolCallIds.Contains(normalizedToolCallId) ||
                    block.Kind is not (ChatSessionContentBlockKind.ToolOutput or ChatSessionContentBlockKind.ToolReference))
                {
                    continue;
                }

                var invocationXml = resourceStore.LoadInvocation(session, normalizedToolCallId);
                if (string.IsNullOrWhiteSpace(invocationXml))
                {
                    continue;
                }

                var toolCallBlock = new ChatSessionContentBlockModel
                {
                    Kind = ChatSessionContentBlockKind.ToolCall,
                    Content = invocationXml,
                    Title = FirstNonEmpty(TryExtractToolName(invocationXml), block.Title ?? string.Empty),
                    BadgeText = "Tool Call",
                    IsStreaming = false,
                    ToolCallId = normalizedToolCallId,
                    CallerAgentId = block.CallerAgentId,
                    ResourcePath = block.ResourcePath
                };

                record.Blocks.Insert(index, toolCallBlock);
                existingToolCallIds.Add(normalizedToolCallId);
                index++;
            }
        }

        private static ChatSessionContentBlockModel LoadBlockElement(
            ChatSessionModel session,
            XElement blockElement)
        {
            var kind = Enum.TryParse<ChatSessionContentBlockKind>((string?)blockElement.Attribute("Kind"), true, out var parsedKind)
                ? parsedKind
                : ChatSessionContentBlockKind.Text;

            var toolCallId = NullIfWhiteSpace((string?)blockElement.Attribute("ToolCallID"))
                ?? NullIfWhiteSpace(blockElement.Element("SkyweaverPreservedContent")?
                    .Elements()
                    .FirstOrDefault(element => string.Equals(element.Name.LocalName, "Tool", StringComparison.OrdinalIgnoreCase))
                    ?.Attribute("ToolCallID")
                    ?.Value);
            var callerAgentId = NullIfWhiteSpace((string?)blockElement.Attribute("CallerAgentID"))
                ?? NullIfWhiteSpace(blockElement.Element("SkyweaverPreservedContent")?
                    .Elements()
                    .FirstOrDefault(element => string.Equals(element.Name.LocalName, "Tool", StringComparison.OrdinalIgnoreCase))
                    ?.Attribute("CallerAgentID")
                    ?.Value);
            var resourcePath = NullIfWhiteSpace((string?)blockElement.Attribute("ResourcePath"));
            var content = LoadBlockContent(session, kind, blockElement, toolCallId);

            var block = new ChatSessionContentBlockModel
            {
                Id = ((string?)blockElement.Attribute("Id") ?? Guid.NewGuid().ToString("N")).Trim(),
                Kind = kind,
                Content = content,
                Title = NullIfWhiteSpace((string?)blockElement.Element("Title")),
                Language = NullIfWhiteSpace((string?)blockElement.Element("Language")),
                BadgeText = NullIfWhiteSpace((string?)blockElement.Element("BadgeText")),
                IsStreaming = ParseBool((string?)blockElement.Element("IsStreaming")),
                ToolCallId = toolCallId,
                CallerAgentId = callerAgentId,
                ResourcePath = resourcePath
            };

            foreach (var itemElement in blockElement.Element("Metadata")?.Elements("Item") ?? Enumerable.Empty<XElement>())
            {
                var key = ((string?)itemElement.Attribute("Key") ?? string.Empty).Trim();
                if (key.Length > 0)
                {
                    block.Metadata[key] = itemElement.Value ?? string.Empty;
                }
            }

            return block;
        }

        private static ChatSessionContentBlockModel LoadLegacyPartElement(XElement partElement)
        {
            var partType = Enum.TryParse<ChatMessagePartType>((string?)partElement.Attribute("Type"), true, out var parsedPartType)
                ? parsedPartType
                : ChatMessagePartType.Text;

            return new ChatSessionContentBlockModel
            {
                Kind = ToBlockKind(partType),
                Content = (string?)partElement.Element("Content") ?? string.Empty,
                Title = NullIfWhiteSpace((string?)partElement.Element("Title")),
                Language = NullIfWhiteSpace((string?)partElement.Element("Language")),
                BadgeText = NullIfWhiteSpace((string?)partElement.Element("BadgeText")),
                IsStreaming = ParseBool((string?)partElement.Element("IsStreaming")),
                ToolCallId = NullIfWhiteSpace((string?)partElement.Attribute("ToolCallID")),
                CallerAgentId = NullIfWhiteSpace((string?)partElement.Attribute("CallerAgentID")),
                ResourcePath = NullIfWhiteSpace((string?)partElement.Attribute("ResourcePath"))
            };
        }

        private static string LoadBlockContent(
            ChatSessionModel session,
            ChatSessionContentBlockKind kind,
            XElement blockElement,
            string? toolCallId)
        {
            var resourceStore = new ChatSessionToolCallResourceStore();
            return kind switch
            {
                ChatSessionContentBlockKind.ToolCall => FirstNonEmpty(
                    resourceStore.LoadInvocation(session, toolCallId),
                    (string?)blockElement.Element("Content") ?? string.Empty),
                ChatSessionContentBlockKind.ToolOutput or ChatSessionContentBlockKind.ToolReference => FirstNonEmpty(
                    resourceStore.LoadOutput(session, toolCallId),
                    (string?)blockElement.Element("Content") ?? string.Empty),
                ChatSessionContentBlockKind.Image or ChatSessionContentBlockKind.Audio => FirstNonEmpty(
                    NullIfWhiteSpace((string?)blockElement.Attribute("ResourcePath")) ?? string.Empty,
                    blockElement.Element("SkyweaverPreservedContent")?
                        .Elements()
                        .FirstOrDefault()
                        ?.Attribute("Path")
                        ?.Value ?? string.Empty,
                    (string?)blockElement.Element("Content") ?? string.Empty),
                ChatSessionContentBlockKind.HostPreservedContent => blockElement.Element("SkyweaverPreservedContent")?.ToString(SaveOptions.DisableFormatting)
                    ?? (string?)blockElement.Element("Content")
                    ?? string.Empty,
                _ => (string?)blockElement.Element("Content") ?? string.Empty
            };
        }

        private static string TryExtractToolName(string? invocationXml)
        {
            if (string.IsNullOrWhiteSpace(invocationXml))
            {
                return string.Empty;
            }

            try
            {
                var document = XDocument.Parse(invocationXml, LoadOptions.PreserveWhitespace);
                var root = document.Root;
                var toolElement = root == null
                    ? null
                    : string.Equals(root.Name.LocalName, "Tool", StringComparison.OrdinalIgnoreCase)
                        ? root
                        : root.Elements()
                            .FirstOrDefault(element => string.Equals(element.Name.LocalName, "Tool", StringComparison.OrdinalIgnoreCase));

                return toolElement?.Attributes()
                    .FirstOrDefault(attribute =>
                        string.Equals(attribute.Name.LocalName, "ToolName", StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(attribute.Name.LocalName, "Name", StringComparison.OrdinalIgnoreCase))
                    ?.Value
                    ?.Trim() ?? string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        private static bool IsHostPreservedBlock(ChatSessionContentBlockKind kind)
        {
            return kind is ChatSessionContentBlockKind.ToolCall
                or ChatSessionContentBlockKind.ToolOutput
                or ChatSessionContentBlockKind.ToolReference
                or ChatSessionContentBlockKind.Image
                or ChatSessionContentBlockKind.Audio
                or ChatSessionContentBlockKind.HostPreservedContent;
        }

        private static ChatMessagePartType ToLegacyPartType(ChatSessionContentBlockKind kind)
        {
            return kind switch
            {
                ChatSessionContentBlockKind.Code => ChatMessagePartType.Code,
                ChatSessionContentBlockKind.Status => ChatMessagePartType.Status,
                ChatSessionContentBlockKind.Placeholder => ChatMessagePartType.Placeholder,
                ChatSessionContentBlockKind.StructuredXml => ChatMessagePartType.StructuredXml,
                ChatSessionContentBlockKind.ToolCall => ChatMessagePartType.ToolCall,
                ChatSessionContentBlockKind.ToolOutput or ChatSessionContentBlockKind.ToolReference => ChatMessagePartType.ToolOutput,
                ChatSessionContentBlockKind.Image => ChatMessagePartType.Image,
                ChatSessionContentBlockKind.Audio => ChatMessagePartType.Audio,
                ChatSessionContentBlockKind.HostPreservedContent => ChatMessagePartType.HostPreservedContent,
                ChatSessionContentBlockKind.Reasoning => ChatMessagePartType.Reasoning,
                _ => ChatMessagePartType.Text
            };
        }

        private static ChatSessionContentBlockKind ToBlockKind(ChatMessagePartType partType)
        {
            return partType switch
            {
                ChatMessagePartType.Code => ChatSessionContentBlockKind.Code,
                ChatMessagePartType.Status => ChatSessionContentBlockKind.Status,
                ChatMessagePartType.Placeholder => ChatSessionContentBlockKind.Placeholder,
                ChatMessagePartType.StructuredXml => ChatSessionContentBlockKind.StructuredXml,
                ChatMessagePartType.ToolCall => ChatSessionContentBlockKind.ToolCall,
                ChatMessagePartType.Tool or ChatMessagePartType.ToolOutput => ChatSessionContentBlockKind.ToolOutput,
                ChatMessagePartType.Image => ChatSessionContentBlockKind.Image,
                ChatMessagePartType.Audio => ChatSessionContentBlockKind.Audio,
                ChatMessagePartType.HostPreservedContent => ChatSessionContentBlockKind.HostPreservedContent,
                ChatMessagePartType.Reasoning => ChatSessionContentBlockKind.Reasoning,
                _ => ChatSessionContentBlockKind.Text
            };
        }

        private static void AddOptionalAttribute(XElement element, string name, string? value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                element.SetAttributeValue(name, value.Trim());
            }
        }

        private static string FirstNonEmpty(params string[] values)
        {
            return values.FirstOrDefault(value => !string.IsNullOrWhiteSpace(value)) ?? string.Empty;
        }

        private void EnsureRootFolder()
        {
            Directory.CreateDirectory(RootFolderPath);
        }

        private void HydratePaths(ChatSessionModel session)
        {
            if (string.IsNullOrWhiteSpace(session.SessionFolderPath))
            {
                session.SessionFolderPath = Path.Combine(RootFolderPath, session.Name);
            }

            if (string.IsNullOrWhiteSpace(session.ResourcesFolderPath))
            {
                session.ResourcesFolderPath = Path.Combine(session.SessionFolderPath, "ChatSessionResources");
            }

            if (string.IsNullOrWhiteSpace(session.SessionFilePath))
            {
                session.SessionFilePath = Path.Combine(session.SessionFolderPath, $"{session.Name}.xml");
            }
        }

        private static string ValidateSessionName(string sessionName)
        {
            var trimmedName = sessionName.Trim();
            if (string.IsNullOrWhiteSpace(trimmedName))
            {
                throw new InvalidOperationException("会话名称不能为空。");
            }

            var invalidChars = Path.GetInvalidFileNameChars();
            if (trimmedName.IndexOfAny(invalidChars) >= 0)
            {
                throw new InvalidOperationException("会话名称包含无效文件名字符。");
            }

            return trimmedName;
        }

        private static DateTime ParseDateTime(string? value)
        {
            return DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var parsed)
                ? parsed
                : DateTime.UtcNow;
        }

        private static string? NullIfWhiteSpace(string? value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value;
        }

        private static bool ParseBool(string? value)
        {
            return bool.TryParse(value, out var parsed) && parsed;
        }

        private static string GetDisplayName(ChatMessageRole role)
        {
            return role switch
            {
                ChatMessageRole.Assistant => "Skyweaver 助手",
                ChatMessageRole.System => "系统",
                _ => "用户"
            };
        }

        private static string GetAvatarPath(ChatMessageRole role)
        {
            return role switch
            {
                ChatMessageRole.Assistant => AssistantAvatarPath,
                ChatMessageRole.System => SystemAvatarPath,
                _ => UserAvatarPath
            };
        }
    }
}
