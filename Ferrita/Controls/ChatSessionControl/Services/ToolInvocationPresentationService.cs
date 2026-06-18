using System.Linq;
using System.Windows;
using Ferrita.Controls.ChatSessionControl.Models;
using Ferrita.Controls.ChatSessionControl.Views;
using Ferrita.Services.FerritaTools;

namespace Ferrita.Controls.ChatSessionControl.Services
{
    public sealed class ToolInvocationPresentationService
    {
        private const string DefaultToolIconPath = "pack://application:,,,/Resources/Script.png";

        private readonly Dictionary<string, FerritaToolRegistration> _registrations;
        private readonly FerritaToolInvocationStreamingParser _streamingParser;

        public ToolInvocationPresentationService()
            : this(new FerritaToolManager())
        {
        }

        public ToolInvocationPresentationService(FerritaToolManager toolManager)
        {
            ArgumentNullException.ThrowIfNull(toolManager);

            var registrations = toolManager.GetRegisteredTools().ToArray();
            _registrations = registrations
                .ToDictionary(registration => registration.Definition.Name, StringComparer.OrdinalIgnoreCase);
            _streamingParser = new FerritaToolInvocationStreamingParser(
                registrations.Select(registration => registration.Definition));
        }

        public ToolInvocationPresentationHandle CreatePresentation(int toolCallIndex, string? toolName)
        {
            return CreatePresentation(toolCallIndex, toolName, preferConfirmationPresentation: false);
        }

        public ToolInvocationPresentationHandle CreateConfirmationPresentation(
            FerritaToolInvocation invocation,
            int toolCallIndex)
        {
            ArgumentNullException.ThrowIfNull(invocation);

            var handle = CreatePresentation(toolCallIndex, invocation.ToolName, preferConfirmationPresentation: true);
            ApplyInvocationToPresentationState(handle.State, invocation);
            return handle;
        }

        public bool TryAttachPresentation(
            ChatMessagePartModel part,
            int? preferredToolCallIndex = null)
        {
            ArgumentNullException.ThrowIfNull(part);

            if (part.PartType != ChatMessagePartType.ToolCall)
            {
                return false;
            }

            var snapshot = TryParseSnapshot(part.Content);
            var toolName = ResolveToolName(part, snapshot);
            var toolCallIndex = ResolveToolCallIndex(preferredToolCallIndex, snapshot, part.ToolCallId);

            if (part.ToolPresentationState != null && part.ToolPresentationView != null)
            {
                ApplyContentToPresentationState(part, snapshot, toolName);
                return true;
            }

            var handle = CreatePresentation(toolCallIndex, toolName);
            part.AttachToolPresentation(handle.State, handle.View);

            ApplyContentToPresentationState(part, snapshot, toolName);

            return true;
        }

        private ToolInvocationPresentationHandle CreatePresentation(
            int toolCallIndex,
            string? toolName,
            bool preferConfirmationPresentation)
        {
            var normalizedToolName = string.IsNullOrWhiteSpace(toolName)
                ? $"Tool #{toolCallIndex}"
                : toolName.Trim();

            if (_registrations.TryGetValue(normalizedToolName, out var registration))
            {
                var state = new FerritaToolInvocationPresentationState(
                    toolCallIndex,
                    registration.Definition.Name,
                    registration.Definition.Description,
                    registration.IconPath);
                state.EnsureParameterDefinitions(registration.Definition.Parameters);

                var view = (preferConfirmationPresentation
                               ? registration.CreateConfirmationPresentation(state)
                               : registration.CreateInvocationPresentation(state))
                           ?? ToolInvocationCardFactory.CreateDefault(
                               state,
                               registration.Definition.Description,
                               registration.IconPath);

                return new ToolInvocationPresentationHandle(state, view);
            }

            var fallbackState = new FerritaToolInvocationPresentationState(
                toolCallIndex,
                normalizedToolName,
                toolDescription: string.Empty,
                iconPath: DefaultToolIconPath);

            return new ToolInvocationPresentationHandle(
                fallbackState,
                ToolInvocationCardFactory.CreateDefault(
                    fallbackState,
                    "This tool does not provide a custom invocation card.",
                    DefaultToolIconPath));
        }

        private void ApplyContentToPresentationState(
            ChatMessagePartModel part,
            FerritaStreamingToolCallSnapshot? snapshot,
            string? toolName)
        {
            var state = part.ToolPresentationState;
            if (state == null)
            {
                return;
            }

            if (snapshot != null)
            {
                state.ApplySnapshot(snapshot, ResolveParameterDefinitions(snapshot.ToolName));
                state.ApplyToolProgress(part.ToolProgress);
                ApplyResultToPresentationState(part, state);
                return;
            }

            if (!string.IsNullOrWhiteSpace(toolName))
            {
                state.ToolName = toolName.Trim();
            }

            state.RawToolXml = NormalizeXml(part.Content);
            state.IsInvocationClosed = !part.IsStreaming;
            state.ApplyToolProgress(part.ToolProgress);
            ApplyResultToPresentationState(part, state);
        }

