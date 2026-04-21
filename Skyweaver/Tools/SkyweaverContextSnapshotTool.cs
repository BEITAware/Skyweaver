using System.Windows;
using Skyweaver.Controls.ChatSessionControl.Views;
using Skyweaver.Services.SkyweaverTools;

namespace Skyweaver.Tools
{
    public sealed class SkyweaverContextSnapshotTool : ISkyweaverTool, ISkyweaverToolInvocationPresentationProvider
    {
        public const string ToolName = "SkyweaverContextSnapshot";

        private static readonly SkyweaverToolDefinition s_definition = new(
            ToolName,
            "读取当前 Skyweaver 运行时上下文，并返回简明快照。",
            "GuideBot",
            [
                new SkyweaverToolParameterDefinition(
                    "IncludeProperties",
                    "是否在快照中包含自定义上下文属性。",
                    SkyweaverToolParameterType.Boolean,
                    isRequired: false,
                    defaultValue: "true"),
                new SkyweaverToolParameterDefinition(
                    "UppercaseLabels",
                    "是否使用全大写标签输出。",
                    SkyweaverToolParameterType.Boolean,
                    isRequired: false,
                    defaultValue: "false")
            ]);

        public SkyweaverToolDefinition Definition => s_definition;

        public FrameworkElement? CreateInvocationPresentation(SkyweaverToolInvocationPresentationContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            return ToolInvocationCardFactory.Create(
                context,
                [
                    new ToolInvocationCardFieldDefinition("包含属性", "IncludeProperties", "默认包含"),
                    new ToolInvocationCardFieldDefinition("标签大写", "UppercaseLabels", "默认保持原样")
                ]);
        }

        public Task<SkyweaverToolResult> ExecuteAsync(
            SkyweaverToolContext context,
            SkyweaverToolArguments arguments,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var includeProperties = arguments.GetBoolean("IncludeProperties", true);
            var uppercaseLabels = arguments.GetBoolean("UppercaseLabels");

            string Label(string value) => uppercaseLabels ? value.ToUpperInvariant() : value;

            var lines = new List<string>
            {
                $"{Label("Application")}: {context.ApplicationName}",
                $"{Label("Timestamp")}: {context.Timestamp:yyyy-MM-dd HH:mm:ss zzz}",
                $"{Label("Session")}: {(string.IsNullOrWhiteSpace(context.SessionTitle) ? "n/a" : context.SessionTitle)}",
                $"{Label("Workspace")}: {(string.IsNullOrWhiteSpace(context.WorkspacePath) ? "n/a" : context.WorkspacePath)}"
            };

            if (includeProperties)
            {
                if (context.Properties.Count == 0)
                {
                    lines.Add($"{Label("Properties")}: none");
                }
                else
                {
                    lines.Add($"{Label("Properties")}:");
                    foreach (var property in context.Properties.OrderBy(item => item.Key, StringComparer.OrdinalIgnoreCase))
                    {
                        lines.Add($"- {property.Key}: {property.Value}");
                    }
                }
            }

            return Task.FromResult(SkyweaverToolResult.Success(
                string.Join(Environment.NewLine, lines),
                new Dictionary<string, object?>
                {
                    ["applicationName"] = context.ApplicationName,
                    ["sessionTitle"] = context.SessionTitle,
                    ["workspacePath"] = context.WorkspacePath,
                    ["timestamp"] = context.Timestamp
                }));
        }
    }
}
