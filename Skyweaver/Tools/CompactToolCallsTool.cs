using Skyweaver.Services.SkyweaverTools;

namespace Skyweaver.Tools
{
    public sealed class CompactToolCallsTool : ISkyweaverTool
    {
        public SkyweaverToolDefinition Definition { get; } = new(
            SkyweaverBuiltInToolNames.CompactToolCalls,
            "Host-managed MinCompaction tool. Hidden from normal agent prompts.",
            parameters:
            [
                new SkyweaverToolParameterDefinition(
                    SkyweaverBuiltInToolNames.CompactionToolCallIdsParameter,
                    "JSON array of ToolCallID values to compact.",
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
                "CompactToolCalls is executed only by the host during hidden MinCompaction passes."));
        }
    }
}
