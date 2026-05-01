using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Skyweaver.Controls.ChatSessionControl.Models;
using Skyweaver.Models.ChatSession;
using Skyweaver.Services.ChatSession;

namespace Skyweaver.Controls.ChatSessionControl.Services
{
    public sealed class ChatComposerImageAttachmentService
    {
        private const string ComposerImagesFolderName = "ComposerImages";
        private const int MaxStoredPixelSize = 768;

        public ChatComposerAttachmentModel SavePastedImage(
            ChatSessionModel session,
            BitmapSource image)
        {
            ArgumentNullException.ThrowIfNull(session);
            ArgumentNullException.ThrowIfNull(image);

            var resourcesFolder = ChatSessionResourceLayout.EnsureResources(session);
            var imageFolder = Path.Combine(resourcesFolder, ComposerImagesFolderName);
            Directory.CreateDirectory(imageFolder);

            var fileName = $"{DateTime.UtcNow:yyyyMMddHHmmssfff}_{Guid.NewGuid():N}.png";
            var filePath = Path.Combine(imageFolder, fileName);
            SaveScaledPng(image, filePath);
            return new ChatComposerAttachmentModel(filePath, "image/png", fileName);
        }

        private static void SaveScaledPng(BitmapSource source, string filePath)
        {
            var scaledSource = ScaleToMaxSize(source, MaxStoredPixelSize);
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(scaledSource));

            using var stream = File.Create(filePath);
            encoder.Save(stream);
        }

        private static BitmapSource ScaleToMaxSize(BitmapSource source, int maxPixelSize)
        {
            if (source.PixelWidth <= 0 || source.PixelHeight <= 0)
            {
                return source;
            }

            var largestSide = Math.Max(source.PixelWidth, source.PixelHeight);
            if (largestSide <= maxPixelSize)
            {
                return EnsurePbgra32(source);
            }

            var scale = (double)maxPixelSize / largestSide;
            var transformed = new TransformedBitmap(
                source,
                new ScaleTransform(scale, scale));
            transformed.Freeze();
            return EnsurePbgra32(transformed);
        }

        private static BitmapSource EnsurePbgra32(BitmapSource source)
        {
            if (source.Format == PixelFormats.Pbgra32 || source.Format == PixelFormats.Bgra32)
            {
                if (source.CanFreeze)
                {
                    source.Freeze();
                }

                return source;
            }

            var converted = new FormatConvertedBitmap(source, PixelFormats.Pbgra32, null, 0);
            converted.Freeze();
            return converted;
        }
    }
}
