using System.Text;
using System.Xml;
using System.Xml.Linq;
using Skyweaver.Controls.AgentConfigurationControl.Models;
using Skyweaver.Controls.AgentConfigurationControl.Services;
using Skyweaver.Controls.ChatSessionControl.Services;
using Skyweaver.Controls.LanguageModelConfigurationControl.Models;
using Skyweaver.Controls.LanguageModelConfigurationControl.Services;
using Skyweaver.Services.SkyweaverTools;
using Skyweaver.Tools;

namespace Skyweaver.Services.AgentLoop
{
    public sealed class AgentLoopService
    {
        private const int MaxConsecutiveRecoverableFailures = 3;
        private const string RequestRepairToolName = "_request";
        private const int StreamingTraceRawContentTailLength = 256;

        private sealed class InvalidAssistantResponseException : InvalidOperationException
        {
            public InvalidAssistantResponseException(
                string message,
                int attemptNumber,
                string? modelId = null,
                AgentAssistantResponse? assistantResponse = null,
                IReadOnlyList<AgentToolBackfill>? toolBackfills = null,
                AgentLoopFinalOutput? latestMessageOutput = null,
                Exception? innerException = null)
                : base(message, innerException)
            {
                AttemptNumber = attemptNumber;
                ModelId = modelId;
                AssistantResponse = assistantResponse ?? new AgentAssistantResponse(string.Empty, Array.Empty<AgentAssistantResponsePart>());
                ToolBackfills = toolBackfills ?? Array.Empty<AgentToolBackfill>();
                LatestMessageOutput = latestMessageOutput;
            }

            public int AttemptNumber { get; }

            public string? ModelId { get; }

            public AgentAssistantResponse AssistantResponse { get; }

            public IReadOnlyList<AgentToolBackfill> ToolBackfills { get; }

            public AgentLoopFinalOutput? LatestMessageOutput { get; }
        }

        private sealed record ToolExecutionAuthorization(
            bool CanExecute,
            bool HasHostConfirmation,
            string? ErrorMessage);

        private sealed record StreamedResponseResult(
            int AttemptNumber,
            string? ModelId,
            AgentAssistantResponse AssistantResponse,
            IReadOnlyList<AgentToolBackfill> ToolBackfills,
            AgentLoopFinalOutput? FinalOutput,
            AgentLoopFinalOutput? LatestMessageOutput);

        private sealed class CreateMessageStreamingTracker
        {
            private const string ToolClosingTag = "</Tool>";
            private readonly AgentLoopOutputKind _outputKind;
            private int _emittedLength;

            public CreateMessageStreamingTracker(AgentLoopOutputKind outputKind)
            {
                _outputKind = outputKind;
            }

            public AgentLoopOutputKind OutputKind => _outputKind;

            public string ExtractDelta(string rawContent)
            {
                var visibleContent = ExtractVisibleCreateMessageContent(rawContent);
                if (visibleContent.Length <= _emittedLength)
                {
                    return string.Empty;
                }

                var delta = visibleContent[_emittedLength..];
                _emittedLength = visibleContent.Length;
                return delta;
            }

            private static string ExtractVisibleCreateMessageContent(string rawContent)
            {
                if (string.IsNullOrEmpty(rawContent))
                {
                    return string.Empty;
                }

                var builder = new StringBuilder();
                var searchIndex = 0;

                while (searchIndex < rawContent.Length)
                {
                    var toolStartIndex = IndexOfToolOpeningTag(rawContent, searchIndex);
                    if (toolStartIndex < 0)
                    {
                        break;
                    }

                    var toolOpenTagEndIndex = rawContent.IndexOf('>', toolStartIndex);
                    if (toolOpenTagEndIndex < 0)
                    {
                        break;
                    }

                    var toolOpenTag = rawContent.Substring(toolStartIndex, toolOpenTagEndIndex - toolStartIndex + 1);
                    var bodyStartIndex = toolOpenTagEndIndex + 1;
                    if (IsSelfClosingToolTag(toolOpenTag))
                    {
                        searchIndex = bodyStartIndex;
                        continue;
                    }

                    var toolCloseStartIndex = rawContent.IndexOf(
                        ToolClosingTag,
                        bodyStartIndex,
                        StringComparison.OrdinalIgnoreCase);
                    var visibleBodyEndIndex = toolCloseStartIndex >= 0
                        ? toolCloseStartIndex
                        : rawContent.Length;

                    if (IsCreateMessage(TryExtractToolName(toolOpenTag) ?? string.Empty) &&
                        visibleBodyEndIndex > bodyStartIndex)
                    {
                        builder.Append(rawContent, bodyStartIndex, visibleBodyEndIndex - bodyStartIndex);
                    }

                    if (toolCloseStartIndex < 0)
                    {
                        break;
                    }

                    searchIndex = toolCloseStartIndex + ToolClosingTag.Length;
                }

                return builder.ToString();
            }

            private static int IndexOfToolOpeningTag(string text, int startIndex)
            {
                var searchIndex = Math.Max(0, startIndex);

                while (searchIndex < text.Length)
                {
                    var matchIndex = text.IndexOf("<Tool", searchIndex, StringComparison.OrdinalIgnoreCase);
                    if (matchIndex < 0)
                    {
                        return -1;
                    }

                    var nameEndIndex = matchIndex + "<Tool".Length;
                    if (nameEndIndex >= text.Length ||
                        char.IsWhiteSpace(text[nameEndIndex]) ||
                        text[nameEndIndex] is '>' or '/')
                    {
                        return matchIndex;
                    }

                    searchIndex = matchIndex + 1;
                }

                return -1;
            }

            private static string? TryExtractToolName(string toolOpenTag)
            {
                if (string.IsNullOrWhiteSpace(toolOpenTag))
                {
                    return null;
                }

                var parseCandidate = IsSelfClosingToolTag(toolOpenTag)
                    ? toolOpenTag
                    : $"{toolOpenTag}</Tool>";

                try
                {
                    var element = XElement.Parse(parseCandidate, LoadOptions.PreserveWhitespace);
                    return element.Attributes()
                        .FirstOrDefault(attribute =>
                            string.Equals(attribute.Name.LocalName, "ToolName", StringComparison.OrdinalIgnoreCase))
                        ?.Value
                        ?.Trim();
                }
                catch
                {
                    return null;
                }
            }

            private static bool IsSelfClosingToolTag(string toolOpenTag)
            {
                return toolOpenTag.TrimEnd().EndsWith("/>", StringComparison.Ordinal);
            }
        }

        private readonly AgentSystemPromptBuilder _systemPromptBuilder;
        private readonly IAgentLanguageModelResolver _languageModelResolver;
        private readonly ILanguageModelChatService _chatService;
        private readonly SkyweaverToolManager _toolManager;
        private readonly AgentLoopContextManager _contextManager;

        public AgentLoopService()
            : this(
                new AgentSystemPromptBuilder(),
                new AgentLanguageModelResolver(),
                new LanguageModelChatService(),
                new SkyweaverToolManager(),
                new AgentLoopContextManager())
        {
        }

