using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Skyweaver.Controls.ChatSessionControl.Views;
using Skyweaver.Services;
using Skyweaver.Services.SkyweaverTools;

namespace Skyweaver.Tools
{
    public sealed class SetClipboardTextTool :
        ISkyweaverTool,
        ISkyweaverToolInvocationPresentationProvider,
        ISkyweaverToolPromptDescriptionProvider
    {
        public const string ToolName = "System_SetClipboardText";

        private static readonly SkyweaverToolDefinition s_definition = new(
            ToolName,
            "Copies the specified text to the host system clipboard. The user must grant permission to write to their clipboard.",
            "Copy",
            [
                new SkyweaverToolParameterDefinition(
                    "Text",
                    "The text to copy to the clipboard.",
                    SkyweaverToolParameterType.String,
                    isRequired: true)
            ],
            isSystemTool: false,
            canBelongToToolKit: true,
            defaultAgentPermission: SkyweaverToolDefaultAgentPermission.RequireConfirmation);

        public SkyweaverToolDefinition Definition => s_definition;

        public string GetPromptDescription(SkyweaverToolPromptDescriptionContext context)
        {
            ArgumentNullException.ThrowIfNull(context);
            return "Copies the provided text into the user's system clipboard. This should be used when the user explicitly requests you to copy something to their clipboard, or when providing a convenient snippet they need to paste elsewhere. It requires user confirmation for security.";
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

            try
            {
                var text = arguments.GetString("Text") ?? string.Empty;

                if (ClipboardAccessService.TrySetText(text, out var errorMessage))
                {
                    return Task.FromResult(SkyweaverToolResult.Success(
                        "Successfully copied text to the clipboard.",
                        new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase)
                        {
                            ["textLength"] = text.Length
                        }));
                }

                return Task.FromResult(SkyweaverToolResult.Failure(
                    $"Failed to copy text to clipboard. {errorMessage}",
                    new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase)
                    {
                        ["textLength"] = text.Length,
                        ["errorMessage"] = errorMessage
                    }));
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                return Task.FromResult(SkyweaverToolResult.Failure($"Unexpected error writing to clipboard: {ex.Message}"));
            }
        }
    }
}
