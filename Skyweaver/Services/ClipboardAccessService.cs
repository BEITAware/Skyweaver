using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Skyweaver.Services
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

                    return Clipboard.GetImage();
                },
                out BitmapSource? result,
                out errorMessage);

            if (result?.CanFreeze == true)
            {
                result.Freeze();
            }

            image = result;
            return succeeded && result != null;
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
                return "Unable to access the system clipboard.";
            }

            if (exception is ExternalException)
            {
                return "The system clipboard is currently in use by another application. Please try again.";
            }

            if (exception is COMException)
            {
                return "The system clipboard is temporarily unavailable. Please try again.";
            }

            if (exception is ThreadStateException)
            {
                return "The current thread cannot access the system clipboard.";
            }

            return $"Unable to access the system clipboard: {exception.Message}";
        }
    }
}
