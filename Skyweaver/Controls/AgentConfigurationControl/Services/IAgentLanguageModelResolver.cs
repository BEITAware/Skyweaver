using Skyweaver.Controls.AgentConfigurationControl.Models;
using Skyweaver.Controls.LanguageModelConfigurationControl.Models;

namespace Skyweaver.Controls.AgentConfigurationControl.Services
{
    public interface IAgentLanguageModelResolver
    {
        IReadOnlyList<LanguageModelDefinition> GetCandidateModels(AgentDefinition agent);

        IReadOnlyList<LanguageModelDefinition> GetCandidateModelsForCapabilityLayer(string capabilityLayerKey);

        Task<T> ExecuteWithFallbackAsync<T>(
            AgentDefinition agent,
            Func<LanguageModelDefinition, CancellationToken, Task<T>> operationAsync,
            CancellationToken cancellationToken = default);

        Task<T> ExecuteCapabilityLayerWithFallbackAsync<T>(
            string capabilityLayerKey,
            Func<LanguageModelDefinition, CancellationToken, Task<T>> operationAsync,
            CancellationToken cancellationToken = default);

        int GetMinimumContextWindowTokens(AgentDefinition agent);

        int GetCapabilityLayerMinimumContextWindowTokens(string capabilityLayerKey);
    }
}
