using System.IO;
using System.Text;
using System.Windows;
using Skyweaver.Controls.ChatSessionControl.Views;
using Skyweaver.Services.SkyweaverTools;

namespace Skyweaver.Tools
{
    public sealed class FileSystemIONewDirectoryTool :
        ISkyweaverTool,
        ISkyweaverToolConfigurationProvider,
        ISkyweaverToolInvocationPresentationProvider,
        ISkyweaverToolPromptDescriptionProvider
    {
        public const string ToolName = "FileSystemIO_NewDirectory";

        private const string SettingsRootElementName = "FileSystemIONewDirectorySettings";

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
                    new ToolInvocationCardFieldDefinition("Directory", "DirectoryPath", "Waiting for directory path...")
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
                        $"Path points to an existing file, not a directory: {targetPath.ResolvedPath}",
                        BuildData(targetPath, settings, didCreate: false)));
                }

                if (Directory.Exists(targetPath.ResolvedPath))
                {
                    return Task.FromResult(SkyweaverToolResult.Success(
                        BuildNoChangeContent(targetPath, settings),
                        BuildData(targetPath, settings, didCreate: false)));
                }

                Directory.CreateDirectory(targetPath.ResolvedPath);

                return Task.FromResult(SkyweaverToolResult.Success(
                    BuildSuccessContent(targetPath, settings),
                    BuildData(targetPath, settings, didCreate: true)));
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex) when (IsExpectedException(ex))
            {
                return Task.FromResult(SkyweaverToolResult.Failure(
                    $"Failed to create directory: {ex.Message}",
                    BuildData(targetPath, settings, didCreate: false)));
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
                        "The directory path to create. Relative paths resolve against the current workspace. You may also use LateralFS\\NodeName\\relative\\folder; the host resolves that shortcut to the node virtual folder and blocks path traversal outside the node.",
                        SkyweaverToolParameterType.String,
                        isRequired: true)
                ],
                isSystemTool: false);
        }

        private static string BuildDescription(ToolFileSystemPermissionSettings settings)
        {
            return ToolFileSystemMutationSupport.BuildPromptDescription(
                "Creates a directory and any missing parent directories. If the directory already exists, the tool returns success without changing it.",
                settings.PermissionScope);
        }

        private static string BuildSuccessContent(ToolResolvedPathInfo targetPath, ToolFileSystemPermissionSettings settings)
        {
            var builder = new StringBuilder(256);
            ToolFileSystemMutationSupport.AppendPathInfo(builder, targetPath);
            builder.AppendLine($"PermissionScope: {settings.PermissionScope}");
            builder.AppendLine("CreatedDirectory: true");
            return builder.ToString().TrimEnd();
        }

        private static string BuildNoChangeContent(ToolResolvedPathInfo targetPath, ToolFileSystemPermissionSettings settings)
        {
            var builder = new StringBuilder(256);
            ToolFileSystemMutationSupport.AppendPathInfo(builder, targetPath);
            builder.AppendLine($"PermissionScope: {settings.PermissionScope}");
            builder.AppendLine("CreatedDirectory: false");
            builder.AppendLine("NoChangeReason: Directory already exists.");
            return builder.ToString().TrimEnd();
        }

        private static IReadOnlyDictionary<string, object?> BuildData(
            ToolResolvedPathInfo? targetPath,
            ToolFileSystemPermissionSettings settings,
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
