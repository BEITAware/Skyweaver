using System.IO;
using System.Xml.Linq;
using Skyweaver.Services.Directories;

namespace Skyweaver.Services.SkyweaverTools
{
    public sealed class SkyweaverToolConfigurationRepository
    {
        private readonly object _syncRoot = new();

        public string ConfigurationDirectoryPath => SkyweaverDirectoryRuntime.Instance.ConfigurationDirectoryPath;

        public string ConfigurationFilePath => Path.Combine(ConfigurationDirectoryPath, "ToolConfiguration.xml");

        public IReadOnlyDictionary<string, SkyweaverToolPersistedState> Load()
        {
            lock (_syncRoot)
            {
                EnsureConfigurationDirectory();

                if (!File.Exists(ConfigurationFilePath))
                {
                    Save(Array.Empty<SkyweaverToolPersistedState>());
                    return new Dictionary<string, SkyweaverToolPersistedState>(StringComparer.OrdinalIgnoreCase);
                }

                var document = XDocument.Load(ConfigurationFilePath);
                var root = document.Root ?? throw new InvalidDataException("ToolConfiguration.xml is missing its root element.");

                return root.Elements("Tool")
                    .Select(ParsePersistedState)
                    .Where(state => state != null)
                    .Cast<SkyweaverToolPersistedState>()
                    .GroupBy(state => state.ToolName, StringComparer.OrdinalIgnoreCase)
                    .ToDictionary(
                        group => group.Key,
                        group => group.Last(),
                        StringComparer.OrdinalIgnoreCase);
            }
        }

        public void Save(IEnumerable<SkyweaverToolPersistedState> states)
        {
            ArgumentNullException.ThrowIfNull(states);

            lock (_syncRoot)
            {
                EnsureConfigurationDirectory();

                var document = new XDocument(
                    new XElement("Tools",
                        new XAttribute("SchemaVersion", 2),
                        states
                            .Where(item => !string.IsNullOrWhiteSpace(item.ToolName))
                            .OrderBy(item => item.ToolName, StringComparer.OrdinalIgnoreCase)
                            .Select(CreateToolElement)));

                document.Save(ConfigurationFilePath);
            }
        }

        private void EnsureConfigurationDirectory()
        {
            Directory.CreateDirectory(ConfigurationDirectoryPath);
        }

        private static bool ParseBool(string? value, bool fallback)
        {
            return bool.TryParse(value, out var parsed) ? parsed : fallback;
        }

        private static SkyweaverToolPersistedState? ParsePersistedState(XElement toolElement)
        {
            var toolName = ((string?)toolElement.Attribute("Name") ?? string.Empty).Trim();
            if (toolName.Length == 0)
            {
                return null;
            }

            var configuration = toolElement.Element("Configuration")?.Elements().FirstOrDefault()
                ?? toolElement.Elements().FirstOrDefault(element =>
                    !string.Equals(element.Name.LocalName, "Configuration", StringComparison.OrdinalIgnoreCase));

            return new SkyweaverToolPersistedState(
                toolName,
                ParseBool((string?)toolElement.Attribute("Enabled"), true),
                configuration);
        }

        private static XElement CreateToolElement(SkyweaverToolPersistedState state)
        {
            var toolElement = new XElement("Tool",
                new XAttribute("Name", state.ToolName),
                new XAttribute("Enabled", state.IsEnabled));

            var configuration = state.GetConfiguration();
            if (configuration != null)
            {
                toolElement.Add(new XElement("Configuration", configuration));
            }

            return toolElement;
        }
    }
}
