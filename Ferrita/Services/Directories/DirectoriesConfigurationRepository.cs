using System.IO;
using System.Xml.Linq;
using Ferrita.Models.Directories;

namespace Ferrita.Services.Directories
{
    public sealed class DirectoriesConfigurationRepository
    {
        private readonly object _syncRoot = new();

        public string ConfigurationDirectoryPath => FerritaDirectoryDefaults.DefaultConfigurationDirectoryPath;

        public string ConfigurationFilePath => FerritaDirectoryDefaults.DirectoriesConfigurationFilePath;

        public DirectoriesConfiguration Load()
        {
            lock (_syncRoot)
            {
                EnsureConfigurationDirectory();

                if (!File.Exists(ConfigurationFilePath))
                {
                    var configuration = FerritaDirectoryDefaults.CreateDefaultConfiguration();
                    Save(configuration);
                    return configuration;
                }

                var document = XDocument.Load(ConfigurationFilePath);
                var root = document.Root ?? throw new InvalidDataException("Directories.xml 缺少根节点。");

                return FerritaDirectoryDefaults.NormalizeConfiguration(new DirectoriesConfiguration
                {
                    ChatSessionsDirectoryPath = ((string?)root.Element("ChatSessionsDirectory") ?? string.Empty).Trim(),
                    ConfigurationDirectoryPath = ((string?)root.Element("ConfigurationDirectory") ?? string.Empty).Trim(),
                    DebugDirectoryPath = ((string?)root.Element("DebugDirectory") ?? string.Empty).Trim(),
                    SessionFlowsDirectoryPath = ((string?)root.Element("SessionFlowsDirectory") ?? string.Empty).Trim(),
                    AerialCityDirectoryPath = ((string?)root.Element("AerialCityDirectory") ?? string.Empty).Trim()
                });
            }
        }

        public void Save(DirectoriesConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(configuration);

            lock (_syncRoot)
            {
                EnsureConfigurationDirectory();

                var normalizedConfiguration = FerritaDirectoryDefaults.NormalizeConfiguration(configuration);
                var document = new XDocument(
                    new XElement("Directories",
                        new XAttribute("SchemaVersion", 1),
                        new XElement("ChatSessionsDirectory", normalizedConfiguration.ChatSessionsDirectoryPath),
                        new XElement("ConfigurationDirectory", normalizedConfiguration.ConfigurationDirectoryPath),
                        new XElement("DebugDirectory", normalizedConfiguration.DebugDirectoryPath),
                        new XElement("SessionFlowsDirectory", normalizedConfiguration.SessionFlowsDirectoryPath),
                        new XElement("AerialCityDirectory", normalizedConfiguration.AerialCityDirectoryPath)));

                document.Save(ConfigurationFilePath);
            }
        }

        private void EnsureConfigurationDirectory()
        {
            Directory.CreateDirectory(ConfigurationDirectoryPath);
        }
    }
}
