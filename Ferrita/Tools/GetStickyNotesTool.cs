using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Ferrita.PageControls.Tiles.ViewModels;
using Ferrita.Services.FerritaTools;
using Ferrita.Services.StickyNotes;
using Ferrita.ViewModels;

namespace Ferrita.Tools
{
    public sealed class GetStickyNotesTool : IFerritaTool, IFerritaToolPromptDescriptionProvider
    {
        public const string ToolName = "GetStickyNotes";

        private static readonly FerritaToolDefinition s_definition = new(
            ToolName,
            "Gets all sticky notes (their codes and contents) and the sticky notes journal.",
            "StickyNotes",
            Array.Empty<FerritaToolParameterDefinition>(),
            defaultAgentPermission: FerritaToolDefaultAgentPermission.Allow);

        public FerritaToolDefinition Definition => s_definition;

        public string GetPromptDescription(FerritaToolPromptDescriptionContext context)
        {
            return "Gets the codes and contents of all sticky note tiles, along with all journal entries in StickyNotesJournal.xml.\n" +
                   "### 便笺与日志获取规则 (GetStickyNotes 侧重)：\n" +
                   "- 当你通过此工具获取到便笺内容和日志后，应重点检查其中是否存在尚未完成且你能够完成的任务。如果存在，必须主动推动该任务的完成。\n" +
                   "- 核心规则：代理若发现便笺上有分配的任务并且自身有能力完成，必须协助完成它。任务完成后，必须调用 CreateJournalEntry 写入日志记录状态（以避免其他代理做重复工作），并调用 ReplyStickyNote 回复磁贴以通知用户。如果获取到的便笺内容不属于任务，应予以忽略。";
        }

        public Task<FerritaToolResult> ExecuteAsync(
            FerritaToolContext context,
            FerritaToolArguments arguments,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var tiles = Application.Current.Dispatcher.Invoke(() =>
                {
                    var mainWindow = Application.Current.MainWindow;
                    var mainVm = mainWindow?.DataContext as MainViewModel;
                    var tilesVm = mainVm?.TilesPage;
                    if (tilesVm == null) return Array.Empty<object>();

                    return tilesVm.Tiles
                        .Where(t => t.IsStickyNote)
                        .Select(t => new
                        {
                            Code = t.Code,
                            Content = t.StickyNoteText,
                            Color = t.StickyNoteColor
                        })
                        .ToArray();
                });

                var journalEntries = StickyNotesService.GetJournalEntries()
                    .Select(e => new
                    {
                        Creator = e.Creator,
                        DateTime = e.DateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                        Content = e.Content
                    })
                    .ToArray();

                var resultObj = new
                {
                    StickyNotes = tiles,
                    Journal = journalEntries
                };

                string jsonResult = JsonSerializer.Serialize(resultObj, new JsonSerializerOptions { WriteIndented = true });
                return Task.FromResult(FerritaToolResult.Success(jsonResult));
            }
            catch (Exception ex)
            {
                return Task.FromResult(FerritaToolResult.Failure($"Failed to get sticky notes: {ex.Message}"));
            }
        }
    }
}
