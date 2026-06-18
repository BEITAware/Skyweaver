using Ferrita.Infrastructure.Mvvm;

namespace Ferrita.Controls.AgentConfigurationControl.Models
{
    public sealed class AgentToolKitSelectionDefinition : ObservableObject
    {
        private string _toolKitKey = string.Empty;

        public string ToolKitKey
        {
            get => _toolKitKey;
            set => SetProperty(ref _toolKitKey, value?.Trim() ?? string.Empty);
        }
    }
}
