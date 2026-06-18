namespace Ferrita.Services.FerritaTools
{
    public sealed class FerritaToolKitService
    {
        private readonly FerritaToolKitConfigurationRepository _repository;
        private readonly FerritaToolManager _toolManager;

        public FerritaToolKitService()
            : this(new FerritaToolKitConfigurationRepository(), new FerritaToolManager())
        {
        }

        public FerritaToolKitService(
            FerritaToolKitConfigurationRepository repository,
            FerritaToolManager toolManager)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _toolManager = toolManager ?? throw new ArgumentNullException(nameof(toolManager));
        }

        public IReadOnlyList<FerritaToolKitDefinition> Load()
        {
            var defaultToolKits = BuildDefaultToolKits()
                .ToDictionary(toolKit => toolKit.Key, toolKit => toolKit, StringComparer.OrdinalIgnoreCase);
            var persistedToolKits = _repository.Load();

            foreach (var persistedToolKit in persistedToolKits)
            {
                if (string.IsNullOrWhiteSpace(persistedToolKit.Key))
                {
                    continue;
                }

                var clone = persistedToolKit.DeepClone();
                clone.IsDefaultToolKit = defaultToolKits.ContainsKey(persistedToolKit.Key);
                defaultToolKits[persistedToolKit.Key] = clone;
            }

            return defaultToolKits.Values
                .OrderBy(toolKit => toolKit.DisplayNameOrFallback, StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        public IReadOnlyDictionary<string, IReadOnlyList<string>> BuildToolKitMembershipMap(
            IReadOnlyList<FerritaToolKitDefinition>? availableToolKits = null)
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

        public FerritaToolKitResolutionResult ResolveByNames(
            IEnumerable<string> requestedNames,
            IReadOnlyList<FerritaToolKitDefinition>? availableToolKits = null)
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

            var loaded = new List<FerritaToolKitDefinition>();
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

            return new FerritaToolKitResolutionResult
            {
                LoadedToolKits = loaded,
                MissingNames = missing,
                AmbiguousNames = ambiguous
            };
        }

        private IReadOnlyList<FerritaToolKitDefinition> BuildDefaultToolKits()
        {
            var toolKitsByKey = new Dictionary<string, FerritaToolKitDefinition>(StringComparer.OrdinalIgnoreCase);

            foreach (var registration in _toolManager.GetRegisteredTools(resolveIcons: false)
                         .Where(item => item.Definition.CanBelongToToolKit))
            {
                foreach (var toolKitKey in registration.Definition.DefaultToolKitKeys)
                {
                    if (!toolKitsByKey.TryGetValue(toolKitKey, out var toolKit))
                    {
                        toolKit = new FerritaToolKitDefinition
                        {
                            Key = toolKitKey,
                            Name = toolKitKey,
                            IsDefaultToolKit = true
                        };
                        toolKitsByKey[toolKitKey] = toolKit;
                    }

                    if (toolKit.Tools.Any(entry =>
                            string.Equals(entry.ToolName, registration.Definition.Name, StringComparison.OrdinalIgnoreCase)))
                    {
                        continue;
                    }

                    toolKit.Tools.Add(new FerritaToolKitEntry
                    {
                        ToolName = registration.Definition.Name
                    });
                }
            }

            foreach (var toolKit in toolKitsByKey.Values)
            {
                var orderedEntries = toolKit.Tools
                    .OrderBy(entry => entry.ToolName, StringComparer.OrdinalIgnoreCase)
                    .Select(entry => entry.DeepClone())
                    .ToArray();

                toolKit.Tools.Clear();
                foreach (var orderedEntry in orderedEntries)
                {
                    toolKit.Tools.Add(orderedEntry);
                }
            }

            return toolKitsByKey.Values
                .Where(toolKit => toolKit.Tools.Count > 0)
                .Select(toolKit => toolKit.DeepClone())
                .ToArray();
        }
    }
}
