using Ferrita.Services.FerritaTools;

namespace Ferrita.Tools
{
    public sealed class PassToMainAgentTool : IFerritaTool
    {
        public const string ToolName = FerritaBuiltInToolNames.PassToMainAgent;

        private static readonly FerritaToolDefinition s_definition = new(
            ToolName,
            "Only for sub-agents. Call this tool to end the sub-agent loop and pass the final payload back to the main agent. The tool payload becomes the sub-agent's response returned to SpawnSubAgent.",
            "GuideBot",
            [
                new FerritaToolParameterDefinition(
                    FerritaBuiltInToolNames.PassToMainAgentParameter,
                    "The exact content to return to the main agent. For structured tasks, this can be XML or preserved content.",
                    FerritaToolParameterType.String,
                    isRequired: true)
            ],
            isSystemTool: true,
            supportsAsyncInvocation: false);

        public FerritaToolDefinition Definition => s_definition;

        public Task<FerritaToolResult> ExecuteAsync(
            FerritaToolContext context,
            FerritaToolArguments arguments,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!context.IsSubAgent)
            {
                return Task.FromResult(FerritaToolResult.Failure("PassToMainAgent can only be used inside a sub-agent loop."));
            }

            var payload = arguments.GetString(FerritaBuiltInToolNames.PassToMainAgentParameter)?.Trim() ?? string.Empty;
            if (payload.Length == 0)
            {
                return Task.FromResult(FerritaToolResult.Failure("PassToMainAgent requires non-empty content."));
            }

            return Task.FromResult(FerritaToolResult.Success(
                "PassToMainAgent accepted.",
                new Dictionary<string, object?>
                {
                    ["passToMainAgent"] = payload
                }));
        }
    }
}
