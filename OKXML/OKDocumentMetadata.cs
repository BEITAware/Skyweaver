using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace OKXML;

/// <summary>
/// 表示标题与它的区域 Hash 内容的键值对（用于 XML 序列化）。
/// </summary>
public class TitleHashEntry
{
    /// <summary>
    /// 标题行文本。
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 该标题控制区域的简短 Hash。
    /// </summary>
    public string Hash { get; set; } = string.Empty;
}

/// <summary>
/// 表示 OKXML 页面的元数据属性。
/// </summary>
[XmlRoot("OKDocumentMetadata")]
public class OKDocumentMetadata
{
    /// <summary>
    /// 唯一标识符。
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 页面的名称。
    /// </summary>
    public string PageName { get; set; } = string.Empty;

    /// <summary>
    /// 是否已嵌入。
    /// </summary>
    public bool Embedded { get; set; } = false;

    /// <summary>
    /// 是否已废弃。
    /// </summary>
    public bool Deprecated { get; set; } = false;

    /// <summary>
    /// 版本号，默认格式：n.m（如 1.0, 1.1, 2.1 等）。
    /// </summary>
    public string Version { get; set; } = "1.0";

    /// <summary>
    /// 整个文档本身的文档级 Hash（不剔除链接体）。
    /// </summary>
    public string DocumentHash { get; set; } = string.Empty;

    /// <summary>
    /// 标题与 Hash 字典对列表，用于 XML 存储。
    /// </summary>
    [XmlArray("TitleHashes")]
    [XmlArrayItem("Entry")]
    public List<TitleHashEntry> TitleHashes { get; set; } = new();
}
