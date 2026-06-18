using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Input;
using Ferrita.Commands;
using Ferrita.Controls.EmbeddingModelConfigurationControl.Models;
using Ferrita.Controls.EmbeddingModelConfigurationControl.Services;
using Ferrita.Infrastructure.Mvvm;
using Ferrita.Models.AerialCityRag;
using Ferrita.Services.AerialCityRag;
using Ferrita.Services.Directories;
using Ferrita.Services.Localization;

namespace Ferrita.Controls.FerritaPreferencesControl.ViewModels.Pages
{
    public sealed class SemanticSearchPreferencesPageViewModel : ObservableObject
    {
        private readonly AerialCityRagConfigurationRepository _configurationRepository;
        private readonly EmbeddingModelConfigurationRepository _embeddingModelRepository;
        private AerialCityRagConfiguration _configuration;
        private EmbeddingModelOption? _selectedEmbeddingModel;
        private string _statusMessage;

        public SemanticSearchPreferencesPageViewModel()
        {
            _configurationRepository = new AerialCityRagConfigurationRepository();
            _embeddingModelRepository = new EmbeddingModelConfigurationRepository(new EmbeddingModelConfigurationPathProvider());
            _configuration = _configurationRepository.Load();
            _statusMessage = L("SemanticSearch.Status.Loaded", "语义搜索配置已加载。");

            RefreshEmbeddingModelsCommand = new RelayCommand(RefreshEmbeddingModels);
            OpenConfigurationDirectoryCommand = new RelayCommand(OpenConfigurationDirectory);
            LocalizationRuntime.Instance.LanguageChanged += (_, _) => RefreshLocalizedText();

            RefreshEmbeddingModels();
        }

        public string Title => L("SemanticSearch.Page.Title", "语义搜索");

        public string Description => L("SemanticSearch.Page.Description", "配置 AerialCity RAG 工具的启用状态与检索时使用的嵌入模型。");

        public ObservableCollection<EmbeddingModelOption> EmbeddingModels { get; } = new();

        public bool IsAerialCityRagEnabled
        {
            get => _configuration.IsEnabled;
            set
            {
                if (_configuration.IsEnabled == value)
                {
                    return;
                }

                _configuration.IsEnabled = value;
                OnPropertyChanged();
                PersistConfiguration(value ? L("SemanticSearch.Status.RagEnabled", "AerialCity RAG 已启用。") : L("SemanticSearch.Status.RagDisabled", "AerialCity RAG 已关闭。"));
            }
        }

        public EmbeddingModelOption? SelectedEmbeddingModel
        {
            get => _selectedEmbeddingModel;
            set
            {
                if (!SetProperty(ref _selectedEmbeddingModel, value))
                {
                    return;
                }

                _configuration.SelectedEmbeddingModelKey = value?.Key ?? string.Empty;
                OnPropertyChanged(nameof(SelectedEmbeddingModelSummary));
                PersistConfiguration(value == null
                    ? L("SemanticSearch.Status.EmbeddingModelCleared", "嵌入模型选择已清空。")
                    : string.Format(L("SemanticSearch.Status.EmbeddingModelSelectedFormat", "已选择嵌入模型：{0}"), value.DisplayName));
            }
        }

        public string SelectedEmbeddingModelSummary => SelectedEmbeddingModel?.Summary ?? L("SemanticSearch.EmbeddingModel.NotSelected", "未选择嵌入模型");

        public int MinimumEmbeddingConcurrency => AerialCityRagConfiguration.MinimumEmbeddingConcurrency;

        public int MaximumEmbeddingConcurrency => AerialCityRagConfiguration.MaximumEmbeddingConcurrency;

        public int EmbeddingConcurrency
        {
            get => _configuration.EmbeddingConcurrency;
            set
            {
                if (_configuration.EmbeddingConcurrency == value)
                {
                    return;
                }

                _configuration.EmbeddingConcurrency = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(EmbeddingConcurrencySummary));
                PersistConfiguration(string.Format(L("SemanticSearch.Status.EmbeddingConcurrencySavedFormat", "嵌入并发量已设为 {0}。"), _configuration.EmbeddingConcurrency));
            }
        }

        public string EmbeddingConcurrencySummary => string.Format(L("SemanticSearch.EmbeddingConcurrency.SummaryFormat", "{0} 个并发请求"), EmbeddingConcurrency);

        public string AerialCityDirectoryPath => FerritaDirectoryRuntime.Instance.AerialCityDirectoryPath;

