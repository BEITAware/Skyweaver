using System;
using Skyweaver.Models.Multimodal;

namespace Skyweaver.Services.Multimodal
{
    /// <summary>
    /// 多模态配置运行时，管理内存中的多模态配置并协调持久化。
    /// </summary>
    public sealed class MultimodalRuntime
    {
        private readonly object _syncRoot = new();
        private readonly MultimodalConfigurationRepository _configurationRepository;
        private MultimodalConfiguration _configuration;

        private MultimodalRuntime()
        {
            _configurationRepository = new MultimodalConfigurationRepository();
            _configuration = CloneConfiguration(_configurationRepository.Load());
        }

        /// <summary>
        /// 获取运行时单例实例。
        /// </summary>
        public static MultimodalRuntime Instance { get; } = new();

        /// <summary>
        /// 配置文件的绝对路径。
        /// </summary>
        public string ConfigurationFilePath => _configurationRepository.ConfigurationFilePath;

        /// <summary>
        /// 是否启用文档字符识别。
        /// </summary>
        public bool EnableDocumentCharacterRecognition
        {
            get
            {
                lock (_syncRoot)
                {
                    return _configuration.EnableDocumentCharacterRecognition;
                }
            }
        }

        /// <summary>
        /// 硬件计算方案（CPU 或 GPU）。
        /// </summary>
        public string HardwareSolution
        {
            get
            {
                lock (_syncRoot)
                {
                    return _configuration.HardwareSolution;
                }
            }
        }

        /// <summary>
        /// 配置发生变化时触发的事件。
        /// </summary>
        public event EventHandler? ConfigurationChanged;

        /// <summary>
        /// 获取当前配置的深拷贝副本。
        /// </summary>
        public MultimodalConfiguration GetConfiguration()
        {
            lock (_syncRoot)
            {
                return CloneConfiguration(_configuration);
            }
        }

        /// <summary>
        /// 保存并应用新的配置。
        /// </summary>
        public void SaveConfiguration(MultimodalConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(configuration);

            lock (_syncRoot)
            {
                _configuration = CloneConfiguration(configuration);
                _configurationRepository.Save(_configuration);
            }

            ConfigurationChanged?.Invoke(this, EventArgs.Empty);
        }

        private static MultimodalConfiguration CloneConfiguration(MultimodalConfiguration configuration)
        {
            return new MultimodalConfiguration
            {
                EnableDocumentCharacterRecognition = configuration.EnableDocumentCharacterRecognition,
                HardwareSolution = configuration.HardwareSolution
            };
        }
    }
}
