using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ferrita.Controls.LanguageModelConfigurationControl.Models;
using Ferrita.Services.FerritaTools;
using Tokenizers.HuggingFace.Tokenizer;

namespace Ferrita.Controls.LanguageModelConfigurationControl.Services
{
    public sealed class LanguageModelChatService : ILanguageModelChatService
    {
        public IReadOnlyList<string> AvailableInterfaceTypes => LanguageModelInterfaceCatalog.AvailableInterfaceTypes;

        public LanguageModelInterfaceSettings CreateInterfaceSettings(string? interfaceType)
        {
            return LanguageModelInterfaceCatalog.CreateInterfaceSettings(interfaceType);
        }

        public void Validate(LanguageModelDefinition model)
        {
            ResolveAdapter(model).Validate(model);
        }

        public async Task<LanguageModelChatResponse> GetResponseAsync(
            LanguageModelDefinition model,
            IReadOnlyList<LanguageModelChatMessage> messages,
            CancellationToken cancellationToken = default,
            Func<LanguageModelMediaProcessingProgress, CancellationToken, ValueTask>? mediaProcessingProgress = null,
            IReadOnlyList<FerritaPromptToolDefinition>? tools = null)
        {
            ArgumentNullException.ThrowIfNull(model);
            ArgumentNullException.ThrowIfNull(messages);

            var projectedMessages = await LanguageModelChatTransportProjection.ProjectMessagesAsync(
                    messages,
                    model,
                    mediaProcessingProgress,
                    cancellationToken)
                .ConfigureAwait(false);
            return await ResolveAdapter(model).GetResponseAsync(model, projectedMessages, cancellationToken, tools).ConfigureAwait(false);
        }

        public async Task<int> CountTokensAsync(
            LanguageModelDefinition model,
            IReadOnlyList<LanguageModelChatMessage> messages,
            CancellationToken cancellationToken = default,
            Func<LanguageModelMediaProcessingProgress, CancellationToken, ValueTask>? mediaProcessingProgress = null)
        {
            ArgumentNullException.ThrowIfNull(model);
            ArgumentNullException.ThrowIfNull(messages);

            System.Diagnostics.Debug.WriteLine($"[LanguageModelChatService] Starting CountTokensAsync for model: {model.Key} (Interface: {model.InterfaceType})");
            System.Diagnostics.Debug.WriteLine($"[LanguageModelChatService] Original messages count: {messages.Count}. Projecting messages...");

            var projectedMessages = await LanguageModelChatTransportProjection.ProjectMessagesAsync(
                    messages,
                    model,
                    mediaProcessingProgress,
                    cancellationToken)
                .ConfigureAwait(false);

            System.Diagnostics.Debug.WriteLine($"[LanguageModelChatService] Projected messages count: {projectedMessages.Count}. Attempting to count via Adapter...");

            int tokenCount = 0;
            bool success = false;

            using var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);

            try
            {
                tokenCount = await ResolveAdapter(model).CountTokensAsync(model, projectedMessages, linkedCts.Token).ConfigureAwait(false);
                System.Diagnostics.Debug.WriteLine($"[LanguageModelChatService] Adapter returned token count: {tokenCount}");
                if (tokenCount > 0)
                {
                    success = true;
                }
            }
            catch (OperationCanceledException) when (timeoutCts.IsCancellationRequested && !cancellationToken.IsCancellationRequested)
            {
                System.Diagnostics.Debug.WriteLine($"[LanguageModelChatService] Adapter token counting timed out after 5s. Will fallback to local estimation.");
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[LanguageModelChatService] Adapter count failed: {ex.Message}. Will fallback to local estimation.");
                // 官方方法出错，将退化到本地估计
            }

            if (!success)
            {
                System.Diagnostics.Debug.WriteLine($"[LanguageModelChatService] Adapter count was 0 or failed. Executing EstimateTokensLocally.");
                tokenCount = EstimateTokensLocally(projectedMessages);
            }

            System.Diagnostics.Debug.WriteLine($"[LanguageModelChatService] Final Token Count: {tokenCount}");
            return tokenCount;
        }

