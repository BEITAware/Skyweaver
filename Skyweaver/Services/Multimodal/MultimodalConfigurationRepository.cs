using System;
using System.IO;
using System.Xml.Linq;
using Skyweaver.Models.Multimodal;
using Skyweaver.Services.Directories;

namespace Skyweaver.Services.Multimodal
{
    public sealed class MultimodalConfigurationRepository
    {
        private readonly object _syncRoot = new();

        public string ConfigurationDirectoryPath => SkyweaverDirectoryRuntime.Instance.ConfigurationDirectoryPath;

        public string ConfigurationFilePath => Path.Combine(ConfigurationDirectoryPath, "MultimodalPreferences.xml");

        public MultimodalConfiguration Load()
        {
            lock (_syncRoot)
            {
                EnsureConfigurationDirectory();

                if (!File.Exists(ConfigurationFilePath))
                {
                    var configuration = new MultimodalConfiguration();
                    Save(configuration);
                    return configuration;
                }

                try
                {
                    var document = XDocument.Load(ConfigurationFilePath);
                    var root = document.Root ?? throw new InvalidDataException("MultimodalPreferences.xml is missing its root element.");
                    
                    var enableOcrStr = (string?)root.Attribute("EnableDocumentCharacterRecognition") ?? (string?)root.Element("EnableDocumentCharacterRecognition") ?? "false";
                    bool.TryParse(enableOcrStr, out var enableOcr);

                    var hardwareSolution = (string?)root.Attribute("HardwareSolution") ?? (string?)root.Element("HardwareSolution") ?? "CPU";

                    return new MultimodalConfiguration
                    {
                        EnableDocumentCharacterRecognition = enableOcr,
                        HardwareSolution = hardwareSolution
                    };
                }
                catch
                {
                    var configuration = new MultimodalConfiguration();
                    Save(configuration);
                    return configuration;
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
                    new XElement("MultimodalPreferences",
                        new XAttribute("SchemaVersion", 1),
                        new XAttribute("EnableDocumentCharacterRecognition", configuration.EnableDocumentCharacterRecognition),
                        new XAttribute("HardwareSolution", configuration.HardwareSolution)
                    )
                );

                document.Save(ConfigurationFilePath);
            }
        }

        private void EnsureConfigurationDirectory()
        {
            Directory.CreateDirectory(ConfigurationDirectoryPath);
        }
    }
}
