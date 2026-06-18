using Ferrita.Controls.FerritaPreferencesControl.Models;
using Ferrita.Infrastructure.Mvvm;
using Ferrita.Services.Localization;

namespace Ferrita.Controls.FerritaPreferencesControl.ViewModels
{
    public sealed class SelectablePreferencePageViewModel : ObservableObject
    {
        private bool _isSelected;

        public SelectablePreferencePageViewModel(PreferencePageInfo pageInfo)
        {
            PageInfo = pageInfo;
            LocalizationRuntime.Instance.LanguageChanged += (_, _) => OnPropertyChanged(nameof(DisplayName));
        }

        public PreferencePageInfo PageInfo { get; }

        public string Id => PageInfo.Id;

        public string DisplayName => LocalizationRuntime.Instance.GetString(PageInfo.DisplayNameResourceKey, PageInfo.DisplayName);

        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }
    }
}
