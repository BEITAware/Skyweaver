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
            Directory.CreateDirectory(session.ResourcesFolderPath);

            session.UpdatedAt = DateTime.UtcNow;

            var conversationElement = new XElement(
                "Conversation",
                session.Messages.Select(message =>
                    new XElement("Message",
                        new XAttribute("Id", message.Id),
                        new XAttribute("Role", message.Role),
                        new XElement("DisplayName", message.DisplayName),
                        new XElement("AvatarPath", message.AvatarPath),
                        new XElement("TimestampUtc", message.Timestamp.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture)),
                        new XElement("Parts",
                            message.Parts.Select(part =>
                                new XElement("Part",
                                    new XAttribute("Type", part.PartType),
                                    new XElement("Title", part.Title ?? string.Empty),
                                    new XElement("Language", part.Language ?? string.Empty),
                                    new XElement("BadgeText", part.BadgeText ?? string.Empty),
                                    new XElement("IsStreaming", part.IsStreaming),
                                    new XElement("Content", part.Content)))))));

            var conversationHistoryElement = new XElement(
                "ConversationHistory",
                session.ConversationHistory.Select(message =>
                    new XElement("Message",
                        new XAttribute("Role", message.Role),
                        new XElement("AuthorName", message.AuthorName ?? string.Empty),
                        new XElement("Content", message.Content ?? string.Empty))));

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
                    conversationElement,
                    conversationHistoryElement));

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

            var messageElements = root.Element("Conversation")?.Elements("Message") ?? Enumerable.Empty<XElement>();
            foreach (var messageElement in messageElements)
            {
                var role = Enum.TryParse<ChatMessageRole>((string?)messageElement.Attribute("Role"), true, out var parsedRole)
                    ? parsedRole
                    : ChatMessageRole.User;

                var message = new ChatMessageModel(
                    role,
                    (string?)messageElement.Element("DisplayName") ?? GetDisplayName(role),
                    (string?)messageElement.Element("AvatarPath") ?? GetAvatarPath(role),
                    ParseDateTime((string?)messageElement.Element("TimestampUtc")));

                var partElements = messageElement.Element("Parts")?.Elements("Part") ?? Enumerable.Empty<XElement>();
                foreach (var partElement in partElements)
                {
                    var partType = Enum.TryParse<ChatMessagePartType>((string?)partElement.Attribute("Type"), true, out var parsedPartType)
                        ? parsedPartType
                        : ChatMessagePartType.Text;

                    message.Parts.Add(new ChatMessagePartModel(
                        partType,
                        (string?)partElement.Element("Content") ?? string.Empty,
                        NullIfWhiteSpace((string?)partElement.Element("Title")),
                        NullIfWhiteSpace((string?)partElement.Element("Language")),
                        NullIfWhiteSpace((string?)partElement.Element("BadgeText")),
                        ParseBool((string?)partElement.Element("IsStreaming"))));
                }

                session.Messages.Add(message);
            }

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
