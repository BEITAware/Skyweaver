using Skyweaver.Controls.MultiFunctionPageBase.Models;
using Skyweaver.Infrastructure.Mvvm;

namespace Skyweaver.Controls.SkyweaverPreferencesControl.ViewModels
{
    public sealed class SkyweaverPreferencesControlViewModel : ObservableObject
    {
        public string Title { get; } = "Skyweaver首选项";

        public string Description { get; } = "统一承载 Skyweaver 的全局偏好设置、外观和默认行为。";

        public string Hint { get; } = "后续可以直接补充真实设置分组、表单状态与保存逻辑。";

        public PageScaffoldModel Scaffold { get; } = new()
        {
            EmptyStateTitle = "首选项骨架已就位",
            EmptyStateDescription = "当前页面已经独立迁入 Controls，可继续补充通用设置、模型配置与工作区默认值。",
            EmptyStateHint = "建议未来按分类拆成外观、运行、模型、路径等配置分区。"
        };
    }
}
