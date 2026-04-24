namespace Skyweaver.Models.LateralFileSystem
{
    public sealed class LateralFileSystemNodeStorageSummary
    {
        public long HydratedBytes { get; init; }

        public int HydratedFileCount { get; init; }

        public int HydratedPlaceholderFileCount { get; init; }

        public int FullFileCount { get; init; }

        public int PlaceholderFileCount { get; init; }

        public int TotalFileCount { get; init; }

        public int TotalDirectoryCount { get; init; }

        public bool UsedFallbackEstimation { get; init; }
    }
}
