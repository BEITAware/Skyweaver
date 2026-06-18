using Ferrita.Services.FerritaTools;

namespace Ferrita.Tools
{
    public sealed class GetAsyncToolProgressTool : IFerritaTool
    {
        private static readonly FerritaToolDefinition s_definition = new(
            FerritaBuiltInToolNames.GetAsyncToolProgress,
            "Returns the latest progress snapshot for specified async tool calls. This tool is internal and must be invoked synchronously.",
            "GuideBot",
            [
                new FerritaToolParameterDefinition(
                    FerritaBuiltInToolNames.GetAsyncToolProgressParameter,
                    "A JSON array of async ToolCallId values to inspect. Example: [\"TC1\", \"TC2\"].",
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
                    "GetAsyncToolProgress is handled by the agent loop runtime and cannot be executed through the generic tool manager."));
        }
    }
}
