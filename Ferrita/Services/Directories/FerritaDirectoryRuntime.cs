using System.IO;
using Ferrita.Models.Directories;

namespace Ferrita.Services.Directories
{
    public sealed class FerritaDirectoryRuntime
    {
        private readonly object _syncRoot = new();
        private readonly DirectoriesConfigurationRepository _configurationRepository;
        private DirectoriesConfiguration _configuration;

        private FerritaDirectoryRuntime()
        {
            _configurationRepository = new DirectoriesConfigurationRepository();
            _configuration = FerritaDirectoryDefaults.NormalizeConfiguration(_configurationRepository.Load());
        }

        public static FerritaDirectoryRuntime Instance { get; } = new();

        public string DirectoriesConfigurationFilePath => _configurationRepository.ConfigurationFilePath;

        public string FixedConfigurationDirectoryPath => _configurationRepository.ConfigurationDirectoryPath;

        public string DefaultApplicationDirectoryPath => FerritaDirectoryDefaults.ApplicationDirectoryPath;

        public string ChatSessionsDirectoryPath => GetDirectoryPath(
            configuration => configuration.ChatSessionsDirectoryPath,
            FerritaDirectoryDefaults.DefaultChatSessionsDirectoryPath);

        public string ConfigurationDirectoryPath => GetDirectoryPath(
            configuration => configuration.ConfigurationDirectoryPath,
            FerritaDirectoryDefaults.DefaultConfigurationDirectoryPath);

        public string DebugDirectoryPath => GetDirectoryPath(
            configuration => configuration.DebugDirectoryPath,
            FerritaDirectoryDefaults.DefaultDebugDirectoryPath);

        public string SessionFlowsDirectoryPath => GetDirectoryPath(
            configuration => configuration.SessionFlowsDirectoryPath,
            FerritaDirectoryDefaults.DefaultSessionFlowsDirectoryPath);

        public string AerialCityDirectoryPath => GetDirectoryPath(
            configuration => configuration.AerialCityDirectoryPath,
            FerritaDirectoryDefaults.DefaultAerialCityDirectoryPath);

        public string KnowledgeDirectoryPath => GetDirectoryPath(
            configuration => configuration.KnowledgeDirectoryPath,
            FerritaDirectoryDefaults.DefaultKnowledgeDirectoryPath);

        public event EventHandler? ConfigurationChanged;

        public DirectoriesConfiguration GetConfiguration()
        {
            lock (_syncRoot)
            {
                return CloneConfiguration(_configuration);
            }
        }

        public void SaveConfiguration(DirectoriesConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(configuration);

            lock (_syncRoot)
            {
                var normalizedConfiguration = FerritaDirectoryDefaults.NormalizeConfiguration(configuration);
                EnsureConfiguredDirectories(normalizedConfiguration);

                _configuration = CloneConfiguration(normalizedConfiguration);
                _configurationRepository.Save(_configuration);
            }

            ConfigurationChanged?.Invoke(this, EventArgs.Empty);
        }

        private string GetDirectoryPath(Func<DirectoriesConfiguration, string> selector, string fallbackDirectoryPath)
        {
            lock (_syncRoot)
            {
                return FerritaDirectoryDefaults.NormalizeDirectoryPath(selector(_configuration), fallbackDirectoryPath);
            }
        }

        private static DirectoriesConfiguration CloneConfiguration(DirectoriesConfiguration configuration)
        {
            return new DirectoriesConfiguration
            {
                ChatSessionsDirectoryPath = configuration.ChatSessionsDirectoryPath,
                ConfigurationDirectoryPath = configuration.ConfigurationDirectoryPath,
                DebugDirectoryPath = configuration.DebugDirectoryPath,
                SessionFlowsDirectoryPath = configuration.SessionFlowsDirectoryPath,
                AerialCityDirectoryPath = configuration.AerialCityDirectoryPath,
                KnowledgeDirectoryPath = configuration.KnowledgeDirectoryPath
            };
        }

        private static void EnsureConfiguredDirectories(DirectoriesConfiguration configuration)
        {
            Directory.CreateDirectory(configuration.ChatSessionsDirectoryPath);
            Directory.CreateDirectory(configuration.ConfigurationDirectoryPath);
            Directory.CreateDirectory(configuration.DebugDirectoryPath);
            Directory.CreateDirectory(configuration.SessionFlowsDirectoryPath);
            Directory.CreateDirectory(configuration.AerialCityDirectoryPath);
            Directory.CreateDirectory(configuration.KnowledgeDirectoryPath);
        }
    }
}
