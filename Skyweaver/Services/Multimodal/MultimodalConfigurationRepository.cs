using System;
using System.IO;
using System.Xml.Linq;
using Skyweaver.Models.Multimodal;

namespace Skyweaver.Services.Multimodal
{
    public sealed class MultimodalConfigurationRepository
    {
        private readonly MultimodalPathProvider _pathProvider;
        private readonly object _syncRoot = new();

        public MultimodalConfigurationRepository(MultimodalPathProvider pathProvider)
        {
            _pathProvider = pathProvider;
        }

        public string ConfigurationFilePath => _pathProvider.ConfigurationFilePath;

        public MultimodalConfiguration Load()
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
                    var root = document.Root ?? throw new InvalidDataException("Multimodal 配置 XML 缺少根节点。");

                    return new MultimodalConfiguration
                    {
                        EnableOcr = (bool?)root.Element("EnableOcr") ?? false,
                        EnableLongImageAutoParse = (bool?)root.Element("EnableLongImageAutoParse") ?? true
                    };
                }
                catch (Exception)
                {
                    // 解析发生任何异常时回退到默认配置
                    return CreateDefaultConfiguration();
                }
            }
        }

        public void Save(MultimodalConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(configuration);

            lock (_syncRoot)
            {
                EnsureConfigurationDirectory();

                var document = new XDocument(
                    new XElement("MultimodalConfiguration",
                        new XAttribute("SchemaVersion", 1),
                        new XElement("EnableOcr", configuration.EnableOcr),
                        new XElement("EnableLongImageAutoParse", configuration.EnableLongImageAutoParse)));

                document.Save(ConfigurationFilePath);
            }
        }

        private void EnsureConfigurationDirectory()
        {
            Directory.CreateDirectory(_pathProvider.ConfigurationDirectoryPath);
        }

        private static MultimodalConfiguration CreateDefaultConfiguration()
        {
            return new MultimodalConfiguration();
        }

        private static T ReadEnum<T>(XElement? element, T fallback) where T : struct, Enum
        {
            var text = (string?)element;
            return Enum.TryParse<T>(text, out var value) ? value : fallback;
        }
    }
}
