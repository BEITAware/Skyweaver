using System.Text.RegularExpressions;

namespace Ferrita.Services.FerritaTools
{
    public sealed class FerritaToolParameterDefinition
    {
        private static readonly Regex s_namePattern = new("^[A-Za-z0-9_-]+$", RegexOptions.Compiled);

        public FerritaToolParameterDefinition(
            string name,
            string description,
            FerritaToolParameterType parameterType = FerritaToolParameterType.String,
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

        public FerritaToolParameterType ParameterType { get; }

        public bool IsRequired { get; }

        public string? DefaultValue { get; }
    }
}
