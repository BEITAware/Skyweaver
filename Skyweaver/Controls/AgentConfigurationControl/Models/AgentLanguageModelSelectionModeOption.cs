namespace Skyweaver.Controls.AgentConfigurationControl.Models
{
    public sealed class AgentLanguageModelSelectionModeOption
    {
        public AgentLanguageModelSelectionModeOption(AgentLanguageModelSelectionMode mode, string displayName)
        {
            Mode = mode;
            DisplayName = displayName;
        }

        public AgentLanguageModelSelectionMode Mode { get; }

        public string DisplayName { get; }
    }
}
