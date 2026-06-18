using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using Ferrita.Controls.AgentConfigurationControl.Models;
using Ferrita.Controls.LanguageModelConfigurationControl.Models;
using Ferrita.Controls.LanguageModelConfigurationControl.Services;
using Ferrita.Services.Directories;
using Ferrita.Services.FerritaTools;

namespace Ferrita.Services.AgentLoop
{
    internal sealed class AgentLoopPreparedRequestDebugSnapshot
    {
        public string SystemPrompt { get; init; } = string.Empty;

        public string Input { get; init; } = string.Empty;

        public IReadOnlyList<LanguageModelChatMessage> PersistentHistory { get; init; } =
            Array.Empty<LanguageModelChatMessage>();

        public IReadOnlyList<LanguageModelChatMessage> TurnHistory { get; init; } =
            Array.Empty<LanguageModelChatMessage>();

        public IReadOnlyList<LanguageModelChatMessage> PreparedMessages { get; init; } =
            Array.Empty<LanguageModelChatMessage>();

        public AgentLoopContextCompressionInfo? ContextCompression { get; init; }
    }

    internal sealed class AgentLoopDebugRunContext
    {
        public AgentLoopDebugRunContext(string runDirectoryPath, string runId)
        {
            RunDirectoryPath = runDirectoryPath;
            RunId = runId;
        }

        public string RunDirectoryPath { get; }

        public string RunId { get; }
    }

    internal sealed class AgentLoopStreamingUpdateDebugSnapshot
    {
        public int SequenceNumber { get; init; }

        public DateTimeOffset ReceivedAtLocal { get; init; }

        public LanguageModelStreamingChatUpdate Update { get; init; } = new();

        public bool WasAppendedToRawContent { get; init; }

        public int RawContentLengthBeforeAppend { get; init; }

        public int RawContentLengthAfterAppend { get; init; }

        public string RawContentTailAfterAppend { get; init; } = string.Empty;
    }

    internal static class AgentLoopDebugRecorder
    {
        private static readonly JsonSerializerOptions s_jsonOptions = new()
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        public static AgentLoopDebugRunContext? TryCreateRunContext(AgentLoopRequest request)
        {
#if DEBUG
            ArgumentNullException.ThrowIfNull(request);

            try
            {
                var rootDirectoryPath = FerritaDirectoryRuntime.Instance.DebugDirectoryPath;
                Directory.CreateDirectory(rootDirectoryPath);

                var timestamp = DateTimeOffset.Now.ToString("yyyyMMdd_HHmmss_fff");
                var sessionSegment = SanitizeSegment(request.ToolContext.SessionTitle, "session");
                var agentSegment = SanitizeSegment(request.Agent.DisplayNameOrFallback, "agent");
                var runId = $"{timestamp}_{sessionSegment}_{agentSegment}_{Guid.NewGuid():N}";
                var runDirectoryPath = Path.Combine(rootDirectoryPath, runId);
                Directory.CreateDirectory(runDirectoryPath);

                return new AgentLoopDebugRunContext(runDirectoryPath, runId);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"AgentLoop debug initialization failed: {ex}");
                return null;
            }
#else
            return null;
#endif
        }

        public static void RecordRunStart(
            AgentLoopDebugRunContext? runContext,
            AgentLoopRequest request,
            string systemPrompt)
        {
#if DEBUG
            if (runContext == null)
            {
                return;
            }

            TryWriteJson(
                runContext,
                "run.json",
                new
                {
                    Kind = "AgentLoopRun",
                    RunId = runContext.RunId,
                    GeneratedAtLocal = DateTimeOffset.Now,
                    Agent = BuildAgentRecord(request.Agent),
                    ToolContext = BuildToolContextRecord(request.ToolContext),
                    SupportsHostToolConfirmation = request.ToolConfirmationCallback != null,
                    InitialInput = request.Input ?? string.Empty,
                    SystemPrompt = systemPrompt ?? string.Empty,
                    InitialHistory = BuildMessageRecords(request.History),
                    InitialHistoryTranscript = BuildTranscript(request.History)
                });
#endif
        }

