using Ferrita.Infrastructure.Mvvm;
using Ferrita.Services.Localization;

namespace Ferrita.Panels.Filmstrip.ViewModels
{
    public sealed class FilmstripPanelViewModel : ObservableObject
    {
        public string Title => L("FilmstripPanel.Title", "胶片条");

        public string Description => L("FilmstripPanel.Description", "该面板已独立成模块，后续可挂接缩略图时间线、历史快照或输出预览。");

        private static string L(string resourceKey, string fallback)
        {
            return LocalizationRuntime.Instance.GetString(resourceKey, fallback);
        }
    }
}
