using System.IO;
using System.Xml.Linq;
using Ferrita.Models.ShellIntegration;

namespace Ferrita.Services.ShellIntegration
{
    public sealed class ShellIntegrationConfigurationRepository
    {
        private readonly ShellIntegrationPathProvider _pathProvider;
        private readonly object _syncRoot = new();

        public ShellIntegrationConfigurationRepository(ShellIntegrationPathProvider pathProvider)
        {
            _pathProvider = pathProvider;
        }

        public string ConfigurationFilePath => _pathProvider.ConfigurationFilePath;

        public ShellIntegrationConfiguration Load()
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
                var root = document.Root ?? throw new InvalidDataException("ShellIntegration.xml is missing its root element.");

                var sessionFlowElement = root.Element("SessionFlow");

                return new ShellIntegrationConfiguration
                {
                    IsEnabled = (bool?)root.Element("IsEnabled") ?? false,
                    SessionFlowGraphId = ((string?)sessionFlowElement?.Element("GraphId") ?? string.Empty).Trim(),
                    SessionFlowGraphName = ((string?)sessionFlowElement?.Element("GraphName") ?? string.Empty).Trim(),
                    SessionFlowFilePath = ((string?)sessionFlowElement?.Element("FilePath") ?? string.Empty).Trim()
                };
            }
        }

        public void Save(ShellIntegrationConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(configuration);

            lock (_syncRoot)
            {
                EnsureConfigurationDirectory();

                var document = new XDocument(
                    new XElement("ShellIntegrationConfiguration",
                        new XAttribute("SchemaVersion", 1),
                        new XElement("IsEnabled", configuration.IsEnabled),
                        new XElement("SessionFlow",
                            new XElement("GraphId", configuration.SessionFlowGraphId),
                            new XElement("GraphName", configuration.SessionFlowGraphName),
                            new XElement("FilePath", configuration.SessionFlowFilePath))));

                document.Save(ConfigurationFilePath);
            }
        }

        private void EnsureConfigurationDirectory()
        {
            Directory.CreateDirectory(_pathProvider.ConfigurationDirectoryPath);
        }

        private static ShellIntegrationConfiguration CreateDefaultConfiguration()
        {
            return new ShellIntegrationConfiguration();
        }
    }
}
