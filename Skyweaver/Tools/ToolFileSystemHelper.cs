using System.IO;
using System.Text;
using Skyweaver.Models.LateralFileSystem;
using Skyweaver.Services.LateralFileSystem;

namespace Skyweaver.Tools
{
    internal static class ToolFileSystemHelper
    {
        public sealed record LateralFileSystemPathResolution(
            string ResolvedPath,
            string NodeName,
            string NodeId,
            string NodeVirtualRootPath,
            string RelativePath,
            bool UsedShortcut);

        private const string LateralFileSystemShortcutRoot = "LateralFS";

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

            if (TryResolveLateralFileSystemShortcut(normalizedPath, out var lateralResolution))
            {
                return lateralResolution.ResolvedPath;
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

        public static bool TryResolveLateralFileSystemShortcut(
            string requestedPath,
            out LateralFileSystemPathResolution resolution)
        {
            resolution = default!;

            if (!TryParseLateralFileSystemShortcut(requestedPath, out var nodeName, out var relativePath))
            {
                return false;
            }

            var node = FindLateralFileSystemNodeByName(nodeName)
                ?? throw new InvalidOperationException($"LateralFS node not found: {nodeName}");
            var nodeRoot = Path.GetFullPath(node.VirtualRootPath);
            var resolvedPath = string.IsNullOrWhiteSpace(relativePath)
                ? nodeRoot
                : Path.GetFullPath(Path.Combine(nodeRoot, relativePath));

            if (!IsPathInsideOrSame(resolvedPath, nodeRoot))
            {
                throw new InvalidOperationException("LateralFS shortcut path cannot escape the selected node root.");
            }

            resolution = new LateralFileSystemPathResolution(
                resolvedPath,
                node.Name,
                node.Id,
                nodeRoot,
                NormalizePathForPrompt(Path.GetRelativePath(nodeRoot, resolvedPath)),
                UsedShortcut: true);
            return true;
        }

        public static bool TryGetContainingLateralFileSystemNode(
            string fullPath,
            out LateralFileSystemPathResolution resolution)
        {
            resolution = default!;

            if (string.IsNullOrWhiteSpace(fullPath))
            {
                return false;
            }

            var resolvedPath = Path.GetFullPath(fullPath);
            var node = LateralFileSystemRuntime.Instance.GetNodes()
                .Where(candidate => !string.IsNullOrWhiteSpace(candidate.VirtualRootPath))
                .Where(candidate => IsPathInsideOrSame(resolvedPath, candidate.VirtualRootPath))
                .OrderByDescending(candidate => Path.GetFullPath(candidate.VirtualRootPath).Length)
                .FirstOrDefault();

            if (node == null)
            {
                return false;
            }

            var nodeRoot = Path.GetFullPath(node.VirtualRootPath);
            resolution = new LateralFileSystemPathResolution(
                resolvedPath,
                node.Name,
                node.Id,
                nodeRoot,
                NormalizePathForPrompt(Path.GetRelativePath(nodeRoot, resolvedPath)),
                UsedShortcut: false);
            return true;
        }

        public static bool IsPathInsideOrSame(string candidatePath, string rootPath)
        {
            if (string.IsNullOrWhiteSpace(candidatePath) || string.IsNullOrWhiteSpace(rootPath))
            {
                return false;
            }

            var normalizedCandidate = Path.GetFullPath(candidatePath);
            var normalizedRoot = Path.GetFullPath(rootPath);

            if (string.Equals(normalizedCandidate, normalizedRoot, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            var rootWithSeparator = normalizedRoot.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                + Path.DirectorySeparatorChar;
            return normalizedCandidate.StartsWith(rootWithSeparator, StringComparison.OrdinalIgnoreCase);
        }

        private static bool TryParseLateralFileSystemShortcut(
            string requestedPath,
            out string nodeName,
            out string relativePath)
        {
            nodeName = string.Empty;
            relativePath = string.Empty;

            var normalized = (requestedPath ?? string.Empty).Trim().Trim('"');
            if (normalized.Length == 0)
            {
                return false;
            }

            if (!normalized.StartsWith(LateralFileSystemShortcutRoot, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            if (normalized.Length == LateralFileSystemShortcutRoot.Length)
            {
                throw new InvalidOperationException("LateralFS shortcut must include a node name.");
            }

            var separator = normalized[LateralFileSystemShortcutRoot.Length];
            if (separator is not ('\\' or '/'))
            {
                return false;
            }

            var rest = normalized[(LateralFileSystemShortcutRoot.Length + 1)..];
            var separatorIndex = rest.IndexOfAny(['\\', '/']);
            nodeName = separatorIndex < 0
                ? rest.Trim()
                : rest[..separatorIndex].Trim();
            relativePath = separatorIndex < 0
                ? string.Empty
                : rest[(separatorIndex + 1)..];

            if (nodeName.Length == 0)
            {
                throw new InvalidOperationException("LateralFS shortcut must include a node name.");
            }

            return true;
        }

        private static LateralFileSystemNodeModel? FindLateralFileSystemNodeByName(string nodeName)
        {
            return LateralFileSystemRuntime.Instance.GetNodes()
                .FirstOrDefault(node => string.Equals(node.Name, nodeName, StringComparison.OrdinalIgnoreCase));
        }
    }
}
