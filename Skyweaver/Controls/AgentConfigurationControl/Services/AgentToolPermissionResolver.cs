using Skyweaver.Controls.AgentConfigurationControl.Models;
using Skyweaver.Services.SkyweaverTools;

namespace Skyweaver.Controls.AgentConfigurationControl.Services
{
    public sealed class AgentToolPermissionResolver
    {
        private readonly AgentConfigurationRepository _configurationRepository;
        private readonly SkyweaverToolManager _toolManager;

        public AgentToolPermissionResolver()
            : this(
                new AgentConfigurationRepository(new AgentConfigurationPathProvider()),
                new SkyweaverToolManager())
        {
        }

        public AgentToolPermissionResolver(
            AgentConfigurationRepository configurationRepository,
            SkyweaverToolManager toolManager)
        {
            _configurationRepository = configurationRepository;
            _toolManager = toolManager;
        }

        public AgentToolEffectiveDecision Resolve(string agentId, string toolName)
        {
            if (string.IsNullOrWhiteSpace(agentId))
            {
                throw new ArgumentException("代理 ID 不能为空。", nameof(agentId));
            }

            if (string.IsNullOrWhiteSpace(toolName))
            {
                throw new ArgumentException("工具名称不能为空。", nameof(toolName));
            }

            var agent = _configurationRepository.Load()
                .FirstOrDefault(item => string.Equals(item.AgentId, agentId.Trim(), StringComparison.OrdinalIgnoreCase));

            return agent == null
                ? AgentToolEffectiveDecision.Denied
                : Resolve(agent, toolName);
        }

        public AgentToolEffectiveDecision Resolve(AgentDefinition agent, string toolName)
        {
            ArgumentNullException.ThrowIfNull(agent);

            if (string.IsNullOrWhiteSpace(toolName))
            {
                throw new ArgumentException("工具名称不能为空。", nameof(toolName));
            }

            var toolRegistration = _toolManager.GetRegisteredTools(resolveIcons: false).FirstOrDefault(item =>
                string.Equals(item.Definition.Name, toolName.Trim(), StringComparison.OrdinalIgnoreCase));

            if (toolRegistration == null || !toolRegistration.IsEnabled)
            {
                return AgentToolEffectiveDecision.Denied;
            }

            return AgentToolPermissionEvaluator.Resolve(agent, toolRegistration);
        }
    }
}
