using System.Globalization;
using System.IO;
using System.Text;
using System.Windows;
using Skyweaver.Controls.ChatSessionControl.Views;
using Skyweaver.Services.SkyweaverTools;

namespace Skyweaver.Tools
{
    public sealed class ReadDirectoryRecursiveTool : ISkyweaverTool, ISkyweaverToolInvocationPresentationProvider
    {
        public const string ToolName = "ReadDirectoryRecursive";

        private static readonly SkyweaverToolDefinition s_definition = new(
            ToolName,
            "读取目录并按清晰层级返回文件列表。Depth 为可选参数，省略时默认为 1，也就是常见的目录列表。",
            "Script",
            [
                new SkyweaverToolParameterDefinition(
                    "Directory",
                    "要枚举的目录路径。相对路径会相对于当前工作区解析。",
                    SkyweaverToolParameterType.String,
                    isRequired: true),
                new SkyweaverToolParameterDefinition(
                    "Depth",
                    "最大递归深度。1 表示仅列出当前目录；省略时默认为 1。",
                    SkyweaverToolParameterType.Integer,
                    isRequired: false,
                    defaultValue: "1")
            ],
            defaultAgentPermission: SkyweaverToolDefaultAgentPermission.Allow);

        public SkyweaverToolDefinition Definition => s_definition;

        public FrameworkElement? CreateInvocationPresentation(SkyweaverToolInvocationPresentationContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            return ToolInvocationCardFactory.Create(
                context,
                [
                    new ToolInvocationCardFieldDefinition("目录", "Directory", "等待目录路径..."),
                    new ToolInvocationCardFieldDefinition("深度", "Depth", "默认深度为 1")
                ]);
        }

        public Task<SkyweaverToolResult> ExecuteAsync(
            SkyweaverToolContext context,
            SkyweaverToolArguments arguments,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var requestedDirectory = arguments.GetString("Directory") ?? string.Empty;
            string? resolvedDirectory = null;

            try
            {
                resolvedDirectory = ToolFileSystemHelper.ResolvePath(requestedDirectory, context.WorkspacePath);
                var relativeDirectory = ToolFileSystemHelper.TryGetWorkspaceRelativePath(context.WorkspacePath, resolvedDirectory);

                if (File.Exists(resolvedDirectory))
                {
                    return Task.FromResult(SkyweaverToolResult.Failure(
                        $"Path points to a file, not a directory: {resolvedDirectory}",
                        BuildData(resolvedDirectory, relativeDirectory, requestedDepth: null, directoryCount: null, fileCount: null)));
                }

                if (!Directory.Exists(resolvedDirectory))
                {
                    return Task.FromResult(SkyweaverToolResult.Failure(
                        $"Directory not found: {resolvedDirectory}",
                        BuildData(resolvedDirectory, relativeDirectory, requestedDepth: null, directoryCount: null, fileCount: null)));
                }

                var requestedDepth = arguments.GetInteger("Depth", 1);
                if (requestedDepth <= 0)
                {
                    return Task.FromResult(SkyweaverToolResult.Failure(
                        "Depth must be an integer greater than or equal to 1.",
                        BuildData(resolvedDirectory, relativeDirectory, requestedDepth, directoryCount: null, fileCount: null)));
                }

                var rootDirectory = new DirectoryInfo(resolvedDirectory);
                var rootNode = ReadNode(rootDirectory, resolvedDirectory, currentDepth: 1, requestedDepth, cancellationToken);
                var directoryCount = CountDirectories(rootNode);
                var fileCount = CountFiles(rootNode);

                return Task.FromResult(SkyweaverToolResult.Success(
                    BuildContent(resolvedDirectory, relativeDirectory, requestedDepth, rootNode, directoryCount, fileCount),
                    BuildData(resolvedDirectory, relativeDirectory, requestedDepth, directoryCount, fileCount)));
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or InvalidOperationException or ArgumentException or NotSupportedException)
            {
                return Task.FromResult(SkyweaverToolResult.Failure(
                    $"Failed to read directory: {ex.Message}",
                    BuildData(resolvedDirectory, ToolFileSystemHelper.TryGetWorkspaceRelativePath(context.WorkspacePath, resolvedDirectory ?? string.Empty), requestedDepth: null, directoryCount: null, fileCount: null)));
            }
        }

        private static DirectoryListingNode ReadNode(
            DirectoryInfo directoryInfo,
            string rootPath,
            int currentDepth,
            int? requestedDepth,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var node = new DirectoryListingNode(
                GetDirectoryName(directoryInfo),
                ToolFileSystemHelper.NormalizePathForPrompt(Path.GetRelativePath(rootPath, directoryInfo.FullName)));

            try
            {
                foreach (var file in directoryInfo.EnumerateFiles().OrderBy(item => item.Name, StringComparer.OrdinalIgnoreCase))
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    node.Files.Add(file.Name);
                }
            }
            catch (Exception ex) when (IsExpectedReadException(ex))
            {
                node.Notes.Add($"files unavailable: {ex.Message}");
            }

            DirectoryInfo[] childDirectories;
            try
            {
                childDirectories = directoryInfo.EnumerateDirectories()
                    .OrderBy(item => item.Name, StringComparer.OrdinalIgnoreCase)
                    .ToArray();
            }
            catch (Exception ex) when (IsExpectedReadException(ex))
            {
                node.Notes.Add($"subdirectories unavailable: {ex.Message}");
                return node;
            }