        public AgentLoopService(
            AgentSystemPromptBuilder systemPromptBuilder,
            IAgentLanguageModelResolver languageModelResolver,
            ILanguageModelChatService chatService,
            SkyweaverToolManager toolManager,
            AgentLoopContextManager contextManager)
        {
            _systemPromptBuilder = systemPromptBuilder ?? throw new ArgumentNullException(nameof(systemPromptBuilder));
            _languageModelResolver = languageModelResolver ?? throw new ArgumentNullException(nameof(languageModelResolver));
            _chatService = chatService ?? throw new ArgumentNullException(nameof(chatService));
            _toolManager = toolManager ?? throw new ArgumentNullException(nameof(toolManager));
            _contextManager = contextManager ?? throw new ArgumentNullException(nameof(contextManager));
        }

        public Task<AgentLoopResult> RunAsync(
            AgentLoopRequest request,
            CancellationToken cancellationToken = default)
        {
            return RunCoreAsync(request, onEventAsync: null, cancellationToken);
        }

        public Task<AgentLoopResult> RunStreamingAsync(
            AgentLoopRequest request,
            Func<AgentLoopRuntimeEvent, CancellationToken, ValueTask> onEventAsync,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(onEventAsync);
            return RunCoreAsync(request, onEventAsync, cancellationToken);
        }

        private async Task<AgentLoopResult> RunCoreAsync(
            AgentLoopRequest request,
            Func<AgentLoopRuntimeEvent, CancellationToken, ValueTask>? onEventAsync,
            CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            ValidateRequest(request);

            var systemPrompt = _systemPromptBuilder.BuildCompleteSystemPrompt(
                request.Agent,
                supportsHostToolConfirmation: request.ToolConfirmationCallback != null);
            var debugRunContext = AgentLoopDebugRecorder.TryCreateRunContext(request);
            AgentLoopDebugRecorder.RecordRunStart(debugRunContext, request, systemPrompt);

            var persistentHistory = (request.History ?? Array.Empty<LanguageModelChatMessage>())
                .Select(message => message.Clone())
                .ToList();
            var turnHistory = new List<LanguageModelChatMessage>();
            var iterations = new List<AgentLoopIteration>();
            string? lastModelId = null;
            AgentLoopFinalOutput? latestMessageOutput = null;
            var consecutiveRecoverableFailures = 0;

            for (var iterationNumber = 1; iterationNumber <= request.MaxIterations; iterationNumber++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var preparedContext = await _contextManager.PrepareAsync(
                    request.Agent,
                    systemPrompt,
                    request.Input,
                    persistentHistory,
                    turnHistory,
                    debugRunContext,
                    iterationNumber,
                    cancellationToken).ConfigureAwait(false);

                persistentHistory = preparedContext.PersistentHistory
                    .Select(message => message.Clone())
                    .ToList();
                turnHistory = preparedContext.TurnHistory
                    .Select(message => message.Clone())
                    .ToList();

                var preparedSnapshot = new AgentLoopPreparedRequestDebugSnapshot
                {
                    SystemPrompt = systemPrompt,
                    Input = request.Input,
                    PersistentHistory = preparedContext.PersistentHistory
                        .Select(message => message.Clone())
                        .ToArray(),
                    TurnHistory = preparedContext.TurnHistory
                        .Select(message => message.Clone())
                        .ToArray(),
                    PreparedMessages = preparedContext.PreparedMessages
                        .Select(message => message.Clone())
                        .ToArray(),
                    ContextCompression = preparedContext.ContextCompression
                };

                if (preparedContext.ContextCompression != null)
                {
                    await PublishAsync(
                        onEventAsync,
                        new AgentLoopRuntimeEvent
                        {
                            Kind = AgentLoopRuntimeEventKind.ContextCompressionApplied,
                            IterationNumber = iterationNumber,
                            ContextCompression = preparedContext.ContextCompression
                        },
                        cancellationToken).ConfigureAwait(false);
                }

                StreamedResponseResult response;
                try
                {
                    response = await InvokeModelStreamingAsync(
                        request,
                        preparedSnapshot,
                        debugRunContext,
                        iterationNumber,
                        latestMessageOutput,
                        onEventAsync,
                        cancellationToken).ConfigureAwait(false);
                }
                catch (InvalidAssistantResponseException ex)
                {
                    if (!string.IsNullOrWhiteSpace(ex.ModelId))
                    {
                        lastModelId = ex.ModelId;
                    }

                    if (ex.LatestMessageOutput != null)
                    {
                        latestMessageOutput = ex.LatestMessageOutput;
                    }

                    consecutiveRecoverableFailures++;
                    var repairMessage = BuildRepairMessage(ex.Message, consecutiveRecoverableFailures);
                    AppendFailedIterationHistory(ex, repairMessage, turnHistory);

                    await PublishAsync(
                        onEventAsync,
                        new AgentLoopRuntimeEvent
                        {
                            Kind = AgentLoopRuntimeEventKind.RepairMessageGenerated,
                            IterationNumber = iterationNumber,
                            ModelId = lastModelId,
                            RepairMessage = repairMessage
                        },
                        cancellationToken).ConfigureAwait(false);

                    AgentLoopDebugRecorder.RecordIterationOutcome(
                        debugRunContext,
                        request.Agent,
                        iterationNumber,
                        ex.AttemptNumber,
                        lastModelId,
                        ex.AssistantResponse,
                        ex.ToolBackfills,
                        repairMessage,
                        finalOutput: null);

                    iterations.Add(new AgentLoopIteration
                    {
                        IterationNumber = iterationNumber,
                        ModelId = lastModelId,
                        AssistantResponse = ex.AssistantResponse,
                        ToolBackfills = ex.ToolBackfills,
                        RepairMessage = repairMessage,
                        FinalOutput = null,
                        ContextCompression = preparedContext.ContextCompression
                    });

                    await PublishAsync(
                        onEventAsync,
                        new AgentLoopRuntimeEvent
                        {
                            Kind = AgentLoopRuntimeEventKind.IterationCompleted,
                            IterationNumber = iterationNumber,
                            ModelId = lastModelId,
                            FinalOutput = null
                        },
                        cancellationToken).ConfigureAwait(false);

                    if (consecutiveRecoverableFailures >= MaxConsecutiveRecoverableFailures)
                    {
                        return new AgentLoopResult
                        {
                            IsCompleted = false,
                            FailureReason =
                                $"The agent loop stopped after {MaxConsecutiveRecoverableFailures} consecutive failed iterations. Last error: {ex.Message}",
                            LastModelId = lastModelId,
                            Iterations = iterations.ToArray()
                        };
                    }

                    continue;
                }

                consecutiveRecoverableFailures = 0;

                if (!string.IsNullOrWhiteSpace(response.ModelId))
                {
                    lastModelId = response.ModelId;
                }

                if (response.LatestMessageOutput != null)
                {
                    latestMessageOutput = response.LatestMessageOutput;
                }

                AppendCurrentTurnHistory(
                    response.AssistantResponse,
                    response.ToolBackfills,
                    turnHistory);

                AgentLoopDebugRecorder.RecordIterationOutcome(
                    debugRunContext,
                    request.Agent,
                    iterationNumber,
                    response.AttemptNumber,
                    response.ModelId,
                    response.AssistantResponse,
                    response.ToolBackfills,
                    repairMessage: null,
                    response.FinalOutput);

                iterations.Add(new AgentLoopIteration
                {
                    IterationNumber = iterationNumber,
                    ModelId = response.ModelId,
                    AssistantResponse = response.AssistantResponse,
                    ToolBackfills = response.ToolBackfills,
                    FinalOutput = response.FinalOutput,
                    ContextCompression = preparedContext.ContextCompression
                });

                await PublishAsync(
                    onEventAsync,
                    new AgentLoopRuntimeEvent
                    {
                        Kind = AgentLoopRuntimeEventKind.IterationCompleted,
                        IterationNumber = iterationNumber,
                        ModelId = response.ModelId,
                        FinalOutput = response.FinalOutput
                    },
                    cancellationToken).ConfigureAwait(false);

                if (response.FinalOutput != null)
                {
                    return new AgentLoopResult
                    {
                        IsCompleted = true,
                        LastModelId = lastModelId,
                        FinalOutput = response.FinalOutput,
                        Iterations = iterations.ToArray()
                    };
                }
            }

            return new AgentLoopResult
            {
                IsCompleted = false,
                FailureReason = $"The agent loop reached the iteration limit ({request.MaxIterations}) before FinishTask closed the turn.",
                LastModelId = lastModelId,
                Iterations = iterations.ToArray()
            };
        }

