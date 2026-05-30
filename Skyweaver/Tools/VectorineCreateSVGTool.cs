using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Skyweaver.Services.SkyweaverTools;

namespace Skyweaver.Tools
{
    /// <summary>
    /// VectorineCreateSVG 工具，归属于 Vectorine 工具集，用于实时渲染 SVG 代码并支持选择背景。
    /// </summary>
    public sealed class VectorineCreateSVGTool :
        ISkyweaverTool,
        ISkyweaverToolInvocationPresentationProvider,
        ISkyweaverToolPromptDescriptionProvider
    {
        public const string ToolName = "VectorineCreateSVG";

        private static readonly SkyweaverToolDefinition s_definition = new(
            ToolName,
            "Renders an SVG vector graphic inside a beautiful styled custom viewport live. NOTE: It is recommended to use VectorineCreateXAML instead of this tool.",
            "Script",
            new[]
            {
                new SkyweaverToolParameterDefinition(
                    "Title",
                    "The optional title to display on the SVG rendering view.",
                    SkyweaverToolParameterType.String,
                    isRequired: false),
                new SkyweaverToolParameterDefinition(
                    "Background",
                    "The background style (optional). Accepts 'Black' or 'White'. Defaults to 'Black'.",
                    SkyweaverToolParameterType.String,
                    isRequired: false),
                new SkyweaverToolParameterDefinition(
                    "SVG",
                    "The raw SVG XML code to render. Note: Prefer VectorineCreateXAML.",
                    SkyweaverToolParameterType.String,
                    isRequired: true)
            },
            isSystemTool: false,
            canBelongToToolKit: true,
            defaultToolKitKeys: new[] { "Vectorine" });

        public SkyweaverToolDefinition Definition => s_definition;

        public string GetPromptDescription(SkyweaverToolPromptDescriptionContext context)
        {
            return "Renders a vector graphic SVG source code live with a custom background. " +
                   "RECOMMENDED: You should use VectorineCreateXAML instead of this tool, as XAML is native, has better layout control, and avoids SVG conversion issues. " +
                   "Accepts parameters: Title (string, optional), Background (string, optional, allowed values: 'Black' or 'White', defaults to 'Black'), and SVG (string, required, raw SVG source code).";
        }

        public FrameworkElement? CreateInvocationPresentation(SkyweaverToolInvocationPresentationContext context)
        {
            ArgumentNullException.ThrowIfNull(context);
            return new VectorineCreateSvgToolInvocationView
            {
                DataContext = new VectorineCreateSvgToolInvocationViewModel(context.State)
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
            var svg = arguments.GetString("SVG") ?? string.Empty;

            var toolData = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase)
            {
                ["title"] = title,
                ["background"] = background,
                ["svgLength"] = svg.Length,
                ["isValid"] = !string.IsNullOrWhiteSpace(svg)
            };

            var content = $"Vectorine SVG Rendered successfully.\nTitle: {title}\nBackground: {background}\nSVG Length: {svg.Length} characters.";
            return Task.FromResult(SkyweaverToolResult.Success(content, toolData));
        }
    }
}
