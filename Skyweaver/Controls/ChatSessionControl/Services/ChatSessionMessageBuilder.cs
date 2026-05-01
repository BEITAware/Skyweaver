using Skyweaver.Controls.ChatSessionControl.Models;
using Skyweaver.Services.SkyweaverTools;

namespace Skyweaver.Controls.ChatSessionControl.Services
{
    public sealed class ChatSessionMessageBuilder
    {
        private readonly ChatMessageModel _message;
        private readonly ToolInvocationPresentationService _toolInvocationPresentationService;
        private readonly Dictionary<ToolCallInstanceKey, ChatMessagePartModel> _toolParts = new();
        private readonly Dictionary<ToolCallInstanceKey, ChatMessagePartModel> _toolOutputParts = new();
        private ChatMessagePartModel? _activeReplyPayloadPart;
        private ChatMessagePartModel? _activeReasoningPart;
        private ChatMessagePartModel? _streamingTextPart;

        public ChatSessionMessageBuilder(
            ChatMessageModel message,
            ToolInvocationPresentationService? toolInvocationPresentationService = null)
        {
            _message = message ?? throw new ArgumentNullException(nameof(message));
            _toolInvocationPresentationService = toolInvocationPresentationService ?? new ToolInvocationPresentationService();
        }

        public ChatMessagePartModel AppendTextDelta(string? textDelta)
        {
            if (string.IsNullOrEmpty(textDelta))
            {
                return _streamingTextPart ?? EnsureStreamingTextPart();
            }

            var part = EnsureStreamingTextPart();
            part.Content += textDelta;
            return part;
        }

        public ChatMessagePartModel AppendReasoningDelta(string? textDelta)
        {
            if (string.IsNullOrEmpty(textDelta))
            {
                return _activeReasoningPart ?? EnsureReasoningPart();
            }

            var part = EnsureReasoningPart();
            part.Content += textDelta;
            return part;
        }

        public ChatMessagePartModel AddToolCall(
            ToolCallInstanceKey toolCallKey,
            SkyweaverToolInvocation invocation,
            bool isStreaming = false,
            string? toolCallId = null,
            string? callerAgentId = null)
        {
            ArgumentNullException.ThrowIfNull(invocation);

            CompleteTextStreaming();

            var part = EnsureToolCallPart(toolCallKey, invocation.ToolName);
            ApplyToolMetadata(part, toolCallId, callerAgentId);
            part.PartType = ChatMessagePartType.ToolCall;
            part.Title = invocation.ToolName;
            part.BadgeText = "工具调用";
            part.IsStreaming = isStreaming;
            part.Content = invocation.InvocationXml;
            return part;
        }

        public ChatMessagePartModel AddOrUpdateToolCall(
            ToolCallInstanceKey toolCallKey,
            SkyweaverStreamingToolCallSnapshot snapshot,
            string? toolCallId = null,
            string? callerAgentId = null)
        {
            ArgumentNullException.ThrowIfNull(snapshot);

            CompleteTextStreaming();

            var part = EnsureToolCallPart(toolCallKey, snapshot.ToolName);
            ApplyToolMetadata(part, toolCallId, callerAgentId);
            part.PartType = ChatMessagePartType.ToolCall;
            part.Title = snapshot.ToolName;
            part.BadgeText = "工具调用";
            part.IsStreaming = true;
            part.Content = snapshot.ToolXmlFragment ?? string.Empty;
            part.ToolPresentationState?.ApplySnapshot(snapshot);
            return part;
        }

        public ChatMessagePartModel AddTextPart(string? text, string? title = null)
        {
            CompleteTextStreaming();

            var part = ChatMessagePartModel.CreateText(text ?? string.Empty, title);
            _message.Parts.Add(part);
            return part;
        }

