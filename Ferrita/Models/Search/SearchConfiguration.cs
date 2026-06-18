using System;
using Ferrita.Infrastructure.Mvvm;

namespace Ferrita.Models.Search
{
    public enum SearchApiType
    {
        BraveSearch,
        Tavily,
        VertexAiSearch
    }

    public sealed class SearchConfiguration : ObservableObject
    {
        private SearchApiType _selectedApi = SearchApiType.BraveSearch;

        // Brave Search API
        private string _braveSearchApiKey = string.Empty;
        private string _braveSearchCountry = "US";
        private string _braveSearchLanguage = "en";
        private string _braveSearchSafeSearch = "moderate"; // off, moderate, strict
        private int _braveSearchCount = 20;
        private string _braveSearchFreshness = "none"; // none, pd, pw, pm, py

        // Tavily API
        private string _tavilyApiKey = string.Empty;
        private string _tavilySearchDepth = "basic"; // basic, advanced
        private bool _tavilyIncludeAnswer;
        private string _tavilyTopic = "general"; // general, news
        private int _tavilyMaxResults = 5;
        private bool _tavilyIncludeImages;
        private bool _tavilyIncludeRawContent;

        // Vertex AI Search
        private string _vertexAiProjectId = string.Empty;
        private string _vertexAiLocation = "global";
        private string _vertexAiDataStoreId = string.Empty;
        private string _vertexAiCredentialsJson = string.Empty;
        private string _vertexAiApiKey = string.Empty;
        private int _vertexAiMaxResults = 10;
        private string _vertexAiSearchModel = "unstructured"; // unstructured, structured

        public SearchApiType SelectedApi
        {
            get => _selectedApi;
            set => SetProperty(ref _selectedApi, value);
        }

        public string BraveSearchApiKey
        {
            get => _braveSearchApiKey;
            set => SetProperty(ref _braveSearchApiKey, value?.Trim() ?? string.Empty);
        }

        public string BraveSearchCountry
        {
            get => _braveSearchCountry;
            set => SetProperty(ref _braveSearchCountry, value?.Trim() ?? "US");
        }

        public string BraveSearchLanguage
        {
            get => _braveSearchLanguage;
            set => SetProperty(ref _braveSearchLanguage, value?.Trim() ?? "en");
        }

        public string BraveSearchSafeSearch
        {
            get => _braveSearchSafeSearch;
            set => SetProperty(ref _braveSearchSafeSearch, value?.Trim() ?? "moderate");
        }

        public int BraveSearchCount
        {
            get => _braveSearchCount;
            set => SetProperty(ref _braveSearchCount, value);
        }

        public string BraveSearchFreshness
        {
            get => _braveSearchFreshness;
            set => SetProperty(ref _braveSearchFreshness, value?.Trim() ?? "none");
        }

        public string TavilyApiKey
        {
            get => _tavilyApiKey;
            set => SetProperty(ref _tavilyApiKey, value?.Trim() ?? string.Empty);
        }

        public string TavilySearchDepth
        {
            get => _tavilySearchDepth;
            set => SetProperty(ref _tavilySearchDepth, value?.Trim() ?? "basic");
        }

        public bool TavilyIncludeAnswer
        {
            get => _tavilyIncludeAnswer;
            set => SetProperty(ref _tavilyIncludeAnswer, value);
        }

        public string TavilyTopic
        {
            get => _tavilyTopic;
            set => SetProperty(ref _tavilyTopic, value?.Trim() ?? "general");
        }

        public int TavilyMaxResults
        {
            get => _tavilyMaxResults;
            set => SetProperty(ref _tavilyMaxResults, value);
        }

        public bool TavilyIncludeImages
        {
            get => _tavilyIncludeImages;
            set => SetProperty(ref _tavilyIncludeImages, value);
        }

        public bool TavilyIncludeRawContent
        {
            get => _tavilyIncludeRawContent;
            set => SetProperty(ref _tavilyIncludeRawContent, value);
        }

        public string VertexAiProjectId
        {
            get => _vertexAiProjectId;
            set => SetProperty(ref _vertexAiProjectId, value?.Trim() ?? string.Empty);
        }

        public string VertexAiLocation
        {
            get => _vertexAiLocation;
            set => SetProperty(ref _vertexAiLocation, value?.Trim() ?? "global");
        }

        public string VertexAiDataStoreId
        {
            get => _vertexAiDataStoreId;
            set => SetProperty(ref _vertexAiDataStoreId, value?.Trim() ?? string.Empty);
        }

        public string VertexAiCredentialsJson
        {
            get => _vertexAiCredentialsJson;
            set => SetProperty(ref _vertexAiCredentialsJson, value?.Trim() ?? string.Empty);
        }

        public string VertexAiApiKey
        {
            get => _vertexAiApiKey;
            set => SetProperty(ref _vertexAiApiKey, value?.Trim() ?? string.Empty);
        }

        public int VertexAiMaxResults
        {
            get => _vertexAiMaxResults;
            set => SetProperty(ref _vertexAiMaxResults, value);
        }

        public string VertexAiSearchModel
        {
            get => _vertexAiSearchModel;
            set => SetProperty(ref _vertexAiSearchModel, value?.Trim() ?? "unstructured");
        }
    }
}
