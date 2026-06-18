using System.IO;

namespace Ferrita.Controls.LanguageModelConfigurationControl.Services
{
    internal sealed record LanguageModelMediaResourceDescriptor(
        string ElementName,
        LanguageModelChatContentBlockKind Kind,
        string MediaType);

    internal static class LanguageModelMediaResourcePolicy
    {
        public const long MaximumLocalMediaBytes = 64L * 1024 * 1024;

        public static bool TryResolvePath(
            string? pathOrUri,
            out LanguageModelMediaResourceDescriptor descriptor)
        {
            descriptor = default!;
            var extension = Path.GetExtension(GetExtensionSource(pathOrUri)).ToLowerInvariant();
            if (extension.Length == 0)
            {
                return false;
            }

            var mediaType = extension switch
            {
                ".png" => "image/png",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".gif" => "image/gif",
                ".bmp" => "image/bmp",
                ".webp" => "image/webp",
                ".wav" => "audio/wav",
                ".mp3" => "audio/mpeg",
                ".m4a" => "audio/mp4",
                ".ogg" => "audio/ogg",
                ".flac" => "audio/flac",
                ".mp4" => "video/mp4",
                ".mpeg" or ".mpg" => "video/mpeg",
                ".mov" => "video/quicktime",
                ".avi" => "video/x-msvideo",
                ".webm" => "video/webm",
                ".mkv" => "video/x-matroska",
                ".pdf" => "application/pdf",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".ppt" => "application/vnd.ms-powerpoint",
                ".pptx" => "application/vnd.openxmlformats-officedocument.presentationml.presentation",
                ".xls" => "application/vnd.ms-excel",
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                _ => string.Empty
            };

            if (mediaType.Length == 0)
            {
                return false;
            }

            descriptor = new LanguageModelMediaResourceDescriptor(
                ResolveElementName(mediaType),
                ResolveKind(mediaType),
                mediaType);
            return true;
        }

