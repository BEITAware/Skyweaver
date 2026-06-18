using Ferrita.Models.ContextManagement;

namespace Ferrita.Services.ContextManagement
{
    public sealed class ContextManagementRuntime
    {
        private readonly object _syncRoot = new();
        private readonly ContextManagementConfigurationRepository _configurationRepository;
        private ContextManagementConfiguration _configuration;

        private ContextManagementRuntime()
        {
            var pathProvider = new ContextManagementPathProvider();
            _configurationRepository = new ContextManagementConfigurationRepository(pathProvider);
            _configuration = CloneConfiguration(_configurationRepository.Load());
        }

        public static ContextManagementRuntime Instance { get; } = new();

        public string ConfigurationFilePath => _configurationRepository.ConfigurationFilePath;

        public bool MinCompactionEnabled
        {
            get
            {
                lock (_syncRoot)
                {
                    return _configuration.MinCompactionEnabled;
                }
            }
        }

        public bool MaxCompactionEnabled
        {
            get
            {
                lock (_syncRoot)
                {
                    return _configuration.MaxCompactionEnabled;
                }
            }
        }

        public event EventHandler? ConfigurationChanged;

        public ContextManagementConfiguration GetConfiguration()
        {
            lock (_syncRoot)
            {
                return CloneConfiguration(_configuration);
            }
        }

        public void SaveConfiguration(ContextManagementConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(configuration);

            lock (_syncRoot)
            {
                _configuration = CloneConfiguration(configuration);
                _configurationRepository.Save(_configuration);
            }

            ConfigurationChanged?.Invoke(this, EventArgs.Empty);
        }

        private static ContextManagementConfiguration CloneConfiguration(ContextManagementConfiguration configuration)
        {
            return new ContextManagementConfiguration
            {
                MinCompactionEnabled = configuration.MinCompactionEnabled,
                MaxCompactionEnabled = configuration.MaxCompactionEnabled,
                LifeCycleEnabled = configuration.LifeCycleEnabled,
                LifeCycleRatioPercent = configuration.LifeCycleRatioPercent,
                RnnOptimizedCompactionEnabled = configuration.RnnOptimizedCompactionEnabled,
                MemoryEnabled = configuration.MemoryEnabled,
                MemoryShareScope = configuration.MemoryShareScope,
                MemoryRetrievalCount = configuration.MemoryRetrievalCount
            };
        }
    }
}
