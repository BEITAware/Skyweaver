using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Ferrita.Controls.ChatSessionControl.Views;
using Ferrita.Services.FerritaTools;

namespace Ferrita.Tools
{
    public sealed class OpenDirectoryTool :
        IFerritaTool,
        IFerritaToolInvocationPresentationProvider,
        IFerritaToolPromptDescriptionProvider
    {
        public const string ToolName = "OpenDirectory";

        private static readonly FerritaToolDefinition s_definition = new(
            ToolName,
            "Opens a specific directory on the host machine using the default file explorer.",
            "FolderOpen", // Use a generic folder icon name that exists, like "FolderOpen"
            [
                new FerritaToolParameterDefinition(
                    "DirectoryPath",
                    "The path of the directory to open. Relative paths resolve against the current workspace.",
                    FerritaToolParameterType.String,
                    isRequired: true)
            ],
            defaultAgentPermission: FerritaToolDefaultAgentPermission.RequireConfirmation); // Requires confirmation as it opens UI on host

        public FerritaToolDefinition Definition => s_definition;

        public string GetPromptDescription(FerritaToolPromptDescriptionContext context)
        {
            ArgumentNullException.ThrowIfNull(context);
            return "Opens a directory on the host machine using the default file explorer (like Windows Explorer or macOS Finder). Relative paths are resolved against the current workspace.";
        }

        public FrameworkElement? CreateInvocationPresentation(FerritaToolInvocationPresentationContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            return ToolInvocationCardFactory.Create(
                context,
                [
                    new ToolInvocationCardFieldDefinition("Directory Path", "DirectoryPath", "Waiting for directory path...")
                ]);
        }

        public Task<FerritaToolResult> ExecuteAsync(
            FerritaToolContext context,
            FerritaToolArguments arguments,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var requestedPath = arguments.GetString("DirectoryPath") ?? string.Empty;
            string resolvedPath;

            try
            {
                resolvedPath = ToolFileSystemHelper.ResolvePath(requestedPath, context.WorkspacePath);
            }
            catch (Exception ex)
            {
                return Task.FromResult(FerritaToolResult.Failure($"Invalid directory path: {ex.Message}"));
            }

            if (!Directory.Exists(resolvedPath))
            {
                if (File.Exists(resolvedPath))
                {
                    return Task.FromResult(FerritaToolResult.Failure($"Path points to a file, not a directory: {resolvedPath}"));
                }
                return Task.FromResult(FerritaToolResult.Failure($"Directory not found: {resolvedPath}"));
            }

            try
            {
                // Open directory using default shell execute, safe and cross-platform way to open folders
                var processStartInfo = new ProcessStartInfo
                {
                    FileName = resolvedPath,
                    UseShellExecute = true
                };

                Process.Start(processStartInfo);

                return Task.FromResult(FerritaToolResult.Success($"Successfully opened directory: {resolvedPath}"));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"OpenDirectoryTool execution failed: {ex}");
                return Task.FromResult(FerritaToolResult.Failure($"Failed to open directory: {ex.Message}"));
            }
        }
    }
}
