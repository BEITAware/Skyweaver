using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Input;
using Ferrita.Commands;
using Ferrita.Infrastructure.Mvvm;
using Ferrita.Models.Search;
using Ferrita.Services.Directories;
using Ferrita.Services.Localization;
using Ferrita.Services.Search;

namespace Ferrita.Controls.FerritaPreferencesControl.ViewModels.Pages
{
    public sealed class SearchPreferencesPageViewModel : ObservableObject
    {
        private readonly SearchConfigurationRepository _configurationRepository;
        private SearchConfiguration _configuration;
        private string _statusMessage;

        public SearchPreferencesPageViewModel()
        {
            _configurationRepository = new SearchConfigurationRepository();
            _configuration = _configurationRepository.Load();
            _statusMessage = L("Search.Status.Loaded", "搜索配置已加载。");

            OpenConfigurationDirectoryCommand = new RelayCommand(OpenConfigurationDirectory);
            LocalizationRuntime.Instance.LanguageChanged += (_, _) => RefreshLocalizedText();
        }

        public string Title => L("Search.Page.Title", "搜索配置");

        public string Description => L("Search.Page.Description", "配置搜索 API (Brave Search、Tavily、Vertex AI Search) 以供引擎在检索时使用。");

        public IEnumerable<SearchApiTypeOption> SearchApis => new[]
        {
            new SearchApiTypeOption { Type = SearchApiType.BraveSearch, DisplayName = "Brave Search" },
            new SearchApiTypeOption { Type = SearchApiType.Tavily, DisplayName = "Tavily Search" },
            new SearchApiTypeOption { Type = SearchApiType.VertexAiSearch, DisplayName = "Vertex AI Search" }
        };

        public IEnumerable<string> TavilySearchDepthOptions => new[] { "basic", "advanced" };

        public IEnumerable<string> BraveSearchSafeSearchOptions => new[] { "off", "moderate", "strict" };

        public IEnumerable<string> BraveSearchFreshnessOptions => new[] { "none", "pd", "pw", "pm", "py" };

        public IEnumerable<string> TavilyTopicOptions => new[] { "general", "news" };

        public IEnumerable<string> VertexAiSearchModelOptions => new[] { "unstructured", "structured" };

