using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Skyweaver.Tools
{
    /// <summary>
    /// Hashline 辅助工具类，提供统一的行解析与哈希生成算法。
    /// </summary>
    internal static class HashlineHelper
    {
        public sealed class FileLine
        {
            public string Text { get; set; } = string.Empty;
            public string LineBreak { get; set; } = string.Empty;
            public string Hash { get; set; } = string.Empty;
        }

        /// <summary>
        /// 将文本内容拆分为 FileLine 对象列表，记录每行的纯文本内容及其后面的换行符。
        /// </summary>
        public static List<FileLine> ParseFileLines(string content)
        {
            var list = new List<FileLine>();
            if (string.IsNullOrEmpty(content))
            {
                return list;
            }

            int i = 0;
            while (i < content.Length)
            {
                int nextLineBreak = content.IndexOf('\n', i);
                if (nextLineBreak < 0)
                {
                    string lineText = content.Substring(i);
                    string lineBreak = "";
                    if (lineText.EndsWith("\r"))
                    {
                        lineText = lineText.Substring(0, lineText.Length - 1);
                        lineBreak = "\r";
                    }
                    list.Add(new FileLine { Text = lineText, LineBreak = lineBreak });
                    break;
                }
                else
                {
                    string rawLine = content.Substring(i, nextLineBreak - i);
                    string text = rawLine;
                    string lineBreak = "\n";
                    if (rawLine.EndsWith("\r"))
                    {
                        text = rawLine.Substring(0, rawLine.Length - 1);
                        lineBreak = "\r\n";
                    }
                    list.Add(new FileLine { Text = text, LineBreak = lineBreak });
                    i = nextLineBreak + 1;
                }
            }

            return list;
        }

        /// <summary>
        /// 批量计算所有行的哈希值。
        /// </summary>
        public static void CalculateHashes(List<FileLine> lines)
        {
            for (int i = 0; i < lines.Count; i++)
            {
                lines[i].Hash = ComputeLineHash(lines, i);
            }
        }

        /// <summary>
        /// 计算指定行的上下文哈希值。
        /// 通过参考当前行及其前后各 10 行（共 21 行）来进行哈希计算。
        /// </summary>
        public static string ComputeLineHash(List<FileLine> lines, int index)
        {
            string[] window = new string[21];
            for (int k = 0; k < 21; k++)
            {
                int targetIndex = index - 10 + k;
                if (targetIndex >= 0 && targetIndex < lines.Count)
                {
                    window[k] = lines[targetIndex].Text;
                }
                else
                {
                    window[k] = string.Empty;
                }
            }

            string combined = string.Join("\n", window);
            byte[] hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(combined));
            uint value = BitConverter.ToUInt32(hashBytes, 0);

            const string alphabet = "0123456789abcdefghijklmnopqrstuvwxyz";
            uint remainder = value % 1679616; // 36^4

            char[] chars = new char[4];
            for (int k = 3; k >= 0; k--)
            {
                chars[k] = alphabet[(int)(remainder % 36)];
                remainder /= 36;
            }
            return new string(chars);
        }
    }
}
