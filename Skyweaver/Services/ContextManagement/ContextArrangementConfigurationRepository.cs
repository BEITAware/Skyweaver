using System;
using System.IO;
using System.Xml.Linq;
using Skyweaver.Models.ContextManagement;

namespace Skyweaver.Services.ContextManagement
{
    public sealed class ContextArrangementConfigurationRepository
    {
        private readonly ContextArrangementPathProvider _pathProvider;
        private readonly object _syncRoot = new();

        public ContextArrangementConfigurationRepository(ContextArrangementPathProvider pathProvider)
        {
            _pathProvider = pathProvider;
        }

        public string ConfigurationFilePath => _pathProvider.ConfigurationFilePath;

        public ContextArrangementConfiguration Load()
        {
            lock (_syncRoot)
            {
                EnsureConfigurationDirectory();

                if (!File.Exists(ConfigurationFilePath))
                {
                    var configuration = CreateDefaultConfiguration();
                    Save(configuration);
                    return configuration;
                }

                try
                {
                    var document = XDocument.Load(ConfigurationFilePath);
                    var root = document.Root ?? throw new InvalidDataException("ContextArrangement 配置 XML 缺少根节点。");

                    return new ContextArrangementConfiguration
                    {
                        OptimizeToolCallPrompt = (bool?)root.Element("OptimizeToolCallPrompt") ?? false,
                        ToolCallIdTable = (bool?)root.Element("ToolCallIdTable") ?? false
                    };
                }
                catch
                {
                    return CreateDefaultConfiguration();
                }
            }
        }

        public void Save(ContextArrangementConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(configuration);

            lock (_syncRoot)
            {
                EnsureConfigurationDirectory();

                var document = new XDocument(
                    new XElement("ContextArrangementConfiguration",
                        new XAttribute("SchemaVersion", 1),
                        new XElement("OptimizeToolCallPrompt", configuration.OptimizeToolCallPrompt),
                        new XElement("ToolCallIdTable", configuration.ToolCallIdTable)));

                document.Save(ConfigurationFilePath);
            }
        }

        private void EnsureConfigurationDirectory()
        {
            Directory.CreateDirectory(_pathProvider.ConfigurationDirectoryPath);
        }

        private static ContextArrangementConfiguration CreateDefaultConfiguration()
        {
            return new ContextArrangementConfiguration();
        }
    }
}
