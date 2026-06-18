using System;
using System.IO;
using System.Xml.Linq;
using Ferrita.Models.Search;
using Ferrita.Services.Directories;

namespace Ferrita.Services.Search
{
    public sealed class SearchConfigurationRepository
    {
        private readonly object _syncRoot = new();

        public string ConfigurationDirectoryPath => FerritaDirectoryRuntime.Instance.ConfigurationDirectoryPath;

        public string ConfigurationFilePath => Path.Combine(ConfigurationDirectoryPath, "SearchPreferences.xml");

        public SearchConfiguration Load()
        {
            lock (_syncRoot)
            {
                EnsureConfigurationDirectory();

                if (!File.Exists(ConfigurationFilePath))
                {
                    var configuration = new SearchConfiguration();
                    Save(configuration);
                    return configuration;
                }

                try
                {
                    var document = XDocument.Load(ConfigurationFilePath);
                    var root = document.Root ?? throw new InvalidDataException("SearchPreferences.xml is missing its root element.");
                    
                    var selectedApiStr = (string?)root.Attribute("SelectedApi") ?? (string?)root.Element("SelectedApi") ?? "BraveSearch";
                    Enum.TryParse(selectedApiStr, out SearchApiType selectedApi);

                    var config = new SearchConfiguration
                    {
                        SelectedApi = selectedApi
                    };

                    var brave = root.Element("BraveSearch");
                    if (brave != null)
                    {
                        config.BraveSearchApiKey = ((string?)brave.Attribute("ApiKey") ?? (string?)brave.Element("ApiKey") ?? string.Empty).Trim();
                        config.BraveSearchCountry = ((string?)brave.Attribute("Country") ?? (string?)brave.Element("Country") ?? "US").Trim();
                        config.BraveSearchLanguage = ((string?)brave.Attribute("Language") ?? (string?)brave.Element("Language") ?? "en").Trim();
                        config.BraveSearchSafeSearch = ((string?)brave.Attribute("SafeSearch") ?? (string?)brave.Element("SafeSearch") ?? "moderate").Trim();
                        config.BraveSearchCount = ParseInt((string?)brave.Attribute("Count") ?? (string?)brave.Element("Count"), 20);
                        config.BraveSearchFreshness = ((string?)brave.Attribute("Freshness") ?? (string?)brave.Element("Freshness") ?? "none").Trim();
                    }

                    var tavily = root.Element("Tavily");
                    if (tavily != null)
                    {
                        config.TavilyApiKey = ((string?)tavily.Attribute("ApiKey") ?? (string?)tavily.Element("ApiKey") ?? string.Empty).Trim();
                        config.TavilySearchDepth = ((string?)tavily.Attribute("SearchDepth") ?? (string?)tavily.Element("SearchDepth") ?? "basic").Trim();
                        config.TavilyIncludeAnswer = ParseBool((string?)tavily.Attribute("IncludeAnswer") ?? (string?)tavily.Element("IncludeAnswer"));
                        config.TavilyTopic = ((string?)tavily.Attribute("Topic") ?? (string?)tavily.Element("Topic") ?? "general").Trim();
                        config.TavilyMaxResults = ParseInt((string?)tavily.Attribute("MaxResults") ?? (string?)tavily.Element("MaxResults"), 5);
                        config.TavilyIncludeImages = ParseBool((string?)tavily.Attribute("IncludeImages") ?? (string?)tavily.Element("IncludeImages"));
                        config.TavilyIncludeRawContent = ParseBool((string?)tavily.Attribute("IncludeRawContent") ?? (string?)tavily.Element("IncludeRawContent"));
                    }

                    var vertex = root.Element("VertexAiSearch");
                    if (vertex != null)
                    {
                        config.VertexAiProjectId = ((string?)vertex.Attribute("ProjectId") ?? (string?)vertex.Element("ProjectId") ?? string.Empty).Trim();
                        config.VertexAiLocation = ((string?)vertex.Attribute("Location") ?? (string?)vertex.Element("Location") ?? "global").Trim();
                        config.VertexAiDataStoreId = ((string?)vertex.Attribute("DataStoreId") ?? (string?)vertex.Element("DataStoreId") ?? string.Empty).Trim();
                        config.VertexAiCredentialsJson = ((string?)vertex.Attribute("CredentialsJson") ?? (string?)vertex.Element("CredentialsJson") ?? string.Empty).Trim();
                        config.VertexAiApiKey = ((string?)vertex.Attribute("ApiKey") ?? (string?)vertex.Element("ApiKey") ?? string.Empty).Trim();
                        config.VertexAiMaxResults = ParseInt((string?)vertex.Attribute("MaxResults") ?? (string?)vertex.Element("MaxResults"), 10);
                        config.VertexAiSearchModel = ((string?)vertex.Attribute("SearchModel") ?? (string?)vertex.Element("SearchModel") ?? "unstructured").Trim();
                    }

                    return config;
                }
                catch
                {
                    var configuration = new SearchConfiguration();
                    Save(configuration);
                    return configuration;
                }
            }
        }

        public void Save(SearchConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(configuration);

            lock (_syncRoot)
            {
                EnsureConfigurationDirectory();

                var document = new XDocument(
                    new XElement("SearchPreferences",
                        new XAttribute("SchemaVersion", 1),
                        new XAttribute("SelectedApi", configuration.SelectedApi.ToString()),
                        new XElement("BraveSearch",
                            new XAttribute("ApiKey", configuration.BraveSearchApiKey),
                            new XAttribute("Country", configuration.BraveSearchCountry),
                            new XAttribute("Language", configuration.BraveSearchLanguage),
                            new XAttribute("SafeSearch", configuration.BraveSearchSafeSearch),
                            new XAttribute("Count", configuration.BraveSearchCount),
                            new XAttribute("Freshness", configuration.BraveSearchFreshness)),
                        new XElement("Tavily",
                            new XAttribute("ApiKey", configuration.TavilyApiKey),
                            new XAttribute("SearchDepth", configuration.TavilySearchDepth),
                            new XAttribute("IncludeAnswer", configuration.TavilyIncludeAnswer),
                            new XAttribute("Topic", configuration.TavilyTopic),
                            new XAttribute("MaxResults", configuration.TavilyMaxResults),
                            new XAttribute("IncludeImages", configuration.TavilyIncludeImages),
                            new XAttribute("IncludeRawContent", configuration.TavilyIncludeRawContent)),
                        new XElement("VertexAiSearch",
                            new XAttribute("ProjectId", configuration.VertexAiProjectId),
                            new XAttribute("Location", configuration.VertexAiLocation),
                            new XAttribute("DataStoreId", configuration.VertexAiDataStoreId),
                            new XAttribute("CredentialsJson", configuration.VertexAiCredentialsJson),
                            new XAttribute("ApiKey", configuration.VertexAiApiKey),
                            new XAttribute("MaxResults", configuration.VertexAiMaxResults),
                            new XAttribute("SearchModel", configuration.VertexAiSearchModel))
                    )
                );

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

        private static int ParseInt(string? value, int defaultValue)
        {
            return int.TryParse(value, out var parsed) ? parsed : defaultValue;
        }
    }
}
