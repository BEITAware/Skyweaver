using System.IO;
using Ferrita.Controls.WorkflowEditorControl.Services;
using Ferrita.Models.ChatSession;

namespace Ferrita.Services.ChatSession
{
    public sealed class ChatSessionFlowBindingOption
    {
        public string GraphId { get; init; } = string.Empty;

        public string GraphName { get; init; } = string.Empty;

        public string FilePath { get; init; } = string.Empty;

        public string DisplayName => string.IsNullOrWhiteSpace(GraphName) ? "未命名会话流" : GraphName;

        public ChatSessionFlowBinding ToBinding()
        {
            return new ChatSessionFlowBinding
            {
                GraphId = GraphId,
                GraphName = GraphName,
                FilePath = FilePath
            };
        }
    }

    public sealed class ChatSessionFlowBindingService
    {
        private const string DefaultGraphBaseName = "会话流节点图";
        private readonly SessionFlowRepository _sessionFlowRepository;
        private readonly SessionFlowRuntimeCompiler _runtimeCompiler;

        public ChatSessionFlowBindingService()
            : this(
                new SessionFlowRepository(new SessionFlowPathProvider()),
                new SessionFlowRuntimeCompiler())
        {
        }

        public ChatSessionFlowBindingService(
            SessionFlowRepository sessionFlowRepository,
            SessionFlowRuntimeCompiler runtimeCompiler)
        {
            _sessionFlowRepository = sessionFlowRepository ?? throw new ArgumentNullException(nameof(sessionFlowRepository));
            _runtimeCompiler = runtimeCompiler ?? throw new ArgumentNullException(nameof(runtimeCompiler));
        }

        public IReadOnlyList<ChatSessionFlowBindingOption> GetAvailableBindings(bool ensureDefaultGraph = false)
        {
            if (ensureDefaultGraph)
            {
                EnsureDefaultGraphExists();
            }

            return _sessionFlowRepository.LoadAllMetadata()
                .OrderByDescending(document => document.UpdatedAtUtc)
                .ThenBy(document => document.Name, StringComparer.CurrentCultureIgnoreCase)
                .Select(document => new ChatSessionFlowBindingOption
                {
                    GraphId = document.GraphId,
                    GraphName = document.Name,
                    FilePath = document.FilePath
                })
                .ToArray();
        }

        public ChatSessionFlowBinding ResolveDefaultBinding(bool ensureDefaultGraph = false)
        {
            return GetAvailableBindings(ensureDefaultGraph)
                .FirstOrDefault()?
                .ToBinding()
                ?? new ChatSessionFlowBinding();
        }

        public ChatSessionFlowBinding ResolveBinding(ChatSessionFlowBinding? preferredBinding, bool ensureDefaultGraph = false)
        {
            if (preferredBinding?.IsBound == true)
            {
                return preferredBinding.DeepClone();
            }

            return ResolveDefaultBinding(ensureDefaultGraph);
        }

        public SessionFlowCompilationResult CompileBinding(ChatSessionFlowBinding? binding)
        {
            if (binding?.IsBound != true)
            {
                return new SessionFlowCompilationResult
                {
                    IsSuccess = false,
                    Issues =
                    [
                        new SessionFlowCompilationIssue
                        {
                            Severity = SessionFlowCompilationIssueSeverity.Error,
                            Message = "当前会话尚未绑定会话流。"
                        }
                    ]
                };
            }

            var filePath = binding.FilePath?.Trim() ?? string.Empty;
            if (filePath.Length == 0 || !File.Exists(filePath))
            {
                return new SessionFlowCompilationResult
                {
                    IsSuccess = false,
                    Issues =
                    [
                        new SessionFlowCompilationIssue
                        {
                            Severity = SessionFlowCompilationIssueSeverity.Error,
                            Message = $"绑定的会话流文件不存在：{filePath}"
                        }
                    ]
                };
            }

            try
            {
                var document = _sessionFlowRepository.Load(filePath);
                return _runtimeCompiler.Compile(document);
            }
            catch (Exception ex)
            {
                return new SessionFlowCompilationResult
                {
                    IsSuccess = false,
                    Issues =
                    [
                        new SessionFlowCompilationIssue
                        {
                            Severity = SessionFlowCompilationIssueSeverity.Error,
                            Message = $"读取或编译会话流失败：{ex.Message}"
                        }
                    ]
                };
            }
        }

        private void EnsureDefaultGraphExists()
        {
            var existingDocuments = _sessionFlowRepository.LoadAllMetadata();
            if (existingDocuments.Count > 0)
            {
                return;
            }

            var graphName = _sessionFlowRepository.CreateUniqueGraphName(DefaultGraphBaseName);
            _sessionFlowRepository.Create(graphName);
        }
    }
}
