using Skyweaver.Controls.NodeEditorControl.Models;
using Skyweaver.Infrastructure.Mvvm;

namespace Skyweaver.Controls.NodeEditorControl.ViewModels
{
    public sealed class NodeEditorControlViewModel : ObservableObject
    {
        private NodeEditorSurfaceModel _surface = new();

        public NodeEditorSurfaceModel Surface
        {
            get => _surface;
            set => SetProperty(ref _surface, value);
        }
    }
}
