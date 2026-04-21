using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Skyweaver.Commands;
using Skyweaver.Controls.ChatSessionControl.Models;
using Skyweaver.Controls.ChatSessionControl.Services;
using Skyweaver.Controls.WorkflowEditorControl.Models;
using Skyweaver.Infrastructure.Mvvm;
using Skyweaver.Models.ChatSession;
using Skyweaver.Services.AgentLoop;
using Skyweaver.Services.ChatSession;

namespace Skyweaver.Controls.ChatSessionControl.ViewModels
{
    public sealed class ChatSessionControlViewModel : ObservableObject
    {
        private const string UserAvatarPath = "pack://application:,,,/Resources/image.png";
        private const string AssistantAvatarPath = "pack://application:,,,/Resources/GuideBot.png";
        private const string SystemAvatarPath = "pack://application:,,,/Resources/QuestionBot.png";

        private readonly ChatSessionModel? _sessionModel;
        private readonly ChatSessionRepository? _chatSessionRepository;
        private readonly ChatSessionFlowBindingService _flowBindingService;
        private readonly ChatSessionRuntimeService _runtimeService;
        private readonly ToolInvocationPresentationService _toolInvocationPresentationService;
        private readonly string _sessionFlowValidationSummary;
        private readonly Dictionary<string, ChatSessionMessageBuilder> _activeAgentMessageBuilders =
            new(StringComparer.OrdinalIgnoreCase);

        private string _draftMessageText = string.Empty;
        private ChatMessageModel? _selectedMessage;
        private bool _isExecutionActive;
        private string _executionStatusText = "就绪";
        private bool _suppressPersistence;
        private CancellationTokenSource? _executionCancellationSource;

        public ObservableCollection<ChatMessageModel> Messages { get; } = new();

        public string SessionTitle { get; }

        public string? SessionSubtitle { get; }

        public bool HasBoundSessionFlow => _sessionModel?.HasBoundFlow == true;

        public string BoundSessionFlowName => _sessionModel?.BoundFlowDisplayName ?? "未绑定会话流";

        public string BoundSessionFlowSummary => HasBoundSessionFlow
            ? $"当前会话流：{BoundSessionFlowName}"
            : "当前会话尚未绑定会话流。";

        public string SessionFlowValidationSummary => _sessionFlowValidationSummary;

