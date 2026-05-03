namespace Skyweaver.Models.ChatSession
{
    public sealed class ChatSessionResourceManifest
    {
        public List<ChatSessionResourceManifestEntry> Resources { get; } = new();

        public ChatSessionResourceManifestEntry? FindById(string? resourceId)
        {
            if (string.IsNullOrWhiteSpace(resourceId))
            {
                return null;
            }

            return Resources.FirstOrDefault(resource =>
                string.Equals(resource.Id, resourceId.Trim(), StringComparison.OrdinalIgnoreCase));
        }
    }

    public sealed class ChatSessionResourceManifestEntry
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N");

        public string Kind { get; set; } = string.Empty;

        public string Path { get; set; } = string.Empty;

        public string? MediaType { get; set; }

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        public long? SizeBytes { get; set; }

        public string? Hash { get; set; }
    }
}
