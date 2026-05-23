using System.Windows;
using System.Windows.Controls;
using Skyweaver.Controls.ChatSessionControl.Models;

namespace Skyweaver.Controls.ChatSessionControl.Views
{
    public class ChatMessagePartTemplateSelector : DataTemplateSelector
    {
        public DataTemplate? TextTemplate { get; set; }

        public DataTemplate? CodeTemplate { get; set; }

        public DataTemplate? StatusTemplate { get; set; }

        public DataTemplate? PlaceholderTemplate { get; set; }

        public DataTemplate? ToolCallTemplate { get; set; }

        public DataTemplate? ToolOutputTemplate { get; set; }

        public DataTemplate? StructuredXmlTemplate { get; set; }

        public DataTemplate? ImageTemplate { get; set; }

        public DataTemplate? MediaResourceTemplate { get; set; }

        public DataTemplate? ReasoningTemplate { get; set; }

        public override DataTemplate? SelectTemplate(object item, DependencyObject container)
        {
            if (item is not ChatMessagePartModel part)
            {
                return base.SelectTemplate(item, container);
            }

            return part.PartType switch
            {
                ChatMessagePartType.Text => TextTemplate,
                ChatMessagePartType.Code => CodeTemplate,
                ChatMessagePartType.Status => StatusTemplate,
                ChatMessagePartType.Placeholder => PlaceholderTemplate,
                ChatMessagePartType.ToolCall => ToolCallTemplate ?? StatusTemplate,
                ChatMessagePartType.ToolOutput => ToolOutputTemplate ?? StatusTemplate,
                ChatMessagePartType.StructuredXml => StructuredXmlTemplate ?? TextTemplate,
                ChatMessagePartType.Image => ImageTemplate ?? StatusTemplate ?? TextTemplate,
                ChatMessagePartType.Audio => MediaResourceTemplate ?? StatusTemplate ?? TextTemplate,
                ChatMessagePartType.Video => MediaResourceTemplate ?? StatusTemplate ?? TextTemplate,
                ChatMessagePartType.Document => MediaResourceTemplate ?? StatusTemplate ?? TextTemplate,
                ChatMessagePartType.TextAttachment => MediaResourceTemplate ?? StatusTemplate ?? TextTemplate,
                ChatMessagePartType.HostPreservedContent => StructuredXmlTemplate ?? TextTemplate,
                ChatMessagePartType.Reasoning => ReasoningTemplate ?? StatusTemplate ?? TextTemplate,
                ChatMessagePartType.Tool => ToolOutputTemplate ?? StatusTemplate,
                _ => TextTemplate
            };
        }
    }
}