        public ChatMessagePartModel CompleteToolCall(
            ToolCallInstanceKey toolCallKey,
            string toolOutputXml,
            string? toolCallId = null,
            string? callerAgentId = null)
        {
            if (!_toolParts.TryGetValue(toolCallKey, out var part))
            {
                var fallbackOutputPart = ChatMessagePartModel.CreateToolOutput(
                    toolOutputXml,
                    $"Tool #{toolCallKey.ToolCallIndex}");
                ApplyToolMetadata(fallbackOutputPart, toolCallId, callerAgentId);
                _toolOutputParts[toolCallKey] = fallbackOutputPart;
                _message.Parts.Add(fallbackOutputPart);
                return fallbackOutputPart;
            }

            ApplyToolMetadata(part, toolCallId, callerAgentId);
            part.PartType = ChatMessagePartType.ToolCall;
            part.BadgeText = "工具调用";
            part.IsStreaming = false;
            if (part.ToolPresentationState != null)
            {
                part.ToolPresentationState.IsInvocationClosed = true;
            }

            var existingIndex = _message.Parts.IndexOf(part);
            var outputPart = EnsureToolOutputPart(
                toolCallKey,
                string.IsNullOrWhiteSpace(part.Title)
                    ? $"Tool #{toolCallKey.ToolCallIndex}"
                    : part.Title,
                existingIndex < 0 ? null : existingIndex + 1);
            ApplyToolMetadata(outputPart, toolCallId, callerAgentId);
            outputPart.PartType = ChatMessagePartType.ToolOutput;
            outputPart.BadgeText = "工具输出";
            outputPart.IsStreaming = false;
            outputPart.Content = toolOutputXml ?? string.Empty;
            return outputPart;
        }

        public ChatMessagePartModel AddMalformedToolCall(
            ToolCallInstanceKey toolCallKey,
            string content,
            string? errorMessage,
            string? toolCallId = null,
            string? callerAgentId = null)
        {
            CompleteTextStreaming();

            var normalizedContent = string.IsNullOrWhiteSpace(errorMessage)
                ? content
                : $"{errorMessage}{Environment.NewLine}{Environment.NewLine}{content}";

            if (_toolParts.TryGetValue(toolCallKey, out var part))
            {
                ApplyToolMetadata(part, toolCallId, callerAgentId);
                part.PartType = ChatMessagePartType.ToolOutput;
                part.Title = "工具解析错误";
                part.BadgeText = "工具输出";
                part.IsStreaming = false;
                part.IsUserVisible = true;
                part.Content = normalizedContent;
                return part;
            }

            part = ChatMessagePartModel.CreateToolOutput(
                normalizedContent,
                "工具解析错误",
                isUserVisible: true);
            ApplyToolMetadata(part, toolCallId, callerAgentId);
            _toolParts[toolCallKey] = part;
            _message.Parts.Add(part);
            return part;
        }

        public ChatMessagePartModel AddStructuredXml(string xmlText, string? title = null)
        {
            CompleteTextStreaming();

            var part = ChatMessagePartModel.CreateStructuredXml(xmlText, title ?? "结构化 XML");
            _message.Parts.Add(part);
            return part;
        }

        public ChatMessagePartModel AppendReplyTextDelta(string? text, string? title = null)
        {
            return AppendReplyPayloadDelta(
                ChatMessagePartType.Text,
                text ?? string.Empty,
                title,
                language: null,
                badgeText: null);
        }

        public ChatMessagePartModel AppendReplyStructuredXmlDelta(string? xmlText, string? title = null)
        {
            return AppendReplyPayloadDelta(
                ChatMessagePartType.StructuredXml,
                xmlText ?? string.Empty,
                title ?? "回复 XML",
                language: null,
                badgeText: "XML");
        }

        public ChatMessagePartModel SetReplyText(string? text, string? title = null)
        {
            return CommitReplyPayload(
                ChatMessagePartType.Text,
                text ?? string.Empty,
                title,
                language: null,
                badgeText: null);
        }

        public ChatMessagePartModel SetReplyStructuredXml(string xmlText, string? title = null)
        {
            return CommitReplyPayload(
                ChatMessagePartType.StructuredXml,
                xmlText ?? string.Empty,
                title ?? "回复 XML",
                language: null,
                badgeText: "XML");
        }