        public static void RecordPreparedRequest(
            AgentLoopDebugRunContext? runContext,
            AgentLoopRequest request,
            AgentLoopPreparedRequestDebugSnapshot snapshot,
            IReadOnlyList<LanguageModelDefinition> candidates,
            LanguageModelDefinition activeCandidate,
            int iterationNumber,
            int attemptNumber)
        {
#if DEBUG
            if (runContext == null)
            {
                return;
            }

            var fileName = BuildMainRequestFileName(iterationNumber, attemptNumber);
            TryWriteJson(
                runContext,
                fileName,
                new
                {
                    Kind = "AgentLoopMainRequest",
                    RunId = runContext.RunId,
                    GeneratedAtLocal = DateTimeOffset.Now,
                    IterationNumber = iterationNumber,
                    AttemptNumber = attemptNumber,
                    Agent = BuildAgentRecord(request.Agent),
                    ToolContext = BuildToolContextRecord(request.ToolContext),
                    SupportsHostToolConfirmation = request.ToolConfirmationCallback != null,
                    ActiveCandidate = BuildModelRecord(activeCandidate),
                    CandidateOrder = (candidates ?? Array.Empty<LanguageModelDefinition>())
                        .Select(BuildModelRecord)
                        .ToArray(),
                    SystemPrompt = snapshot.SystemPrompt ?? string.Empty,
                    Input = snapshot.Input ?? string.Empty,
                    ContextCompression = snapshot.ContextCompression,
                    PersistentHistory = BuildMessageRecords(snapshot.PersistentHistory),
                    PersistentHistoryTranscript = BuildTranscript(snapshot.PersistentHistory),
                    TurnHistory = BuildMessageRecords(snapshot.TurnHistory),
                    TurnHistoryTranscript = BuildTranscript(snapshot.TurnHistory),
                    PreparedMessages = BuildMessageRecords(snapshot.PreparedMessages),
                    PreparedMessagesTranscript = BuildTranscript(snapshot.PreparedMessages)
                });
#endif
        }

        public static void RecordMainRequestFailure(
            AgentLoopDebugRunContext? runContext,
            AgentDefinition agent,
            LanguageModelDefinition activeCandidate,
            int iterationNumber,
            int attemptNumber,
            string? resolvedModelId,
            string partialRawContent,
            bool hasStartedStreaming,
            Exception exception)
        {
#if DEBUG
            if (runContext == null)
            {
                return;
            }

            var fileName = BuildMainFailureFileName(iterationNumber, attemptNumber);
            TryWriteJson(
                runContext,
                fileName,
                new
                {
                    Kind = "AgentLoopMainRequestFailure",
                    RunId = runContext.RunId,
                    GeneratedAtLocal = DateTimeOffset.Now,
                    IterationNumber = iterationNumber,
                    AttemptNumber = attemptNumber,
                    Agent = BuildAgentRecord(agent),
                    ActiveCandidate = BuildModelRecord(activeCandidate),
                    ResolvedModelId = NormalizeText(resolvedModelId),
                    FailedAfterStreamingStarted = hasStartedStreaming,
                    PartialRawContent = partialRawContent ?? string.Empty,
                    Exception = BuildExceptionRecord(exception)
                });
#endif
        }

        public static void RecordStreamingTrace(
            AgentLoopDebugRunContext? runContext,
            AgentDefinition agent,
            LanguageModelDefinition activeCandidate,
            int iterationNumber,
            int attemptNumber,
            string? resolvedModelId,
            IReadOnlyList<AgentLoopStreamingUpdateDebugSnapshot> updates,
            string finalRawContent,
            bool hasStartedStreaming,
            bool completedNormally,
            string? terminalMessage)
        {
#if DEBUG
            if (runContext == null)
            {
                return;
            }

            var fileName = BuildStreamingTraceFileName(iterationNumber, attemptNumber);
            TryWriteJson(
                runContext,
                fileName,
                new
                {
                    Kind = "AgentLoopStreamingTrace",
                    RunId = runContext.RunId,
                    GeneratedAtLocal = DateTimeOffset.Now,
                    IterationNumber = iterationNumber,
                    AttemptNumber = attemptNumber,
                    Agent = BuildAgentRecord(agent),
                    ActiveCandidate = BuildModelRecord(activeCandidate),
                    ResolvedModelId = NormalizeText(resolvedModelId),
                    HasStartedStreaming = hasStartedStreaming,
                    CompletedNormally = completedNormally,
                    TerminalMessage = NormalizeText(terminalMessage),
                    UpdateCount = updates?.Count ?? 0,
                    FinalRawContentLength = finalRawContent?.Length ?? 0,
                    FinalRawContentTail = TakeTail(finalRawContent, 512),
                    FinalRawContent = finalRawContent ?? string.Empty,
                    Updates = BuildStreamingUpdateRecords(updates)
                });
#endif
        }

