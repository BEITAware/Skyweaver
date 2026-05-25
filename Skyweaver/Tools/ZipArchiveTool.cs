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
    public sealed class ZipArchiveTool :
        ISkyweaverTool,
        ISkyweaverToolInvocationPresentationProvider,
        ISkyweaverToolPromptDescriptionProvider
    {
        public const string ToolName = "ZipArchive";

        private static readonly SkyweaverToolDefinition s_definition = new(
            ToolName,
            "Creates or extracts ZIP archives. Supports compressing a file or directory into a .zip file, or extracting a .zip file to a directory.",
            "Archive",
            [
                new SkyweaverToolParameterDefinition(
                    "Operation",
                    "The operation to perform: 'Create' or 'Extract'.",
                    SkyweaverToolParameterType.String,
                    isRequired: true),
                new SkyweaverToolParameterDefinition(
                    "SourcePath",
                    "The path of the file or directory to compress, or the path of the .zip file to extract.",
                    SkyweaverToolParameterType.String,
                    isRequired: true),
                new SkyweaverToolParameterDefinition(
                    "DestinationPath",
                    "The path of the output .zip file (for Create) or the output directory (for Extract).",
                    SkyweaverToolParameterType.String,
                    isRequired: true)
            ],
            defaultAgentPermission: SkyweaverToolDefaultAgentPermission.RequireConfirmation);

        public SkyweaverToolDefinition Definition => s_definition;

        public string GetPromptDescription(SkyweaverToolPromptDescriptionContext context)
        {
            ArgumentNullException.ThrowIfNull(context);
            return "Creates or extracts ZIP archives. Set Operation to 'Create' to compress SourcePath (file or directory) into DestinationPath (.zip). Set Operation to 'Extract' to uncompress SourcePath (.zip) into DestinationPath (directory).";
        }

        public FrameworkElement? CreateInvocationPresentation(SkyweaverToolInvocationPresentationContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            return ToolInvocationCardFactory.Create(
                context,
                [
                    new ToolInvocationCardFieldDefinition("Operation", "Operation", "Create or Extract"),
                    new ToolInvocationCardFieldDefinition("Source", "SourcePath", "Source path"),
                    new ToolInvocationCardFieldDefinition("Destination", "DestinationPath", "Destination path")
                ]);
        }

        public async Task<SkyweaverToolResult> ExecuteAsync(
            SkyweaverToolContext context,
            SkyweaverToolArguments arguments,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var operation = arguments.GetString("Operation")?.Trim() ?? string.Empty;
            var sourcePathRaw = arguments.GetString("SourcePath")?.Trim() ?? string.Empty;
            var destinationPathRaw = arguments.GetString("DestinationPath")?.Trim() ?? string.Empty;

            if (string.IsNullOrEmpty(operation) || string.IsNullOrEmpty(sourcePathRaw) || string.IsNullOrEmpty(destinationPathRaw))
            {
                return SkyweaverToolResult.Failure("Operation, SourcePath, and DestinationPath are required.");
            }

            string resolvedSource;
            string resolvedDestination;

            try
            {
                resolvedSource = ToolFileSystemHelper.ResolvePath(sourcePathRaw, context.WorkspacePath);
                resolvedDestination = ToolFileSystemHelper.ResolvePath(destinationPathRaw, context.WorkspacePath);
            }
            catch (Exception ex)
            {
                return SkyweaverToolResult.Failure($"Failed to resolve paths: {ex.Message}");
            }

            try
            {
                if (string.Equals(operation, "Create", StringComparison.OrdinalIgnoreCase))
                {
                    return await CreateArchiveAsync(resolvedSource, resolvedDestination, cancellationToken).ConfigureAwait(false);
                }
                else if (string.Equals(operation, "Extract", StringComparison.OrdinalIgnoreCase))
                {
                    return await ExtractArchiveAsync(resolvedSource, resolvedDestination, cancellationToken).ConfigureAwait(false);
                }
                else
                {
                    return SkyweaverToolResult.Failure($"Invalid operation: {operation}. Supported operations are 'Create' and 'Extract'.");
                }
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ZipArchiveTool execution failed: {ex}");
                return SkyweaverToolResult.Failure($"Operation '{operation}' failed: {ex.Message}");
            }
        }

        private async Task<SkyweaverToolResult> CreateArchiveAsync(string sourcePath, string destinationPath, CancellationToken cancellationToken)
        {
            if (File.Exists(destinationPath))
            {
                return SkyweaverToolResult.Failure($"Destination file already exists: {destinationPath}");
            }

            var destinationDir = Path.GetDirectoryName(destinationPath);
            if (!string.IsNullOrEmpty(destinationDir) && !Directory.Exists(destinationDir))
            {
                Directory.CreateDirectory(destinationDir);
            }

            return await Task.Run(() =>
            {
                if (Directory.Exists(sourcePath))
                {
                    ZipFile.CreateFromDirectory(sourcePath, destinationPath, CompressionLevel.Optimal, false);
                }
                else if (File.Exists(sourcePath))
                {
                    using var archive = ZipFile.Open(destinationPath, ZipArchiveMode.Create);
                    archive.CreateEntryFromFile(sourcePath, Path.GetFileName(sourcePath));
                }
                else
                {
                    return SkyweaverToolResult.Failure($"Source path not found: {sourcePath}");
                }

                return SkyweaverToolResult.Success($"Successfully created archive at: {destinationPath}");
            }, cancellationToken).ConfigureAwait(false);
        }

        private async Task<SkyweaverToolResult> ExtractArchiveAsync(string sourcePath, string destinationPath, CancellationToken cancellationToken)
        {
            if (!File.Exists(sourcePath))
            {
                return SkyweaverToolResult.Failure($"Source archive not found: {sourcePath}");
            }

            if (!Directory.Exists(destinationPath))
            {
                Directory.CreateDirectory(destinationPath);
            }

            return await Task.Run(() =>
            {
                ZipFile.ExtractToDirectory(sourcePath, destinationPath, true);
                return SkyweaverToolResult.Success($"Successfully extracted archive to: {destinationPath}");
            }, cancellationToken).ConfigureAwait(false);
        }
    }
}
