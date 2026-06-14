using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Skyweaver.Controls.ChatSessionControl.Views;
using Skyweaver.Services.SkyweaverTools;

namespace Skyweaver.Tools
{
    public sealed class SleepTool :
        ISkyweaverTool,
        ISkyweaverToolInvocationPresentationProvider,
        ISkyweaverToolPromptDescriptionProvider
    {
        public const string ToolName = "Sleep";

        private static readonly SkyweaverToolDefinition s_definition = new(
            ToolName,
            "Pauses execution for a specified number of seconds.",
            "Schedule",
            [
                new SkyweaverToolParameterDefinition(
                    "Seconds",
                    "The number of seconds to sleep (wait).",
                    SkyweaverToolParameterType.Number,
                    isRequired: true)
            ],
            defaultAgentPermission: SkyweaverToolDefaultAgentPermission.Allow);

        public SkyweaverToolDefinition Definition => s_definition;

        public string GetPromptDescription(SkyweaverToolPromptDescriptionContext context)
        {
            ArgumentNullException.ThrowIfNull(context);
            return "Pauses execution for a specified number of seconds. Useful when you need to wait for an external process, server, or file operation to complete before proceeding.";
        }

        public FrameworkElement? CreateInvocationPresentation(SkyweaverToolInvocationPresentationContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            return ToolInvocationCardFactory.Create(
                context,
                [
                    new ToolInvocationCardFieldDefinition("Seconds", "Seconds", "Waiting for seconds...")
                ]);
        }

        public async Task<SkyweaverToolResult> ExecuteAsync(
            SkyweaverToolContext context,
            SkyweaverToolArguments arguments,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            int seconds = arguments.GetInteger("Seconds", 0);

            if (seconds < 0)
            {
                return SkyweaverToolResult.Failure("Seconds parameter must be a non-negative number.");
            }

            // Limit sleep to avoid hanging the agent for too long (e.g., max 5 minutes)
            if (seconds > 300)
            {
                return SkyweaverToolResult.Failure("Seconds parameter cannot exceed 300 (5 minutes).");
            }

            try
            {
                await Task.Delay(TimeSpan.FromSeconds(seconds), cancellationToken);
                return SkyweaverToolResult.Success($"Slept for {seconds} seconds.");
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SleepTool execution failed: {ex}");
                return SkyweaverToolResult.Failure($"Failed to sleep: {ex.Message}");
            }
        }
    }
}