        public static void RecordIterationOutcome(
            AgentLoopDebugRunContext? runContext,
            AgentDefinition agent,
            int iterationNumber,
            int attemptNumber,
            string? modelId,
            AgentAssistantResponse assistantResponse,
            IReadOnlyList<AgentToolBackfill> toolBackfills,
            AgentLoopFinalOutput? finalOutput)
        {
#if DEBUG
            if (runContext == null)
            {
                return;
            }

            var fileName = BuildIterationOutcomeFileName(iterationNumber);
            TryWriteJson(
                runContext,
                fileName,
                new
                {
                    Kind = "AgentLoopIterationOutcome",
                    RunId = runContext.RunId,
                    GeneratedAtLocal = DateTimeOffset.Now,
                    IterationNumber = iterationNumber,
                    AttemptNumber = attemptNumber,
                    Agent = BuildAgentRecord(agent),
                    ModelId = NormalizeText(modelId),
                    AssistantResponse = BuildAssistantResponseRecord(assistantResponse),
                    ToolBackfills = BuildToolBackfillRecords(toolBackfills),
                    FinalOutput = BuildFinalOutputRecord(finalOutput)
                });
#endif
        }

        private static object BuildAgentRecord(AgentDefinition agent)
        {
            return new
            {
                AgentId = agent.AgentIdOrFallback,
                DisplayName = agent.DisplayNameOrFallback,
                IsStructuredXmlIO = agent.IsStructuredXmlIO,
                LanguageModelSelectionMode = agent.LanguageModelSelectionMode.ToString(),
                SelectedLanguageModelKey = NormalizeText(agent.SelectedLanguageModelKey),
                SelectedCapabilityLayerKey = NormalizeText(agent.SelectedCapabilityLayerKey)
            };
        }

        private static object BuildToolContextRecord(FerritaToolContext toolContext)
        {
            ArgumentNullException.ThrowIfNull(toolContext);

            return new
            {
                ApplicationName = NormalizeText(toolContext.ApplicationName),
                SessionTitle = NormalizeText(toolContext.SessionTitle),
                WorkspacePath = NormalizeText(toolContext.WorkspacePath),
                Timestamp = toolContext.Timestamp,
                Properties = toolContext.Properties
                    .OrderBy(item => item.Key, StringComparer.OrdinalIgnoreCase)
                    .ToDictionary(
                        item => item.Key,
                        item => item.Value ?? string.Empty,
                        StringComparer.OrdinalIgnoreCase)
            };
        }

        private static object BuildModelRecord(LanguageModelDefinition model)
        {
            return new
            {
                Key = NormalizeText(model.Key),
                DisplayName = NormalizeText(model.DisplayName),
                SummaryModelId = NormalizeText(model.SummaryModelId),
                InterfaceType = NormalizeText(model.InterfaceType),
                EffectiveContextWindowTokens = model.EffectiveContextWindowTokens,
                IsFullyConfigured = model.IsFullyConfigured
            };
        }

        private static object[] BuildMessageRecords(IEnumerable<LanguageModelChatMessage>? messages)
        {
            if (messages == null)
            {
                return Array.Empty<object>();
            }

            return messages
                .Select((message, index) => new
                {
                    Index = index,
                    Role = message.Role.ToString(),
                    AuthorName = NormalizeText(message.AuthorName),
                    Content = message.Content ?? string.Empty
                })
                .Cast<object>()
                .ToArray();
        }

        private static object BuildAssistantResponseRecord(AgentAssistantResponse assistantResponse)
        {
            ArgumentNullException.ThrowIfNull(assistantResponse);

            return new
            {
                RawContent = assistantResponse.RawContent ?? string.Empty,
                NaturalLanguageText = assistantResponse.GetNaturalLanguageText(),
                Parts = assistantResponse.Parts
                    .Select((part, index) => new
                    {
                        Index = index,
                        Kind = part.Kind.ToString(),
                        Content = part.Content ?? string.Empty,
                        ParseError = NormalizeText(part.ParseError),
                        ToolCallIndex = part.ToolCallIndex,
                        ToolCalls = part.ToolCalls.Select(BuildToolInvocationRecord).ToArray()
                    })
                    .ToArray()
            };
        }

        private static object[] BuildToolBackfillRecords(IReadOnlyList<AgentToolBackfill>? toolBackfills)
        {
            if (toolBackfills == null || toolBackfills.Count == 0)
            {
                return Array.Empty<object>();
            }

