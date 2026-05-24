using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using Skyweaver.Controls.LanguageModelConfigurationControl.Models;

namespace Skyweaver.Controls.LanguageModelConfigurationControl.Services
{
    public sealed class CapabilityLayerConfigurationRepository
    {
        private readonly LanguageModelConfigurationPathProvider _pathProvider;
        private readonly object _syncRoot = new();

        public CapabilityLayerConfigurationRepository(LanguageModelConfigurationPathProvider pathProvider)
        {
            _pathProvider = pathProvider;
        }

        public string ConfigurationFilePath => _pathProvider.CapabilityLayerFilePath;

        public IReadOnlyList<CapabilityLayerDefinition> Load()
        {
            lock (_syncRoot)
            {
                EnsureConfigurationDirectory();

                if (!File.Exists(ConfigurationFilePath))
                {
                    var initialDefinitions = new List<CapabilityLayerDefinition>();
                    CapabilityLayerBuiltIns.EnsureBuiltInLayers(initialDefinitions);
                    SaveInternal(initialDefinitions);
                    return initialDefinitions;
                }

                var document = XDocument.Load(ConfigurationFilePath);
                var root = document.Root ?? throw new InvalidDataException("CapabilityLayer 配置 XML 缺少根节点。");
                var requiresMigrationSave = false;

                var definitions = root.Elements("CapabilityLayer")
                    .Select(element =>
                    {
                        var key = ((string?)element.Element("Key") ?? string.Empty).Trim();
                        if (key.Length == 0)
                        {
                            key = Guid.NewGuid().ToString("N");
                            requiresMigrationSave = true;
                        }

                        var definition = new CapabilityLayerDefinition
                        {
                            Key = key,
                            Name = ((string?)element.Element("Name") ?? string.Empty).Trim()
                        };

                        foreach (var item in element.Elements("LanguageModelRef"))
                        {
                            definition.LanguageModels.Add(new CapabilityLayerEntry
                            {
                                LanguageModelKey = ((string?)item.Attribute("Key") ?? string.Empty).Trim()
                            });
                        }

                        return definition;
                    })
                    .ToList();

                if (CapabilityLayerBuiltIns.EnsureBuiltInLayers(definitions))
                {
                    requiresMigrationSave = true;
                }

                if (requiresMigrationSave)
                {
                    SaveInternal(definitions);
                }

                return definitions;
            }
        }

        public void Save(IEnumerable<CapabilityLayerDefinition> definitions)
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
            Directory.CreateDirectory(_pathProvider.ConfigurationDirectoryPath);
        }

        private void SaveInternal(IEnumerable<CapabilityLayerDefinition> definitions)
        {
            var document = new XDocument(
                new XElement("CapabilityLayers",
                    new XAttribute("SchemaVersion", 2),
                    definitions.Select(definition => new XElement("CapabilityLayer",
                        new XElement("Key", definition.Key),
                        new XElement("Name", definition.Name),
                        definition.LanguageModels.Select(item => new XElement("LanguageModelRef",
                            new XAttribute("Key", item.LanguageModelKey)))))));

            document.Save(ConfigurationFilePath);
        }
    }
}
