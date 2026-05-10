using System.Text;
using System.Windows;
using Newtonsoft.Json.Linq;
using Skyweaver.Controls.ChatSessionControl.Views;
using Skyweaver.Services.SkyweaverTools;

namespace Skyweaver.Tools
{
    public sealed class LoadToolKitsTool : ISkyweaverTool, ISkyweaverToolInvocationPresentationProvider, ISkyweaverToolPromptDescriptionProvider
    {
        public const string ToolName = "LoadToolKits";

        private readonly SkyweaverToolKitService _toolKitService;
        private readonly SkyweaverPromptToolCatalogService _promptToolCatalogService;

        private static readonly SkyweaverToolDefinition s_definition = new(
            ToolName,
            "加载若干个工具集，并让这些工具集中的工具在后续代理迭代中可调用。",
            "GuideBot",
            [
                new SkyweaverToolParameterDefinition(
                    "ToolKits",
                    "要加载的工具集名称 JSON 数组。例如：[\"文件工具\", \"笔记工具\"]。",
                    SkyweaverToolParameterType.Json,
                    isRequired: true)
            ],
            isSystemTool: false,
            canBelongToToolKit: false,
            defaultAgentPermission: SkyweaverToolDefaultAgentPermission.Allow);

        public LoadToolKitsTool()
            : this(new SkyweaverToolKitService(), new SkyweaverPromptToolCatalogService())
        {
        }

        public LoadToolKitsTool(
            SkyweaverToolKitService toolKitService,
            SkyweaverPromptToolCatalogService promptToolCatalogService)
        {
            _toolKitService = toolKitService ?? throw new ArgumentNullException(nameof(toolKitService));
            _promptToolCatalogService = promptToolCatalogService ?? throw new ArgumentNullException(nameof(promptToolCatalogService));
        }

        public SkyweaverToolDefinition Definition => s_definition;

        public string GetPromptDescription(SkyweaverToolPromptDescriptionContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            var availableToolKitNames = context.AvailableToolKits
                .Select(GetToolKitDisplayName)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(item => item, StringComparer.OrdinalIgnoreCase)
                .ToArray();

            if (availableToolKitNames.Length == 0)
            {
                return "加载若干个工具集，并让这些工具集中的工具从后续代理迭代开始可调用。当前没有可用工具集。";
            }

            return $"加载若干个工具集，并让这些工具集中的工具从后续代理迭代开始可调用。当前可用工具集：{string.Join("、", availableToolKitNames)}。调用回填会附带新增工具及其说明。";
        }

        public FrameworkElement? CreateInvocationPresentation(SkyweaverToolInvocationPresentationContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            return ToolInvocationCardFactory.Create(
                context,
                [
                    new ToolInvocationCardFieldDefinition("工具集", "ToolKits", "填写工具集名称 JSON 数组")
                ]);
        }

