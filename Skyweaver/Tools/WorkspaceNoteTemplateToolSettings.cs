using Newtonsoft.Json;
using System.Xml.Linq;
using Skyweaver.Services.SkyweaverTools;

namespace Skyweaver.Tools
{
    public sealed class WorkspaceNoteTemplatePreset
    {
        public WorkspaceNoteTemplatePreset(
            string key,
            string displayName,
            string description,
            int defaultPriority,
            IReadOnlyList<string> summaryPrompts)
        {
            Key = (key ?? string.Empty).Trim();
            DisplayName = (displayName ?? string.Empty).Trim();
            Description = (description ?? string.Empty).Trim();
            DefaultPriority = defaultPriority;
            SummaryPrompts = summaryPrompts?.ToArray() ?? Array.Empty<string>();
        }

        public string Key { get; }

        public string DisplayName { get; }

        public string Description { get; }

        public int DefaultPriority { get; }

        public IReadOnlyList<string> SummaryPrompts { get; }
    }

    public sealed class WorkspaceNoteTemplateToolSettings
    {
        private const string RootElementName = "WorkspaceNoteTemplateSettings";
        private const string DefaultPresetKey = "general";

        private static readonly IReadOnlyList<WorkspaceNoteTemplatePreset> s_presets =
        [
            new WorkspaceNoteTemplatePreset(
                "general",
                "General",
                "Balanced template suitable for everyday tasks and ad-hoc notes.",
                defaultPriority: 1,
                summaryPrompts:
                [
                    "Add a short task overview here",
                    "Capture the next action here",
                    "Capture risks or blockers here"
                ]),
            new WorkspaceNoteTemplatePreset(
                "planning",
                "Planning",
                "Focus on milestones, sequencing and ownership.",
                defaultPriority: 2,
                summaryPrompts:
                [
                    "Clarify the desired outcome",
                    "List milestones or checkpoints",
                    "Record dependencies or risks"
                ]),
            new WorkspaceNoteTemplatePreset(
                "investigation",
                "Investigation",
                "Focus on evidence, observations and open questions.",
                defaultPriority: 1,
                summaryPrompts:
                [
                    "Summarize the question being explored",
                    "List evidence or observations",
                    "Capture open questions and follow-ups"
                ])
        ];

        public static IReadOnlyList<WorkspaceNoteTemplatePreset> Presets => s_presets;

        public string PresetKey { get; set; } = DefaultPresetKey;

        public bool IncludeContextMetadata { get; set; } = true;

        public string DefaultTagsText { get; set; } = "memo";

        public int SummaryBulletCount { get; set; } = 3;

        public WorkspaceNoteTemplatePreset ResolvePreset()
        {
            return ResolvePreset(PresetKey);
        }

        public IReadOnlyList<string> GetDefaultTags()
        {
            return (DefaultTagsText ?? string.Empty)
                .Split([',', ';', '\r', '\n'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Where(tag => !string.IsNullOrWhiteSpace(tag))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        public string DescribeDefaultTags()
        {
            var tags = GetDefaultTags();
            return tags.Count == 0 ? "memo" : string.Join(", ", tags);
        }

        public string CreateDefaultTagsJson()
        {
            var tags = GetDefaultTags();
            return JsonConvert.SerializeObject(tags.Count == 0 ? new[] { "memo" } : tags);
        }

        public IReadOnlyList<string> BuildSummaryPrompts()
        {
            var preset = ResolvePreset();
            var targetCount = ClampSummaryBulletCount(SummaryBulletCount);
            var prompts = preset.SummaryPrompts.Take(targetCount).ToList();

            while (prompts.Count < targetCount)
            {
                prompts.Add($"Capture additional {preset.DisplayName.ToLowerInvariant()} detail {prompts.Count + 1} here");
            }

            return prompts;
        }

        public XElement ToXElement()
        {
            return new XElement(RootElementName,
                new XElement("PresetKey", ResolvePreset().Key),
                new XElement("IncludeContextMetadata", IncludeContextMetadata),
                new XElement("DefaultTagsText", DefaultTagsText ?? string.Empty),
                new XElement("SummaryBulletCount", ClampSummaryBulletCount(SummaryBulletCount)));
        }

        public static WorkspaceNoteTemplateToolSettings FromConfiguration(SkyweaverToolConfigurationState? configuration)
        {
            var payload = configuration?.GetPayload();
            if (payload == null)
            {
                return new WorkspaceNoteTemplateToolSettings();
            }

            var root = string.Equals(payload.Name.LocalName, RootElementName, StringComparison.OrdinalIgnoreCase)
                ? payload
                : payload.Element(RootElementName);

            if (root == null)
            {
                return new WorkspaceNoteTemplateToolSettings();
            }

            return new WorkspaceNoteTemplateToolSettings
            {
                PresetKey = ResolvePreset((string?)root.Element("PresetKey")).Key,
                IncludeContextMetadata = ParseBool((string?)root.Element("IncludeContextMetadata"), fallback: true),
                DefaultTagsText = ((string?)root.Element("DefaultTagsText") ?? "memo").Trim(),
                SummaryBulletCount = ClampSummaryBulletCount(ParseInt((string?)root.Element("SummaryBulletCount"), fallback: 3))
            };
        }

        public static WorkspaceNoteTemplatePreset ResolvePreset(string? key)
        {
            var normalizedKey = (key ?? string.Empty).Trim();
            return s_presets.FirstOrDefault(item =>
                       string.Equals(item.Key, normalizedKey, StringComparison.OrdinalIgnoreCase))
                   ?? s_presets[0];
        }

        public static int ClampSummaryBulletCount(int value)
        {
            return Math.Clamp(value, 2, 6);
        }

        private static bool ParseBool(string? value, bool fallback)
        {
            return bool.TryParse(value, out var parsed) ? parsed : fallback;
        }

        private static int ParseInt(string? value, int fallback)
        {
            return int.TryParse(value, out var parsed) ? parsed : fallback;
        }
    }
}
