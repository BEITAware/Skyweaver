using Ferrita.Services.FerritaTools;

namespace Ferrita.Tools
{
    public sealed class CompactToolCallsTool : IFerritaTool
    {
        public FerritaToolDefinition Definition { get; } = new(
            FerritaBuiltInToolNames.CompactToolCalls,
            "Host-managed MinCompaction tool. Hidden from normal agent prompts.",
            parameters:
            [
                new FerritaToolParameterDefinition(
                    FerritaBuiltInToolNames.CompactionToolCallIdsParameter,
                    "JSON array of ToolCallID values to compact.",
                    FerritaToolParameterType.Json,
                    isRequired: true)
            ],
            isSystemTool: true,
            canBelongToToolKit: false,
            supportsAsyncInvocation: false);

        public Task<FerritaToolResult> ExecuteAsync(
            FerritaToolContext context,
            FerritaToolArguments arguments,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(FerritaToolResult.Failure(
                "CompactToolCalls is executed only by the host during hidden MinCompaction passes."));
        }
    }
}
