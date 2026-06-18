using System.Globalization;

namespace Ferrita.Controls.WorkflowEditorControl.Services
{
    public static class SessionFlowBooleanNormalizer
    {
        private static readonly HashSet<string> TrueValues = new(StringComparer.OrdinalIgnoreCase)
        {
            "true",
            "t",
            "yes",
            "y",
            "on",
            "enabled",
            "enable",
            "ok",
            "是",
            "真",
            "对",
            "對",
            "开",
            "開"
        };

        private static readonly HashSet<string> FalseValues = new(StringComparer.OrdinalIgnoreCase)
        {
            "false",
            "f",
            "no",
            "n",
            "off",
            "disabled",
            "disable",
            "none",
            "否",
            "假",
            "错",
            "錯",
            "关",
            "關"
        };

        public static bool TryNormalize(object? value, out bool result)
        {
            switch (value)
            {
                case bool boolValue:
                    result = boolValue;
                    return true;

                case sbyte signedByteValue:
                    result = signedByteValue != 0;
                    return true;

                case byte byteValue:
                    result = byteValue != 0;
                    return true;

                case short shortValue:
                    result = shortValue != 0;
                    return true;

                case ushort unsignedShortValue:
                    result = unsignedShortValue != 0;
                    return true;

                case int intValue:
                    result = intValue != 0;
                    return true;

                case uint unsignedIntValue:
                    result = unsignedIntValue != 0;
                    return true;

                case long longValue:
                    result = longValue != 0;
                    return true;

                case ulong unsignedLongValue:
                    result = unsignedLongValue != 0;
                    return true;

                case float floatValue:
                    result = Math.Abs(floatValue) > float.Epsilon;
                    return true;

                case double doubleValue:
                    result = Math.Abs(doubleValue) > double.Epsilon;
                    return true;

                case decimal decimalValue:
                    result = decimalValue != 0m;
                    return true;

                case string text:
                    return TryNormalize(text, out result);
            }

            result = false;
            return false;
        }

        public static bool TryNormalize(string? text, out bool result)
        {
            var normalized = (text ?? string.Empty).Trim();
            if (normalized.Length == 0)
            {
                result = false;
                return false;
            }

            if (TrueValues.Contains(normalized))
            {
                result = true;
                return true;
            }

            if (FalseValues.Contains(normalized))
            {
                result = false;
                return true;
            }

            if (bool.TryParse(normalized, out var parsedBool))
            {
                result = parsedBool;
                return true;
            }

            if (long.TryParse(normalized, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsedLong))
            {
                result = parsedLong != 0;
                return true;
            }

            if (double.TryParse(normalized, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var parsedDouble))
            {
                result = Math.Abs(parsedDouble) > double.Epsilon;
                return true;
            }

            result = false;
            return false;
        }
    }
}
