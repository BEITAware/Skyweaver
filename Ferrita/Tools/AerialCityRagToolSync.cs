using Ferrita.Services.AerialCityRag;

namespace Ferrita.Tools
{
    internal static class AerialCityRagToolSync
    {
        public static Task<AerialCityRagFileSyncResult> RefreshFileAsync(
            string filePath,
            string? workspacePath,
            CancellationToken cancellationToken)
        {
            return new AerialCityRagService().RefreshFileAfterMutationAsync(
                filePath,
                workspacePath,
                cancellationToken);
        }

        public static IReadOnlyDictionary<string, object?> WithSyncData(
            IReadOnlyDictionary<string, object?> data,
            AerialCityRagFileSyncResult syncResult)
        {
            var merged = new Dictionary<string, object?>(data, StringComparer.OrdinalIgnoreCase)
            {
                ["aerialCityRagSyncAttempted"] = !syncResult.IsNoOp,
                ["aerialCityRagSyncSucceeded"] = syncResult.Succeeded,
                ["aerialCityRagSyncMessage"] = syncResult.Message,
                ["aerialCityRagSyncFilePath"] = syncResult.FilePath,
                ["aerialCityRagSyncDatabasePath"] = syncResult.DatabasePath
            };

            if (syncResult.Statistics is { } statistics)
            {
                merged["aerialCityRagFilesReembedded"] = statistics.FilesReembedded;
                merged["aerialCityRagFilesRemoved"] = statistics.FilesRemoved;
                merged["aerialCityRagSegmentsInserted"] = statistics.SegmentsInserted;
                merged["aerialCityRagSegmentsUpdated"] = statistics.SegmentsUpdated;
                merged["aerialCityRagSegmentsReused"] = statistics.SegmentsReused;
                merged["aerialCityRagSegmentsDeleted"] = statistics.SegmentsDeleted;
                merged["aerialCityRagFilesExcludedByMode"] = statistics.FilesExcludedByMode;
                merged["aerialCityRagFilesFailed"] = statistics.FilesFailed;
            }

            return merged;
        }
    }
}
