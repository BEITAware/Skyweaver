using System.Text;
using Skyweaver.Services.SkyweaverTools;

namespace Skyweaver.Controls.ChatSessionControl.Services
{
    public enum AssistantStreamingPartKind
    {
        Text = 0,
        ToolCall = 1,
        MalformedToolCall = 2
    }

    public sealed class AssistantStreamingPart
    {
        public AssistantStreamingPartKind Kind { get; init; }

        public string Content { get; init; } = string.Empty;

        public SkyweaverToolInvocation? ToolInvocation { get; init; }

        public string? ErrorMessage { get; init; }

        public int ToolCallIndex { get; init; }
    }

    public sealed class AssistantStreamingParseResult
    {
        public IReadOnlyList<AssistantStreamingPart> Parts { get; init; } = Array.Empty<AssistantStreamingPart>();

        public bool HasOpenToolsBlock { get; init; }

        public string PendingBuffer { get; init; } = string.Empty;
    }

    public sealed class AssistantResponseStreamingParser
    {
        private readonly SkyweaverToolManager _toolManager;
        private readonly StringBuilder _buffer = new();
        private const string ToolsOpenTagPrefix = "<Tools";
        private const string ToolOpenTagPrefix = "<Tool";
        private bool _insideToolsBlock;
        private int _toolCallIndex;

        public AssistantResponseStreamingParser()
            : this(new SkyweaverToolManager())
        {
        }

        public AssistantResponseStreamingParser(SkyweaverToolManager toolManager)
        {
            _toolManager = toolManager ?? throw new ArgumentNullException(nameof(toolManager));
        }

        public AssistantStreamingParseResult Append(string? chunk)
        {
            if (!string.IsNullOrEmpty(chunk))
            {
                _buffer.Append(chunk);
            }

            return ProcessBuffer(isFinal: false);
        }

        public AssistantStreamingParseResult Complete()
        {
            return ProcessBuffer(isFinal: true);
        }

        public void Reset()
        {
            _buffer.Clear();
            _insideToolsBlock = false;
            _toolCallIndex = 0;
        }

