using System;
using System.Collections.Generic;
using System.Linq;
using Skyweaver.Controls.LanguageModelConfigurationControl.Models;

namespace Skyweaver.Controls.LanguageModelConfigurationControl.Services
{
    public static class CapabilityLayerBuiltIns
    {
        public const string ContextCompressionLayerKey = "builtin.context-compression";
        public const string Utility1FastLayerKey = "builtin.utility-i-fast";
        public const string Utility2SmartLayerKey = "builtin.utility-ii-smart";

        public static bool EnsureBuiltInLayers(ICollection<CapabilityLayerDefinition> definitions)
        {
            ArgumentNullException.ThrowIfNull(definitions);

            var wasChanged = false;

            // 1. 保持对 ContextCompressionLayerKey 的现有处理，确保在 XML 配置加载时清除 IsBuiltIn
            foreach (var definition in definitions)
            {
                if (definition.IsBuiltIn &&
                    string.Equals(definition.Key, ContextCompressionLayerKey, StringComparison.OrdinalIgnoreCase))
                {
                    definition.IsBuiltIn = false;
                    wasChanged = true;
                }
            }

            // 2. 确保 实用I（快速）层级存在并被标记为 BuiltIn
            var utility1 = definitions.FirstOrDefault(d => string.Equals(d.Key, Utility1FastLayerKey, StringComparison.OrdinalIgnoreCase));
            if (utility1 == null)
            {
                utility1 = new CapabilityLayerDefinition
                {
                    Key = Utility1FastLayerKey,
                    Name = "实用I（快速）",
                    IsBuiltIn = true
                };
                definitions.Add(utility1);
                wasChanged = true;
            }
            else
            {
                utility1.IsBuiltIn = true;
            }

            // 3. 确保 实用II（智能）层级存在并被标记为 BuiltIn
            var utility2 = definitions.FirstOrDefault(d => string.Equals(d.Key, Utility2SmartLayerKey, StringComparison.OrdinalIgnoreCase));
            if (utility2 == null)
            {
                utility2 = new CapabilityLayerDefinition
                {
                    Key = Utility2SmartLayerKey,
                    Name = "实用II（智能）",
                    IsBuiltIn = true
                };
                definitions.Add(utility2);
                wasChanged = true;
            }
            else
            {
                utility2.IsBuiltIn = true;
            }

            return wasChanged;
        }
    }
}
