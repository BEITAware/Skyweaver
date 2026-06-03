using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Skyweaver.Controls.ChatSessionControl.Views;
using Skyweaver.Services.SkyweaverTools;
using Skyweaver.Services;

namespace Skyweaver.Tools
{
    public sealed class GetClipboardTextTool :
        ISkyweaverTool,
        ISkyweaverToolInvocationPresentationProvider,
        ISkyweaverToolPromptDescriptionProvider
    {
        public const string ToolName = "GetClipboardText";

        private static readonly SkyweaverToolDefinition s_definition = new(
            ToolName,
            "Reads the current text content from the system clipboard.",
            "Device",
            [],
            defaultAgentPermission: SkyweaverToolDefaultAgentPermission.RequireConfirmation);

        public SkyweaverToolDefinition Definition => s_definition;

        public string GetPromptDescription(SkyweaverToolPromptDescriptionContext context)
        {
            ArgumentNullException.ThrowIfNull(context);
            return "Reads the current text content from the system clipboard. Useful for accessing data the user has just copied. Requires user confirmation.";
        }

        public FrameworkElement? CreateInvocationPresentation(SkyweaverToolInvocationPresentationContext context)
        {
            ArgumentNullException.ThrowIfNull(context);
            return ToolInvocationCardFactory.Create(context, []);
        }

        public async Task<SkyweaverToolResult> ExecuteAsync(
            SkyweaverToolContext context,
            SkyweaverToolArguments arguments,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var tcs = new TaskCompletionSource<SkyweaverToolResult>();

            var thread = new Thread(() =>
            {
                try
                {
                    if (ClipboardAccessService.TryGetText(out var text, out var errorMessage))
                    {
                        if (string.IsNullOrEmpty(text))
                        {
                            tcs.TrySetResult(SkyweaverToolResult.Success("Clipboard is empty or does not contain text."));
                        }
                        else
                        {
                            tcs.TrySetResult(SkyweaverToolResult.Success(text));
                        }
                    }
                    else
                    {
                        tcs.TrySetResult(SkyweaverToolResult.Failure(errorMessage ?? "Failed to retrieve text from the clipboard. It may be locked by another process or contain non-text data."));
                    }
                }
                catch (Exception ex)
                {
                    tcs.TrySetResult(SkyweaverToolResult.Failure($"Exception reading clipboard: {ex.Message}"));
                }
            });

            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();

            return await tcs.Task.ConfigureAwait(false);
        }
    }
}
