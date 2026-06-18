using Ferrita.Infrastructure.Mvvm;

namespace Ferrita.Controls.FerritaPreferencesControl.ViewModels
{
    public abstract class PreferencePageViewModelBase : ObservableObject
    {
        public abstract string PageId { get; }
    }
}
