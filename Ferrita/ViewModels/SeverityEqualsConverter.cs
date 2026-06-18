using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Ferrita.ViewModels
{
    /// <summary>
    /// 用于 RadioButton 与 FactSeverity 枚举双向绑定的值转换器
    /// </summary>
    public class SeverityEqualsConverter : IValueConverter
    {
        /// <summary>
        /// 目标严重性等级
        /// </summary>
        public FactSeverity TargetSeverity { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is FactSeverity severity)
            {
                return severity == TargetSeverity;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isChecked && isChecked)
            {
                return TargetSeverity;
            }
            return DependencyProperty.UnsetValue;
        }
    }
}
