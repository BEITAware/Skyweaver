using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Xml;
using System.Xml.Linq;
using Ferrita.Controls.AgentConfigurationControl.Models;
using Ferrita.Controls.AgentConfigurationControl.Services;

namespace Ferrita.Services.FerritaTools
{
    public sealed class FerritaToolManager
    {
        private const string DefaultIconPath = "pack://application:,,,/Resources/Script.png";
        private const string ToolNamespacePrefix = "Ferrita.Tools";
        private static readonly string[] s_supportedIconExtensions = [".png", ".ico", ".jpg", ".jpeg", ".bmp"];
        private readonly FerritaToolConfigurationRepository _configurationRepository = new();

        public string ConfigurationDirectoryPath => _configurationRepository.ConfigurationDirectoryPath;

        public string ConfigurationFilePath => _configurationRepository.ConfigurationFilePath;

        public string ToolDirectoryPath => Path.Combine(ResolveContentRootPath(), "Tools");

        public string IconDirectoryPath => Path.Combine(ResolveContentRootPath(), "Icons");

        public IReadOnlyList<FerritaToolRegistration> GetRegisteredTools(bool resolveIcons = true)
        {
            var configuredStates = _configurationRepository.Load();
            var registrations = new List<FerritaToolRegistration>();
            var usedToolNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var toolType in DiscoverToolTypes())
            {
                var tool = CreateTool(toolType);
                var baseDefinition = tool.Definition ?? throw new InvalidOperationException($"{toolType.FullName} returned a null definition.");
                var persistedState = configuredStates.TryGetValue(baseDefinition.Name, out var storedState)
                    ? storedState
                    : new FerritaToolPersistedState(baseDefinition.Name, isEnabled: true, configuration: null);
                var configurationState = persistedState.ToConfigurationState();
                var definition = ResolveEffectiveDefinition(tool, baseDefinition, configurationState);

                if (!usedToolNames.Add(definition.Name))
                {
                    throw new InvalidOperationException($"Duplicate tool name detected: {definition.Name}");
                }

                registrations.Add(new FerritaToolRegistration(
                    tool,
                    baseDefinition,
                    definition,
                    resolveIcons ? ResolveIconPath(definition.IconName) : DefaultIconPath,
                    baseDefinition.IsSystemTool ? true : persistedState.IsEnabled,
                    configurationState));
            }

