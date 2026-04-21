using Skyweaver.Infrastructure.Mvvm;

namespace Skyweaver.Panels.Filmstrip.ViewModels
{
    public sealed class FilmstripPanelViewModel : ObservableObject
    {
        public string Title => "胶片条";

        public string Description => "该面板已独立成模块，后续可挂接缩略图时间线、历史快照或输出预览。";
    }
}
