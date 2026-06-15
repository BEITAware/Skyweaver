using System;
using System.Threading;
using System.Threading.Tasks;
using Skyweaver.Services.SkyweaverTools;
using Skyweaver.Services.StickyNotes;

namespace Skyweaver.Tools
{
    public sealed class CreateJournalEntryTool : ISkyweaverTool, ISkyweaverToolPromptDescriptionProvider
    {
        public const string ToolName = "CreateJournalEntry";

        private static readonly SkyweaverToolDefinition s_definition = new(
            ToolName,
            "Creates a new journal entry in the sticky notes journal.",
            "StickyNotes",
            new[]
            {
                new SkyweaverToolParameterDefinition(
                    "Content",
                    "The text content of the journal entry.",
                    SkyweaverToolParameterType.String,
                    isRequired: true)
            },
            defaultAgentPermission: SkyweaverToolDefaultAgentPermission.Allow);

        public SkyweaverToolDefinition Definition => s_definition;

        public string GetPromptDescription(SkyweaverToolPromptDescriptionContext context)
        {
            return "Creates a new entry in StickyNotesJournal.xml, recording your agent name, timestamp, and detailed status/message.\n" +
                   "### 便笺与日志获取规则 (CreateJournalEntry 侧重)：\n" +
                   "- 当你（代理）完成了便笺磁贴上分派的任务后，你必须调用此 CreateJournalEntry 工具向日志中记录一条已完成的日志。这有助于使其他代理在未来的运行中获知你已接手并完成了任务，从而避免代理之间的重复工作。\n" +
                   "- 核心规则：代理若发现便笺上有分配的任务并且自身有能力完成，必须协助完成它. 任务完成后，必须调用 CreateJournalEntry 写入日志记录状态（以避免其他代理做重复工作），并调用 ReplyStickyNote 回复磁贴以通知用户。如果获取到的便笺内容不属于任务，应予以忽略。";
        }

        public Task<SkyweaverToolResult> ExecuteAsync(
            SkyweaverToolContext context,
            SkyweaverToolArguments arguments,
            CancellationToken cancellationToken = default)
        {
            try
            {
                string? content = arguments.GetString("Content");
                if (string.IsNullOrWhiteSpace(content))
                {
                    return Task.FromResult(SkyweaverToolResult.Failure("The 'Content' parameter is required."));
                }

                string creator = context.CurrentAgent?.DisplayNameOrFallback ?? "Agent";
                StickyNotesService.AddJournalEntry(creator, content);

                return Task.FromResult(SkyweaverToolResult.Success("Journal entry created successfully."));
            }
            catch (Exception ex)
            {
                return Task.FromResult(SkyweaverToolResult.Failure($"Failed to create journal entry: {ex.Message}"));
            }
        }
    }
}
