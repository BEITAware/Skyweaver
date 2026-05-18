using Skyweaver.Infrastructure.Mvvm;
using Skyweaver.Services.Localization;

namespace Skyweaver.Controls.ChatSessionControl.Models
{
    public sealed class ChatComposerAttachmentModel : ObservableObject
    {
        public ChatComposerAttachmentModel(
            string resourcePath,
            string mediaType,
            string? displayName = null)
        {
            ResourcePath = resourcePath ?? string.Empty;
            MediaType = string.IsNullOrWhiteSpace(mediaType) ? "image/png" : mediaType.Trim();
            DisplayName = string.IsNullOrWhiteSpace(displayName) ? L("ChatMessagePart.Badge.Image", "图片") : displayName.Trim();
        }

        public string ResourcePath { get; }

        public string MediaType { get; }

        public string DisplayName { get; }

        public bool IsImage => MediaType.StartsWith("image/", StringComparison.OrdinalIgnoreCase);

        public bool IsAudio => MediaType.StartsWith("audio/", StringComparison.OrdinalIgnoreCase);

        public string KindLabel => IsAudio ? "Audio" : IsImage ? "Image" : "File";

        public string? PreviewSourcePath => IsImage ? ResourcePath : null;

        private static string L(string resourceKey, string fallback)
        {
            return LocalizationRuntime.Instance.GetString(resourceKey, fallback);
        }
    }
}
