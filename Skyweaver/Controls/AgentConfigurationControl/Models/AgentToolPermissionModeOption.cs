namespace Skyweaver.Controls.AgentConfigurationControl.Models
{
    public sealed class AgentToolPermissionModeOption
    {
        public AgentToolPermissionModeOption(AgentToolPermissionMode mode, string displayName)
        {
            Mode = mode;
            DisplayName = displayName ?? mode.ToString();
        }

        public AgentToolPermissionMode Mode { get; }

        public string DisplayName { get; }
    }
}
