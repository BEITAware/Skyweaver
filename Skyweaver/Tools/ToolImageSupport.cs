using System.IO;
using System.Windows.Media.Imaging;

namespace Skyweaver.Tools
{
    internal sealed record ValidatedImagePath(
        string RequestedPath,
        string ResolvedPath,
        int PixelWidth,
        int PixelHeight);

    internal static class ToolImageSupport
    {
        public static List<ValidatedImagePath> ResolveAndValidateImagePaths(
            IEnumerable<string> requestedPaths,
            string? workspacePath,
            CancellationToken cancellationToken,
            List<string> errors)
        {
            ArgumentNullException.ThrowIfNull(requestedPaths);
            ArgumentNullException.ThrowIfNull(errors);

            var validatedPaths = new List<ValidatedImagePath>();
            foreach (var requestedPath in requestedPaths)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (string.IsNullOrWhiteSpace(requestedPath))
                {
                    errors.Add("An image path was null or empty.");
                    continue;
                }

                string resolvedPath;
                try
                {
                    resolvedPath = ToolFileSystemHelper.ResolvePath(requestedPath, workspacePath);
                }
                catch (Exception ex) when (ex is InvalidOperationException or ArgumentException or NotSupportedException)
                {
                    errors.Add($"Failed to resolve path '{requestedPath}': {ex.Message}");
                    continue;
                }

                if (!File.Exists(resolvedPath))
                {
                    errors.Add($"Image file does not exist: {resolvedPath}");
                    continue;
                }

                if (TryReadImageMetadata(resolvedPath, out var metadata, out var error))
                {
                    validatedPaths.Add(new ValidatedImagePath(
                        requestedPath,
                        resolvedPath,
                        metadata.PixelWidth,
                        metadata.PixelHeight));
                }
                else
                {
                    errors.Add($"Invalid image '{resolvedPath}': {error}");
                }
            }

            return validatedPaths;
        }

        public static bool TryReadImageMetadata(
            string resolvedPath,
            out (int PixelWidth, int PixelHeight) metadata,
            out string error)
        {
            try
            {
                using var stream = new FileStream(resolvedPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                var decoder = BitmapDecoder.Create(
                    stream,
                    BitmapCreateOptions.PreservePixelFormat,
                    BitmapCacheOption.OnLoad);
                var frame = decoder.Frames.FirstOrDefault();
                if (frame == null)
                {
                    metadata = default;
                    error = "The file could not be decoded into an image frame.";
                    return false;
                }

                if (frame.PixelWidth <= 0 || frame.PixelHeight <= 0)
                {
                    metadata = default;
                    error = "The decoded image has invalid dimensions.";
                    return false;
                }

                metadata = (frame.PixelWidth, frame.PixelHeight);
                error = string.Empty;
                return true;
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or FileFormatException or NotSupportedException or ArgumentException)
            {
                metadata = default;
                error = ex.Message;
                return false;
            }
        }
    }
}
