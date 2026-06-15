using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Skyweaver.PageControls.Tiles.ViewModels;
using Skyweaver.Services.SkyweaverTools;
using Skyweaver.ViewModels;

namespace Skyweaver.Tools
{
    public sealed class CreateStickyNoteTool : ISkyweaverTool, ISkyweaverToolPromptDescriptionProvider
    {
        public const string ToolName = "CreateStickyNote";

        private static readonly SkyweaverToolDefinition s_definition = new(
            ToolName,
            "Creates a new sticky note tile attached to the '代理' (Agent) group.",
            "StickyNotes",
            new[]
            {
                new SkyweaverToolParameterDefinition(
                    "Content",
                    "The text content of the sticky note.",
                    SkyweaverToolParameterType.String,
                    isRequired: true),
                new SkyweaverToolParameterDefinition(
                    "Size",
                    "The size of the sticky note. Allowed: 1x1, 2x2, 2x1. Defaults to 1x1.",
                    SkyweaverToolParameterType.String,
                    isRequired: false)
            },
            defaultAgentPermission: SkyweaverToolDefaultAgentPermission.RequireConfirmation);

        public SkyweaverToolDefinition Definition => s_definition;

        public string GetPromptDescription(SkyweaverToolPromptDescriptionContext context)
        {
            return "Creates a new sticky note tile with the specified content and size, automatically adding it to the '代理' group.\n" +
                   "### 便笺与日志获取规则 (CreateStickyNote 侧重)：\n" +
                   "- 当你使用此工具创建有关任务的便笺磁贴时，请在便笺内容中写明明确的任务要求。此便笺未来可能会被你或其他代理获取并执行，执行者在任务完成后应写入日志并回复该磁贴以确认完工。\n" +
                   "- 核心规则：代理若发现便笺上有分配的任务并且自身有能力完成，必须协助完成它。任务完成后，必须调用 CreateJournalEntry 写入日志记录状态（以避免其他代理做重复工作），并调用 ReplyStickyNote 回复磁贴以通知用户。如果获取到的便笺内容不属于任务，应予以忽略。";
        }

        public Task<SkyweaverToolResult> ExecuteAsync(
            SkyweaverToolContext context,
            SkyweaverToolArguments arguments,
            CancellationToken cancellationToken = default)
        {
            try
            {
                string? content = arguments.GetString("Content");
                string size = arguments.GetString("Size") ?? "1x1";

                if (string.IsNullOrWhiteSpace(content))
                {
                    return Task.FromResult(SkyweaverToolResult.Failure("The 'Content' parameter is required."));
                }

                bool success = Application.Current.Dispatcher.Invoke(() =>
                {
                    var mainWindow = Application.Current.MainWindow;
                    var mainVm = mainWindow?.DataContext as MainViewModel;
                    var tilesVm = mainVm?.TilesPage;
                    if (tilesVm == null) return false;

                    tilesVm.CreateStickyNoteFromAgent(content, size);
                    return true;
                });

                if (success)
                {
                    return Task.FromResult(SkyweaverToolResult.Success("Sticky note created successfully."));
                }
                else
                {
                    return Task.FromResult(SkyweaverToolResult.Failure("Could not access Tiles page view model."));
                }
            }
            catch (Exception ex)
            {
                return Task.FromResult(SkyweaverToolResult.Failure($"Failed to create sticky note: {ex.Message}"));
            }
        }
    }
}