        private async Task<StreamedResponseResult> InvokeModelStreamingAsync(
            AgentLoopRequest request,
            AgentLoopPreparedRequestDebugSnapshot preparedSnapshot,
            AgentLoopDebugRunContext? debugRunContext,
            int iterationNumber,
            AgentLoopFinalOutput? latestMessageOutput,
            Func<AgentLoopRuntimeEvent, CancellationToken, ValueTask>? onEventAsync,
            CancellationToken cancellationToken)
        {
            var candidates = _languageModelResolver.GetCandidateModels(request.Agent);
            if (candidates.Count == 0)
            {
                throw new InvalidOperationException(
                    $"Agent '{request.Agent.DisplayNameOrFallback}' has no resolved candidate language model.");
            }

            Exception? lastError = null;
            var failureMessages = new List<string>();
            var attemptNumber = 0;

            foreach (var candidate in candidates)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (!candidate.InterfaceSettings.IsFullyConfigured)
                {
                    failureMessages.Add($"{GetLanguageModelDisplayName(candidate)}: interface settings are incomplete.");
                    continue;
                }

                attemptNumber++;
                AgentLoopDebugRecorder.RecordPreparedRequest(
                    debugRunContext,
                    request,
                    preparedSnapshot,
                    candidates,
                    candidate,
                    iterationNumber,
                    attemptNumber);

                var modelId = candidate.SummaryModelId;
                var hasStartedStreaming = false;
                var rawContentBuilder = new StringBuilder();
                AgentAssistantResponse? parsedAssistantResponse = null;
                var toolInvocationStreamingParser = new SkyweaverToolInvocationStreamingParser(
                    _toolManager.GetRegisteredTools(resolveIcons: false)
                        .Select(registration => registration.Definition));
                IReadOnlyList<SkyweaverStreamingToolCallSnapshot> previousToolCallSnapshots =
                    Array.Empty<SkyweaverStreamingToolCallSnapshot>();
                var createMessageStreamingTracker = new CreateMessageStreamingTracker(
                    request.Agent.IsStructuredXmlIO
                        ? AgentLoopOutputKind.StructuredXml
                        : AgentLoopOutputKind.NaturalLanguage);
                var streamingUpdates = new List<AgentLoopStreamingUpdateDebugSnapshot>();

                try
                {
                    await PublishAsync(
                        onEventAsync,
                        new AgentLoopRuntimeEvent
                        {
                            Kind = AgentLoopRuntimeEventKind.IterationStarted,
                            IterationNumber = iterationNumber,
                            ModelId = modelId
                        },
                        cancellationToken).ConfigureAwait(false);

                    await foreach (var update in _chatService.GetStreamingResponseAsync(
                                       candidate,
                                       preparedSnapshot.PreparedMessages.Select(message => message.Clone()).ToArray(),
                                       cancellationToken).ConfigureAwait(false))
                    {
                        if (!string.IsNullOrWhiteSpace(update.ModelId))
                        {
                            modelId = update.ModelId;
                        }

                        var rawContentLengthBeforeAppend = rawContentBuilder.Length;
                        var wasAppendedToRawContent = !string.IsNullOrEmpty(update.TextDelta);
                        if (wasAppendedToRawContent)
                        {
                            hasStartedStreaming = true;
                            rawContentBuilder.Append(update.TextDelta);
                        }

                        streamingUpdates.Add(new AgentLoopStreamingUpdateDebugSnapshot
                        {
                            SequenceNumber = streamingUpdates.Count + 1,
                            ReceivedAtLocal = DateTimeOffset.Now,
                            Update = update,
                            WasAppendedToRawContent = wasAppendedToRawContent,
                            RawContentLengthBeforeAppend = rawContentLengthBeforeAppend,
                            RawContentLengthAfterAppend = rawContentBuilder.Length,
                            RawContentTailAfterAppend = GetStringBuilderTail(rawContentBuilder, StreamingTraceRawContentTailLength)
                        });

                        if (!wasAppendedToRawContent)
                        {
                            continue;
                        }

                        var currentRawContent = rawContentBuilder.ToString();
                        var currentToolCallSnapshots = toolInvocationStreamingParser.Parse(currentRawContent);
                        await PublishStreamingToolCallUpdatesAsync(
                            onEventAsync,
                            currentToolCallSnapshots,
                            previousToolCallSnapshots,
                            iterationNumber,
                            modelId,
                            cancellationToken).ConfigureAwait(false);
                        previousToolCallSnapshots = currentToolCallSnapshots;

                        var streamedDelta = createMessageStreamingTracker.ExtractDelta(currentRawContent);
                        if (!string.IsNullOrEmpty(streamedDelta))
                        {
                            await PublishAsync(
                                onEventAsync,
                                new AgentLoopRuntimeEvent
                                {
                                    Kind = AgentLoopRuntimeEventKind.TextDelta,
                                    IterationNumber = iterationNumber,
                                    ModelId = modelId,
                                    TextDelta = streamedDelta,
                                    TextDeltaOutputKind = createMessageStreamingTracker.OutputKind
                                },
                                cancellationToken).ConfigureAwait(false);
                        }
                    }

                    var rawContent = rawContentBuilder.ToString();
                    if (!TryParseAssistantToolTree(
                            rawContent,
                            out var normalizedToolsXml,
                            out var assistantResponse,
                            out var parseError))
                    {
                        await PublishAsync(
                            onEventAsync,
                            new AgentLoopRuntimeEvent
                            {
                                Kind = AgentLoopRuntimeEventKind.MalformedToolCall,
                                IterationNumber = iterationNumber,
                                ModelId = modelId,
                                ToolXml = rawContent,
                                ErrorMessage = parseError
                            },
                            cancellationToken).ConfigureAwait(false);

                        throw new InvalidAssistantResponseException(
                            parseError,
                            attemptNumber,
                            modelId,
                            CreateRejectedAssistantResponse(rawContent, parseError));
                    }

                    parsedAssistantResponse = assistantResponse;

                    await PublishAsync(
                        onEventAsync,
                        new AgentLoopRuntimeEvent
                        {
                            Kind = AgentLoopRuntimeEventKind.AssistantToolTreeReceived,
                            IterationNumber = iterationNumber,
                            ModelId = modelId,
                            ToolXml = normalizedToolsXml
                        },
                        cancellationToken).ConfigureAwait(false);

                    var executionResult = await ExecuteAssistantToolTreeAsync(
                        request,
                        assistantResponse.GetToolCalls(),
                        iterationNumber,
                        modelId,
                        latestMessageOutput,
                        onEventAsync,
                        cancellationToken).ConfigureAwait(false);

                    AgentLoopDebugRecorder.RecordStreamingTrace(
                        debugRunContext,
                        request.Agent,
                        candidate,
                        iterationNumber,
                        attemptNumber,
                        modelId,
                        streamingUpdates,
                        rawContent,
                        hasStartedStreaming,
                        completedNormally: true,
                        terminalMessage: null);

                    return new StreamedResponseResult(
                        attemptNumber,
                        modelId,
                        assistantResponse,
                        executionResult.ToolBackfills,
                        executionResult.FinalOutput,
                        executionResult.LatestMessageOutput);
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (InvalidAssistantResponseException ex)
                {
                    AgentLoopDebugRecorder.RecordStreamingTrace(
                        debugRunContext,
                        request.Agent,
                        candidate,
                        iterationNumber,
                        attemptNumber,
                        modelId,
                        streamingUpdates,
                        rawContentBuilder.ToString(),
                        hasStartedStreaming,
                        completedNormally: false,
                        terminalMessage: ex.Message);

                    AgentLoopDebugRecorder.RecordMainRequestFailure(
                        debugRunContext,
                        request.Agent,
                        candidate,
                        iterationNumber,
                        attemptNumber,
                        modelId,
                        rawContentBuilder.ToString(),
                        hasStartedStreaming,
                        ex);
                    throw;
                }
                catch (Exception ex)
                {
                    AgentLoopDebugRecorder.RecordStreamingTrace(
                        debugRunContext,
                        request.Agent,
                        candidate,
                        iterationNumber,
                        attemptNumber,
                        modelId,
                        streamingUpdates,
                        rawContentBuilder.ToString(),
                        hasStartedStreaming,
                        completedNormally: false,
                        terminalMessage: ex.Message);

                    AgentLoopDebugRecorder.RecordMainRequestFailure(
                        debugRunContext,
                        request.Agent,
                        candidate,
                        iterationNumber,
                        attemptNumber,
                        modelId,
                        rawContentBuilder.ToString(),
                        hasStartedStreaming,
                        ex);

                    if (hasStartedStreaming)
                    {
                        throw new InvalidAssistantResponseException(
                            $"Streaming response from model '{GetLanguageModelDisplayName(candidate)}' failed after partial output: {ex.Message}",
                            attemptNumber,
                            modelId,
                            parsedAssistantResponse ?? CreateRejectedAssistantResponse(rawContentBuilder.ToString(), ex.Message),
                            innerException: ex);
                    }

                    lastError = ex;
                    failureMessages.Add($"{GetLanguageModelDisplayName(candidate)}: {ex.Message}");
                }
            }

            var failureText = failureMessages.Count == 0
                ? "No callable candidate language model is available."
                : $"Tried candidates in order: {string.Join("; ", failureMessages)}";

            throw new InvalidOperationException(
                $"Agent '{request.Agent.DisplayNameOrFallback}' could not start a streaming response. {failureText}",
                lastError);
        }

