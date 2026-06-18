using Ferrita.Controls.WorkflowEditorControl.Models;
using Ferrita.Models.ChatSession;

namespace Ferrita.Services.ChatSession
{
    public enum SessionFlowHandoffProjectionPolicy
    {
        FinalOutputOnly = 0,
        OutputWithEvidence = 1,
        ToolResultsOnly = 2,
        SummaryOnly = 3,
        FullTrace = 4
    }

    public sealed class SessionFlowHandoffProjectionService
    {
        public SessionFlowPayload Project(
            ChatSessionTranscript transcript,
            string upstreamNodeId,
            SessionFlowHandoffProjectionPolicy policy = SessionFlowHandoffProjectionPolicy.FinalOutputOnly)
        {
            ArgumentNullException.ThrowIfNull(transcript);

            lock (transcript.SyncRoot)
            {
            var entries = transcript.Entries
                .Where(entry => string.Equals(entry.NodeId, upstreamNodeId, StringComparison.OrdinalIgnoreCase))
                .Where(entry => ShouldInclude(entry, policy))
                .ToArray();

            var content = string.Join(
                    Environment.NewLine + Environment.NewLine,
                    entries.SelectMany(entry => entry.Blocks.Select(block => block.Content))
                        .Where(content => !string.IsNullOrWhiteSpace(content)))
                .Trim();

            return SessionFlowPayload.FromNaturalLanguage(content);
            }
        }

        private static bool ShouldInclude(
            ChatSessionTranscriptEntry entry,
            SessionFlowHandoffProjectionPolicy policy)
        {
            return policy switch
            {
                SessionFlowHandoffProjectionPolicy.FinalOutputOnly =>
                    entry.HandoffPolicy == TranscriptHandoffPolicy.FinalOutput,
                SessionFlowHandoffProjectionPolicy.OutputWithEvidence =>
                    entry.HandoffPolicy is TranscriptHandoffPolicy.FinalOutput or TranscriptHandoffPolicy.Evidence,
                SessionFlowHandoffProjectionPolicy.ToolResultsOnly =>
                    entry.HandoffPolicy is TranscriptHandoffPolicy.ToolResult or TranscriptHandoffPolicy.Evidence,
                SessionFlowHandoffProjectionPolicy.SummaryOnly =>
                    entry.HandoffPolicy == TranscriptHandoffPolicy.Summary,
                SessionFlowHandoffProjectionPolicy.FullTrace =>
                    entry.HandoffPolicy != TranscriptHandoffPolicy.ExcludeByDefault ||
                    entry.Visibility != TranscriptVisibility.InternalOnly,
                _ => false
            };
        }
    }
}