            return registrations
                .OrderBy(item => item.Definition.Name, StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        public IReadOnlyList<FerritaToolRegistration> GetEnabledTools(bool resolveIcons = true)
        {
            return GetRegisteredTools(resolveIcons)
                .Where(item => item.IsEnabled)
                .ToArray();
        }

        public void SaveConfiguration(IEnumerable<KeyValuePair<string, bool>> states)
        {
            ArgumentNullException.ThrowIfNull(states);

            var mergedStates = new Dictionary<string, FerritaToolPersistedState>(
                _configurationRepository.Load(),
                StringComparer.OrdinalIgnoreCase);

            foreach (var state in states)
            {
                var toolName = (state.Key ?? string.Empty).Trim();
                if (toolName.Length == 0)
                {
                    continue;
                }

                mergedStates[toolName] = mergedStates.TryGetValue(toolName, out var existingState)
                    ? existingState.WithIsEnabled(state.Value)
                    : new FerritaToolPersistedState(toolName, state.Value, configuration: null);
            }

            _configurationRepository.Save(mergedStates.Values);
        }

        public void SaveConfiguration(IEnumerable<FerritaToolPersistedState> states)
        {
            ArgumentNullException.ThrowIfNull(states);

            var mergedStates = new Dictionary<string, FerritaToolPersistedState>(
                _configurationRepository.Load(),
                StringComparer.OrdinalIgnoreCase);

            foreach (var state in states)
            {
                mergedStates[state.ToolName] = state;
            }

            _configurationRepository.Save(mergedStates.Values);
        }

        public async Task<FerritaToolResult> ExecuteAsync(
            string toolName,
            IReadOnlyDictionary<string, string?>? rawArguments,
            FerritaToolContext context,
            CancellationToken cancellationToken = default)
        {
            return await ExecuteAsync(
                toolName,
                rawArguments,
                context,
                agent: null,
                hasHostConfirmation: false,
                cancellationToken).ConfigureAwait(false);
        }

        public async Task<FerritaToolResult> ExecuteAsync(
            string toolName,
            IReadOnlyDictionary<string, string?>? rawArguments,
            FerritaToolContext context,
            AgentDefinition? agent,
            bool hasHostConfirmation = false,
            CancellationToken cancellationToken = default)
        {
            var registration = GetToolRegistrationOrThrow(toolName);
            EnsureExecutionAllowed(registration, agent, hasHostConfirmation);

            var arguments = FerritaToolArguments.Bind(registration.Definition.Parameters, rawArguments);
            var executionContext = context.WithCurrentToolConfiguration(
                registration.Definition.Name,
                registration.ConfigurationState.GetPayload());
            return await registration.Tool.ExecuteAsync(executionContext, arguments, cancellationToken).ConfigureAwait(false);
        }

        // Input XML:
        // <Tool ToolName="tool_name">
        //   <param_a>value</param_a>
        // </Tool>
        // <ToolAsync ToolName="tool_name">
        //   <param_a>value</param_a>
        // </ToolAsync>
        //
        // Output XML:
        // <ToolsReturn>
        //   <ToolReturn ToolName="tool_name" ToolCallId="TC1">
        //     <StringReturn1>...</StringReturn1>
        //   </ToolReturn>
        // </ToolsReturn>
        public IReadOnlyList<FerritaToolInvocation> ParseToolInvocationXml(string toolInvocationXml)
        {
            if (string.IsNullOrWhiteSpace(toolInvocationXml))
            {
                throw new InvalidOperationException("No tool invocation XML was provided.");
            }

            var invocationDocument = XDocument.Parse(toolInvocationXml, LoadOptions.PreserveWhitespace);
            var root = invocationDocument.Root;
            if (root == null || !(IsElementNamed(root, "Tool") || IsElementNamed(root, "ToolAsync")))
            {
                throw new InvalidOperationException("Tool invocation XML must use a single <Tool> or <ToolAsync> root element.");
            }

            return ParseInvocationDocument(invocationDocument);
        }

        public async Task<IReadOnlyList<FerritaToolReturnPayload>> ExecuteInvocationsAsync(
            IEnumerable<FerritaToolInvocation> invocations,
            FerritaToolContext context,
            CancellationToken cancellationToken = default)
        {
            return await ExecuteInvocationsAsync(
                invocations,
                context,
                agent: null,
                hasHostConfirmation: false,
                cancellationToken).ConfigureAwait(false);
        }

        public async Task<IReadOnlyList<FerritaToolReturnPayload>> ExecuteInvocationsAsync(
            IEnumerable<FerritaToolInvocation> invocations,
            FerritaToolContext context,
            AgentDefinition? agent,
            bool hasHostConfirmation = false,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(invocations);

            var toolReturns = new List<FerritaToolReturnPayload>();
            foreach (var invocation in invocations)
            {
                cancellationToken.ThrowIfCancellationRequested();

                try
                {
                    var result = await ExecuteAsync(
                        invocation.ToolName,
                        invocation.RawArguments,
                        context,
                        agent,
                        hasHostConfirmation,
                        cancellationToken).ConfigureAwait(false);
                    toolReturns.Add(CreateToolReturnPayload(invocation.ToolName, result));
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    toolReturns.Add(CreateErrorToolReturnPayload(invocation.ToolName, $"Tool execution failed: {ex.Message}"));
                }
            }

            return toolReturns;
        }

        public FerritaToolReturnPayload CreateToolReturnPayload(string toolName, FerritaToolResult result)
        {
            return CreateToolReturnPayload(toolName, result, toolCallId: null);
        }

        public FerritaToolReturnPayload CreateToolReturnPayload(
            string toolName,
            FerritaToolResult result,
            string? toolCallId)
        {
            return new FerritaToolReturnPayload(toolName, result, toolCallId);
        }

        public FerritaToolReturnPayload CreateErrorToolReturnPayload(string toolName, string errorMessage)
        {
            return CreateErrorToolReturnPayload(toolName, errorMessage, toolCallId: null);
        }

        public FerritaToolReturnPayload CreateErrorToolReturnPayload(
            string toolName,
            string errorMessage,
            string? toolCallId)
        {
            return CreateToolReturnPayload(toolName, FerritaToolResult.Failure(errorMessage), toolCallId);
        }

        public string BuildToolsReturnXml(IEnumerable<FerritaToolReturnPayload> toolReturns)
        {
            ArgumentNullException.ThrowIfNull(toolReturns);

            var document = new XDocument(new XElement(
                "ToolsReturn",
                toolReturns.Select(CreateToolReturnElement)));
            return document.ToString();
        }

        public async Task<string> ExecuteXmlAsync(
            string toolsInvocationXml,
            FerritaToolContext context,
            CancellationToken cancellationToken = default)
        {
            return await ExecuteXmlAsync(
                toolsInvocationXml,
                context,
                agent: null,
                hasHostConfirmation: false,
                cancellationToken).ConfigureAwait(false);
        }

        public async Task<string> ExecuteXmlAsync(
            string toolsInvocationXml,
            FerritaToolContext context,
            AgentDefinition? agent,
            bool hasHostConfirmation = false,
            CancellationToken cancellationToken = default)
        {
            IReadOnlyList<FerritaToolInvocation> invocations;
            try
            {
                invocations = ParseToolInvocationXml(toolsInvocationXml);
            }
            catch (Exception ex) when (ex is InvalidOperationException or XmlException)
            {
                return BuildToolsReturnXml([CreateErrorToolReturnPayload("_request", ex.Message)]);
            }

            var toolReturns = await ExecuteInvocationsAsync(
                invocations,
                context,
                agent,
                hasHostConfirmation,
                cancellationToken).ConfigureAwait(false);
            return BuildToolsReturnXml(toolReturns);
        }

        public void OpenDirectory(string path)
        {
            Directory.CreateDirectory(path);

            Process.Start(new ProcessStartInfo
            {
                FileName = path,
                UseShellExecute = true
            });
        }

        private static IReadOnlyDictionary<string, string?> ParseRawArguments(XElement toolElement)
        {
            var arguments = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);

            foreach (var child in toolElement.Elements())
            {
                if (string.Equals(child.Name.LocalName, "Parameter", StringComparison.OrdinalIgnoreCase))
                {
                    var parameterName = GetAttributeValue(child, "Name")
                        ?? GetAttributeValue(child, "ParameterName")
                        ?? GetAttributeValue(child, "Key");
                    if (!string.IsNullOrWhiteSpace(parameterName))
                    {
                        arguments[parameterName] = NormalizeRawArgument(child, unwrapParameterElement: true);
                    }

                    continue;
                }

                arguments[child.Name.LocalName] = NormalizeRawArgument(child, unwrapParameterElement: false);
            }

            return arguments;
        }

