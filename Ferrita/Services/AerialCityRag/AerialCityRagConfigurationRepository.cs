using System.IO;
using System.Xml.Linq;
using Ferrita.Models.AerialCityRag;
using Ferrita.Services.Directories;

namespace Ferrita.Services.AerialCityRag
{
    public sealed class AerialCityRagConfigurationRepository
    {
        private readonly object _syncRoot = new();

        public string ConfigurationDirectoryPath => FerritaDirectoryRuntime.Instance.ConfigurationDirectoryPath;

        public string ConfigurationFilePath => Path.Combine(ConfigurationDirectoryPath, "SemanticSearch.xml");

        public AerialCityRagConfiguration Load()
        {
            lock (_syncRoot)
            {
                EnsureConfigurationDirectory();

                if (!File.Exists(ConfigurationFilePath))
                {
                    var configuration = new AerialCityRagConfiguration();
                    Save(configuration);
                    return configuration;
                }

                var document = XDocument.Load(ConfigurationFilePath);
                var root = document.Root ?? throw new InvalidDataException("SemanticSearch.xml is missing its root element.");
                var rag = root.Element("AerialCityRag") ?? root;

                return new AerialCityRagConfiguration
                {
                    IsEnabled = ParseBool((string?)rag.Attribute("Enabled") ?? (string?)rag.Element("Enabled")),
                    SelectedEmbeddingModelKey = ((string?)rag.Attribute("SelectedEmbeddingModelKey")
                        ?? (string?)rag.Element("SelectedEmbeddingModelKey")
                        ?? string.Empty).Trim(),
                    EmbeddingConcurrency = ParseInt(
                        (string?)rag.Attribute("EmbeddingConcurrency") ??
                        (string?)rag.Element("EmbeddingConcurrency"),
                        AerialCityRagConfiguration.MinimumEmbeddingConcurrency)
                };
            }
        }

        public void Save(AerialCityRagConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(configuration);

            lock (_syncRoot)
            {
                EnsureConfigurationDirectory();

                var document = new XDocument(
                    new XElement("SemanticSearch",
                        new XAttribute("SchemaVersion", 1),
                        new XElement("AerialCityRag",
                            new XAttribute("Enabled", configuration.IsEnabled),
                            new XAttribute("SelectedEmbeddingModelKey", configuration.SelectedEmbeddingModelKey),
                            new XAttribute("EmbeddingConcurrency", configuration.EmbeddingConcurrency))));

                document.Save(ConfigurationFilePath);
            }
        }

        private void EnsureConfigurationDirectory()
        {
            Directory.CreateDirectory(ConfigurationDirectoryPath);
        }

        private static bool ParseBool(string? value)
        {
            return bool.TryParse(value, out var parsed) && parsed;
        }

        private static int ParseInt(string? value, int fallback)
        {
            return int.TryParse(value, out var parsed) ? parsed : fallback;
        }
    }
}
