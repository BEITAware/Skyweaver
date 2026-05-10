using Skyweaver.Services.SkyweaverTools;

namespace Skyweaver.Tools
{
    public sealed class WaitForAsyncToolsTool : ISkyweaverTool
    {
        public const string ToolName = "WaitForAsyncTools";

        private static readonly SkyweaverToolDefinition s_definition = new(
            ToolName,
            "Blocks the agent loop until the specified async tool calls have completed. This tool is internal and must be invoked synchronously.",
            "GuideBot",
            [
                new SkyweaverToolParameterDefinition(
                    SkyweaverBuiltInToolNames.WaitForAsyncToolsParameter,
                    "A JSON array of ToolCallId values to wait for. Example: [\"TC1\", \"TC2\"].",
                    SkyweaverToolParameterType.Json,
                    isRequired: true)
            ],
            isSystemTool: true,
            canBelongToToolKit: false,
            supportsAsyncInvocation: false);

        public SkyweaverToolDefinition Definition => s_definition;

        public Task<SkyweaverToolResult> ExecuteAsync(
            SkyweaverToolContext context,
            SkyweaverToolArguments arguments,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(
                SkyweaverToolResult.Failure(
                    "WaitForAsyncTools is handled by the agent loop runtime and cannot be executed through the generic tool manager."));
        }
    }
}
