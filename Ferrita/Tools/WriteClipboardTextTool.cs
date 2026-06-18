using System;
using System.Threading;
using System.Threading.Tasks;
using Ferrita.Services;
using Ferrita.Services.FerritaTools;

namespace Ferrita.Tools
{
    public sealed class WriteClipboardTextTool : IFerritaTool, IFerritaToolPromptDescriptionProvider
    {
        public const string ToolName = "WriteClipboardText";

        private static readonly FerritaToolDefinition s_definition = new(
            ToolName,
            "Writes text to the system clipboard.",
            "System",
            [
                new FerritaToolParameterDefinition(
                    "Text",
                    "The text to write to the clipboard.",
                    FerritaToolParameterType.String,
                    isRequired: true)
            ],
            defaultAgentPermission: FerritaToolDefaultAgentPermission.RequireConfirmation);

        public FerritaToolDefinition Definition => s_definition;

        public string GetPromptDescription(FerritaToolPromptDescriptionContext context)
        {
            return "Writes text to the system clipboard. Use this when the user explicitly asks to copy something to the clipboard.";
        }

        public Task<FerritaToolResult> ExecuteAsync(
            FerritaToolContext context,
            FerritaToolArguments arguments,
            CancellationToken cancellationToken = default)
        {
            try
            {
                string? text = arguments.GetString("Text");
                if (text == null)
                {
                    return Task.FromResult(FerritaToolResult.Failure("Missing or invalid 'Text' parameter."));
                }

                if (ClipboardAccessService.TrySetText(text, out var errorMessage))
                {
                    return Task.FromResult(FerritaToolResult.Success("Text successfully written to the clipboard."));
                }
                else
                {
                    return Task.FromResult(FerritaToolResult.Failure(errorMessage ?? "Failed to write text to clipboard."));
                }
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                return Task.FromResult(FerritaToolResult.Failure($"An unexpected error occurred: {ex.Message}"));
            }
        }
    }
}
