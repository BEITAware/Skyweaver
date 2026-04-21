using System.IO;
using System.Xml.Linq;
using Skyweaver.Models.LateralFileSystem;

namespace Skyweaver.Services.LateralFileSystem
{
    public sealed class LateralFileSystemConfigurationRepository
    {
        private readonly LateralFileSystemPathProvider _pathProvider;
        private readonly object _syncRoot = new();

        public LateralFileSystemConfigurationRepository(LateralFileSystemPathProvider pathProvider)
        {
            _pathProvider = pathProvider;
        }

        public string ConfigurationFilePath => _pathProvider.ConfigurationFilePath;

        public LateralFileSystemConfiguration Load()
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
                var root = document.Root ?? throw new InvalidDataException("LateralFileSystem 配置 XML 缺少根节点。");

                return new LateralFileSystemConfiguration
                {
                    IsEnabled = (bool?)root.Element("IsEnabled") ?? false,
                    WorkingRootDirectory = ((string?)root.Element("WorkingRootDirectory") ?? string.Empty).Trim()
                };
            }
        }

        public void Save(LateralFileSystemConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(configuration);

            lock (_syncRoot)
            {
                EnsureConfigurationDirectory();

                var document = new XDocument(
                    new XElement("LateralFileSystemConfiguration",
                        new XAttribute("SchemaVersion", 1),
                        new XElement("IsEnabled", configuration.IsEnabled),
                        new XElement("WorkingRootDirectory", configuration.WorkingRootDirectory ?? string.Empty)));

                document.Save(ConfigurationFilePath);
            }
        }

        private void EnsureConfigurationDirectory()
        {
            Directory.CreateDirectory(_pathProvider.ConfigurationDirectoryPath);
        }

        private static LateralFileSystemConfiguration CreateDefaultConfiguration()
        {
            return new LateralFileSystemConfiguration();
        }
    }
}
