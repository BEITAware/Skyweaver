using System;
using System.Globalization;
using System.IO;
using System.Xml.Linq;
using Skyweaver.Models.ContextManagement;

namespace Skyweaver.Services.ContextManagement
{
    public sealed class ContextManagementConfigurationRepository
    {
        private readonly ContextManagementPathProvider _pathProvider;
        private readonly object _syncRoot = new();

        public ContextManagementConfigurationRepository(ContextManagementPathProvider pathProvider)
        {
            _pathProvider = pathProvider;
        }

        public string ConfigurationFilePath => _pathProvider.ConfigurationFilePath;

        public ContextManagementConfiguration Load()
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
                var root = document.Root ?? throw new InvalidDataException("ContextManagement 配置 XML 缺少根节点。");

                return new ContextManagementConfiguration
                {
                    MinCompactionEnabled = (bool?)root.Element("MinCompactionEnabled") ?? false,
                    MaxCompactionEnabled = (bool?)root.Element("MaxCompactionEnabled") ?? true,
                    LifeCycleEnabled = (bool?)root.Element("LifeCycleEnabled") ?? false,
                    LifeCycleRatioPercent = ReadDouble(root.Element("LifeCycleRatioPercent"), 100d),
                    RnnOptimizedCompactionEnabled = (bool?)root.Element("RnnOptimizedCompactionEnabled") ?? false,
                    MemoryEnabled = (bool?)root.Element("MemoryEnabled") ?? false,
                    MemoryShareScope = ReadEnum(root.Element("MemoryShareScope"), MemoryShareScope.SessionFlow),
                    MemoryRetrievalCount = (int?)root.Element("MemoryRetrievalCount") ?? 5
                };
            }
        }

        public void Save(ContextManagementConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(configuration);

            lock (_syncRoot)
            {
                EnsureConfigurationDirectory();

                var document = new XDocument(
                    new XElement("ContextManagementConfiguration",
                        new XAttribute("SchemaVersion", 1),
                        new XElement("MinCompactionEnabled", configuration.MinCompactionEnabled),
                        new XElement("MaxCompactionEnabled", configuration.MaxCompactionEnabled),
                        new XElement("LifeCycleEnabled", configuration.LifeCycleEnabled),
                        new XElement("LifeCycleRatioPercent", configuration.LifeCycleRatioPercent.ToString("0.##", CultureInfo.InvariantCulture)),
                        new XElement("RnnOptimizedCompactionEnabled", configuration.RnnOptimizedCompactionEnabled),
                        new XElement("MemoryEnabled", configuration.MemoryEnabled),
                        new XElement("MemoryShareScope", configuration.MemoryShareScope.ToString()),
                        new XElement("MemoryRetrievalCount", configuration.MemoryRetrievalCount)));

                document.Save(ConfigurationFilePath);
            }
        }

        private void EnsureConfigurationDirectory()
        {
            Directory.CreateDirectory(_pathProvider.ConfigurationDirectoryPath);
        }

        private static ContextManagementConfiguration CreateDefaultConfiguration()
        {
            return new ContextManagementConfiguration();
        }

        private static double ReadDouble(XElement? element, double fallback)
        {
            var text = (string?)element;
            return double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out var value)
                ? Math.Clamp(value, 10d, 500d)
                : fallback;
        }

        private static T ReadEnum<T>(XElement? element, T fallback) where T : struct, Enum
        {
            var text = (string?)element;
            return Enum.TryParse<T>(text, out var value) ? value : fallback;
        }
    }
}
