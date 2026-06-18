using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Skyweaver.Controls.ChatSessionControl.Views;
using Skyweaver.Services.SkyweaverTools;

namespace Skyweaver.Tools
{
    public sealed class Base64Tool :
        ISkyweaverTool,
        ISkyweaverToolInvocationPresentationProvider,
        ISkyweaverToolPromptDescriptionProvider
    {
        public const string ToolName = "Base64EncodeDecode";

        private static readonly SkyweaverToolDefinition s_definition = new(
            ToolName,
            "Encodes or decodes text using Base64.",
            "Script",
            [
                new SkyweaverToolParameterDefinition(
                    "Text",
                    "The text to encode or decode.",
                    SkyweaverToolParameterType.String,
                    isRequired: true),
                new SkyweaverToolParameterDefinition(
                    "Mode",
                    "The operation mode: 'Encode' or 'Decode'.",
                    SkyweaverToolParameterType.String,
                    isRequired: true)
            ],
            defaultAgentPermission: SkyweaverToolDefaultAgentPermission.Allow);

        public SkyweaverToolDefinition Definition => s_definition;

        public string GetPromptDescription(SkyweaverToolPromptDescriptionContext context)
        {
            ArgumentNullException.ThrowIfNull(context);
            return "Encodes or decodes a given text string using Base64. Mode must be 'Encode' or 'Decode'. Useful for handling encoded data in strings.";
        }

        public FrameworkElement? CreateInvocationPresentation(SkyweaverToolInvocationPresentationContext context)
        {
            ArgumentNullException.ThrowIfNull(context);
            return ToolInvocationCardFactory.Create(
                context,
                [
                    new ToolInvocationCardFieldDefinition("Mode", "Mode", "Waiting for mode..."),
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
            var mode = arguments.GetString("Mode") ?? string.Empty;

            if (string.IsNullOrWhiteSpace(text))
            {
                return Task.FromResult(SkyweaverToolResult.Failure("Text parameter is required."));
            }

            try
            {
                if (mode.Equals("Encode", StringComparison.OrdinalIgnoreCase))
                {
                    var bytes = Encoding.UTF8.GetBytes(text);
                    var encoded = Convert.ToBase64String(bytes);
                    return Task.FromResult(SkyweaverToolResult.Success(encoded));
                }
                else if (mode.Equals("Decode", StringComparison.OrdinalIgnoreCase))
                {
                    var bytes = Convert.FromBase64String(text);
                    var decoded = Encoding.UTF8.GetString(bytes);
                    return Task.FromResult(SkyweaverToolResult.Success(decoded));
                }
                else
                {
                    return Task.FromResult(SkyweaverToolResult.Failure("Invalid Mode. Must be 'Encode' or 'Decode'."));
                }
            }
            catch (FormatException ex)
            {
                return Task.FromResult(SkyweaverToolResult.Failure($"Invalid Base64 string: {ex.Message}"));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Base64Tool execution failed: {ex}");
                return Task.FromResult(SkyweaverToolResult.Failure($"Failed to process Base64: {ex.Message}"));
            }
        }
    }
}
