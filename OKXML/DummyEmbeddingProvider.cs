using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AerialCity.Core.Primitives;
using AerialCity.Embedding;

namespace OKXML;

/// <summary>
/// 一个默认的 Dummy 嵌入提供程序，用于在没有配置真实 Embedding 模型的环境中运行 OKXML。
/// </summary>
public class DummyEmbeddingProvider : IEmbeddingProvider
{
    /// <summary>
    /// 获取向量维度，默认为 768。
    /// </summary>
    public int Dimensions { get; }

    /// <summary>
    /// 初始化一个新的 DummyEmbeddingProvider 实例。
    /// </summary>
    /// <param name="dimensions">向量维度，默认 768。</param>
    public DummyEmbeddingProvider(int dimensions = 768)
    {
        Dimensions = dimensions;
    }

    /// <summary>
    /// 异步计算单段文本的嵌入向量。
    /// </summary>
    public Task<EmbeddingVector> EmbedTextAsync(string text, CancellationToken ct = default)
    {
        return Task.FromResult(GeneratePseudoRandomVector(text));
    }

    /// <summary>
    /// 异步批量计算文本段的嵌入向量。
    /// </summary>
    public Task<IReadOnlyList<EmbeddingVector>> EmbedBatchAsync(IReadOnlyList<string> texts, CancellationToken ct = default)
    {
        IReadOnlyList<EmbeddingVector> result = texts.Select(GeneratePseudoRandomVector).ToList();
        return Task.FromResult(result);
    }

    /// <summary>
    /// 异步计算二进制内容的嵌入向量。
    /// </summary>
    public Task<EmbeddingVector> EmbedBinaryAsync(ReadOnlyMemory<byte> data, string mimeType, CancellationToken ct = default)
    {
        return Task.FromResult(GeneratePseudoRandomVector(data.Length.ToString()));
    }

    private EmbeddingVector GeneratePseudoRandomVector(string text)
    {
        float[] values = new float[Dimensions];
        Random rand = new(text.GetHashCode());
        for (int i = 0; i < values.Length; i++)
        {
            values[i] = (float)rand.NextDouble() * 0.1f;
        }
        return new EmbeddingVector(values);
    }
}
