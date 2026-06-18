using Ferrita.Controls.AgentConfigurationControl.Models;
using Ferrita.Services.FerritaTools;

namespace Ferrita.Controls.AgentConfigurationControl.Services
{
    internal static class AgentToolPermissionEvaluator
    {
        public static AgentToolEffectiveDecision Resolve(
            AgentDefinition agent,
            FerritaToolRegistration registration)
        {
            ArgumentNullException.ThrowIfNull(agent);
            ArgumentNullException.ThrowIfNull(registration);

            if (registration.Definition.IsSystemTool)
            {
                return AgentToolEffectiveDecision.Allowed;
            }

            if (!registration.IsEnabled)
            {
                return AgentToolEffectiveDecision.Denied;
            }

            var permission = agent.ToolPermissions.FirstOrDefault(item =>
                string.Equals(item.ToolName, registration.Definition.Name, StringComparison.OrdinalIgnoreCase));

            var mode = permission?.Permission ?? MapDefaultPermission(registration.Definition.DefaultAgentPermission);
            return mode switch
            {
                AgentToolPermissionMode.Disabled => AgentToolEffectiveDecision.Denied,
                AgentToolPermissionMode.RequireConfirmation => AgentToolEffectiveDecision.RequiresUserConfirmation,
                _ => AgentToolEffectiveDecision.Allowed
            };
        }

        private static AgentToolPermissionMode MapDefaultPermission(FerritaToolDefaultAgentPermission defaultPermission)
        {
            return defaultPermission switch
            {
                FerritaToolDefaultAgentPermission.Disabled => AgentToolPermissionMode.Disabled,
                FerritaToolDefaultAgentPermission.Allow => AgentToolPermissionMode.Allow,
                _ => AgentToolPermissionMode.RequireConfirmation
            };
        }
    }
}
