using Skyweaver.Controls.LanguageModelConfigurationControl.Models;

namespace Skyweaver.Controls.LanguageModelConfigurationControl.Services
{
    public static class CapabilityLayerBuiltIns
    {
        public const string ContextCompressionLayerKey = "builtin.context-compression";

        public static bool EnsureBuiltInLayers(ICollection<CapabilityLayerDefinition> definitions)
        {
            ArgumentNullException.ThrowIfNull(definitions);

            var wasChanged = false;

            foreach (var definition in definitions)
            {
                if (definition.IsBuiltIn &&
                    string.Equals(definition.Key, ContextCompressionLayerKey, StringComparison.OrdinalIgnoreCase))
                {
                    definition.IsBuiltIn = false;
                    wasChanged = true;
                }
            }

            return wasChanged;
        }
    }
}
