using System.Collections.ObjectModel;
using Skyweaver.Infrastructure.Mvvm;
using Skyweaver.Services.Localization;

namespace Skyweaver.Services.SkyweaverTools
{
    public sealed class SkyweaverToolKitEntry : ObservableObject
    {
        private string _toolName = string.Empty;

        public string ToolName
        {
            get => _toolName;
            set => SetProperty(ref _toolName, value?.Trim() ?? string.Empty);
        }

        public SkyweaverToolKitEntry DeepClone()
        {
            return new SkyweaverToolKitEntry
            {
                ToolName = ToolName
            };
        }
    }

    public sealed class SkyweaverToolKitDefinition : ObservableObject
    {
        private string _key = Guid.NewGuid().ToString("N");
        private string _name = string.Empty;
        private bool _isDefaultToolKit;

        public string Key
        {
            get => _key;
            set => SetProperty(ref _key, value?.Trim() ?? string.Empty);
        }

        public string Name
        {
            get => _name;
            set
            {
                if (SetProperty(ref _name, value?.Trim() ?? string.Empty))
                {
                    OnPropertyChanged(nameof(DisplayNameOrFallback));
                }
            }
        }

        public ObservableCollection<SkyweaverToolKitEntry> Tools { get; } = new();

        public bool IsDefaultToolKit
        {
            get => _isDefaultToolKit;
            set => SetProperty(ref _isDefaultToolKit, value);
        }

        public string DisplayNameOrFallback => string.IsNullOrWhiteSpace(Name)
            ? LF("ToolConfiguration.ToolKits.UnnamedFormat", "未命名工具集 ({0})", GetShortKey(Key))
            : Name;

        public string ManagementHint => L(
            "ToolConfiguration.ToolKits.ManagementHint",
            "加入该工具集的工具不会在代理循环开始时默认暴露给 LLM，需要通过 LoadToolKits 显式加载。");

        public void RefreshLocalizedText()
        {
            OnPropertyChanged(nameof(DisplayNameOrFallback));
            OnPropertyChanged(nameof(ManagementHint));
        }

        public SkyweaverToolKitDefinition DeepClone()
        {
            var clone = new SkyweaverToolKitDefinition
            {
                Key = Key,
                Name = Name,
                IsDefaultToolKit = IsDefaultToolKit
            };

            foreach (var tool in Tools)
            {
                clone.Tools.Add(tool.DeepClone());
            }

            return clone;
        }

        private static string GetShortKey(string? key)
        {
            var normalized = key?.Trim() ?? string.Empty;
            return normalized.Length <= 8 ? normalized : normalized[..8];
        }

        private static string L(string resourceKey, string fallback)
        {
            return LocalizationRuntime.Instance.GetString(resourceKey, fallback);
        }

        private static string LF(string resourceKey, string fallback, params object[] args)
        {
            return string.Format(L(resourceKey, fallback), args);
        }
    }

    public sealed class SkyweaverToolKitResolutionResult
    {
        public IReadOnlyList<SkyweaverToolKitDefinition> LoadedToolKits { get; init; } =
            Array.Empty<SkyweaverToolKitDefinition>();

        public IReadOnlyList<string> MissingNames { get; init; } = Array.Empty<string>();

        public IReadOnlyList<string> AmbiguousNames { get; init; } = Array.Empty<string>();
    }
}
