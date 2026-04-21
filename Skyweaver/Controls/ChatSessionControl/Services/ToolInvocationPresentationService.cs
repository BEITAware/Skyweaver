using System.Windows;
using System.Linq;
using Skyweaver.Controls.ChatSessionControl.Views;
using Skyweaver.Services.SkyweaverTools;

namespace Skyweaver.Controls.ChatSessionControl.Services
{
    public sealed class ToolInvocationPresentationService
    {
        private const string DefaultToolIconPath = "pack://application:,,,/Resources/Script.png";

        private readonly Dictionary<string, SkyweaverToolRegistration> _registrations;

        public ToolInvocationPresentationService()
            : this(new SkyweaverToolManager())
        {
        }

        public ToolInvocationPresentationService(SkyweaverToolManager toolManager)
        {
            ArgumentNullException.ThrowIfNull(toolManager);

            _registrations = toolManager.GetRegisteredTools()
                .ToDictionary(registration => registration.Definition.Name, StringComparer.OrdinalIgnoreCase);
        }

        public ToolInvocationPresentationHandle CreatePresentation(int toolCallIndex, string? toolName)
        {
            var normalizedToolName = string.IsNullOrWhiteSpace(toolName)
                ? $"Tool #{toolCallIndex}"
                : toolName.Trim();

            if (_registrations.TryGetValue(normalizedToolName, out var registration))
            {
                var state = new SkyweaverToolInvocationPresentationState(
                    toolCallIndex,
                    registration.Definition.Name,
                    registration.Definition.Description,
                    registration.IconPath);
                state.EnsureParameterDefinitions(registration.Definition.Parameters);

                var view = registration.CreateInvocationPresentation(state)
                           ?? ToolInvocationCardFactory.CreateDefault(
                               state,
                               registration.Definition.Description,
                               registration.IconPath);

                return new ToolInvocationPresentationHandle(state, view);
            }

            var fallbackState = new SkyweaverToolInvocationPresentationState(
                toolCallIndex,
                normalizedToolName,
                toolDescription: string.Empty,
                iconPath: DefaultToolIconPath);

            return new ToolInvocationPresentationHandle(
                fallbackState,
                ToolInvocationCardFactory.CreateDefault(
                    fallbackState,
                    "该工具未提供专用参数呈现控件。",
                    DefaultToolIconPath));
        }
    }

    public sealed record ToolInvocationPresentationHandle(
        SkyweaverToolInvocationPresentationState State,
        FrameworkElement View);
}
