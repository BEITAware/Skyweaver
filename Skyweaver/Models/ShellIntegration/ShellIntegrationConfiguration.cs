using Skyweaver.Infrastructure.Mvvm;

namespace Skyweaver.Models.ShellIntegration
{
    public sealed class ShellIntegrationConfiguration : ObservableObject
    {
        private bool _isEnabled;
        private string _sessionFlowGraphId = string.Empty;
        private string _sessionFlowGraphName = string.Empty;
        private string _sessionFlowFilePath = string.Empty;

        public bool IsEnabled
        {
            get => _isEnabled;
            set => SetProperty(ref _isEnabled, value);
        }

        public string SessionFlowGraphId
        {
            get => _sessionFlowGraphId;
            set => SetProperty(ref _sessionFlowGraphId, value?.Trim() ?? string.Empty);
        }

        public string SessionFlowGraphName
        {
            get => _sessionFlowGraphName;
            set => SetProperty(ref _sessionFlowGraphName, value?.Trim() ?? string.Empty);
        }

        public string SessionFlowFilePath
        {
            get => _sessionFlowFilePath;
            set => SetProperty(ref _sessionFlowFilePath, value?.Trim() ?? string.Empty);
        }
    }
}
