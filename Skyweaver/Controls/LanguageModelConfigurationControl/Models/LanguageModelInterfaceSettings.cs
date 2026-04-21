using Skyweaver.Infrastructure.Mvvm;

namespace Skyweaver.Controls.LanguageModelConfigurationControl.Models
{
    public abstract class LanguageModelInterfaceSettings : ObservableObject
    {
        public abstract string InterfaceType { get; }

        public abstract bool IsFullyConfigured { get; }

        public abstract string SummaryModelId { get; }
    }
}
