using Skyweaver.Infrastructure.Mvvm;

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
            DisplayName = string.IsNullOrWhiteSpace(displayName) ? "Image" : displayName.Trim();
        }

        public string ResourcePath { get; }

        public string MediaType { get; }

        public string DisplayName { get; }
    }
}