        private static int EstimateTokensLocally(IReadOnlyList<LanguageModelChatMessage> messages)
        {
            System.Diagnostics.Debug.WriteLine("[LanguageModelChatService] Starting EstimateTokensLocally.");
            // 尝试在多个可能的位置寻找本地 tokenizer.json
            var pathsToTry = new[]
            {
                Path.Combine(AppContext.BaseDirectory, "Resources", "tokenizer.json"),
                Path.Combine(AppContext.BaseDirectory, "resources", "tokenizer.json"),
                Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "Ferrita", "Resources", "tokenizer.json"),
                Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "Ferrita", "resources", "tokenizer.json"),
                Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "Ferrita", "Resources", "tokenizer.json"),
                Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "Ferrita", "resources", "tokenizer.json"),
                Path.Combine(AppContext.BaseDirectory, "tokenizer.json")
            };

            string? tokenizerPath = null;
            foreach (var path in pathsToTry)
            {
                if (File.Exists(path))
                {
                    tokenizerPath = path;
                    break;
                }
            }

            if (tokenizerPath != null)
            {
                System.Diagnostics.Debug.WriteLine($"[LanguageModelChatService] Found tokenizer at: {tokenizerPath}");
                try
                {
                    using var tokenizer = Tokenizer.FromFile(tokenizerPath);
                    int totalTokens = 0;
                    foreach (var message in messages)
                    {
                        var role = message.Role.ToString().ToLowerInvariant();
                        // 包含角色和内容，并加上常见的 ChatML 标记结构
                        var text = $"<|im_start|>{role}\n{message.Content}<|im_end|>\n";
                        if (!string.IsNullOrEmpty(message.ReasoningContent))
                        {
                            text += $"<|im_start|>thought\n{message.ReasoningContent}<|im_end|>\n";
                        }

                        var encodings = tokenizer.Encode(text, true);
                        var firstEncoding = encodings?.FirstOrDefault();
                        if (firstEncoding != null && firstEncoding.Ids != null)
                        {
                            totalTokens += firstEncoding.Ids.Count();
                        }
                    }
                    System.Diagnostics.Debug.WriteLine($"[LanguageModelChatService] Local Tokenizer computed token count: {totalTokens}");
                    return totalTokens;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[LanguageModelChatService] Local Tokenizer failed: {ex.Message}. Falling back to character estimation.");
                    // 如果本地 Tokenizer 运行出错，则退化到字符数估算
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("[LanguageModelChatService] Tokenizer not found. Falling back to character count estimation.");
            }

            // 退化估算方法：平均每个单词/中文字符估算
            int characterCount = 0;
            foreach (var message in messages)
            {
                characterCount += message.Content?.Length ?? 0;
                characterCount += message.ReasoningContent?.Length ?? 0;
            }
            // 英文单词和中文混合下，粗略以字符数除以 2.5 作为估算，避免返回 0
            int estimated = Math.Max(1, (int)(characterCount / 2.5));
            System.Diagnostics.Debug.WriteLine($"[LanguageModelChatService] Fallback character estimation computed token count: {estimated} (Based on {characterCount} characters)");
            return estimated;
        }

        public async IAsyncEnumerable<LanguageModelStreamingChatUpdate> GetStreamingResponseAsync(
            LanguageModelDefinition model,
            IReadOnlyList<LanguageModelChatMessage> messages,
            [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default,
            Func<LanguageModelMediaProcessingProgress, CancellationToken, ValueTask>? mediaProcessingProgress = null,
            IReadOnlyList<FerritaPromptToolDefinition>? tools = null)
        {
            ArgumentNullException.ThrowIfNull(model);
            ArgumentNullException.ThrowIfNull(messages);

            var projectedMessages = await LanguageModelChatTransportProjection.ProjectMessagesAsync(
                    messages,
                    model,
                    mediaProcessingProgress,
                    cancellationToken)
                .ConfigureAwait(false);
            await foreach (var update in ResolveAdapter(model)
                               .GetStreamingResponseAsync(model, projectedMessages, cancellationToken, tools)
                               .ConfigureAwait(false))
            {
                yield return update;
            }
        }

        private static ILanguageModelInterfaceAdapter ResolveAdapter(LanguageModelDefinition model)
        {
            ArgumentNullException.ThrowIfNull(model);
            return LanguageModelInterfaceCatalog.CreateAdapter(model.InterfaceType);
        }
    }
}
