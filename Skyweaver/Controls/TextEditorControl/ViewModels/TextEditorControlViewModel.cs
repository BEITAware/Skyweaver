using Skyweaver.Controls.MultiFunctionPageBase.Models;
using Skyweaver.Infrastructure.Mvvm;

namespace Skyweaver.Controls.TextEditorControl.ViewModels
{
    public sealed class TextEditorControlViewModel : ObservableObject
    {
        public string Title { get; }

        public string Description { get; } = "适合承载脚本、提示词、配置片段和说明文本的编辑工作页。";

        public string Hint { get; } = "后续可以继续补充文档内容、脏状态、保存命令和语法高亮适配。";

        public PageScaffoldModel Scaffold { get; }

        public TextEditorControlViewModel(int instanceNumber)
        {
            Title = instanceNumber > 1 ? $"文本编辑器 {instanceNumber}" : "文本编辑器";
            Scaffold = new PageScaffoldModel
            {
                EmptyStateTitle = "文本编辑页骨架已独立",
                EmptyStateDescription = "该页面已经从多功能工作区临时占位中拆出，后续可以直接接入真实编辑器内核。",
                EmptyStateHint = "如果未来支持多文档和保存流程，可以在 ViewModel 中管理文档状态。"
            };
        }
    }
}
