using Ferrita.Infrastructure.Mvvm;

namespace Ferrita.Models.PresentationUI
{
    public sealed class PresentationUIConfiguration : ObservableObject
    {
        private bool _collapseReasoningByDefault = true;

        public bool CollapseReasoningByDefault
        {
            get => _collapseReasoningByDefault;
            set => SetProperty(ref _collapseReasoningByDefault, value);
        }
    }
}
