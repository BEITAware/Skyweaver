using System;
using System.Threading;
using System.Threading.Tasks;
using Ferrita.Services.FerritaTools;
using Ferrita.Services.StickyNotes;

namespace Ferrita.Tools
{
    public sealed class ReplyStickyNoteTool : IFerritaTool, IFerritaToolPromptDescriptionProvider
    {
        public const string ToolName = "ReplyStickyNote";

        private static readonly FerritaToolDefinition s_definition = new(
            ToolName,
            "Replies to a sticky note tile by its code.",
            "StickyNotes",
            new[]
            {
                new FerritaToolParameterDefinition(
                    "TileCode",
                    "The code of the sticky note tile to reply to.",
                    FerritaToolParameterType.String,
                    isRequired: true),
                new FerritaToolParameterDefinition(
                    "ReplyContent",
                    "The reply content.",
                    FerritaToolParameterType.String,
                    isRequired: true)
            },
            defaultAgentPermission: FerritaToolDefaultAgentPermission.Allow);

        public FerritaToolDefinition Definition => s_definition;

        public string GetPromptDescription(FerritaToolPromptDescriptionContext context)
        {
            return "Replies to a specific sticky note. This triggers an Aero-style exclamation warning icon on the tile for 3 days or until hover/read.\n" +
                   "### 便笺与日志获取规则 (ReplyStickyNote 侧重)：\n" +
                   "- 当你（代理）完成了便笺磁贴上分派的任务后，除了使用 CreateJournalEntry 写入日志以更新状态，你必须调用此 ReplyStickyNote 工具对该磁贴进行回复，以向用户同步最新的完成结果并清除或置位未读图标。\n" +
                   "- 核心规则：代理若发现便笺上有分配的任务并且自身有能力完成，必须协助完成它。任务完成后，必须调用 CreateJournalEntry 写入日志记录状态（以避免其他代理做重复工作），并调用 ReplyStickyNote 回复磁贴以通知用户。如果获取到的便笺内容不属于任务，应予以忽略。";
        }

        public Task<FerritaToolResult> ExecuteAsync(
            FerritaToolContext context,
            FerritaToolArguments arguments,
            CancellationToken cancellationToken = default)
        {
            try
            {
                string? tileCode = arguments.GetString("TileCode");
                string? content = arguments.GetString("ReplyContent");

                if (string.IsNullOrWhiteSpace(tileCode) || string.IsNullOrWhiteSpace(content))
                {
                    return Task.FromResult(FerritaToolResult.Failure("Both 'TileCode' and 'ReplyContent' are required parameters."));
                }

                string creator = context.CurrentAgent?.DisplayNameOrFallback ?? "Agent";
                StickyNotesService.AddReply(tileCode, creator, content);

                return Task.FromResult(FerritaToolResult.Success($"Successfully replied to sticky note '{tileCode}'."));
            }
            catch (Exception ex)
            {
                return Task.FromResult(FerritaToolResult.Failure($"Failed to reply to sticky note: {ex.Message}"));
            }
        }
    }
}
