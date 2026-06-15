using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Skyweaver.Services.Directories;

namespace Skyweaver.Services.StickyNotes
{
    public class JournalEntry
    {
        public string Creator { get; set; } = string.Empty;
        public DateTime DateTime { get; set; }
        public string Content { get; set; } = string.Empty;
    }

    public class StickyNoteReply
    {
        public string TileCode { get; set; } = string.Empty;
        public string Creator { get; set; } = string.Empty;
        public DateTime DateTime { get; set; }
        public string Content { get; set; } = string.Empty;
        public bool IsRead { get; set; }
    }

    public static class StickyNotesService
    {
        public static event Action<string>? ReplyAdded;

        public static void NotifyReplyAdded(string tileCode)
        {
            ReplyAdded?.Invoke(tileCode);
        }

        private static string GetJournalFilePath()
        {
            var dir = Path.Combine(SkyweaverDirectoryRuntime.Instance.ConfigurationDirectoryPath, "Tiles");
            Directory.CreateDirectory(dir);
            return Path.Combine(dir, "StickyNotesJournal.xml");
        }

        private static string GetRepliesFilePath()
        {
            var dir = Path.Combine(SkyweaverDirectoryRuntime.Instance.ConfigurationDirectoryPath, "Tiles");
            Directory.CreateDirectory(dir);
            return Path.Combine(dir, "StickyNotesReplies.xml");
        }

        // ==================== 便笺日志 (Journal) ====================

        public static void AddJournalEntry(string creator, string content)
        {
            var filePath = GetJournalFilePath();
            XDocument doc;
            if (File.Exists(filePath))
            {
                try
                {
                    doc = XDocument.Load(filePath);
                }
                catch
                {
                    doc = new XDocument(new XElement("StickyNotesJournal"));
                }
            }
            else
            {
                doc = new XDocument(new XElement("StickyNotesJournal"));
            }

            var root = doc.Root ?? new XElement("StickyNotesJournal");
            if (doc.Root == null)
            {
                doc.Add(root);
            }

            var entry = new XElement("Entry",
                new XAttribute("Creator", creator ?? string.Empty),
                new XAttribute("DateTime", DateTime.Now.ToString("O")),
                content ?? string.Empty
            );

            root.Add(entry);
            doc.Save(filePath);
        }

        public static List<JournalEntry> GetJournalEntries()
        {
            var filePath = GetJournalFilePath();
            var list = new List<JournalEntry>();
            if (!File.Exists(filePath))
            {
                return list;
            }

            try
            {
                var doc = XDocument.Load(filePath);
                var root = doc.Root;
                if (root == null) return list;

                foreach (var el in root.Elements("Entry"))
                {
                    var creator = (string?)el.Attribute("Creator") ?? string.Empty;
                    var dtStr = (string?)el.Attribute("DateTime");
                    var content = el.Value;

                    DateTime.TryParse(dtStr, out var dt);
                    list.Add(new JournalEntry
                    {
                        Creator = creator,
                        DateTime = dt,
                        Content = content
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error reading journal entries: {ex.Message}");
            }

            return list;
        }

        public static bool DeleteJournalEntry(string creator, DateTime dateTime)
        {
            var filePath = GetJournalFilePath();
            if (!File.Exists(filePath))
            {
                return false;
            }

            try
            {
                var doc = XDocument.Load(filePath);
                var root = doc.Root;
                if (root == null) return false;

                XElement? target = null;
                foreach (var el in root.Elements("Entry"))
                {
                    var c = (string?)el.Attribute("Creator");
                    var dtStr = (string?)el.Attribute("DateTime");
                    if (string.Equals(c, creator, StringComparison.OrdinalIgnoreCase) && DateTime.TryParse(dtStr, out var dt))
                    {
                        if (Math.Abs((dt - dateTime).TotalSeconds) < 2)
                        {
                            target = el;
                            break;
                        }
                    }
                }

                if (target != null)
                {
                    target.Remove();
                    doc.Save(filePath);
                    return true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error deleting journal entry: {ex.Message}");
            }

            return false;
        }

        public static void DeleteAllJournalEntries()
        {
            var filePath = GetJournalFilePath();
            if (File.Exists(filePath))
            {
                try
                {
                    File.Delete(filePath);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error clearing journal entries: {ex.Message}");
                }
            }
        }

        // ==================== 便笺回复 (Replies) ====================

        public static void AddReply(string tileCode, string creator, string content)
        {
            var filePath = GetRepliesFilePath();
            XDocument doc;
            if (File.Exists(filePath))
            {
                try
                {
                    doc = XDocument.Load(filePath);
                }
                catch
                {
                    doc = new XDocument(new XElement("StickyNotesReplies"));
                }
            }
            else
            {
                doc = new XDocument(new XElement("StickyNotesReplies"));
            }

            var root = doc.Root ?? new XElement("StickyNotesReplies");
            if (doc.Root == null)
            {
                doc.Add(root);
            }

            var entry = new XElement("Reply",
                new XAttribute("TileCode", tileCode ?? string.Empty),
                new XAttribute("Creator", creator ?? string.Empty),
                new XAttribute("DateTime", DateTime.Now.ToString("O")),
                new XAttribute("IsRead", "false"),
                content ?? string.Empty
            );

            root.Add(entry);
            doc.Save(filePath);

            NotifyReplyAdded(tileCode ?? string.Empty);
        }

        public static List<StickyNoteReply> GetReplies(string tileCode)
        {
            var filePath = GetRepliesFilePath();
            var list = new List<StickyNoteReply>();
            if (!File.Exists(filePath))
            {
                return list;
            }

            try
            {
                var doc = XDocument.Load(filePath);
                var root = doc.Root;
                if (root == null) return list;

                foreach (var el in root.Elements("Reply"))
                {
                    var tc = (string?)el.Attribute("TileCode") ?? string.Empty;
                    if (string.Equals(tc, tileCode, StringComparison.OrdinalIgnoreCase))
                    {
                        var creator = (string?)el.Attribute("Creator") ?? string.Empty;
                        var dtStr = (string?)el.Attribute("DateTime");
                        var isReadStr = (string?)el.Attribute("IsRead");
                        var content = el.Value;

                        DateTime.TryParse(dtStr, out var dt);
                        bool.TryParse(isReadStr, out var isRead);

                        list.Add(new StickyNoteReply
                        {
                            TileCode = tc,
                            Creator = creator,
                            DateTime = dt,
                            Content = content,
                            IsRead = isRead
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error reading replies: {ex.Message}");
            }

            return list;
        }

        public static void MarkRepliesAsRead(string tileCode)
        {
            var filePath = GetRepliesFilePath();
            if (!File.Exists(filePath)) return;

            try
            {
                var doc = XDocument.Load(filePath);
                var root = doc.Root;
                if (root == null) return;

                bool changed = false;
                foreach (var el in root.Elements("Reply"))
                {
                    var tc = (string?)el.Attribute("TileCode");
                    if (string.Equals(tc, tileCode, StringComparison.OrdinalIgnoreCase))
                    {
                        var isReadAttr = el.Attribute("IsRead");
                        if (isReadAttr == null || isReadAttr.Value == "false")
                        {
                            el.SetAttributeValue("IsRead", "true");
                            changed = true;
                        }
                    }
                }

                if (changed)
                {
                    doc.Save(filePath);
                    NotifyReplyAdded(tileCode ?? string.Empty);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error marking replies as read: {ex.Message}");
            }
        }
    }
}
