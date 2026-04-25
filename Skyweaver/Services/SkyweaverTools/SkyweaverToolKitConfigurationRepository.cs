using System.IO;
using System.Xml.Linq;

namespace Skyweaver.Services.SkyweaverTools
{
    public sealed class SkyweaverToolKitConfigurationRepository
    {
        private readonly object _syncRoot = new();
        private readonly SkyweaverToolConfigurationRepository _toolConfigurationRepository = new();

        public string ConfigurationDirectoryPath => _toolConfigurationRepository.ConfigurationDirectoryPath;

        public string ConfigurationFilePath => Path.Combine(ConfigurationDirectoryPath, "ToolKitConfiguration.xml");

        public IReadOnlyList<SkyweaverToolKitDefinition> Load()
        {
            lock (_syncRoot)
            {
                EnsureConfigurationDirectory();

                if (!File.Exists(ConfigurationFilePath))
                {
                    Save(Array.Empty<SkyweaverToolKitDefinition>());
                    return Array.Empty<SkyweaverToolKitDefinition>();
                }

                var document = XDocument.Load(ConfigurationFilePath);
                var root = document.Root ?? throw new InvalidDataException("ToolKitConfiguration.xml 缺少根节点。");
                var requiresMigrationSave = false;

                var definitions = root.Elements("ToolKit")
                    .Select(element =>
                    {
                        var key = ((string?)element.Element("Key") ?? string.Empty).Trim();
                        if (key.Length == 0)
                        {
                            key = Guid.NewGuid().ToString("N");
                            requiresMigrationSave = true;
                        }

                        var definition = new SkyweaverToolKitDefinition
                        {
                            Key = key,
                            Name = ((string?)element.Element("Name") ?? string.Empty).Trim()
                        };

                        foreach (var toolRef in element.Elements("ToolRef"))
                        {
                            definition.Tools.Add(new SkyweaverToolKitEntry
                            {
                                ToolName = ((string?)toolRef.Attribute("Name") ?? string.Empty).Trim()
                            });
                        }

                        return definition;
                    })
                    .ToList();

                if (requiresMigrationSave)
                {
                    SaveInternal(definitions);
                }

                return definitions;
            }
        }

        public void Save(IEnumerable<SkyweaverToolKitDefinition> definitions)
        {
            ArgumentNullException.ThrowIfNull(definitions);

            lock (_syncRoot)
            {
                EnsureConfigurationDirectory();
                SaveInternal(definitions);
            }
        }

        private void EnsureConfigurationDirectory()
        {
            Directory.CreateDirectory(ConfigurationDirectoryPath);
        }

        private void SaveInternal(IEnumerable<SkyweaverToolKitDefinition> definitions)
        {
            var document = new XDocument(
                new XElement("ToolKits",
                    new XAttribute("SchemaVersion", 1),
                    definitions
                        .Where(definition => !string.IsNullOrWhiteSpace(definition.Key))
                        .OrderBy(definition => definition.DisplayNameOrFallback, StringComparer.OrdinalIgnoreCase)
                        .Select(definition => new XElement("ToolKit",
                            new XElement("Key", definition.Key),
                            new XElement("Name", definition.Name),
                            definition.Tools.Select(tool => new XElement("ToolRef",
                                new XAttribute("Name", tool.ToolName)))))));

            document.Save(ConfigurationFilePath);
        }
    }
}
