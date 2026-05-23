using Skyweaver.Services.SkyweaverTools;

namespace Skyweaver.Tools
{
    public sealed class GetAsyncToolProgressTool : ISkyweaverTool
    {
        private static readonly SkyweaverToolDefinition s_definition = new(
            SkyweaverBuiltInToolNames.GetAsyncToolProgress,
            "Returns the latest progress snapshot for specified async tool calls. This tool is internal and must be invoked synchronously.",
            "GuideBot",
            [
                new SkyweaverToolParameterDefinition(
                    SkyweaverBuiltInToolNames.GetAsyncToolProgressParameter,
                    "A JSON array of async ToolCallId values to inspect. Example: [\"TC1\", \"TC2\"].",
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
                    "GetAsyncToolProgress is handled by the agent loop runtime and cannot be executed through the generic tool manager."));
        }
    }
}
