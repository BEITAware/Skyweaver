using System.Collections.ObjectModel;
using Skyweaver.Infrastructure.Mvvm;
using Skyweaver.Services.Localization;

namespace Skyweaver.Tools
{
    public sealed class WorkspaceNoteTemplateToolConfigurationViewModel : ObservableObject
    {
        public sealed class PresetOptionViewModel
        {
            public PresetOptionViewModel(WorkspaceNoteTemplatePreset preset)
            {
                Key = preset.Key;
                DisplayName = preset.DisplayName;
                Description = preset.Description;
            }

            public string Key { get; }

            public string DisplayName { get; }

            public string Description { get; }
        }

        private readonly Action _notifyConfigurationChanged;
        private PresetOptionViewModel? _selectedPreset;
        private bool _includeContextMetadata;
        private string _defaultTagsText = string.Empty;
        private int _selectedSummaryBulletCount;

        public WorkspaceNoteTemplateToolConfigurationViewModel(
            WorkspaceNoteTemplateToolSettings settings,
            Action notifyConfigurationChanged)
        {
            _notifyConfigurationChanged = notifyConfigurationChanged ?? throw new ArgumentNullException(nameof(notifyConfigurationChanged));

            Presets = new ObservableCollection<PresetOptionViewModel>(
                WorkspaceNoteTemplateToolSettings.Presets.Select(preset => new PresetOptionViewModel(preset)));
            SummaryBulletCountOptions = new ObservableCollection<int>(Enumerable.Range(2, 5));

            var resolvedPreset = settings.ResolvePreset();
            _selectedPreset = Presets.FirstOrDefault(item =>
                                 string.Equals(item.Key, resolvedPreset.Key, StringComparison.OrdinalIgnoreCase))
                             ?? Presets.FirstOrDefault();
            _includeContextMetadata = settings.IncludeContextMetadata;
            _defaultTagsText = settings.DefaultTagsText ?? string.Empty;
            _selectedSummaryBulletCount = WorkspaceNoteTemplateToolSettings.ClampSummaryBulletCount(settings.SummaryBulletCount);
        }

        public ObservableCollection<PresetOptionViewModel> Presets { get; }

        public ObservableCollection<int> SummaryBulletCountOptions { get; }

        public PresetOptionViewModel? SelectedPreset
        {
            get => _selectedPreset;
            set
            {
                if (SetProperty(ref _selectedPreset, value))
                {
                    OnPropertyChanged(nameof(PresetDescription));
                    OnPropertyChanged(nameof(PreviewText));
                    NotifyConfigurationChanged();
                }
            }
        }

        public bool IncludeContextMetadata
        {
            get => _includeContextMetadata;
            set
            {
                if (SetProperty(ref _includeContextMetadata, value))
                {
                    OnPropertyChanged(nameof(PreviewText));
                    NotifyConfigurationChanged();
                }
            }
        }

        public string DefaultTagsText
        {
            get => _defaultTagsText;
            set
            {
                if (SetProperty(ref _defaultTagsText, value?.Trim() ?? string.Empty))
                {
                    OnPropertyChanged(nameof(DefaultTagsHint));
                    OnPropertyChanged(nameof(PreviewText));
                    NotifyConfigurationChanged();
                }
            }
        }

        public int SelectedSummaryBulletCount
        {
            get => _selectedSummaryBulletCount;
            set
            {
                var normalized = WorkspaceNoteTemplateToolSettings.ClampSummaryBulletCount(value);
                if (SetProperty(ref _selectedSummaryBulletCount, normalized))
                {
                    OnPropertyChanged(nameof(PreviewText));
                    NotifyConfigurationChanged();
                }
            }
        }

        public string PresetDescription => SelectedPreset?.Description ?? string.Empty;

        public string DefaultTagsHint
        {
            get
            {
                var settings = ToSettings();
                var tags = settings.GetDefaultTags();
                return tags.Count == 0
                    ? L("WorkspaceNoteTemplate.DefaultTagsHint.Empty", "留空时会回退到 memo。")
                    : LF("WorkspaceNoteTemplate.DefaultTagsHint.Format", "将作为 Tags 参数默认值：{0}", string.Join(", ", tags));
            }
        }

        public string PreviewText
        {
            get
            {
                var settings = ToSettings();
                var preset = settings.ResolvePreset();

                var lines = new List<string>
                {
                    LF("WorkspaceNoteTemplate.Preview.PresetFormat", "预设：{0}", preset.DisplayName),
                    IncludeContextMetadata
                        ? L("WorkspaceNoteTemplate.Preview.ContextMetadataIncluded", "会附带 Workspace / Session 上下文行。")
                        : L("WorkspaceNoteTemplate.Preview.ContextMetadataExcluded", "不会附带 Workspace / Session 上下文行。"),
                    LF("WorkspaceNoteTemplate.Preview.DefaultTagsFormat", "默认标签：{0}", settings.DescribeDefaultTags()),
                    L("WorkspaceNoteTemplate.Preview.SummaryHeader", "摘要占位行：")
                };

                lines.AddRange(settings.BuildSummaryPrompts().Select(item => $"- {item}"));
                return string.Join(Environment.NewLine, lines);
            }
        }

        public WorkspaceNoteTemplateToolSettings ToSettings()
        {
            return new WorkspaceNoteTemplateToolSettings
            {
                PresetKey = SelectedPreset?.Key ?? WorkspaceNoteTemplateToolSettings.ResolvePreset(null).Key,
                IncludeContextMetadata = IncludeContextMetadata,
                DefaultTagsText = DefaultTagsText,
                SummaryBulletCount = SelectedSummaryBulletCount
            };
        }

        private void NotifyConfigurationChanged()
        {
            _notifyConfigurationChanged();
        }

        private static string L(string resourceKey, string fallback)
        {
            return LocalizationRuntime.Instance.GetString(resourceKey, fallback);
        }

        private static string LF(string resourceKey, string fallbackFormat, params object?[] args)
        {
            var format = L(resourceKey, fallbackFormat);
            return string.Format(format, args);
        }
    }
}
