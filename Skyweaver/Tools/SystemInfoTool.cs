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
            "Retrieves basic system information like OS version, CPU cores, and memory usage.",
            "Device",
            [],
            defaultAgentPermission: SkyweaverToolDefaultAgentPermission.Allow);

        public SkyweaverToolDefinition Definition => s_definition;

        public string GetPromptDescription(SkyweaverToolPromptDescriptionContext context)
        {
            ArgumentNullException.ThrowIfNull(context);
            return "Retrieves basic information about the host operating system, hardware (CPU architecture, logical cores), and current application process memory usage. Use this to gather context about the environment you are running in.";
        }

        public FrameworkElement? CreateInvocationPresentation(SkyweaverToolInvocationPresentationContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            return ToolInvocationCardFactory.CreateDefault(
                context.State,
                "Retrieving system information...",
                context.IconPath);
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
                builder.AppendLine($"OS Description: {RuntimeInformation.OSDescription}");
                builder.AppendLine($"OS Architecture: {RuntimeInformation.OSArchitecture}");
                builder.AppendLine($"Framework Description: {RuntimeInformation.FrameworkDescription}");
                builder.AppendLine($"Process Architecture: {RuntimeInformation.ProcessArchitecture}");
                builder.AppendLine($"Logical CPU Cores: {Environment.ProcessorCount}");

                using var process = Process.GetCurrentProcess();
                var workingSetMb = process.WorkingSet64 / (1024.0 * 1024.0);
                var privateMemoryMb = process.PrivateMemorySize64 / (1024.0 * 1024.0);

                builder.AppendLine($"Current Process Working Set: {workingSetMb:F2} MB");
                builder.AppendLine($"Current Process Private Memory: {privateMemoryMb:F2} MB");
                builder.AppendLine($"Machine Name: {Environment.MachineName}");
                builder.AppendLine($"User Domain Name: {Environment.UserDomainName}");
                builder.AppendLine($"User Name: {Environment.UserName}");
                builder.AppendLine($"System Directory: {Environment.SystemDirectory}");
                builder.AppendLine($"Current Directory: {Environment.CurrentDirectory}");

                return Task.FromResult(SkyweaverToolResult.Success(builder.ToString().TrimEnd()));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SystemInfoTool execution failed: {ex}");
                return Task.FromResult(SkyweaverToolResult.Failure($"Failed to retrieve system information: {ex.Message}"));
            }
        }
    }
}
