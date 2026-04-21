using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Skyweaver.Models.LateralFileSystem;

namespace Skyweaver.Services.LateralFileSystem
{
    public sealed class LateralFileSystemTreeRepository
    {
        private readonly object _syncRoot = new();

        public LateralFileSystemTreeModel Load(string workingRootDirectory)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(workingRootDirectory);

            lock (_syncRoot)
            {
                var treeFilePath = LateralFileSystemPathProvider.GetTreeFilePath(workingRootDirectory);
                if (!File.Exists(treeFilePath))
                {
                    return new LateralFileSystemTreeModel();
                }

                var document = XDocument.Load(treeFilePath);
                var root = document.Root ?? throw new InvalidDataException("LateralFileSystemTree XML 缺少根节点。");
                var tree = new LateralFileSystemTreeModel
                {
                    SchemaVersion = (int?)root.Attribute("SchemaVersion") ?? 1,
                    UpdatedAtUtc = ParseDateTime((string?)root.Attribute("UpdatedAtUtc"))
                };

                foreach (var nodeElement in root.Elements("Node"))
                {
                    var node = new LateralFileSystemNodeModel
                    {
                        Id = (string?)nodeElement.Attribute("Id") ?? Guid.NewGuid().ToString("N"),
                        Name = (string?)nodeElement.Element("Name") ?? string.Empty,
                        VirtualRootPath = (string?)nodeElement.Element("VirtualRootPath") ?? string.Empty,
                        Owner = (string?)nodeElement.Element("Owner") ?? string.Empty,
                        Kind = Enum.TryParse<LateralFileSystemNodeKind>((string?)nodeElement.Element("Kind"), true, out var kind)
                            ? kind
                            : LateralFileSystemNodeKind.Projection,
                        ProjectionSourcePath = NullIfWhiteSpace((string?)nodeElement.Element("ProjectionSourcePath")),
                        ParentNodeId = NullIfWhiteSpace((string?)nodeElement.Element("ParentNodeId")),
                        IsActive = (bool?)nodeElement.Element("IsActive") ?? false,
                        ProviderInstanceId = (string?)nodeElement.Element("ProviderInstanceId") ?? Guid.NewGuid().ToString("D"),
                        ContentVersion = (string?)nodeElement.Element("ContentVersion") ?? Guid.NewGuid().ToString("N"),
                        CreatedAtUtc = ParseDateTime((string?)nodeElement.Element("CreatedAtUtc")),
                        UpdatedAtUtc = ParseDateTime((string?)nodeElement.Element("UpdatedAtUtc")),
                        SchemaVersion = (int?)nodeElement.Element("SchemaVersion") ?? 1
                    };

                    foreach (var editedPathElement in nodeElement.Element("EditedRelativePaths")?.Elements("Path") ?? Enumerable.Empty<XElement>())
                    {
                        var editedPath = NullIfWhiteSpace(editedPathElement.Value);
                        if (editedPath is not null)
                        {
                            node.EditedRelativePaths.Add(editedPath);
                        }
                    }

                    foreach (var localOnlyPathElement in nodeElement.Element("LocalOnlyRelativePaths")?.Elements("Path") ?? Enumerable.Empty<XElement>())
                    {
                        var localOnlyPath = NullIfWhiteSpace(localOnlyPathElement.Value);
                        if (localOnlyPath is not null)
                        {
                            node.LocalOnlyRelativePaths.Add(localOnlyPath);
                        }
                    }

                    foreach (var propertyElement in nodeElement.Element("Properties")?.Elements("Property") ?? Enumerable.Empty<XElement>())
                    {
                        node.Properties.Add(new LateralFileSystemNodeProperty
                        {
                            Key = (string?)propertyElement.Attribute("Key") ?? string.Empty,
                            Value = (string?)propertyElement.Attribute("Value") ?? string.Empty
                        });
                    }

                    tree.Nodes.Add(node);
                }

                return tree;
            }
        }

        public void Save(string workingRootDirectory, LateralFileSystemTreeModel tree)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(workingRootDirectory);
            ArgumentNullException.ThrowIfNull(tree);

            lock (_syncRoot)
            {
                Directory.CreateDirectory(workingRootDirectory);
                tree.UpdatedAtUtc = DateTime.UtcNow;

                var document = new XDocument(
                    new XElement("LateralFileSystemTree",
                        new XAttribute("SchemaVersion", tree.SchemaVersion),
                        new XAttribute("UpdatedAtUtc", tree.UpdatedAtUtc.ToString("O", CultureInfo.InvariantCulture)),
                        tree.Nodes.Select(node =>
                            new XElement("Node",
                                new XAttribute("Id", node.Id),
                                new XElement("Name", node.Name),
                                new XElement("VirtualRootPath", node.VirtualRootPath),
                                new XElement("Owner", node.Owner),
                                new XElement("Kind", node.Kind),
                                new XElement("ProjectionSourcePath", node.ProjectionSourcePath ?? string.Empty),
                                new XElement("ParentNodeId", node.ParentNodeId ?? string.Empty),
                                new XElement("IsActive", node.IsActive),
                                new XElement("ProviderInstanceId", node.ProviderInstanceId),
                                new XElement("ContentVersion", node.ContentVersion),
                                new XElement("CreatedAtUtc", node.CreatedAtUtc.ToString("O", CultureInfo.InvariantCulture)),
                                new XElement("UpdatedAtUtc", node.UpdatedAtUtc.ToString("O", CultureInfo.InvariantCulture)),
                                new XElement("SchemaVersion", node.SchemaVersion),
                                new XElement("EditedRelativePaths", node.EditedRelativePaths.Select(path => new XElement("Path", path))),
                                new XElement("LocalOnlyRelativePaths", node.LocalOnlyRelativePaths.Select(path => new XElement("Path", path))),
                                new XElement("Properties",
                                    node.Properties.Select(property =>
                                        new XElement("Property",
                                            new XAttribute("Key", property.Key),
                                            new XAttribute("Value", property.Value))))))));

                document.Save(LateralFileSystemPathProvider.GetTreeFilePath(workingRootDirectory));
            }
        }

        private static DateTime ParseDateTime(string? value)
        {
            return DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var parsed)
                ? parsed
                : DateTime.UtcNow;
        }

        private static string? NullIfWhiteSpace(string? value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value;
        }
    }
}