        private static string? GetAttributeValue(XElement element, string attributeName)
        {
            ArgumentNullException.ThrowIfNull(element);

            return element.Attributes()
                .FirstOrDefault(attribute =>
                    string.Equals(attribute.Name.LocalName, attributeName, StringComparison.OrdinalIgnoreCase))
                ?.Value
                ?.Trim();
        }

        private static string? NormalizeRawArgument(XElement element, bool unwrapParameterElement)
        {
            if (element.HasElements)
            {
                var xmlText = unwrapParameterElement
                    ? string.Concat(element.Nodes().Select(node => node.ToString(SaveOptions.DisableFormatting)))
                    : element.ToString(SaveOptions.DisableFormatting);
                return string.IsNullOrWhiteSpace(xmlText) ? null : xmlText.Trim();
            }

            var attributeValue = GetAttributeValue(element, "Value");
            if (!string.IsNullOrWhiteSpace(attributeValue))
            {
                return attributeValue;
            }

            return NormalizeRawArgument(element.Value);
        }

        private static string? NormalizeRawArgument(string? value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }

        private static IReadOnlyList<FerritaToolInvocation> ParseInvocationDocument(XDocument invocationDocument)
        {
            var root = invocationDocument.Root;
            if (root == null)
            {
                throw new InvalidOperationException("Tool invocation XML is missing a root element.");
            }

            var isAsyncInvocation = IsElementNamed(root, "ToolAsync");
            if (!(IsElementNamed(root, "Tool") || isAsyncInvocation))
            {
                throw new InvalidOperationException("Tool invocation XML must use a single <Tool> or <ToolAsync> root element.");
            }

            var toolName = GetAttributeValue(root, "ToolName")
                ?? GetAttributeValue(root, "Name");
            if (string.IsNullOrWhiteSpace(toolName))
            {
                throw new InvalidOperationException($"<{root.Name.LocalName}> element is missing ToolName.");
            }

            return
            [
                new FerritaToolInvocation(
                    toolName,
                    ParseRawArguments(root),
                    root,
                    isAsyncInvocation)
            ];
        }

