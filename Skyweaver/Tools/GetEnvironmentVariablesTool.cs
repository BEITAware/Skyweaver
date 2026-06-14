using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Skyweaver.Controls.ChatSessionControl.Views;
using Skyweaver.Services.SkyweaverTools;

namespace Skyweaver.Tools
{
    public sealed class GetEnvironmentVariablesTool :
        ISkyweaverTool,
        ISkyweaverToolInvocationPresentationProvider,
        ISkyweaverToolPromptDescriptionProvider
    {
        public const string ToolName = "GetEnvironmentVariables";

        private static readonly SkyweaverToolDefinition s_definition = new(
            ToolName,
            "Retrieves all environment variables for the current process.",
            "Console",
            [],
            defaultAgentPermission: SkyweaverToolDefaultAgentPermission.Allow);

        public SkyweaverToolDefinition Definition => s_definition;

        public string GetPromptDescription(SkyweaverToolPromptDescriptionContext context)
        {
            ArgumentNullException.ThrowIfNull(context);
            return "Retrieves all environment variables for the current process. Useful for debugging configuration and execution environment.";
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
                var variables = Environment.GetEnvironmentVariables();
                var builder = new StringBuilder();
                builder.AppendLine("=== Environment Variables ===");

                foreach (DictionaryEntry entry in variables.Cast<DictionaryEntry>().OrderBy(e => e.Key))
                {
                    builder.AppendLine($"{entry.Key}={entry.Value}");
                }

                return Task.FromResult(SkyweaverToolResult.Success(builder.ToString().TrimEnd()));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetEnvironmentVariablesTool execution failed: {ex}");
                return Task.FromResult(SkyweaverToolResult.Failure($"Failed to retrieve environment variables: {ex.Message}"));
            }
        }
    }
}