        private static string GetStringBuilderTail(StringBuilder builder, int maxLength)
        {
            ArgumentNullException.ThrowIfNull(builder);

            if (builder.Length == 0 || maxLength <= 0)
            {
                return string.Empty;
            }

            var length = Math.Min(maxLength, builder.Length);
            return builder.ToString(builder.Length - length, length);
        }

        private async Task<(IReadOnlyList<AgentToolBackfill> ToolBackfills, AgentLoopFinalOutput? FinalOutput, AgentLoopFinalOutput? LatestMessageOutput)>
            ExecuteAssistantToolTreeAsync(
                AgentLoopRequest request,
                IReadOnlyList<SkyweaverToolInvocation> invocations,
                int iterationNumber,
                string? modelId,
                AgentLoopFinalOutput? latestMessageOutput,
                Func<AgentLoopRuntimeEvent, CancellationToken, ValueTask>? onEventAsync,
                CancellationToken cancellationToken)
        {
            var toolBackfills = new List<AgentToolBackfill>();
            var finishInvocations = new List<(int ToolCallIndex, SkyweaverToolInvocation Invocation)>();
            var currentLatestMessageOutput = latestMessageOutput;
            const int partIndex = 0;

            for (var invocationIndex = 0; invocationIndex < invocations.Count; invocationIndex++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var invocation = invocations[invocationIndex];
                var toolCallIndex = invocationIndex + 1;

                if (IsFinishTask(invocation.ToolName))
                {
                    finishInvocations.Add((toolCallIndex, invocation));
                    continue;
                }

                AgentToolBackfill backfill;
                AgentLoopFinalOutput? createdMessage = null;

                if (IsCreateMessage(invocation.ToolName))
                {
                    backfill = ExecuteCreateMessageInvocation(
                        request.Agent,
                        invocation,
                        partIndex,
                        toolCallIndex,
                        out createdMessage);
                }
                else
                {
                    var toolReturns = await ExecuteAuthorizedInvocationsAsync(
                        request,
                        [invocation],
                        iterationNumber,
                        partIndex,
                        cancellationToken).ConfigureAwait(false);
                    backfill = CreateBackfill(partIndex, toolCallIndex, toolReturns);
                }

                toolBackfills.Add(backfill);

                await PublishAsync(
                    onEventAsync,
                    new AgentLoopRuntimeEvent
                    {
                        Kind = AgentLoopRuntimeEventKind.ToolOutputReceived,
                        IterationNumber = iterationNumber,
                        ModelId = modelId,
                        PartIndex = partIndex,
                        ToolCallIndex = toolCallIndex,
                        ToolInvocation = invocation,
                        ToolOutputXml = backfill.ToolsReturnXml,
                        ToolReturns = backfill.ToolReturns
                    },
                    cancellationToken).ConfigureAwait(false);

                if (createdMessage != null)
                {
                    currentLatestMessageOutput = createdMessage;
                    await PublishAsync(
                        onEventAsync,
                        new AgentLoopRuntimeEvent
                        {
                            Kind = AgentLoopRuntimeEventKind.MessageCreated,
                            IterationNumber = iterationNumber,
                            ModelId = modelId,
                            PartIndex = partIndex,
                            ToolCallIndex = toolCallIndex,
                            ToolInvocation = invocation,
                            MessageOutput = createdMessage
                        },
                        cancellationToken).ConfigureAwait(false);
                }
            }

            if (finishInvocations.Count == 0)
            {
                return (toolBackfills, null, currentLatestMessageOutput);
            }

            if (finishInvocations.Count > 1)
            {
                foreach (var finishInvocation in finishInvocations)
                {
                    var errorBackfill = CreateBackfill(
                        partIndex,
                        finishInvocation.ToolCallIndex,
                        [_toolManager.CreateErrorToolReturnPayload(
                            FinishTaskTool.ToolName,
                            "FinishTask may only be called once in a single assistant response.")]);
                    toolBackfills.Add(errorBackfill);

                    await PublishAsync(
                        onEventAsync,
                        new AgentLoopRuntimeEvent
                        {
                            Kind = AgentLoopRuntimeEventKind.ToolOutputReceived,
                            IterationNumber = iterationNumber,
                            ModelId = modelId,
                            PartIndex = partIndex,
                            ToolCallIndex = finishInvocation.ToolCallIndex,
                            ToolInvocation = finishInvocation.Invocation,
                            ToolOutputXml = errorBackfill.ToolsReturnXml,
                            ToolReturns = errorBackfill.ToolReturns
                        },
                        cancellationToken).ConfigureAwait(false);
                }

                return (toolBackfills, null, currentLatestMessageOutput);
            }

            var finishInvocationCandidate = finishInvocations[0];
            if (!TryValidateFinishTaskInvocation(finishInvocationCandidate.Invocation, out var finishError))
            {
                var errorBackfill = CreateBackfill(
                    partIndex,
                    finishInvocationCandidate.ToolCallIndex,
                    [_toolManager.CreateErrorToolReturnPayload(
                        FinishTaskTool.ToolName,
                        finishError)]);
                toolBackfills.Add(errorBackfill);

                await PublishAsync(
                    onEventAsync,
                    new AgentLoopRuntimeEvent
                    {
                        Kind = AgentLoopRuntimeEventKind.ToolOutputReceived,
                        IterationNumber = iterationNumber,
                        ModelId = modelId,
                        PartIndex = partIndex,
                        ToolCallIndex = finishInvocationCandidate.ToolCallIndex,
                        ToolInvocation = finishInvocationCandidate.Invocation,
                        ToolOutputXml = errorBackfill.ToolsReturnXml,
                        ToolReturns = errorBackfill.ToolReturns
                    },
                    cancellationToken).ConfigureAwait(false);

                return (toolBackfills, null, currentLatestMessageOutput);
            }

            if (currentLatestMessageOutput == null)
            {
                var errorBackfill = CreateBackfill(
                    partIndex,
                    finishInvocationCandidate.ToolCallIndex,
                    [_toolManager.CreateErrorToolReturnPayload(
                        FinishTaskTool.ToolName,
                        $"FinishTask requires a successful {CreateMessageTool.ToolName} before the turn can close.")]);
                toolBackfills.Add(errorBackfill);

                await PublishAsync(
                    onEventAsync,
                    new AgentLoopRuntimeEvent
                    {
                        Kind = AgentLoopRuntimeEventKind.ToolOutputReceived,
                        IterationNumber = iterationNumber,
                        ModelId = modelId,
                        PartIndex = partIndex,
                        ToolCallIndex = finishInvocationCandidate.ToolCallIndex,
                        ToolInvocation = finishInvocationCandidate.Invocation,
                        ToolOutputXml = errorBackfill.ToolsReturnXml,
                        ToolReturns = errorBackfill.ToolReturns
                    },
                    cancellationToken).ConfigureAwait(false);

                return (toolBackfills, null, currentLatestMessageOutput);
            }

            var successBackfill = CreateBackfill(
                partIndex,
                finishInvocationCandidate.ToolCallIndex,
                [_toolManager.CreateToolReturnPayload(
                    FinishTaskTool.ToolName,
                    SkyweaverToolResult.Success("FinishTask accepted."))]);
            toolBackfills.Add(successBackfill);

            await PublishAsync(
                onEventAsync,
                new AgentLoopRuntimeEvent
                {
                    Kind = AgentLoopRuntimeEventKind.ToolOutputReceived,
                    IterationNumber = iterationNumber,
                    ModelId = modelId,
                    PartIndex = partIndex,
                    ToolCallIndex = finishInvocationCandidate.ToolCallIndex,
                    ToolInvocation = finishInvocationCandidate.Invocation,
                    ToolOutputXml = successBackfill.ToolsReturnXml,
                    ToolReturns = successBackfill.ToolReturns
                },
                cancellationToken).ConfigureAwait(false);

            await PublishAsync(
                onEventAsync,
                new AgentLoopRuntimeEvent
                {
                    Kind = AgentLoopRuntimeEventKind.FinalOutputProduced,
                    IterationNumber = iterationNumber,
                    ModelId = modelId,
                    PartIndex = partIndex,
                    ToolCallIndex = finishInvocationCandidate.ToolCallIndex,
                    FinalOutput = currentLatestMessageOutput
                },
                cancellationToken).ConfigureAwait(false);

            return (toolBackfills, currentLatestMessageOutput, currentLatestMessageOutput);
        }

