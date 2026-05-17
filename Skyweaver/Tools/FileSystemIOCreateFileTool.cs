using System.IO;
using System.Text;
using System.Windows;
using Skyweaver.Controls.ChatSessionControl.Views;
using Skyweaver.Services.SkyweaverTools;

namespace Skyweaver.Tools
{
    public sealed class FileSystemIOCreateFileTool :
        ISkyweaverTool,
        ISkyweaverToolConfigurationProvider,
        ISkyweaverToolInvocationPresentationProvider,
        ISkyweaverToolPromptDescriptionProvider
    {
        public const string ToolName = "FileSystemIO_CreateFile";

        private const string SettingsRootElementName = "FileSystemIOCreateFileSettings";

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
                    new ToolInvocationCardFieldDefinition("File", "FilePath", "Waiting for file path...")
                ]);
        }

        public async Task<SkyweaverToolResult> ExecuteAsync(
            SkyweaverToolContext context,
            SkyweaverToolArguments arguments,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var settings = ToolFileSystemPermissionSettings.FromConfiguration(context.CurrentToolConfiguration, SettingsRootElementName);
            var requestedPath = arguments.GetString("FilePath") ?? string.Empty;
            ToolResolvedPathInfo? targetPath = null;

            try
            {
                targetPath = ToolFileSystemMutationSupport.ResolveAuthorizedPath(
                    requestedPath,
                    context.WorkspacePath,
                    settings.PermissionScope);

                if (Directory.Exists(targetPath.ResolvedPath))
                {
                    return SkyweaverToolResult.Failure(
                        $"Path points to an existing directory, not a file: {targetPath.ResolvedPath}",
                        BuildData(targetPath, settings, parentDirectoryCreated: false, didCreate: false));
                }

                if (File.Exists(targetPath.ResolvedPath))
                {
                    return SkyweaverToolResult.Failure(
                        $"File already exists: {targetPath.ResolvedPath}",
                        BuildData(targetPath, settings, parentDirectoryCreated: false, didCreate: false));
                }

                var parentDirectory = Path.GetDirectoryName(targetPath.ResolvedPath);
                var parentDirectoryCreated = false;
                if (!string.IsNullOrWhiteSpace(parentDirectory))
                {
                    if (File.Exists(parentDirectory))
                    {
                        return SkyweaverToolResult.Failure(
                            $"The parent path is an existing file, not a directory: {parentDirectory}",
                            BuildData(targetPath, settings, parentDirectoryCreated: false, didCreate: false));
                    }

                    if (!Directory.Exists(parentDirectory))
                    {
                        Directory.CreateDirectory(parentDirectory);
                        parentDirectoryCreated = true;
                    }
                }

                using (new FileStream(targetPath.ResolvedPath, FileMode.CreateNew, FileAccess.Write, FileShare.None))
                {
                }
                var ragSync = await AerialCityRagToolSync.RefreshFileAsync(
                    targetPath.ResolvedPath,
                    context.WorkspacePath,
                    cancellationToken).ConfigureAwait(false);

                return SkyweaverToolResult.Success(
                    BuildSuccessContent(targetPath, settings, parentDirectoryCreated),
                    AerialCityRagToolSync.WithSyncData(
                        BuildData(targetPath, settings, parentDirectoryCreated, didCreate: true),
                        ragSync));
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex) when (IsExpectedException(ex))
            {
                return SkyweaverToolResult.Failure(
                    $"Failed to create file: {ex.Message}",
                    BuildData(targetPath, settings, parentDirectoryCreated: false, didCreate: false));
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
                        "FilePath",
                        "The file path to create. Relative paths resolve against the current workspace. You may also use LateralFS\\NodeName\\relative\\file.ext; the host resolves that shortcut to the node virtual folder and blocks path traversal outside the node.",
                        SkyweaverToolParameterType.String,
                        isRequired: true)
                ],
                isSystemTool: false);
        }

        private static string BuildDescription(ToolFileSystemPermissionSettings settings)
        {
            return ToolFileSystemMutationSupport.BuildPromptDescription(
                "Creates a new empty file. If the parent directory does not exist, the tool creates it first.",
                settings.PermissionScope);
        }

        private static string BuildSuccessContent(
            ToolResolvedPathInfo targetPath,
            ToolFileSystemPermissionSettings settings,
            bool parentDirectoryCreated)
        {
            var builder = new StringBuilder(256);
            ToolFileSystemMutationSupport.AppendPathInfo(builder, targetPath);
            builder.AppendLine($"PermissionScope: {settings.PermissionScope}");
            builder.AppendLine($"ParentDirectoryCreated: {parentDirectoryCreated}");
            builder.AppendLine("CreatedFile: true");
            return builder.ToString().TrimEnd();
        }

        private static IReadOnlyDictionary<string, object?> BuildData(
            ToolResolvedPathInfo? targetPath,
            ToolFileSystemPermissionSettings settings,
            bool parentDirectoryCreated,
            bool didCreate)
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
                ["parentDirectoryCreated"] = parentDirectoryCreated,
                ["didCreate"] = didCreate
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
