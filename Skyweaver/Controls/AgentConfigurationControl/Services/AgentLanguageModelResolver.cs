using Skyweaver.Controls.AgentConfigurationControl.Models;
using Skyweaver.Controls.LanguageModelConfigurationControl.Models;
using Skyweaver.Controls.LanguageModelConfigurationControl.Services;

namespace Skyweaver.Controls.AgentConfigurationControl.Services
{
    public sealed class AgentLanguageModelResolver : IAgentLanguageModelResolver
    {
        private readonly LanguageModelConfigurationRepository _languageModelRepository;
        private readonly CapabilityLayerConfigurationRepository _capabilityLayerRepository;

        private sealed record ResolutionPlan(
            bool UseFallback,
            string SelectionDisplayName,
            IReadOnlyList<LanguageModelDefinition> Candidates,
            IReadOnlyList<string> Issues);

        public AgentLanguageModelResolver()
            : this(
                new LanguageModelConfigurationRepository(new LanguageModelConfigurationPathProvider()),
                new CapabilityLayerConfigurationRepository(new LanguageModelConfigurationPathProvider()))
        {
        }

        public AgentLanguageModelResolver(
            LanguageModelConfigurationRepository languageModelRepository,
            CapabilityLayerConfigurationRepository capabilityLayerRepository)
        {
            _languageModelRepository = languageModelRepository ?? throw new ArgumentNullException(nameof(languageModelRepository));
            _capabilityLayerRepository = capabilityLayerRepository ?? throw new ArgumentNullException(nameof(capabilityLayerRepository));
        }

        public IReadOnlyList<LanguageModelDefinition> GetCandidateModels(AgentDefinition agent)
        {
            var plan = BuildResolutionPlan(agent, LoadLanguageModelMap(), LoadCapabilityLayerMap());
            return plan.Candidates;
        }

        public IReadOnlyList<LanguageModelDefinition> GetCandidateModelsForCapabilityLayer(string capabilityLayerKey)
        {
            var languageModelMap = LoadLanguageModelMap();
            var capabilityLayerMap = LoadCapabilityLayerMap();
            return BuildCapabilityLayerPlan(capabilityLayerKey, languageModelMap, capabilityLayerMap).Cands;
        }

        public Task<LanguageModelDefinition> ResolveFirstAvailableAsync(
            AgentDefinition agent,
            Func<LanguageModelDefinition, CancellationToken, Task>? probeAsync = null,
            CancellationToken cancellationToken = default)
        {
            return ExecuteWithFallbackAsync(
                agent,
                async (model, ct) =>
                {
                    if (probeAsync != null)
                    {
                        await probeAsync(model, ct).ConfigureAwait(false);
                    }

                    return model;
                },
                cancellationToken);
        }

        public async Task<T> ExecuteWithFallbackAsync<T>(
            AgentDefinition agent,
            Func<LanguageModelDefinition, CancellationToken, Task<T>> operationAsync,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(agent);
            ArgumentNullException.ThrowIfNull(operationAsync);

            var languageModelMap = LoadLanguageModelMap();
            var capabilityLayerMap = LoadCapabilityLayerMap();
            var plan = BuildResolutionPlan(agent, languageModelMap, capabilityLayerMap);

            if (plan.Candidates.Count == 0)
            {
                throw new InvalidOperationException(BuildMissingSelectionMessage(agent, plan));
            }

            if (!plan.UseFallback)
            {
                var targetModel = plan.Candidates[0];
                EnsureModelConfigured(agent, targetModel);
                return await operationAsync(targetModel, cancellationToken).ConfigureAwait(false);
            }

            Exception? lastError = null;
            var failureMessages = new List<string>();

            foreach (var candidate in plan.Candidates)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (!candidate.InterfaceSettings.IsFullyConfigured)
                {
                    failureMessages.Add($"{GetLanguageModelDisplayName(candidate)}：接口配置不完整");
                    continue;
                }

                try
                {
                    return await operationAsync(candidate, cancellationToken).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    lastError = ex;
                    failureMessages.Add($"{GetLanguageModelDisplayName(candidate)}：{ex.Message}");
                }
            }

            var failureText = failureMessages.Count == 0
                ? "没有可调用的候选语言模型。"
                : $"已按顺序尝试：{string.Join("；", failureMessages)}";

            throw new InvalidOperationException(
                $"代理“{agent.DisplayNameOrFallback}”绑定的功能层级“{plan.SelectionDisplayName}”没有找到无错误可用的语言模型。{failureText}",
                lastError);
        }