        private void ApplyInvocationToPresentationState(
            FerritaToolInvocationPresentationState state,
            FerritaToolInvocation invocation)
        {
            ArgumentNullException.ThrowIfNull(state);
            ArgumentNullException.ThrowIfNull(invocation);

            state.ToolName = invocation.ToolName;
            state.RawToolXml = NormalizeXml(invocation.InvocationXml);
            state.IsInvocationClosed = true;
            state.ClearToolResult();

            var parameterDefinitions = ResolveParameterDefinitions(invocation.ToolName);
            state.EnsureParameterDefinitions(parameterDefinitions);

            Dictionary<string, FerritaToolParameterDefinition>? definitionsByName = null;
            if (parameterDefinitions != null)
            {
                definitionsByName = parameterDefinitions
                    .GroupBy(item => item.Name, StringComparer.OrdinalIgnoreCase)
                    .ToDictionary(group => group.Key, group => group.Last(), StringComparer.OrdinalIgnoreCase);
            }

            foreach (var argument in invocation.RawArguments)
            {
                FerritaToolParameterDefinition? definition = null;
                definitionsByName?.TryGetValue(argument.Key, out definition);

                var parameterState = state.GetOrCreateParameterState(argument.Key, definition);
                parameterState.Value = argument.Value ?? string.Empty;
                parameterState.IsClosed = true;
            }
        }

        private static void ApplyResultToPresentationState(
            ChatMessagePartModel part,
            FerritaToolInvocationPresentationState state)
        {
            if (part.HasToolResult)
            {
                state.ApplyToolResult(part.ToolResultContent, part.ToolResultPresentationKind);
                return;
            }

            state.ClearToolResult();
        }

        private FerritaStreamingToolCallSnapshot? TryParseSnapshot(string? content)
        {
            var normalizedContent = NormalizeXml(content);
            if (normalizedContent.Length == 0)
            {
                return null;
            }

            return _streamingParser.Parse(normalizedContent).FirstOrDefault();
        }

        private IReadOnlyList<FerritaToolParameterDefinition>? ResolveParameterDefinitions(string? toolName)
        {
            if (string.IsNullOrWhiteSpace(toolName))
            {
                return null;
            }

            return _registrations.TryGetValue(toolName.Trim(), out var registration)
                ? registration.Definition.Parameters
                : null;
        }

        private static string? ResolveToolName(
            ChatMessagePartModel part,
            FerritaStreamingToolCallSnapshot? snapshot)
        {
            if (!string.IsNullOrWhiteSpace(snapshot?.ToolName))
            {
                return snapshot.ToolName.Trim();
            }

            return string.IsNullOrWhiteSpace(part.Title)
                ? null
                : part.Title.Trim();
        }

        private static int ResolveToolCallIndex(
            int? preferredToolCallIndex,
            FerritaStreamingToolCallSnapshot? snapshot,
            string? toolCallId)
        {
            if (preferredToolCallIndex is > 0)
            {
                return preferredToolCallIndex.Value;
            }

            if (snapshot?.ToolCallIndex is > 0)
            {
                return snapshot.ToolCallIndex;
            }

            return TryParseToolCallIndex(toolCallId, out var parsedToolCallIndex)
                ? parsedToolCallIndex
                : 1;
        }

        private static bool TryParseToolCallIndex(string? toolCallId, out int toolCallIndex)
        {
            toolCallIndex = 0;
            if (string.IsNullOrWhiteSpace(toolCallId))
            {
                return false;
            }

            var normalizedToolCallId = toolCallId.Trim();
            var digitStartIndex = normalizedToolCallId.Length;
            while (digitStartIndex > 0 && char.IsDigit(normalizedToolCallId[digitStartIndex - 1]))
            {
                digitStartIndex--;
            }

            if (digitStartIndex >= normalizedToolCallId.Length ||
                !int.TryParse(normalizedToolCallId[digitStartIndex..], out toolCallIndex) ||
                toolCallIndex <= 0)
            {
                toolCallIndex = 0;
                return false;
            }

            return true;
        }

        private static string NormalizeXml(string? content)
        {
            return string.IsNullOrWhiteSpace(content)
                ? string.Empty
                : content.Trim();
        }
    }

    public sealed record ToolInvocationPresentationHandle(
        FerritaToolInvocationPresentationState State,
        FrameworkElement View);
}
