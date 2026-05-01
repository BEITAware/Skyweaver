using System.Xml.Linq;
using Skyweaver.Controls.LanguageModelConfigurationControl.Services;

namespace Skyweaver.Controls.WorkflowEditorControl.Models
{
    public enum SessionFlowPayloadKind
    {
        NaturalLanguage = 0,
        StructuredXml = 1
    }

    public sealed class SessionFlowPayload
    {
        private SessionFlowPayload(
            SessionFlowPayloadKind kind,
            string content,
            IEnumerable<LanguageModelChatContentBlock>? contentBlocks = null)
        {
            Kind = kind;
            Content = content ?? string.Empty;
            ContentBlocks = NormalizeContentBlocks(contentBlocks);
        }

        public SessionFlowPayloadKind Kind { get; }

        public string Content { get; }

        public IReadOnlyList<LanguageModelChatContentBlock> ContentBlocks { get; }

        public bool IsNaturalLanguage => Kind == SessionFlowPayloadKind.NaturalLanguage;

        public bool IsStructuredXml => Kind == SessionFlowPayloadKind.StructuredXml;

        public static SessionFlowPayload FromNaturalLanguage(string text)
        {
            return new SessionFlowPayload(SessionFlowPayloadKind.NaturalLanguage, text ?? string.Empty);
        }

        public static SessionFlowPayload FromNaturalLanguage(
            string text,
            IEnumerable<LanguageModelChatContentBlock>? contentBlocks)
        {
            return new SessionFlowPayload(
                SessionFlowPayloadKind.NaturalLanguage,
                text ?? string.Empty,
                contentBlocks);
        }

        public static SessionFlowPayload FromStructuredXml(string xmlText)
        {
            if (!TryNormalizeXml(xmlText, out var normalizedXml, out var errorMessage))
            {
                throw new InvalidOperationException(errorMessage);
            }

            return new SessionFlowPayload(SessionFlowPayloadKind.StructuredXml, normalizedXml);
        }

        public static bool TryCreate(
            SessionFlowPortType portType,
            string content,
            out SessionFlowPayload? payload,
            out string errorMessage)
        {
            if (portType == SessionFlowPortType.XmlField)
            {
                if (!TryNormalizeXml(content, out var normalizedXml, out errorMessage))
                {
                    payload = null;
                    return false;
                }

                payload = new SessionFlowPayload(SessionFlowPayloadKind.StructuredXml, normalizedXml);
                return true;
            }

            payload = new SessionFlowPayload(SessionFlowPayloadKind.NaturalLanguage, content ?? string.Empty);
            errorMessage = string.Empty;
            return true;
        }

        private static IReadOnlyList<LanguageModelChatContentBlock> NormalizeContentBlocks(
            IEnumerable<LanguageModelChatContentBlock>? contentBlocks)
        {
            if (contentBlocks == null)
            {
                return Array.Empty<LanguageModelChatContentBlock>();
            }

            return contentBlocks
                .Where(block => block != null)
                .Select(block => block.Clone())
                .ToArray();
        }

        private static bool TryNormalizeXml(
            string? xmlText,
            out string normalizedXml,
            out string errorMessage)
        {
            normalizedXml = string.Empty;
            errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(xmlText))
            {
                errorMessage = "XML 载荷不能为空。";
                return false;
            }

            try
            {
                var document = XDocument.Parse(xmlText, LoadOptions.PreserveWhitespace);
                if (document.Root == null)
                {
                    errorMessage = "XML 载荷缺少根节点。";
                    return false;
                }

                normalizedXml = document.ToString();
                return true;
            }
            catch (Exception ex)
            {
                errorMessage = $"XML 载荷解析失败：{ex.Message}";
                return false;
            }
        }
    }
}