        public async Task<T> ExecuteCapabilityLayerWithFallbackAsync<T>(
            string capabilityLayerKey,
            Func<LanguageModelDefinition, CancellationToken, Task<T>> operationAsync,
            CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(capabilityLayerKey);
            ArgumentNullException.ThrowIfNull(operationAsync);

            var languageModelMap = LoadLanguageModelMap();
            var capabilityLayerMap = LoadCapabilityLayerMap();
            var plan = BuildCapabilityLayerPlan(capabilityLayerKey, languageModelMap, capabilityLayerMap);

            if (plan.Cands.Count == 0)
            {
                var issues = plan.Issues.Count == 0 ? string.Empty : $" {string.Join("；", plan.Issues)}";
                throw new InvalidOperationException(
                    $"功能层级“{plan.SelectionDisplayName}”无法解析出候选语言模型。{issues}");
            }

            Exception? lastError = null;
            var failureMessages = new List<string>();

            foreach (var candidate in plan.Cands)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (!candidate.InterfaceSettings.IsFullyConfigured)
                {
                    failureMessages.Add($"{GetLanguageModelDisplayName(candidate)}：接口配置不完整");
                    continue;
                }

                try
                {
                    return await operationAsync(candidate, cancellationToken).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    lastError = ex;
                    failureMessages.Add($"{GetLanguageModelDisplayName(candidate)}：{ex.Message}");
                }
            }

            var failureText = failureMessages.Count == 0
                ? "没有可调用的候选语言模型。"
                : $"已按顺序尝试：{string.Join("；", failureMessages)}";

