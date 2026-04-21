using Skyweaver.Controls.LanguageModelConfigurationControl.Models;

namespace Skyweaver.Controls.LanguageModelConfigurationControl.Services
{
    public static class CapabilityLayerBuiltIns
    {
        public const string ContextCompressionLayerKey = "builtin.context-compression";
        public const string ContextCompressionLayerName = "上下文压缩";

        public static CapabilityLayerDefinition CreateContextCompressionLayer()
        {
            return new CapabilityLayerDefinition
            {
                Key = ContextCompressionLayerKey,
                Name = ContextCompressionLayerName,
                IsBuiltIn = true
            };
        }

        public static bool ApplyBuiltInMetadata(CapabilityLayerDefinition definition)
        {
            ArgumentNullException.ThrowIfNull(definition);

            if (!string.Equals(definition.Key, ContextCompressionLayerKey, StringComparison.OrdinalIgnoreCase))
            {
                definition.IsBuiltIn = false;
                return false;
            }

            var wasChanged = false;
            if (!definition.IsBuiltIn)
            {
                definition.IsBuiltIn = true;
                wasChanged = true;
            }

            if (!string.Equals(definition.Name, ContextCompressionLayerName, StringComparison.Ordinal))
            {
                definition.Name = ContextCompressionLayerName;
                wasChanged = true;
            }

            return wasChanged;
        }

        public static bool EnsureBuiltInLayers(ICollection<CapabilityLayerDefinition> definitions)
        {
            ArgumentNullException.ThrowIfNull(definitions);

            var wasChanged = false;

            foreach (var definition in definitions)
            {
                wasChanged |= ApplyBuiltInMetadata(definition);
            }

            if (definitions.All(definition =>
                    !string.Equals(definition.Key, ContextCompressionLayerKey, StringComparison.OrdinalIgnoreCase)))
            {
                definitions.Add(CreateContextCompressionLayer());
                wasChanged = true;
            }

            return wasChanged;
        }
    }
}
