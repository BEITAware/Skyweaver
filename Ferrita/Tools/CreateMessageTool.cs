using Ferrita.Services.FerritaTools;

namespace Ferrita.Tools
{
    public sealed class CreateMessageTool : IFerritaTool
    {
        public const string ToolName = "CreateMessage";

        private static readonly FerritaToolDefinition s_definition = new(
            ToolName,
            "Create the assistant reply payload for the current turn. Put the reply content directly inside <Tool ToolName=\"CreateMessage\">...</Tool>. This tool does not terminate the agent loop. To end the entire loop, you must call FinishTask after a successful CreateMessage. If FinishTask is not called, the system will automatically initiate the next turn.",
            "GuideBot",
            parameters: [],
            isSystemTool: true);

        public FerritaToolDefinition Definition => s_definition;

        public Task<FerritaToolResult> ExecuteAsync(
            FerritaToolContext context,
            FerritaToolArguments arguments,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(FerritaToolResult.Success("CreateMessage acknowledged."));
        }
    }
}