        public void CompleteTextStreaming()
        {
            if (_streamingTextPart == null)
            {
                return;
            }

            _streamingTextPart.IsStreaming = false;
            _streamingTextPart = null;
        }

        public void CompleteReplyStreaming()
        {
            if (_activeReplyPayloadPart == null)
            {
                return;
            }

            _activeReplyPayloadPart.IsStreaming = false;
            _activeReplyPayloadPart = null;
        }

        public void CompleteReasoningStreaming()
        {
            if (_activeReasoningPart == null)
            {
                return;
            }

            _activeReasoningPart.IsStreaming = false;
            _activeReasoningPart = null;
        }

        public void ApplyStreamingPart(AssistantStreamingPart part)
        {
            ArgumentNullException.ThrowIfNull(part);

            switch (part.Kind)
            {
                case AssistantStreamingPartKind.Text:
                    AppendTextDelta(part.Content);
                    break;
                case AssistantStreamingPartKind.ToolCall when part.ToolInvocation != null:
                    AddToolCall(
                        ToolCallInstanceKey.Create(
                            iterationNumber: null,
                            partIndex: null,
                            toolCallIndex: part.ToolCallIndex),
                        part.ToolInvocation,
                        isStreaming: true);
                    break;
                case AssistantStreamingPartKind.MalformedToolCall:
                    AddMalformedToolCall(
                        ToolCallInstanceKey.Create(
                            iterationNumber: null,
                            partIndex: null,
                            toolCallIndex: part.ToolCallIndex),
                        part.Content,
                        part.ErrorMessage);
                    break;
            }
        }

        public void FinalizeOpenToolCalls(string? message)
        {
            foreach (var item in _toolParts
                         .Where(item => item.Value.PartType == ChatMessagePartType.ToolCall)
                         .ToArray())
            {
                var toolCallKey = item.Key;
                var toolPart = item.Value;
                var existingIndex = _message.Parts.IndexOf(toolPart);
                toolPart.PartType = ChatMessagePartType.ToolCall;
                toolPart.BadgeText = "工具调用";
                toolPart.IsStreaming = false;
                if (toolPart.ToolPresentationState != null)
                {
                    toolPart.ToolPresentationState.IsInvocationClosed = true;
                }

                var outputPart = EnsureToolOutputPart(
                    toolCallKey,
                    string.IsNullOrWhiteSpace(toolPart.Title)
                        ? $"Tool #{toolCallKey.ToolCallIndex}"
                        : toolPart.Title,
                    existingIndex < 0 ? null : existingIndex + 1);
                outputPart.PartType = ChatMessagePartType.ToolOutput;
                outputPart.BadgeText = "工具输出";
                outputPart.IsStreaming = false;
                outputPart.Content = message ?? string.Empty;
            }
        }

        private ChatMessagePartModel EnsureStreamingTextPart()
        {
            if (_streamingTextPart != null)
            {
                return _streamingTextPart;
            }

            _streamingTextPart = ChatMessagePartModel.CreateText(string.Empty);
            _streamingTextPart.IsStreaming = true;
            _message.Parts.Add(_streamingTextPart);
            return _streamingTextPart;
        }

        private ChatMessagePartModel EnsureReasoningPart()
        {
            if (_activeReasoningPart != null)
            {
                _activeReasoningPart.IsStreaming = true;
                return _activeReasoningPart;
            }

            _activeReasoningPart = ChatMessagePartModel.CreateReasoning(string.Empty, isStreaming: true);
            _message.Parts.Add(_activeReasoningPart);
            return _activeReasoningPart;
        }

        private ChatMessagePartModel EnsureToolCallPart(ToolCallInstanceKey toolCallKey, string? toolName)
        {
            if (_toolParts.TryGetValue(toolCallKey, out var existingPart))
            {
                EnsureToolPresentation(existingPart, toolCallKey, toolName);
                return existingPart;
            }

            var part = ChatMessagePartModel.CreateToolCall(string.Empty, toolName, isStreaming: true);
            EnsureToolPresentation(part, toolCallKey, toolName);

            _toolParts[toolCallKey] = part;
            _message.Parts.Add(part);
            return part;
        }