        private static bool IsElementNamed(XElement element, string name)
        {
            return string.Equals(element.Name.LocalName, name, StringComparison.OrdinalIgnoreCase);
        }

        private static XElement CreateToolReturnElement(FerritaToolReturnPayload payload)
        {
            var primaryMessage = SanitizeXmlText(payload.PrimaryMessage);
            var toolReturn = new XElement(
                "ToolReturn",
                new XAttribute("ToolName", payload.ToolName),
                payload.ToolCallId == null
                    ? null
                    : new XAttribute("ToolCallId", payload.ToolCallId),
                new XElement("StringReturn1", primaryMessage));

            if (!payload.IsSuccess)
            {
                toolReturn.Add(new XElement("ErrorMessage", primaryMessage));
            }

            var stringReturnIndex = 2;
            foreach (var item in payload.Result.Data.OrderBy(item => item.Key, StringComparer.OrdinalIgnoreCase))
            {
                var valueText = SanitizeXmlText($"{item.Key}={SerializeReturnValue(item.Value)}");
                toolReturn.Add(new XElement($"StringReturn{stringReturnIndex}", valueText));
                stringReturnIndex++;
            }

            return toolReturn;
        }

        private static string SerializeReturnValue(object? value)
        {
            return value switch
            {
                null => string.Empty,
                DateTimeOffset dateTimeOffset => dateTimeOffset.ToString("O", CultureInfo.InvariantCulture),
                DateTime dateTime => dateTime.ToString("O", CultureInfo.InvariantCulture),
                IFormattable formattable => formattable.ToString(null, CultureInfo.InvariantCulture) ?? string.Empty,
                _ => value.ToString() ?? string.Empty
            };
        }

        private static string SanitizeXmlText(string? text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            var builder = new StringBuilder(text.Length);
            for (var index = 0; index < text.Length; index++)
            {
                var current = text[index];
                if (char.IsHighSurrogate(current))
                {
                    if (index + 1 < text.Length &&
                        char.IsLowSurrogate(text[index + 1]) &&
                        XmlConvert.IsXmlSurrogatePair(current, text[index + 1]))
                    {
                        builder.Append(current);
                        builder.Append(text[index + 1]);
                        index++;
                        continue;
                    }

                    AppendEscapedCodeUnit(builder, current);
                    continue;
                }

                if (char.IsLowSurrogate(current))
                {
                    AppendEscapedCodeUnit(builder, current);
                    continue;
                }

                if (XmlConvert.IsXmlChar(current))
                {
                    builder.Append(current);
                    continue;
                }

                AppendEscapedCodeUnit(builder, current);
            }

            return builder.ToString();
        }

        private static void AppendEscapedCodeUnit(StringBuilder builder, char value)
        {
            builder.Append(@"\u");
            builder.Append(((int)value).ToString("X4", CultureInfo.InvariantCulture));
        }

        private static IReadOnlyList<Type> DiscoverToolTypes()
        {
            Type[] discoveredTypes;
            try
            {
                discoveredTypes = typeof(IFerritaTool).Assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                discoveredTypes = ex.Types
                    .Where(type => type != null)
                    .Cast<Type>()
                    .ToArray();
            }

            return discoveredTypes
                .Where(type =>
                    typeof(IFerritaTool).IsAssignableFrom(type) &&
                    type.Namespace != null &&
                    type.Namespace.StartsWith(ToolNamespacePrefix, StringComparison.Ordinal) &&
                    type is { IsInterface: false, IsAbstract: false })
                .OrderBy(type => type.FullName, StringComparer.Ordinal)
                .ToArray();
        }

        private static IFerritaTool CreateTool(Type toolType)
        {
            if (toolType.GetConstructor(Type.EmptyTypes) == null)
            {
                throw new InvalidOperationException($"Tool type '{toolType.FullName}' must expose a public parameterless constructor.");
            }

            return (IFerritaTool)Activator.CreateInstance(toolType)!;
        }

        private FerritaToolRegistration GetToolRegistrationOrThrow(string toolName)
        {
            var registration = GetRegisteredTools(resolveIcons: false).FirstOrDefault(item =>
                string.Equals(item.Definition.Name, toolName, StringComparison.OrdinalIgnoreCase));

            if (registration == null)
            {
                throw new InvalidOperationException($"Tool not found: {toolName}");
            }

            if (!registration.IsEnabled)
            {
                throw new InvalidOperationException($"Tool '{toolName}' is currently disabled.");
            }

            return registration;
        }

