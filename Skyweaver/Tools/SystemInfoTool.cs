using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Skyweaver.Controls.ChatSessionControl.Views;
using Skyweaver.Services.SkyweaverTools;

namespace Skyweaver.Tools
{
    public sealed class SystemInfoTool :
        ISkyweaverTool,
        ISkyweaverToolInvocationPresentationProvider,
        ISkyweaverToolPromptDescriptionProvider
    {
        public const string ToolName = "SystemInfo";

        private static readonly SkyweaverToolDefinition s_definition = new(
            ToolName,
            "Retrieves basic system information such as OS version, architecture, and memory.",
            "Device",
            [],
            defaultAgentPermission: SkyweaverToolDefaultAgentPermission.Allow);

        public SkyweaverToolDefinition Definition => s_definition;

        public string GetPromptDescription(SkyweaverToolPromptDescriptionContext context)
        {
            ArgumentNullException.ThrowIfNull(context);
            return "Retrieves basic system information including OS description, architecture, machine name, number of processors, and process architecture.";
        }

        public FrameworkElement? CreateInvocationPresentation(SkyweaverToolInvocationPresentationContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            return ToolInvocationCardFactory.Create(
                context,
                []);
        }

        public Task<SkyweaverToolResult> ExecuteAsync(
            SkyweaverToolContext context,
            SkyweaverToolArguments arguments,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                var builder = new StringBuilder();
                builder.AppendLine("System Information");
                builder.AppendLine("------------------");
                builder.AppendLine($"OS Description:        {RuntimeInformation.OSDescription}");
                builder.AppendLine($"OS Architecture:       {RuntimeInformation.OSArchitecture}");
                builder.AppendLine($"Process Architecture:  {RuntimeInformation.ProcessArchitecture}");
                builder.AppendLine($"Framework Description: {RuntimeInformation.FrameworkDescription}");
                builder.AppendLine($"Machine Name:          {Environment.MachineName}");
                builder.AppendLine($"Processor Count:       {Environment.ProcessorCount}");
                builder.AppendLine($"System Directory:      {Environment.SystemDirectory}");
                builder.AppendLine($"Current Directory:     {Environment.CurrentDirectory}");
                builder.AppendLine($"User Domain Name:      {Environment.UserDomainName}");
                builder.AppendLine($"User Name:             {Environment.UserName}");

                // Memory info is platform specific, but we can get the current process memory
                using var currentProcess = Process.GetCurrentProcess();
                builder.AppendLine($"Current Process ID:    {currentProcess.Id}");
                builder.AppendLine($"Process Memory (MB):   {currentProcess.WorkingSet64 / (1024.0 * 1024.0):F2}");

                return Task.FromResult(SkyweaverToolResult.Success(builder.ToString().TrimEnd()));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SystemInfoTool execution failed: {ex}");
                return Task.FromResult(SkyweaverToolResult.Failure($"Failed to retrieve system info: {ex.Message}"));
            }
        }
    }
}
