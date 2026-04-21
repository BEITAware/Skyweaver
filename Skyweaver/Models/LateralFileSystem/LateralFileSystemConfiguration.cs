using Skyweaver.Infrastructure.Mvvm;

namespace Skyweaver.Models.LateralFileSystem
{
    public sealed class LateralFileSystemConfiguration : ObservableObject
    {
        private bool _isEnabled;
        private string _workingRootDirectory = string.Empty;

        public bool IsEnabled
        {
            get => _isEnabled;
            set => SetProperty(ref _isEnabled, value);
        }

        public string WorkingRootDirectory
        {
            get => _workingRootDirectory;
            set => SetProperty(ref _workingRootDirectory, value ?? string.Empty);
        }
    }
}
