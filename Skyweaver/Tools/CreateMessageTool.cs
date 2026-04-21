using Skyweaver.Services.SkyweaverTools;

namespace Skyweaver.Tools
{
    public sealed class CreateMessageTool : ISkyweaverTool
    {
        public const string ToolName = "CreateMessage";

        private static readonly SkyweaverToolDefinition s_definition = new(
            ToolName,
            "Create the assistant reply payload for the current turn. Put the reply content directly inside <Tool ToolName=\"CreateMessage\">...</Tool>. This tool does not end the loop. Call FinishTask after the latest successful CreateMessage when the turn is ready to close.",
            "GuideBot",
            parameters: [],
            isSystemTool: true);

        public SkyweaverToolDefinition Definition => s_definition;

        public Task<SkyweaverToolResult> ExecuteAsync(
            SkyweaverToolContext context,
            SkyweaverToolArguments arguments,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(SkyweaverToolResult.Success("CreateMessage acknowledged."));
        }
    }
}
