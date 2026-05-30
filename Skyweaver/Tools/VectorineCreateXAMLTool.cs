using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Skyweaver.Services.SkyweaverTools;

namespace Skyweaver.Tools
{
    /// <summary>
    /// VectorineCreateXAML 工具，归属于 Vectorine 工具集，用于实时渲染 XAML 代码并支持选择背景。
    /// 推荐使用此工具而非 VectorineCreateSVG。
    /// </summary>
    public sealed class VectorineCreateXAMLTool :
        ISkyweaverTool,
        ISkyweaverToolInvocationPresentationProvider,
        ISkyweaverToolPromptDescriptionProvider
    {
        public const string ToolName = "VectorineCreateXAML";

        private static readonly SkyweaverToolDefinition s_definition = new(
            ToolName,
            "Renders a XAML vector graphic or UI layout inside a beautifully styled custom viewport live. RECOMMENDED: Prefer this tool over VectorineCreateSVG for rendering vector assets as XAML is native to WPF, more expressive, and avoids conversion issues.",
            "Script",
            new[]
            {
                new SkyweaverToolParameterDefinition(
                    "Title",
                    "The optional title to display on the XAML rendering view.",
                    SkyweaverToolParameterType.String,
                    isRequired: false),
                new SkyweaverToolParameterDefinition(
                    "Background",
                    "The background style (optional). Accepts 'Black' or 'White'. Defaults to 'Black'.",
                    SkyweaverToolParameterType.String,
                    isRequired: false),
                new SkyweaverToolParameterDefinition(
                    "XAML",
                    "The raw XAML XML code to render (e.g., <Canvas>, <DrawingGroup>). Ensure you include xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" and xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\" on the root element. Prefer using this tool over VectorineCreateSVG.",
                    SkyweaverToolParameterType.String,
                    isRequired: true)
            },
            isSystemTool: false,
            canBelongToToolKit: true,
            defaultToolKitKeys: new[] { "Vectorine" });

        public SkyweaverToolDefinition Definition => s_definition;

        public string GetPromptDescription(SkyweaverToolPromptDescriptionContext context)
        {
            return "Renders a vector graphic or UI layout XAML source code live with a custom background. " +
                   "RECOMMENDED: You should prefer this tool (VectorineCreateXAML) over VectorineCreateSVG because WPF/XAML is native, offers much better layout control, supports rich brush shapes, and eliminates SVG conversion overhead. " +
                   "Note: The root XAML element MUST include standard WPF XML namespaces: xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" and xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\". " +
                   "Accepts parameters: Title (string, optional), Background (string, optional, allowed values: 'Black' or 'White', defaults to 'Black'), and XAML (string, required, raw XAML source code).";
        }

        public FrameworkElement? CreateInvocationPresentation(SkyweaverToolInvocationPresentationContext context)
        {
            ArgumentNullException.ThrowIfNull(context);
            return new VectorineCreateXamlToolInvocationView
            {
                DataContext = new VectorineCreateXamlToolInvocationViewModel(context.State)
            };
        }

        public Task<SkyweaverToolResult> ExecuteAsync(
            SkyweaverToolContext context,
            SkyweaverToolArguments arguments,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var title = arguments.GetString("Title") ?? string.Empty;
            var background = arguments.GetString("Background") ?? string.Empty;
            var xaml = arguments.GetString("XAML") ?? string.Empty;

            var toolData = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase)
            {
                ["title"] = title,
                ["background"] = background,
                ["xamlLength"] = xaml.Length,
                ["isValid"] = !string.IsNullOrWhiteSpace(xaml)
            };

            var content = $"Vectorine XAML Rendered successfully.\nTitle: {title}\nBackground: {background}\nXAML Length: {xaml.Length} characters.";
            return Task.FromResult(SkyweaverToolResult.Success(content, toolData));
        }
    }
}
