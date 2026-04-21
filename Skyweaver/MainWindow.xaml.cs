using System.Windows;
using Skyweaver.ViewModels;

namespace Skyweaver
{
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();
            _viewModel = new MainViewModel();
            DataContext = _viewModel;
        }

        private void SessionListPanelView_Loaded()
        {

        }
    }
}
