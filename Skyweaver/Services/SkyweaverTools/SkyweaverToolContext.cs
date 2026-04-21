using System.Xml.Linq;

namespace Skyweaver.Services.SkyweaverTools
{
    public sealed class SkyweaverToolContext
    {
        public string ApplicationName { get; init; } = "Skyweaver";

        public string? SessionTitle { get; init; }

        public string? WorkspacePath { get; init; }

        public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.Now;

        public IReadOnlyDictionary<string, string> Properties { get; init; } =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public string? CurrentToolName { get; init; }

        public SkyweaverToolConfigurationState? CurrentToolConfiguration { get; init; }

        public SkyweaverToolContext WithCurrentToolConfiguration(string toolName, XElement? configuration)
        {
            var normalizedToolName = (toolName ?? string.Empty).Trim();
            if (normalizedToolName.Length == 0)
            {
                throw new ArgumentException("Tool name cannot be empty.", nameof(toolName));
            }

            return new SkyweaverToolContext
            {
                ApplicationName = ApplicationName,
                SessionTitle = SessionTitle,
                WorkspacePath = WorkspacePath,
                Timestamp = Timestamp,
                Properties = CloneProperties(Properties),
                CurrentToolName = normalizedToolName,
                CurrentToolConfiguration = new SkyweaverToolConfigurationState(normalizedToolName, configuration)
            };
        }

        private static IReadOnlyDictionary<string, string> CloneProperties(IReadOnlyDictionary<string, string> properties)
        {
            return properties.Count == 0
                ? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                : new Dictionary<string, string>(properties, StringComparer.OrdinalIgnoreCase);
        }
    }
}
