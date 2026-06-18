using Ferrita.Controls.NodeEditorControl.Models;
using Ferrita.Infrastructure.Mvvm;

namespace Ferrita.Controls.NodeEditorControl.ViewModels
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
