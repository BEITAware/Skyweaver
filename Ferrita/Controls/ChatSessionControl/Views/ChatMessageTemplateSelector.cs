using System.Windows;
using System.Windows.Controls;
using Ferrita.Controls.ChatSessionControl.Models;

namespace Ferrita.Controls.ChatSessionControl.Views
{
    public class ChatMessageTemplateSelector : DataTemplateSelector
    {
        public DataTemplate? UserTemplate { get; set; }

        public DataTemplate? AssistantTemplate { get; set; }

        public DataTemplate? SystemTemplate { get; set; }

        public override DataTemplate? SelectTemplate(object item, DependencyObject container)
        {
            if (item is not ChatMessageModel message)
            {
                return base.SelectTemplate(item, container);
            }

            return message.Role switch
            {
                ChatMessageRole.User => UserTemplate,
                ChatMessageRole.Assistant => AssistantTemplate,
                ChatMessageRole.System => SystemTemplate,
                _ => UserTemplate
            };
        }
    }
}
