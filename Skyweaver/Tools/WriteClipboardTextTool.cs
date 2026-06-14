using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Skyweaver.Controls.ChatSessionControl.Views;
using Skyweaver.Services.SkyweaverTools;

namespace Skyweaver.Tools
{
    public sealed class WriteClipboardTextTool :
        ISkyweaverTool,
        ISkyweaverToolInvocationPresentationProvider,
        ISkyweaverToolPromptDescriptionProvider
    {
        public const string ToolName = "WriteClipboardText";

        private static readonly SkyweaverToolDefinition s_definition = new(
            ToolName,
            "Writes text content to the system clipboard.",
            "ClipboardText", // icon name
            [
                new SkyweaverToolParameterDefinition(
                    "Text",
                    "The text content to copy to the clipboard.",
                    SkyweaverToolParameterType.String,
                    isRequired: true)
            ],
            defaultAgentPermission: SkyweaverToolDefaultAgentPermission.RequireConfirmation);

        public SkyweaverToolDefinition Definition => s_definition;

        public string GetPromptDescription(SkyweaverToolPromptDescriptionContext context)
        {
            ArgumentNullException.ThrowIfNull(context);
            return "Writes text content to the system clipboard. Useful for copying generated content or data for the user to paste elsewhere.";
        }

        public FrameworkElement? CreateInvocationPresentation(SkyweaverToolInvocationPresentationContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            return ToolInvocationCardFactory.Create(
                context,
                [
                    new ToolInvocationCardFieldDefinition("Text", "Text", "Waiting for text...")
                ]);
        }

        public Task<SkyweaverToolResult> ExecuteAsync(
            SkyweaverToolContext context,
            SkyweaverToolArguments arguments,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var text = arguments.GetString("Text") ?? string.Empty;
            var tcs = new TaskCompletionSource<SkyweaverToolResult>();

            var thread = new Thread(() =>
            {
                try
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        tcs.TrySetCanceled(cancellationToken);
                        return;
                    }

                    if (Skyweaver.Services.ClipboardAccessService.TrySetText(text, out string? errorMessage))
                    {
                        tcs.TrySetResult(SkyweaverToolResult.Success("Text copied to clipboard successfully."));
                    }
                    else
                    {
                        tcs.TrySetResult(SkyweaverToolResult.Failure($"Failed to write to clipboard: {errorMessage}"));
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"WriteClipboardTextTool execution failed: {ex}");
                    tcs.TrySetResult(SkyweaverToolResult.Failure($"Failed to write to clipboard: {ex.Message}"));
                }
            });

            thread.SetApartmentState(ApartmentState.STA);
            thread.IsBackground = true;
            thread.Start();

            // Handle external cancellation to avoid hanging if the STA thread gets stuck
            cancellationToken.Register(() => tcs.TrySetCanceled(cancellationToken));

            return tcs.Task;
        }
    }
}
