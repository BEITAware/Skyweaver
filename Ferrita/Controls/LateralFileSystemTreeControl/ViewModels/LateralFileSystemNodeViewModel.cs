using Ferrita.Infrastructure.Mvvm;
using Ferrita.Models.LateralFileSystem;
using System.Linq;

namespace Ferrita.Controls.LateralFileSystemTreeControl.ViewModels
{
    public class LateralFileSystemNodeViewModel : ObservableObject
    {
        private readonly LateralFileSystemNodeModel _model;

        public LateralFileSystemNodeViewModel(LateralFileSystemNodeModel model)
        {
            _model = model;
            var xProp = _model.Properties.FirstOrDefault(p => p.Key == "VisualX");
            var yProp = _model.Properties.FirstOrDefault(p => p.Key == "VisualY");
            if (xProp != null && double.TryParse(xProp.Value, out double x)) _x = x;
            if (yProp != null && double.TryParse(yProp.Value, out double y)) _y = y;
        }

        public LateralFileSystemNodeModel Model => _model;

        public string Id => _model.Id;

        public string Name
        {
            get => _model.Name;
            set { _model.Name = value; OnPropertyChanged(); }
        }

        public string VirtualRootPath
        {
            get => _model.VirtualRootPath;
            set { _model.VirtualRootPath = value; OnPropertyChanged(); }
        }

        public string Owner
        {
            get => _model.Owner;
            set { _model.Owner = value; OnPropertyChanged(); }
        }

        public LateralFileSystemNodeKind Kind
        {
            get => _model.Kind;
            set { _model.Kind = value; OnPropertyChanged(); }
        }

        public string? ProjectionSourcePath
        {
            get => _model.ProjectionSourcePath;
            set { _model.ProjectionSourcePath = value; OnPropertyChanged(); }
        }

        public string? ParentNodeId
        {
            get => _model.ParentNodeId;
            set { _model.ParentNodeId = value; OnPropertyChanged(); }
        }

        public bool IsActive
        {
            get => _model.IsActive;
            set { _model.IsActive = value; OnPropertyChanged(); }
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }

        private double _x;
        public double X
        {
            get => _x;
            set
            {
                if (SetProperty(ref _x, value))
                {
                    SaveProperty("VisualX", value.ToString());
                    OnPropertyChanged(nameof(CenterX));
                }
            }
        }

        private double _y;
        public double Y
        {
            get => _y;
            set
            {
                if (SetProperty(ref _y, value))
                {
                    SaveProperty("VisualY", value.ToString());
                    OnPropertyChanged(nameof(CenterY));
                }
            }
        }

        public double CenterX => X + 100;
        public double CenterY => Y + 50;

        private void SaveProperty(string key, string value)
        {
            var prop = _model.Properties.FirstOrDefault(p => p.Key == key);
            if (prop == null)
            {
                prop = new LateralFileSystemNodeProperty { Key = key, Value = value };
                _model.Properties.Add(prop);
            }
            else
            {
                prop.Value = value;
            }
        }
    }
}
