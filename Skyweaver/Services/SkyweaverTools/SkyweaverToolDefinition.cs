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
            bool canBelongToToolKit = true)
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
            var parameterNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var parameter in normalizedParameters)
            {
                if (!parameterNames.Add(parameter.Name))
                {
                    throw new ArgumentException($"Tool '{normalizedName}' contains a duplicated parameter: {parameter.Name}", nameof(parameters));
                }
            }

            Name = normalizedName;
            Description = (description ?? string.Empty).Trim();
            IconName = string.IsNullOrWhiteSpace(iconName) ? null : iconName.Trim();
            Parameters = normalizedParameters;
            IsSystemTool = isSystemTool;
            CanBelongToToolKit = !isSystemTool && canBelongToToolKit;
        }

        public string Name { get; }

        public string Description { get; }

        public string? IconName { get; }

        public IReadOnlyList<SkyweaverToolParameterDefinition> Parameters { get; }

        public bool IsSystemTool { get; }

        public bool CanBelongToToolKit { get; }

        public bool CanUserDisable => !IsSystemTool;

        public bool RequiresAgentPermission => !IsSystemTool;
    }
}
