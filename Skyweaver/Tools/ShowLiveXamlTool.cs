using System.Text;
using System.Windows;
using Skyweaver.Services.LiveXaml;
using Skyweaver.Services.SkyweaverTools;

namespace Skyweaver.Tools
{
    public sealed class ShowLiveXamlTool :
        ISkyweaverTool,
        ISkyweaverToolInvocationPresentationProvider,
        ISkyweaverToolPromptDescriptionProvider
    {
        public const string ToolName = "ShowLiveXAML";

        private static readonly SkyweaverToolDefinition s_definition = new(
            ToolName,
            "Validates and renders a fresh in-chat preview for an existing LiveXAML .xaml file.",
            "Script",
            [
                new SkyweaverToolParameterDefinition(
                    "XAMLFilePath",
                    "The full absolute path of the .xaml file to preview. Always reuse the exact absolute path returned by InitializeLiveXAML or by later file-editing results.",
                    SkyweaverToolParameterType.String,
                    isRequired: true)
            ],
            isSystemTool: false,
            canBelongToToolKit: true,
            defaultToolKitKeys: ["LiveXAML"]);

        public SkyweaverToolDefinition Definition => s_definition;

        public string GetPromptDescription(SkyweaverToolPromptDescriptionContext context)
        {
            return
                "Renders a fresh LiveXAML preview inside this tool call card. Pass the full absolute .xaml path returned by InitializeLiveXAML. " +
                "The tool automatically reads the sibling .xaml.cs file if one exists, validates the pair, and creates a new embedded WPF preview instance for this call. " +
                "Every ShowLiveXAML call is independent: edit the files with normal file tools, then call ShowLiveXAML again to see a new result. " +
                "The runtime is designed to feel as close to normal WPF as possible without external libraries: use embeddable roots like UserControl or layout panels, x:Class is supported, normal constructor + InitializeComponent patterns are supported, data binding is supported, event handler attributes such as Click=\"OnClick\" are supported, x:Name / Name behave like named controls in code-behind, and local helper classes such as converters defined in the same .xaml.cs assembly are supported through normal clr-namespace mappings. " +
                "Standard .NET / System / WPF assemblies are already referenced. External DLLs and NuGet packages are not supported. Window roots are not embeddable; use UserControl or another embeddable FrameworkElement root instead.";
        }

        public FrameworkElement? CreateInvocationPresentation(SkyweaverToolInvocationPresentationContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            return new ShowLiveXamlToolInvocationView
            {
                DataContext = new ShowLiveXamlToolInvocationViewModel(context.State)
            };
        }

        public Task<SkyweaverToolResult> ExecuteAsync(
            SkyweaverToolContext context,
            SkyweaverToolArguments arguments,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                var normalizedXamlFilePath = LiveXamlFileSupport.NormalizeAbsoluteXamlPath(
                    arguments.GetString("XAMLFilePath") ?? string.Empty);
                var codeBehindFilePath = LiveXamlFileSupport.ResolveSiblingCodeBehindPath(normalizedXamlFilePath);
                var validationResult = LiveXamlRuntime.Validate(normalizedXamlFilePath, codeBehindFilePath);

                var toolData = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase)
                {
                    ["xamlFilePath"] = normalizedXamlFilePath,
                    ["codeBehindFilePath"] = codeBehindFilePath,
                    ["rootClassName"] = validationResult.RootClassName,
                    ["rootElementTypeName"] = validationResult.RootElementTypeName,
                    ["isValid"] = validationResult.IsSuccess
                };

                var content = BuildShowResultContent(normalizedXamlFilePath, codeBehindFilePath, validationResult);
                return Task.FromResult(validationResult.IsSuccess
                    ? SkyweaverToolResult.Success(content, toolData)
                    : SkyweaverToolResult.Failure(content, toolData));
            }
            catch (Exception ex)
            {
                return Task.FromResult(SkyweaverToolResult.Failure(
                    $"ShowLiveXAML failed: {ex.Message}"));
            }
        }

        private static string BuildShowResultContent(
            string xamlFilePath,
            string? codeBehindFilePath,
            LiveXamlLoadResult validationResult)
        {
            var builder = new StringBuilder(384);
            builder.AppendLine($"XAMLFilePath: {xamlFilePath}");
            builder.AppendLine($"CodeBehindFilePath: {codeBehindFilePath ?? "(not found)"}");
            if (!string.IsNullOrWhiteSpace(validationResult.RootClassName))
            {
                builder.AppendLine($"ResolvedRootClassName: {validationResult.RootClassName}");
            }

            if (!string.IsNullOrWhiteSpace(validationResult.RootElementTypeName))
            {
                builder.AppendLine($"ResolvedRootElementType: {validationResult.RootElementTypeName}");
            }

            builder.AppendLine(validationResult.IsSuccess
                ? "Validation: Passed"
                : "Validation: Failed");
            builder.AppendLine("Preview: The rendered result appears directly in the ShowLiveXAML tool card for this call.");

            var diagnosticsText = validationResult.BuildDiagnosticsText();
            if (!string.IsNullOrWhiteSpace(diagnosticsText))
            {
                builder.AppendLine();
                builder.AppendLine("Diagnostics:");
                builder.AppendLine(diagnosticsText);
            }

            return builder.ToString().TrimEnd();
        }
    }
}