            return toolBackfills
                .OrderBy(item => item.PartIndex)
                .ThenBy(item => item.ToolCallIndex)
                .Select(backfill => new
                {
                    backfill.PartIndex,
                    backfill.ToolCallIndex,
                    backfill.ToolCallId,
                    backfill.ToolsReturnXml,
                    ToolReturns = backfill.ToolReturns.Select(BuildToolReturnRecord).ToArray()
                })
                .Cast<object>()
                .ToArray();
        }

        private static object BuildToolInvocationRecord(FerritaToolInvocation invocation)
        {
            ArgumentNullException.ThrowIfNull(invocation);

            return new
            {
                ToolName = invocation.ToolName,
                invocation.IsAsyncInvocation,
                InvocationXml = invocation.InvocationXml,
                RawArguments = invocation.RawArguments
                    .OrderBy(item => item.Key, StringComparer.OrdinalIgnoreCase)
                    .ToDictionary(
                        item => item.Key,
                        item => item.Value,
                        StringComparer.OrdinalIgnoreCase)
            };
        }

        private static object BuildToolReturnRecord(FerritaToolReturnPayload payload)
        {
            ArgumentNullException.ThrowIfNull(payload);

            return new
            {
                payload.ToolName,
                payload.IsSuccess,
                payload.PrimaryMessage,
                Result = new
                {
                    payload.Result.IsSuccess,
                    payload.Result.Content,
                    Data = NormalizeDebugValue(payload.Result.Data)
                }
            };
        }

        private static object? BuildFinalOutputRecord(AgentLoopFinalOutput? finalOutput)
        {
            if (finalOutput == null)
            {
                return null;
            }

            return new
            {
                finalOutput.Content,
                Kind = finalOutput.Kind.ToString(),
                Source = finalOutput.Source.ToString(),
                finalOutput.IsStructuredXml,
                finalOutput.IsFromPassdownPayload
            };
        }

        private static object[] BuildStreamingUpdateRecords(
            IReadOnlyList<AgentLoopStreamingUpdateDebugSnapshot>? updates)
        {
            if (updates == null || updates.Count == 0)
            {
                return Array.Empty<object>();
            }

            return updates
                .OrderBy(update => update.SequenceNumber)
                .Select(update => new
                {
                    update.SequenceNumber,
                    update.ReceivedAtLocal,
                    update.WasAppendedToRawContent,
                    update.RawContentLengthBeforeAppend,
                    update.RawContentLengthAfterAppend,
                    RawContentTailAfterAppend = update.RawContentTailAfterAppend ?? string.Empty,
                    Update = BuildStreamingUpdateRecord(update.Update)
                })
                .Cast<object>()
                .ToArray();
        }

        private static object BuildStreamingUpdateRecord(LanguageModelStreamingChatUpdate update)
        {
            ArgumentNullException.ThrowIfNull(update);

            return new
            {
                TextDelta = update.TextDelta ?? string.Empty,
                RawText = update.RawText,
                update.WasTextSanitized,
                ModelId = NormalizeText(update.ModelId),
                Role = NormalizeText(update.Role),
                AuthorName = NormalizeText(update.AuthorName),
                FinishReason = NormalizeText(update.FinishReason),
                ResponseId = NormalizeText(update.ResponseId),
                MessageId = NormalizeText(update.MessageId),
                ConversationId = NormalizeText(update.ConversationId),
                update.CreatedAt,
                ContinuationToken = NormalizeText(update.ContinuationToken),
                RawRepresentationType = NormalizeText(update.RawRepresentationType),
                RawRepresentationSummary = NormalizeText(update.RawRepresentationSummary),
                AdditionalProperties = NormalizeDebugValue(update.AdditionalProperties),
                Contents = (update.Contents ?? Array.Empty<LanguageModelStreamingContentDebugItem>())
                    .Select(BuildStreamingContentRecord)
                    .ToArray()
            };
        }

        private static object BuildStreamingContentRecord(LanguageModelStreamingContentDebugItem content)
        {
            ArgumentNullException.ThrowIfNull(content);

            return new
            {
                ContentType = NormalizeText(content.ContentType),
                content.Text,
                Summary = NormalizeText(content.Summary),
                RawRepresentationType = NormalizeText(content.RawRepresentationType),
                RawRepresentationSummary = NormalizeText(content.RawRepresentationSummary),
                AdditionalProperties = NormalizeDebugValue(content.AdditionalProperties)
            };
        }

        private static object BuildExceptionRecord(Exception exception)
        {
            ArgumentNullException.ThrowIfNull(exception);

