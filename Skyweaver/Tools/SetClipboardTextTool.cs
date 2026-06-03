using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Skyweaver.Controls.ChatSessionControl.Views;
using Skyweaver.Services.SkyweaverTools;
using Skyweaver.Services;

namespace Skyweaver.Tools
{
    public sealed class SetClipboardTextTool :
        ISkyweaverTool,
        ISkyweaverToolInvocationPresentationProvider,
        ISkyweaverToolPromptDescriptionProvider
    {
        public const string ToolName = "SetClipboardText";

        private static readonly SkyweaverToolDefinition s_definition = new(
            ToolName,
            "Writes text to the system clipboard.",
            "Device",
            [
                new SkyweaverToolParameterDefinition(
                    "Text",
                    "The text to write to the clipboard.",
                    SkyweaverToolParameterType.String,
                    isRequired: true)
            ],
            defaultAgentPermission: SkyweaverToolDefaultAgentPermission.RequireConfirmation);

        public SkyweaverToolDefinition Definition => s_definition;

        public string GetPromptDescription(SkyweaverToolPromptDescriptionContext context)
        {
            ArgumentNullException.ThrowIfNull(context);
            return "Writes the specified text to the system clipboard. Useful for providing output to the user that they can easily paste elsewhere. Requires user confirmation.";
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

        public async Task<SkyweaverToolResult> ExecuteAsync(
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
                    if (ClipboardAccessService.TrySetText(text, out var errorMessage))
                    {
                        tcs.TrySetResult(SkyweaverToolResult.Success("Text successfully copied to clipboard."));
                    }
                    else
                    {
                        tcs.TrySetResult(SkyweaverToolResult.Failure(errorMessage ?? "Failed to set clipboard text. It may be locked by another process."));
                    }
                }
                catch (Exception ex)
                {
                    tcs.TrySetResult(SkyweaverToolResult.Failure($"Exception writing to clipboard: {ex.Message}"));
                }
            });

            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();

            return await tcs.Task.ConfigureAwait(false);
        }
    }
}
