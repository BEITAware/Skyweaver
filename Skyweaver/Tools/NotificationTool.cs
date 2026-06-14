using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Skyweaver.Controls.ChatSessionControl.Views;
using Skyweaver.Services.Notifications;
using Skyweaver.Services.SkyweaverTools;

namespace Skyweaver.Tools
{
    public sealed class NotificationTool :
        ISkyweaverTool,
        ISkyweaverToolInvocationPresentationProvider,
        ISkyweaverToolPromptDescriptionProvider
    {
        public const string ToolName = "ShowNotification";

        private static readonly SkyweaverToolDefinition s_definition = new(
            ToolName,
            "Shows an in-app transient notification message to the user.",
            "Device",
            [
                new SkyweaverToolParameterDefinition(
                    "Message",
                    "The notification message to show.",
                    SkyweaverToolParameterType.String,
                    isRequired: true)
            ],
            defaultAgentPermission: SkyweaverToolDefaultAgentPermission.Allow);

        public SkyweaverToolDefinition Definition => s_definition;

        public string GetPromptDescription(SkyweaverToolPromptDescriptionContext context)
        {
            ArgumentNullException.ThrowIfNull(context);
            return "Shows an in-app transient notification message to the user. Useful for alerting the user of important events or task completion.";
        }

        public FrameworkElement? CreateInvocationPresentation(SkyweaverToolInvocationPresentationContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            return ToolInvocationCardFactory.Create(
                context,
                [
                    new ToolInvocationCardFieldDefinition("Message", "Message", "Waiting for message...")
                ]);
        }

        public Task<SkyweaverToolResult> ExecuteAsync(
            SkyweaverToolContext context,
            SkyweaverToolArguments arguments,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var message = arguments.GetString("Message");

            if (string.IsNullOrWhiteSpace(message))
            {
                return Task.FromResult(SkyweaverToolResult.Failure("Message cannot be empty."));
            }

            try
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    NotificationService.Instance.ShowTransient(message);
                });
                return Task.FromResult(SkyweaverToolResult.Success($"Notification shown: {message}"));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"NotificationTool execution failed: {ex}");
                return Task.FromResult(SkyweaverToolResult.Failure($"Failed to show notification: {ex.Message}"));
            }
        }
    }
}
