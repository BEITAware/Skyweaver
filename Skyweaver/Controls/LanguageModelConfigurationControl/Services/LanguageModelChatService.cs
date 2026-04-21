using Skyweaver.Controls.LanguageModelConfigurationControl.Models;

namespace Skyweaver.Controls.LanguageModelConfigurationControl.Services
{
    public sealed class LanguageModelChatService : ILanguageModelChatService
    {
        public IReadOnlyList<string> AvailableInterfaceTypes => LanguageModelInterfaceCatalog.AvailableInterfaceTypes;

        public LanguageModelInterfaceSettings CreateInterfaceSettings(string? interfaceType)
        {
            return LanguageModelInterfaceCatalog.CreateInterfaceSettings(interfaceType);
        }

        public void Validate(LanguageModelDefinition model)
        {
            ResolveAdapter(model).Validate(model);
        }

        public Task<LanguageModelChatResponse> GetResponseAsync(
            LanguageModelDefinition model,
            IReadOnlyList<LanguageModelChatMessage> messages,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(model);
            ArgumentNullException.ThrowIfNull(messages);

            return ResolveAdapter(model).GetResponseAsync(model, messages, cancellationToken);
        }

        public IAsyncEnumerable<LanguageModelStreamingChatUpdate> GetStreamingResponseAsync(
            LanguageModelDefinition model,
            IReadOnlyList<LanguageModelChatMessage> messages,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(model);
            ArgumentNullException.ThrowIfNull(messages);

            return ResolveAdapter(model).GetStreamingResponseAsync(model, messages, cancellationToken);
        }

        private static ILanguageModelInterfaceAdapter ResolveAdapter(LanguageModelDefinition model)
        {
            ArgumentNullException.ThrowIfNull(model);
            return LanguageModelInterfaceCatalog.CreateAdapter(model.InterfaceType);
        }
    }
}
