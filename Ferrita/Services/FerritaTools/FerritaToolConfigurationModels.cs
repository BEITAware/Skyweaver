using System.Windows;
using System.Xml.Linq;

namespace Ferrita.Services.FerritaTools
{
    public sealed class FerritaToolConfigurationState
    {
        private readonly XElement? _payload;

        public FerritaToolConfigurationState(string toolName, XElement? payload)
        {
            ToolName = (toolName ?? string.Empty).Trim();
            _payload = payload == null ? null : new XElement(payload);
        }

        public string ToolName { get; }

        public bool HasPayload => _payload != null;

        public XElement? GetPayload()
        {
            return _payload == null ? null : new XElement(_payload);
        }

        public static FerritaToolConfigurationState Empty(string toolName)
        {
            return new FerritaToolConfigurationState(toolName, payload: null);
        }
    }

    public sealed class FerritaToolPersistedState
    {
        private readonly XElement? _configuration;

        public FerritaToolPersistedState(string toolName, bool isEnabled, XElement? configuration)
        {
            ToolName = (toolName ?? string.Empty).Trim();
            if (ToolName.Length == 0)
            {
                throw new ArgumentException("Tool name cannot be empty.", nameof(toolName));
            }

            IsEnabled = isEnabled;
            _configuration = configuration == null ? null : new XElement(configuration);
        }

        public string ToolName { get; }

        public bool IsEnabled { get; }

        public XElement? GetConfiguration()
        {
            return _configuration == null ? null : new XElement(_configuration);
        }

        public FerritaToolConfigurationState ToConfigurationState()
        {
            return new FerritaToolConfigurationState(ToolName, _configuration);
        }

        public FerritaToolPersistedState WithIsEnabled(bool isEnabled)
        {
            return new FerritaToolPersistedState(ToolName, isEnabled, _configuration);
        }

        public FerritaToolPersistedState WithConfiguration(XElement? configuration)
        {
            return new FerritaToolPersistedState(ToolName, IsEnabled, configuration);
        }
    }

    public sealed class FerritaToolConfigurationEditorContext
    {
        public FerritaToolConfigurationEditorContext(
            FerritaToolDefinition baseDefinition,
            FerritaToolDefinition effectiveDefinition,
            FerritaToolConfigurationState initialConfiguration)
        {
            BaseDefinition = baseDefinition ?? throw new ArgumentNullException(nameof(baseDefinition));
            EffectiveDefinition = effectiveDefinition ?? throw new ArgumentNullException(nameof(effectiveDefinition));
            InitialConfiguration = initialConfiguration ?? throw new ArgumentNullException(nameof(initialConfiguration));
        }

        public string ToolName => BaseDefinition.Name;

        public FerritaToolDefinition BaseDefinition { get; }

        public FerritaToolDefinition EffectiveDefinition { get; }

        public FerritaToolConfigurationState InitialConfiguration { get; }
    }

    public abstract class FerritaToolConfigurationPresenter : IDisposable
    {
        public abstract FrameworkElement View { get; }

        public event EventHandler? ConfigurationChanged;

        protected void RaiseConfigurationChanged()
        {
            ConfigurationChanged?.Invoke(this, EventArgs.Empty);
        }

        public abstract bool TryCaptureConfiguration(out XElement? configuration, out string? errorMessage);

        public virtual void Dispose()
        {
        }
    }

    public interface IFerritaToolConfigurationProvider
    {
        FerritaToolDefinition GetEffectiveDefinition(FerritaToolConfigurationState configuration);

        FerritaToolConfigurationPresenter? CreateConfigurationPresenter(FerritaToolConfigurationEditorContext context);
    }
}
