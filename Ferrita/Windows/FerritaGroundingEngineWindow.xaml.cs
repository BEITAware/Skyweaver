using System;
using System.Windows;
using System.Windows.Input;
using Ferrita.ViewModels;

namespace Ferrita.Windows
{
    /// <summary>
    /// FerritaGroundingEngineWindow.xaml 的交互逻辑
    /// </summary>
    public partial class FerritaGroundingEngineWindow : Window
    {
        public FerritaGroundingEngineWindow()
        {
            InitializeComponent();
            DataContext = new FerritaGroundingEngineViewModel();
        }

        private void Header_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
