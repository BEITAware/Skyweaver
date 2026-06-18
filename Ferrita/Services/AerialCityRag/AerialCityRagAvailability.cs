using Ferrita.Services.Directories;

namespace Ferrita.Services.AerialCityRag
{
    public static class AerialCityRagAvailability
    {
        private static readonly HashSet<string> s_toolNames = new(StringComparer.OrdinalIgnoreCase)
        {
            "InitializeAerialCityRAG",
            "SemanticSearch",
            "KeywordSearch"
        };

        public static bool IsAerialCityRagTool(string? toolName)
        {
            return !string.IsNullOrWhiteSpace(toolName) && s_toolNames.Contains(toolName.Trim());
        }

        public static bool AreToolsAvailable()
        {
            try
            {
                var configuration = new AerialCityRagConfigurationRepository().Load();
                return configuration.IsEnabled &&
                    !string.IsNullOrWhiteSpace(FerritaDirectoryRuntime.Instance.AerialCityDirectoryPath);
            }
            catch
            {
                return false;
            }
        }
    }
}