        private ChatMessagePartModel EnsureToolOutputPart(
            ToolCallInstanceKey toolCallKey,
            string? title,
            int? insertIndex = null)
        {
            if (_toolOutputParts.TryGetValue(toolCallKey, out var existingPart))
            {
                existingPart.Title = title;
                return existingPart;
            }

            var part = ChatMessagePartModel.CreateToolOutput(string.Empty, title);
            _toolOutputParts[toolCallKey] = part;
            if (insertIndex is int index && index >= 0 && index <= _message.Parts.Count)
            {
                _message.Parts.Insert(index, part);
            }
            else
            {
                _message.Parts.Add(part);
            }

            return part;
        }

        private void EnsureToolPresentation(
            ChatMessagePartModel part,
            ToolCallInstanceKey toolCallKey,
            string? toolName)
        {
            ArgumentNullException.ThrowIfNull(part);

            if (part.ToolPresentationState != null && part.ToolPresentationView != null)
            {
                if (!string.IsNullOrWhiteSpace(toolName))
                {
                    part.ToolPresentationState.ToolName = toolName.Trim();
                }

                return;
            }

            var handle = _toolInvocationPresentationService.CreatePresentation(
                toolCallKey.ToolCallIndex,
                toolName);
            part.AttachToolPresentation(handle.State, handle.View);
        }

        private static void ApplyToolMetadata(
            ChatMessagePartModel part,
            string? toolCallId,
            string? callerAgentId)
        {
            if (!string.IsNullOrWhiteSpace(toolCallId))
            {
                part.ToolCallId = toolCallId;
            }

            if (!string.IsNullOrWhiteSpace(callerAgentId))
            {
                part.CallerAgentId = callerAgentId;
            }
        }

        private ChatMessagePartModel AppendReplyPayloadDelta(
            ChatMessagePartType partType,
            string content,
            string? title,
            string? language,
            string? badgeText)
        {
            CompleteTextStreaming();

            if (_activeReplyPayloadPart == null ||
                !_activeReplyPayloadPart.IsStreaming ||
                _activeReplyPayloadPart.PartType != partType)
            {
                _activeReplyPayloadPart = new ChatMessagePartModel(
                    partType,
                    string.Empty,
                    title,
                    language,
                    badgeText,
                    isStreaming: true);
                _message.Parts.Add(_activeReplyPayloadPart);
            }
            else
            {
                _activeReplyPayloadPart.Title = title;
                _activeReplyPayloadPart.Language = language;
                _activeReplyPayloadPart.BadgeText = badgeText;
            }

            if (!string.IsNullOrEmpty(content))
            {
                _activeReplyPayloadPart.Content += content;
            }

            return _activeReplyPayloadPart;
        }

        private ChatMessagePartModel CommitReplyPayload(
            ChatMessagePartType partType,
            string content,
            string? title,
            string? language,
            string? badgeText)
        {
            CompleteTextStreaming();

            if (_activeReplyPayloadPart != null &&
                _activeReplyPayloadPart.IsStreaming &&
                _activeReplyPayloadPart.PartType == partType)
            {
                _activeReplyPayloadPart.PartType = partType;
                _activeReplyPayloadPart.Content = content;
                _activeReplyPayloadPart.Title = title;
                _activeReplyPayloadPart.Language = language;
                _activeReplyPayloadPart.BadgeText = badgeText;
                _activeReplyPayloadPart.IsStreaming = false;

                var committedPart = _activeReplyPayloadPart;
                _activeReplyPayloadPart = null;
                return committedPart;
            }

            var part = new ChatMessagePartModel(
                partType,
                content,
                title,
                language,
                badgeText);
            _message.Parts.Add(part);
            _activeReplyPayloadPart = null;
            return part;
        }
    }
}
