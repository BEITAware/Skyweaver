using Skyweaver.Infrastructure.Mvvm;

namespace Skyweaver.Controls.SkyweaverPreferencesControl.ViewModels
{
    public abstract class PreferencePageViewModelBase : ObservableObject
    {
        public abstract string PageId { get; }
    }
}
