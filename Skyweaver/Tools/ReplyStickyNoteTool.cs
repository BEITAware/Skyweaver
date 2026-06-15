using System;
using System.Threading;
using System.Threading.Tasks;
using Skyweaver.Services.SkyweaverTools;
using Skyweaver.Services.StickyNotes;

namespace Skyweaver.Tools
{
    public sealed class ReplyStickyNoteTool : ISkyweaverTool, ISkyweaverToolPromptDescriptionProvider
    {
        public const string ToolName = "ReplyStickyNote";

        private static readonly SkyweaverToolDefinition s_definition = new(
            ToolName,
            "Replies to a sticky note tile by its code.",
            "StickyNotes",
            new[]
            {
                new SkyweaverToolParameterDefinition(
                    "TileCode",
                    "The code of the sticky note tile to reply to.",
                    SkyweaverToolParameterType.String,
                    isRequired: true),
                new SkyweaverToolParameterDefinition(
                    "ReplyContent",
                    "The reply content.",
                    SkyweaverToolParameterType.String,
                    isRequired: true)
            },
            defaultAgentPermission: SkyweaverToolDefaultAgentPermission.Allow);

        public SkyweaverToolDefinition Definition => s_definition;

        public string GetPromptDescription(SkyweaverToolPromptDescriptionContext context)
        {
            return "Replies to a specific sticky note. This triggers an Aero-style exclamation warning icon on the tile for 3 days or until hover/read.\n" +
                   "### 便笺与日志获取规则 (ReplyStickyNote 侧重)：\n" +
                   "- 当你（代理）完成了便笺磁贴上分派的任务后，除了使用 CreateJournalEntry 写入日志以更新状态，你必须调用此 ReplyStickyNote 工具对该磁贴进行回复，以向用户同步最新的完成结果并清除或置位未读图标。\n" +
                   "- 核心规则：代理若发现便笺上有分配的任务并且自身有能力完成，必须协助完成它。任务完成后，必须调用 CreateJournalEntry 写入日志记录状态（以避免其他代理做重复工作），并调用 ReplyStickyNote 回复磁贴以通知用户。如果获取到的便笺内容不属于任务，应予以忽略。";
        }

        public Task<SkyweaverToolResult> ExecuteAsync(
            SkyweaverToolContext context,
            SkyweaverToolArguments arguments,
            CancellationToken cancellationToken = default)
        {
            try
            {
                string? tileCode = arguments.GetString("TileCode");
                string? content = arguments.GetString("ReplyContent");

                if (string.IsNullOrWhiteSpace(tileCode) || string.IsNullOrWhiteSpace(content))
                {
                    return Task.FromResult(SkyweaverToolResult.Failure("Both 'TileCode' and 'ReplyContent' are required parameters."));
                }

                string creator = context.CurrentAgent?.DisplayNameOrFallback ?? "Agent";
                StickyNotesService.AddReply(tileCode, creator, content);

                return Task.FromResult(SkyweaverToolResult.Success($"Successfully replied to sticky note '{tileCode}'."));
            }
            catch (Exception ex)
            {
                return Task.FromResult(SkyweaverToolResult.Failure($"Failed to reply to sticky note: {ex.Message}"));
            }
        }
    }
}
