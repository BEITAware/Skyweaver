using System.Windows;
using System.Windows.Controls;
using Skyweaver.Controls.AgentConfigurationControl.Models;
using Skyweaver.Controls.AgentConfigurationControl.ViewModels;

namespace Skyweaver.Controls.AgentConfigurationControl.Views
{
    public partial class AgentConfigurationControl : UserControl
    {
        public AgentConfigurationControl()
        {
            InitializeComponent();
        }

        private void OnInputSchemaTreeViewSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (DataContext is AgentConfigurationControlViewModel viewModel)
            {
                viewModel.SelectedInputNode = e.NewValue as XmlElementNodeDefinition;
            }
        }

        private void OnOutputSchemaTreeViewSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (DataContext is AgentConfigurationControlViewModel viewModel)
            {
                viewModel.SelectedOutputNode = e.NewValue as XmlElementNodeDefinition;
            }
        }
    }
}
