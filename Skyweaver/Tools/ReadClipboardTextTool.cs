using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Skyweaver.Controls.ChatSessionControl.Views;
using Skyweaver.Services;
using Skyweaver.Services.SkyweaverTools;

namespace Skyweaver.Tools
{
    public sealed class ReadClipboardTextTool :
        ISkyweaverTool,
        ISkyweaverToolInvocationPresentationProvider,
        ISkyweaverToolPromptDescriptionProvider
    {
        public const string ToolName = "ReadClipboardText";

        private static readonly SkyweaverToolDefinition s_definition = new(
            ToolName,
            "Reads text content currently copied to the system clipboard.",
            "ClipboardText",
            [],
            defaultAgentPermission: SkyweaverToolDefaultAgentPermission.RequireConfirmation);

        public SkyweaverToolDefinition Definition => s_definition;

        public string GetPromptDescription(SkyweaverToolPromptDescriptionContext context)
        {
            ArgumentNullException.ThrowIfNull(context);
            return "Reads text content currently copied to the system clipboard. Useful for accessing data the user has just copied.";
        }

        public FrameworkElement? CreateInvocationPresentation(SkyweaverToolInvocationPresentationContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            return ToolInvocationCardFactory.Create(
                context,
                []);
        }

        public Task<SkyweaverToolResult> ExecuteAsync(
            SkyweaverToolContext context,
            SkyweaverToolArguments arguments,
            CancellationToken cancellationToken = default)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (ClipboardAccessService.TryGetText(out string text, out string? errorMessage))
                {
                    return Task.FromResult(SkyweaverToolResult.Success(text));
                }
                else
                {
                    return Task.FromResult(SkyweaverToolResult.Failure(errorMessage ?? "The clipboard does not contain text or cannot be accessed."));
                }
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ReadClipboardTextTool execution failed: {ex}");
                return Task.FromResult(SkyweaverToolResult.Failure($"Failed to read clipboard: {ex.Message}"));
            }
        }
    }
}
