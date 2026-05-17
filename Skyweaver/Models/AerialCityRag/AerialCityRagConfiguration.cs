using Skyweaver.Infrastructure.Mvvm;

namespace Skyweaver.Models.AerialCityRag
{
    public sealed class AerialCityRagConfiguration : ObservableObject
    {
        public const int MinimumEmbeddingConcurrency = 1;
        public const int MaximumEmbeddingConcurrency = 200;

        private bool _isEnabled;
        private string _selectedEmbeddingModelKey = string.Empty;
        private int _embeddingConcurrency = MinimumEmbeddingConcurrency;

        public bool IsEnabled
        {
            get => _isEnabled;
            set => SetProperty(ref _isEnabled, value);
        }

        public string SelectedEmbeddingModelKey
        {
            get => _selectedEmbeddingModelKey;
            set => SetProperty(ref _selectedEmbeddingModelKey, value?.Trim() ?? string.Empty);
        }

        public int EmbeddingConcurrency
        {
            get => _embeddingConcurrency;
            set => SetProperty(
                ref _embeddingConcurrency,
                Math.Clamp(value, MinimumEmbeddingConcurrency, MaximumEmbeddingConcurrency));
        }
    }
}
