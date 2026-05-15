using Skyweaver.Infrastructure.Mvvm;

namespace Skyweaver.Models.ContextManagement
{
    public sealed class ContextManagementConfiguration : ObservableObject
    {
        private bool _minCompactionEnabled;
        private bool _maxCompactionEnabled;
        private bool _lifeCycleEnabled;
        private double _lifeCycleRatioPercent = 100d;
        private bool _rnnOptimizedCompactionEnabled;

        public bool MinCompactionEnabled
        {
            get => _minCompactionEnabled;
            set => SetProperty(ref _minCompactionEnabled, value);
        }

        public bool MaxCompactionEnabled
        {
            get => _maxCompactionEnabled;
            set => SetProperty(ref _maxCompactionEnabled, value);
        }

        public bool LifeCycleEnabled
        {
            get => _lifeCycleEnabled;
            set => SetProperty(ref _lifeCycleEnabled, value);
        }

        public double LifeCycleRatioPercent
        {
            get => _lifeCycleRatioPercent;
            set => SetProperty(ref _lifeCycleRatioPercent, Math.Clamp(value, 10d, 500d));
        }

        public bool RnnOptimizedCompactionEnabled
        {
            get => _rnnOptimizedCompactionEnabled;
            set => SetProperty(ref _rnnOptimizedCompactionEnabled, value);
        }
    }
}
