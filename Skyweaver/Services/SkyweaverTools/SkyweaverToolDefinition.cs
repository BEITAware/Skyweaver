using System.Text.RegularExpressions;

namespace Skyweaver.Services.SkyweaverTools
{
    public sealed class SkyweaverToolDefinition
    {
        private static readonly Regex s_namePattern = new("^[A-Za-z0-9_-]+$", RegexOptions.Compiled);

        public SkyweaverToolDefinition(
            string name,
            string description,
            string? iconName = null,
            IReadOnlyList<SkyweaverToolParameterDefinition>? parameters = null,
            bool isSystemTool = false,
            bool canBelongToToolKit = true,
            SkyweaverToolDefaultAgentPermission defaultAgentPermission = SkyweaverToolDefaultAgentPermission.RequireConfirmation,
            IReadOnlyList<string>? defaultToolKitKeys = null,
            bool supportsAsyncInvocation = true)
        {
            var normalizedName = (name ?? string.Empty).Trim();
            if (normalizedName.Length == 0)
            {
                throw new ArgumentException("Tool name cannot be empty.", nameof(name));
            }

            if (!s_namePattern.IsMatch(normalizedName))
            {
                throw new ArgumentException("Tool names may only contain letters, numbers, '_' or '-'.", nameof(name));
            }

            var normalizedParameters = parameters?.ToArray() ?? Array.Empty<SkyweaverToolParameterDefinition>();
            var normalizedToolKitKeys = NormalizeToolKitKeys(defaultToolKitKeys);
            var parameterNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var parameter in normalizedParameters)
            {
                if (!parameterNames.Add(parameter.Name))
                {
                    throw new ArgumentException($"Tool '{normalizedName}' contains a duplicated parameter: {parameter.Name}", nameof(parameters));
                }
            }

            if ((!canBelongToToolKit || isSystemTool) && normalizedToolKitKeys.Count > 0)
            {
                throw new ArgumentException(
                    $"Tool '{normalizedName}' cannot declare default toolkit membership because toolkit membership is disabled for this tool.",
                    nameof(defaultToolKitKeys));
            }

            Name = normalizedName;
            Description = (description ?? string.Empty).Trim();
            IconName = string.IsNullOrWhiteSpace(iconName) ? null : iconName.Trim();
            Parameters = normalizedParameters;
            IsSystemTool = isSystemTool;
            CanBelongToToolKit = !isSystemTool && canBelongToToolKit;
            DefaultAgentPermission = defaultAgentPermission;
            DefaultToolKitKeys = normalizedToolKitKeys;
            SupportsAsyncInvocation = supportsAsyncInvocation;
        }

        public string Name { get; }

        public string Description { get; }

        public string? IconName { get; }

        public IReadOnlyList<SkyweaverToolParameterDefinition> Parameters { get; }

        public bool IsSystemTool { get; }

        public bool CanBelongToToolKit { get; }

        public SkyweaverToolDefaultAgentPermission DefaultAgentPermission { get; }

        public IReadOnlyList<string> DefaultToolKitKeys { get; }

        public bool SupportsAsyncInvocation { get; }

        public bool CanUserDisable => !IsSystemTool;

        public bool RequiresAgentPermission => !IsSystemTool;

        private static IReadOnlyList<string> NormalizeToolKitKeys(IReadOnlyList<string>? defaultToolKitKeys)
        {
            if (defaultToolKitKeys == null || defaultToolKitKeys.Count == 0)
            {
                return Array.Empty<string>();
            }

            var normalizedKeys = new List<string>(defaultToolKitKeys.Count);
            var seenKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var rawKey in defaultToolKitKeys)
            {
                var normalizedKey = (rawKey ?? string.Empty).Trim();
                if (normalizedKey.Length == 0)
                {
                    continue;
                }

                if (!s_namePattern.IsMatch(normalizedKey))
                {
                    throw new ArgumentException(
                        $"Toolkit keys may only contain letters, numbers, '_' or '-': {normalizedKey}",
                        nameof(defaultToolKitKeys));
                }

                if (seenKeys.Add(normalizedKey))
                {
                    normalizedKeys.Add(normalizedKey);
                }
            }

            return normalizedKeys.ToArray();
        }
    }
}
