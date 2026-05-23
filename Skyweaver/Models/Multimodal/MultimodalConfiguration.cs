using Skyweaver.Infrastructure.Mvvm;

namespace Skyweaver.Models.Multimodal
{
    public sealed class MultimodalConfiguration : ObservableObject
    {
        private bool _enableDocumentCharacterRecognition = false;
        private string _hardwareSolution = "CPU"; // "CPU" or "GPU"

        public bool EnableDocumentCharacterRecognition
        {
            get => _enableDocumentCharacterRecognition;
            set => SetProperty(ref _enableDocumentCharacterRecognition, value);
        }

        public string HardwareSolution
        {
            get => _hardwareSolution;
            set => SetProperty(ref _hardwareSolution, value?.Trim() ?? "CPU");
        }
    }
}
