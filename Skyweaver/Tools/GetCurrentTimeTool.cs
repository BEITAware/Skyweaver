using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Skyweaver.Controls.ChatSessionControl.Views;
using Skyweaver.Services.SkyweaverTools;

namespace Skyweaver.Tools
{
    public sealed class GetCurrentTimeTool :
        ISkyweaverTool,
        ISkyweaverToolInvocationPresentationProvider,
        ISkyweaverToolPromptDescriptionProvider
    {
        public const string ToolName = "GetCurrentTime";

        private static readonly SkyweaverToolDefinition s_definition = new(
            ToolName,
            "Retrieves the current date and time of the system, including UTC time and time zone information.",
            "Time",
            [],
            defaultAgentPermission: SkyweaverToolDefaultAgentPermission.Allow);

        public SkyweaverToolDefinition Definition => s_definition;

        public string GetPromptDescription(SkyweaverToolPromptDescriptionContext context)
        {
            ArgumentNullException.ThrowIfNull(context);
            return "Retrieves the current date and time of the host system, including local time, UTC time, and time zone information. Useful for grounding conversations in the current temporal context.";
        }

        public FrameworkElement? CreateInvocationPresentation(SkyweaverToolInvocationPresentationContext context)
        {
            ArgumentNullException.ThrowIfNull(context);
            return ToolInvocationCardFactory.Create(context, []);
        }

        public Task<SkyweaverToolResult> ExecuteAsync(
            SkyweaverToolContext context,
            SkyweaverToolArguments arguments,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                var now = DateTime.Now;
                var utcNow = DateTime.UtcNow;
                var timeZone = TimeZoneInfo.Local;

                var builder = new StringBuilder();
                builder.AppendLine("=== Current System Time ===");
                builder.AppendLine($"Local Time: {now:yyyy-MM-dd HH:mm:ss.fff}");
                builder.AppendLine($"UTC Time: {utcNow:yyyy-MM-dd HH:mm:ss.fff}");
                builder.AppendLine($"Time Zone: {timeZone.DisplayName} ({(timeZone.BaseUtcOffset.Ticks >= 0 ? "+" : "-")}{timeZone.BaseUtcOffset:hh\\:mm})");

                return Task.FromResult(SkyweaverToolResult.Success(builder.ToString().TrimEnd()));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetCurrentTimeTool execution failed: {ex}");
                return Task.FromResult(SkyweaverToolResult.Failure($"Failed to retrieve current time: {ex.Message}"));
            }
        }
    }
}
