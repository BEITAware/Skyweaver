using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Skyweaver.Controls.ChatSessionControl.Views;
using Skyweaver.Services.SkyweaverTools;

namespace Skyweaver.Tools
{
    public sealed class GenerateUuidTool :
        ISkyweaverTool,
        ISkyweaverToolInvocationPresentationProvider,
        ISkyweaverToolPromptDescriptionProvider
    {
        public const string ToolName = "GenerateUuid";

        private static readonly SkyweaverToolDefinition s_definition = new(
            ToolName,
            "Generates one or more random UUIDs (v4).",
            "Create",
            [
                new SkyweaverToolParameterDefinition(
                    "Count",
                    "The number of UUIDs to generate. Defaults to 1.",
                    SkyweaverToolParameterType.String,
                    isRequired: false,
                    defaultValue: "1")
            ],
            defaultAgentPermission: SkyweaverToolDefaultAgentPermission.Allow);

        public SkyweaverToolDefinition Definition => s_definition;

        public string GetPromptDescription(SkyweaverToolPromptDescriptionContext context)
        {
            ArgumentNullException.ThrowIfNull(context);
            return "Generates random UUIDs (version 4). Useful when you need unique identifiers for data creation, mock data, or scripting.";
        }

        public FrameworkElement? CreateInvocationPresentation(SkyweaverToolInvocationPresentationContext context)
        {
            ArgumentNullException.ThrowIfNull(context);
            return ToolInvocationCardFactory.Create(
                context,
                [
                    new ToolInvocationCardFieldDefinition("Count", "Count", "Default is 1")
                ]);
        }

        public Task<SkyweaverToolResult> ExecuteAsync(
            SkyweaverToolContext context,
            SkyweaverToolArguments arguments,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var countStr = arguments.GetString("Count");
            int count = 1;

            if (!string.IsNullOrWhiteSpace(countStr))
            {
                if (!int.TryParse(countStr, out count) || count < 1)
                {
                    return Task.FromResult(SkyweaverToolResult.Failure("Count must be a positive integer."));
                }
            }

            // Cap the count to prevent abuse
            if (count > 100)
            {
                count = 100;
            }

            try
            {
                var builder = new StringBuilder();
                for (int i = 0; i < count; i++)
                {
                    builder.AppendLine(Guid.NewGuid().ToString());
                }

                return Task.FromResult(SkyweaverToolResult.Success(builder.ToString().TrimEnd()));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GenerateUuidTool execution failed: {ex}");
                return Task.FromResult(SkyweaverToolResult.Failure($"Failed to generate UUIDs: {ex.Message}"));
            }
        }
    }
}
