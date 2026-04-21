using System.Globalization;
using System.Windows;
using Newtonsoft.Json.Linq;
using Skyweaver.Controls.ChatSessionControl.Views;
using Skyweaver.Services.SkyweaverTools;

namespace Skyweaver.Tools
{
    public sealed class WorkspaceNoteTemplateTool : ISkyweaverTool, ISkyweaverToolConfigurationProvider, ISkyweaverToolInvocationPresentationProvider
    {
        public const string ToolName = "WorkspaceNoteTemplate";

        private static readonly SkyweaverToolDefinition s_definition = new(
            ToolName,
            "生成一个工作区笔记模板。宿主可以通过工具自身配置扩展默认值、摘要布局和运行时行为。",
            "Script",
            [
                new SkyweaverToolParameterDefinition(
                    "Title",
                    "笔记标题。",
                    SkyweaverToolParameterType.String,
                    isRequired: true),
                new SkyweaverToolParameterDefinition(
                    "Priority",
                    "任务优先级，数值越大表示越紧急。",
                    SkyweaverToolParameterType.Integer,
                    isRequired: false,
                    defaultValue: "1"),
                new SkyweaverToolParameterDefinition(
                    "Pin",
                    "是否置顶该笔记。",
                    SkyweaverToolParameterType.Boolean,
                    isRequired: false,
                    defaultValue: "false"),
                new SkyweaverToolParameterDefinition(
                    "Tags",
                    "笔记标签的 JSON 数组。省略时使用宿主保存的默认标签。",
                    SkyweaverToolParameterType.Json,
                    isRequired: false,
                    defaultValue: "[\"memo\"]")
            ]);

        public SkyweaverToolDefinition Definition => s_definition;

        public SkyweaverToolDefinition GetEffectiveDefinition(SkyweaverToolConfigurationState configuration)
        {
            var settings = WorkspaceNoteTemplateToolSettings.FromConfiguration(configuration);
            var preset = settings.ResolvePreset();

            return new SkyweaverToolDefinition(
                ToolName,
                $"生成一个 {preset.DisplayName} 工作区笔记模板。默认标签为 {settings.DescribeDefaultTags()}，并且默认{(settings.IncludeContextMetadata ? "包含" : "不包含")}上下文元数据。",
                "Script",
                [
                    new SkyweaverToolParameterDefinition(
                        "Title",
                        "笔记标题。",
                        SkyweaverToolParameterType.String,
                        isRequired: true),
                    new SkyweaverToolParameterDefinition(
                        "Priority",
                        "任务优先级，数值越大表示越紧急。",
                        SkyweaverToolParameterType.Integer,
                        isRequired: false,
                        defaultValue: preset.DefaultPriority.ToString(CultureInfo.InvariantCulture)),
                    new SkyweaverToolParameterDefinition(
                        "Pin",
                        "是否置顶该笔记。",
                        SkyweaverToolParameterType.Boolean,
                        isRequired: false,
                        defaultValue: "false"),
                    new SkyweaverToolParameterDefinition(
                        "Tags",
                        "笔记标签的 JSON 数组。默认值来自宿主保存的工具配置。",
                        SkyweaverToolParameterType.Json,
                        isRequired: false,
                        defaultValue: settings.CreateDefaultTagsJson())
                ]);
        }

        public SkyweaverToolConfigurationPresenter? CreateConfigurationPresenter(SkyweaverToolConfigurationEditorContext context)
        {
            return new WorkspaceNoteTemplateToolConfigurationPresenter(context);
        }

        public FrameworkElement? CreateInvocationPresentation(SkyweaverToolInvocationPresentationContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            return ToolInvocationCardFactory.Create(
                context,
                [
                    new ToolInvocationCardFieldDefinition("标题", "Title", "等待标题..."),
                    new ToolInvocationCardFieldDefinition("优先级", "Priority", "默认优先级"),
                    new ToolInvocationCardFieldDefinition("置顶", "Pin", "默认不置顶"),
                    new ToolInvocationCardFieldDefinition("标签", "Tags", "默认标签来自工具配置")
                ]);
        }

        public Task<SkyweaverToolResult> ExecuteAsync(
            SkyweaverToolContext context,
            SkyweaverToolArguments arguments,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var settings = WorkspaceNoteTemplateToolSettings.FromConfiguration(context.CurrentToolConfiguration);
            var preset = settings.ResolvePreset();
            var title = arguments.GetString("Title") ?? "Untitled note";
            var priority = arguments.GetInteger("Priority", preset.DefaultPriority);
            var pin = arguments.GetBoolean("Pin");
            var tags = arguments.GetJson("Tags");
            var tagText = FormatTags(tags);

            var lines = new List<string>
            {
                $"# {title}",
                $"Preset: {preset.DisplayName}",
                $"Priority: {priority}",
                $"Pinned: {pin}",
                $"Tags: {tagText}",
                string.Empty,
                "Summary:"
            };

            if (settings.IncludeContextMetadata)
            {
                lines.Insert(3, $"Workspace: {(string.IsNullOrWhiteSpace(context.WorkspacePath) ? "n/a" : context.WorkspacePath)}");
                lines.Insert(4, $"Session: {(string.IsNullOrWhiteSpace(context.SessionTitle) ? "n/a" : context.SessionTitle)}");
            }

            foreach (var prompt in settings.BuildSummaryPrompts())
            {
                lines.Add($"- {prompt}");
            }

            return Task.FromResult(SkyweaverToolResult.Success(
                string.Join(Environment.NewLine, lines),
                new Dictionary<string, object?>
                {
                    ["title"] = title,
                    ["preset"] = preset.Key,
                    ["priority"] = priority,
                    ["pin"] = pin,
                    ["tags"] = tagText,
                    ["includeContextMetadata"] = settings.IncludeContextMetadata
                }));
        }

        private static string FormatTags(JToken? tags)
        {
            if (tags is not JArray array || array.Count == 0)
            {
                return "none";
            }

            var values = array
                .Values<string>()
                .Where(item => !string.IsNullOrWhiteSpace(item))
                .Select(item => item!.Trim())
                .ToArray();

            return values.Length == 0 ? "none" : string.Join(", ", values);
        }
    }
}
