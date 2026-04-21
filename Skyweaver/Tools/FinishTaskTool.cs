using Skyweaver.Services.SkyweaverTools;

namespace Skyweaver.Tools
{
    public sealed class FinishTaskTool : ISkyweaverTool
    {
        public const string ToolName = "FinishTask";

        private static readonly SkyweaverToolDefinition s_definition = new(
            ToolName,
            "Close the current assistant turn after a successful CreateMessage. FinishTask does not carry the reply payload. It only ends the current loop once the latest CreateMessage is ready to be returned downstream.",
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
            return Task.FromResult(SkyweaverToolResult.Success("FinishTask acknowledged."));
        }
    }
}
