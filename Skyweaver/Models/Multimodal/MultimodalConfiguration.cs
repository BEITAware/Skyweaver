using System;
using Skyweaver.Infrastructure.Mvvm;

namespace Skyweaver.Models.Multimodal
{
    public enum OcrHardwareOption
    {
        CPU,
        GPU
    }

    public sealed class MultimodalConfiguration : ObservableObject
    {
        private bool _enableOcr;
        private OcrHardwareOption _hardwareOption = OcrHardwareOption.CPU;

        public bool EnableOcr
        {
            get => _enableOcr;
            set => SetProperty(ref _enableOcr, value);
        }

        public OcrHardwareOption HardwareOption
        {
            get => _hardwareOption;
            set => SetProperty(ref _hardwareOption, value);
        }
    }
}
