using System.Windows;
using System.Windows.Controls;
using Ferrita.Controls.AgentConfigurationControl.Models;
using Ferrita.Controls.AgentConfigurationControl.ViewModels;

namespace Ferrita.Controls.AgentConfigurationControl.Views
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
