using Skyweaver.Services.SkyweaverTools;

namespace Skyweaver.Tools
{
    public sealed class RetrieveCompactedToolCallsTool : ISkyweaverTool
    {
        public SkyweaverToolDefinition Definition { get; } = new(
            SkyweaverBuiltInToolNames.RetrieveCompactedToolCalls,
            "Retrieves original params and return content for compacted tool calls in the next agent loop.",
            parameters:
            [
                new SkyweaverToolParameterDefinition(
                    SkyweaverBuiltInToolNames.CompactionToolCallIdsParameter,
                    "JSON array of compacted ToolCallID values to retrieve.",
                    SkyweaverToolParameterType.Json,
                    isRequired: true)
            ],
            isSystemTool: true,
            canBelongToToolKit: false,
            supportsAsyncInvocation: false);

        public Task<SkyweaverToolResult> ExecuteAsync(
            SkyweaverToolContext context,
            SkyweaverToolArguments arguments,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(SkyweaverToolResult.Failure(
                "RetrieveCompactedToolCalls is executed by the agent loop host."));
        }
    }
}
