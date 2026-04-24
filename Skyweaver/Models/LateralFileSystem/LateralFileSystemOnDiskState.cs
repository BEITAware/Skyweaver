namespace Skyweaver.Models.LateralFileSystem
{
    [Flags]
    public enum LateralFileSystemOnDiskState
    {
        None = 0,
        Placeholder = 1 << 0,
        HydratedPlaceholder = 1 << 1,
        DirtyPlaceholder = 1 << 2,
        Full = 1 << 3,
        Tombstone = 1 << 4,
        Unknown = 1 << 30
    }
}
