using System;
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
            "Retrieves the current system date and time.",
            "Schedule", // Reusing an existing icon
            [],
            defaultAgentPermission: SkyweaverToolDefaultAgentPermission.Allow);

        public SkyweaverToolDefinition Definition => s_definition;

        public string GetPromptDescription(SkyweaverToolPromptDescriptionContext context)
        {
            ArgumentNullException.ThrowIfNull(context);
            return "Retrieves the current system date and time. Useful for timestamping or context-aware operations.";
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
                var now = DateTimeOffset.Now;
                var utcNow = DateTimeOffset.UtcNow;

                var result = $"Local Time: {now:O}\n" +
                             $"UTC Time: {utcNow:O}\n" +
                             $"Time Zone: {TimeZoneInfo.Local.StandardName} (UTC{now.Offset.Hours:+00;-00}:{now.Offset.Minutes:00})";

                return Task.FromResult(SkyweaverToolResult.Success(result));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetCurrentTimeTool execution failed: {ex}");
                return Task.FromResult(SkyweaverToolResult.Failure($"Failed to get current time: {ex.Message}"));
            }
        }
    }
}
