using Ferrita.Services.FerritaTools;

namespace Ferrita.Tools
{
    public sealed class FinishTaskTool : IFerritaTool
    {
        public const string ToolName = "FinishTask";

        private static readonly FerritaToolDefinition s_definition = new(
            ToolName,
            "Ends the entire agent loop and terminates the task. This tool signals that the agent's work is complete and terminates the agent loop. Ending a turn (the current assistant response) does not require any tool; however, if FinishTask is not called, the system will automatically proceed to the next turn (iteration). Call this tool only after a successful CreateMessage if you want to return a final message to the user.",
            "GuideBot",
            parameters: [],
            isSystemTool: true,
            supportsAsyncInvocation: false);

        public FerritaToolDefinition Definition => s_definition;

        public Task<FerritaToolResult> ExecuteAsync(
            FerritaToolContext context,
            FerritaToolArguments arguments,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(FerritaToolResult.Success("FinishTask acknowledged."));
        }
    }
}
