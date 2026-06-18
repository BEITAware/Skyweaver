namespace Ferrita.Services.FerritaTools
{
    public interface IFerritaTool
    {
        FerritaToolDefinition Definition { get; }

        Task<FerritaToolResult> ExecuteAsync(
            FerritaToolContext context,
            FerritaToolArguments arguments,
            CancellationToken cancellationToken = default);
    }
}