        private AssistantStreamingParseResult ProcessBuffer(bool isFinal)
        {
            var completedParts = new List<AssistantStreamingPart>();

            while (true)
            {
                if (!_insideToolsBlock)
                {
                    var bufferText = _buffer.ToString();
                    var toolsStartIndex = IndexOfOpeningTag(bufferText, "Tools");
                    var standaloneToolStartIndex = IndexOfOpeningTag(bufferText, "Tool");
                    var pseudoToolStartIndex = SkyweaverToolSyntaxInspector.IndexOfInvalidPseudoToolMarkup(bufferText);
                    var nextTagStartIndex = GetNearestTagStartIndex(
                        toolsStartIndex,
                        standaloneToolStartIndex,
                        pseudoToolStartIndex);
                    if (nextTagStartIndex < 0)
                    {
                        if (_buffer.Length > 0)
                        {
                            var trailingPrefixLength = isFinal
                                ? 0
                                : Math.Max(
                                    GetTrailingPotentialTagPrefixLength(bufferText, ToolsOpenTagPrefix),
                                    Math.Max(
                                        GetTrailingPotentialTagPrefixLength(bufferText, ToolOpenTagPrefix),
                                        SkyweaverToolSyntaxInspector.GetTrailingPotentialInvalidPseudoToolPrefixLength(bufferText)));
                            var flushLength = bufferText.Length - trailingPrefixLength;

                            if (flushLength > 0)
                            {
                                completedParts.Add(CreateTextPart(bufferText[..flushLength]));
                                _buffer.Remove(0, flushLength);
                            }
                        }

                        break;
                    }

                    if (nextTagStartIndex > 0)
                    {
                        completedParts.Add(CreateTextPart(bufferText[..nextTagStartIndex]));
                        _buffer.Remove(0, nextTagStartIndex);
                        bufferText = _buffer.ToString();
                        toolsStartIndex = IndexOfOpeningTag(bufferText, "Tools");
                        standaloneToolStartIndex = IndexOfOpeningTag(bufferText, "Tool");
                        pseudoToolStartIndex = SkyweaverToolSyntaxInspector.IndexOfInvalidPseudoToolMarkup(bufferText);
                    }

                    if (toolsStartIndex == 0)
                    {
                        var toolsOpenTagEnd = bufferText.IndexOf('>');
                        if (toolsOpenTagEnd < 0)
                        {
                            break;
                        }

                        _buffer.Remove(0, toolsOpenTagEnd + 1);
                        _insideToolsBlock = true;
                        continue;
                    }

                    if (standaloneToolStartIndex == 0)
                    {
                        var standaloneToolEndIndex = IndexOfClosingElementEnd(bufferText, "Tool", 0);
                        if (standaloneToolEndIndex < 0)
                        {
                            if (isFinal)
                            {
                                completedParts.Add(CreateMalformedPart(_buffer.ToString(), "<Tool> 元素缺少闭合的 </Tool> 标签。"));
                                _buffer.Clear();
                            }

                            break;
                        }

                        var completedStandaloneToolXml = bufferText[..standaloneToolEndIndex];
                        TryAppendCompletedTool(completedParts, completedStandaloneToolXml, completedStandaloneToolXml);
                        _buffer.Remove(0, standaloneToolEndIndex);
                        continue;
                    }

                    if (pseudoToolStartIndex == 0)
                    {
                        var pseudoToolLength = SkyweaverToolSyntaxInspector.GetInvalidPseudoToolMarkupLength(
                            bufferText,
                            isFinal);
                        if (pseudoToolLength < 0)
                        {
                            break;
                        }

                        completedParts.Add(CreateMalformedPart(
                            bufferText[..pseudoToolLength],
                            SkyweaverToolSyntaxInspector.InvalidPseudoToolMarkupErrorMessage));
                        _buffer.Remove(0, pseudoToolLength);
                        continue;
                    }
                }

                var toolsBuffer = _buffer.ToString();
                TrimLeadingWhitespace();
                toolsBuffer = _buffer.ToString();

                var nextToolStartIndex = IndexOfOpeningTag(toolsBuffer, "Tool");
                var toolsEndStartIndex = IndexOfClosingTagStart(toolsBuffer, "Tools");

                if (toolsEndStartIndex >= 0 && (nextToolStartIndex < 0 || toolsEndStartIndex < nextToolStartIndex))
                {
                    var toolsEndIndex = FindTagEnd(toolsBuffer, toolsEndStartIndex);
                    if (toolsEndIndex < 0)
                    {
                        break;
                    }

                    _buffer.Remove(0, toolsEndIndex);
                    _insideToolsBlock = false;
                    continue;
                }

                if (nextToolStartIndex < 0)
                {
                    if (isFinal && _buffer.Length > 0)
                    {
                        completedParts.Add(CreateMalformedPart(_buffer.ToString(), "工具调用 XML 不完整。"));
                        _buffer.Clear();
                        _insideToolsBlock = false;
                    }

                    break;
                }

                if (nextToolStartIndex > 0)
                {
                    _buffer.Remove(0, nextToolStartIndex);
                    toolsBuffer = _buffer.ToString();
                }

                var toolEndIndex = IndexOfClosingElementEnd(toolsBuffer, "Tool", 0);
                if (toolEndIndex < 0)
                {
                    if (isFinal)
                    {
                        completedParts.Add(CreateMalformedPart(_buffer.ToString(), "<Tool> 元素缺少闭合的 </Tool> 标签。"));
                        _buffer.Clear();
                        _insideToolsBlock = false;
                    }

                    break;
                }

                var completedToolXml = toolsBuffer[..toolEndIndex];
                TryAppendCompletedTool(completedParts, completedToolXml, $"<Tools>{completedToolXml}</Tools>");

                _buffer.Remove(0, completedToolXml.Length);
            }

            return new AssistantStreamingParseResult
            {
                Parts = completedParts,
                HasOpenToolsBlock = _insideToolsBlock,
                PendingBuffer = _buffer.ToString()
            };
        }

        private void TryAppendCompletedTool(
            ICollection<AssistantStreamingPart> completedParts,
            string content,
            string parseCandidateXml)
        {
            try
            {
                var invocations = _toolManager.ParseToolsInvocationXml(parseCandidateXml);
                if (invocations.Count > 0)
                {
                    completedParts.Add(new AssistantStreamingPart
                    {
                        Kind = AssistantStreamingPartKind.ToolCall,
                        Content = content,
                        ToolInvocation = invocations[0],
                        ToolCallIndex = ++_toolCallIndex
                    });
                }
            }
            catch (Exception ex)
            {
                completedParts.Add(CreateMalformedPart(content, ex.Message));
            }
        }

