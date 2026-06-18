using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Ferrita.Controls.FerritaPreferencesControl.Views
{
    public partial class FerritaPreferencesControl : UserControl
    {
        public static readonly DependencyProperty BackgroundTimeProperty =
            DependencyProperty.Register(
                nameof(BackgroundTime),
                typeof(double),
                typeof(FerritaPreferencesControl),
                new PropertyMetadata(0.0));

        public static readonly DependencyProperty BackgroundAspectRatioProperty =
            DependencyProperty.Register(
                nameof(BackgroundAspectRatio),
                typeof(double),
                typeof(FerritaPreferencesControl),
                new PropertyMetadata(1.0));

        private readonly Stopwatch _backgroundStopwatch = Stopwatch.StartNew();
        private bool _isRenderingHooked;

        public FerritaPreferencesControl()
        {
            InitializeComponent();
            Loaded += FerritaPreferencesControl_Loaded;
            Unloaded += FerritaPreferencesControl_Unloaded;
        }

        public double BackgroundTime
        {
            get => (double)GetValue(BackgroundTimeProperty);
            set => SetValue(BackgroundTimeProperty, value);
        }

        public double BackgroundAspectRatio
        {
            get => (double)GetValue(BackgroundAspectRatioProperty);
            set => SetValue(BackgroundAspectRatioProperty, value);
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            UpdateBackgroundAspectRatio();
        }

        private void FerritaPreferencesControl_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateBackgroundAspectRatio();
            HookBackgroundRendering();
        }

        private void FerritaPreferencesControl_Unloaded(object sender, RoutedEventArgs e)
        {
            UnhookBackgroundRendering();
        }

        private void HookBackgroundRendering()
        {
            if (_isRenderingHooked)
            {
                return;
            }

            CompositionTarget.Rendering += CompositionTarget_Rendering;
            _isRenderingHooked = true;
        }

        private void UnhookBackgroundRendering()
        {
            if (!_isRenderingHooked)
            {
                return;
            }

            CompositionTarget.Rendering -= CompositionTarget_Rendering;
            _isRenderingHooked = false;
        }

        private void CompositionTarget_Rendering(object? sender, EventArgs e)
        {
            BackgroundTime = _backgroundStopwatch.Elapsed.TotalSeconds;
        }

        private void UpdateBackgroundAspectRatio()
        {
            var height = ActualHeight;
            BackgroundAspectRatio = height > 0 ? Math.Max(ActualWidth / height, 0.0001) : 1.0;
        }
    }
}