        public string ConfigurationFilePath => _configurationRepository.ConfigurationFilePath;

        public string StatusMessage
        {
            get => _statusMessage;
            private set => SetProperty(ref _statusMessage, value);
        }

        public ICommand RefreshEmbeddingModelsCommand { get; }

        public ICommand OpenConfigurationDirectoryCommand { get; }

        private void RefreshEmbeddingModels()
        {
            var selectedKey = _configuration.SelectedEmbeddingModelKey;
            EmbeddingModels.Clear();

            foreach (var model in _embeddingModelRepository.Load())
            {
                EmbeddingModels.Add(EmbeddingModelOption.FromDefinition(model));
            }

            SelectedEmbeddingModel = EmbeddingModels.FirstOrDefault(model =>
                    string.Equals(model.Key, selectedKey, StringComparison.Ordinal)) ??
                EmbeddingModels.FirstOrDefault();

            if (SelectedEmbeddingModel != null && !string.Equals(selectedKey, SelectedEmbeddingModel.Key, StringComparison.Ordinal))
            {
                _configuration.SelectedEmbeddingModelKey = SelectedEmbeddingModel.Key;
                PersistConfiguration(string.Format(L("SemanticSearch.Status.EmbeddingModelAutoSelectedFormat", "已自动选择嵌入模型：{0}"), SelectedEmbeddingModel.DisplayName));
                return;
            }

            StatusMessage = EmbeddingModels.Count == 0
                ? L("SemanticSearch.Status.NoEmbeddingModels", "尚未配置嵌入模型。请先在嵌入模型配置面板中添加模型。")
                : string.Format(L("SemanticSearch.Status.EmbeddingModelsLoadedFormat", "已获取 {0} 个嵌入模型。"), EmbeddingModels.Count);
        }

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
                _configuration = _configurationRepository.Load();
                OnPropertyChanged(nameof(IsAerialCityRagEnabled));
                OnPropertyChanged(nameof(EmbeddingConcurrency));
                OnPropertyChanged(nameof(EmbeddingConcurrencySummary));
                OnPropertyChanged(nameof(AerialCityDirectoryPath));
                OnPropertyChanged(nameof(ConfigurationFilePath));
                StatusMessage = successMessage;
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format(L("Localization.Status.SaveFailedFormat", "保存失败：{0}"), ex.Message);
            }
        }

        private void RefreshLocalizedText()
        {
            OnPropertyChanged(nameof(Title));
            OnPropertyChanged(nameof(Description));
            OnPropertyChanged(nameof(SelectedEmbeddingModelSummary));
            OnPropertyChanged(nameof(EmbeddingConcurrencySummary));
        }

        private static string L(string resourceKey, string fallback)
        {
            return LocalizationRuntime.Instance.GetString(resourceKey, fallback);
        }

        public sealed class EmbeddingModelOption
        {
            public required string Key { get; init; }

            public required string DisplayName { get; init; }

            public required string Summary { get; init; }

            public bool IsFullyConfigured { get; init; }

            public bool SupportsMultimodalEmbedding { get; init; }

            public static EmbeddingModelOption FromDefinition(EmbeddingModelDefinition definition)
            {
                var displayName = string.IsNullOrWhiteSpace(definition.DisplayName)
                    ? definition.SummaryModelId
                    : definition.DisplayName;

                if (string.IsNullOrWhiteSpace(displayName))
                {
                    displayName = definition.Key;
                }

                var readiness = definition.IsFullyConfigured
                    ? L("SemanticSearch.EmbeddingModel.Readiness.Configured", "已配置")
                    : L("SemanticSearch.EmbeddingModel.Readiness.Incomplete", "未完成");
                var multimodal = definition.SupportsMultimodalEmbedding
                    ? L("SemanticSearch.EmbeddingModel.Modality.Multimodal", "多模态")
                    : L("SemanticSearch.EmbeddingModel.Modality.Text", "文本");
                var modelId = string.IsNullOrWhiteSpace(definition.SummaryModelId)
                    ? definition.InterfaceType
                    : $"{definition.InterfaceType} / {definition.SummaryModelId}";

                return new EmbeddingModelOption
                {
                    Key = definition.Key,
                    DisplayName = displayName,
                    Summary = $"{modelId}，{definition.Dimensions:N0}d，{multimodal}，{readiness}",
                    IsFullyConfigured = definition.IsFullyConfigured,
                    SupportsMultimodalEmbedding = definition.SupportsMultimodalEmbedding
                };
            }
        }
    }
}
