using Skyweaver.Controls.ChatSessionControl.Models;
using Skyweaver.Services.AgentLoop;
using Skyweaver.Services.SkyweaverTools;

namespace Skyweaver.Services.ChatSession
{
    public static class ChatSessionInternalToolVisibility
    {
        public static bool IsInternalToolName(string? toolName)
        {
            return string.Equals(
                toolName?.Trim(),
                SkyweaverBuiltInToolNames.Passdown,
                StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsInternalToolXml(string? xml)
        {
            if (string.IsNullOrWhiteSpace(xml))
            {
                return false;
            }

            return HasPassdownToolAttribute(xml, "ToolName") ||
                   HasPassdownToolAttribute(xml, "Name");
        }

        public static bool IsInternalToolToolsReturnXml(string? xml)
        {
            if (string.IsNullOrWhiteSpace(xml) ||
                xml.IndexOf("<ToolsReturn", StringComparison.OrdinalIgnoreCase) < 0)
            {
                return false;
            }

            return HasPassdownToolAttribute(xml, "ToolName") ||
                   HasPassdownToolAttribute(xml, "Name");
        }

        public static bool IsInternalToolPart(ChatMessagePartModel? part)
        {
            if (part == null)
            {
                return false;
            }

            return part.PartType is ChatMessagePartType.ToolCall or ChatMessagePartType.ToolOutput or ChatMessagePartType.Tool
                   && (IsInternalToolName(part.Title) ||
                       IsInternalToolXml(part.Content) ||
                       IsInternalToolToolsReturnXml(part.Content));
        }

        public static bool IsInternalToolRuntimeEvent(ChatSessionRuntimeEvent runtimeEvent)
        {
            ArgumentNullException.ThrowIfNull(runtimeEvent);

            return IsInternalToolName(runtimeEvent.ToolInvocation?.ToolName) ||
                   IsInternalToolName(runtimeEvent.ToolCallSnapshot?.ToolName) ||
                   runtimeEvent.ToolReturns.Any(item => IsInternalToolName(item.ToolName)) ||
                   IsInternalToolXml(runtimeEvent.ToolXml) ||
                   IsInternalToolToolsReturnXml(runtimeEvent.ToolOutputXml);
        }

        public static bool IsInternalToolRuntimeEvent(AgentLoopRuntimeEvent runtimeEvent)
        {
            ArgumentNullException.ThrowIfNull(runtimeEvent);

            return IsInternalToolName(runtimeEvent.ToolInvocation?.ToolName) ||
                   IsInternalToolName(runtimeEvent.ToolCallSnapshot?.ToolName) ||
                   runtimeEvent.ToolReturns.Any(item => IsInternalToolName(item.ToolName)) ||
                   IsInternalToolXml(runtimeEvent.ToolXml) ||
                   IsInternalToolToolsReturnXml(runtimeEvent.ToolOutputXml);
        }

        private static bool HasPassdownToolAttribute(string xml, string attributeName)
        {
            return xml.IndexOf($"{attributeName}=\"{SkyweaverBuiltInToolNames.Passdown}\"", StringComparison.OrdinalIgnoreCase) >= 0 ||
                   xml.IndexOf($"{attributeName}='{SkyweaverBuiltInToolNames.Passdown}'", StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }
}
