namespace Skyweaver.Models.LateralFileSystem
{
    public sealed class LateralFileSystemFileEntryModel
    {
        public string Name { get; init; } = string.Empty;

        public string FullPath { get; init; } = string.Empty;

        public string RelativePath { get; init; } = string.Empty;

        public bool IsDirectory { get; init; }

        public long LogicalSizeBytes { get; init; }

        public long HydratedSizeBytes { get; init; }

        public LateralFileSystemOnDiskState OnDiskState { get; init; }
    }
}
