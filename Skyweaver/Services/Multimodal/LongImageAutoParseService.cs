using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Skyweaver.Services.Multimodal
{
    /// <summary>
    /// 长图像自动解析服务。当图像的宽高比超过 21:9 或 9:21 时，
    /// 将其沿长边均匀切分为 n 份，使每份比例落在 16:9 到 9:16 之间，
    /// 且尽可能接近 1:1。
    /// </summary>
    public static class LongImageAutoParseService
    {
        private const string SliceFolderName = "LongImageSlices";

        // 宽高比阈值：超过 21:9 ≈ 2.333 或小于 9:21 ≈ 0.4286 则触发
        private const double TriggerRatioMax = 21.0 / 9.0;
        private const double TriggerRatioMin = 9.0 / 21.0;

        // 切分后每片的宽高比限制：16:9 ≈ 1.778 到 9:16 ≈ 0.5625
        private const double SliceRatioMax = 16.0 / 9.0;
        private const double SliceRatioMin = 9.0 / 16.0;

        /// <summary>
        /// 判断指定图像文件是否需要长图自动解析。
        /// 如果需要，返回切片后的文件路径列表；否则返回 null。
        /// </summary>
        public static IReadOnlyList<string>? TryAutoParse(string imagePath)
        {
            if (string.IsNullOrWhiteSpace(imagePath) || !File.Exists(imagePath))
            {
                return null;
            }

            int pixelWidth, pixelHeight;
            try
            {
                (pixelWidth, pixelHeight) = ReadImageDimensions(imagePath);
            }
            catch
            {
                return null;
            }

            if (pixelWidth <= 0 || pixelHeight <= 0)
            {
                return null;
            }

            var aspectRatio = (double)pixelWidth / pixelHeight;

            // 不超过阈值，不需要切分
            if (aspectRatio <= TriggerRatioMax && aspectRatio >= TriggerRatioMin)
            {
                return null;
            }

            // 确定长边方向
            var isHorizontallyLong = pixelWidth > pixelHeight;
            var longSide = isHorizontallyLong ? pixelWidth : pixelHeight;
            var shortSide = isHorizontallyLong ? pixelHeight : pixelWidth;

            // 寻找最佳 n
            var bestN = FindOptimalN(longSide, shortSide);
            if (bestN <= 1)
            {
                return null;
            }

            // 检查缓存
            var sliceFolder = EnsureSliceFolder(imagePath);
            var cacheKey = BuildCacheKey(imagePath, pixelWidth, pixelHeight, bestN);
            var stampPath = Path.Combine(sliceFolder, ".slice-stamp");
            var existingSlices = GetExistingSlices(sliceFolder, bestN);
            if (existingSlices != null &&
                File.Exists(stampPath) &&
                string.Equals(File.ReadAllText(stampPath), cacheKey, StringComparison.Ordinal))
            {
                return existingSlices;
            }

            // 执行切分
            var slicePaths = SliceImage(imagePath, sliceFolder, bestN, isHorizontallyLong, longSide, shortSide);
            File.WriteAllText(stampPath, cacheKey);
            return slicePaths;
        }

        /// <summary>
        /// 寻找最优切分数 n。
        /// 当图像的长边被均切为 n 份时，得到的每张图片尺寸落在 16:9 到 9:16 之间。
        /// 若有多个 n，选择最终比例最接近 1:1 的。
        /// </summary>
        internal static int FindOptimalN(int longSide, int shortSide)
        {
            var bestN = -1;
            var bestDistance = double.MaxValue;

            // n 的上界：切片后长边不能小于1像素，且不超过合理限制
            var maxN = Math.Min(longSide, 100);

            for (var n = 1; n <= maxN; n++)
            {
                var sliceLong = (double)longSide / n;
                // 切片的宽高比（保持短边不变）
                var sliceRatio = sliceLong / shortSide;

                // 检查是否落在 [9:16, 16:9] 范围内
                if (sliceRatio >= SliceRatioMin && sliceRatio <= SliceRatioMax)
                {
                    // 距离 1:1 的程度
                    var distance = Math.Abs(sliceRatio - 1.0);
                    if (distance < bestDistance)
                    {
                        bestDistance = distance;
                        bestN = n;
                    }
                }
            }

            return bestN;
        }

        private static IReadOnlyList<string> SliceImage(
            string imagePath,
            string sliceFolder,
            int n,
            bool isHorizontallyLong,
            int longSide,
            int shortSide)
        {
            // 清理旧的切片文件
            ClearSliceFiles(sliceFolder);

            var source = LoadBitmapSource(imagePath);
            var slicePaths = new List<string>(n);

            for (var i = 0; i < n; i++)
            {
                int x, y, sliceWidth, sliceHeight;
                var sliceLongSize = longSide / n;
                var offset = i * sliceLongSize;

                // 最后一片取剩余全部
                if (i == n - 1)
                {
                    sliceLongSize = longSide - offset;
                }

                if (isHorizontallyLong)
                {
                    x = offset;
                    y = 0;
                    sliceWidth = sliceLongSize;
                    sliceHeight = shortSide;
                }
                else
                {
                    x = 0;
                    y = offset;
                    sliceWidth = shortSide;
                    sliceHeight = sliceLongSize;
                }

                // 确保不越界
                if (x + sliceWidth > source.PixelWidth)
                {
                    sliceWidth = source.PixelWidth - x;
                }

                if (y + sliceHeight > source.PixelHeight)
                {
                    sliceHeight = source.PixelHeight - y;
                }

                if (sliceWidth <= 0 || sliceHeight <= 0)
                {
                    continue;
                }

                var croppedBitmap = new CroppedBitmap(source, new System.Windows.Int32Rect(x, y, sliceWidth, sliceHeight));
                croppedBitmap.Freeze();

                var slicePath = Path.Combine(sliceFolder, $"slice-{(i + 1).ToString("0000", CultureInfo.InvariantCulture)}.png");
                SaveAsPng(croppedBitmap, slicePath);
                slicePaths.Add(slicePath);
            }

            return slicePaths;
        }

        private static (int width, int height) ReadImageDimensions(string imagePath)
        {
            using var stream = File.OpenRead(imagePath);
            var decoder = BitmapDecoder.Create(
                stream,
                BitmapCreateOptions.DelayCreation,
                BitmapCacheOption.None);

            if (decoder.Frames.Count == 0)
            {
                return (0, 0);
            }

            var frame = decoder.Frames[0];
            return (frame.PixelWidth, frame.PixelHeight);
        }

        private static BitmapSource LoadBitmapSource(string imagePath)
        {
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(imagePath, UriKind.Absolute);
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();
            bitmap.Freeze();
            return bitmap;
        }

        private static void SaveAsPng(BitmapSource source, string outputPath)
        {
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(source));
            using var stream = File.Create(outputPath);
            encoder.Save(stream);
        }

        private static string EnsureSliceFolder(string imagePath)
        {
            var directory = Path.GetDirectoryName(imagePath);
            if (string.IsNullOrWhiteSpace(directory))
            {
                directory = Path.GetTempPath();
            }

            var sourceHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(imagePath)))[..16];
            var sliceFolder = Path.Combine(directory, SliceFolderName, $"{Path.GetFileNameWithoutExtension(imagePath)}-{sourceHash}");
            Directory.CreateDirectory(sliceFolder);
            return sliceFolder;
        }

        private static string BuildCacheKey(string imagePath, int width, int height, int n)
        {
            var fileInfo = new FileInfo(imagePath);
            return $"{fileInfo.FullName}|{width}|{height}|{n}|{fileInfo.Length.ToString(CultureInfo.InvariantCulture)}|{fileInfo.LastWriteTimeUtc.Ticks.ToString(CultureInfo.InvariantCulture)}";
        }

        private static IReadOnlyList<string>? GetExistingSlices(string sliceFolder, int expectedCount)
        {
            var files = new List<string>();
            for (var i = 1; i <= expectedCount; i++)
            {
                var path = Path.Combine(sliceFolder, $"slice-{i.ToString("0000", CultureInfo.InvariantCulture)}.png");
                if (!File.Exists(path) || new FileInfo(path).Length == 0)
                {
                    return null;
                }

                files.Add(path);
            }

            return files;
        }

        private static void ClearSliceFiles(string sliceFolder)
        {
            foreach (var filePath in Directory.EnumerateFiles(sliceFolder, "slice-*.png", SearchOption.TopDirectoryOnly))
            {
                try
                {
                    File.Delete(filePath);
                }
                catch
                {
                    // 忽略无法删除的文件
                }
            }
        }
    }
}
