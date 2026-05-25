using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Skyweaver.Controls.ChatSessionControl.Views;
using Skyweaver.Services.SkyweaverTools;

namespace Skyweaver.Tools
{
    public sealed class OpenDirectoryInExplorerTool :
        ISkyweaverTool,
        ISkyweaverToolInvocationPresentationProvider,
        ISkyweaverToolPromptDescriptionProvider
    {
        public const string ToolName = "OpenDirectoryInExplorer";

        private static readonly SkyweaverToolDefinition s_definition = new(
            ToolName,
            "Opens a specified directory in the system's default file explorer.",
            "FolderOpen",
            [
                new SkyweaverToolParameterDefinition(
                    "DirectoryPath",
                    "The path of the directory to open.",
                    SkyweaverToolParameterType.String,
                    isRequired: true)
            ],
            defaultAgentPermission: SkyweaverToolDefaultAgentPermission.RequireConfirmation);

        public SkyweaverToolDefinition Definition => s_definition;

        public string GetPromptDescription(SkyweaverToolPromptDescriptionContext context)
        {
            ArgumentNullException.ThrowIfNull(context);
            return "Opens a directory in the host OS's default file explorer. Useful when the user wants to visually inspect a folder. The path must point to an existing directory.";
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

            var requestedPath = arguments.GetString("DirectoryPath")?.Trim() ?? string.Empty;

            if (string.IsNullOrEmpty(requestedPath))
            {
                return Task.FromResult(SkyweaverToolResult.Failure("DirectoryPath is required."));
            }

            string resolvedPath;

            try
            {
                resolvedPath = ToolFileSystemHelper.ResolvePath(requestedPath, context.WorkspacePath);
            }
            catch (Exception ex)
            {
                return Task.FromResult(SkyweaverToolResult.Failure($"Failed to resolve path: {ex.Message}"));
            }

            if (!Directory.Exists(resolvedPath))
            {
                return Task.FromResult(SkyweaverToolResult.Failure($"Directory not found: {resolvedPath}"));
            }

            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = resolvedPath,
                    UseShellExecute = true,
                    Verb = "open"
                });

                return Task.FromResult(SkyweaverToolResult.Success($"Successfully opened directory: {resolvedPath}"));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"OpenDirectoryInExplorerTool execution failed: {ex}");
                return Task.FromResult(SkyweaverToolResult.Failure($"Failed to open directory: {ex.Message}"));
            }
        }
    }
}
