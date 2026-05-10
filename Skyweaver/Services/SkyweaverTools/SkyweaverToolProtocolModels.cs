using System.Xml.Linq;

namespace Skyweaver.Services.SkyweaverTools
{
    public sealed class SkyweaverToolInvocation
    {
        private readonly XElement _sourceElement;

        public SkyweaverToolInvocation(
            string toolName,
            IReadOnlyDictionary<string, string?> rawArguments,
            XElement sourceElement,
            bool isAsyncInvocation = false)
        {
            ToolName = string.IsNullOrWhiteSpace(toolName)
                ? throw new ArgumentException("Tool name cannot be empty.", nameof(toolName))
                : toolName.Trim();
            RawArguments = new Dictionary<string, string?>(
                rawArguments ?? throw new ArgumentNullException(nameof(rawArguments)),
                StringComparer.OrdinalIgnoreCase);
            _sourceElement = new XElement(sourceElement ?? throw new ArgumentNullException(nameof(sourceElement)));
            IsAsyncInvocation = isAsyncInvocation;
        }

        public string ToolName { get; }

        public IReadOnlyDictionary<string, string?> RawArguments { get; }

        public bool IsAsyncInvocation { get; }

        public string InvocationXml => _sourceElement.ToString(SaveOptions.DisableFormatting);

        public XElement ToXElement()
        {
            return new XElement(_sourceElement);
        }
    }

    public sealed class SkyweaverToolReturnPayload
    {
        public SkyweaverToolReturnPayload(
            string toolName,
            SkyweaverToolResult result,
            string? toolCallId = null)
        {
            ToolName = string.IsNullOrWhiteSpace(toolName)
                ? throw new ArgumentException("Tool name cannot be empty.", nameof(toolName))
                : toolName.Trim();
            Result = result ?? throw new ArgumentNullException(nameof(result));
            ToolCallId = string.IsNullOrWhiteSpace(toolCallId)
                ? null
                : Skyweaver.Services.ChatSession.ChatSessionToolCallIdGenerator.Normalize(toolCallId);
        }

        public string ToolName { get; }

        public SkyweaverToolResult Result { get; }

        public string? ToolCallId { get; }

        public bool IsSuccess => Result.IsSuccess;

        public string PrimaryMessage => string.IsNullOrWhiteSpace(Result.Content)
            ? (Result.IsSuccess ? "Tool executed successfully." : "Tool execution failed.")
            : Result.Content;
    }
}
