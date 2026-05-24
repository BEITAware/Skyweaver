using System;
using Skyweaver.Infrastructure.Mvvm;

namespace Skyweaver.Models.ContextManagement
{
    public enum MemoryShareScope
    {
        SessionFlow,
        Agent,
        Application
    }

    public sealed class ContextManagementConfiguration : ObservableObject
    {
        private bool _minCompactionEnabled;
        private bool _maxCompactionEnabled = true;
        private bool _lifeCycleEnabled;
        private double _lifeCycleRatioPercent = 100d;
        private bool _rnnOptimizedCompactionEnabled;
        private bool _memoryEnabled;
        private MemoryShareScope _memoryShareScope = MemoryShareScope.SessionFlow;
        private int _memoryRetrievalCount = 5;

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

        public bool MemoryEnabled
        {
            get => _memoryEnabled;
            set => SetProperty(ref _memoryEnabled, value);
        }

        public MemoryShareScope MemoryShareScope
        {
            get => _memoryShareScope;
            set => SetProperty(ref _memoryShareScope, value);
        }

        public int MemoryRetrievalCount
        {
            get => _memoryRetrievalCount;
            set => SetProperty(ref _memoryRetrievalCount, Math.Clamp(value, 0, 30));
        }
    }
}
