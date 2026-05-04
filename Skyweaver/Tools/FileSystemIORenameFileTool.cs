using System.IO;
using System.Text;
using System.Windows;
using Skyweaver.Controls.ChatSessionControl.Views;
using Skyweaver.Services.SkyweaverTools;

namespace Skyweaver.Tools
{
    public sealed class FileSystemIORenameFileTool :
        ISkyweaverTool,
        ISkyweaverToolConfigurationProvider,
        ISkyweaverToolInvocationPresentationProvider,
        ISkyweaverToolPromptDescriptionProvider
    {
        public const string ToolName = "FileSystemIO_RenameFile";

        private const string SettingsRootElementName = "FileSystemIORenameFileSettings";

        private static readonly SkyweaverToolDefinition s_definition = BuildDefinition(new ToolFileSystemPermissionSettings());

        public SkyweaverToolDefinition Definition => s_definition;

        public SkyweaverToolDefinition GetEffectiveDefinition(SkyweaverToolConfigurationState configuration)
        {
            return BuildDefinition(ToolFileSystemPermissionSettings.FromConfiguration(configuration, SettingsRootElementName));
        }

        public SkyweaverToolConfigurationPresenter? CreateConfigurationPresenter(SkyweaverToolConfigurationEditorContext context)
        {
            return new ToolFileSystemPermissionConfigurationPresenter(context, SettingsRootElementName, ToolName);
        }

        public string GetPromptDescription(SkyweaverToolPromptDescriptionContext context)
        {
            return BuildDescription(ToolFileSystemPermissionSettings.FromConfiguration(context.ConfigurationState, SettingsRootElementName));
        }

        public FrameworkElement? CreateInvocationPresentation(SkyweaverToolInvocationPresentationContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            return ToolInvocationCardFactory.Create(
                context,
                [
                    new ToolInvocationCardFieldDefinition("Source", "SourcePath", "Waiting for source path..."),
                    new ToolInvocationCardFieldDefinition("Destination", "DestinationPath", "Waiting for destination path...")
                ]);
        }

        public Task<SkyweaverToolResult> ExecuteAsync(
            SkyweaverToolContext context,
            SkyweaverToolArguments arguments,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var settings = ToolFileSystemPermissionSettings.FromConfiguration(context.CurrentToolConfiguration, SettingsRootElementName);
            var sourcePath = arguments.GetString("SourcePath") ?? string.Empty;
            var destinationPath = arguments.GetString("DestinationPath") ?? string.Empty;
            ToolResolvedPathInfo? resolvedSource = null;
            ToolResolvedPathInfo? resolvedDestination = null;

            try
            {
                resolvedSource = ToolFileSystemMutationSupport.ResolveAuthorizedPath(
                    sourcePath,
                    context.WorkspacePath,
                    settings.PermissionScope);
                resolvedDestination = ToolFileSystemMutationSupport.ResolveAuthorizedPath(
                    destinationPath,
                    context.WorkspacePath,
                    settings.PermissionScope);

                var sourceIsFile = File.Exists(resolvedSource.ResolvedPath);
                var sourceIsDirectory = Directory.Exists(resolvedSource.ResolvedPath);
                if (!sourceIsFile && !sourceIsDirectory)
                {
                    return Task.FromResult(SkyweaverToolResult.Failure(
                        $"Source path does not exist: {resolvedSource.ResolvedPath}",
                        BuildData(resolvedSource, resolvedDestination, settings, entryKind: null, destinationParentCreated: false, didMove: false)));
                }

                var entryKind = sourceIsFile ? "File" : "Directory";
                if (ToolFileSystemMutationSupport.AreSamePath(resolvedSource.ResolvedPath, resolvedDestination.ResolvedPath))
                {
                    return Task.FromResult(SkyweaverToolResult.Success(
                        BuildNoChangeContent(resolvedSource, resolvedDestination, settings, entryKind),
                        BuildData(resolvedSource, resolvedDestination, settings, entryKind, destinationParentCreated: false, didMove: false)));
                }

                if (File.Exists(resolvedDestination.ResolvedPath))
                {
                    return Task.FromResult(SkyweaverToolResult.Failure(
                        $"A file already exists at the destination path: {resolvedDestination.ResolvedPath}",
                        BuildData(resolvedSource, resolvedDestination, settings, entryKind, destinationParentCreated: false, didMove: false)));
                }

                if (Directory.Exists(resolvedDestination.ResolvedPath))
                {
                    return Task.FromResult(SkyweaverToolResult.Failure(
                        $"A directory already exists at the destination path: {resolvedDestination.ResolvedPath}",
                        BuildData(resolvedSource, resolvedDestination, settings, entryKind, destinationParentCreated: false, didMove: false)));
                }

                if (sourceIsDirectory &&
                    ToolFileSystemHelper.IsPathInsideOrSame(resolvedDestination.ResolvedPath, resolvedSource.ResolvedPath))
                {
                    return Task.FromResult(SkyweaverToolResult.Failure(
                        "The destination path cannot be the same as, or inside, the source directory.",
                        BuildData(resolvedSource, resolvedDestination, settings, entryKind, destinationParentCreated: false, didMove: false)));
                }

                var destinationParent = Path.GetDirectoryName(resolvedDestination.ResolvedPath);
                var destinationParentCreated = false;
                if (!string.IsNullOrWhiteSpace(destinationParent))
                {
                    if (File.Exists(destinationParent))
                    {
                        return Task.FromResult(SkyweaverToolResult.Failure(
                            $"The destination parent path is an existing file, not a directory: {destinationParent}",
                            BuildData(resolvedSource, resolvedDestination, settings, entryKind, destinationParentCreated: false, didMove: false)));
                    }

                    if (!Directory.Exists(destinationParent))
                    {
                        Directory.CreateDirectory(destinationParent);
                        destinationParentCreated = true;
                    }
                }

                if (sourceIsFile)
                {
                    File.Move(resolvedSource.ResolvedPath, resolvedDestination.ResolvedPath);
                }
                else
                {
                    Directory.Move(resolvedSource.ResolvedPath, resolvedDestination.ResolvedPath);
                }

                return Task.FromResult(SkyweaverToolResult.Success(
                    BuildSuccessContent(resolvedSource, resolvedDestination, settings, entryKind, destinationParentCreated),
                    BuildData(resolvedSource, resolvedDestination, settings, entryKind, destinationParentCreated, didMove: true)));
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex) when (IsExpectedException(ex))
            {
                return Task.FromResult(SkyweaverToolResult.Failure(
                    $"Failed to rename or move entry: {ex.Message}",
                    BuildData(resolvedSource, resolvedDestination, settings, entryKind: null, destinationParentCreated: false, didMove: false)));
            }
        }

        private static SkyweaverToolDefinition BuildDefinition(ToolFileSystemPermissionSettings settings)
        {
            return new SkyweaverToolDefinition(
                ToolName,
                BuildDescription(settings),
                "Script",
                [
                    new SkyweaverToolParameterDefinition(
                        "SourcePath",
                        "The current file or directory path. Relative paths resolve against the current workspace. You may also use LateralFS\\NodeName\\relative\\path; the host resolves that shortcut to the node virtual folder and blocks path traversal outside the node.",
                        SkyweaverToolParameterType.String,
                        isRequired: true),
                    new SkyweaverToolParameterDefinition(
                        "DestinationPath",
                        "The new file or directory path. Relative paths resolve against the current workspace. You may also use LateralFS\\NodeName\\relative\\path; the host resolves that shortcut to the node virtual folder and blocks path traversal outside the node.",
                        SkyweaverToolParameterType.String,
                        isRequired: true)
                ],
                isSystemTool: false);
        }

        private static string BuildDescription(ToolFileSystemPermissionSettings settings)
        {
            return ToolFileSystemMutationSupport.BuildPromptDescription(
                "Renames or moves a file or directory. The tool validates the source, destination, and destination parent before creating any missing destination directories or moving the entry.",
                settings.PermissionScope);
        }

        private static string BuildSuccessContent(
            ToolResolvedPathInfo sourcePath,
            ToolResolvedPathInfo destinationPath,
            ToolFileSystemPermissionSettings settings,
            string entryKind,
            bool destinationParentCreated)
        {
            var builder = new StringBuilder(512);
            builder.AppendLine($"EntryKind: {entryKind}");
            builder.AppendLine($"PermissionScope: {settings.PermissionScope}");
            builder.AppendLine($"DestinationParentCreated: {destinationParentCreated}");
            builder.AppendLine();
            builder.AppendLine("Source:");
            ToolFileSystemMutationSupport.AppendPathInfo(builder, sourcePath);
            builder.AppendLine();
            builder.AppendLine("Destination:");
            ToolFileSystemMutationSupport.AppendPathInfo(builder, destinationPath);
            return builder.ToString().TrimEnd();
        }

        private static string BuildNoChangeContent(
            ToolResolvedPathInfo sourcePath,
            ToolResolvedPathInfo destinationPath,
            ToolFileSystemPermissionSettings settings,
            string entryKind)
        {
            var builder = new StringBuilder(512);
            builder.AppendLine("No changes were made because the source and destination resolve to the same path.");
            builder.AppendLine($"EntryKind: {entryKind}");
            builder.AppendLine($"PermissionScope: {settings.PermissionScope}");
            builder.AppendLine();
            builder.AppendLine("Source:");
            ToolFileSystemMutationSupport.AppendPathInfo(builder, sourcePath);
            builder.AppendLine();
            builder.AppendLine("Destination:");
            ToolFileSystemMutationSupport.AppendPathInfo(builder, destinationPath);
            return builder.ToString().TrimEnd();
        }

        private static IReadOnlyDictionary<string, object?> BuildData(
            ToolResolvedPathInfo? sourcePath,
            ToolResolvedPathInfo? destinationPath,
            ToolFileSystemPermissionSettings settings,
            string? entryKind,
            bool destinationParentCreated,
            bool didMove)
        {
            return new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase)
            {
                ["entryKind"] = entryKind,
                ["permissionScope"] = settings.PermissionScope.ToString(),
                ["sourceResolvedPath"] = sourcePath?.ResolvedPath,
                ["sourceWorkspaceRelativePath"] = sourcePath?.WorkspaceRelativePath,
                ["sourceLateralNodeName"] = sourcePath?.LateralNodeName,
                ["sourceLateralNodeId"] = sourcePath?.LateralNodeId,
                ["sourceLateralNodeVirtualRootPath"] = sourcePath?.LateralNodeVirtualRootPath,
                ["sourceLateralRelativePath"] = sourcePath?.LateralRelativePath,
                ["sourceUsedLateralShortcut"] = sourcePath?.UsedLateralShortcut,
                ["destinationResolvedPath"] = destinationPath?.ResolvedPath,
                ["destinationWorkspaceRelativePath"] = destinationPath?.WorkspaceRelativePath,
                ["destinationLateralNodeName"] = destinationPath?.LateralNodeName,
                ["destinationLateralNodeId"] = destinationPath?.LateralNodeId,
                ["destinationLateralNodeVirtualRootPath"] = destinationPath?.LateralNodeVirtualRootPath,
                ["destinationLateralRelativePath"] = destinationPath?.LateralRelativePath,
                ["destinationUsedLateralShortcut"] = destinationPath?.UsedLateralShortcut,
                ["destinationParentCreated"] = destinationParentCreated,
                ["didMove"] = didMove
            };
        }

        private static bool IsExpectedException(Exception ex)
        {
            return ex is IOException
                or UnauthorizedAccessException
                or InvalidOperationException
                or ArgumentException
                or NotSupportedException;
        }
    }
}
