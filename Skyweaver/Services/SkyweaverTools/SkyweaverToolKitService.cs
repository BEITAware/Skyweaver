namespace Skyweaver.Services.SkyweaverTools
{
    public sealed class SkyweaverToolKitService
    {
        private readonly SkyweaverToolKitConfigurationRepository _repository;

        public SkyweaverToolKitService()
            : this(new SkyweaverToolKitConfigurationRepository())
        {
        }

        public SkyweaverToolKitService(SkyweaverToolKitConfigurationRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public IReadOnlyList<SkyweaverToolKitDefinition> Load()
        {
            return _repository.Load()
                .Select(definition => definition.DeepClone())
                .ToArray();
        }

        public IReadOnlyDictionary<string, IReadOnlyList<string>> BuildToolKitMembershipMap(
            IReadOnlyList<SkyweaverToolKitDefinition>? availableToolKits = null)
        {
            var source = availableToolKits ?? Load();

            return source
                .SelectMany(toolKit => toolKit.Tools.Select(entry => new
                {
                    ToolName = entry.ToolName?.Trim() ?? string.Empty,
                    ToolKitKey = toolKit.Key?.Trim() ?? string.Empty
                }))
                .Where(item => item.ToolName.Length > 0 && item.ToolKitKey.Length > 0)
                .GroupBy(item => item.ToolName, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(
                    group => group.Key,
                    group => (IReadOnlyList<string>)group
                        .Select(item => item.ToolKitKey)
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .OrderBy(item => item, StringComparer.OrdinalIgnoreCase)
                        .ToArray(),
                    StringComparer.OrdinalIgnoreCase);
        }

        public SkyweaverToolKitResolutionResult ResolveByNames(
            IEnumerable<string> requestedNames,
            IReadOnlyList<SkyweaverToolKitDefinition>? availableToolKits = null)
        {
            ArgumentNullException.ThrowIfNull(requestedNames);

            var source = availableToolKits ?? Load();
            var byKey = source
                .Where(toolKit => !string.IsNullOrWhiteSpace(toolKit.Key))
                .GroupBy(toolKit => toolKit.Key, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(group => group.Key, group => group.Last(), StringComparer.OrdinalIgnoreCase);
            var byName = source
                .GroupBy(toolKit => toolKit.Name?.Trim() ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(group => group.Key, group => group.ToArray(), StringComparer.OrdinalIgnoreCase);

            var loaded = new List<SkyweaverToolKitDefinition>();
            var missing = new List<string>();
            var ambiguous = new List<string>();
            var usedKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var requestedName in requestedNames
                         .Select(item => item?.Trim() ?? string.Empty)
                         .Where(item => item.Length > 0)
                         .Distinct(StringComparer.OrdinalIgnoreCase))
            {
                if (byKey.TryGetValue(requestedName, out var keyedMatch))
                {
                    if (usedKeys.Add(keyedMatch.Key))
                    {
                        loaded.Add(keyedMatch.DeepClone());
                    }

                    continue;
                }

                if (!byName.TryGetValue(requestedName, out var namedMatches) || namedMatches.Length == 0)
                {
                    missing.Add(requestedName);
                    continue;
                }

                if (namedMatches.Length > 1)
                {
                    ambiguous.Add(requestedName);
                    continue;
                }

                var namedMatch = namedMatches[0];
                if (usedKeys.Add(namedMatch.Key))
                {
                    loaded.Add(namedMatch.DeepClone());
                }
            }

            return new SkyweaverToolKitResolutionResult
            {
                LoadedToolKits = loaded,
                MissingNames = missing,
                AmbiguousNames = ambiguous
            };
        }
    }
}
