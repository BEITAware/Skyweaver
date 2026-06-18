using System;
using Ferrita.Models.Multimodal;

namespace Ferrita.Services.Multimodal
{
    public sealed class MultimodalRuntime
    {
        private readonly object _syncRoot = new();
        private readonly MultimodalConfigurationRepository _configurationRepository;
        private MultimodalConfiguration _configuration;

        private MultimodalRuntime()
        {
            var pathProvider = new MultimodalPathProvider();
            _configurationRepository = new MultimodalConfigurationRepository(pathProvider);
            _configuration = CloneConfiguration(_configurationRepository.Load());
        }

        public static MultimodalRuntime Instance { get; } = new();

        public string ConfigurationFilePath => _configurationRepository.ConfigurationFilePath;

        public event EventHandler? ConfigurationChanged;

        public MultimodalConfiguration GetConfiguration()
        {
            lock (_syncRoot)
            {
                return CloneConfiguration(_configuration);
            }
        }

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
                EnableOcr = configuration.EnableOcr,
                EnableLongImageAutoParse = configuration.EnableLongImageAutoParse
            };
        }
    }
}
