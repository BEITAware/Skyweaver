namespace Skyweaver.Controls.AgentConfigurationControl.Models
{
    public sealed class AgentConfigurationReferenceOption
    {
        public AgentConfigurationReferenceOption(string key, string displayName)
        {
            Key = key ?? string.Empty;
            DisplayName = displayName ?? string.Empty;
        }

        public string Key { get; }

        public string DisplayName { get; }
    }
}