        private static async Task PublishStreamingToolCallUpdatesAsync(
            Func<AgentLoopRuntimeEvent, CancellationToken, ValueTask>? onEventAsync,
            IReadOnlyList<SkyweaverStreamingToolCallSnapshot> currentSnapshots,
            IReadOnlyList<SkyweaverStreamingToolCallSnapshot> previousSnapshots,
            int iterationNumber,
            string? modelId,
            CancellationToken cancellationToken)
        {
            if (onEventAsync == null || currentSnapshots.Count == 0)
            {
                return;
            }

            var previousByToolCallIndex = previousSnapshots.ToDictionary(
                snapshot => snapshot.ToolCallIndex,
                snapshot => snapshot);

            foreach (var snapshot in currentSnapshots.OrderBy(item => item.ToolCallIndex))
            {
                var eventKind = previousByToolCallIndex.TryGetValue(snapshot.ToolCallIndex, out var previousSnapshot)
                    ? AreEquivalent(previousSnapshot, snapshot)
                        ? (AgentLoopRuntimeEventKind?)null
                        : AgentLoopRuntimeEventKind.ToolCallUpdated
                    : AgentLoopRuntimeEventKind.ToolCallStarted;

                if (eventKind == null)
                {
                    continue;
                }

                await PublishAsync(
                    onEventAsync,
                    new AgentLoopRuntimeEvent
                    {
                        Kind = eventKind.Value,
                        IterationNumber = iterationNumber,
                        ModelId = modelId,
                        PartIndex = 0,
                        ToolCallIndex = snapshot.ToolCallIndex,
                        ToolCallSnapshot = snapshot,
                        ToolXml = snapshot.ToolXmlFragment
                    },
                    cancellationToken).ConfigureAwait(false);
            }
        }

