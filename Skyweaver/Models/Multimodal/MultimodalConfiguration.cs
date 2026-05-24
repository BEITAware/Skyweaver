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
        private bool _enableLongImageAutoParse = true;

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

        /// <summary>
        /// 启用长图像自动解析。当图像宽高比超过 21:9 或 9:21 时，
        /// 自动将其沿长边均匀切分为多张比例在 16:9 到 9:16 之间的子图，再投影给 LLM。
        /// </summary>
        public bool EnableLongImageAutoParse
        {
            get => _enableLongImageAutoParse;
            set => SetProperty(ref _enableLongImageAutoParse, value);
        }
    }
}
