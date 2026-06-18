using System.Xml.Linq;
using Ferrita.Controls.AgentConfigurationControl.Models;

namespace Ferrita.Services.FerritaTools
{
    public sealed class FerritaToolContext
    {
        public string ApplicationName { get; init; } = "Ferrita";

        public string? SessionTitle { get; init; }

        public string? WorkspacePath { get; init; }

        public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.Now;

        public IReadOnlyDictionary<string, string> Properties { get; init; } =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public string? CurrentToolName { get; init; }

        public FerritaToolConfigurationState? CurrentToolConfiguration { get; init; }

        public AgentDefinition? CurrentAgent { get; init; }

        public bool SupportsHostToolConfirmation { get; init; }

        public IReadOnlyList<FerritaToolKitDefinition> AvailableToolKits { get; init; } =
            Array.Empty<FerritaToolKitDefinition>();

        public bool IsSubAgent { get; init; }

        public Func<FerritaToolProgressUpdate, CancellationToken, ValueTask>? ToolProgressReporter { get; init; }

        public bool SupportsToolProgress => ToolProgressReporter != null;

        public ValueTask ReportProgressAsync(
            FerritaToolProgressUpdate progress,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(progress);

            return ToolProgressReporter == null
                ? ValueTask.CompletedTask
                : ToolProgressReporter(progress.Normalize(), cancellationToken);
        }

        public FerritaToolContext WithToolProgressReporter(
            Func<FerritaToolProgressUpdate, CancellationToken, ValueTask>? toolProgressReporter)
        {
            return new FerritaToolContext
            {
                ApplicationName = ApplicationName,
                SessionTitle = SessionTitle,
                WorkspacePath = WorkspacePath,
                Timestamp = Timestamp,
                Properties = CloneProperties(Properties),
                CurrentToolName = CurrentToolName,
                CurrentToolConfiguration = CurrentToolConfiguration == null
                    ? null
                    : new FerritaToolConfigurationState(
                        CurrentToolConfiguration.ToolName,
                        CurrentToolConfiguration.GetPayload()),
                CurrentAgent = CurrentAgent,
                SupportsHostToolConfirmation = SupportsHostToolConfirmation,
                AvailableToolKits = CloneToolKits(AvailableToolKits),
                IsSubAgent = IsSubAgent,
                ToolProgressReporter = toolProgressReporter
            };
        }

        public FerritaToolContext WithRuntimeAgent(
            AgentDefinition? agent,
            bool supportsHostToolConfirmation)
        {
            return new FerritaToolContext
            {
                ApplicationName = ApplicationName,
                SessionTitle = SessionTitle,
                WorkspacePath = WorkspacePath,
                Timestamp = Timestamp,
                Properties = CloneProperties(Properties),
                CurrentToolName = CurrentToolName,
                CurrentToolConfiguration = CurrentToolConfiguration == null
                    ? null
                    : new FerritaToolConfigurationState(
                        CurrentToolConfiguration.ToolName,
                        CurrentToolConfiguration.GetPayload()),
                CurrentAgent = agent,
                SupportsHostToolConfirmation = supportsHostToolConfirmation,
                AvailableToolKits = CloneToolKits(AvailableToolKits),
                IsSubAgent = IsSubAgent,
                ToolProgressReporter = ToolProgressReporter
            };
        }

        public FerritaToolContext WithSubAgentMode(bool isSubAgent)
        {
            return new FerritaToolContext
            {
                ApplicationName = ApplicationName,
                SessionTitle = SessionTitle,
                WorkspacePath = WorkspacePath,
                Timestamp = Timestamp,
                Properties = CloneProperties(Properties),
                CurrentToolName = CurrentToolName,
                CurrentToolConfiguration = CurrentToolConfiguration == null
                    ? null
                    : new FerritaToolConfigurationState(
                        CurrentToolConfiguration.ToolName,
                        CurrentToolConfiguration.GetPayload()),
                CurrentAgent = CurrentAgent,
                SupportsHostToolConfirmation = SupportsHostToolConfirmation,
                AvailableToolKits = CloneToolKits(AvailableToolKits),
                IsSubAgent = isSubAgent,
                ToolProgressReporter = ToolProgressReporter
            };
        }

        public FerritaToolContext WithAvailableToolKits(IReadOnlyList<FerritaToolKitDefinition>? availableToolKits)
        {
            return new FerritaToolContext
            {
                ApplicationName = ApplicationName,
                SessionTitle = SessionTitle,
                WorkspacePath = WorkspacePath,
                Timestamp = Timestamp,
                Properties = CloneProperties(Properties),
                CurrentToolName = CurrentToolName,
                CurrentToolConfiguration = CurrentToolConfiguration == null
                    ? null
                    : new FerritaToolConfigurationState(
                        CurrentToolConfiguration.ToolName,
                        CurrentToolConfiguration.GetPayload()),
                CurrentAgent = CurrentAgent,
                SupportsHostToolConfirmation = SupportsHostToolConfirmation,
                AvailableToolKits = CloneToolKits(availableToolKits),
                IsSubAgent = IsSubAgent,
                ToolProgressReporter = ToolProgressReporter
            };
        }

        public FerritaToolContext WithCurrentToolConfiguration(string toolName, XElement? configuration)
        {
            var normalizedToolName = (toolName ?? string.Empty).Trim();
            if (normalizedToolName.Length == 0)
            {
                throw new ArgumentException("Tool name cannot be empty.", nameof(toolName));
            }

            return new FerritaToolContext
            {
                ApplicationName = ApplicationName,
                SessionTitle = SessionTitle,
                WorkspacePath = WorkspacePath,
                Timestamp = Timestamp,
                Properties = CloneProperties(Properties),
                CurrentToolName = normalizedToolName,
                CurrentToolConfiguration = new FerritaToolConfigurationState(normalizedToolName, configuration),
                CurrentAgent = CurrentAgent,
                SupportsHostToolConfirmation = SupportsHostToolConfirmation,
                AvailableToolKits = CloneToolKits(AvailableToolKits),
                IsSubAgent = IsSubAgent,
                ToolProgressReporter = ToolProgressReporter
            };
        }

        private static IReadOnlyDictionary<string, string> CloneProperties(IReadOnlyDictionary<string, string> properties)
        {
            return properties.Count == 0
                ? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                : new Dictionary<string, string>(properties, StringComparer.OrdinalIgnoreCase);
        }

        private static IReadOnlyList<FerritaToolKitDefinition> CloneToolKits(
            IReadOnlyList<FerritaToolKitDefinition>? toolKits)
        {
            if (toolKits == null || toolKits.Count == 0)
            {
                return Array.Empty<FerritaToolKitDefinition>();
            }

            return toolKits
                .Select(toolKit => toolKit.DeepClone())
                .ToArray();
        }
    }
}
