using Ferrita.Models.ShellIntegration;

namespace Ferrita.Services.ShellIntegration
{
    public sealed class ShellIntegrationRuntime
    {
        private readonly object _syncRoot = new();
        private readonly ShellIntegrationConfigurationRepository _configurationRepository;
        private readonly ShellIntegrationRegistrar _registrar;
        private ShellIntegrationConfiguration _configuration;

        private ShellIntegrationRuntime()
        {
            var pathProvider = new ShellIntegrationPathProvider();
            _configurationRepository = new ShellIntegrationConfigurationRepository(pathProvider);
            _registrar = new ShellIntegrationRegistrar();
            _configuration = CloneConfiguration(_configurationRepository.Load());
        }

        public static ShellIntegrationRuntime Instance { get; } = new();

        public string ConfigurationFilePath => _configurationRepository.ConfigurationFilePath;

        public event EventHandler? ConfigurationChanged;

        public ShellIntegrationConfiguration GetConfiguration()
        {
            lock (_syncRoot)
            {
                return CloneConfiguration(_configuration);
            }
        }

        public void SaveConfiguration(ShellIntegrationConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(configuration);

            lock (_syncRoot)
            {
                _configuration = CloneConfiguration(configuration);
                _configurationRepository.Save(_configuration);
            }

            ConfigurationChanged?.Invoke(this, EventArgs.Empty);
        }

        public ShellIntegrationApplyResult ApplyConfiguredRegistration()
        {
            var shouldRegister = GetConfiguration().IsEnabled;

            try
            {
                if (shouldRegister)
                {
                    _registrar.Register();
                }
                else
                {
                    _registrar.Unregister();
                }

                return ShellIntegrationApplyResult.Success(_registrar.IsRegistered());
            }
            catch (Exception ex)
            {
                return ShellIntegrationApplyResult.Failure(ex.Message, IsRegistered());
            }
        }

        public bool IsRegistered()
        {
            try
            {
                return _registrar.IsRegistered();
            }
            catch
            {
                return false;
            }
        }

        private static ShellIntegrationConfiguration CloneConfiguration(ShellIntegrationConfiguration configuration)
        {
            return new ShellIntegrationConfiguration
            {
                IsEnabled = configuration.IsEnabled,
                SessionFlowGraphId = configuration.SessionFlowGraphId,
                SessionFlowGraphName = configuration.SessionFlowGraphName,
                SessionFlowFilePath = configuration.SessionFlowFilePath
            };
        }
    }
}
