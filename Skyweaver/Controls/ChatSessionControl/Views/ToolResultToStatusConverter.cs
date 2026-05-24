using System;
using System.Globalization;
using System.Windows.Data;

namespace Skyweaver.Controls.ChatSessionControl.Views
{
    public class ToolResultToStatusConverter : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not string content || string.IsNullOrWhiteSpace(content))
            {
                return string.Empty;
            }

            var isWebSearch = string.Equals(parameter as string, "WebSearch", StringComparison.OrdinalIgnoreCase);

            // 只有当文本以特定的系统失败提示开头时才认定为失败，避免误判网页正文中正常包含 error/failed 等词的情况
            var isFailed = content.StartsWith("Tool execution failed:", StringComparison.OrdinalIgnoreCase) ||
                           content.StartsWith("Search execution failed:", StringComparison.OrdinalIgnoreCase) ||
                           content.StartsWith("Browse execution failed:", StringComparison.OrdinalIgnoreCase) ||
                           content.StartsWith("Error:", StringComparison.OrdinalIgnoreCase);

            if (isFailed)
            {
                if (content.Length > 200)
                {
                    return $"执行失败: {content.Substring(0, 197)}...";
                }
                return $"执行失败: {content}";
            }

            return isWebSearch ? "搜索成功，已将结果发送给助手。" : "网页内容提取成功，已发送给助手。";
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
