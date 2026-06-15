using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Skyweaver.Services.Directories;

namespace Skyweaver.Services.ChatSession
{
    public sealed class PromptHistoryAttachmentInfo
    {
        public string? DisplayName { get; set; }
        public string? ResourcePath { get; set; }
        public string? MediaType { get; set; }
        public string? PreservedContentXml { get; set; }
    }

    public sealed class PromptHistoryService
    {
        private static readonly object _syncRoot = new();
        private static PromptHistoryService? _instance;

        public static PromptHistoryService Instance
        {
            get
            {
                lock (_syncRoot)
                {
                    return _instance ??= new PromptHistoryService();
                }
            }
        }

        private PromptHistoryService()
        {
        }

        private string GetXmlFilePath()
        {
            var sessionsDir = SkyweaverDirectoryRuntime.Instance.ChatSessionsDirectoryPath;
            var promptHistoryDir = Path.Combine(sessionsDir, "PromptHistory");
            if (!Directory.Exists(promptHistoryDir))
            {
                Directory.CreateDirectory(promptHistoryDir);
            }
            return Path.Combine(promptHistoryDir, "PromptHistory.xml");
        }

        public void AddHistoryEntry(string text, string sessionType, IEnumerable<PromptHistoryAttachmentInfo>? attachments = null)
        {
            lock (_syncRoot)
            {
                try
                {
                    var filePath = GetXmlFilePath();
                    XDocument doc;
                    if (File.Exists(filePath))
                    {
                        try
                        {
                            doc = XDocument.Load(filePath);
                        }
                        catch
                        {
                            doc = new XDocument(new XElement("PromptHistory"));
                        }
                    }
                    else
                    {
                        doc = new XDocument(new XElement("PromptHistory"));
                    }

                    var root = doc.Root;
                    if (root == null)
                    {
                        root = new XElement("PromptHistory");
                        doc.Add(root);
                    }

                    var entry = new XElement("PromptEntry",
                        new XAttribute("Timestamp", DateTime.UtcNow.ToString("o")),
                        new XAttribute("SessionType", sessionType),
                        new XElement("Text", text ?? string.Empty)
                    );

                    if (attachments != null)
                    {
                        foreach (var att in attachments)
                        {
                            string preservedXml;
                            if (!string.IsNullOrWhiteSpace(att.PreservedContentXml))
                            {
                                preservedXml = att.PreservedContentXml;
                            }
                            else
                            {
                                preservedXml = PreservedTextContentXml.Build(
                                    string.Empty,
                                    att.DisplayName,
                                    att.ResourcePath,
                                    att.MediaType
                                );
                            }

                            try
                            {
                                var preservedElement = XElement.Parse(preservedXml);
                                entry.Add(preservedElement);
                            }
                            catch
                            {
                                // Ignore parse error
                            }
                        }
                    }

                    root.Add(entry);
                    doc.Save(filePath);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Failed to save prompt history: {ex.Message}");
                }
            }
        }

        public List<string> GetHistoryTexts()
        {
            lock (_syncRoot)
            {
                var list = new List<string>();
                try
                {
                    var filePath = GetXmlFilePath();
                    if (!File.Exists(filePath))
                    {
                        return list;
                    }

                    var doc = XDocument.Load(filePath);
                    var entries = doc.Root?.Elements("PromptEntry") ?? Enumerable.Empty<XElement>();
                    foreach (var entry in entries)
                    {
                        var textElement = entry.Element("Text");
                        if (textElement != null)
                        {
                            list.Add(textElement.Value);
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Failed to read prompt history: {ex.Message}");
                }
                return list;
            }
        }
    }
}
