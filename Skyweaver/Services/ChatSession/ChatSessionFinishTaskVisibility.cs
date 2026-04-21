using Skyweaver.Controls.ChatSessionControl.Models;
using Skyweaver.Services.AgentLoop;
using Skyweaver.Tools;

namespace Skyweaver.Services.ChatSession
{
    public static class ChatSessionFinishTaskVisibility
    {
        public static bool IsCreateMessageName(string? toolName)
        {
            return string.Equals(toolName?.Trim(), CreateMessageTool.ToolName, StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsFinishTaskName(string? toolName)
        {
            return string.Equals(toolName?.Trim(), FinishTaskTool.ToolName, StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsInternalToolName(string? toolName)
        {
            return IsFinishTaskName(toolName) || IsCreateMessageName(toolName);
        }

        public static bool IsCreateMessageToolXml(string? xml)
        {
            if (string.IsNullOrWhiteSpace(xml))
            {
                return false;
            }

            return xml.IndexOf($"ToolName=\"{CreateMessageTool.ToolName}\"", StringComparison.OrdinalIgnoreCase) >= 0 ||
                   xml.IndexOf($"ToolName='{CreateMessageTool.ToolName}'", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        public static bool IsFinishTaskToolXml(string? xml)
        {
            if (string.IsNullOrWhiteSpace(xml))
            {
                return false;
            }

            return xml.IndexOf($"ToolName=\"{FinishTaskTool.ToolName}\"", StringComparison.OrdinalIgnoreCase) >= 0 ||
                   xml.IndexOf($"ToolName='{FinishTaskTool.ToolName}'", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        public static bool IsInternalToolXml(string? xml)
        {
            return IsFinishTaskToolXml(xml) || IsCreateMessageToolXml(xml);
        }

        public static bool IsFinishTaskToolsReturnXml(string? xml)
        {
            if (string.IsNullOrWhiteSpace(xml))
            {
                return false;
            }

            return xml.IndexOf($"ToolName=\"{FinishTaskTool.ToolName}\"", StringComparison.OrdinalIgnoreCase) >= 0 ||
                   xml.IndexOf($"ToolName='{FinishTaskTool.ToolName}'", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        public static bool IsInternalToolToolsReturnXml(string? xml)
        {
            return IsFinishTaskToolsReturnXml(xml) || IsCreateMessageToolXml(xml);
        }

        public static bool IsInternalToolPart(ChatMessagePartModel? part)
        {
            if (part == null)
            {
                return false;
            }

            return part.PartType is ChatMessagePartType.ToolCall or ChatMessagePartType.ToolOutput or ChatMessagePartType.Tool
                   && (IsInternalToolName(part.Title) || IsInternalToolXml(part.Content) || IsInternalToolToolsReturnXml(part.Content));
        }

        public static bool IsFinishTaskPart(ChatMessagePartModel? part)
        {
            if (part == null)
            {
                return false;
            }

            return part.PartType is ChatMessagePartType.ToolCall or ChatMessagePartType.ToolOutput or ChatMessagePartType.Tool
                   && (IsFinishTaskName(part.Title) || IsFinishTaskToolXml(part.Content) || IsFinishTaskToolsReturnXml(part.Content));
        }

        public static bool IsInternalToolRuntimeEvent(ChatSessionRuntimeEvent runtimeEvent)
        {
            ArgumentNullException.ThrowIfNull(runtimeEvent);

            return IsInternalToolName(runtimeEvent.ToolInvocation?.ToolName) ||
                   runtimeEvent.ToolReturns.Any(item => IsInternalToolName(item.ToolName)) ||
                   IsInternalToolXml(runtimeEvent.ToolXml) ||
                   IsInternalToolToolsReturnXml(runtimeEvent.ToolOutputXml);
        }

        public static bool IsFinishTaskRuntimeEvent(ChatSessionRuntimeEvent runtimeEvent)
        {
            ArgumentNullException.ThrowIfNull(runtimeEvent);

            return IsFinishTaskName(runtimeEvent.ToolInvocation?.ToolName) ||
                   runtimeEvent.ToolReturns.Any(item => IsFinishTaskName(item.ToolName)) ||
                   IsFinishTaskToolXml(runtimeEvent.ToolXml) ||
                   IsFinishTaskToolsReturnXml(runtimeEvent.ToolOutputXml);
        }

        public static bool IsInternalToolRuntimeEvent(AgentLoopRuntimeEvent runtimeEvent)
        {
            ArgumentNullException.ThrowIfNull(runtimeEvent);

            return IsInternalToolName(runtimeEvent.ToolInvocation?.ToolName) ||
                   runtimeEvent.ToolReturns.Any(item => IsInternalToolName(item.ToolName)) ||
                   IsInternalToolXml(runtimeEvent.ToolXml) ||
                   IsInternalToolToolsReturnXml(runtimeEvent.ToolOutputXml);
        }

        public static bool IsFinishTaskRuntimeEvent(AgentLoopRuntimeEvent runtimeEvent)
        {
            ArgumentNullException.ThrowIfNull(runtimeEvent);

            return IsFinishTaskName(runtimeEvent.ToolInvocation?.ToolName) ||
                   runtimeEvent.ToolReturns.Any(item => IsFinishTaskName(item.ToolName)) ||
                   IsFinishTaskToolXml(runtimeEvent.ToolXml) ||
                   IsFinishTaskToolsReturnXml(runtimeEvent.ToolOutputXml);
        }
    }
}