        public SearchApiType SelectedApi
        {
            get => _configuration.SelectedApi;
            set
            {
                if (_configuration.SelectedApi == value)
                {
                    return;
                }

                _configuration.SelectedApi = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsBraveSearchSelected));
                OnPropertyChanged(nameof(IsTavilySelected));
                OnPropertyChanged(nameof(IsVertexAiSearchSelected));
                PersistConfiguration(string.Format(L("Search.Status.ApiChangedFormat", "已切换搜索 API 为：{0}"), value));
            }
        }

        public bool IsBraveSearchSelected => SelectedApi == SearchApiType.BraveSearch;
        public bool IsTavilySelected => SelectedApi == SearchApiType.Tavily;
        public bool IsVertexAiSearchSelected => SelectedApi == SearchApiType.VertexAiSearch;

        // Brave Search
        public string BraveSearchApiKey
        {
            get => _configuration.BraveSearchApiKey;
            set
            {
                if (_configuration.BraveSearchApiKey == value) return;
                _configuration.BraveSearchApiKey = value;
                OnPropertyChanged();
                PersistConfiguration(L("Search.Status.BraveApiKeySaved", "Brave Search API Key 已保存。"));
            }
        }

        public string BraveSearchCountry
        {
            get => _configuration.BraveSearchCountry;
            set
            {
                if (_configuration.BraveSearchCountry == value) return;
                _configuration.BraveSearchCountry = value;
                OnPropertyChanged();
                PersistConfiguration(L("Search.Status.BraveCountrySaved", "Brave Search 国家/地区代码已保存。"));
            }
        }

        public string BraveSearchLanguage
        {
            get => _configuration.BraveSearchLanguage;
            set
            {
                if (_configuration.BraveSearchLanguage == value) return;
                _configuration.BraveSearchLanguage = value;
                OnPropertyChanged();
                PersistConfiguration(L("Search.Status.BraveLanguageSaved", "Brave Search 语言代码已保存。"));
            }
        }

        public string BraveSearchSafeSearch
        {
            get => _configuration.BraveSearchSafeSearch;
            set
            {
                if (_configuration.BraveSearchSafeSearch == value) return;
                _configuration.BraveSearchSafeSearch = value;
                OnPropertyChanged();
                PersistConfiguration(L("Search.Status.BraveSafeSearchSaved", "Brave Search 安全搜索设置已保存。"));
            }
        }

        public int BraveSearchCount
        {
            get => _configuration.BraveSearchCount;
            set
            {
                if (_configuration.BraveSearchCount == value) return;
                _configuration.BraveSearchCount = value;
                OnPropertyChanged();
                PersistConfiguration(string.Format(L("Search.Status.BraveCountSavedFormat", "Brave Search 搜索结果数已设为 {0}。"), value));
            }
        }

        public string BraveSearchFreshness
        {
            get => _configuration.BraveSearchFreshness;
            set
            {
                if (_configuration.BraveSearchFreshness == value) return;
                _configuration.BraveSearchFreshness = value;
                OnPropertyChanged();
                PersistConfiguration(L("Search.Status.BraveFreshnessSaved", "Brave Search 新鲜度设置已保存。"));
            }
        }

        // Tavily
        public string TavilyApiKey
        {
            get => _configuration.TavilyApiKey;
            set
            {
                if (_configuration.TavilyApiKey == value) return;
                _configuration.TavilyApiKey = value;
                OnPropertyChanged();
                PersistConfiguration(L("Search.Status.TavilyApiKeySaved", "Tavily API Key 已保存。"));
            }
        }

        public string TavilySearchDepth
        {
            get => _configuration.TavilySearchDepth;
            set
            {
                if (_configuration.TavilySearchDepth == value) return;
                _configuration.TavilySearchDepth = value;
                OnPropertyChanged();
                PersistConfiguration(L("Search.Status.TavilyDepthSaved", "Tavily 搜索深度已保存。"));
            }
        }

        public bool TavilyIncludeAnswer
        {
            get => _configuration.TavilyIncludeAnswer;
            set
            {
                if (_configuration.TavilyIncludeAnswer == value) return;
                _configuration.TavilyIncludeAnswer = value;
                OnPropertyChanged();
                PersistConfiguration(L("Search.Status.TavilyIncludeAnswerSaved", "Tavily 包含回答选项已更新。"));
            }
        }

        public string TavilyTopic
        {
            get => _configuration.TavilyTopic;
            set
            {
                if (_configuration.TavilyTopic == value) return;
                _configuration.TavilyTopic = value;
                OnPropertyChanged();
                PersistConfiguration(L("Search.Status.TavilyTopicSaved", "Tavily 搜索类别已保存。"));
            }
        }

        public int TavilyMaxResults
        {
            get => _configuration.TavilyMaxResults;
            set
            {
                if (_configuration.TavilyMaxResults == value) return;
                _configuration.TavilyMaxResults = value;
                OnPropertyChanged();
                PersistConfiguration(string.Format(L("Search.Status.TavilyMaxResultsSavedFormat", "Tavily 搜索结果数已设为 {0}。"), value));
            }
        }

        public bool TavilyIncludeImages
        {
            get => _configuration.TavilyIncludeImages;
            set
            {
                if (_configuration.TavilyIncludeImages == value) return;
                _configuration.TavilyIncludeImages = value;
                OnPropertyChanged();
                PersistConfiguration(L("Search.Status.TavilyIncludeImagesSaved", "Tavily 包含图片设置已更新。"));
            }
        }

        public bool TavilyIncludeRawContent
        {
            get => _configuration.TavilyIncludeRawContent;
            set
            {
                if (_configuration.TavilyIncludeRawContent == value) return;
                _configuration.TavilyIncludeRawContent = value;
                OnPropertyChanged();
                PersistConfiguration(L("Search.Status.TavilyIncludeRawContentSaved", "Tavily 包含网页原文设置已更新。"));
            }
        }

        // Vertex AI Search
        public string VertexAiProjectId
        {
            get => _configuration.VertexAiProjectId;
            set
            {
                if (_configuration.VertexAiProjectId == value) return;
                _configuration.VertexAiProjectId = value;
                OnPropertyChanged();
                PersistConfiguration(L("Search.Status.VertexProjectIdSaved", "Vertex AI Project ID 已保存。"));
            }
        }

        public string VertexAiLocation
        {
            get => _configuration.VertexAiLocation;
            set
            {
                if (_configuration.VertexAiLocation == value) return;
                _configuration.VertexAiLocation = value;
                OnPropertyChanged();
                PersistConfiguration(L("Search.Status.VertexLocationSaved", "Vertex AI 区域位置已保存。"));
            }
        }

        public string VertexAiDataStoreId
        {
            get => _configuration.VertexAiDataStoreId;
            set
            {
                if (_configuration.VertexAiDataStoreId == value) return;
                _configuration.VertexAiDataStoreId = value;
                OnPropertyChanged();
                PersistConfiguration(L("Search.Status.VertexDataStoreIdSaved", "Vertex AI DataStore ID 已保存。"));
            }
        }

        public string VertexAiCredentialsJson
        {
            get => _configuration.VertexAiCredentialsJson;
            set
            {
                if (_configuration.VertexAiCredentialsJson == value) return;
                _configuration.VertexAiCredentialsJson = value;
                OnPropertyChanged();
                PersistConfiguration(L("Search.Status.VertexCredentialsSaved", "Vertex AI 凭证 JSON 已保存。"));
            }
        }

        public string VertexAiApiKey
        {
            get => _configuration.VertexAiApiKey;
            set
            {
                if (_configuration.VertexAiApiKey == value) return;
                _configuration.VertexAiApiKey = value;
                OnPropertyChanged();
                PersistConfiguration(L("Search.Status.VertexApiKeySaved", "Vertex AI API Key 已保存。"));
            }
        }

        public int VertexAiMaxResults
        {
            get => _configuration.VertexAiMaxResults;
            set
            {
                if (_configuration.VertexAiMaxResults == value) return;
                _configuration.VertexAiMaxResults = value;
                OnPropertyChanged();
                PersistConfiguration(string.Format(L("Search.Status.VertexMaxResultsSavedFormat", "Vertex AI 搜索结果数已设为 {0}。"), value));
            }
        }

        public string VertexAiSearchModel
        {
            get => _configuration.VertexAiSearchModel;
            set
            {
                if (_configuration.VertexAiSearchModel == value) return;
                _configuration.VertexAiSearchModel = value;
                OnPropertyChanged();
                PersistConfiguration(L("Search.Status.VertexSearchModelSaved", "Vertex AI 搜索模式已保存。"));
            }
        }

        public string ConfigurationFilePath => _configurationRepository.ConfigurationFilePath;

        public string StatusMessage
        {
            get => _statusMessage;
            private set => SetProperty(ref _statusMessage, value);
        }

        public ICommand OpenConfigurationDirectoryCommand { get; }

        private void OpenConfigurationDirectory()
        {
            try
            {
                var directoryPath = Path.GetDirectoryName(ConfigurationFilePath) ?? FerritaDirectoryRuntime.Instance.ConfigurationDirectoryPath;
                Directory.CreateDirectory(directoryPath);
                Process.Start(new ProcessStartInfo
                {
                    FileName = directoryPath,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format(L("Common.Status.OpenConfigurationDirectoryFailedFormat", "打开配置目录失败：{0}"), ex.Message);
            }
        }

        private void PersistConfiguration(string successMessage)
        {
            try
            {
                _configurationRepository.Save(_configuration);
                StatusMessage = successMessage;
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format(L("Search.Status.SaveFailedFormat", "保存失败：{0}"), ex.Message);
            }
        }

        private void RefreshLocalizedText()
        {
            OnPropertyChanged(nameof(Title));
            OnPropertyChanged(nameof(Description));
        }

        private static string L(string resourceKey, string fallback)
        {
            return LocalizationRuntime.Instance.GetString(resourceKey, fallback);
        }
    }

    public sealed class SearchApiTypeOption
    {
        public SearchApiType Type { get; init; }
        public required string DisplayName { get; init; }
    }
}
