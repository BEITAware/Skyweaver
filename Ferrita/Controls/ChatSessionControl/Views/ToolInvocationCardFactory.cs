using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Ferrita.Services.Localization;
using Ferrita.Services.FerritaTools;

namespace Ferrita.Controls.ChatSessionControl.Views
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

    public sealed class ToolInvocationCardFieldViewModel : Ferrita.Infrastructure.Mvvm.ObservableObject
    {
        private readonly string _toolName;
        private readonly string _parameterName;
        private readonly string _originalLabel;
        private readonly string _originalEmptyValueText;

        public ToolInvocationCardFieldViewModel(
            string toolName,
            string parameterName,
            string originalLabel,
            FerritaToolInvocationParameterPresentationState parameter,
            string originalEmptyValueText)
        {
            _toolName = toolName;
            _parameterName = parameterName;
            _originalLabel = originalLabel;
            Parameter = parameter;
            _originalEmptyValueText = originalEmptyValueText;

            LocalizationRuntime.Instance.LanguageChanged += OnLanguageChanged;
        }

        private void OnLanguageChanged(object? sender, EventArgs e)
        {
            OnPropertyChanged(nameof(Label));
            OnPropertyChanged(nameof(EmptyValueText));
        }

        public string Label => LocalizationRuntime.Instance.GetString($"Tool.{_toolName}.Param.{_parameterName}.Label", _originalLabel);

        public FerritaToolInvocationParameterPresentationState Parameter { get; }

        public string EmptyValueText => LocalizationRuntime.Instance.GetString($"Tool.{_toolName}.Param.{_parameterName}.EmptyText", _originalEmptyValueText);
    }

    public sealed class ToolInvocationCardViewModel : Ferrita.Infrastructure.Mvvm.ObservableObject
    {
        private readonly string _originalDescription;

        public ToolInvocationCardViewModel(
            FerritaToolInvocationPresentationState state,
            string originalDescription,
            string iconPath,
            IEnumerable<ToolInvocationCardFieldViewModel>? fields)
        {
            State = state ?? throw new ArgumentNullException(nameof(state));
            _originalDescription = originalDescription ?? string.Empty;
            IconPath = iconPath ?? string.Empty;
            Fields = new ObservableCollection<ToolInvocationCardFieldViewModel>(fields ?? Array.Empty<ToolInvocationCardFieldViewModel>());

            LocalizationRuntime.Instance.LanguageChanged += OnLanguageChanged;
        }

        private void OnLanguageChanged(object? sender, EventArgs e)
        {
            OnPropertyChanged(nameof(Description));
        }

        public FerritaToolInvocationPresentationState State { get; }

        public string Description => LocalizationRuntime.Instance.GetString($"Tool.{State.ToolName}.Description", _originalDescription);

        public string IconPath { get; }

        public string ComputerUseIconPath
        {
            get
            {
                var toolName = State.ToolName;
                if (string.IsNullOrEmpty(toolName)) return IconPath;

                // 键盘相关
                if (toolName.Contains("Key", StringComparison.OrdinalIgnoreCase) || 
                    toolName.Contains("Type", StringComparison.OrdinalIgnoreCase) ||
                    toolName.Contains("Press", StringComparison.OrdinalIgnoreCase) ||
                    toolName.Contains("Hold", StringComparison.OrdinalIgnoreCase) ||
                    toolName.Contains("Release", StringComparison.OrdinalIgnoreCase) ||
                    toolName.Contains("TextInput", StringComparison.OrdinalIgnoreCase))
                {
                    return "pack://application:,,,/Resources/Keyboard.png";
                }
                
                // 鼠标相关
                if (toolName.Contains("Click", StringComparison.OrdinalIgnoreCase) || 
                    toolName.Contains("Drag", StringComparison.OrdinalIgnoreCase) ||
                    toolName.Contains("Move", StringComparison.OrdinalIgnoreCase) ||
                    toolName.Contains("Scroll", StringComparison.OrdinalIgnoreCase) ||
                    toolName.Contains("Hover", StringComparison.OrdinalIgnoreCase) ||
                    toolName.Contains("Mouse", StringComparison.OrdinalIgnoreCase))
                {
                    return "pack://application:,,,/Resources/MouseClick.png";
                }

                // 其他通用
                return "pack://application:,,,/Resources/ComputerUse.png";
            }
        }

        public ObservableCollection<ToolInvocationCardFieldViewModel> Fields { get; }

        public bool HasFields => Fields.Count > 0;
    }

    public static class ToolInvocationCardFactory
    {
        public static FrameworkElement Create(
            FerritaToolInvocationPresentationContext context,
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
                    var toolName = context.EffectiveDefinition.Name;
                    return new ToolInvocationCardFieldViewModel(
                        toolName,
                        field.ParameterName,
                        field.Label,
                        parameterState,
                        field.EmptyValueText);
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
            FerritaToolInvocationPresentationContext context,
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
                    var toolName = context.EffectiveDefinition.Name;
                    return new ToolInvocationCardFieldViewModel(
                        toolName,
                        field.ParameterName,
                        field.Label,
                        parameterState,
                        field.EmptyValueText);
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

        public static FrameworkElement CreateWebSearch(
            FerritaToolInvocationPresentationContext context,
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
                    var toolName = context.EffectiveDefinition.Name;
                    return new ToolInvocationCardFieldViewModel(
                        toolName,
                        field.ParameterName,
                        field.Label,
                        parameterState,
                        field.EmptyValueText);
                })
                .ToArray();

            return new WebSearchToolInvocationCardView
            {
                DataContext = new ToolInvocationCardViewModel(
                    context.State,
                    context.EffectiveDefinition.Description,
                    context.IconPath,
                    fieldViewModels)
            };
        }

        public static FrameworkElement CreateWebBrowse(
            FerritaToolInvocationPresentationContext context,
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
                    var toolName = context.EffectiveDefinition.Name;
                    return new ToolInvocationCardFieldViewModel(
                        toolName,
                        field.ParameterName,
                        field.Label,
                        parameterState,
                        field.EmptyValueText);
                })
                .ToArray();

            return new WebBrowseToolInvocationCardView
            {
                DataContext = new ToolInvocationCardViewModel(
                    context.State,
                    context.EffectiveDefinition.Description,
                    context.IconPath,
                    fieldViewModels)
            };
        }

        public static FrameworkElement CreateComputerUse(
            FerritaToolInvocationPresentationContext context,
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
                    var toolName = context.EffectiveDefinition.Name;
                    return new ToolInvocationCardFieldViewModel(
                        toolName,
                        field.ParameterName,
                        field.Label,
                        parameterState,
                        field.EmptyValueText);
                })
                .ToArray();

            return new ComputerUseToolInvocationCardView
            {
                DataContext = new ToolInvocationCardViewModel(
                    context.State,
                    context.EffectiveDefinition.Description,
                    context.IconPath,
                    fieldViewModels)
            };
        }

        public static FrameworkElement CreateDefault(
            FerritaToolInvocationPresentationState state,
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
