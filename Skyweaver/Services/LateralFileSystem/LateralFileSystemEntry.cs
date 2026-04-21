namespace Skyweaver.Services.LateralFileSystem
{
    internal sealed class LateralFileSystemEntry
    {
        public string RelativePath { get; init; } = string.Empty;

        public string Name { get; init; } = string.Empty;

        public string FullPath { get; init; } = string.Empty;

        public bool IsDirectory { get; init; }

        public long FileSize { get; init; }

        public DateTime CreationTimeUtc { get; init; }

        public DateTime LastAccessTimeUtc { get; init; }

        public DateTime LastWriteTimeUtc { get; init; }

        public DateTime ChangeTimeUtc { get; init; }

        public uint FileAttributes { get; init; }
    }
}
