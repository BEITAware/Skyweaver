using Ferrita.Services.FerritaTools;

namespace Ferrita.Tools
{
    public sealed class WaitForAsyncToolsTool : IFerritaTool
    {
        public const string ToolName = "WaitForAsyncTools";

        private static readonly FerritaToolDefinition s_definition = new(
            ToolName,
            "Blocks the agent loop until the specified async tool calls have completed. This tool is internal and must be invoked synchronously.",
            "GuideBot",
            [
                new FerritaToolParameterDefinition(
                    FerritaBuiltInToolNames.WaitForAsyncToolsParameter,
                    "A JSON array of ToolCallId values to wait for. Example: [\"TC1\", \"TC2\"].",
                    FerritaToolParameterType.Json,
                    isRequired: true)
            ],
            isSystemTool: true,
            canBelongToToolKit: false,
            supportsAsyncInvocation: false);

        public FerritaToolDefinition Definition => s_definition;

        public Task<FerritaToolResult> ExecuteAsync(
            FerritaToolContext context,
            FerritaToolArguments arguments,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(
                FerritaToolResult.Failure(
                    "WaitForAsyncTools is handled by the agent loop runtime and cannot be executed through the generic tool manager."));
        }
    }
}
