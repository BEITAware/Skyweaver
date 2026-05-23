using System.ComponentModel;
using System.Windows;
using Skyweaver.ViewModels;

namespace Skyweaver
{
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _viewModel;
        private bool _isGuiClosingHandled;

        public MainWindow()
        {
            InitializeComponent();
            _viewModel = new MainViewModel();
            DataContext = _viewModel;
            Closing += MainWindow_Closing;
        }

        private void SessionListPanelView_Loaded()
        {

        }

        private async void MainWindow_Closing(object? sender, CancelEventArgs e)
        {
            if (_isGuiClosingHandled)
            {
                return;
            }

            _isGuiClosingHandled = true;
            e.Cancel = true;
            IsEnabled = false;

            try
            {
                await _viewModel.HandleGuiClosingAsync();
            }
            finally
            {
                e.Cancel = false;
                Close();
            }
        }
    }
}
