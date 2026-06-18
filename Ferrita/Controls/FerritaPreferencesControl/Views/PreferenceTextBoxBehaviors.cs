using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Ferrita.Controls.FerritaPreferencesControl.Views
{
    public static class PreferenceTextBoxBehaviors
    {
        public static readonly DependencyProperty SelectAllOnFirstClickProperty =
            DependencyProperty.RegisterAttached(
                "SelectAllOnFirstClick",
                typeof(bool),
                typeof(PreferenceTextBoxBehaviors),
                new PropertyMetadata(false, OnSelectAllOnFirstClickChanged));

        public static bool GetSelectAllOnFirstClick(DependencyObject obj)
        {
            return (bool)obj.GetValue(SelectAllOnFirstClickProperty);
        }

        public static void SetSelectAllOnFirstClick(DependencyObject obj, bool value)
        {
            obj.SetValue(SelectAllOnFirstClickProperty, value);
        }

        private static void OnSelectAllOnFirstClickChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not TextBox textBox)
            {
                return;
            }

            if ((bool)e.NewValue)
            {
                textBox.PreviewMouseLeftButtonDown += TextBox_PreviewMouseLeftButtonDown;
                textBox.GotKeyboardFocus += TextBox_GotKeyboardFocus;
            }
            else
            {
                textBox.PreviewMouseLeftButtonDown -= TextBox_PreviewMouseLeftButtonDown;
                textBox.GotKeyboardFocus -= TextBox_GotKeyboardFocus;
            }
        }

        private static void TextBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not TextBox textBox || textBox.IsKeyboardFocusWithin)
            {
                return;
            }

            textBox.Focus();
            e.Handled = true;
        }

        private static void TextBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                textBox.SelectAll();
            }
        }
    }
}
