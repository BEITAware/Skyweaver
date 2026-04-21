using System.Collections.ObjectModel;

namespace Skyweaver.Models.LateralFileSystem
{
    public sealed class LateralFileSystemNodeModel
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N");

        public string Name { get; set; } = string.Empty;

        public string VirtualRootPath { get; set; } = string.Empty;

        public string Owner { get; set; } = string.Empty;

        public LateralFileSystemNodeKind Kind { get; set; } = LateralFileSystemNodeKind.Projection;

        public string? ProjectionSourcePath { get; set; }

        public string? ParentNodeId { get; set; }

        public bool IsActive { get; set; }

        public string ProviderInstanceId { get; set; } = Guid.NewGuid().ToString("D");

        public string ContentVersion { get; set; } = Guid.NewGuid().ToString("N");

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;

        public int SchemaVersion { get; set; } = 1;

        public Collection<string> EditedRelativePaths { get; } = new();

        public Collection<string> LocalOnlyRelativePaths { get; } = new();

        public Collection<LateralFileSystemNodeProperty> Properties { get; } = new();
    }
}
