using Skyweaver.Controls.LanguageModelConfigurationControl.Models;

namespace Skyweaver.Controls.LanguageModelConfigurationControl.Services
{
    public interface ILanguageModelChatService
    {
        Task<LanguageModelChatResponse> GetResponseAsync(
            LanguageModelDefinition model,
            IReadOnlyList<LanguageModelChatMessage> messages,
            CancellationToken cancellationToken = default,
            Func<LanguageModelMediaProcessingProgress, CancellationToken, ValueTask>? mediaProcessingProgress = null);

        Task<int> CountTokensAsync(
            LanguageModelDefinition model,
            IReadOnlyList<LanguageModelChatMessage> messages,
            CancellationToken cancellationToken = default,
            Func<LanguageModelMediaProcessingProgress, CancellationToken, ValueTask>? mediaProcessingProgress = null);

        IAsyncEnumerable<LanguageModelStreamingChatUpdate> GetStreamingResponseAsync(
            LanguageModelDefinition model,
            IReadOnlyList<LanguageModelChatMessage> messages,
            CancellationToken cancellationToken = default,
            Func<LanguageModelMediaProcessingProgress, CancellationToken, ValueTask>? mediaProcessingProgress = null);
    }
}
