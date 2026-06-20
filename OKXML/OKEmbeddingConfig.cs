using AerialCity.Embedding;

namespace OKXML;

/// <summary>
/// OKXML 嵌入服务的配置项，用于连接 AerialCity 的真实 API 嵌入服务。
/// 支持 OpenAI 兼容接口和 Google Gemini 接口。
/// </summary>
public sealed class OKEmbeddingConfig
{
    /// <summary>
    /// API 密钥。
    /// </summary>
    public required string ApiKey { get; init; }

    /// <summary>
    /// 嵌入模型名称（如 "text-embedding-3-small"、"text-embedding-004" 等）。
    /// </summary>
    public required string Model { get; init; }

    /// <summary>
    /// API 类型：OpenAI 兼容接口或 Google Gemini 接口。
    /// </summary>
    public EmbeddingApiType ApiType { get; init; } = EmbeddingApiType.OpenAI;

    /// <summary>
    /// 可选的 Base URL。如果使用非官方端点（如本地代理或兼容服务），可以在此指定。
    /// 若为 null，则使用各 API 的默认官方地址。
    /// </summary>
    public string? BaseUrl { get; init; }

    /// <summary>
    /// 输出向量的维度。如果模型支持自定义维度（如 OpenAI text-embedding-3-*），可在此指定。
    /// 若为 null，则使用模型默认维度。
    /// </summary>
    public int? Dimensions { get; init; }

    /// <summary>
    /// 是否对返回的向量进行 L2 归一化。默认为 true。
    /// </summary>
    public bool Normalize { get; init; } = true;

    /// <summary>
    /// 额外的提供程序专属参数（如 taskType、encoding_format 等）。
    /// </summary>
    public Dictionary<string, object?> Parameters { get; init; } = [];
}
