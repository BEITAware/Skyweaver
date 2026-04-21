using System.Collections.ObjectModel;
using System.Windows;
using System.Xml.Linq;
using Skyweaver.Infrastructure.Mvvm;
using Skyweaver.Services.SkyweaverTools;

namespace Skyweaver.Controls.ChatSessionControl.Models
{
    public class ChatMessagePartModel : ObservableObject
    {
        private ChatMessagePartType _partType;
        private string _content;
        private string? _title;
        private string? _language;
        private string? _badgeText;
        private bool _isStreaming;
        private SkyweaverToolInvocationPresentationState? _toolPresentationState;
        private FrameworkElement? _toolPresentationView;

        public ChatMessagePartType PartType
        {
            get => _partType;
            set
            {
                if (SetProperty(ref _partType, value))
                {
                    RebuildStructuredXmlNodes();
                }
            }
        }

        public string Content
        {
            get => _content;
            set
            {
                if (SetProperty(ref _content, value))
                {
                    RebuildStructuredXmlNodes();
                }
            }
        }

        public string? BadgeText
        {
            get => _badgeText;
            set => SetProperty(ref _badgeText, value);
        }

        public bool IsStreaming
        {
            get => _isStreaming;
            set => SetProperty(ref _isStreaming, value);
        }

        public string? Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public string? Language
        {
            get => _language;
            set => SetProperty(ref _language, value);
        }

        public ObservableCollection<ChatStructuredXmlNodeModel> StructuredXmlNodes { get; } = new();

        public bool HasStructuredXmlNodes => StructuredXmlNodes.Count > 0;

        public SkyweaverToolInvocationPresentationState? ToolPresentationState
        {
            get => _toolPresentationState;
            private set => SetProperty(ref _toolPresentationState, value);
        }

        public FrameworkElement? ToolPresentationView
        {
            get => _toolPresentationView;
            private set => SetProperty(ref _toolPresentationView, value);
        }

        public ChatMessagePartModel(
            ChatMessagePartType partType,
            string content,
            string? title = null,
            string? language = null,
            string? badgeText = null,
            bool isStreaming = false)
        {
            _partType = partType;
            _content = content;
            _title = title;
            _language = language;
            _badgeText = badgeText;
            _isStreaming = isStreaming;
            StructuredXmlNodes.CollectionChanged += (_, _) => OnPropertyChanged(nameof(HasStructuredXmlNodes));
            RebuildStructuredXmlNodes();
        }

        public static ChatMessagePartModel CreateText(string content, string? title = null)
        {
            return new ChatMessagePartModel(ChatMessagePartType.Text, content, title);
        }

        public static ChatMessagePartModel CreateCode(string content, string? title = null, string? language = null)
        {
            return new ChatMessagePartModel(ChatMessagePartType.Code, content, title, language);
        }

        public static ChatMessagePartModel CreateStatus(string content, string? title = null)
        {
            return new ChatMessagePartModel(ChatMessagePartType.Status, content, title);
        }

        public static ChatMessagePartModel CreatePlaceholder(string content, string? title = null)
        {
            return new ChatMessagePartModel(ChatMessagePartType.Placeholder, content, title);
        }

        public static ChatMessagePartModel CreateTool(string content, string? title = null)
        {
            return CreateToolOutput(content, title);
        }

        public static ChatMessagePartModel CreateToolCall(string content, string? title = null, bool isStreaming = false)
        {
            return new ChatMessagePartModel(ChatMessagePartType.ToolCall, content, title, badgeText: "Tool Call", isStreaming: isStreaming);
        }

        public static ChatMessagePartModel CreateToolOutput(string content, string? title = null, bool isStreaming = false)
        {
            return new ChatMessagePartModel(ChatMessagePartType.ToolOutput, content, title, badgeText: "Tool Output", isStreaming: isStreaming);
        }

        public static ChatMessagePartModel CreateStructuredXml(string xmlText, string? title = null)
        {
            return new ChatMessagePartModel(ChatMessagePartType.StructuredXml, xmlText, title, badgeText: "XML");
        }

        public void AttachToolPresentation(
            SkyweaverToolInvocationPresentationState state,
            FrameworkElement view)
        {
            ToolPresentationState = state ?? throw new ArgumentNullException(nameof(state));
            ToolPresentationView = view ?? throw new ArgumentNullException(nameof(view));
        }

        private void RebuildStructuredXmlNodes()
        {
            StructuredXmlNodes.Clear();

            if (PartType != ChatMessagePartType.StructuredXml || string.IsNullOrWhiteSpace(Content))
            {
                return;
            }

            try
            {
                var document = XDocument.Parse(Content, LoadOptions.PreserveWhitespace);
                if (document.Root == null)
                {
                    return;
                }

                StructuredXmlNodes.Add(CreateXmlNode(document.Root));
            }
            catch
            {
                // Keep the raw text visible even when XML is incomplete or malformed.
            }
        }

        private static ChatStructuredXmlNodeModel CreateXmlNode(XElement element)
        {
            var node = new ChatStructuredXmlNodeModel
            {
                Name = element.Name.LocalName,
                Value = element.HasElements
                    ? string.Empty
                    : (element.Value ?? string.Empty).Trim()
            };

            foreach (var attribute in element.Attributes())
            {
                node.Children.Add(new ChatStructuredXmlNodeModel
                {
                    Name = $"@{attribute.Name.LocalName}",
                    Value = attribute.Value
                });
            }

            foreach (var child in element.Elements())
            {
                node.Children.Add(CreateXmlNode(child));
            }

            return node;
        }
    }
}