        private static void EnsureExecutionAllowed(
            FerritaToolRegistration registration,
            AgentDefinition? agent,
            bool hasHostConfirmation)
        {
            if (agent == null || !registration.RequiresAgentPermission)
            {
                return;
            }

            var decision = AgentToolPermissionEvaluator.Resolve(agent, registration);
            switch (decision)
            {
                case AgentToolEffectiveDecision.Allowed:
                    return;

                case AgentToolEffectiveDecision.RequiresUserConfirmation when hasHostConfirmation:
                    return;

                case AgentToolEffectiveDecision.RequiresUserConfirmation:
                    throw new InvalidOperationException(
                        $"Tool '{registration.Definition.Name}' requires host confirmation before execution.");

                default:
                    throw new InvalidOperationException(
                        $"Tool '{registration.Definition.Name}' is not available to agent '{agent.AgentId}'.");
            }
        }

        private static FerritaToolDefinition ResolveEffectiveDefinition(
            IFerritaTool tool,
            FerritaToolDefinition baseDefinition,
            FerritaToolConfigurationState configurationState)
        {
            if (tool is not IFerritaToolConfigurationProvider configurationProvider)
            {
                return baseDefinition;
            }

            var effectiveDefinition = configurationProvider.GetEffectiveDefinition(configurationState)
                ?? throw new InvalidOperationException(
                    $"Tool '{baseDefinition.Name}' returned a null effective definition.");

            ValidateEffectiveDefinition(tool.GetType(), baseDefinition, effectiveDefinition);
            return effectiveDefinition;
        }

        private static void ValidateEffectiveDefinition(
            Type toolType,
            FerritaToolDefinition baseDefinition,
            FerritaToolDefinition effectiveDefinition)
        {
            if (!string.Equals(baseDefinition.Name, effectiveDefinition.Name, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    $"Tool '{toolType.FullName}' changed its public tool name from '{baseDefinition.Name}' to '{effectiveDefinition.Name}'.");
            }

            if (baseDefinition.IsSystemTool != effectiveDefinition.IsSystemTool)
            {
                throw new InvalidOperationException(
                    $"Tool '{toolType.FullName}' changed its system-tool flag for '{baseDefinition.Name}'.");
            }

            if (baseDefinition.SupportsAsyncInvocation != effectiveDefinition.SupportsAsyncInvocation)
            {
                throw new InvalidOperationException(
                    $"Tool '{toolType.FullName}' changed its async-invocation support for '{baseDefinition.Name}'.");
            }
        }

        private string ResolveContentRootPath()
        {
            var projectDirectory = TryResolveProjectDirectory();
            if (!string.IsNullOrWhiteSpace(projectDirectory))
            {
                return projectDirectory;
            }

            return AppContext.BaseDirectory;
        }

        private string ResolveIconPath(string? iconName)
        {
            if (string.IsNullOrWhiteSpace(iconName))
            {
                return DefaultIconPath;
            }

            var trimmedIconName = iconName.Trim();
            if (trimmedIconName.StartsWith("pack://", StringComparison.OrdinalIgnoreCase) ||
                Path.IsPathRooted(trimmedIconName))
            {
                return trimmedIconName;
            }

            foreach (var candidate in EnumerateIconCandidates(trimmedIconName))
            {
                if (File.Exists(candidate))
                {
                    return candidate;
                }
            }

            var resourceIcon = TryResolveResourceIcon(trimmedIconName);
            return resourceIcon ?? DefaultIconPath;
        }

        private IEnumerable<string> EnumerateIconCandidates(string iconName)
        {
            IEnumerable<string> candidateNames = Path.HasExtension(iconName)
                ? [iconName]
                : s_supportedIconExtensions.Select(extension => $"{iconName}{extension}").Append(iconName);

            var candidateDirectories = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                IconDirectoryPath,
                Path.Combine(AppContext.BaseDirectory, "Icons")
            };

            foreach (var directory in candidateDirectories)
            {
                foreach (var candidateName in candidateNames)
                {
                    yield return Path.Combine(directory, candidateName);
                }
            }
        }

        private static string? TryResolveProjectDirectory()
        {
            var currentDirectory = new DirectoryInfo(AppContext.BaseDirectory);
            while (currentDirectory != null)
            {
                if (File.Exists(Path.Combine(currentDirectory.FullName, "Ferrita.csproj")))
                {
                    return currentDirectory.FullName;
                }

                currentDirectory = currentDirectory.Parent;
            }

            return null;
        }