        public string DraftMessageText
        {
            get => _draftMessageText;
            set
            {
                if (SetProperty(ref _draftMessageText, value ?? string.Empty))
                {
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public ChatMessageModel? SelectedMessage
        {
            get => _selectedMessage;
            set
            {
                if (SetProperty(ref _selectedMessage, value))
                {
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public bool IsExecutionActive
        {
            get => _isExecutionActive;
            private set
            {
                if (SetProperty(ref _isExecutionActive, value))
                {
                    OnPropertyChanged(nameof(CanSendMessage));
                    OnPropertyChanged(nameof(CanCancelExecution));
                    OnPropertyChanged(nameof(ComposerHintText));
                    OnPropertyChanged(nameof(ComposerPrimaryButtonText));
                    OnPropertyChanged(nameof(ComposerPrimaryCommand));
                    OnPropertyChanged(nameof(IsComposerPrimaryButtonLatched));
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public bool CanSendMessage => !IsExecutionActive && !string.IsNullOrWhiteSpace(DraftMessageText);

        public bool CanCancelExecution => IsExecutionActive;

        public bool IsEmpty => Messages.Count == 0;

        public string ExecutionStatusText
        {
            get => _executionStatusText;
            private set => SetProperty(ref _executionStatusText, value ?? string.Empty);
        }

        public string ComposerHintText => IsExecutionActive
            ? "当前回合正在执行。你可以继续整理下一条输入；按 Ctrl+Enter 或 Shift+Enter 换行，点击“中止”结束本轮。"
            : "按 Enter 发送，Ctrl+Enter 或 Shift+Enter 换行。发送后会编译并执行绑定的会话流，直到命中“返回”节点。";

        public string ComposerPrimaryButtonText => IsExecutionActive ? "中止" : "发送";

        public ICommand ComposerPrimaryCommand => IsExecutionActive ? CancelExecutionCommand : SendMessageCommand;

        public bool IsComposerPrimaryButtonLatched => IsExecutionActive;

        public ICommand SendMessageCommand { get; }

        public ICommand CancelExecutionCommand { get; }

        public ICommand RemoveSelectedMessageCommand { get; }

        public ICommand RemoveMessageCommand { get; }

        public ICommand ClearMessagesCommand { get; }

        public ChatSessionControlViewModel(
            string sessionTitle,
            string? sessionSubtitle = null,
            ChatSessionModel? sessionModel = null,
            ChatSessionRepository? chatSessionRepository = null,
            ChatSessionRuntimeService? runtimeService = null)
        {
            SessionTitle = string.IsNullOrWhiteSpace(sessionTitle) ? "聊天会话" : sessionTitle;
            SessionSubtitle = string.IsNullOrWhiteSpace(sessionSubtitle) ? null : sessionSubtitle;
            _sessionModel = sessionModel;
            _chatSessionRepository = chatSessionRepository;
            _runtimeService = runtimeService ?? new ChatSessionRuntimeService();
            _toolInvocationPresentationService = new ToolInvocationPresentationService();
            _flowBindingService = new ChatSessionFlowBindingService();
            _sessionFlowValidationSummary = BuildSessionFlowValidationSummary(sessionModel);

            SendMessageCommand = new AsyncRelayCommand(SendMessageAsync, () => CanSendMessage);
            CancelExecutionCommand = new RelayCommand(CancelExecution, () => CanCancelExecution);
            RemoveSelectedMessageCommand = new RelayCommand(RemoveSelectedMessage, () => SelectedMessage != null && !IsExecutionActive);
            RemoveMessageCommand = new RelayCommand<ChatMessageModel>(RemoveMessage, message => message != null && !IsExecutionActive);
            ClearMessagesCommand = new RelayCommand(ClearMessages, () => Messages.Count > 0 && !IsExecutionActive);

            Messages.CollectionChanged += (_, _) =>
            {
                OnPropertyChanged(nameof(IsEmpty));
                CommandManager.InvalidateRequerySuggested();
            };

            Messages.CollectionChanged += OnMessagesCollectionChanged;

            if (_sessionModel != null)
            {
                LoadPersistedMessages(_sessionModel);
            }
            else
            {
                SeedMessages();
            }
        }

        private async Task SendMessageAsync()
        {
            var trimmedText = DraftMessageText.Trim();
            if (trimmedText.Length == 0)
            {
                return;
            }

            if (_sessionModel == null)
            {
                AddSystemStatusMessage("当前聊天页没有绑定 ChatSessionModel，无法执行会话流。", "运行时不可用");
                return;
            }

            if (_chatSessionRepository == null)
            {
                AddSystemStatusMessage("当前聊天页没有绑定 ChatSessionRepository，无法持久化会话执行结果。", "运行时不可用");
                return;
            }

            var userMessage = CreateMessage(
                ChatMessageRole.User,
                ChatMessagePartModel.CreateText(trimmedText));
            Messages.Add(userMessage);
            SelectedMessage = userMessage;
            DraftMessageText = string.Empty;
            PersistSession();

            _activeAgentMessageBuilders.Clear();
            _executionCancellationSource?.Dispose();
            _executionCancellationSource = new CancellationTokenSource();
            IsExecutionActive = true;
            ExecutionStatusText = $"正在执行：{BoundSessionFlowName}";

            ChatSessionRuntimeResult? runtimeResult = null;

            try
            {
                runtimeResult = await _runtimeService.ExecuteTurnAsync(
                    new ChatSessionRuntimeRequest
                    {
                        Session = _sessionModel,
                        UserText = trimmedText,
                        ToolConfirmationCallback = ConfirmToolInvocationAsync
                    },
                    HandleRuntimeEventAsync,
                    _executionCancellationSource.Token).ConfigureAwait(true);
            }
            catch (Exception ex)
            {
                runtimeResult = new ChatSessionRuntimeResult
                {
                    FailureReason = ex.Message
                };

                AddSystemStatusMessage(ex.Message, "执行失败");
            }
            finally
            {
                var incompleteToolMessage = runtimeResult switch
                {
                    { IsCompleted: true } => null,
                    { IsCancelled: true } => "执行已取消。",
                    _ => "执行未完成，未闭合的工具调用已终止。"
                };

                FinalizeActiveAgentMessages(incompleteToolMessage);
                _executionCancellationSource?.Dispose();
                _executionCancellationSource = null;
                IsExecutionActive = false;

                ExecutionStatusText = runtimeResult switch
                {
                    { IsCompleted: true } => "就绪",
                    { IsCancelled: true } => "已取消",
                    _ => "执行失败"
                };

                PersistSession();
            }
        }

        private void CancelExecution()
        {
            if (!IsExecutionActive)
            {
                return;
            }

            _runtimeService.CancelActiveExecution(_sessionModel?.SessionId);
            _executionCancellationSource?.Cancel();
            ExecutionStatusText = "正在取消当前执行…";
        }

        private ValueTask HandleRuntimeEventAsync(
            ChatSessionRuntimeEvent runtimeEvent,
            CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(runtimeEvent);

            var dispatcher = Application.Current?.Dispatcher;
            if (dispatcher == null || dispatcher.CheckAccess())
            {
                ApplyRuntimeEvent(runtimeEvent);
                return ValueTask.CompletedTask;
            }

            return new ValueTask(dispatcher.InvokeAsync(() => ApplyRuntimeEvent(runtimeEvent)).Task);
        }

        private void ApplyRuntimeEvent(ChatSessionRuntimeEvent runtimeEvent)
        {
            switch (runtimeEvent.Kind)
            {
                case ChatSessionRuntimeEventKind.ExecutionStarted:
                    ExecutionStatusText = $"正在执行：{runtimeEvent.FlowName}";
                    break;

                case ChatSessionRuntimeEventKind.NodeStarted:
                    ExecutionStatusText = runtimeEvent.IsSkipped
                        ? $"节点已跳过：{runtimeEvent.NodeTitle}"
                        : $"正在执行节点：{runtimeEvent.NodeTitle}";
                    break;

                case ChatSessionRuntimeEventKind.NodeCompleted:
                    ExecutionStatusText = runtimeEvent.IsSkipped
                        ? $"节点已跳过：{runtimeEvent.NodeTitle}"
                        : $"节点完成：{runtimeEvent.NodeTitle}";
                    ApplyNodeCompletion(runtimeEvent);
                    break;

                case ChatSessionRuntimeEventKind.AgentIterationStarted:
                    ExecutionStatusText = runtimeEvent.IterationNumber is int startedIteration
                        ? $"代理 {runtimeEvent.NodeTitle} 迭代 {startedIteration}"
                        : $"代理 {runtimeEvent.NodeTitle} 开始迭代";
                    break;

                case ChatSessionRuntimeEventKind.AgentIterationCompleted:
                    ExecutionStatusText = runtimeEvent.IterationNumber is int completedIteration
                        ? $"代理 {runtimeEvent.NodeTitle} 完成迭代 {completedIteration}"
                        : $"代理 {runtimeEvent.NodeTitle} 完成迭代";
                    break;

                case ChatSessionRuntimeEventKind.TextDelta:
                    if (!runtimeEvent.IsHiddenAgent)
                    {
                        var builder = GetOrCreateAgentMessageBuilder(runtimeEvent);
                        switch (runtimeEvent.TextDeltaOutputKind)
                        {
                            case AgentLoopOutputKind.StructuredXml:
                                builder.AppendReplyStructuredXmlDelta(runtimeEvent.TextDelta);
                                break;
                            case AgentLoopOutputKind.NaturalLanguage:
                                builder.AppendReplyTextDelta(runtimeEvent.TextDelta);
                                break;
                            default:
                                builder.AppendTextDelta(runtimeEvent.TextDelta);
                                break;
                        }
                    }

                    break;

                case ChatSessionRuntimeEventKind.AssistantToolTreeReceived:
                    break;

                case ChatSessionRuntimeEventKind.ToolCallStarted:
                    if (!runtimeEvent.IsHiddenAgent &&
                        !ChatSessionFinishTaskVisibility.IsInternalToolRuntimeEvent(runtimeEvent) &&
                        runtimeEvent.ToolCallSnapshot != null &&
                        runtimeEvent.ToolCallIndex is int toolCallIndex)
                    {
                        var toolCallKey = CreateToolCallKey(runtimeEvent, toolCallIndex);
                        GetOrCreateAgentMessageBuilder(runtimeEvent)
                            .AddOrUpdateToolCall(toolCallKey, runtimeEvent.ToolCallSnapshot);
                    }

                    break;

                case ChatSessionRuntimeEventKind.ToolCallUpdated:
                    if (!runtimeEvent.IsHiddenAgent &&
                        !ChatSessionFinishTaskVisibility.IsInternalToolRuntimeEvent(runtimeEvent) &&
                        runtimeEvent.ToolCallSnapshot != null &&
                        runtimeEvent.ToolCallIndex is int updatedToolCallIndex)
                    {
                        var updatedToolCallKey = CreateToolCallKey(runtimeEvent, updatedToolCallIndex);
                        GetOrCreateAgentMessageBuilder(runtimeEvent)
                            .AddOrUpdateToolCall(updatedToolCallKey, runtimeEvent.ToolCallSnapshot);
                    }

                    break;

                case ChatSessionRuntimeEventKind.MalformedToolCall:
                    if (!runtimeEvent.IsHiddenAgent &&
                        !ChatSessionFinishTaskVisibility.IsInternalToolRuntimeEvent(runtimeEvent) &&
                        runtimeEvent.ToolCallIndex is int malformedToolCallIndex)
                    {
                        var malformedToolCallKey = CreateToolCallKey(runtimeEvent, malformedToolCallIndex);
                        GetOrCreateAgentMessageBuilder(runtimeEvent)
                            .AddMalformedToolCall(
                                malformedToolCallKey,
                                runtimeEvent.ToolXml ?? string.Empty,
                                runtimeEvent.Message);
                    }

                    break;

                case ChatSessionRuntimeEventKind.ToolOutputReceived:
                    if (!runtimeEvent.IsHiddenAgent &&
                        !ChatSessionFinishTaskVisibility.IsInternalToolRuntimeEvent(runtimeEvent) &&
                        runtimeEvent.ToolCallIndex is int completedToolCallIndex)
                    {
                        var completedToolCallKey = CreateToolCallKey(runtimeEvent, completedToolCallIndex);
                        GetOrCreateAgentMessageBuilder(runtimeEvent)
                            .CompleteToolCall(
                                completedToolCallKey,
                                runtimeEvent.ToolOutputXml ?? runtimeEvent.Message ?? string.Empty);
                    }

                    break;

                case ChatSessionRuntimeEventKind.AssistantMessageCreated:
                    ApplyAssistantMessageCreated(runtimeEvent);
                    break;

                case ChatSessionRuntimeEventKind.ContextCompressionApplied:
                    AddSystemStatusMessage(BuildCompressionMessage(runtimeEvent), "上下文压缩");
                    break;

                case ChatSessionRuntimeEventKind.RepairMessageGenerated:
                    AddSystemStatusMessage(runtimeEvent.Message ?? "代理需要继续调用 FinishTask 才能结束当前任务。", "修复提示");
                    break;

                case ChatSessionRuntimeEventKind.AgentFinalOutputProduced:
                    ApplyAgentFinalOutput(runtimeEvent);
                    break;

                case ChatSessionRuntimeEventKind.StructuredOutputProduced:
                    break;

                case ChatSessionRuntimeEventKind.ExecutionCompleted:
                    ExecutionStatusText = "执行完成";
                    AppendAssistantReturnMessage(runtimeEvent.Payload, runtimeEvent.IsPayloadAlreadyPresented);
                    break;

                case ChatSessionRuntimeEventKind.ExecutionFailed:
                    ExecutionStatusText = "执行失败";
                    AddSystemStatusMessage(runtimeEvent.Message ?? "执行失败。", "执行失败");
                    break;

                case ChatSessionRuntimeEventKind.ExecutionCancelled:
                    ExecutionStatusText = "执行已取消";
                    AddSystemStatusMessage(runtimeEvent.Message ?? "当前执行已取消。", "执行已取消");
                    break;
            }
        }

        private void ApplyNodeCompletion(ChatSessionRuntimeEvent runtimeEvent)
        {
            if (runtimeEvent.IsHiddenAgent || runtimeEvent.IsSkipped)
            {
                return;
            }

            if (runtimeEvent.NodeKind != SessionFlowNodeKind.Agent)
            {
                return;
            }

            var builder = GetOrCreateAgentMessageBuilder(runtimeEvent);
            builder.CompleteTextStreaming();
        }

        private void ApplyAgentFinalOutput(ChatSessionRuntimeEvent runtimeEvent)
        {
            if (runtimeEvent.IsHiddenAgent || runtimeEvent.Payload == null)
            {
                return;
            }

            var builder = GetOrCreateAgentMessageBuilder(runtimeEvent);
            builder.CompleteTextStreaming();
            if (runtimeEvent.IsPayloadFromFinishTask)
            {
                ApplyAssistantPayload(builder, runtimeEvent.Payload);
            }
        }

        private void ApplyAssistantMessageCreated(ChatSessionRuntimeEvent runtimeEvent)
        {
            if (runtimeEvent.IsHiddenAgent || runtimeEvent.Payload == null)
            {
                return;
            }

            var builder = GetOrCreateAgentMessageBuilder(runtimeEvent);
            builder.CompleteTextStreaming();
            ApplyAssistantPayload(builder, runtimeEvent.Payload);
        }

        private static ToolCallInstanceKey CreateToolCallKey(ChatSessionRuntimeEvent runtimeEvent, int toolCallIndex)
        {
            ArgumentNullException.ThrowIfNull(runtimeEvent);

            return ToolCallInstanceKey.Create(
                runtimeEvent.IterationNumber,
                runtimeEvent.PartIndex,
                toolCallIndex);
        }

        private ChatSessionMessageBuilder GetOrCreateAgentMessageBuilder(ChatSessionRuntimeEvent runtimeEvent)
        {
            var nodeId = runtimeEvent.NodeId ?? Guid.NewGuid().ToString("N");
            if (_activeAgentMessageBuilders.TryGetValue(nodeId, out var builder))
            {
                return builder;
            }

            var message = new ChatMessageModel(
                ChatMessageRole.Assistant,
                string.IsNullOrWhiteSpace(runtimeEvent.NodeTitle)
                    ? GetDisplayName(ChatMessageRole.Assistant)
                    : runtimeEvent.NodeTitle,
                GetAvatarPath(ChatMessageRole.Assistant),
                DateTime.Now);

            Messages.Add(message);
            SelectedMessage = message;

            builder = new ChatSessionMessageBuilder(message, _toolInvocationPresentationService);
            _activeAgentMessageBuilders[nodeId] = builder;
            return builder;
        }

        private void FinalizeActiveAgentMessages(string? incompleteToolMessage)
        {
            foreach (var builder in _activeAgentMessageBuilders.Values)
            {
                builder.CompleteTextStreaming();
                builder.CompleteReplyStreaming();
                if (!string.IsNullOrWhiteSpace(incompleteToolMessage))
                {
                    builder.FinalizeOpenToolCalls(incompleteToolMessage);
                }
            }

            _activeAgentMessageBuilders.Clear();
        }

        private static void ApplyAssistantPayload(
            ChatSessionMessageBuilder builder,
            SessionFlowPayload payload)
        {
            ArgumentNullException.ThrowIfNull(builder);
            ArgumentNullException.ThrowIfNull(payload);

            if (payload.IsStructuredXml)
            {
                builder.SetReplyStructuredXml(payload.Content);
                return;
            }

            builder.SetReplyText(payload.Content);
        }

        private Task<AgentToolConfirmationResult> ConfirmToolInvocationAsync(
            AgentToolConfirmationRequest request,
            CancellationToken cancellationToken)
        {
            var dispatcher = Application.Current?.Dispatcher;
            if (dispatcher == null || dispatcher.CheckAccess())
            {
                return Task.FromResult(ShowToolConfirmationDialog(request));
            }

            return dispatcher.InvokeAsync(() => ShowToolConfirmationDialog(request)).Task;
        }

        private static AgentToolConfirmationResult ShowToolConfirmationDialog(AgentToolConfirmationRequest request)
        {
            var builder = new StringBuilder();
            builder.AppendLine($"代理：{request.Agent.DisplayNameOrFallback}");
            builder.AppendLine($"工具：{request.Invocation.ToolName}");
            builder.AppendLine($"迭代：{request.IterationNumber}");
            builder.AppendLine();
            builder.AppendLine("调用内容：");
            builder.AppendLine(request.Invocation.InvocationXml);
            builder.AppendLine();
            builder.Append("是否允许继续执行该工具调用？");

            var result = MessageBox.Show(
                Application.Current?.MainWindow,
                builder.ToString(),
                "工具调用确认",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            return result == MessageBoxResult.Yes
                ? AgentToolConfirmationResult.Approve()
                : AgentToolConfirmationResult.Reject("用户拒绝了该工具调用。");
        }

        private void AppendAssistantReturnMessage(
            SessionFlowPayload? payload,
            bool isPayloadAlreadyPresented)
        {
            if (payload == null || isPayloadAlreadyPresented)
            {
                return;
            }

            var message = payload.IsStructuredXml
                ? CreateMessage(
                    ChatMessageRole.Assistant,
                    ChatMessagePartModel.CreateStructuredXml(payload.Content, "结构化 XML"))
                : CreateMessage(
                    ChatMessageRole.Assistant,
                    ChatMessagePartModel.CreateText(payload.Content));

            Messages.Add(message);
            SelectedMessage = message;
        }

        private void AddSystemStatusMessage(string content, string title)
        {
            var message = CreateMessage(
                ChatMessageRole.System,
                ChatMessagePartModel.CreateStatus(content, title));
            Messages.Add(message);
            SelectedMessage = message;
        }

        private static string BuildCompressionMessage(ChatSessionRuntimeEvent runtimeEvent)
        {
            var info = runtimeEvent.ContextCompression;
            if (info == null)
            {
                return runtimeEvent.Message ?? "本轮执行触发了上下文压缩。";
            }

            return string.Join(
                Environment.NewLine,
                [
                    runtimeEvent.Message ?? "本轮执行触发了上下文压缩。",
                    $"压缩模型：{info.CompressionModelId ?? "未返回模型标识"}",
                    $"上下文窗口：{info.ContextWindowTokens:N0} tokens",
                    $"压缩前估算：{info.EstimatedTokenCountBeforeCompression:N0} tokens",
                    $"压缩后估算：{info.EstimatedTokenCountAfterCompression:N0} tokens",
                    $"目标上限：{info.TargetTokenCountAfterCompression:N0} tokens"
                ]);
        }

        private void RemoveSelectedMessage()
        {
            RemoveMessage(SelectedMessage);
        }

        private void RemoveMessage(ChatMessageModel? message)
        {
            if (message == null)
            {
                return;
            }

            var removedIndex = Messages.IndexOf(message);
            if (removedIndex < 0)
            {
                return;
            }

            Messages.RemoveAt(removedIndex);

            if (Messages.Count == 0)
            {
                SelectedMessage = null;
                RebuildConversationHistoryFromVisibleMessages();
                PersistSession();
                return;
            }

            var nextIndex = Math.Min(removedIndex, Messages.Count - 1);
            SelectedMessage = Messages[nextIndex];
            RebuildConversationHistoryFromVisibleMessages();
            PersistSession();
        }

        private void ClearMessages()
        {
            Messages.Clear();
            SelectedMessage = null;
            RebuildConversationHistoryFromVisibleMessages();
            PersistSession();
        }

        private void RebuildConversationHistoryFromVisibleMessages()
        {
            if (_sessionModel == null)
            {
                return;
            }

            // Keep the persisted transcript as the source of truth once it exists.
            // Visible chat cards are only a presentation layer and may omit hidden-agent
            // activity or intermediate tool traffic that must survive across turns.
            if (_sessionModel.ConversationHistory.Count > 0)
            {
                return;
            }

            _sessionModel.ConversationHistory.Clear();
            foreach (var message in ChatSessionTurnHistoryBuilder.BuildFromMessages(Messages))
            {
                _sessionModel.ConversationHistory.Add(message.Clone());
            }
        }

        private void OnMessagesCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (var item in e.NewItems.OfType<ChatMessageModel>())
                {
                    item.Parts.CollectionChanged += OnMessagePartsCollectionChanged;
                }
            }

            if (e.OldItems != null)
            {
                foreach (var item in e.OldItems.OfType<ChatMessageModel>())
                {
                    item.Parts.CollectionChanged -= OnMessagePartsCollectionChanged;
                }
            }
        }

        private void OnMessagePartsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            PersistSession();
        }

        private void LoadPersistedMessages(ChatSessionModel sessionModel)
        {
            _suppressPersistence = true;

            try
            {
                foreach (var message in sessionModel.Messages)
                {
                    if (IsLegacySessionInitializationMessage(message))
                    {
                        continue;
                    }

                    if (TryCloneMessage(message, out var clonedMessage) && clonedMessage != null)
                    {
                        Messages.Add(clonedMessage);
                    }
                }

                if (Messages.Count == 0)
                {
                    SeedMessages();
                }

                SelectedMessage = Messages.LastOrDefault();
            }
            finally
            {
                _suppressPersistence = false;
            }
        }

        private void PersistSession()
        {
            if (_suppressPersistence || _sessionModel == null || _chatSessionRepository == null)
            {
                return;
            }

            _sessionModel.Messages.Clear();
            foreach (var message in Messages)
            {
                if (IsLegacySessionInitializationMessage(message))
                {
                    continue;
                }

                if (TryCloneMessage(message, out var clonedMessage) && clonedMessage != null)
                {
                    _sessionModel.Messages.Add(clonedMessage);
                }
            }

            _sessionModel.ContextSummary = Messages.Count == 0
                ? "空会话"
                : $"当前共 {Messages.Count} 条消息，最后更新时间 {DateTime.Now:yyyy-MM-dd HH:mm:ss}";

            _chatSessionRepository.Save(_sessionModel);
        }

        private static bool IsLegacySessionInitializationMessage(ChatMessageModel message)
        {
            if (message.Role != ChatMessageRole.System || message.Parts.Count != 1)
            {
                return false;
            }

            var part = message.Parts[0];
            if (part.PartType != ChatMessagePartType.Status)
            {
                return false;
            }

            return string.Equals(part.Title, "初始化完成", StringComparison.Ordinal)
                || string.Equals(part.Title, "会话已就绪", StringComparison.Ordinal);
        }

        private static bool TryCloneMessage(ChatMessageModel source, out ChatMessageModel? clone)
        {
            var clonedParts = source.Parts
                .Where(part => !ChatSessionFinishTaskVisibility.IsFinishTaskPart(part))
                .Select(part => new ChatMessagePartModel(
                    part.PartType,
                    part.Content,
                    part.Title,
                    part.Language,
                    part.BadgeText,
                    part.IsStreaming))
                .ToArray();

            if (clonedParts.Length == 0)
            {
                clone = null;
                return false;
            }

            clone = new ChatMessageModel(
                source.Role,
                source.DisplayName,
                source.AvatarPath,
                source.Timestamp,
                clonedParts);
            return true;
        }

        private ChatMessageModel CreateMessage(ChatMessageRole role, params ChatMessagePartModel[] parts)
        {
            return new ChatMessageModel(role, GetDisplayName(role), GetAvatarPath(role), DateTime.Now, parts);
        }

        private static string GetDisplayName(ChatMessageRole role)
        {
            return role switch
            {
                ChatMessageRole.Assistant => "Skyweaver 助手",
                ChatMessageRole.System => "系统",
                _ => "用户"
            };
        }

        private static string GetAvatarPath(ChatMessageRole role)
        {
            return role switch
            {
                ChatMessageRole.Assistant => AssistantAvatarPath,
                ChatMessageRole.System => SystemAvatarPath,
                _ => UserAvatarPath
            };
        }

        private string BuildSessionFlowValidationSummary(ChatSessionModel? sessionModel)
        {
            if (sessionModel?.HasBoundFlow != true)
            {
                return "当前会话尚未绑定会话流。";
            }

            var compilationResult = _flowBindingService.CompileBinding(sessionModel.FlowBinding);
            if (compilationResult.IsSuccess)
            {
                return "绑定的会话流已通过运行时编译校验。";
            }

            var firstError = compilationResult.Errors.FirstOrDefault()?.Message;
            if (!string.IsNullOrWhiteSpace(firstError))
            {
                return firstError;
            }

            return compilationResult.Issues.FirstOrDefault()?.Message ?? "绑定的会话流尚未通过运行时校验。";
        }

        private void SeedMessages()
        {
            SelectedMessage = Messages.LastOrDefault();
        }
    }
}
