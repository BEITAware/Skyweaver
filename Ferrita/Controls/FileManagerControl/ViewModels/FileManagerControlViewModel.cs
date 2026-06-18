using Ferrita.Controls.MultiFunctionPageBase.Models;
using Ferrita.Infrastructure.Mvvm;
using Ferrita.Services.Localization;

namespace Ferrita.Controls.FileManagerControl.ViewModels
{
    public sealed class FileManagerControlViewModel : ObservableObject
    {
        public string Title { get; }

        public string Description { get; } = L("FileManager.Description", "适合承载工程目录浏览、筛选、文件操作与预览联动。");

        public string Hint { get; } = L("FileManager.Hint", "后续可以继续扩展路径状态、工具栏命令和多选操作。");

        public PageScaffoldModel Scaffold { get; }

        public FileManagerControlViewModel(int instanceNumber)
        {
            Title = instanceNumber > 1
                ? LF("FileManager.Title.NumberedFormat", "文件管理器 {0}", instanceNumber)
                : L("FileManager.Title", "文件管理器");
            Scaffold = new PageScaffoldModel
            {
                EmptyStateTitle = L("FileManager.EmptyState.Title", "文件管理器入口已拆分"),
                EmptyStateDescription = L("FileManager.EmptyState.Description", "页面已经独立到 Controls 下，后续可以直接接文件树、列表区、预览区和命令条。"),
                EmptyStateHint = L("FileManager.EmptyState.Hint", "建议未来把路径状态、筛选条件和选中项抽成独立模型。")
            };
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
