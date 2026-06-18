using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ferrita.Services;
using Ferrita.Services.FerritaTools;

namespace Ferrita.Tools
{
    public sealed class ReadClipboardTextTool : IFerritaTool, IFerritaToolPromptDescriptionProvider
    {
        public const string ToolName = "ReadClipboardText";

        private static readonly FerritaToolDefinition s_definition = new(
            ToolName,
            "Reads text from the system clipboard.",
            "System",
            [],
            defaultAgentPermission: FerritaToolDefaultAgentPermission.RequireConfirmation);

        public FerritaToolDefinition Definition => s_definition;

        public string GetPromptDescription(FerritaToolPromptDescriptionContext context)
        {
            return "Reads text from the system clipboard. Use this when you need to access information the user has copied.";
        }

        public Task<FerritaToolResult> ExecuteAsync(
            FerritaToolContext context,
            FerritaToolArguments arguments,
            CancellationToken cancellationToken = default)
        {
            try
            {
                if (ClipboardAccessService.TryGetText(out string text, out var errorMessage))
                {
                    return Task.FromResult(FerritaToolResult.Success(
                        "Text successfully read from the clipboard.",
                        new Dictionary<string, object?> { ["Text"] = text }
                    ));
                }
                else
                {
                    return Task.FromResult(FerritaToolResult.Failure(errorMessage ?? "Failed to read text from clipboard."));
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
