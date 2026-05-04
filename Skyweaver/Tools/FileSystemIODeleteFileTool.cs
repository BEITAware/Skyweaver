using System.IO;
using System.Text;
using System.Windows;
using Newtonsoft.Json.Linq;
using Skyweaver.Controls.ChatSessionControl.Views;
using Skyweaver.Services.SkyweaverTools;

namespace Skyweaver.Tools
{
    public sealed class FileSystemIODeleteFileTool :
        ISkyweaverTool,
        ISkyweaverToolConfigurationProvider,
        ISkyweaverToolInvocationPresentationProvider,
        ISkyweaverToolPromptDescriptionProvider
    {
        public const string ToolName = "FileSystemIO_DeleteFile";

        private const string SettingsRootElementName = "FileSystemIODeleteFileSettings";

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
                    new ToolInvocationCardFieldDefinition("Files", "Files", "Waiting for JSON file list...")
                ]);
        }

        public Task<SkyweaverToolResult> ExecuteAsync(
            SkyweaverToolContext context,
            SkyweaverToolArguments arguments,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var settings = ToolFileSystemPermissionSettings.FromConfiguration(context.CurrentToolConfiguration, SettingsRootElementName);
            var filesJson = arguments.GetJson("Files");
            if (filesJson == null || filesJson.Type != JTokenType.Array)
            {
                return Task.FromResult(SkyweaverToolResult.Failure(
                    "Files parameter is missing or is not a valid JSON array.",
                    BuildData(settings, requestedCount: 0, deletedPaths: [], validationErrors: ["Files parameter is missing or invalid."], partialDeletion: false, completedWithWarnings: false, firstFailure: null)));
            }

            var requestedPaths = new List<string>();
            var validationErrors = new List<string>();

            foreach (var fileToken in filesJson)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (fileToken.Type != JTokenType.String)
                {
                    validationErrors.Add($"Invalid JSON token type: {fileToken.Type}. Expected String.");
                    continue;
                }

                var requestedPath = fileToken.Value<string>();
                if (string.IsNullOrWhiteSpace(requestedPath))
                {
                    validationErrors.Add("A file path was null or empty.");
                    continue;
                }

                requestedPaths.Add(requestedPath);
            }

            if (requestedPaths.Count == 0)
            {
                if (validationErrors.Count == 0)
                {
                    validationErrors.Add("At least one file path is required.");
                }

                return Task.FromResult(SkyweaverToolResult.Failure(
                    "No valid file paths were supplied.",
                    BuildData(settings, requestedCount: 0, deletedPaths: [], validationErrors, partialDeletion: false, completedWithWarnings: false, firstFailure: null)));
            }

            var deletionTargets = new List<ToolResolvedPathInfo>();
            var seenPaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var requestedPath in requestedPaths)
            {
                cancellationToken.ThrowIfCancellationRequested();

                ToolResolvedPathInfo pathInfo;
                try
                {
                    pathInfo = ToolFileSystemMutationSupport.ResolveAuthorizedPath(
                        requestedPath,
                        context.WorkspacePath,
                        settings.PermissionScope);
                }
                catch (Exception ex) when (IsExpectedException(ex))
                {
                    validationErrors.Add($"Failed to authorize path '{requestedPath}': {ex.Message}");
                    continue;
                }

                var comparisonPath = ToolFileSystemMutationSupport.NormalizeComparisonPath(pathInfo.ResolvedPath);
                if (!seenPaths.Add(comparisonPath))
                {
                    validationErrors.Add($"Duplicate file path is not allowed: {pathInfo.ResolvedPath}");
                    continue;
                }

                if (Directory.Exists(pathInfo.ResolvedPath))
                {
                    validationErrors.Add($"Path points to a directory, not a file: {pathInfo.ResolvedPath}");
                    continue;
                }

                if (!File.Exists(pathInfo.ResolvedPath))
                {
                    validationErrors.Add($"File not found: {pathInfo.ResolvedPath}");
                    continue;
                }

                deletionTargets.Add(pathInfo);
            }

            if (validationErrors.Count > 0)
            {
                return Task.FromResult(SkyweaverToolResult.Failure(
                    BuildValidationFailureContent(settings, requestedPaths.Count, validationErrors),
                    BuildData(settings, requestedPaths.Count, [], validationErrors, partialDeletion: false, completedWithWarnings: false, firstFailure: null)));
            }

            var deletedPaths = new List<string>();
            foreach (var target in deletionTargets)
            {
                cancellationToken.ThrowIfCancellationRequested();

                try
                {
                    File.Delete(target.ResolvedPath);
                    deletedPaths.Add(target.ResolvedPath);
                }
                catch (Exception ex) when (IsExpectedException(ex))
                {
                    var firstFailure = $"Failed to delete {target.ResolvedPath}: {ex.Message}";
                    if (deletedPaths.Count > 0)
                    {
                        return Task.FromResult(SkyweaverToolResult.Success(
                            BuildPartialSuccessContent(settings, requestedPaths.Count, deletedPaths, firstFailure),
                            BuildData(settings, requestedPaths.Count, deletedPaths, validationErrors: [], partialDeletion: true, completedWithWarnings: true, firstFailure)));
                    }

                    return Task.FromResult(SkyweaverToolResult.Failure(
                        firstFailure,
                        BuildData(settings, requestedPaths.Count, deletedPaths, validationErrors: [], partialDeletion: false, completedWithWarnings: false, firstFailure)));
                }
            }

            return Task.FromResult(SkyweaverToolResult.Success(
                BuildSuccessContent(settings, deletedPaths),
                BuildData(settings, requestedPaths.Count, deletedPaths, validationErrors: [], partialDeletion: false, completedWithWarnings: false, firstFailure: null)));
        }

        private static SkyweaverToolDefinition BuildDefinition(ToolFileSystemPermissionSettings settings)
        {
            return new SkyweaverToolDefinition(
                ToolName,
                BuildDescription(settings),
                "Script",
                [
                    new SkyweaverToolParameterDefinition(
                        "Files",
                        "A JSON array of file paths to delete. Relative paths resolve against the current workspace. Each element may also use LateralFS\\NodeName\\relative\\file.ext; the host resolves that shortcut to the node virtual folder and blocks path traversal outside the node.",
                        SkyweaverToolParameterType.Json,
                        isRequired: true)
                ],
                isSystemTool: false);
        }

        private static string BuildDescription(ToolFileSystemPermissionSettings settings)
        {
            return ToolFileSystemMutationSupport.BuildPromptDescription(
                "Deletes one or more files. The tool validates every requested path before deleting anything; if validation fails, no files are deleted. If an unexpected runtime error occurs after deletions have started, the tool stops and reports the partial progress explicitly.",
                settings.PermissionScope);
        }

        private static string BuildValidationFailureContent(
            ToolFileSystemPermissionSettings settings,
            int requestedCount,
            IReadOnlyList<string> validationErrors)
        {
            var builder = new StringBuilder(512);
            builder.AppendLine($"PermissionScope: {settings.PermissionScope}");
            builder.AppendLine($"RequestedFiles: {requestedCount}");
            builder.AppendLine("DeletedFiles: 0");
            builder.AppendLine("Validation failed. No files were deleted.");
            builder.AppendLine();
            builder.AppendLine("Errors:");
            foreach (var error in validationErrors)
            {
                builder.AppendLine($"- {error}");
            }

            return builder.ToString().TrimEnd();
        }

        private static string BuildSuccessContent(
            ToolFileSystemPermissionSettings settings,
            IReadOnlyList<string> deletedPaths)
        {
            var builder = new StringBuilder(512);
            builder.AppendLine($"PermissionScope: {settings.PermissionScope}");
            builder.AppendLine($"RequestedFiles: {deletedPaths.Count}");
            builder.AppendLine($"DeletedFiles: {deletedPaths.Count}");
            builder.AppendLine();
            builder.AppendLine("Deleted Paths:");
            foreach (var deletedPath in deletedPaths)
            {
                builder.AppendLine($"- {deletedPath}");
            }

            return builder.ToString().TrimEnd();
        }

        private static string BuildPartialSuccessContent(
            ToolFileSystemPermissionSettings settings,
            int requestedCount,
            IReadOnlyList<string> deletedPaths,
            string firstFailure)
        {
            var builder = new StringBuilder(512);
            builder.AppendLine("Partial delete completed with warnings.");
            builder.AppendLine($"PermissionScope: {settings.PermissionScope}");
            builder.AppendLine($"RequestedFiles: {requestedCount}");
            builder.AppendLine($"DeletedFiles: {deletedPaths.Count}");
            builder.AppendLine($"FirstFailure: {firstFailure}");
            builder.AppendLine();
            builder.AppendLine("Deleted Paths:");
            foreach (var deletedPath in deletedPaths)
            {
                builder.AppendLine($"- {deletedPath}");
            }

            return builder.ToString().TrimEnd();
        }

        private static IReadOnlyDictionary<string, object?> BuildData(
            ToolFileSystemPermissionSettings settings,
            int requestedCount,
            IReadOnlyList<string> deletedPaths,
            IReadOnlyList<string> validationErrors,
            bool partialDeletion,
            bool completedWithWarnings,
            string? firstFailure)
        {
            return new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase)
            {
                ["permissionScope"] = settings.PermissionScope.ToString(),
                ["requestedCount"] = requestedCount,
                ["deletedCount"] = deletedPaths.Count,
                ["deletedPaths"] = deletedPaths.ToArray(),
                ["validationErrors"] = validationErrors.ToArray(),
                ["partialDeletion"] = partialDeletion,
                ["completedWithWarnings"] = completedWithWarnings,
                ["firstFailure"] = firstFailure
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
