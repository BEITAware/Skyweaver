using System.Xml.Linq;

namespace Ferrita.Services.FerritaTools
{
    public sealed class FerritaToolInvocation
    {
        private readonly XElement _sourceElement;

        public FerritaToolInvocation(
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

    public sealed class FerritaToolReturnPayload
    {
        public FerritaToolReturnPayload(
            string toolName,
            FerritaToolResult result,
            string? toolCallId = null)
        {
            ToolName = string.IsNullOrWhiteSpace(toolName)
                ? throw new ArgumentException("Tool name cannot be empty.", nameof(toolName))
                : toolName.Trim();
            Result = result ?? throw new ArgumentNullException(nameof(result));
            ToolCallId = string.IsNullOrWhiteSpace(toolCallId)
                ? null
                : Ferrita.Services.ChatSession.ChatSessionToolCallIdGenerator.Normalize(toolCallId);
        }

        public string ToolName { get; }

        public FerritaToolResult Result { get; }

        public string? ToolCallId { get; }

        public bool IsSuccess => Result.IsSuccess;

        public string PrimaryMessage => string.IsNullOrWhiteSpace(Result.Content)
            ? (Result.IsSuccess ? "Tool executed successfully." : "Tool execution failed.")
            : Result.Content;
    }
}
