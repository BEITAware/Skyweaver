using System;
using System.Threading;
using System.Threading.Tasks;
using Ferrita.Services.Notifications;
using Ferrita.Services.FerritaTools;

namespace Ferrita.Tools
{
    public sealed class ShowNotificationTool : IFerritaTool, IFerritaToolPromptDescriptionProvider
    {
        public const string ToolName = "ShowNotification";

        private static readonly FerritaToolDefinition s_definition = new(
            ToolName,
            "Shows a transient in-app notification message to the user.",
            "UI",
            [
                new FerritaToolParameterDefinition(
                    "Message",
                    "The message to display in the notification.",
                    FerritaToolParameterType.String,
                    isRequired: true)
            ],
            defaultAgentPermission: FerritaToolDefaultAgentPermission.Allow);

        public FerritaToolDefinition Definition => s_definition;

        public string GetPromptDescription(FerritaToolPromptDescriptionContext context)
        {
            return "Shows a transient in-app notification message to the user. Use this to provide non-intrusive feedback or alerts.";
        }

        public Task<FerritaToolResult> ExecuteAsync(
            FerritaToolContext context,
            FerritaToolArguments arguments,
            CancellationToken cancellationToken = default)
        {
            try
            {
                string? message = arguments.GetString("Message");
                if (message == null)
                {
                    return Task.FromResult(FerritaToolResult.Failure("Missing or invalid 'Message' parameter."));
                }

                NotificationService.Instance.ShowTransient(message);

                return Task.FromResult(FerritaToolResult.Success("Notification shown successfully."));
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                return Task.FromResult(FerritaToolResult.Failure($"An unexpected error occurred: {ex.Message}"));
            }
        }
    }
}
