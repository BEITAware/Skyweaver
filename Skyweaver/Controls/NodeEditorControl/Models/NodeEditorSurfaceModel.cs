using Skyweaver.Infrastructure.Mvvm;

namespace Skyweaver.Controls.NodeEditorControl.Models
{
    public sealed class NodeEditorSurfaceModel : ObservableObject
    {
        private double _canvasWidth = 8000;
        private double _canvasHeight = 8000;
        private double _scale = 1;
        private double _offsetX;
        private double _offsetY;

        public double CanvasWidth
        {
            get => _canvasWidth;
            set => SetProperty(ref _canvasWidth, value);
        }

        public double CanvasHeight
        {
            get => _canvasHeight;
            set => SetProperty(ref _canvasHeight, value);
        }

        public double Scale
        {
            get => _scale;
            set => SetProperty(ref _scale, value);
        }

        public double OffsetX
        {
            get => _offsetX;
            set => SetProperty(ref _offsetX, value);
        }

        public double OffsetY
        {
            get => _offsetY;
            set => SetProperty(ref _offsetY, value);
        }
    }
}
