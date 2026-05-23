using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using Skyweaver.Controls.ChatSessionControl.Models;

namespace Skyweaver.Controls.ChatSessionControl.Views
{
    /// <summary>
    /// 媒体资源图标转换器，根据媒体资源的文件类型和后缀名返回对应的图标
    /// </summary>
    public class MediaResourceIconConverter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not ChatMessagePartModel part)
            {
                return null;
            }

            const string GeneralDocUri = "pack://application:,,,/Skyweaver;component/Resources/GeneralDocument.png";
            const string PdfUri = "pack://application:,,,/Skyweaver;component/Resources/PDFile.png";
            const string AudioUri = "pack://application:,,,/Skyweaver;component/Resources/Audio.png";
            const string VideoUri = "pack://application:,,,/Skyweaver;component/Resources/Video.png";

            try
            {
                if (part.PartType == ChatMessagePartType.Document)
                {
                    string? path = part.ResourcePath ?? part.Title;
                    if (!string.IsNullOrEmpty(path))
                    {
                        string ext = Path.GetExtension(path).ToLower();
                        if (ext == ".pdf")
                        {
                            return new BitmapImage(new Uri(PdfUri));
                        }
                    }
                    return new BitmapImage(new Uri(GeneralDocUri));
                }
                else if (part.PartType == ChatMessagePartType.TextAttachment)
                {
                    return new BitmapImage(new Uri(GeneralDocUri));
                }
                else if (part.PartType == ChatMessagePartType.Audio)
                {
                    return new BitmapImage(new Uri(AudioUri));
                }
                else if (part.PartType == ChatMessagePartType.Video)
                {
                    return new BitmapImage(new Uri(VideoUri));
                }
            }
            catch
            {
                // 忽略转换异常，Fallback 到 XAML 中的备用折角图标
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
