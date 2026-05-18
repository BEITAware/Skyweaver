using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Skyweaver.Services.Localization;
using Skyweaver.Services.SkyweaverTools;

namespace Skyweaver.Controls.ChatSessionControl.Views
{
    public sealed class ToolInvocationCardFieldDefinition
    {
        public ToolInvocationCardFieldDefinition(
            string label,
            string parameterName,
            string? emptyValueText = null)
        {
            Label = string.IsNullOrWhiteSpace(label) ? parameterName : label.Trim();
            ParameterName = string.IsNullOrWhiteSpace(parameterName) ? string.Empty : parameterName.Trim();
            EmptyValueText = string.IsNullOrWhiteSpace(emptyValueText) ? L("ToolInvocation.WaitingForParameterFallback", "等待参数...") : emptyValueText.Trim();
        }

        public string Label { get; }

        public string ParameterName { get; }

        public string EmptyValueText { get; }

        private static string L(string resourceKey, string fallback)
        {
            return LocalizationRuntime.Instance.GetString(resourceKey, fallback);
        }
    }

    public sealed class ToolInvocationCardFieldViewModel
    {
        public ToolInvocationCardFieldViewModel(
            string label,
            SkyweaverToolInvocationParameterPresentationState parameter,
            string emptyValueText)
        {
            Label = label;
            Parameter = parameter;
            EmptyValueText = emptyValueText;
        }

        public string Label { get; }

        public SkyweaverToolInvocationParameterPresentationState Parameter { get; }

        public string EmptyValueText { get; }
    }

    public sealed class ToolInvocationCardViewModel
    {
        public ToolInvocationCardViewModel(
            SkyweaverToolInvocationPresentationState state,
            string description,
            string iconPath,
            IEnumerable<ToolInvocationCardFieldViewModel>? fields)
        {
            State = state ?? throw new ArgumentNullException(nameof(state));
            Description = description ?? string.Empty;
            IconPath = iconPath ?? string.Empty;
            Fields = new ObservableCollection<ToolInvocationCardFieldViewModel>(fields ?? Array.Empty<ToolInvocationCardFieldViewModel>());
        }

        public SkyweaverToolInvocationPresentationState State { get; }

        public string Description { get; }

        public string IconPath { get; }

        public ObservableCollection<ToolInvocationCardFieldViewModel> Fields { get; }

        public bool HasFields => Fields.Count > 0;
    }

    public static class ToolInvocationCardFactory
    {
        public static FrameworkElement Create(
            SkyweaverToolInvocationPresentationContext context,
            IEnumerable<ToolInvocationCardFieldDefinition>? fields = null)
        {
            ArgumentNullException.ThrowIfNull(context);

            context.State.EnsureParameterDefinitions(context.EffectiveDefinition.Parameters);

            var fieldViewModels = (fields ?? Array.Empty<ToolInvocationCardFieldDefinition>())
                .Where(field => !string.IsNullOrWhiteSpace(field.ParameterName))
                .Select(field =>
                {
                    var definition = context.EffectiveDefinition.Parameters.FirstOrDefault(parameter =>
                        string.Equals(parameter.Name, field.ParameterName, StringComparison.OrdinalIgnoreCase));
                    var parameterState = context.State.GetOrCreateParameterState(field.ParameterName, definition);
                    return new ToolInvocationCardFieldViewModel(field.Label, parameterState, field.EmptyValueText);
                })
                .ToArray();

            return new ToolInvocationCardView
            {
                DataContext = new ToolInvocationCardViewModel(
                    context.State,
                    context.EffectiveDefinition.Description,
                    context.IconPath,
                    fieldViewModels)
            };
        }

        public static FrameworkElement CreateAerialCity(
            SkyweaverToolInvocationPresentationContext context,
            IEnumerable<ToolInvocationCardFieldDefinition>? fields = null)
        {
            ArgumentNullException.ThrowIfNull(context);

            context.State.EnsureParameterDefinitions(context.EffectiveDefinition.Parameters);

            var fieldViewModels = (fields ?? Array.Empty<ToolInvocationCardFieldDefinition>())
                .Where(field => !string.IsNullOrWhiteSpace(field.ParameterName))
                .Select(field =>
                {
                    var definition = context.EffectiveDefinition.Parameters.FirstOrDefault(parameter =>
                        string.Equals(parameter.Name, field.ParameterName, StringComparison.OrdinalIgnoreCase));
                    var parameterState = context.State.GetOrCreateParameterState(field.ParameterName, definition);
                    return new ToolInvocationCardFieldViewModel(field.Label, parameterState, field.EmptyValueText);
                })
                .ToArray();

            return new AerialCityToolInvocationCardView
            {
                DataContext = new ToolInvocationCardViewModel(
                    context.State,
                    context.EffectiveDefinition.Description,
                    context.IconPath,
                    fieldViewModels)
            };
        }

        public static FrameworkElement CreateDefault(
            SkyweaverToolInvocationPresentationState state,
            string? description,
            string? iconPath)
        {
            ArgumentNullException.ThrowIfNull(state);

            return new ToolInvocationCardView
            {
                DataContext = new ToolInvocationCardViewModel(
                    state,
                    description ?? L("ToolInvocation.DefaultDescription", "该工具未提供专用参数呈现控件。"),
                    iconPath ?? string.Empty,
                    fields: null)
            };
        }

        private static string L(string resourceKey, string fallback)
        {
            return LocalizationRuntime.Instance.GetString(resourceKey, fallback);
        }
    }
}
