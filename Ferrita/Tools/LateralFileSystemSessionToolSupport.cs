using Ferrita.Models.LateralFileSystem;
using Ferrita.Services.LateralFileSystem;
using Ferrita.Services.FerritaTools;

namespace Ferrita.Tools
{
    internal static class LateralFileSystemSessionToolSupport
    {
        public static string GetRequiredSessionId(FerritaToolContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            if (context.Properties.TryGetValue("sessionId", out var sessionId) &&
                !string.IsNullOrWhiteSpace(sessionId))
            {
                return sessionId.Trim();
            }

            throw new InvalidOperationException("The current tool context does not contain a sessionId.");
        }

        public static LateralFileSystemNodeModel? FindLatestSessionOwnedNode(
            LateralFileSystemRuntime runtime,
            string sessionId)
        {
            ArgumentNullException.ThrowIfNull(runtime);

            if (string.IsNullOrWhiteSpace(sessionId))
            {
                return null;
            }

            return runtime.GetNodes()
                .Where(node => string.Equals(node.Owner, sessionId.Trim(), StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(node => node.CreatedAtUtc)
                .ThenByDescending(node => node.UpdatedAtUtc)
                .FirstOrDefault();
        }
    }
}
