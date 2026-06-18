using Ferrita.Services.FerritaTools;

namespace Ferrita.Tools
{
    public sealed class RetrieveCompactedToolCallsTool : IFerritaTool
    {
        public FerritaToolDefinition Definition { get; } = new(
            FerritaBuiltInToolNames.RetrieveCompactedToolCalls,
            "Retrieves original params and return content for compacted tool calls in the next agent loop.",
            parameters:
            [
                new FerritaToolParameterDefinition(
                    FerritaBuiltInToolNames.CompactionToolCallIdsParameter,
                    "JSON array of compacted ToolCallID values to retrieve.",
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
                "RetrieveCompactedToolCalls is executed by the agent loop host."));
        }
    }
}