        private static bool AreEquivalent(
            SkyweaverStreamingToolCallSnapshot left,
            SkyweaverStreamingToolCallSnapshot right)
        {
            return left.ToolCallIndex == right.ToolCallIndex &&
                   string.Equals(left.ToolName, right.ToolName, StringComparison.OrdinalIgnoreCase) &&
                   left.IsInvocationClosed == right.IsInvocationClosed &&
                   string.Equals(left.ToolXmlFragment, right.ToolXmlFragment, StringComparison.Ordinal);
        }

        private AgentToolBackfill ExecuteCreateMessageInvocation(
            AgentDefinition agent,
            SkyweaverToolInvocation invocation,
            int partIndex,
            int toolCallIndex,
            out AgentLoopFinalOutput? messageOutput)
        {
            messageOutput = null;
            if (!TryBuildCreateMessageOutput(agent, invocation, out var createdOutput, out var errorMessage))
            {
                return CreateBackfill(
                    partIndex,
                    toolCallIndex,
                    [_toolManager.CreateErrorToolReturnPayload(
                        CreateMessageTool.ToolName,
                        errorMessage)]);
            }

            messageOutput = createdOutput;
            return CreateBackfill(
                partIndex,
                toolCallIndex,
                [_toolManager.CreateToolReturnPayload(
                    CreateMessageTool.ToolName,
                    SkyweaverToolResult.Success("CreateMessage accepted."))]);
        }

        private static async ValueTask PublishAsync(
            Func<AgentLoopRuntimeEvent, CancellationToken, ValueTask>? onEventAsync,
            AgentLoopRuntimeEvent update,
            CancellationToken cancellationToken)
        {
            if (onEventAsync == null)
            {
                return;
            }

            await onEventAsync(update, cancellationToken).ConfigureAwait(false);
        }

        private static void ValidateRequest(AgentLoopRequest request)
        {
            ArgumentNullException.ThrowIfNull(request.Agent);

            if (request.MaxIterations <= 0)
            {
                throw new InvalidOperationException("MaxIterations must be greater than zero.");
            }

            if (request.Agent.IsStructuredXmlIO &&
                !TryParseXmlWithRoot(
                    request.Input,
                    AgentDefinition.InputRootName,
                    out _,
                    out var inputError))
            {
                throw new InvalidOperationException(
                    $"Structured agent input must be a complete <{AgentDefinition.InputRootName}> XML document. {inputError}");
            }
        }

        private static bool TryParseAssistantToolTree(
            string rawContent,
            out string normalizedToolsXml,
            out AgentAssistantResponse assistantResponse,
            out string errorMessage)
        {
            normalizedToolsXml = string.Empty;
            assistantResponse = new AgentAssistantResponse(string.Empty, Array.Empty<AgentAssistantResponsePart>());
            errorMessage = string.Empty;

            var trimmed = rawContent?.Trim() ?? string.Empty;
            if (trimmed.Length == 0)
            {
                errorMessage = "Assistant response is empty. Every assistant response must be exactly one <Tools> XML tree.";
                return false;
            }

            XDocument document;
            try
            {
                document = XDocument.Parse(trimmed, LoadOptions.PreserveWhitespace);
            }
            catch (Exception ex) when (ex is XmlException or InvalidOperationException)
            {
                errorMessage = $"Assistant response must be exactly one <Tools> XML tree with no surrounding text. {ex.Message}";
                return false;
            }

            var root = document.Root;
            if (root == null)
            {
                errorMessage = "Assistant response is missing a root element. Use a single <Tools> root.";
                return false;
            }

            if (!string.Equals(root.Name.LocalName, "Tools", StringComparison.Ordinal))
            {
                errorMessage = $"Assistant response must use <Tools> as the root element, not <{root.Name.LocalName}>.";
                return false;
            }

            if (root.Nodes().Any(node =>
                    node is XElement element &&
                    !string.Equals(element.Name.LocalName, "Tool", StringComparison.Ordinal)))
            {
                errorMessage = "Only <Tool> elements are allowed directly under <Tools>.";
                return false;
            }

            if (root.Nodes().Any(node =>
                    node is XText text &&
                    !string.IsNullOrWhiteSpace(text.Value)))
            {
                errorMessage = "Non-whitespace text is not allowed directly under <Tools>.";
                return false;
            }

            var toolElements = root.Elements()
                .Where(element => string.Equals(element.Name.LocalName, "Tool", StringComparison.Ordinal))
                .ToArray();
            if (toolElements.Length == 0)
            {
                errorMessage = "Assistant response must contain at least one <Tool> inside <Tools>.";
                return false;
            }

            normalizedToolsXml = document.ToString(SaveOptions.DisableFormatting);

            IReadOnlyList<SkyweaverToolInvocation> invocations;
            try
            {
                invocations = new SkyweaverToolManager().ParseToolsInvocationXml(normalizedToolsXml);
            }
            catch (Exception ex)
            {
                errorMessage = $"Assistant response tools could not be parsed: {ex.Message}";
                return false;
            }

            if (invocations.Count == 0)
            {
                errorMessage = "Assistant response must contain at least one <Tool> inside <Tools>.";
                return false;
            }

            assistantResponse = new AgentAssistantResponse(
                normalizedToolsXml,
                [AgentAssistantResponsePart.CreateToolCall(
                    normalizedToolsXml,
                    invocations,
                    toolCallIndex: 1)]);
            return true;
        }

