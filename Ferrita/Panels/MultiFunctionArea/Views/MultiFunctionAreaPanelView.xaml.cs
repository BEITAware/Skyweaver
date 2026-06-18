using System.Windows.Controls;
using System.Windows;

namespace Ferrita.Panels.MultiFunctionArea.Views
{
    public partial class MultiFunctionAreaPanelView : UserControl
    {
        public MultiFunctionAreaPanelView()
        {
            InitializeComponent();
        }

        private void CreateTabButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.ContextMenu != null)
            {
                button.ContextMenu.PlacementTarget = button;
                button.ContextMenu.DataContext = DataContext;
                button.ContextMenu.IsOpen = true;
            }
        }
    }
}
