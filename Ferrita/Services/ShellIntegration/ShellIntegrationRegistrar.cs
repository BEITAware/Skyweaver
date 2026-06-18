using System.IO;
using Microsoft.Win32;
using Ferrita.Services.Localization;

namespace Ferrita.Services.ShellIntegration
{
    public sealed class ShellIntegrationRegistrar
    {
        private const string VerbName = "Ferrita.ShellChat";
        private const string FileVerbPath = @"Software\Classes\*\shell\" + VerbName;
        private const string DirectoryVerbPath = @"Software\Classes\Directory\shell\" + VerbName;
        private const string DirectoryBackgroundVerbPath = @"Software\Classes\Directory\Background\shell\" + VerbName;

        public void Register()
        {
            var executablePath = ResolveFerritaExecutablePath();
            var iconValue = $"{Quote(executablePath)},0";
            var menuText = LocalizationRuntime.Instance.GetString(
                "ShellIntegration.Menu.Text",
                "问问Ferrita...");

            RegisterVerb(
                FileVerbPath,
                menuText,
                iconValue,
                $"{Quote(executablePath)} --shell-chat --shell-context \"%1\"",
                supportsMultiSelect: true);
            RegisterVerb(
                DirectoryVerbPath,
                menuText,
                iconValue,
                $"{Quote(executablePath)} --shell-chat --shell-context \"%1\"",
                supportsMultiSelect: false);
            RegisterVerb(
                DirectoryBackgroundVerbPath,
                menuText,
                iconValue,
                $"{Quote(executablePath)} --shell-chat --shell-background \"%V\"",
                supportsMultiSelect: false);
        }

        public void Unregister()
        {
            DeleteVerb(FileVerbPath);
            DeleteVerb(DirectoryVerbPath);
            DeleteVerb(DirectoryBackgroundVerbPath);
        }

        public bool IsRegistered()
        {
            return HasCommand(FileVerbPath) &&
                   HasCommand(DirectoryVerbPath) &&
                   HasCommand(DirectoryBackgroundVerbPath);
        }

        private static void RegisterVerb(
            string verbPath,
            string menuText,
            string iconValue,
            string commandValue,
            bool supportsMultiSelect)
        {
            using var verbKey = Registry.CurrentUser.CreateSubKey(verbPath, writable: true)
                ?? throw new InvalidOperationException($"Failed to create registry key: HKCU\\{verbPath}");

            verbKey.SetValue(string.Empty, menuText, RegistryValueKind.String);
            verbKey.SetValue("MUIVerb", menuText, RegistryValueKind.String);
            verbKey.SetValue("Icon", iconValue, RegistryValueKind.String);

            if (supportsMultiSelect)
            {
                verbKey.SetValue("MultiSelectModel", "Player", RegistryValueKind.String);
            }
            else
            {
                verbKey.DeleteValue("MultiSelectModel", throwOnMissingValue: false);
            }

            using var commandKey = verbKey.CreateSubKey("command", writable: true)
                ?? throw new InvalidOperationException($"Failed to create registry key: HKCU\\{verbPath}\\command");

            commandKey.SetValue(string.Empty, commandValue, RegistryValueKind.String);
        }

        private static void DeleteVerb(string verbPath)
        {
            Registry.CurrentUser.DeleteSubKeyTree(verbPath, throwOnMissingSubKey: false);
        }

        private static bool HasCommand(string verbPath)
        {
            using var key = Registry.CurrentUser.OpenSubKey($@"{verbPath}\command", writable: false);
            return key?.GetValue(string.Empty) is string command && !string.IsNullOrWhiteSpace(command);
        }

        private static string ResolveFerritaExecutablePath()
        {
            var baseDirectoryExecutablePath = Path.Combine(AppContext.BaseDirectory, "Ferrita.exe");
            if (File.Exists(baseDirectoryExecutablePath))
            {
                return baseDirectoryExecutablePath;
            }

            var currentExecutablePath = Environment.ProcessPath ?? System.Diagnostics.Process.GetCurrentProcess().MainModule?.FileName ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(currentExecutablePath) && File.Exists(currentExecutablePath))
            {
                return currentExecutablePath;
            }

            throw new FileNotFoundException("Could not locate Ferrita.exe for shell integration registration.");
        }

        private static string Quote(string value)
        {
            return $"\"{value}\"";
        }
    }
}
