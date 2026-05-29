using System;
using System.Windows;
using System.Windows.Input;
using Skyweaver.ViewModels;

namespace Skyweaver.Windows
{
    /// <summary>
    /// SkyweaverGroundingEngineWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SkyweaverGroundingEngineWindow : Window
    {
        public SkyweaverGroundingEngineWindow()
        {
            InitializeComponent();
            DataContext = new SkyweaverGroundingEngineViewModel();
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
