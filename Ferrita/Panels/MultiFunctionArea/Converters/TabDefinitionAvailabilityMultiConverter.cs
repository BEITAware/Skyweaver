using System;
using System.Globalization;
using System.Windows.Data;
using Ferrita.Panels.MultiFunctionArea.Models;
using Ferrita.Panels.MultiFunctionArea.ViewModels;

namespace Ferrita.Panels.MultiFunctionArea.Converters
{
    public sealed class TabDefinitionAvailabilityMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 2)
            {
                return false;
            }

            if (values[0] is not MultiFunctionTabDefinition definition)
            {
                return false;
            }

            if (values[1] is not MultiFunctionAreaPanelViewModel viewModel)
            {
                return true;
            }

            return viewModel.CanCreateTab(definition.TypeKey);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