            return new
            {
                Type = exception.GetType().FullName,
                exception.Message,
                StackTrace = exception.ToString()
            };
        }

        private static object? NormalizeDebugValue(object? value)
        {
            if (value == null)
            {
                return null;
            }

            if (value is string or bool or byte or sbyte or short or ushort or int or uint or long or ulong or float or double or decimal or char or DateTime or DateTimeOffset or TimeSpan or Guid)
            {
                return value;
            }

            if (value is IReadOnlyDictionary<string, object?> readOnlyDictionary)
            {
                return readOnlyDictionary.ToDictionary(
                    item => item.Key,
                    item => NormalizeDebugValue(item.Value),
                    StringComparer.OrdinalIgnoreCase);
            }

            if (value is IDictionary dictionary)
            {
                var normalizedDictionary = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
                foreach (DictionaryEntry entry in dictionary)
                {
                    var key = entry.Key?.ToString();
                    if (string.IsNullOrWhiteSpace(key))
                    {
                        continue;
                    }

                    normalizedDictionary[key] = NormalizeDebugValue(entry.Value);
                }

                return normalizedDictionary;
            }

            if (value is IEnumerable enumerable && value is not string)
            {
                var normalizedList = new List<object?>();
                foreach (var item in enumerable)
                {
                    normalizedList.Add(NormalizeDebugValue(item));
                }

                return normalizedList;
            }

            return value.ToString();
        }

        private static string BuildTranscript(IEnumerable<LanguageModelChatMessage>? messages)
        {
            if (messages == null)
            {
                return string.Empty;
            }

            var builder = new StringBuilder();
            var index = 0;

            foreach (var message in messages)
            {
                if (builder.Length > 0)
                {
                    builder.AppendLine();
                    builder.AppendLine();
                }

                builder.Append('[');
                builder.Append(index);
                builder.Append("] ");
                builder.Append(message.Role);
                if (!string.IsNullOrWhiteSpace(message.AuthorName))
                {
                    builder.Append(" / ");
                    builder.Append(message.AuthorName);
                }

                builder.AppendLine();
                builder.Append(NormalizeText(message.Content));
                index++;
            }

            return builder.ToString();
        }

        private static void TryWriteJson(AgentLoopDebugRunContext runContext, string fileName, object payload)
        {
#if DEBUG
            try
            {
                var filePath = Path.Combine(runContext.RunDirectoryPath, fileName);
                var json = JsonSerializer.Serialize(payload, s_jsonOptions);
                File.WriteAllText(filePath, json, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"AgentLoop debug write failed for '{fileName}': {ex}");
            }
#endif
        }

        private static string BuildMainRequestFileName(int iterationNumber, int attemptNumber)
        {
            return $"iteration{iterationNumber:00}_attempt{attemptNumber:00}_main_request.json";
        }

        private static string BuildMainFailureFileName(int iterationNumber, int attemptNumber)
        {
            return $"iteration{iterationNumber:00}_attempt{attemptNumber:00}_main_failure.json";
        }

        private static string BuildStreamingTraceFileName(int iterationNumber, int attemptNumber)
        {
            return $"iteration{iterationNumber:00}_attempt{attemptNumber:00}_streaming_trace.json";
        }

        private static string BuildIterationOutcomeFileName(int iterationNumber)
        {
            return $"iteration{iterationNumber:00}_outcome.json";
        }

        private static string SanitizeSegment(string? value, string fallback)
        {
            var normalized = NormalizeText(value);
            if (normalized.Length == 0)
            {
                normalized = fallback;
            }

            var invalidCharacters = Path.GetInvalidFileNameChars();
            var builder = new StringBuilder(normalized.Length);

            foreach (var character in normalized)
            {
                if (char.IsWhiteSpace(character))
                {
                    builder.Append('_');
                    continue;
                }

                builder.Append(invalidCharacters.Contains(character) ? '_' : character);
            }

            var sanitized = builder.ToString().Trim('_', ' ', '.');
            if (sanitized.Length == 0)
            {
                sanitized = fallback;
            }

            return sanitized.Length <= 48 ? sanitized : sanitized[..48];
        }

        private static string NormalizeText(string? value)
        {
            return string.IsNullOrWhiteSpace(value)
                ? string.Empty
                : value.Trim();
        }

        private static string TakeTail(string? value, int maxLength)
        {
            if (string.IsNullOrEmpty(value) || maxLength <= 0)
            {
                return string.Empty;
            }

            return value.Length <= maxLength
                ? value
                : value[^maxLength..];
        }
    }
}