        private static bool IsCreateMessage(string toolName)
        {
            return string.Equals(toolName, CreateMessageTool.ToolName, StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsFinishTask(string toolName)
        {
            return string.Equals(toolName, FinishTaskTool.ToolName, StringComparison.OrdinalIgnoreCase);
        }

        private async Task<IReadOnlyList<SkyweaverToolReturnPayload>> ExecuteAuthorizedInvocationsAsync(
            AgentLoopRequest request,
            IReadOnlyList<SkyweaverToolInvocation> invocations,
            int iterationNumber,
            int partIndex,
            CancellationToken cancellationToken)
        {
            var toolReturns = new List<SkyweaverToolReturnPayload>(invocations.Count);

            foreach (var invocation in invocations)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var authorization = await AuthorizeToolInvocationAsync(
                    request,
                    invocation,
                    iterationNumber,
                    partIndex,
                    cancellationToken).ConfigureAwait(false);

                if (!authorization.CanExecute)
                {
                    toolReturns.Add(_toolManager.CreateErrorToolReturnPayload(
                        invocation.ToolName,
                        authorization.ErrorMessage ?? $"Tool '{invocation.ToolName}' cannot be executed."));
                    continue;
                }

                try
                {
                    var result = await _toolManager.ExecuteAsync(
                        invocation.ToolName,
                        invocation.RawArguments,
                        request.ToolContext,
                        request.Agent,
                        authorization.HasHostConfirmation,
                        cancellationToken).ConfigureAwait(false);
                    toolReturns.Add(_toolManager.CreateToolReturnPayload(invocation.ToolName, result));
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    toolReturns.Add(_toolManager.CreateErrorToolReturnPayload(
                        invocation.ToolName,
                        $"Tool execution failed: {ex.Message}"));
                }
            }

            return toolReturns;
        }

        private async Task<ToolExecutionAuthorization> AuthorizeToolInvocationAsync(
            AgentLoopRequest request,
            SkyweaverToolInvocation invocation,
            int iterationNumber,
            int partIndex,
            CancellationToken cancellationToken)
        {
            var registration = _toolManager.GetRegisteredTools(resolveIcons: false).FirstOrDefault(item =>
                string.Equals(item.Definition.Name, invocation.ToolName, StringComparison.OrdinalIgnoreCase));

            if (registration == null)
            {
                return new ToolExecutionAuthorization(
                    false,
                    false,
                    $"Tool not found: {invocation.ToolName}");
            }

            if (!registration.IsEnabled)
            {
                return new ToolExecutionAuthorization(
                    false,
                    false,
                    $"Tool '{invocation.ToolName}' is currently disabled.");
            }

            var permissionDecision = AgentToolPermissionEvaluator.Resolve(request.Agent, registration);
            if (permissionDecision == AgentToolEffectiveDecision.Allowed)
            {
                return new ToolExecutionAuthorization(true, false, null);
            }

            if (permissionDecision == AgentToolEffectiveDecision.Denied)
            {
                return new ToolExecutionAuthorization(
                    false,
                    false,
                    $"Tool '{invocation.ToolName}' is not available to agent '{request.Agent.AgentId}'.");
            }

            return await RequestToolConfirmationAsync(
                request,
                invocation,
                permissionDecision,
                iterationNumber,
                partIndex,
                cancellationToken).ConfigureAwait(false);
        }

        private async Task<ToolExecutionAuthorization> RequestToolConfirmationAsync(
            AgentLoopRequest request,
            SkyweaverToolInvocation invocation,
            AgentToolEffectiveDecision permissionDecision,
            int iterationNumber,
            int partIndex,
            CancellationToken cancellationToken)
        {
            if (request.ToolConfirmationCallback == null)
            {
                return new ToolExecutionAuthorization(
                    false,
                    false,
                    $"Tool '{invocation.ToolName}' requires host confirmation before execution, but the current runtime does not provide a confirmation callback.");
            }

            try
            {
                var confirmationResult = await request.ToolConfirmationCallback(
                    new AgentToolConfirmationRequest
                    {
                        Agent = request.Agent,
                        Invocation = invocation,
                        PermissionDecision = permissionDecision,
                        IterationNumber = iterationNumber,
                        PartIndex = partIndex
                    },
                    cancellationToken).ConfigureAwait(false);

                if (confirmationResult?.IsApproved == true)
                {
                    return new ToolExecutionAuthorization(true, true, null);
                }

                var rejectionMessage = confirmationResult == null || string.IsNullOrWhiteSpace(confirmationResult.RejectionMessage)
                    ? $"Tool '{invocation.ToolName}' was not approved by the host."
                    : confirmationResult.RejectionMessage.Trim();

                return new ToolExecutionAuthorization(false, false, rejectionMessage);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                return new ToolExecutionAuthorization(
                    false,
                    false,
                    $"Host confirmation failed for tool '{invocation.ToolName}': {ex.Message}");
            }
        }

        private static bool TryBuildCreateMessageOutput(
            AgentDefinition agent,
            SkyweaverToolInvocation invocation,
            out AgentLoopFinalOutput output,
            out string errorMessage)
        {
            output = null!;
            errorMessage = string.Empty;

            if (!TryExtractInvocationPayload(invocation, requirePayload: true, out var payload, out errorMessage))
            {
                return false;
            }

            if (agent.IsStructuredXmlIO)
            {
                if (!TryParseXmlWithRoot(
                        payload,
                        AgentDefinition.OutputRootName,
                        out var normalizedXml,
                        out var xmlError))
                {
                    errorMessage = $"{CreateMessageTool.ToolName} for this agent must contain one complete <{AgentDefinition.OutputRootName}> XML document. {xmlError}";
                    return false;
                }

                output = new AgentLoopFinalOutput
                {
                    Content = normalizedXml,
                    Kind = AgentLoopOutputKind.StructuredXml,
                    Source = AgentLoopFinalOutputSource.AssistantText
                };
                return true;
            }

            if (TryParseXmlWithRoot(
                    payload,
                    AgentDefinition.OutputRootName,
                    out _,
                    out _))
            {
                errorMessage = $"{CreateMessageTool.ToolName} for this agent must contain plain text, not a standalone <{AgentDefinition.OutputRootName}> XML block.";
                return false;
            }

            output = new AgentLoopFinalOutput
            {
                Content = payload,
                Kind = AgentLoopOutputKind.NaturalLanguage,
                Source = AgentLoopFinalOutputSource.AssistantText
            };
            return true;
        }

        private static bool TryValidateFinishTaskInvocation(
            SkyweaverToolInvocation invocation,
            out string errorMessage)
        {
            errorMessage = string.Empty;

            if (!TryExtractInvocationPayload(invocation, requirePayload: false, out var payload, out errorMessage))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(payload))
            {
                return true;
            }

            if (TryParseBooleanValue(payload, out var booleanValue))
            {
                if (booleanValue)
                {
                    return true;
                }

                errorMessage = "FinishTask cannot be false. Omit it until the turn is ready to close.";
                return false;
            }

            if (invocation.RawArguments.Count == 1 &&
                invocation.RawArguments.Values.FirstOrDefault() is string legacyValue &&
                TryParseBooleanValue(legacyValue, out var legacyBooleanValue))
            {
                if (legacyBooleanValue)
                {
                    return true;
                }

                errorMessage = "FinishTask cannot be false. Omit it until the turn is ready to close.";
                return false;
            }

            errorMessage = $"FinishTask does not accept a reply payload. Use {CreateMessageTool.ToolName} for the reply, then call FinishTask with an empty body.";
            return false;
        }

        private static bool TryExtractInvocationPayload(
            SkyweaverToolInvocation invocation,
            bool requirePayload,
            out string payload,
            out string errorMessage)
        {
            payload = string.Empty;
            errorMessage = string.Empty;

            XElement toolElement;
            try
            {
                toolElement = XElement.Parse(invocation.InvocationXml, LoadOptions.PreserveWhitespace);
            }
            catch (XmlException ex)
            {
                errorMessage = $"Tool invocation XML could not be parsed: {ex.Message}";
                return false;
            }

            payload = toolElement.HasElements
                ? string.Concat(toolElement.Nodes().Select(node => node.ToString(SaveOptions.DisableFormatting))).Trim()
                : (toolElement.Value?.Trim() ?? string.Empty);

            if (payload.Length == 0 && invocation.RawArguments.Count == 1)
            {
                payload = invocation.RawArguments.Values.FirstOrDefault()?.Trim() ?? string.Empty;
            }

            if (requirePayload && payload.Length == 0)
            {
                errorMessage = $"Tool '{invocation.ToolName}' requires a payload inside the <Tool> element.";
                return false;
            }

            return true;
        }

