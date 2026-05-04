using System.IO;
using System.Text;
using System.Windows;
using Skyweaver.Controls.ChatSessionControl.Views;
using Skyweaver.Services.SkyweaverTools;

namespace Skyweaver.Tools
{
    public sealed class FileSystemIODeleteDirectoryTool :
        ISkyweaverTool,
        ISkyweaverToolConfigurationProvider,
        ISkyweaverToolInvocationPresentationProvider,
        ISkyweaverToolPromptDescriptionProvider
    {
        public const string ToolName = "FileSystemIO_DeleteDirectory";

        private const string SettingsRootElementName = "FileSystemIODeleteDirectorySettings";

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
                    new ToolInvocationCardFieldDefinition("Directory", "DirectoryPath", "Waiting for directory path..."),
                    new ToolInvocationCardFieldDefinition("Recursive", "Recursive", "Default true")
                ]);
        }

        public Task<SkyweaverToolResult> ExecuteAsync(
            SkyweaverToolContext context,
            SkyweaverToolArguments arguments,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var settings = ToolFileSystemPermissionSettings.FromConfiguration(context.CurrentToolConfiguration, SettingsRootElementName);
            var requestedPath = arguments.GetString("DirectoryPath") ?? string.Empty;
            var recursive = arguments.GetBoolean("Recursive", true);
            ToolResolvedPathInfo? targetPath = null;

            try
            {
                targetPath = ToolFileSystemMutationSupport.ResolveAuthorizedPath(
                    requestedPath,
                    context.WorkspacePath,
                    settings.PermissionScope);

                if (File.Exists(targetPath.ResolvedPath))
                {
                    return Task.FromResult(SkyweaverToolResult.Failure(
                        $"Path points to a file, not a directory: {targetPath.ResolvedPath}",
                        BuildData(targetPath, settings, recursive, didDelete: false)));
                }

                if (!Directory.Exists(targetPath.ResolvedPath))
                {
                    return Task.FromResult(SkyweaverToolResult.Failure(
                        $"Directory not found: {targetPath.ResolvedPath}",
                        BuildData(targetPath, settings, recursive, didDelete: false)));
                }

                if (!recursive && Directory.EnumerateFileSystemEntries(targetPath.ResolvedPath).Any())
                {
                    return Task.FromResult(SkyweaverToolResult.Failure(
                        $"Directory is not empty and Recursive is false: {targetPath.ResolvedPath}",
                        BuildData(targetPath, settings, recursive, didDelete: false)));
                }

                Directory.Delete(targetPath.ResolvedPath, recursive);

                return Task.FromResult(SkyweaverToolResult.Success(
                    BuildSuccessContent(targetPath, settings, recursive),
                    BuildData(targetPath, settings, recursive, didDelete: true)));
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex) when (IsExpectedException(ex))
            {
                return Task.FromResult(SkyweaverToolResult.Failure(
                    $"Failed to delete directory: {ex.Message}",
                    BuildData(targetPath, settings, recursive, didDelete: false)));
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
                        "DirectoryPath",
                        "The directory path to delete. Relative paths resolve against the current workspace. You may also use LateralFS\\NodeName\\relative\\folder; the host resolves that shortcut to the node virtual folder and blocks path traversal outside the node.",
                        SkyweaverToolParameterType.String,
                        isRequired: true),
                    new SkyweaverToolParameterDefinition(
                        "Recursive",
                        "If true, removes all files and subdirectories. Defaults to true.",
                        SkyweaverToolParameterType.Boolean,
                        isRequired: false,
                        defaultValue: "true")
                ],
                isSystemTool: false);
        }

        private static string BuildDescription(ToolFileSystemPermissionSettings settings)
        {
            return ToolFileSystemMutationSupport.BuildPromptDescription(
                "Deletes a directory. The tool validates the target type before deleting it, and when Recursive is false it refuses to delete non-empty directories.",
                settings.PermissionScope);
        }

        private static string BuildSuccessContent(
            ToolResolvedPathInfo targetPath,
            ToolFileSystemPermissionSettings settings,
            bool recursive)
        {
            var builder = new StringBuilder(256);
            ToolFileSystemMutationSupport.AppendPathInfo(builder, targetPath);
            builder.AppendLine($"PermissionScope: {settings.PermissionScope}");
            builder.AppendLine($"Recursive: {recursive}");
            builder.AppendLine("DeletedDirectory: true");
            return builder.ToString().TrimEnd();
        }

        private static IReadOnlyDictionary<string, object?> BuildData(
            ToolResolvedPathInfo? targetPath,
            ToolFileSystemPermissionSettings settings,
            bool recursive,
            bool didDelete)
        {
            return new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase)
            {
                ["resolvedPath"] = targetPath?.ResolvedPath,
                ["workspaceRelativePath"] = targetPath?.WorkspaceRelativePath,
                ["permissionScope"] = settings.PermissionScope.ToString(),
                ["lateralNodeName"] = targetPath?.LateralNodeName,
                ["lateralNodeId"] = targetPath?.LateralNodeId,
                ["lateralNodeVirtualRootPath"] = targetPath?.LateralNodeVirtualRootPath,
                ["lateralRelativePath"] = targetPath?.LateralRelativePath,
                ["usedLateralShortcut"] = targetPath?.UsedLateralShortcut,
                ["recursive"] = recursive,
                ["didDelete"] = didDelete
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
