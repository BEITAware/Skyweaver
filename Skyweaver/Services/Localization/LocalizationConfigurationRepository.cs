using System.IO;
using System.Xml.Linq;
using Skyweaver.Models.Localization;

namespace Skyweaver.Services.Localization
{
    public sealed class LocalizationConfigurationRepository
    {
        private readonly LocalizationPathProvider _pathProvider;
        private readonly object _syncRoot = new();

        public LocalizationConfigurationRepository(LocalizationPathProvider pathProvider)
        {
            _pathProvider = pathProvider;
        }

        public string ConfigurationFilePath => _pathProvider.ConfigurationFilePath;

        public LocalizationConfiguration Load()
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

                var document = XDocument.Load(ConfigurationFilePath);
                var root = document.Root ?? throw new InvalidDataException("Localization 配置 XML 缺少根节点。");

                return new LocalizationConfiguration
                {
                    LanguageCode = ((string?)root.Element("LanguageCode") ?? "zh-CN").Trim()
                };
            }
        }

        public void Save(LocalizationConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(configuration);

            lock (_syncRoot)
            {
                EnsureConfigurationDirectory();

                var document = new XDocument(
                    new XElement("LocalizationConfiguration",
                        new XAttribute("SchemaVersion", 1),
                        new XElement("LanguageCode", configuration.LanguageCode)));

                document.Save(ConfigurationFilePath);
            }
        }

        private void EnsureConfigurationDirectory()
        {
            Directory.CreateDirectory(_pathProvider.ConfigurationDirectoryPath);
        }

        private static LocalizationConfiguration CreateDefaultConfiguration()
        {
            return new LocalizationConfiguration();
        }
    }
}
