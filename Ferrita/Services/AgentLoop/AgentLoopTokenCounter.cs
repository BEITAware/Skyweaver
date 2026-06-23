using Ferrita.Controls.LanguageModelConfigurationControl.Models;
using Ferrita.Controls.LanguageModelConfigurationControl.Services;

namespace Ferrita.Services.AgentLoop
{
    public sealed class AgentLoopTokenCounter
    {
        private readonly ILanguageModelChatService _chatService;
        private readonly AgentLoopCompactionStore _compactionStore;

        public AgentLoopTokenCounter()
            : this(new LanguageModelChatService(), new AgentLoopCompactionStore())
        {
        }

        public AgentLoopTokenCounter(
            ILanguageModelChatService chatService,
            AgentLoopCompactionStore compactionStore)
        {
            _chatService = chatService ?? throw new ArgumentNullException(nameof(chatService));
            _compactionStore = compactionStore ?? throw new ArgumentNullException(nameof(compactionStore));
        }

        public async Task<AgentLoopTokenCountResult> CountAsync(
            LanguageModelDefinition model,
            IReadOnlyList<LanguageModelChatMessage> messages,
            string? compactionFilePath,
            Func<LanguageModelMediaProcessingProgress, CancellationToken, ValueTask>? progressCallback = null,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(model);
            ArgumentNullException.ThrowIfNull(messages);

            System.Diagnostics.Debug.WriteLine("[AgentLoopTokenCounter] --------------------------------------------------");
            System.Diagnostics.Debug.WriteLine("[AgentLoopTokenCounter] Token counting started.");
            System.Diagnostics.Debug.WriteLine($"[AgentLoopTokenCounter] Message Sequence ({messages.Count} messages):");
            
            int totalTextLength = 0;
            int totalMediaItems = 0;
            
            for (int i = 0; i < messages.Count; i++)
            {
                var msg = messages[i];
                int contentLen = msg.Content?.Length ?? 0;
                int reasoningLen = msg.ReasoningContent?.Length ?? 0;
                int mediaCount = msg.ContentBlocks?.Count(b => !b.IsTextLike) ?? 0;
                
                totalTextLength += contentLen + reasoningLen;
                totalMediaItems += mediaCount;
                
                System.Diagnostics.Debug.WriteLine($"  [{i:D2}] Role: {msg.Role,-10} | ContentLength: {contentLen,-5} | ReasoningLength: {reasoningLen,-5} | MediaItems: {mediaCount}");
                
                if (mediaCount > 0 && msg.ContentBlocks != null)
                {
                    int mIdx = 0;
                    for (int j = 0; j < msg.ContentBlocks.Count; j++)
                    {
                        var block = msg.ContentBlocks[j];
                        if (!block.IsTextLike)
                        {
                            System.Diagnostics.Debug.WriteLine($"       - Media[{mIdx}]: Kind={block.Kind}, MediaType={block.MediaType}, DataLength={(block.Data?.Length ?? 0)} bytes");
                            mIdx++;
                        }
                    }
                }
            }
            
            System.Diagnostics.Debug.WriteLine($"[AgentLoopTokenCounter] Summary: TotalTextLength={totalTextLength}, TotalMediaItems={totalMediaItems}");

            var modelKey = BuildModelKey(model);
            System.Diagnostics.Debug.WriteLine($"[AgentLoopTokenCounter] Target Model Key: {modelKey}");

            var hash = AgentLoopCompactionStore.CreateTokenCountHash(messages);
            System.Diagnostics.Debug.WriteLine($"[AgentLoopTokenCounter] Request Hash: {hash}");

            if (_compactionStore.TryGetTokenCount(compactionFilePath, modelKey, hash, out var cachedTokenCount))
            {
                System.Diagnostics.Debug.WriteLine($"[AgentLoopTokenCounter] Cache HIT! Cached token count: {cachedTokenCount} (Source: Compaction.xml)");
                System.Diagnostics.Debug.WriteLine("[AgentLoopTokenCounter] --------------------------------------------------");
                return new AgentLoopTokenCountResult(cachedTokenCount, modelKey, hash, "Compaction.xml cache");
            }
            
            System.Diagnostics.Debug.WriteLine("[AgentLoopTokenCounter] Cache MISS. Falling back to LLM API / Local Estimation...");

            var tokenCount = await _chatService.CountTokensAsync(
                model,
                messages.Select(message => message.Clone()).ToArray(),
                cancellationToken,
                progressCallback).ConfigureAwait(false);
                
            System.Diagnostics.Debug.WriteLine($"[AgentLoopTokenCounter] Counting completed. Token count: {tokenCount} (Source: LLM API / Local)");

            _compactionStore.SaveTokenCount(
                compactionFilePath,
                modelKey,
                hash,
                tokenCount,
                "LLM API");

            System.Diagnostics.Debug.WriteLine("[AgentLoopTokenCounter] --------------------------------------------------");
            return new AgentLoopTokenCountResult(tokenCount, modelKey, hash, "LLM API");
        }



        private static string BuildModelKey(LanguageModelDefinition model)
        {
            var interfaceType = string.IsNullOrWhiteSpace(model.InterfaceType) ? "unknown-interface" : model.InterfaceType.Trim();
            var modelId = string.IsNullOrWhiteSpace(model.SummaryModelId)
                ? string.IsNullOrWhiteSpace(model.Key) ? "unknown-model" : model.Key.Trim()
                : model.SummaryModelId.Trim();
            return $"{interfaceType}:{modelId}";
        }
    }

    public sealed record AgentLoopTokenCountResult(
        int TokenCount,
        string ModelKey,
        string Hash,
        string Source);
}
