using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Skyweaver.Controls.ChatSessionControl.Views;
using Skyweaver.Services.SkyweaverTools;

namespace Skyweaver.Tools
{
    public sealed class ReadEnvironmentVariableTool :
        ISkyweaverTool,
        ISkyweaverToolInvocationPresentationProvider,
        ISkyweaverToolPromptDescriptionProvider
    {
        public const string ToolName = "ReadEnvironmentVariable";

        private static readonly SkyweaverToolDefinition s_definition = new(
            ToolName,
            "Reads the value of a specific environment variable.",
            "Variable",
            [
                new SkyweaverToolParameterDefinition(
                    "VariableName",
                    "The name of the environment variable to read (e.g., PATH, USERPROFILE).",
                    SkyweaverToolParameterType.String,
                    isRequired: true)
            ],
            defaultAgentPermission: SkyweaverToolDefaultAgentPermission.RequireConfirmation);

        public SkyweaverToolDefinition Definition => s_definition;

        public string GetPromptDescription(SkyweaverToolPromptDescriptionContext context)
        {
            ArgumentNullException.ThrowIfNull(context);
            return "Reads the value of a specific environment variable from the host system. Useful for system exploration and understanding the environment configuration.";
        }

        public FrameworkElement? CreateInvocationPresentation(SkyweaverToolInvocationPresentationContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            return ToolInvocationCardFactory.Create(
                context,
                [
                    new ToolInvocationCardFieldDefinition("Variable Name", "VariableName", "Waiting for variable name...")
                ]);
        }

        public Task<SkyweaverToolResult> ExecuteAsync(
            SkyweaverToolContext context,
            SkyweaverToolArguments arguments,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var variableName = arguments.GetString("VariableName") ?? string.Empty;

            if (string.IsNullOrWhiteSpace(variableName))
            {
                return Task.FromResult(SkyweaverToolResult.Failure("Variable name cannot be empty."));
            }

            try
            {
                var value = Environment.GetEnvironmentVariable(variableName);

                if (value == null)
                {
                    return Task.FromResult(SkyweaverToolResult.Success($"Environment variable '{variableName}' is not set or does not exist."));
                }

                return Task.FromResult(SkyweaverToolResult.Success($"{variableName}={value}"));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ReadEnvironmentVariableTool execution failed: {ex}");
                return Task.FromResult(SkyweaverToolResult.Failure($"Failed to read environment variable: {ex.Message}"));
            }
        }
    }
}
