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
            "Terminates a running process on the host machine by its Process ID.",
            "Device",
            [
                new SkyweaverToolParameterDefinition(
                    "ProcessId",
                    "The Process ID (PID) of the process to terminate.",
                    SkyweaverToolParameterType.Integer,
                    isRequired: true)
            ],
            defaultAgentPermission: SkyweaverToolDefaultAgentPermission.RequireConfirmation);

        public SkyweaverToolDefinition Definition => s_definition;

        public string GetPromptDescription(SkyweaverToolPromptDescriptionContext context)
        {
            ArgumentNullException.ThrowIfNull(context);
            return "Terminates a running process on the host machine using its Process ID (PID). Use this tool with caution as killing critical system processes can cause instability.";
        }

        public FrameworkElement? CreateInvocationPresentation(SkyweaverToolInvocationPresentationContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            return ToolInvocationCardFactory.Create(
                context,
                [
                    new ToolInvocationCardFieldDefinition("Process ID", "ProcessId", "No PID specified")
                ]);
        }

        public Task<SkyweaverToolResult> ExecuteAsync(
            SkyweaverToolContext context,
            SkyweaverToolArguments arguments,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var processId = arguments.GetInteger("ProcessId", -1);
            if (processId == -1)
            {
                return Task.FromResult(SkyweaverToolResult.Failure("Missing required parameter: ProcessId."));
            }

            try
            {
                var process = Process.GetProcessById(processId);
                var processName = process.ProcessName;

                process.Kill();
                // Wait a bit to let it terminate
                process.WaitForExit(1000);

                return Task.FromResult(SkyweaverToolResult.Success(
                    $"Successfully terminated process '{processName}' (PID: {processId})."
                ));
            }
            catch (ArgumentException)
            {
                return Task.FromResult(SkyweaverToolResult.Failure(
                    $"No process found with PID: {processId}."
                ));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"KillProcessTool execution failed: {ex}");
                return Task.FromResult(SkyweaverToolResult.Failure(
                    $"Failed to terminate process (PID: {processId}): {ex.Message}"
                ));
            }
        }
    }
}
