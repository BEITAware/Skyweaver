using System.Windows;
using System.Xml.Linq;

namespace Skyweaver.Services.SkyweaverTools
{
    public sealed class SkyweaverToolConfigurationState
    {
        private readonly XElement? _payload;

        public SkyweaverToolConfigurationState(string toolName, XElement? payload)
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

        public static SkyweaverToolConfigurationState Empty(string toolName)
        {
            return new SkyweaverToolConfigurationState(toolName, payload: null);
        }
    }

    public sealed class SkyweaverToolPersistedState
    {
        private readonly XElement? _configuration;

        public SkyweaverToolPersistedState(string toolName, bool isEnabled, XElement? configuration)
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

        public SkyweaverToolConfigurationState ToConfigurationState()
        {
            return new SkyweaverToolConfigurationState(ToolName, _configuration);
        }

        public SkyweaverToolPersistedState WithIsEnabled(bool isEnabled)
        {
            return new SkyweaverToolPersistedState(ToolName, isEnabled, _configuration);
        }

        public SkyweaverToolPersistedState WithConfiguration(XElement? configuration)
        {
            return new SkyweaverToolPersistedState(ToolName, IsEnabled, configuration);
        }
    }

    public sealed class SkyweaverToolConfigurationEditorContext
    {
        public SkyweaverToolConfigurationEditorContext(
            SkyweaverToolDefinition baseDefinition,
            SkyweaverToolDefinition effectiveDefinition,
            SkyweaverToolConfigurationState initialConfiguration)
        {
            BaseDefinition = baseDefinition ?? throw new ArgumentNullException(nameof(baseDefinition));
            EffectiveDefinition = effectiveDefinition ?? throw new ArgumentNullException(nameof(effectiveDefinition));
            InitialConfiguration = initialConfiguration ?? throw new ArgumentNullException(nameof(initialConfiguration));
        }

        public string ToolName => BaseDefinition.Name;

        public SkyweaverToolDefinition BaseDefinition { get; }

        public SkyweaverToolDefinition EffectiveDefinition { get; }

        public SkyweaverToolConfigurationState InitialConfiguration { get; }
    }

    public abstract class SkyweaverToolConfigurationPresenter : IDisposable
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

    public interface ISkyweaverToolConfigurationProvider
    {
        SkyweaverToolDefinition GetEffectiveDefinition(SkyweaverToolConfigurationState configuration);

        SkyweaverToolConfigurationPresenter? CreateConfigurationPresenter(SkyweaverToolConfigurationEditorContext context);
    }
}