        private static string? TryResolveResourceIcon(string iconName)
        {
            IEnumerable<string> fileNames = Path.HasExtension(iconName)
                ? [iconName]
                : s_supportedIconExtensions
                    .Where(extension => string.Equals(extension, ".png", StringComparison.OrdinalIgnoreCase) ||
                                        string.Equals(extension, ".ico", StringComparison.OrdinalIgnoreCase))
                    .Select(extension => $"{iconName}{extension}");

            var applicationType = Type.GetType(
                "System.Windows.Application, PresentationFramework",
                throwOnError: false);
            var getResourceStreamMethod = applicationType?.GetMethod(
                "GetResourceStream",
                BindingFlags.Public | BindingFlags.Static,
                binder: null,
                types: [typeof(Uri)],
                modifiers: null);
            if (getResourceStreamMethod == null)
            {
                return null;
            }

            try
            {
                foreach (var fileName in fileNames)
                {
                    var relativeUri = new Uri($"/Resources/{fileName}", UriKind.Relative);
                    if (getResourceStreamMethod.Invoke(null, [relativeUri]) != null)
                    {
                        return $"pack://application:,,,/Resources/{fileName}";
                    }
                }
            }
            catch (Exception ex) when (ex is FileNotFoundException or FileLoadException or TypeLoadException or TargetInvocationException)
            {
                return null;
            }

            return null;
        }
    }

    public sealed class FerritaToolRegistration
    {
        private const string FallbackIconPath = "pack://application:,,,/Resources/Script.png";

        public FerritaToolRegistration(
            IFerritaTool tool,
            FerritaToolDefinition baseDefinition,
            FerritaToolDefinition definition,
            string iconPath,
            bool isEnabled,
            FerritaToolConfigurationState configurationState)
        {
            Tool = tool;
            BaseDefinition = baseDefinition ?? throw new ArgumentNullException(nameof(baseDefinition));
            Definition = definition ?? throw new ArgumentNullException(nameof(definition));
            IconPath = string.IsNullOrWhiteSpace(iconPath) ? FallbackIconPath : iconPath;
            IsEnabled = isEnabled;
            ConfigurationState = configurationState ?? throw new ArgumentNullException(nameof(configurationState));
        }

        public IFerritaTool Tool { get; }

        public FerritaToolDefinition BaseDefinition { get; }

        public FerritaToolDefinition Definition { get; }

        public Type ImplementationType => Tool.GetType();

        public string IconPath { get; }

        public bool IsEnabled { get; }

        public FerritaToolConfigurationState ConfigurationState { get; }

        public bool IsSystemTool => Definition.IsSystemTool;

        public bool CanUserDisable => Definition.CanUserDisable;

        public bool RequiresAgentPermission => Definition.RequiresAgentPermission;

        public bool CanBelongToToolKit => Definition.CanBelongToToolKit;

        public bool HasCustomConfiguration => Tool is IFerritaToolConfigurationProvider;

        public bool HasCustomInvocationPresentation => Tool is IFerritaToolInvocationPresentationProvider;

        public FerritaToolConfigurationPresenter? CreateConfigurationPresenter()
        {
            if (Tool is not IFerritaToolConfigurationProvider configurationProvider)
            {
                return null;
            }

            return configurationProvider.CreateConfigurationPresenter(
                new FerritaToolConfigurationEditorContext(
                    BaseDefinition,
                    Definition,
                    ConfigurationState));
        }

        public FrameworkElement? CreateInvocationPresentation(FerritaToolInvocationPresentationState state)
        {
            ArgumentNullException.ThrowIfNull(state);

            if (Tool is not IFerritaToolInvocationPresentationProvider presentationProvider)
            {
                return null;
            }

            return presentationProvider.CreateInvocationPresentation(
                new FerritaToolInvocationPresentationContext(
                    BaseDefinition,
                    Definition,
                    IconPath,
                    state));
        }

        public FrameworkElement? CreateConfirmationPresentation(FerritaToolInvocationPresentationState state)
        {
            ArgumentNullException.ThrowIfNull(state);

            if (Tool is IFerritaToolConfirmationPresentationProvider confirmationProvider)
            {
                return confirmationProvider.CreateConfirmationPresentation(
                    new FerritaToolInvocationPresentationContext(
                        BaseDefinition,
                        Definition,
                        IconPath,
                        state));
            }

            return CreateInvocationPresentation(state);
        }
    }
}
