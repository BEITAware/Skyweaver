using Skyweaver.Controls.AgentConfigurationControl.Models;
using Skyweaver.Services.SkyweaverTools;

namespace Skyweaver.Controls.AgentConfigurationControl.Services
{
    internal static class AgentToolPermissionEvaluator
    {
        public static AgentToolEffectiveDecision Resolve(
            AgentDefinition agent,
            SkyweaverToolRegistration registration)
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

            var mode = permission?.Permission ?? AgentToolPermissionMode.RequireConfirmation;
            return mode switch
            {
                AgentToolPermissionMode.Disabled => AgentToolEffectiveDecision.Denied,
                AgentToolPermissionMode.RequireConfirmation => AgentToolEffectiveDecision.RequiresUserConfirmation,
                _ => AgentToolEffectiveDecision.Allowed
            };
        }
    }
}
