using System;
using Skyweaver.Models.ContextManagement;

namespace Skyweaver.Services.ContextManagement
{
    public sealed class ContextArrangementRuntime
    {
        private readonly object _syncRoot = new();
        private readonly ContextArrangementConfigurationRepository _configurationRepository;
        private ContextArrangementConfiguration _configuration;

        private ContextArrangementRuntime()
        {
            var pathProvider = new ContextArrangementPathProvider();
            _configurationRepository = new ContextArrangementConfigurationRepository(pathProvider);
            _configuration = CloneConfiguration(_configurationRepository.Load());
        }

        public static ContextArrangementRuntime Instance { get; } = new();

        public string ConfigurationFilePath => _configurationRepository.ConfigurationFilePath;

        public event EventHandler? ConfigurationChanged;

        public ContextArrangementConfiguration GetConfiguration()
        {
            lock (_syncRoot)
            {
                return CloneConfiguration(_configuration);
            }
        }

        public void SaveConfiguration(ContextArrangementConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(configuration);

            lock (_syncRoot)
            {
                _configuration = CloneConfiguration(configuration);
                _configurationRepository.Save(_configuration);
            }

            ConfigurationChanged?.Invoke(this, EventArgs.Empty);
        }

        private static ContextArrangementConfiguration CloneConfiguration(ContextArrangementConfiguration configuration)
        {
            return new ContextArrangementConfiguration
            {
                OptimizeToolCallPrompt = configuration.OptimizeToolCallPrompt,
                ToolCallIdTable = configuration.ToolCallIdTable
            };
        }
    }
}