        public Task<SkyweaverToolResult> ExecuteAsync(
            SkyweaverToolContext context,
            SkyweaverToolArguments arguments,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var requestedNames = ExtractRequestedToolKitNames(arguments.GetJson("ToolKits"));
            if (requestedNames.Count == 0)
            {
                return Task.FromResult(SkyweaverToolResult.Failure(
                    "LoadToolKits 需要至少一个工具集名称。",
                    new Dictionary<string, object?>
                    {
                        ["requestedToolKits"] = new JArray()
                    }));
            }

            var availableToolKits = context.AvailableToolKits.Count > 0
                ? context.AvailableToolKits.Select(toolKit => toolKit.DeepClone()).ToArray()
                : _toolKitService.Load();
            var resolution = _toolKitService.ResolveByNames(requestedNames, availableToolKits);
            var loadedToolKits = resolution.LoadedToolKits;
            var loadedToolKitKeys = loadedToolKits
                .Select(toolKit => toolKit.Key)
                .Where(key => !string.IsNullOrWhiteSpace(key))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();
            var toolkitToolNames = loadedToolKits
                .SelectMany(toolKit => toolKit.Tools)
                .Select(tool => tool.ToolName?.Trim() ?? string.Empty)
                .Where(toolName => toolName.Length > 0)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();

            var callableTools = context.CurrentAgent == null
                ? Array.Empty<SkyweaverPromptToolDefinition>()
                : _promptToolCatalogService.ResolveCallableTools(
                    context.CurrentAgent,
                    context.SupportsHostToolConfirmation,
                    activeToolKitKeys: loadedToolKitKeys,
                    restrictToToolNames: toolkitToolNames,
                    availableToolKits: availableToolKits);

            var skippedToolNames = toolkitToolNames
                .Except(callableTools.Select(tool => tool.Name), StringComparer.OrdinalIgnoreCase)
                .OrderBy(item => item, StringComparer.OrdinalIgnoreCase)
                .ToArray();

            var content = BuildResultContent(
                loadedToolKits,
                callableTools,
                skippedToolNames,
                resolution.MissingNames,
                resolution.AmbiguousNames);
            var data = new Dictionary<string, object?>
            {
                ["requestedToolKits"] = new JArray(requestedNames),
                ["loadedToolKitKeys"] = new JArray(loadedToolKits.Select(toolKit => toolKit.Key)),
                ["loadedToolKitNames"] = new JArray(loadedToolKits.Select(GetToolKitDisplayName)),
                ["callableToolNames"] = new JArray(callableTools.Select(tool => tool.Name)),
                ["skippedToolNames"] = new JArray(skippedToolNames),
                ["missingToolKits"] = new JArray(resolution.MissingNames),
                ["ambiguousToolKits"] = new JArray(resolution.AmbiguousNames)
            };

            return Task.FromResult(loadedToolKits.Count > 0
                ? SkyweaverToolResult.Success(content, data)
                : SkyweaverToolResult.Failure(content, data));
        }

        private static IReadOnlyList<string> ExtractRequestedToolKitNames(JToken? token)
        {
            if (token is not JArray array)
            {
                return Array.Empty<string>();
            }

            return array
                .Values<string>()
                .Where(item => !string.IsNullOrWhiteSpace(item))
                .Select(item => item!.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        private static string BuildResultContent(
            IReadOnlyList<SkyweaverToolKitDefinition> loadedToolKits,
            IReadOnlyList<SkyweaverPromptToolDefinition> callableTools,
            IReadOnlyList<string> skippedToolNames,
            IReadOnlyList<string> missingToolKitNames,
            IReadOnlyList<string> ambiguousToolKitNames)
        {
            var builder = new StringBuilder();

            if (loadedToolKits.Count > 0)
            {
                builder.AppendLine($"已加载 {loadedToolKits.Count} 个工具集：{string.Join("、", loadedToolKits.Select(GetToolKitDisplayName))}");

                if (callableTools.Count > 0)
                {
                    builder.AppendLine("从下一次代理迭代开始可调用的工具：");
                    SkyweaverToolPromptSupport.AppendToolListing(builder, callableTools);
                }
                else
                {
                    builder.AppendLine("这些工具集当前没有为该代理新增任何可调用工具。");
                }
            }
            else
            {
                builder.AppendLine("未能加载任何工具集。");
            }

            if (skippedToolNames.Count > 0)
            {
                builder.AppendLine();
                builder.AppendLine("以下工具集成员未加入可调用列表：");
                foreach (var toolName in skippedToolNames)
                {
                    builder.AppendLine($"- {toolName}");
                }

                builder.AppendLine("这些工具通常是因为已被禁用、当前代理没有权限，或运行时不支持所需的 Host 确认。");
            }

            if (missingToolKitNames.Count > 0)
            {
                builder.AppendLine();
                builder.AppendLine("未找到的工具集：");
                foreach (var toolKitName in missingToolKitNames)
                {
                    builder.AppendLine($"- {toolKitName}");
                }
            }

            if (ambiguousToolKitNames.Count > 0)
            {
                builder.AppendLine();
                builder.AppendLine("名称不唯一、无法确定的工具集：");
                foreach (var toolKitName in ambiguousToolKitNames)
                {
                    builder.AppendLine($"- {toolKitName}");
                }
            }

            return builder.ToString().TrimEnd();
        }

        private static string GetToolKitDisplayName(SkyweaverToolKitDefinition toolKit)
        {
            return string.IsNullOrWhiteSpace(toolKit.Name)
                ? toolKit.DisplayNameOrFallback
                : toolKit.Name.Trim();
        }
    }
}
