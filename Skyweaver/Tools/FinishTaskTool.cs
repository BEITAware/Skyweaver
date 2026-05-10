using Skyweaver.Services.SkyweaverTools;

namespace Skyweaver.Tools
{
    public sealed class FinishTaskTool : ISkyweaverTool
    {
        public const string ToolName = "FinishTask";

        private static readonly SkyweaverToolDefinition s_definition = new(
            ToolName,
            "Ends the entire agent loop and terminates the task. This tool signals that the agent's work is complete and terminates the agent loop. Ending a turn (the current assistant response) does not require any tool; however, if FinishTask is not called, the system will automatically proceed to the next turn (iteration). Call this tool only after a successful CreateMessage if you want to return a final message to the user.",
            "GuideBot",
            parameters: [],
            isSystemTool: true,
            supportsAsyncInvocation: false);

        public SkyweaverToolDefinition Definition => s_definition;

        public Task<SkyweaverToolResult> ExecuteAsync(
            SkyweaverToolContext context,
            SkyweaverToolArguments arguments,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(SkyweaverToolResult.Success("FinishTask acknowledged."));
        }
    }
}
