using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Ferrita.Controls.PersonaSettingsControl.ViewModels;

namespace Ferrita.Controls.PersonaSettingsControl.Views
{
    public partial class PersonaSettingsControl : UserControl
    {
        private Color _currentBgColor = Color.FromRgb(80, 80, 80);
        private Color _currentGlowColor = Color.FromRgb(128, 128, 128); // 极光主色发光点的当前颜色
        private Models.PersonaModel? _observedPersona;

        public PersonaSettingsControl()
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is PersonaSettingsControlViewModel oldVm)
            {
                oldVm.PropertyChanged -= OnViewModelPropertyChanged;
                DetachObservedPersona();
            }
            if (e.NewValue is PersonaSettingsControlViewModel newVm)
            {
                newVm.PropertyChanged += OnViewModelPropertyChanged;
                AttachObservedPersona(newVm.SelectedPersona);

                UpdateBackground(newVm.SelectedPersona?.BackgroundColorHex, newVm.SelectedPersona?.ColorHex, immediate: true);
            }
        }

        private void OnViewModelPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(PersonaSettingsControlViewModel.SelectedPersona))
            {
                if (DataContext is PersonaSettingsControlViewModel vm)
                {
                    AttachObservedPersona(vm.SelectedPersona);

                    UpdateBackground(vm.SelectedPersona?.BackgroundColorHex, vm.SelectedPersona?.ColorHex, immediate: false);
                }
            }
        }

        private void OnSelectedPersonaPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if ((e.PropertyName == nameof(Models.PersonaModel.BackgroundColorHex) || 
                 e.PropertyName == nameof(Models.PersonaModel.ColorHex)) &&
                DataContext is PersonaSettingsControlViewModel vm)
            {
                UpdateBackground(vm.SelectedPersona?.BackgroundColorHex, vm.SelectedPersona?.ColorHex, immediate: false);
            }
        }

        private void AttachObservedPersona(Models.PersonaModel? persona)
        {
            if (ReferenceEquals(_observedPersona, persona))
            {
                return;
            }

            DetachObservedPersona();
            _observedPersona = persona;
            if (_observedPersona != null)
            {
                _observedPersona.PropertyChanged += OnSelectedPersonaPropertyChanged;
            }
        }

        private void DetachObservedPersona()
        {
            if (_observedPersona == null)
            {
                return;
            }

            _observedPersona.PropertyChanged -= OnSelectedPersonaPropertyChanged;
            _observedPersona = null;
        }

        private void UpdateBackground(string? bgColorHex, string? glowColorHex, bool immediate)
        {
            if (string.IsNullOrWhiteSpace(bgColorHex)) return;
            if (string.IsNullOrWhiteSpace(glowColorHex)) glowColorHex = "#FF808080";

            try
            {
                var newBgColor = (Color)ColorConverter.ConvertFromString(bgColorHex);
                var newGlowColor = (Color)ColorConverter.ConvertFromString(glowColorHex);

                if (immediate)
                {
                    BaseColorLayer.Background = new SolidColorBrush(newBgColor);
                    GlowColorStop.Color = newGlowColor;
                    _currentBgColor = newBgColor;
                    _currentGlowColor = newGlowColor;
                }
                else
                {
                    // 1. 底色渐变动画
                    var bgBrush = new SolidColorBrush(_currentBgColor);
                    BaseColorLayer.Background = bgBrush;

                    var bgAnimation = new ColorAnimation
                    {
                        From = _currentBgColor,
                        To = newBgColor,
                        Duration = new Duration(TimeSpan.FromMilliseconds(650)),
                        EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
                    };

                    bgBrush.BeginAnimation(SolidColorBrush.ColorProperty, bgAnimation);
                    _currentBgColor = newBgColor;

                    // 2. 极光主色发光点颜色动画
                    var glowAnimation = new ColorAnimation
                    {
                        From = _currentGlowColor,
                        To = newGlowColor,
                        Duration = new Duration(TimeSpan.FromMilliseconds(650)),
                        EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
                    };

                    GlowColorStop.BeginAnimation(GradientStop.ColorProperty, glowAnimation);
                    _currentGlowColor = newGlowColor;
                }
            }
            catch
            {
                // fallback to default
            }
        }

        private void OnTopRegionPreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (DataContext is PersonaSettingsControlViewModel vm)
            {
                if (e.Delta > 0)
                {
                    if (vm.PrevPersonaCommand.CanExecute(null))
                    {
                        vm.PrevPersonaCommand.Execute(null);
                        e.Handled = true;
                    }
                }
                else if (e.Delta < 0)
                {
                    if (vm.NextPersonaCommand.CanExecute(null))
                    {
                        vm.NextPersonaCommand.Execute(null);
                        e.Handled = true;
                    }
                }
            }
        }

        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Delete || IsTextEditingElement(e.OriginalSource))
            {
                return;
            }

            if (DataContext is PersonaSettingsControlViewModel vm &&
                vm.DeleteSelectedPersonaCommand.CanExecute(null))
            {
                vm.DeleteSelectedPersonaCommand.Execute(null);
                e.Handled = true;
            }
        }

        private static bool IsTextEditingElement(object originalSource)
        {
            return originalSource is TextBoxBase
                or PasswordBox
                or ComboBox { IsEditable: true };
        }
    }
}