            var canRecurse = !requestedDepth.HasValue || currentDepth < requestedDepth.Value;
            foreach (var childDirectory in childDirectories)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var childNode = new DirectoryListingNode(
                    GetDirectoryName(childDirectory),
                    ToolFileSystemHelper.NormalizePathForPrompt(Path.GetRelativePath(rootPath, childDirectory.FullName)));

                if (IsReparsePoint(childDirectory))
                {
                    childNode.Notes.Add("not expanded: reparse point");
                    node.Children.Add(childNode);
                    continue;
                }

                if (!canRecurse)
                {
                    childNode.Notes.Add("not expanded: depth limit");
                    node.Children.Add(childNode);
                    continue;
                }

                node.Children.Add(ReadNode(childDirectory, rootPath, currentDepth + 1, requestedDepth, cancellationToken));
            }

            return node;
        }

        private static string BuildContent(
            string resolvedDirectory,
            string? relativeDirectory,
            int? requestedDepth,
            DirectoryListingNode rootNode,
            int directoryCount,
            int fileCount)
        {
            var builder = new StringBuilder(2048);
            builder.AppendLine($"RootDirectory: {resolvedDirectory}");

            if (!string.IsNullOrWhiteSpace(relativeDirectory))
            {
                builder.AppendLine($"WorkspaceRelativePath: {relativeDirectory}");
            }

            builder.AppendLine($"Depth: {FormatDepth(requestedDepth)}");
            builder.AppendLine($"DirectoriesReported: {directoryCount}");
            builder.AppendLine($"FilesReported: {fileCount}");
            builder.AppendLine();
            builder.AppendLine("Tree:");
            AppendTree(builder, rootNode, indentLevel: 0);
            builder.AppendLine();
            builder.AppendLine("Files By Directory:");
            AppendFilesByDirectory(builder, rootNode);
            return builder.ToString().TrimEnd();
        }

        private static IReadOnlyDictionary<string, object?> BuildData(
            string? resolvedDirectory,
            string? relativeDirectory,
            int? requestedDepth,
            int? directoryCount,
            int? fileCount)
        {
            return new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase)
            {
                ["resolvedDirectory"] = resolvedDirectory,
                ["workspaceRelativePath"] = relativeDirectory,
                ["depth"] = requestedDepth,
                ["directoriesReported"] = directoryCount,
                ["filesReported"] = fileCount
            };
        }

        private static void AppendTree(StringBuilder builder, DirectoryListingNode node, int indentLevel)
        {
            var indent = new string(' ', indentLevel * 2);
            builder.Append(indent);
            builder.Append(GetTreeLabel(node));

            if (node.Notes.Count > 0)
            {
                builder.Append(" [");
                builder.Append(string.Join("; ", node.Notes));
                builder.Append(']');
            }

            builder.AppendLine();

            foreach (var child in node.Children)
            {
                AppendTree(builder, child, indentLevel + 1);
            }

            foreach (var fileName in node.Files)
            {
                builder.Append(indent);
                builder.Append("  ");
                builder.AppendLine(fileName);
            }

            if (node.Children.Count == 0 && node.Files.Count == 0 && node.Notes.Count == 0)
            {
                builder.Append(indent);
                builder.AppendLine("  (empty)");
            }
        }

        private static void AppendFilesByDirectory(StringBuilder builder, DirectoryListingNode node)
        {
            builder.AppendLine($"[{node.RelativePath}]");

            if (node.Files.Count == 0)
            {
                builder.AppendLine("(no files)");
            }
            else
            {
                foreach (var fileName in node.Files)
                {
                    builder.AppendLine($"- {fileName}");
                }
            }

            foreach (var note in node.Notes)
            {
                builder.AppendLine($"Note: {note}");
            }

            builder.AppendLine();

            foreach (var child in node.Children)
            {
                AppendFilesByDirectory(builder, child);
            }
        }

        private static string FormatDepth(int? requestedDepth)
        {
            return requestedDepth.HasValue
                ? requestedDepth.Value.ToString(CultureInfo.InvariantCulture)
                : "1";
        }

        private static int CountDirectories(DirectoryListingNode node)
        {
            return 1 + node.Children.Sum(CountDirectories);
        }

        private static int CountFiles(DirectoryListingNode node)
        {
            return node.Files.Count + node.Children.Sum(CountFiles);
        }

        private static string GetTreeLabel(DirectoryListingNode node)
        {
            if (node.RelativePath == ".")
            {
                return "./";
            }

            return $"{node.Name}/";
        }

        private static string GetDirectoryName(DirectoryInfo directoryInfo)
        {
            return string.IsNullOrWhiteSpace(directoryInfo.Name)
                ? ToolFileSystemHelper.NormalizePathForPrompt(directoryInfo.FullName)
                : directoryInfo.Name;
        }

        private static bool IsReparsePoint(DirectoryInfo directoryInfo)
        {
            return (directoryInfo.Attributes & FileAttributes.ReparsePoint) != 0;
        }

        private static bool IsExpectedReadException(Exception ex)
        {
            return ex is IOException or UnauthorizedAccessException or ArgumentException or NotSupportedException;
        }

        private sealed class DirectoryListingNode
        {
            public DirectoryListingNode(string name, string relativePath)
            {
                Name = name;
                RelativePath = relativePath;
            }

            public string Name { get; }

            public string RelativePath { get; }

            public List<string> Files { get; } = [];

            public List<DirectoryListingNode> Children { get; } = [];

            public List<string> Notes { get; } = [];
        }
    }
}
