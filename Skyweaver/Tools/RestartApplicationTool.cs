using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Skyweaver.Controls.ChatSessionControl.Views;
using Skyweaver.Services.SkyweaverTools;

namespace Skyweaver.Tools
{
    public sealed class RestartApplicationTool :
        ISkyweaverTool,
        ISkyweaverToolInvocationPresentationProvider,
        ISkyweaverToolPromptDescriptionProvider
    {
        public const string ToolName = "RestartApplication";

        private static readonly SkyweaverToolDefinition s_definition = new(
            ToolName,
            "Restarts the Skyweaver application.",
            "Device",
            [],
            defaultAgentPermission: SkyweaverToolDefaultAgentPermission.RequireConfirmation);

        public SkyweaverToolDefinition Definition => s_definition;

        public string GetPromptDescription(SkyweaverToolPromptDescriptionContext context)
        {
            ArgumentNullException.ThrowIfNull(context);
            return "Restarts the Skyweaver application. Useful for applying settings or recovering from an error state.";
        }

        public FrameworkElement? CreateInvocationPresentation(SkyweaverToolInvocationPresentationContext context)
        {
            ArgumentNullException.ThrowIfNull(context);
            return ToolInvocationCardFactory.Create(context, []);
        }

        public Task<SkyweaverToolResult> ExecuteAsync(
            SkyweaverToolContext context,
            SkyweaverToolArguments arguments,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    var exePath = Process.GetCurrentProcess().MainModule?.FileName;
                    if (!string.IsNullOrEmpty(exePath))
                    {
                        Process.Start(exePath);
                        Application.Current.Shutdown();
                    }
                });
                return Task.FromResult(SkyweaverToolResult.Success("Restarting application..."));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"RestartApplicationTool execution failed: {ex}");
                return Task.FromResult(SkyweaverToolResult.Failure($"Failed to restart application: {ex.Message}"));
            }
        }
    }
}
