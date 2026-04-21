using System.Text.RegularExpressions;

namespace Skyweaver.Services.SkyweaverTools
{
    public sealed class SkyweaverToolParameterDefinition
    {
        private static readonly Regex s_namePattern = new("^[A-Za-z0-9_-]+$", RegexOptions.Compiled);

        public SkyweaverToolParameterDefinition(
            string name,
            string description,
            SkyweaverToolParameterType parameterType = SkyweaverToolParameterType.String,
            bool isRequired = true,
            string? defaultValue = null)
        {
            var normalizedName = (name ?? string.Empty).Trim();
            if (normalizedName.Length == 0)
            {
                throw new ArgumentException("Tool parameter name cannot be empty.", nameof(name));
            }

            if (!s_namePattern.IsMatch(normalizedName))
            {
                throw new ArgumentException("Tool parameter names may only contain letters, numbers, '_' or '-'.", nameof(name));
            }

            Name = normalizedName;
            Description = (description ?? string.Empty).Trim();
            ParameterType = parameterType;
            IsRequired = isRequired;
            DefaultValue = string.IsNullOrWhiteSpace(defaultValue) ? null : defaultValue.Trim();
        }

        public string Name { get; }

        public string Description { get; }

        public SkyweaverToolParameterType ParameterType { get; }

        public bool IsRequired { get; }

        public string? DefaultValue { get; }
    }
}
