using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Skyweaver.Controls.ChatSessionControl.Views;
using Skyweaver.Services.Notifications;
using Skyweaver.Services.SkyweaverTools;

namespace Skyweaver.Tools
{
    public sealed class ShowNotificationTool :
        ISkyweaverTool,
        ISkyweaverToolInvocationPresentationProvider,
        ISkyweaverToolPromptDescriptionProvider
    {
        public const string ToolName = "ShowNotification";

        private static readonly SkyweaverToolDefinition s_definition = new(
            ToolName,
            "Displays a transient notification message in the application UI.",
            "Notifications",
            [
                new SkyweaverToolParameterDefinition(
                    "Message",
                    "The text content of the notification to display to the user.",
                    SkyweaverToolParameterType.String,
                    isRequired: true)
            ],
            defaultAgentPermission: SkyweaverToolDefaultAgentPermission.Allow);

        public SkyweaverToolDefinition Definition => s_definition;

        public string GetPromptDescription(SkyweaverToolPromptDescriptionContext context)
        {
            ArgumentNullException.ThrowIfNull(context);
            return "Displays a short transient notification message to the user in the Skyweaver UI.";
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
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                var message = arguments.GetString("Message");
                if (string.IsNullOrWhiteSpace(message))
                {
                    return Task.FromResult(SkyweaverToolResult.Failure("Message cannot be empty."));
                }

                NotificationService.Instance.ShowTransient(message);

                return Task.FromResult(SkyweaverToolResult.Success($"Successfully displayed notification: '{message}'"));
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ShowNotificationTool execution failed: {ex}");
                return Task.FromResult(SkyweaverToolResult.Failure($"Failed to show notification: {ex.Message}"));
            }
        }
    }
}
