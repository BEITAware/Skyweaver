using System;
using Ferrita.Infrastructure.Mvvm;

namespace Ferrita.Models.Multimodal
{
    public sealed class MultimodalConfiguration : ObservableObject
    {
        private bool _enableOcr;
        private bool _enableLongImageAutoParse = true;

        public bool EnableOcr
        {
            get => _enableOcr;
            set => SetProperty(ref _enableOcr, value);
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
