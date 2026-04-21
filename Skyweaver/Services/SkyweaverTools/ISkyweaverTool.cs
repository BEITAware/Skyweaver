namespace Skyweaver.Services.SkyweaverTools
{
    public interface ISkyweaverTool
    {
        SkyweaverToolDefinition Definition { get; }

        Task<SkyweaverToolResult> ExecuteAsync(
            SkyweaverToolContext context,
            SkyweaverToolArguments arguments,
            CancellationToken cancellationToken = default);
    }
}