        public static bool TryNormalizeMediaType(
            LanguageModelChatContentBlockKind kind,
            string? pathOrUri,
            string? declaredMediaType,
            out string mediaType)
        {
            mediaType = string.Empty;
            var normalizedDeclaredMediaType = NormalizeMediaType(declaredMediaType);

            if (TryResolvePath(pathOrUri, out var descriptor))
            {
                if (descriptor.Kind != kind)
                {
                    return false;
                }

                if (normalizedDeclaredMediaType.Length > 0 &&
                    !IsCompatibleDeclaredMediaType(normalizedDeclaredMediaType, descriptor.MediaType))
                {
                    return false;
                }

                mediaType = descriptor.MediaType;
                return true;
            }

            if (normalizedDeclaredMediaType.Length == 0)
            {
                return false;
            }

            if (LooksLikeLocalPath(pathOrUri))
            {
                return false;
            }

            if (kind == LanguageModelChatContentBlockKind.Image &&
                normalizedDeclaredMediaType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
            {
                mediaType = normalizedDeclaredMediaType;
                return true;
            }

            if (kind == LanguageModelChatContentBlockKind.Audio &&
                normalizedDeclaredMediaType.StartsWith("audio/", StringComparison.OrdinalIgnoreCase))
            {
                mediaType = normalizedDeclaredMediaType;
                return true;
            }

            if (kind == LanguageModelChatContentBlockKind.Video &&
                normalizedDeclaredMediaType.StartsWith("video/", StringComparison.OrdinalIgnoreCase))
            {
                mediaType = normalizedDeclaredMediaType;
                return true;
            }

            if (kind == LanguageModelChatContentBlockKind.Document &&
                IsDocumentMediaType(normalizedDeclaredMediaType))
            {
                mediaType = normalizedDeclaredMediaType;
                return true;
            }

            return false;
        }

        public static bool CanReadLocalMediaFile(
            string? localPath,
            LanguageModelChatContentBlockKind kind,
            string? declaredMediaType,
            out string mediaType,
            out string failureReason)
        {
            mediaType = string.Empty;
            failureReason = string.Empty;

            if (string.IsNullOrWhiteSpace(localPath))
            {
                failureReason = "The media path is empty.";
                return false;
            }

            if (!TryNormalizeMediaType(kind, localPath, declaredMediaType, out mediaType))
            {
                failureReason = $"Unsupported media path or MIME type: {localPath}";
                return false;
            }

            if (!File.Exists(localPath))
            {
                failureReason = $"Media file not found: {localPath}";
                return false;
            }

            try
            {
                var fileInfo = new FileInfo(localPath);
                if (fileInfo.Length > MaximumLocalMediaBytes)
                {
                    failureReason = $"Media file is too large to inline safely: {localPath} ({fileInfo.Length} bytes).";
                    return false;
                }
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or ArgumentException or NotSupportedException)
            {
                failureReason = $"Failed to inspect media file: {ex.Message}";
                return false;
            }

            return true;
        }

        private static string NormalizeMediaType(string? mediaType)
        {
            return string.IsNullOrWhiteSpace(mediaType)
                ? string.Empty
                : mediaType.Trim().ToLowerInvariant();
        }

        private static bool IsCompatibleDeclaredMediaType(string declaredMediaType, string expectedMediaType)
        {
            return string.Equals(declaredMediaType, expectedMediaType, StringComparison.OrdinalIgnoreCase) ||
                   declaredMediaType.StartsWith(GetTopLevelMediaType(expectedMediaType) + "/", StringComparison.OrdinalIgnoreCase);
        }

        private static bool LooksLikeLocalPath(string? pathOrUri)
        {
            if (string.IsNullOrWhiteSpace(pathOrUri))
            {
                return false;
            }

            var value = pathOrUri.Trim();
            if (Uri.TryCreate(value, UriKind.Absolute, out var uri))
            {
                return uri.IsFile;
            }

            return Path.IsPathRooted(value) ||
                   value.Contains(Path.DirectorySeparatorChar) ||
                   value.Contains(Path.AltDirectorySeparatorChar) ||
                   Path.HasExtension(value);
        }

        private static string GetExtensionSource(string? pathOrUri)
        {
            if (string.IsNullOrWhiteSpace(pathOrUri))
            {
                return string.Empty;
            }

            var value = pathOrUri.Trim();
            if (Uri.TryCreate(value, UriKind.Absolute, out var uri))
            {
                return uri.IsFile ? uri.LocalPath : uri.AbsolutePath;
            }

            return value;
        }

        private static string GetTopLevelMediaType(string mediaType)
        {
            var slashIndex = mediaType.IndexOf('/');
            return slashIndex <= 0 ? mediaType : mediaType[..slashIndex];
        }

        private static string ResolveElementName(string mediaType)
        {
            if (mediaType.StartsWith("audio/", StringComparison.OrdinalIgnoreCase))
            {
                return "Audio";
            }

            if (mediaType.StartsWith("video/", StringComparison.OrdinalIgnoreCase))
            {
                return "Video";
            }

            return mediaType.StartsWith("image/", StringComparison.OrdinalIgnoreCase)
                ? "Image"
                : "Document";
        }

        private static LanguageModelChatContentBlockKind ResolveKind(string mediaType)
        {
            if (mediaType.StartsWith("audio/", StringComparison.OrdinalIgnoreCase))
            {
                return LanguageModelChatContentBlockKind.Audio;
            }

            if (mediaType.StartsWith("video/", StringComparison.OrdinalIgnoreCase))
            {
                return LanguageModelChatContentBlockKind.Video;
            }

            return mediaType.StartsWith("image/", StringComparison.OrdinalIgnoreCase)
                ? LanguageModelChatContentBlockKind.Image
                : LanguageModelChatContentBlockKind.Document;
        }

        private static bool IsDocumentMediaType(string mediaType)
        {
            return !mediaType.StartsWith("text/", StringComparison.OrdinalIgnoreCase) &&
                   !IsExplicitTextApplicationMediaType(mediaType) &&
                   mediaType.StartsWith("application/", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsExplicitTextApplicationMediaType(string mediaType)
        {
            return string.Equals(mediaType, "application/json", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(mediaType, "application/xml", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(mediaType, "application/x-yaml", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(mediaType, "application/toml", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(mediaType, "application/sql", StringComparison.OrdinalIgnoreCase) ||
                   mediaType.EndsWith("+json", StringComparison.OrdinalIgnoreCase) ||
                   mediaType.EndsWith("+xml", StringComparison.OrdinalIgnoreCase);
        }
    }
}
