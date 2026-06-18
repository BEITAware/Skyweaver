using Ferrita.Controls.MultiFunctionPageBase.Models;
using Ferrita.Infrastructure.Mvvm;
using Ferrita.Services.Localization;

namespace Ferrita.Controls.AgentWizardControl.ViewModels
{
    public sealed class AgentWizardControlViewModel : ObservableObject
    {
        public string Title { get; } = L("AgentWizard.Title", "创建代理向导");

        public string Description { get; } = L("AgentWizard.Description", "集中承载代理创建、职责配置和模板初始化的多步骤流程。");

        public string Hint { get; } = L("AgentWizard.Hint", "后续可以继续扩展步骤状态、校验结果和最终生成物。");

        public PageScaffoldModel Scaffold { get; } = new()
        {
            EmptyStateTitle = L("AgentWizard.EmptyState.Title", "代理向导骨架已就位"),
            EmptyStateDescription = L("AgentWizard.EmptyState.Description", "当前页面已经独立迁入 Controls，未来可以在这个入口上继续搭建真实向导流程。"),
            EmptyStateHint = L("AgentWizard.EmptyState.Hint", "建议后续将步骤定义、表单数据和校验结果拆成独立模型。")
        };

        private static string L(string resourceKey, string fallback)
        {
            return LocalizationRuntime.Instance.GetString(resourceKey, fallback);
        }
    }
}
