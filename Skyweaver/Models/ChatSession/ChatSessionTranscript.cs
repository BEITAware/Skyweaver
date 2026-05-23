namespace Skyweaver.Models.ChatSession
{
    public enum ChatSessionTurnStatus
    {
        Pending = 0,
        Running = 1,
        Completed = 2,
        Cancelled = 3,
        Failed = 4,
        Interrupted = 5
    }

    public enum ChatSessionTranscriptEntryKind
    {
        UserMessage = 0,
        AgentMessage = 1,
        AgentFinalOutput = 2,
        ToolCall = 3,
        ToolOutput = 4,
        MalformedToolCall = 5,
        Reasoning = 6,
        SystemStatus = 7,
        ExecutionStatus = 8,
        Error = 9,
        ContextCompression = 10,
        StructuredPayload = 11,
        ResourceAttachment = 12,
        HandoffPayload = 13,
        DebugEvent = 14
    }

    public enum ChatSessionParticipantRole
    {
        User = 0,
        Assistant = 1,
        Tool = 2,
        System = 3,
        Runtime = 4
    }

    public enum TranscriptVisibility
    {
        Visible = 0,
        Collapsed = 1,
        Hidden = 2,
        DebugOnly = 3,
        InternalOnly = 4
    }

    public enum TranscriptLlmPolicy
    {
        Include = 0,
        Exclude = 1,
        ToolProtocol = 2,
        Summarize = 3,
        Redact = 4,
        CurrentTurnOnly = 5,
        Evidence = 6,
        DebugOnly = 7
    }

    public enum TranscriptHandoffPolicy
    {
        ExcludeByDefault = 0,
        FinalOutput = 1,
        Evidence = 2,
        ToolResult = 3,
        Summary = 4,
        FullTrace = 5
    }

    public enum ChatSessionEntryStatus
    {
        Open = 0,
        Streaming = 1,
        Completed = 2,
        Failed = 3,
        Cancelled = 4,
        Rejected = 5,
        Malformed = 6
    }

    public enum ChatSessionTranscriptBlockKind
    {
        Text = 0,
        StructuredXml = 1,
        Code = 2,
        Image = 3,
        Audio = 4,
        Video = 5,
        Document = 6,
        File = 7,
        ToolInvocationXml = 8,
        ToolOutputXml = 9,
        ReasoningText = 10,
        StatusText = 11,
        ErrorText = 12,
        ResourceReference = 13,
        CompressedSummary = 14
    }

    public sealed class ChatSessionTranscript
    {
        public string TranscriptId { get; set; } = Guid.NewGuid().ToString("N");

        public int SchemaVersion { get; set; } = 3;

        public long Revision { get; private set; }

        public List<ChatSessionTurnRecord> Turns { get; } = new();

        public List<ChatSessionTranscriptEntry> Entries { get; } = new();

        public ChatSessionTranscriptIndex Index { get; } = new();

        public void Touch()
        {
            Revision++;
            RebuildIndex();
        }

        public void SetRevision(long revision)
        {
            Revision = Math.Max(0, revision);
        }

        public void RebuildIndex()
        {
            Index.Rebuild(Entries);
        }
    }

    public sealed class ChatSessionTurnRecord
    {
        public string TurnId { get; set; } = Guid.NewGuid().ToString("N");

        public int TurnNumber { get; set; }

        public DateTime StartedAtUtc { get; set; } = DateTime.UtcNow;

        public DateTime? CompletedAtUtc { get; set; }

        public ChatSessionTurnStatus Status { get; set; } = ChatSessionTurnStatus.Pending;

        public string? UserEntryId { get; set; }

        public string? FinalEntryId { get; set; }

        public Dictionary<string, string> Metadata { get; } = new(StringComparer.OrdinalIgnoreCase);
    }

    public sealed class ChatSessionTranscriptEntry
    {
        public string EntryId { get; set; } = Guid.NewGuid().ToString("N");

        public string TurnId { get; set; } = string.Empty;

        public string? ParentEntryId { get; set; }

        public ChatSessionTranscriptEntryKind Kind { get; set; } = ChatSessionTranscriptEntryKind.UserMessage;

        public ChatSessionParticipantRole Role { get; set; } = ChatSessionParticipantRole.User;

        public DateTime TimestampUtc { get; set; } = DateTime.UtcNow;

        public string? NodeId { get; set; }

        public string? NodeTitle { get; set; }

        public string? AgentId { get; set; }

        public string? AgentName { get; set; }

        public int? IterationNumber { get; set; }

        public string? ToolCallId { get; set; }

        public string? ToolName { get; set; }

        public int? ToolCallIndex { get; set; }

        public TranscriptVisibility Visibility { get; set; } = TranscriptVisibility.Visible;

        public TranscriptLlmPolicy LlmPolicy { get; set; } = TranscriptLlmPolicy.Include;

        public TranscriptHandoffPolicy HandoffPolicy { get; set; } = TranscriptHandoffPolicy.ExcludeByDefault;

        public ChatSessionEntryStatus Status { get; set; } = ChatSessionEntryStatus.Completed;

        public List<ChatSessionTranscriptBlock> Blocks { get; } = new();

        public Dictionary<string, string> Metadata { get; } = new(StringComparer.OrdinalIgnoreCase);

        public long Revision { get; private set; }

        public void Touch()
        {
            Revision++;
        }

        public void SetRevision(long revision)
        {
            Revision = Math.Max(0, revision);
        }
    }

    public sealed class ChatSessionTranscriptBlock
    {
        public string BlockId { get; set; } = Guid.NewGuid().ToString("N");

        public ChatSessionTranscriptBlockKind Kind { get; set; } = ChatSessionTranscriptBlockKind.Text;

        public string Content { get; set; } = string.Empty;

        public string? Title { get; set; }

        public string? Language { get; set; }

        public string? MediaType { get; set; }

        public string? ResourceId { get; set; }

        public string? ResourcePath { get; set; }

        public Dictionary<string, string> Metadata { get; } = new(StringComparer.OrdinalIgnoreCase);

        public long Revision { get; private set; }

        public void Touch()
        {
            Revision++;
        }

        public void SetRevision(long revision)
        {
            Revision = Math.Max(0, revision);
        }
    }

    public sealed class ChatSessionTranscriptIndex
    {
        public Dictionary<string, List<ChatSessionTranscriptEntry>> EntriesByTurnId { get; } =
            new(StringComparer.OrdinalIgnoreCase);

        public Dictionary<string, List<ChatSessionTranscriptEntry>> EntriesByNodeId { get; } =
            new(StringComparer.OrdinalIgnoreCase);

        public Dictionary<string, List<ChatSessionTranscriptEntry>> EntriesByAgentId { get; } =
            new(StringComparer.OrdinalIgnoreCase);

        public Dictionary<ChatSessionTranscriptEntryKind, List<ChatSessionTranscriptEntry>> EntriesByKind { get; } =
            new();

        public Dictionary<string, List<ChatSessionTranscriptEntry>> EntriesByToolCallId { get; } =
            new(StringComparer.OrdinalIgnoreCase);

        public Dictionary<TranscriptLlmPolicy, List<ChatSessionTranscriptEntry>> EntriesByLlmPolicy { get; } =
            new();

        public Dictionary<TranscriptHandoffPolicy, List<ChatSessionTranscriptEntry>> EntriesByHandoffPolicy { get; } =
            new();

        public void Rebuild(IEnumerable<ChatSessionTranscriptEntry> entries)
        {
            Clear();

            foreach (var entry in entries.Where(entry => entry != null))
            {
                Add(EntriesByTurnId, entry.TurnId, entry);
                Add(EntriesByNodeId, entry.NodeId, entry);
                Add(EntriesByAgentId, entry.AgentId, entry);
                Add(EntriesByKind, entry.Kind, entry);
                Add(EntriesByToolCallId, entry.ToolCallId, entry);
                Add(EntriesByLlmPolicy, entry.LlmPolicy, entry);
                Add(EntriesByHandoffPolicy, entry.HandoffPolicy, entry);
            }
        }

        private void Clear()
        {
            EntriesByTurnId.Clear();
            EntriesByNodeId.Clear();
            EntriesByAgentId.Clear();
            EntriesByKind.Clear();
            EntriesByToolCallId.Clear();
            EntriesByLlmPolicy.Clear();
            EntriesByHandoffPolicy.Clear();
        }

        private static void Add<TKey>(
            IDictionary<TKey, List<ChatSessionTranscriptEntry>> index,
            TKey key,
            ChatSessionTranscriptEntry entry)
            where TKey : notnull
        {
            if (!index.TryGetValue(key, out var list))
            {
                list = new List<ChatSessionTranscriptEntry>();
                index[key] = list;
            }

            list.Add(entry);
        }

        private static void Add(
            IDictionary<string, List<ChatSessionTranscriptEntry>> index,
            string? key,
            ChatSessionTranscriptEntry entry)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return;
            }

            if (!index.TryGetValue(key.Trim(), out var list))
            {
                list = new List<ChatSessionTranscriptEntry>();
                index[key.Trim()] = list;
            }

            list.Add(entry);
        }
    }
}
