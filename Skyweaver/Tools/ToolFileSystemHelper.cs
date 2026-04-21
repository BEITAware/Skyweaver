using System.IO;
using System.Text;

namespace Skyweaver.Tools
{
    internal static class ToolFileSystemHelper
    {
        private static readonly Lazy<bool> s_encodingProviderRegistration = new(() =>
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            return true;
        });

        public static string ResolvePath(string requestedPath, string? workspacePath)
        {
            var normalizedPath = (requestedPath ?? string.Empty).Trim();
            if (normalizedPath.Length == 0)
            {
                throw new InvalidOperationException("Path cannot be empty.");
            }

            var baseDirectory = ResolveBaseDirectory(workspacePath);
            return Path.GetFullPath(
                Path.IsPathRooted(normalizedPath)
                    ? normalizedPath
                    : Path.Combine(baseDirectory, normalizedPath));
        }

        public static string ResolveBaseDirectory(string? workspacePath)
        {
            var candidate = string.IsNullOrWhiteSpace(workspacePath)
                ? Environment.CurrentDirectory
                : workspacePath.Trim();

            return Path.GetFullPath(candidate);
        }

        public static string? TryGetWorkspaceRelativePath(string? workspacePath, string fullPath)
        {
            if (string.IsNullOrWhiteSpace(workspacePath) || string.IsNullOrWhiteSpace(fullPath))
            {
                return null;
            }

            var workspaceRoot = ResolveBaseDirectory(workspacePath);
            var relativePath = Path.GetRelativePath(workspaceRoot, fullPath);
            return NormalizePathForPrompt(relativePath);
        }

        public static string NormalizePathForPrompt(string path)
        {
            var normalized = (path ?? string.Empty).Trim();
            if (normalized.Length == 0 || normalized == ".")
            {
                return ".";
            }

            return normalized
                .Replace(Path.DirectorySeparatorChar, '/')
                .Replace(Path.AltDirectorySeparatorChar, '/');
        }

        public static Encoding ResolveEncoding(string? encodingName)
        {
            _ = s_encodingProviderRegistration.Value;

            var normalizedName = string.IsNullOrWhiteSpace(encodingName)
                ? "utf-8"
                : encodingName.Trim();

            try
            {
                return Encoding.GetEncoding(normalizedName);
            }
            catch (Exception ex) when (ex is ArgumentException or NotSupportedException)
            {
                throw new InvalidOperationException($"Unsupported text encoding: {normalizedName}", ex);
            }
        }

        public static int CountLines(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return 0;
            }

            var lineCount = 1;
            for (var index = 0; index < text.Length; index++)
            {
                if (text[index] == '\n')
                {
                    lineCount++;
                }
            }

            return lineCount;
        }
    }
}
