using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Skyweaver.Services.SkyweaverTools;

namespace Skyweaver.Controls.ChatSessionControl.Views
{
    public partial class PlanItemCheckInvocationCardView : UserControl
    {
        public PlanItemCheckInvocationCardView()
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object? sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is SkyweaverToolInvocationPresentationState oldState)
            {
                oldState.PropertyChanged -= OnStatePropertyChanged;
                foreach (var param in oldState.Parameters)
                {
                    param.PropertyChanged -= OnParamPropertyChanged;
                }
            }

            if (e.NewValue is SkyweaverToolInvocationPresentationState newState)
            {
                newState.PropertyChanged += OnStatePropertyChanged;
                foreach (var param in newState.Parameters)
                {
                    param.PropertyChanged += OnParamPropertyChanged;
                }
                UpdateDisplays(newState);
            }
        }

        private void OnStatePropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (DataContext is SkyweaverToolInvocationPresentationState state)
            {
                UpdateDisplays(state);
            }
        }

        private void OnParamPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (DataContext is SkyweaverToolInvocationPresentationState state)
            {
                UpdateDisplays(state);
            }
        }

        private void UpdateDisplays(SkyweaverToolInvocationPresentationState state)
        {
            var planParam = state.Parameters.FirstOrDefault(p => p.Name.Equals("Planname", System.StringComparison.OrdinalIgnoreCase));
            var itemParam = state.Parameters.FirstOrDefault(p => p.Name.Equals("Item", System.StringComparison.OrdinalIgnoreCase));

            // Subscribe to any newly added parameters if they weren't subscribed before
            foreach (var param in state.Parameters)
            {
                param.PropertyChanged -= OnParamPropertyChanged;
                param.PropertyChanged += OnParamPropertyChanged;
            }

            var planName = planParam?.Value ?? string.Empty;
            var itemName = itemParam?.Value ?? string.Empty;

            PlanNameDisplay = string.IsNullOrWhiteSpace(planName) ? "Active Plan Step Checked" : $"Plan: {planName}";
            ItemNameDisplay = string.IsNullOrWhiteSpace(itemName) ? "Checking step..." : itemName;
        }

        public static readonly DependencyProperty PlanNameDisplayProperty =
            DependencyProperty.Register(nameof(PlanNameDisplay), typeof(string), typeof(PlanItemCheckInvocationCardView), new PropertyMetadata(string.Empty));

        public string PlanNameDisplay
        {
            get => (string)GetValue(PlanNameDisplayProperty);
            set => SetValue(PlanNameDisplayProperty, value);
        }

        public static readonly DependencyProperty ItemNameDisplayProperty =
            DependencyProperty.Register(nameof(ItemNameDisplay), typeof(string), typeof(PlanItemCheckInvocationCardView), new PropertyMetadata(string.Empty));

        public string ItemNameDisplay
        {
            get => (string)GetValue(ItemNameDisplayProperty);
            set => SetValue(ItemNameDisplayProperty, value);
        }
    }
}
