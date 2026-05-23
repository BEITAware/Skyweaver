using System.Diagnostics;
using System.IO;

namespace Skyweaver.Services.Skylifter
{
    public static class SkylifterLauncher
    {
        public static bool IsDaemonOnlyStartup(IEnumerable<string> args)
        {
            return args.Any(arg =>
                string.Equals(arg, "--daemon", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(arg, "--daemon-only", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(arg, "--skylifter-only", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(arg, "/daemon", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(arg, "/daemon-only", StringComparison.OrdinalIgnoreCase));
        }

        public static string? GetCurrentSkyweaverExecutablePath()
        {
            var processPath = Environment.ProcessPath;
            if (!string.IsNullOrWhiteSpace(processPath) && File.Exists(processPath))
            {
                return processPath;
            }

            using var currentProcess = Process.GetCurrentProcess();
            var modulePath = currentProcess.MainModule?.FileName;
            return !string.IsNullOrWhiteSpace(modulePath) && File.Exists(modulePath)
                ? modulePath
                : null;
        }

        public static bool EnsureStarted(string? skyweaverExecutablePath = null)
        {
            if (IsSkylifterAlreadyRunning())
            {
                _ = SkylifterIpcClient.TryRegisterSkyweaverPathAsync(skyweaverExecutablePath);
                return true;
            }

            var skylifterPath = FindSkylifterExecutablePath();
            if (string.IsNullOrWhiteSpace(skylifterPath))
            {
                return false;
            }

            try
            {
                var arguments = string.IsNullOrWhiteSpace(skyweaverExecutablePath)
                    ? string.Empty
                    : $"--skyweaver-exe \"{skyweaverExecutablePath}\"";

                Process.Start(new ProcessStartInfo
                {
                    FileName = skylifterPath,
                    Arguments = arguments,
                    WorkingDirectory = Path.GetDirectoryName(skylifterPath) ?? AppContext.BaseDirectory,
                    UseShellExecute = true
                });

                return true;
            }
            catch
            {
                return false;
            }
        }

        private static bool IsSkylifterAlreadyRunning()
        {
            try
            {
                return Process.GetProcessesByName("Skylifter")
                    .Any(process =>
                    {
                        try
                        {
                            return !process.HasExited;
                        }
                        catch
                        {
                            return false;
                        }
                    });
            }
            catch
            {
                return false;
            }
        }

        private static string? FindSkylifterExecutablePath()
        {
            var baseDirectory = Path.GetFullPath(AppContext.BaseDirectory);
            foreach (var candidate in BuildSkylifterCandidates(baseDirectory))
            {
                if (File.Exists(candidate))
                {
                    return candidate;
                }
            }

            return null;
        }

        private static IEnumerable<string> BuildSkylifterCandidates(string baseDirectory)
        {
            yield return Path.Combine(baseDirectory, "Skylifter.exe");
            yield return Path.Combine(baseDirectory, "Skylifter", "Skylifter.exe");

            var netDirectory = Directory.GetParent(baseDirectory);
            var configurationDirectory = netDirectory?.Parent;
            var binDirectory = configurationDirectory?.Parent;
            var skyweaverProjectDirectory = binDirectory?.Parent;
            var solutionDirectory = skyweaverProjectDirectory?.Parent;

            if (configurationDirectory == null || solutionDirectory == null)
            {
                yield break;
            }

            var configurationName = configurationDirectory.Name;
            yield return Path.Combine(
                solutionDirectory.FullName,
                "Skylifter",
                "bin",
                configurationName,
                "net8.0-windows",
                "Skylifter.exe");

            yield return Path.Combine(
                solutionDirectory.FullName,
                "Skylifter",
                "bin",
                "Debug",
                "net8.0-windows",
                "Skylifter.exe");

            yield return Path.Combine(
                solutionDirectory.FullName,
                "Skylifter",
                "bin",
                "Release",
                "net8.0-windows",
                "Skylifter.exe");
        }
    }
}
