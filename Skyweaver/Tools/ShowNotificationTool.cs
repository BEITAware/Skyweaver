using System;
using System.Collections.Generic;
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
        public const string ToolName = "System_ShowNotification";

        private static readonly SkyweaverToolDefinition s_definition = new(
            ToolName,
            "Shows a transient notification message in the Skyweaver UI.",
            "Message",
            [
                new SkyweaverToolParameterDefinition(
                    "Message",
                    "The message to display in the notification.",
                    SkyweaverToolParameterType.String,
                    isRequired: true)
            ],
            isSystemTool: false,
            canBelongToToolKit: true,
            defaultAgentPermission: SkyweaverToolDefaultAgentPermission.Allow);

        public SkyweaverToolDefinition Definition => s_definition;

        public string GetPromptDescription(SkyweaverToolPromptDescriptionContext context)
        {
            ArgumentNullException.ThrowIfNull(context);
            return "Shows a transient notification in the application UI. Use this to provide brief status updates, alerts, or completion messages to the user without interrupting their workflow. Do not use this for long text or critical information that requires user interaction, as the notification may disappear.";
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

            try
            {
                var message = arguments.GetString("Message");
                if (string.IsNullOrWhiteSpace(message))
                {
                    return Task.FromResult(SkyweaverToolResult.Failure("Message parameter cannot be empty."));
                }

                NotificationService.Instance.ShowTransient(message);

                return Task.FromResult(SkyweaverToolResult.Success(
                    "Successfully showed the notification.",
                    new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase)
                    {
                        ["message"] = message
                    }));
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                return Task.FromResult(SkyweaverToolResult.Failure($"Failed to show notification: {ex.Message}"));
            }
        }
    }
}