            throw new InvalidOperationException(
                $"功能层级“{plan.SelectionDisplayName}”没有找到无错误可用的语言模型。{failureText}",
                lastError);
        }

        public int GetMinimumContextWindowTokens(AgentDefinition agent)
        {
            ArgumentNullException.ThrowIfNull(agent);

            var candidates = GetCandidateModels(agent);
            if (candidates.Count == 0)
            {
                throw new InvalidOperationException(
                    $"代理“{agent.DisplayNameOrFallback}”未能解析到用于计算上下文窗口的语言模型。");
            }

            return candidates.Min(model => model.EffectiveContextWindowTokens);
        }

        public int GetCapabilityLayerMinimumContextWindowTokens(string capabilityLayerKey)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(capabilityLayerKey);

            var candidates = GetCandidateModelsForCapabilityLayer(capabilityLayerKey);
            if (candidates.Count == 0)
            {
                throw new InvalidOperationException(
                    $"功能层级“{capabilityLayerKey}”未能解析到用于计算上下文窗口的语言模型。");
            }

            return candidates.Min(model => model.EffectiveContextWindowTokens);
        }

        private Dictionary<string, LanguageModelDefinition> LoadLanguageModelMap()
        {
            return _languageModelRepository.Load()
                .Where(model => !string.IsNullOrWhiteSpace(model.Key))
                .ToDictionary(model => model.Key, StringComparer.OrdinalIgnoreCase);
        }

        private Dictionary<string, CapabilityLayerDefinition> LoadCapabilityLayerMap()
        {
            return _capabilityLayerRepository.Load()
                .Where(layer => !string.IsNullOrWhiteSpace(layer.Key))
                .ToDictionary(layer => layer.Key, StringComparer.OrdinalIgnoreCase);
        }

        private static ResolutionPlan BuildResolutionPlan(
            AgentDefinition agent,
            IReadOnlyDictionary<string, LanguageModelDefinition> languageModelMap,
            IReadOnlyDictionary<string, CapabilityLayerDefinition> capabilityLayerMap)
        {
            return agent.LanguageModelSelectionMode == AgentLanguageModelSelectionMode.CapabilityLayer
                ? BuildCapabilityLayerPlan(agent, languageModelMap, capabilityLayerMap)
                : BuildSpecificLanguageModelPlan(agent, languageModelMap);
        }

        private static ResolutionPlan BuildSpecificLanguageModelPlan(
            AgentDefinition agent,
            IReadOnlyDictionary<string, LanguageModelDefinition> languageModelMap)
        {
            if (string.IsNullOrWhiteSpace(agent.SelectedLanguageModelKey))
            {
                return new ResolutionPlan(
                    UseFallback: false,
                    SelectionDisplayName: "未选择语言模型",
                    Candidates: Array.Empty<LanguageModelDefinition>(),
                    Issues: ["未为代理绑定具体语言模型。"]);
            }

            if (!languageModelMap.TryGetValue(agent.SelectedLanguageModelKey, out var model))
            {
                return new ResolutionPlan(
                    UseFallback: false,
                    SelectionDisplayName: agent.SelectedLanguageModelKey,
                    Candidates: Array.Empty<LanguageModelDefinition>(),
                    Issues: ["引用的语言模型不存在。"]);
            }

            return new ResolutionPlan(
                UseFallback: false,
                SelectionDisplayName: GetLanguageModelDisplayName(model),
                Candidates: [model],
                Issues: Array.Empty<string>());
        }

        private static ResolutionPlan BuildCapabilityLayerPlan(
            AgentDefinition agent,
            IReadOnlyDictionary<string, LanguageModelDefinition> languageModelMap,
            IReadOnlyDictionary<string, CapabilityLayerDefinition> capabilityLayerMap)
        {
            return BuildCapabilityLayerPlan(agent.SelectedCapabilityLayerKey, languageModelMap, capabilityLayerMap).ToResolutionPlan();
        }

        private static CapabilityLayerResolutionPlan BuildCapabilityLayerPlan(
            string? capabilityLayerKey,
            IReadOnlyDictionary<string, LanguageModelDefinition> languageModelMap,
            IReadOnlyDictionary<string, CapabilityLayerDefinition> capabilityLayerMap)
        {
            if (string.IsNullOrWhiteSpace(capabilityLayerKey))
            {
                return new CapabilityLayerResolutionPlan(
                    SelectionDisplayName: "未选择功能层级",
                    Cands: Array.Empty<LanguageModelDefinition>(),
                    Issues: ["未指定功能层级。"]);
            }

            if (!capabilityLayerMap.TryGetValue(capabilityLayerKey, out var layer))
            {
                return new CapabilityLayerResolutionPlan(
                    SelectionDisplayName: capabilityLayerKey,
                    Cands: Array.Empty<LanguageModelDefinition>(),
                    Issues: ["引用的功能层级不存在。"]);
            }

            var seenKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var candidates = new List<LanguageModelDefinition>();
            var issues = new List<string>();

            foreach (var entry in layer.LanguageModels)
            {
                var languageModelKey = entry.LanguageModelKey?.Trim() ?? string.Empty;
                if (languageModelKey.Length == 0 || !seenKeys.Add(languageModelKey))
                {
                    continue;
                }

                if (!languageModelMap.TryGetValue(languageModelKey, out var model))
                {
                    issues.Add($"功能层级中的语言模型引用“{languageModelKey}”不存在。");
                    continue;
                }

                candidates.Add(model);
            }

            return new CapabilityLayerResolutionPlan(
                SelectionDisplayName: GetCapabilityLayerDisplayName(layer),
                Cands: candidates,
                Issues: issues);
        }

        private static void EnsureModelConfigured(AgentDefinition agent, LanguageModelDefinition model)
        {
            if (model.InterfaceSettings.IsFullyConfigured)
            {
                return;
            }

            throw new InvalidOperationException(
                $"代理“{agent.DisplayNameOrFallback}”绑定的语言模型“{GetLanguageModelDisplayName(model)}”接口配置不完整。");
        }

        private static string BuildMissingSelectionMessage(AgentDefinition agent, ResolutionPlan plan)
        {
            var issues = plan.Issues.Count == 0
                ? string.Empty
                : $" {string.Join("；", plan.Issues)}";

            return plan.UseFallback
                ? $"代理“{agent.DisplayNameOrFallback}”绑定的功能层级无法解析出候选语言模型。{issues}"
                : $"代理“{agent.DisplayNameOrFallback}”未能解析到具体语言模型。{issues}";
        }

        private static string GetLanguageModelDisplayName(LanguageModelDefinition model)
        {
            var displayName = model.DisplayName?.Trim() ?? string.Empty;
            if (displayName.Length > 0)
            {
                return displayName;
            }

            var summaryModelId = model.SummaryModelId?.Trim() ?? string.Empty;
            if (summaryModelId.Length > 0)
            {
                return $"未命名模型 ({summaryModelId})";
            }

            return $"未命名模型 ({GetShortKey(model.Key)})";
        }

        private static string GetCapabilityLayerDisplayName(CapabilityLayerDefinition layer)
        {
            var displayName = layer.Name?.Trim() ?? string.Empty;
            return displayName.Length > 0
                ? displayName
                : $"未命名功能层级 ({GetShortKey(layer.Key)})";
        }

        private static string GetShortKey(string? key)
        {
            var normalizedKey = (key ?? string.Empty).Trim();
            if (normalizedKey.Length <= 8)
            {
                return normalizedKey;
            }

            return normalizedKey[..8];
        }

        private sealed record CapabilityLayerResolutionPlan(
            string SelectionDisplayName,
            IReadOnlyList<LanguageModelDefinition> Cands,
            IReadOnlyList<string> Issues)
        {
            public ResolutionPlan ToResolutionPlan()
            {
                return new ResolutionPlan(
                    UseFallback: true,
                    SelectionDisplayName,
                    Cands,
                    Issues);
            }
        }
    }
}
