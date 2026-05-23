using Skyweaver.Infrastructure.Mvvm;
using Skyweaver.Services.Localization;

namespace Skyweaver.Controls.ChatSessionControl.Models
{
    public enum ChatComposerAttachmentKind
    {
        Image = 0,
        Audio = 1,
        Video = 2,
        Document = 3,
        Text = 4
    }

    public sealed class ChatComposerAttachmentModel : ObservableObject
    {
        public ChatComposerAttachmentModel(
            string resourcePath,
            string mediaType,
            string? displayName = null,
            ChatComposerAttachmentKind? attachmentKind = null,
            string? preservedContentXml = null)
        {
            ResourcePath = resourcePath ?? string.Empty;
            MediaType = string.IsNullOrWhiteSpace(mediaType) ? "image/png" : mediaType.Trim();
            Kind = attachmentKind ?? ResolveKind(MediaType);
            DisplayName = string.IsNullOrWhiteSpace(displayName)
                ? ResolveFallbackDisplayName(Kind)
                : displayName.Trim();
            PreservedContentXml = string.IsNullOrWhiteSpace(preservedContentXml)
                ? null
                : preservedContentXml;
        }

        public string ResourcePath { get; }

        public string MediaType { get; }

        public string DisplayName { get; }

        public ChatComposerAttachmentKind Kind { get; }

        public string? PreservedContentXml { get; }

        public bool IsImage => Kind == ChatComposerAttachmentKind.Image;

        public bool IsAudio => Kind == ChatComposerAttachmentKind.Audio;

        public bool IsVideo => Kind == ChatComposerAttachmentKind.Video;

        public bool IsDocument => Kind == ChatComposerAttachmentKind.Document;

        public bool IsText => Kind == ChatComposerAttachmentKind.Text;

        public string KindLabel => Kind switch
        {
            ChatComposerAttachmentKind.Audio => "Audio",
            ChatComposerAttachmentKind.Video => "Video",
            ChatComposerAttachmentKind.Document => "Document",
            ChatComposerAttachmentKind.Text => "Text",
            _ => "Image"
        };

        public string? PreviewSourcePath => IsImage ? ResourcePath : null;

        private static ChatComposerAttachmentKind ResolveKind(string mediaType)
        {
            if (mediaType.StartsWith("audio/", StringComparison.OrdinalIgnoreCase))
            {
                return ChatComposerAttachmentKind.Audio;
            }

            if (mediaType.StartsWith("video/", StringComparison.OrdinalIgnoreCase))
            {
                return ChatComposerAttachmentKind.Video;
            }

            return mediaType.StartsWith("image/", StringComparison.OrdinalIgnoreCase)
                ? ChatComposerAttachmentKind.Image
                : ChatComposerAttachmentKind.Document;
        }

        private static string ResolveFallbackDisplayName(ChatComposerAttachmentKind kind)
        {
            return kind switch
            {
                ChatComposerAttachmentKind.Audio => L("ChatMessagePart.Badge.Audio", "Audio"),
                ChatComposerAttachmentKind.Video => L("ChatMessagePart.Badge.Video", "Video"),
                ChatComposerAttachmentKind.Document => L("ChatMessagePart.Badge.Document", "Document"),
                ChatComposerAttachmentKind.Text => L("ChatMessagePart.Badge.Text", "Text"),
                _ => L("ChatMessagePart.Badge.Image", "Image")
            };
        }

        private static string L(string resourceKey, string fallback)
        {
            return LocalizationRuntime.Instance.GetString(resourceKey, fallback);
        }
    }
}
