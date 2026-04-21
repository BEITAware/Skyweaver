using Skyweaver.Controls.MultiFunctionPageBase.Models;
using Skyweaver.Infrastructure.Mvvm;

namespace Skyweaver.Controls.FileManagerControl.ViewModels
{
    public sealed class FileManagerControlViewModel : ObservableObject
    {
        public string Title { get; }

        public string Description { get; } = "适合承载工程目录浏览、筛选、文件操作与预览联动。";

        public string Hint { get; } = "后续可以继续扩展路径状态、工具栏命令和多选操作。";

        public PageScaffoldModel Scaffold { get; }

        public FileManagerControlViewModel(int instanceNumber)
        {
            Title = instanceNumber > 1 ? $"文件管理器 {instanceNumber}" : "文件管理器";
            Scaffold = new PageScaffoldModel
            {
                EmptyStateTitle = "文件管理器入口已拆分",
                EmptyStateDescription = "页面已经独立到 Controls 下，后续可以直接接文件树、列表区、预览区和命令条。",
                EmptyStateHint = "建议未来把路径状态、筛选条件和选中项抽成独立模型。"
            };
        }
    }
}
