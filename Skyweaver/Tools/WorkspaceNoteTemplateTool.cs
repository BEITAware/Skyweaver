using System.Globalization;
using System.Windows;
using Newtonsoft.Json.Linq;
using Skyweaver.Controls.ChatSessionControl.Views;
using Skyweaver.Services.Localization;
using Skyweaver.Services.SkyweaverTools;

namespace Skyweaver.Tools
{
    public sealed class WorkspaceNoteTemplateTool : ISkyweaverTool, ISkyweaverToolConfigurationProvider, ISkyweaverToolInvocationPresentationProvider
    {
        public const string ToolName = "WorkspaceNoteTemplate";

        public SkyweaverToolDefinition Definition => CreateDefaultDefinition();

        public SkyweaverToolDefinition GetEffectiveDefinition(SkyweaverToolConfigurationState configuration)
        {
            var settings = WorkspaceNoteTemplateToolSettings.FromConfiguration(configuration);
            var preset = settings.ResolvePreset();

            return new SkyweaverToolDefinition(
                ToolName,
                LF(
                    "WorkspaceNoteTemplate.Tool.EffectiveDescriptionFormat",
                    "生成一个 {0} 工作区笔记模板。默认标签为 {1}，并且默认{2}上下文元数据。",
                    preset.DisplayName,
                    settings.DescribeDefaultTags(),
                    settings.IncludeContextMetadata
                        ? L("WorkspaceNoteTemplate.Tool.ContextMetadata.IncludedToken", "包含")
                        : L("WorkspaceNoteTemplate.Tool.ContextMetadata.ExcludedToken", "不包含")),
                "Script",
                [
                    new SkyweaverToolParameterDefinition(
                        "Title",
                        L("WorkspaceNoteTemplate.Tool.Parameter.Title.Description", "笔记标题。"),
                        SkyweaverToolParameterType.String,
                        isRequired: true),
                    new SkyweaverToolParameterDefinition(
                        "Priority",
                        L("WorkspaceNoteTemplate.Tool.Parameter.Priority.Description", "任务优先级，数值越大表示越紧急。"),
                        SkyweaverToolParameterType.Integer,
                        isRequired: false,
                        defaultValue: preset.DefaultPriority.ToString(CultureInfo.InvariantCulture)),
                    new SkyweaverToolParameterDefinition(
                        "Pin",
                        L("WorkspaceNoteTemplate.Tool.Parameter.Pin.Description", "是否置顶该笔记。"),
                        SkyweaverToolParameterType.Boolean,
                        isRequired: false,
                        defaultValue: "false"),
                    new SkyweaverToolParameterDefinition(
                        "Tags",
                        L("WorkspaceNoteTemplate.Tool.Parameter.Tags.ConfiguredDescription", "笔记标签的 JSON 数组。默认值来自宿主保存的工具配置。"),
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
                    new ToolInvocationCardFieldDefinition(L("WorkspaceNoteTemplate.InvocationField.Title", "标题"), "Title", L("WorkspaceNoteTemplate.InvocationField.Title.Placeholder", "等待标题...")),
                    new ToolInvocationCardFieldDefinition(L("WorkspaceNoteTemplate.InvocationField.Priority", "优先级"), "Priority", L("WorkspaceNoteTemplate.InvocationField.Priority.Placeholder", "默认优先级")),
                    new ToolInvocationCardFieldDefinition(L("WorkspaceNoteTemplate.InvocationField.Pin", "置顶"), "Pin", L("WorkspaceNoteTemplate.InvocationField.Pin.Placeholder", "默认不置顶")),
                    new ToolInvocationCardFieldDefinition(L("WorkspaceNoteTemplate.InvocationField.Tags", "标签"), "Tags", L("WorkspaceNoteTemplate.InvocationField.Tags.Placeholder", "默认标签来自工具配置"))
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
            var title = arguments.GetString("Title") ?? L("WorkspaceNoteTemplate.Execute.UntitledNote", "Untitled note");
            var priority = arguments.GetInteger("Priority", preset.DefaultPriority);
            var pin = arguments.GetBoolean("Pin");
            var tags = arguments.GetJson("Tags");
            var tagText = FormatTags(tags);

            var lines = new List<string>
            {
                $"# {title}",
                LF("WorkspaceNoteTemplate.Execute.PresetFormat", "Preset: {0}", preset.DisplayName),
                LF("WorkspaceNoteTemplate.Execute.PriorityFormat", "Priority: {0}", priority),
                LF("WorkspaceNoteTemplate.Execute.PinnedFormat", "Pinned: {0}", pin),
                LF("WorkspaceNoteTemplate.Execute.TagsFormat", "Tags: {0}", tagText),
                string.Empty,
                L("WorkspaceNoteTemplate.Execute.SummaryHeader", "Summary:")
            };

            if (settings.IncludeContextMetadata)
            {
                var notAvailable = L("WorkspaceNoteTemplate.Execute.NotAvailable", "n/a");
                lines.Insert(3, LF("WorkspaceNoteTemplate.Execute.WorkspaceFormat", "Workspace: {0}", string.IsNullOrWhiteSpace(context.WorkspacePath) ? notAvailable : context.WorkspacePath));
                lines.Insert(4, LF("WorkspaceNoteTemplate.Execute.SessionFormat", "Session: {0}", string.IsNullOrWhiteSpace(context.SessionTitle) ? notAvailable : context.SessionTitle));
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
                return L("WorkspaceNoteTemplate.Execute.None", "none");
            }

            var values = array
                .Values<string>()
                .Where(item => !string.IsNullOrWhiteSpace(item))
                .Select(item => item!.Trim())
                .ToArray();

            return values.Length == 0 ? L("WorkspaceNoteTemplate.Execute.None", "none") : string.Join(", ", values);
        }

        private static SkyweaverToolDefinition CreateDefaultDefinition()
        {
            return new SkyweaverToolDefinition(
                ToolName,
                L("WorkspaceNoteTemplate.Tool.Description", "生成一个工作区笔记模板。宿主可以通过工具自身配置扩展默认值、摘要布局和运行时行为。"),
                "Script",
                [
                    new SkyweaverToolParameterDefinition(
                        "Title",
                        L("WorkspaceNoteTemplate.Tool.Parameter.Title.Description", "笔记标题。"),
                        SkyweaverToolParameterType.String,
                        isRequired: true),
                    new SkyweaverToolParameterDefinition(
                        "Priority",
                        L("WorkspaceNoteTemplate.Tool.Parameter.Priority.Description", "任务优先级，数值越大表示越紧急。"),
                        SkyweaverToolParameterType.Integer,
                        isRequired: false,
                        defaultValue: "1"),
                    new SkyweaverToolParameterDefinition(
                        "Pin",
                        L("WorkspaceNoteTemplate.Tool.Parameter.Pin.Description", "是否置顶该笔记。"),
                        SkyweaverToolParameterType.Boolean,
                        isRequired: false,
                        defaultValue: "false"),
                    new SkyweaverToolParameterDefinition(
                        "Tags",
                        L("WorkspaceNoteTemplate.Tool.Parameter.Tags.Description", "笔记标签的 JSON 数组。省略时使用宿主保存的默认标签。"),
                        SkyweaverToolParameterType.Json,
                        isRequired: false,
                        defaultValue: "[\"memo\"]")
                ]);
        }

        private static string L(string resourceKey, string fallback)
        {
            return LocalizationRuntime.Instance.GetString(resourceKey, fallback);
        }

        private static string LF(string resourceKey, string fallbackFormat, params object?[] args)
        {
            var format = L(resourceKey, fallbackFormat);
            return string.Format(format, args);
        }
    }
}
