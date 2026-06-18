namespace Ferrita.Services.ShellIntegration
{
    public sealed class ShellChatStartupContext
    {
        public static ShellChatStartupContext Empty { get; } = new();

        public IReadOnlyList<string> SelectedPaths { get; init; } = Array.Empty<string>();

        public string BackgroundDirectoryPath { get; init; } = string.Empty;

        public bool HasContext =>
            SelectedPaths.Count > 0 ||
            !string.IsNullOrWhiteSpace(BackgroundDirectoryPath);
    }

    public static class ShellIntegrationCommandLine
    {
        public static bool IsShellChatStartup(IEnumerable<string> args)
        {
            return ParseShellChatStartup(args) != null;
        }

        public static ShellChatStartupContext? ParseShellChatStartup(IEnumerable<string> args)
        {
            ArgumentNullException.ThrowIfNull(args);

            var normalizedArgs = args.ToArray();
            var isShellChatStartup = false;
            var selectedPaths = new List<string>();
            var backgroundDirectoryPath = string.Empty;

            for (var index = 0; index < normalizedArgs.Length; index++)
            {
                var arg = normalizedArgs[index]?.Trim() ?? string.Empty;
                if (arg.Length == 0)
                {
                    continue;
                }

                if (IsSwitch(arg, "--shell-chat", "/shell-chat"))
                {
                    isShellChatStartup = true;
                    continue;
                }

                if (TryReadOptionValues(normalizedArgs, ref index, "--shell-context", "/shell-context", out var contextPaths))
                {
                    selectedPaths.AddRange(contextPaths);
                    continue;
                }

                if (TryReadOptionValue(normalizedArgs, ref index, "--shell-background", "/shell-background", out var backgroundPath) &&
                    !string.IsNullOrWhiteSpace(backgroundPath))
                {
                    backgroundDirectoryPath = backgroundPath.Trim();
                }
            }

            return isShellChatStartup
                ? new ShellChatStartupContext
                {
                    SelectedPaths = selectedPaths
                        .Where(path => !string.IsNullOrWhiteSpace(path))
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .ToArray(),
                    BackgroundDirectoryPath = backgroundDirectoryPath
                }
                : null;
        }

        private static bool IsSwitch(string arg, params string[] names)
        {
            return names.Any(name => string.Equals(arg, name, StringComparison.OrdinalIgnoreCase));
        }

        private static bool IsKnownShellOption(string arg)
        {
            return IsSwitch(arg, "--shell-chat", "/shell-chat") ||
                   IsOptionOrInlineValue(arg, "--shell-context", "/shell-context") ||
                   IsOptionOrInlineValue(arg, "--shell-background", "/shell-background");
        }

        private static bool IsOptionOrInlineValue(
            string arg,
            string dashName,
            string slashName)
        {
            return IsSwitch(arg, dashName, slashName) ||
                   arg.StartsWith(dashName + "=", StringComparison.OrdinalIgnoreCase) ||
                   arg.StartsWith(slashName + ":", StringComparison.OrdinalIgnoreCase);
        }

        private static bool TryReadOptionValues(
            IReadOnlyList<string> args,
            ref int index,
            string dashName,
            string slashName,
            out IReadOnlyList<string> values)
        {
            values = Array.Empty<string>();
            var arg = args[index]?.Trim() ?? string.Empty;
            if (arg.Length == 0)
            {
                return false;
            }

            var collectedValues = new List<string>();
            if (TryReadInlineValue(arg, dashName, '=', out var inlineDashValue) ||
                TryReadInlineValue(arg, slashName, ':', out inlineDashValue))
            {
                AddNonEmpty(collectedValues, inlineDashValue);
                CollectTrailingOptionValues(args, ref index, collectedValues);
                values = collectedValues;
                return true;
            }

            if (!IsSwitch(arg, dashName, slashName))
            {
                return false;
            }

            CollectTrailingOptionValues(args, ref index, collectedValues);
            values = collectedValues;
            return true;
        }

        private static void CollectTrailingOptionValues(
            IReadOnlyList<string> args,
            ref int index,
            ICollection<string> values)
        {
            while (index + 1 < args.Count)
            {
                var nextValue = args[index + 1]?.Trim() ?? string.Empty;
                if (nextValue.Length == 0)
                {
                    index++;
                    continue;
                }

                if (IsKnownShellOption(nextValue))
                {
                    break;
                }

                values.Add(nextValue);
                index++;
            }
        }

        private static void AddNonEmpty(ICollection<string> values, string? value)
        {
            var normalizedValue = value?.Trim() ?? string.Empty;
            if (normalizedValue.Length > 0)
            {
                values.Add(normalizedValue);
            }
        }

        private static bool TryReadOptionValue(
            IReadOnlyList<string> args,
            ref int index,
            string dashName,
            string slashName,
            out string value)
        {
            value = string.Empty;
            var arg = args[index]?.Trim() ?? string.Empty;
            if (arg.Length == 0)
            {
                return false;
            }

            if (TryReadInlineValue(arg, dashName, '=', out value) ||
                TryReadInlineValue(arg, slashName, ':', out value))
            {
                return true;
            }

            if (!IsSwitch(arg, dashName, slashName))
            {
                return false;
            }

            if (index + 1 >= args.Count)
            {
                return true;
            }

            value = args[++index]?.Trim() ?? string.Empty;
            return true;
        }

        private static bool TryReadInlineValue(
            string arg,
            string optionName,
            char separator,
            out string value)
        {
            value = string.Empty;
            var prefix = optionName + separator;
            if (!arg.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            value = arg[prefix.Length..].Trim();
            return true;
        }
    }
}
