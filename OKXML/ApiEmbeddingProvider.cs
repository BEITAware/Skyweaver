using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AerialCity.Core.Primitives;
using AerialCity.Embedding;

namespace OKXML;

/// <summary>
/// 基于 AerialCity ApiEmbeddingService 的真实嵌入提供程序。
/// 通过 OpenAI 兼容接口或 Google Gemini 接口调用远程嵌入模型，
/// 产生真实的语义嵌入向量。
/// </summary>
public class ApiEmbeddingProvider : IEmbeddingProvider
{
    private readonly OKEmbeddingConfig _config;
    private readonly ApiEmbeddingService _service;

    /// <summary>
    /// 获取向量维度。若配置中指定了维度则使用配置值，否则使用默认 768。
    /// </summary>
    public int Dimensions { get; }

    /// <summary>
    /// 初始化一个新的 ApiEmbeddingProvider 实例。
    /// </summary>
    /// <param name="config">嵌入服务配置。</param>
    /// <param name="defaultDimensions">默认维度，当配置未指定维度时使用。</param>
    public ApiEmbeddingProvider(OKEmbeddingConfig config, int defaultDimensions = 768)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
        _service = new ApiEmbeddingService();
        Dimensions = config.Dimensions ?? defaultDimensions;
    }

    /// <summary>
    /// 异步计算单段文本的嵌入向量（通过真实 API 调用）。
    /// </summary>
    public async Task<EmbeddingVector> EmbedTextAsync(string text, CancellationToken ct = default)
    {
        var request = new ApiEmbeddingRequest
        {
            ApiKey = _config.ApiKey,
            Model = _config.Model,
            ApiType = _config.ApiType,
            BaseUrl = _config.BaseUrl,
            Dimensions = _config.Dimensions,
            Normalize = _config.Normalize,
            Content = EmbeddingInput.FromText(text),
            Parameters = new Dictionary<string, object?>(_config.Parameters)
        };

        var result = await _service.EmbedAsync(request, ct);
        return result.Vector;
    }

    /// <summary>
    /// 异步批量计算文本段的嵌入向量。
    /// 注意：当前逐个调用 API，适用于一般规模的 OKXML Wiki。
    /// </summary>
    public async Task<IReadOnlyList<EmbeddingVector>> EmbedBatchAsync(IReadOnlyList<string> texts, CancellationToken ct = default)
    {
        var results = new List<EmbeddingVector>(texts.Count);
        foreach (var text in texts)
        {
            ct.ThrowIfCancellationRequested();
            results.Add(await EmbedTextAsync(text, ct));
        }
        return results;
    }

    /// <summary>
    /// 异步计算二进制内容的嵌入向量（通过真实 API 调用）。
    /// </summary>
    public async Task<EmbeddingVector> EmbedBinaryAsync(ReadOnlyMemory<byte> data, string mimeType, CancellationToken ct = default)
    {
        var request = new ApiEmbeddingRequest
        {
            ApiKey = _config.ApiKey,
            Model = _config.Model,
            ApiType = _config.ApiType,
            BaseUrl = _config.BaseUrl,
            Dimensions = _config.Dimensions,
            Normalize = _config.Normalize,
            Content = EmbeddingInput.FromBinary(data, mimeType),
            Parameters = new Dictionary<string, object?>(_config.Parameters)
        };

        var result = await _service.EmbedAsync(request, ct);
        return result.Vector;
    }
}
