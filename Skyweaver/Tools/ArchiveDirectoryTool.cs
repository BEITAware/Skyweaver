using System;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Skyweaver.Controls.ChatSessionControl.Views;
using Skyweaver.Services.SkyweaverTools;

namespace Skyweaver.Tools
{
    public sealed class ArchiveDirectoryTool :
        ISkyweaverTool,
        ISkyweaverToolInvocationPresentationProvider,
        ISkyweaverToolPromptDescriptionProvider
    {
        public const string ToolName = "ArchiveDirectory";

        private static readonly SkyweaverToolDefinition s_definition = new(
            ToolName,
            "Creates a ZIP archive from a specified directory.",
            "Archive",
            [
                new SkyweaverToolParameterDefinition(
                    "SourceDirectoryPath",
                    "The path to the directory to archive. Relative paths resolve against the current workspace.",
                    SkyweaverToolParameterType.String,
                    isRequired: true),
                new SkyweaverToolParameterDefinition(
                    "DestinationFilePath",
                    "The path where the ZIP archive should be created. If omitted, it creates a ZIP file next to the source directory.",
                    SkyweaverToolParameterType.String,
                    isRequired: false)
            ],
            defaultAgentPermission: SkyweaverToolDefaultAgentPermission.Allow);

        public SkyweaverToolDefinition Definition => s_definition;

        public string GetPromptDescription(SkyweaverToolPromptDescriptionContext context)
        {
            ArgumentNullException.ThrowIfNull(context);
            return "Creates a ZIP archive from a specified directory. Both absolute and workspace-relative paths are supported.";
        }

        public FrameworkElement? CreateInvocationPresentation(SkyweaverToolInvocationPresentationContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            return ToolInvocationCardFactory.Create(
                context,
                [
                    new ToolInvocationCardFieldDefinition("Source Directory", "SourceDirectoryPath", "Waiting for source directory..."),
                    new ToolInvocationCardFieldDefinition("Destination", "DestinationFilePath", "Default destination")
                ]);
        }

        public async Task<SkyweaverToolResult> ExecuteAsync(
            SkyweaverToolContext context,
            SkyweaverToolArguments arguments,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var requestedSourcePath = arguments.GetString("SourceDirectoryPath") ?? string.Empty;
            var requestedDestPath = arguments.GetString("DestinationFilePath");

            string resolvedSourcePath;
            try
            {
                resolvedSourcePath = ToolFileSystemHelper.ResolvePath(requestedSourcePath, context.WorkspacePath);
            }
            catch (Exception ex)
            {
                return SkyweaverToolResult.Failure($"Invalid source directory path: {ex.Message}");
            }

            if (!Directory.Exists(resolvedSourcePath))
            {
                return SkyweaverToolResult.Failure($"Source directory not found: {resolvedSourcePath}");
            }

            string resolvedDestPath;
            if (string.IsNullOrWhiteSpace(requestedDestPath))
            {
                resolvedDestPath = $"{resolvedSourcePath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)}.zip";
            }
            else
            {
                try
                {
                    resolvedDestPath = ToolFileSystemHelper.ResolvePath(requestedDestPath, context.WorkspacePath);
                }
                catch (Exception ex)
                {
                    return SkyweaverToolResult.Failure($"Invalid destination path: {ex.Message}");
                }
            }

            try
            {
                if (File.Exists(resolvedDestPath))
                {
                    File.Delete(resolvedDestPath);
                }

                var destDir = Path.GetDirectoryName(resolvedDestPath);
                if (!string.IsNullOrEmpty(destDir))
                {
                    Directory.CreateDirectory(destDir);
                }

                await Task.Run(() =>
                {
                    ZipFile.CreateFromDirectory(resolvedSourcePath, resolvedDestPath, CompressionLevel.Optimal, includeBaseDirectory: false);
                }, cancellationToken);

                return SkyweaverToolResult.Success($"Successfully created archive at:\n{resolvedDestPath}");
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ArchiveDirectoryTool execution failed: {ex}");
                return SkyweaverToolResult.Failure($"Failed to create archive: {ex.Message}");
            }
        }
    }
}
