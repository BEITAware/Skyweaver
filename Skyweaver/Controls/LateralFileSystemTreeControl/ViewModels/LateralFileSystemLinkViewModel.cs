using Skyweaver.Infrastructure.Mvvm;

namespace Skyweaver.Controls.LateralFileSystemTreeControl.ViewModels
{
    public class LateralFileSystemLinkViewModel : ObservableObject
    {
        public LateralFileSystemNodeViewModel Source { get; }
        public LateralFileSystemNodeViewModel Target { get; }

        public LateralFileSystemLinkViewModel(LateralFileSystemNodeViewModel source, LateralFileSystemNodeViewModel target)
        {
            Source = source;
            Target = target;
            Source.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(Source.CenterX) || e.PropertyName == nameof(Source.CenterY))
                {
                    OnPropertyChanged(nameof(SourceX));
                    OnPropertyChanged(nameof(SourceY));
                }
            };
            Target.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(Target.CenterX) || e.PropertyName == nameof(Target.CenterY))
                {
                    OnPropertyChanged(nameof(TargetX));
                    OnPropertyChanged(nameof(TargetY));
                }
            };
        }

        public double SourceX => Source.CenterX;
        public double SourceY => Source.CenterY;
        public double TargetX => Target.CenterX;
        public double TargetY => Target.CenterY;
    }
}
