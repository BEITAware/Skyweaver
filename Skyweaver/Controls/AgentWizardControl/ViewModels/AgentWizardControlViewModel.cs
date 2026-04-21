using Skyweaver.Controls.MultiFunctionPageBase.Models;
using Skyweaver.Infrastructure.Mvvm;

namespace Skyweaver.Controls.AgentWizardControl.ViewModels
{
    public sealed class AgentWizardControlViewModel : ObservableObject
    {
        public string Title { get; } = "创建代理向导";

        public string Description { get; } = "集中承载代理创建、职责配置和模板初始化的多步骤流程。";

        public string Hint { get; } = "后续可以继续扩展步骤状态、校验结果和最终生成物。";

        public PageScaffoldModel Scaffold { get; } = new()
        {
            EmptyStateTitle = "代理向导骨架已就位",
            EmptyStateDescription = "当前页面已经独立迁入 Controls，未来可以在这个入口上继续搭建真实向导流程。",
            EmptyStateHint = "建议后续将步骤定义、表单数据和校验结果拆成独立模型。"
        };
    }
}
