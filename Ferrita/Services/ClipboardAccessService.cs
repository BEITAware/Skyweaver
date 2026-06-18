using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Ferrita.Services.Localization;

namespace Ferrita.Services
{
    internal static class ClipboardAccessService
    {
        private const int RetryCount = 8;
        private const int RetryDelayMilliseconds = 40;

        public static bool TrySetText(string? text, out string? errorMessage)
        {
            return TryExecute(
                () =>
                {
                    var dataObject = new DataObject();
                    dataObject.SetText(text ?? string.Empty, TextDataFormat.UnicodeText);
                    Clipboard.SetDataObject(dataObject, copy: true);
                    Clipboard.Flush();
                    return true;
                },
                out _,
                out errorMessage);
        }

        public static bool TryGetText(out string text, out string? errorMessage)
        {
            var succeeded = TryExecute(
                () =>
                {
                    if (!Clipboard.ContainsText())
                    {
                        return null;
                    }

                    return Clipboard.GetText();
                },
                out string? result,
                out errorMessage);

            text = result ?? string.Empty;
            return succeeded && result != null;
        }

        public static bool TryGetImage(out BitmapSource? image, out string? errorMessage)
        {
            var succeeded = TryExecute(
                () =>
                {
                    if (!Clipboard.ContainsImage())
                    {
                        return null;
                    }

                    var clipboardImage = Clipboard.GetImage();
                    return clipboardImage == null
                        ? null
                        : CreateThreadSafeImageSnapshot(clipboardImage);
                },
                out BitmapSource? result,
                out errorMessage);

            image = result;
            return succeeded && result != null;
        }

        private static BitmapSource CreateThreadSafeImageSnapshot(BitmapSource source)
        {
            var snapshotSource = source.Format.BitsPerPixel > 0
                ? source
                : new FormatConvertedBitmap(source, PixelFormats.Pbgra32, null, 0);
            var bytesPerPixelStride = checked((snapshotSource.PixelWidth * snapshotSource.Format.BitsPerPixel + 7) / 8);
            var pixelBuffer = new byte[checked(bytesPerPixelStride * snapshotSource.PixelHeight)];

            snapshotSource.CopyPixels(pixelBuffer, bytesPerPixelStride, 0);

            var snapshot = BitmapSource.Create(
                snapshotSource.PixelWidth,
                snapshotSource.PixelHeight,
                snapshotSource.DpiX > 0 ? snapshotSource.DpiX : 96,
                snapshotSource.DpiY > 0 ? snapshotSource.DpiY : 96,
                snapshotSource.Format,
                snapshotSource.Palette,
                pixelBuffer,
                bytesPerPixelStride);
            snapshot.Freeze();
            return snapshot;
        }

        private static bool TryExecute<T>(
            Func<T> operation,
            out T? result,
            out string? errorMessage)
        {
            ArgumentNullException.ThrowIfNull(operation);

            Exception? lastException = null;
            for (var attempt = 0; attempt < RetryCount; attempt++)
            {
                if (TryExecuteOnStaThread(operation, out result, out var exception))
                {
                    errorMessage = null;
                    return true;
                }

                lastException = exception;
                Thread.Sleep(RetryDelayMilliseconds);
            }

            result = default;
            errorMessage = BuildErrorMessage(lastException);
            return false;
        }

        private static bool TryExecuteOnStaThread<T>(
            Func<T> operation,
            out T? result,
            out Exception? exception)
        {
            T? localResult = default;
            Exception? localException = null;
            using var completion = new ManualResetEventSlim(false);

            var thread = new Thread(() =>
            {
                try
                {
                    localResult = operation();
                }
                catch (Exception ex)
                {
                    localException = ex;
                }
                finally
                {
                    completion.Set();
                }
            });

            thread.IsBackground = true;
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            completion.Wait();

            result = localResult;
            exception = localException;
            return exception == null;
        }

        private static string BuildErrorMessage(Exception? exception)
        {
            if (exception == null)
            {
                return L("Clipboard.Error.UnableToAccess", "Unable to access the system clipboard.");
            }

            if (exception is ExternalException)
            {
                return L("Clipboard.Error.InUse", "The system clipboard is currently in use by another application. Please try again.");
            }

            if (exception is COMException)
            {
                return L("Clipboard.Error.TemporarilyUnavailable", "The system clipboard is temporarily unavailable. Please try again.");
            }

            if (exception is ThreadStateException)
            {
                return L("Clipboard.Error.ThreadCannotAccess", "The current thread cannot access the system clipboard.");
            }

            return LF("Clipboard.Error.UnableToAccessFormat", "Unable to access the system clipboard: {0}", exception.Message);
        }

        private static string L(string resourceKey, string fallback)
        {
            return LocalizationRuntime.Instance.GetString(resourceKey, fallback);
        }

        private static string LF(string resourceKey, string fallbackFormat, params object?[] args)
        {
            return string.Format(L(resourceKey, fallbackFormat), args);
        }
    }
}