        private void TrimLeadingWhitespace()
        {
            var index = 0;
            while (index < _buffer.Length && char.IsWhiteSpace(_buffer[index]))
            {
                index++;
            }

            if (index > 0)
            {
                _buffer.Remove(0, index);
            }
        }

        private static AssistantStreamingPart CreateTextPart(string text)
        {
            return new AssistantStreamingPart
            {
                Kind = AssistantStreamingPartKind.Text,
                Content = text
            };
        }

        private AssistantStreamingPart CreateMalformedPart(string content, string errorMessage)
        {
            return new AssistantStreamingPart
            {
                Kind = AssistantStreamingPartKind.MalformedToolCall,
                Content = content,
                ErrorMessage = errorMessage,
                ToolCallIndex = ++_toolCallIndex
            };
        }

        private static int GetNearestTagStartIndex(params int[] candidateIndexes)
        {
            if (candidateIndexes == null || candidateIndexes.Length == 0)
            {
                return -1;
            }

            var nearestIndex = -1;
            foreach (var candidateIndex in candidateIndexes)
            {
                if (candidateIndex < 0)
                {
                    continue;
                }

                if (nearestIndex < 0 || candidateIndex < nearestIndex)
                {
                    nearestIndex = candidateIndex;
                }
            }

            return nearestIndex;
        }

        private static int GetTrailingPotentialTagPrefixLength(string text, string prefix)
        {
            if (string.IsNullOrEmpty(text))
            {
                return 0;
            }

            var maxLength = Math.Min(text.Length, prefix.Length - 1);
            for (var length = maxLength; length > 0; length--)
            {
                if (text.EndsWith(
                        prefix[..length],
                        StringComparison.OrdinalIgnoreCase))
                {
                    return length;
                }
            }

            return 0;
        }

        private static int IndexOfOpeningTag(string text, string elementName, int startIndex = 0)
        {
            var needle = $"<{elementName}";
            var searchIndex = Math.Max(0, startIndex);

            while (searchIndex < text.Length)
            {
                var matchIndex = text.IndexOf(needle, searchIndex, StringComparison.OrdinalIgnoreCase);
                if (matchIndex < 0)
                {
                    return -1;
                }

                var nameEndIndex = matchIndex + needle.Length;
                if (nameEndIndex >= text.Length || IsXmlNameBoundary(text[nameEndIndex]))
                {
                    return matchIndex;
                }

                searchIndex = matchIndex + 1;
            }

            return -1;
        }

        private static int IndexOfClosingTagStart(string text, string elementName, int startIndex = 0)
        {
            var needle = $"</{elementName}";
            var searchIndex = Math.Max(0, startIndex);

            while (searchIndex < text.Length)
            {
                var matchIndex = text.IndexOf(needle, searchIndex, StringComparison.OrdinalIgnoreCase);
                if (matchIndex < 0)
                {
                    return -1;
                }

                var nameEndIndex = matchIndex + needle.Length;
                if (nameEndIndex >= text.Length || IsXmlNameBoundary(text[nameEndIndex]))
                {
                    return matchIndex;
                }

                searchIndex = matchIndex + 1;
            }

            return -1;
        }

        private static int IndexOfClosingElementEnd(string text, string elementName, int startIndex = 0)
        {
            var closingTagStartIndex = IndexOfClosingTagStart(text, elementName, startIndex);
            return closingTagStartIndex < 0
                ? -1
                : FindTagEnd(text, closingTagStartIndex);
        }

        private static int FindTagEnd(string text, int tagStartIndex)
        {
            return tagStartIndex < 0
                ? -1
                : text.IndexOf('>', tagStartIndex) is var tagEndIndex && tagEndIndex >= 0
                    ? tagEndIndex + 1
                    : -1;
        }

        private static bool IsXmlNameBoundary(char character)
        {
            return char.IsWhiteSpace(character) || character is '>' or '/' or '?';
        }
    }
}
