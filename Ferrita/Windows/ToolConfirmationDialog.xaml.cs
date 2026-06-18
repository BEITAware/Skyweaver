using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using Ferrita.Services.Localization;

namespace Ferrita.Windows
{
    public sealed class ToolConfirmationDialogModel
    {
        public string ToolName { get; init; } = string.Empty;

        public string PromptText { get; init; } = string.Empty;

        public string MetadataText { get; init; } = string.Empty;

        public string InvocationXml { get; init; } = string.Empty;

        public FrameworkElement? InvocationPreview { get; init; }
    }

    public partial class ToolConfirmationDialog : Window
    {
        public ToolConfirmationDialog(ToolConfirmationDialogModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            InitializeComponent();

            Title = string.IsNullOrWhiteSpace(model.ToolName)
                ? L("ToolConfirmation.Title", "工具调用确认")
                : string.Format(L("ToolConfirmation.TitleWithToolFormat", "工具调用确认 - {0}"), model.ToolName);

            PromptTextBlock.Text = model.PromptText ?? string.Empty;
            MetadataTextBlock.Text = model.MetadataText ?? string.Empty;
            InvocationPreviewHost.Content = model.InvocationPreview;

            var formattedInvocationXml = FormatInvocationXml(model.InvocationXml);
            if (string.IsNullOrWhiteSpace(formattedInvocationXml))
            {
                InvocationXmlSection.Visibility = Visibility.Collapsed;
            }
            else
            {
                InvocationXmlTextBlock.Text = formattedInvocationXml;
            }
        }

        private void ApproveButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void RejectButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private static string FormatInvocationXml(string? invocationXml)
        {
            if (string.IsNullOrWhiteSpace(invocationXml))
            {
                return string.Empty;
            }

            var normalizedInvocationXml = invocationXml.Trim();
            try
            {
                return XDocument.Parse(normalizedInvocationXml, LoadOptions.PreserveWhitespace).ToString();
            }
            catch
            {
                return normalizedInvocationXml;
            }
        }

        private static string L(string resourceKey, string fallback)
        {
            return LocalizationRuntime.Instance.GetString(resourceKey, fallback);
        }
    }
}
