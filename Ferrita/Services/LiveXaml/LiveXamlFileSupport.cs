using System.IO;
using Microsoft.CodeAnalysis.CSharp;

namespace Ferrita.Services.LiveXaml
{
    public sealed record LiveXamlResolvedFileSet(
        string RootDirectoryPath,
        string XamlFilePath,
        string CodeBehindFilePath);

    public static class LiveXamlFileSupport
    {
        public const string LiveXamlFolderName = "LiveXaml";

        public static string EnsureLiveXamlRootDirectory(string workspacePath)
        {
            var normalizedWorkspacePath = NormalizeWorkspacePath(workspacePath);
            var liveXamlRootPath = Path.Combine(normalizedWorkspacePath, LiveXamlFolderName);
            Directory.CreateDirectory(liveXamlRootPath);
            return liveXamlRootPath;
        }

        public static LiveXamlResolvedFileSet ResolveNewFileSet(string workspacePath, string requestedFileName)
        {
            var liveXamlRootPath = EnsureLiveXamlRootDirectory(workspacePath);
            var normalizedRelativePath = NormalizeRequestedRelativeFileName(requestedFileName);
            var xamlFilePath = GetContainedAbsolutePath(liveXamlRootPath, normalizedRelativePath);
            var codeBehindFilePath = $"{xamlFilePath}.cs";
            return new LiveXamlResolvedFileSet(liveXamlRootPath, xamlFilePath, codeBehindFilePath);
        }

        public static string NormalizeAbsoluteXamlPath(string xamlFilePath)
        {
            if (string.IsNullOrWhiteSpace(xamlFilePath))
            {
                throw new ArgumentException("The XAML file path cannot be empty.", nameof(xamlFilePath));
            }

            var trimmedPath = xamlFilePath.Trim();
            if (!Path.IsPathRooted(trimmedPath))
            {
                throw new ArgumentException(
                    "ShowLiveXAML requires a full absolute .xaml path. Reuse the path returned by InitializeLiveXAML.",
                    nameof(xamlFilePath));
            }

            var fullPath = Path.GetFullPath(trimmedPath);
            if (!fullPath.EndsWith(".xaml", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("The XAML file path must end with .xaml.", nameof(xamlFilePath));
            }

            return fullPath;
        }

        public static string? ResolveSiblingCodeBehindPath(string absoluteXamlFilePath)
        {
            var normalizedXamlFilePath = NormalizeAbsoluteXamlPath(absoluteXamlFilePath);
            var siblingCodeBehindPath = $"{normalizedXamlFilePath}.cs";
            return File.Exists(siblingCodeBehindPath) ? siblingCodeBehindPath : null;
        }

        public static string BuildSuggestedRootClassName(string requestedFileName)
        {
            var normalizedRelativePath = NormalizeRequestedRelativeFileName(requestedFileName);
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(normalizedRelativePath);
            var baseIdentifier = BuildIdentifier(fileNameWithoutExtension);
            return $"LiveXaml.{baseIdentifier}";
        }

        private static string NormalizeWorkspacePath(string workspacePath)
        {
            if (string.IsNullOrWhiteSpace(workspacePath))
            {
                throw new ArgumentException("The current session resource directory is unavailable.", nameof(workspacePath));
            }

            return Path.GetFullPath(workspacePath.Trim());
        }

        private static string NormalizeRequestedRelativeFileName(string requestedFileName)
        {
            if (string.IsNullOrWhiteSpace(requestedFileName))
            {
                throw new ArgumentException("XAMLFileName cannot be empty.", nameof(requestedFileName));
            }

            var normalized = requestedFileName
                .Trim()
                .Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);

            if (Path.IsPathRooted(normalized))
            {
                throw new ArgumentException("InitializeLiveXAML only accepts a file name or relative path inside the session resource folder.", nameof(requestedFileName));
            }

            if (!normalized.EndsWith(".xaml", StringComparison.OrdinalIgnoreCase))
            {
                normalized += ".xaml";
            }

            return normalized;
        }

        private static string GetContainedAbsolutePath(string rootDirectoryPath, string relativePath)
        {
            var fullRootPath = Path.GetFullPath(rootDirectoryPath);
            var fullCandidatePath = Path.GetFullPath(Path.Combine(fullRootPath, relativePath));
            var rootWithTrailingSeparator = fullRootPath.EndsWith(Path.DirectorySeparatorChar)
                ? fullRootPath
                : $"{fullRootPath}{Path.DirectorySeparatorChar}";

            if (!fullCandidatePath.StartsWith(rootWithTrailingSeparator, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("The requested LiveXAML file path escapes the session LiveXaml folder.");
            }

            var directoryPath = Path.GetDirectoryName(fullCandidatePath);
            if (!string.IsNullOrWhiteSpace(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            return fullCandidatePath;
        }

        private static string BuildIdentifier(string rawValue)
        {
            var segments = rawValue
                .Split(['-', '_', '.', ' '], StringSplitOptions.RemoveEmptyEntries)
                .Select(segment => segment.Trim())
                .Where(segment => segment.Length > 0)
                .ToArray();

            var candidate = segments.Length == 0
                ? "LiveXamlView"
                : string.Concat(segments.Select(ToPascalCase));

            if (!SyntaxFacts.IsValidIdentifier(candidate))
            {
                candidate = $"LiveXaml{candidate}";
            }

            return SyntaxFacts.IsValidIdentifier(candidate) ? candidate : "LiveXamlView";
        }

        private static string ToPascalCase(string segment)
        {
            if (segment.Length == 0)
            {
                return string.Empty;
            }

            return segment.Length == 1
                ? char.ToUpperInvariant(segment[0]).ToString()
                : $"{char.ToUpperInvariant(segment[0])}{segment[1..]}";
        }
    }
}