        private AgentToolBackfill CreateBackfill(
            int partIndex,
            int toolCallIndex,
            IReadOnlyList<SkyweaverToolReturnPayload> toolReturns)
        {
            return new AgentToolBackfill
            {
                PartIndex = partIndex,
                ToolCallIndex = toolCallIndex,
                ToolReturns = toolReturns,
                ToolsReturnXml = _toolManager.BuildToolsReturnXml(toolReturns)
            };
        }

        private string BuildRepairMessage(string errorMessage, int consecutiveFailures)
        {
            var normalizedError = string.IsNullOrWhiteSpace(errorMessage)
                ? "The previous assistant response could not be accepted."
                : errorMessage.Trim();

            return
                $"The previous assistant response was rejected and the turn is still open. Reason: {normalizedError} " +
                $"Ignore any partial text from that rejected response. This is consecutive failed iteration {consecutiveFailures} of {MaxConsecutiveRecoverableFailures}. " +
                $"Reply again with exactly one complete <Tools> XML tree. Put user-visible text only inside {CreateMessageTool.ToolName}, and call {FinishTaskTool.ToolName} only after a successful {CreateMessageTool.ToolName}.";
        }

        private void AppendFailedIterationHistory(
            InvalidAssistantResponseException failure,
            string repairMessage,
            ICollection<LanguageModelChatMessage> turnHistory)
        {
            ArgumentNullException.ThrowIfNull(failure);
            ArgumentNullException.ThrowIfNull(turnHistory);

            if (failure.ToolBackfills.Count > 0)
            {
                AppendCurrentTurnHistory(failure.AssistantResponse, failure.ToolBackfills, turnHistory);
            }
            else if (!string.IsNullOrWhiteSpace(failure.AssistantResponse.RawContent))
            {
                turnHistory.Add(new LanguageModelChatMessage(
                    LanguageModelChatRole.Assistant,
                    failure.AssistantResponse.RawContent));
            }

            turnHistory.Add(new LanguageModelChatMessage(
                LanguageModelChatRole.User,
                CreateRepairBackfillXml(repairMessage))
            {
                AuthorName = RequestRepairToolName
            });
        }

        private string CreateRepairBackfillXml(string repairMessage)
        {
            return _toolManager.BuildToolsReturnXml(
            [
                _toolManager.CreateErrorToolReturnPayload(RequestRepairToolName, repairMessage)
            ]);
        }

        private static void AppendCurrentTurnHistory(
            AgentAssistantResponse assistantResponse,
            IReadOnlyList<AgentToolBackfill> toolBackfills,
            ICollection<LanguageModelChatMessage> turnHistory)
        {
            ArgumentNullException.ThrowIfNull(assistantResponse);
            ArgumentNullException.ThrowIfNull(toolBackfills);
            ArgumentNullException.ThrowIfNull(turnHistory);

            var orderedBackfills = toolBackfills
                .OrderBy(item => item.PartIndex)
                .ThenBy(item => item.ToolCallIndex)
                .ToArray();

            var backfillCursor = 0;
            for (var partIndex = 0; partIndex < assistantResponse.Parts.Count; partIndex++)
            {
                var part = assistantResponse.Parts[partIndex];
                if (!part.HasParseError && !string.IsNullOrWhiteSpace(part.Content))
                {
                    turnHistory.Add(new LanguageModelChatMessage(
                        LanguageModelChatRole.Assistant,
                        part.Content));
                }

                while (backfillCursor < orderedBackfills.Length &&
                       orderedBackfills[backfillCursor].PartIndex == partIndex)
                {
                    turnHistory.Add(CreateToolResultMessage(orderedBackfills[backfillCursor]));
                    backfillCursor++;
                }
            }

            while (backfillCursor < orderedBackfills.Length)
            {
                turnHistory.Add(CreateToolResultMessage(orderedBackfills[backfillCursor]));
                backfillCursor++;
            }
        }

        private static LanguageModelChatMessage CreateToolResultMessage(AgentToolBackfill backfill)
        {
            var authorName = backfill.ToolReturns.Count == 1
                ? backfill.ToolReturns[0].ToolName
                : "SkyweaverTools";

            return new LanguageModelChatMessage(LanguageModelChatRole.User, backfill.ToolsReturnXml)
            {
                AuthorName = authorName
            };
        }

        private static AgentAssistantResponse CreateRejectedAssistantResponse(
            string rawContent,
            string errorMessage)
        {
            var normalizedRawContent = rawContent ?? string.Empty;
            if (normalizedRawContent.Length == 0)
            {
                return new AgentAssistantResponse(string.Empty, Array.Empty<AgentAssistantResponsePart>());
            }

            return new AgentAssistantResponse(
                normalizedRawContent,
                [
                    AgentAssistantResponsePart.CreateToolCall(
                        normalizedRawContent,
                        Array.Empty<SkyweaverToolInvocation>(),
                        parseError: string.IsNullOrWhiteSpace(errorMessage)
                            ? "The assistant response was rejected."
                            : errorMessage.Trim(),
                        toolCallIndex: 1)
                ]);
        }

        private static bool TryParseBooleanValue(string rawValue, out bool value)
        {
            switch ((rawValue ?? string.Empty).Trim().ToLowerInvariant())
            {
                case "true":
                case "1":
                case "yes":
                case "y":
                case "on":
                    value = true;
                    return true;

                case "false":
                case "0":
                case "no":
                case "n":
                case "off":
                    value = false;
                    return true;

                default:
                    value = false;
                    return false;
            }
        }

        private static bool TryParseXmlWithRoot(
            string? payload,
            string expectedRootName,
            out string normalizedXml,
            out string errorMessage)
        {
            normalizedXml = string.Empty;
            errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(payload))
            {
                errorMessage = "The payload is empty.";
                return false;
            }

            XDocument document;
            try
            {
                document = XDocument.Parse(payload, LoadOptions.PreserveWhitespace);
            }
            catch (Exception ex) when (ex is XmlException or InvalidOperationException)
            {
                errorMessage = $"The payload could not be parsed as XML: {ex.Message}";
                return false;
            }

            var root = document.Root;
            if (root == null)
            {
                errorMessage = "The payload is missing a root element.";
                return false;
            }

            if (!string.Equals(root.Name.LocalName, expectedRootName, StringComparison.Ordinal))
            {
                errorMessage = $"The root element must be <{expectedRootName}> instead of <{root.Name.LocalName}>.";
                return false;
            }

            normalizedXml = document.ToString();
            return true;
        }

        private static string GetLanguageModelDisplayName(LanguageModelDefinition model)
        {
            ArgumentNullException.ThrowIfNull(model);

            var displayName = model.DisplayName?.Trim() ?? string.Empty;
            if (displayName.Length > 0)
            {
                return displayName;
            }

            var summaryModelId = model.SummaryModelId?.Trim() ?? string.Empty;
            if (summaryModelId.Length > 0)
            {
                return summaryModelId;
            }

            return string.IsNullOrWhiteSpace(model.Key) ? "unnamed-model" : model.Key.Trim();
        }
    }
}
