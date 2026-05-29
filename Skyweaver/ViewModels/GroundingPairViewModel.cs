using System;
using Skyweaver.Infrastructure.Mvvm;

namespace Skyweaver.ViewModels
{
    /// <summary>
    /// 表示事实严重程度的枚举（绿、黄、红三种配色）
    /// </summary>
    public enum FactSeverity
    {
        /// <summary>
        /// 绿色 - 证实/可信 (Verified)
        /// </summary>
        Verified,

        /// <summary>
        /// 黄色 - 未证实/可疑 (Unverified)
        /// </summary>
        Unverified,

        /// <summary>
        /// 红色 - 冲突/不实 (Contradicted)
        /// </summary>
        Contradicted
    }

    /// <summary>
    /// 表示单个左侧聊天消息块与右侧事实块配对的 ViewModel
    /// </summary>
    public class GroundingPairViewModel : ObservableObject
    {
        private string _chatMessage = string.Empty;
        private string _factMessage = string.Empty;
        private FactSeverity _severity = FactSeverity.Verified;
        private double _confidence = 1.0;
        private string _source = string.Empty;
        private string _timestamp = string.Empty;

        /// <summary>
        /// 左侧聊天消息文本
        /// </summary>
        public string ChatMessage
        {
            get => _chatMessage;
            set => SetProperty(ref _chatMessage, value);
        }

        /// <summary>
        /// 右侧事实校验文本
        /// </summary>
        public string FactMessage
        {
            get => _factMessage;
            set => SetProperty(ref _factMessage, value);
        }

        /// <summary>
        /// 事实校验的严重程度，对应红绿黄三种配色
        /// </summary>
        public FactSeverity Severity
        {
            get => _severity;
            set => SetProperty(ref _severity, value);
        }

        /// <summary>
        /// 校验的置信度 (0.00 到 1.00)
        /// </summary>
        public double Confidence
        {
            get => _confidence;
            set => SetProperty(ref _confidence, value);
        }

        /// <summary>
        /// 事实来源或知识库参考文档标识
        /// </summary>
        public string Source
        {
            get => _source;
            set => SetProperty(ref _source, value);
        }

        /// <summary>
        /// 时间戳
        /// </summary>
        public string Timestamp
        {
            get => _timestamp;
            set => SetProperty(ref _timestamp, value);
        }
    }
}
