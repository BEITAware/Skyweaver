using Ferrita.Infrastructure.Mvvm;
using Ferrita.Services.Localization;

namespace Ferrita.Panels.NodeSettings.ViewModels
{
    public sealed class NodeSettingsPanelViewModel : ObservableObject
    {
        public string Title => L("NodeSettingsPanel.Title", "节点参数设定");

        public string Description => L("NodeSettingsPanel.Description", "该面板已从主窗口布局中拆出，后续可独立承载节点属性、参数编辑器和校验状态。");

        private static string L(string resourceKey, string fallback)
        {
            return LocalizationRuntime.Instance.GetString(resourceKey, fallback);
        }
    }
}
