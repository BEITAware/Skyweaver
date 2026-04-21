using Skyweaver.Controls.MultiFunctionPageBase.Models;
using Skyweaver.Infrastructure.Mvvm;

namespace Skyweaver.Controls.LateralFileSystemTreeControl.ViewModels
{
    public sealed class LateralFileSystemTreeControlViewModel : ObservableObject
    {
        public string Title { get; }

        public string Description { get; } = "适合承载紧凑目录树、快捷导航和资源定位入口。";

        public string Hint { get; } = "后续可以接入树节点懒加载、选择同步和上下文菜单。";

        public PageScaffoldModel Scaffold { get; }

        public LateralFileSystemTreeControlViewModel(int instanceNumber)
        {
            Title = instanceNumber > 1 ? $"侧向文件系统树 {instanceNumber}" : "侧向文件系统树";
            Scaffold = new PageScaffoldModel
            {
                EmptyStateTitle = "侧向树控件骨架已就位",
                EmptyStateDescription = "这个页面已经具备独立文件夹与 MVVM 入口，可以继续演化为资源导航侧栏。",
                EmptyStateHint = "后续可与文件管理器共享同一份目录状态。"
            };
        }
    }
}
