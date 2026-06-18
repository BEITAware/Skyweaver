using System.Globalization;
using Newtonsoft.Json.Linq;

namespace Ferrita.Services.FerritaTools
{
    public sealed class FerritaToolArguments
    {
        private readonly Dictionary<string, FerritaToolArgumentValue> _values;

        private FerritaToolArguments(
            IEnumerable<FerritaToolArgumentValue> values,
            IReadOnlyDictionary<string, string?> rawArguments)
        {
            _values = new Dictionary<string, FerritaToolArgumentValue>(StringComparer.OrdinalIgnoreCase);
            foreach (var value in values)
            {
                var parameterName = value.Definition.Name?.Trim();
                if (string.IsNullOrWhiteSpace(parameterName))
                {
                    continue;
                }

                _values[parameterName] = value;
            }

            RawArguments = new Dictionary<string, string?>(rawArguments, StringComparer.OrdinalIgnoreCase);
        }

        public IReadOnlyDictionary<string, string?> RawArguments { get; }

        public IReadOnlyCollection<FerritaToolArgumentValue> Values => _values.Values;

        internal static FerritaToolArguments Bind(
            IReadOnlyList<FerritaToolParameterDefinition> definitions,
            IReadOnlyDictionary<string, string?>? rawArguments)
        {
            var normalizedRawArguments = rawArguments == null
                ? new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase)
                : new Dictionary<string, string?>(rawArguments, StringComparer.OrdinalIgnoreCase);

            var values = new List<FerritaToolArgumentValue>(definitions.Count);
            foreach (var definition in definitions)
            {
                normalizedRawArguments.TryGetValue(definition.Name, out var rawValue);

                var candidate = string.IsNullOrWhiteSpace(rawValue)
                    ? definition.DefaultValue
                    : rawValue?.Trim();

                if (string.IsNullOrWhiteSpace(candidate))
                {
                    if (definition.IsRequired)
                    {
                        throw new InvalidOperationException($"Missing tool parameter: {definition.Name}");
                    }

                    values.Add(new FerritaToolArgumentValue(definition, rawValue, null));
                    continue;
                }

                values.Add(new FerritaToolArgumentValue(
                    definition,
                    rawValue,
                    ConvertValue(definition, candidate)));
            }

            return new FerritaToolArguments(values, normalizedRawArguments);
        }

        public bool Contains(string name)
        {
            return _values.ContainsKey(name);
        }

        public string? GetString(string name)
        {
            return GetValue(name)?.Value as string;
        }

        public bool GetBoolean(string name, bool fallback = false)
        {
            return GetValue(name)?.Value is bool value ? value : fallback;
        }

        public int GetInteger(string name, int fallback = 0)
        {
            return GetValue(name)?.Value is int value ? value : fallback;
        }

        public decimal GetNumber(string name, decimal fallback = 0m)
        {
            return GetValue(name)?.Value is decimal value ? value : fallback;
        }

        public JToken? GetJson(string name)
        {
            return GetValue(name)?.Value as JToken;
        }

        public FerritaToolArgumentValue? GetValue(string name)
        {
            var normalizedName = (name ?? string.Empty).Trim();
            if (normalizedName.Length == 0)
            {
                return null;
            }

            if (_values.TryGetValue(normalizedName, out var value))
            {
                return value;
            }

            return _values.Values.FirstOrDefault(item =>
                string.Equals(item.Definition.Name, normalizedName, StringComparison.OrdinalIgnoreCase));
        }

        private static object ConvertValue(FerritaToolParameterDefinition definition, string rawValue)
        {
            try
            {
                // Keep the switch expression typed as object; otherwise Newtonsoft's
                // implicit JToken conversions can promote scalar branches into JValue.
                return definition.ParameterType switch
                {
                    FerritaToolParameterType.Boolean => (object)ParseBoolean(rawValue),
                    FerritaToolParameterType.Integer => ParseInteger(rawValue),
                    FerritaToolParameterType.Number => ParseNumber(rawValue),
                    FerritaToolParameterType.Json => ParseJson(rawValue),
                    _ => rawValue
                };
            }
            catch (Exception ex) when (ex is FormatException or InvalidOperationException)
            {
                throw new InvalidOperationException(
                    $"Tool parameter '{definition.Name}' could not be parsed as {definition.ParameterType}: {rawValue}",
                    ex);
            }
        }

        private static bool ParseBoolean(string rawValue)
        {
            return rawValue.Trim().ToLowerInvariant() switch
            {
                "true" => true,
                "1" => true,
                "yes" => true,
                "y" => true,
                "on" => true,
                "false" => false,
                "0" => false,
                "no" => false,
                "n" => false,
                "off" => false,
                _ => throw new FormatException("Unrecognized boolean value.")
            };
        }

        private static int ParseInteger(string rawValue)
        {
            if (int.TryParse(rawValue.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var invariantValue))
            {
                return invariantValue;
            }

            if (int.TryParse(rawValue.Trim(), NumberStyles.Integer, CultureInfo.CurrentCulture, out var localValue))
            {
                return localValue;
            }

            throw new FormatException("Unrecognized integer value.");
        }

        private static decimal ParseNumber(string rawValue)
        {
            if (decimal.TryParse(rawValue.Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out var invariantValue))
            {
                return invariantValue;
            }

            if (decimal.TryParse(rawValue.Trim(), NumberStyles.Number, CultureInfo.CurrentCulture, out var localValue))
            {
                return localValue;
            }

            throw new FormatException("Unrecognized numeric value.");
        }

        private static JToken ParseJson(string rawValue)
        {
            return JToken.Parse(rawValue.Trim());
        }
    }

    public sealed class FerritaToolArgumentValue
    {
        internal FerritaToolArgumentValue(
            FerritaToolParameterDefinition definition,
            string? rawValue,
            object? value)
        {
            Definition = definition;
            RawValue = rawValue;
            Value = value;
        }

        public FerritaToolParameterDefinition Definition { get; }

        public string? RawValue { get; }

        public object? Value { get; }
    }
}
