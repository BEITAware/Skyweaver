namespace OKXML;

/// <summary>
/// 表示 BEITAware Extensible Information Worker Knowledge Document Format (O.K.XML) 的基础文档对象。
/// </summary>
public class OKDocument
{
    /// <summary>
    /// 获取或设置文档的唯一标识符。
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置文档的版本。
    /// </summary>
    public string Version { get; set; } = "1.0.0";

    /// <summary>
    /// 获取或设置文档的元数据。
    /// </summary>
    public Dictionary<string, string> Metadata { get; set; } = new();

    /// <summary>
    /// 获取或设置文档的内容。
    /// </summary>
    public string Content { get; set; } = string.Empty;
}
