using Skyweaver.Controls.LanguageModelConfigurationControl.Models;

namespace Skyweaver.Controls.LanguageModelConfigurationControl.Services
{
    internal interface ILanguageModelInterfaceAdapter
    {
        string InterfaceType { get; }

        LanguageModelInterfaceSettings CreateInterfaceSettings();

        void Validate(LanguageModelDefinition model);

        Task<LanguageModelChatResponse> GetResponseAsync(
            LanguageModelDefinition model,
            IReadOnlyList<LanguageModelChatMessage> messages,
            CancellationToken cancellationToken = default);

        IAsyncEnumerable<LanguageModelStreamingChatUpdate> GetStreamingResponseAsync(
            LanguageModelDefinition model,
            IReadOnlyList<LanguageModelChatMessage> messages,
            CancellationToken cancellationToken = default);
    }

    public static class LanguageModelInterfaceCatalog
    {
        private static readonly IReadOnlyDictionary<string, Func<ILanguageModelInterfaceAdapter>> s_adapterFactories =
            new Dictionary<string, Func<ILanguageModelInterfaceAdapter>>(StringComparer.OrdinalIgnoreCase)
            {
                ["MEAI"] = static () => new MeaiLanguageModelInterfaceAdapter()
            };

        public static IReadOnlyList<string> AvailableInterfaceTypes { get; } = s_adapterFactories.Keys
            .OrderBy(static key => key, StringComparer.Ordinal)
            .ToArray();

        public static LanguageModelInterfaceSettings CreateInterfaceSettings(string? interfaceType)
        {
            return CreateAdapter(interfaceType).CreateInterfaceSettings();
        }

        internal static ILanguageModelInterfaceAdapter CreateAdapter(string? interfaceType)
        {
            var normalizedType = (interfaceType ?? string.Empty).Trim();
            if (s_adapterFactories.TryGetValue(normalizedType, out var factory))
            {
                return factory();
            }

            return s_adapterFactories["MEAI"]();
        }
    }
}
