using System.Xml.Linq;

namespace Skyweaver.Controls.WorkflowEditorControl.Models
{
    public enum SessionFlowPortPayloadKind
    {
        NaturalLanguage = 0,
        XmlElement = 1
    }

    public sealed class SessionFlowPortPayload
    {
        private SessionFlowPortPayload(
            SessionFlowPortPayloadKind kind,
            string content,
            string? xmlElementName = null)
        {
            Kind = kind;
            Content = content ?? string.Empty;
            XmlElementName = xmlElementName;
        }

        public SessionFlowPortPayloadKind Kind { get; }

        public string Content { get; }

        public string? XmlElementName { get; }

        public bool IsNaturalLanguage => Kind == SessionFlowPortPayloadKind.NaturalLanguage;

        public bool IsXmlElement => Kind == SessionFlowPortPayloadKind.XmlElement;

        public static SessionFlowPortPayload FromNaturalLanguage(string text)
        {
            return new SessionFlowPortPayload(SessionFlowPortPayloadKind.NaturalLanguage, text ?? string.Empty);
        }

        public static SessionFlowPortPayload FromXmlElement(XElement element)
        {
            ArgumentNullException.ThrowIfNull(element);

            var clone = new XElement(element);
            return new SessionFlowPortPayload(
                SessionFlowPortPayloadKind.XmlElement,
                clone.ToString(SaveOptions.DisableFormatting),
                clone.Name.LocalName);
        }

        public static SessionFlowPortPayload FromXmlText(string xmlText)
        {
            if (!TryParseXmlElement(xmlText, out var element, out var errorMessage))
            {
                throw new InvalidOperationException(errorMessage);
            }

            return FromXmlElement(element);
        }

        public XElement ToXElement()
        {
            if (!TryParseXmlElement(Content, out var element, out var errorMessage))
            {
                throw new InvalidOperationException(errorMessage);
            }

            return element;
        }

        public static bool TryParseXmlElement(
            string? xmlText,
            out XElement element,
            out string errorMessage)
        {
            element = new XElement("Empty");
            errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(xmlText))
            {
                errorMessage = "XML 端口载荷不能为空。";
                return false;
            }

            try
            {
                var document = XDocument.Parse(xmlText, LoadOptions.PreserveWhitespace);
                if (document.Root == null)
                {
                    errorMessage = "XML 端口载荷缺少根节点。";
                    return false;
                }

                element = new XElement(document.Root);
                return true;
            }
            catch (Exception ex)
            {
                errorMessage = $"XML 端口载荷解析失败：{ex.Message}";
                return false;
            }
        }
    }
}
