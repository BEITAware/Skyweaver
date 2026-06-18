using System.Collections.ObjectModel;

namespace Ferrita.Models.LateralFileSystem
{
    public sealed class LateralFileSystemTreeModel
    {
        public int SchemaVersion { get; set; } = 1;

        public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;

        public Collection<LateralFileSystemNodeModel> Nodes { get; } = new();
    }
}
