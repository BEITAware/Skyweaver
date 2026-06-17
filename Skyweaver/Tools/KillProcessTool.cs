using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Skyweaver.Controls.ChatSessionControl.Views;
using Skyweaver.Services.SkyweaverTools;

namespace Skyweaver.Tools
{
    public sealed class KillProcessTool :
        ISkyweaverTool,
        ISkyweaverToolInvocationPresentationProvider,
        ISkyweaverToolPromptDescriptionProvider
    {
        public const string ToolName = "KillProcess";

        private static readonly SkyweaverToolDefinition s_definition = new(
            ToolName,
            "Kills a running process by its Process ID (PID).",
            "Device",
            [
                new SkyweaverToolParameterDefinition(
                    "ProcessId",
                    "The ID of the process to kill.",
                    SkyweaverToolParameterType.Integer,
                    isRequired: true)
            ],
            defaultAgentPermission: SkyweaverToolDefaultAgentPermission.RequireConfirmation);

        public SkyweaverToolDefinition Definition => s_definition;

        public string GetPromptDescription(SkyweaverToolPromptDescriptionContext context)
        {
            ArgumentNullException.ThrowIfNull(context);
            return "Kills a running process on the host machine by its Process ID (PID). Requires user confirmation before execution.";
        }

        public FrameworkElement? CreateInvocationPresentation(SkyweaverToolInvocationPresentationContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            return ToolInvocationCardFactory.Create(
                context,
                [
                    new ToolInvocationCardFieldDefinition("Process ID", "ProcessId", "Waiting for PID...")
                ]);
        }

        public Task<SkyweaverToolResult> ExecuteAsync(
            SkyweaverToolContext context,
            SkyweaverToolArguments arguments,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var argumentValue = arguments.GetValue("ProcessId");
            if (argumentValue == null || argumentValue.Value == null)
            {
                 return Task.FromResult(SkyweaverToolResult.Failure("ProcessId must be provided."));
            }

            int processId = Convert.ToInt32(argumentValue.Value);

            try
            {
                var process = Process.GetProcessById(processId);
                process.Kill();
                return Task.FromResult(SkyweaverToolResult.Success($"Successfully killed process with PID {processId} ({process.ProcessName})."));
            }
            catch (ArgumentException)
            {
                return Task.FromResult(SkyweaverToolResult.Failure($"No process found with PID {processId}."));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"KillProcessTool execution failed: {ex}");
                return Task.FromResult(SkyweaverToolResult.Failure($"Failed to kill process {processId}: {ex.Message}"));
            }
        }
    }
}
