using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Xml.Linq;
using Ferrita.Controls.ChatSessionControl.Views;
using Ferrita.Infrastructure.Mvvm;
using Ferrita.Services.Localization;
using Ferrita.Services.FerritaTools;

namespace Ferrita.Tools
{
    public sealed class EditFileAdvancedTool :
        IFerritaTool,
        IFerritaToolConfigurationProvider,
        IFerritaToolInvocationPresentationProvider,
        IFerritaToolPromptDescriptionProvider
    {
        public const string ToolName = "EditFile_Advanced";

        private const int MaximumAnchorLines = 16;
        private const int DefaultIndentTabWidth = 4;
        private const string BeginningOfFileMarker = "[BOF]";
        private const string EndOfFileMarker = "[EOF]";

        private static readonly Regex s_editMarkerPattern = new(
            @"^[ \t]*\[Existing Code\](?:[ \t]*(?://[^\r\n]*|#[^\r\n]*|/\*[^\r\n]*\*/|<!--[^\r\n]*-->))?[ \t]*\r?$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline);

        private static readonly FerritaToolDefinition s_definition = BuildDefinition(new EditFileAdvancedToolSettings());

        private static readonly UTF8Encoding s_utf8WithoutBomStrict = new(false, true);
        private static readonly UnicodeEncoding s_utf16LittleEndianStrict = new(false, false, true);
        private static readonly UnicodeEncoding s_utf16BigEndianStrict = new(true, false, true);
        private static readonly UTF32Encoding s_utf32LittleEndianStrict = new(false, false, true);
        private static readonly UTF32Encoding s_utf32BigEndianStrict = new(true, false, true);

        private static readonly byte[] s_utf8Preamble = [0xEF, 0xBB, 0xBF];
        private static readonly byte[] s_utf16LittleEndianPreamble = [0xFF, 0xFE];
        private static readonly byte[] s_utf16BigEndianPreamble = [0xFE, 0xFF];
        private static readonly byte[] s_utf32LittleEndianPreamble = [0xFF, 0xFE, 0x00, 0x00];
        private static readonly byte[] s_utf32BigEndianPreamble = [0x00, 0x00, 0xFE, 0xFF];

        public FerritaToolDefinition Definition => s_definition;

        public FerritaToolDefinition GetEffectiveDefinition(FerritaToolConfigurationState configuration)
        {
            return BuildDefinition(EditFileAdvancedToolSettings.FromConfiguration(configuration));
        }

        public FerritaToolConfigurationPresenter? CreateConfigurationPresenter(FerritaToolConfigurationEditorContext context)
        {
            return new EditFileAdvancedToolConfigurationPresenter(context);
        }

        public string GetPromptDescription(FerritaToolPromptDescriptionContext context)
        {
            return BuildDescription(EditFileAdvancedToolSettings.FromConfiguration(context.ConfigurationState));
        }

        public FrameworkElement? CreateInvocationPresentation(FerritaToolInvocationPresentationContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            return ToolInvocationCardFactory.Create(
                context,
                [
                    new ToolInvocationCardFieldDefinition("File", "FilePath", "Waiting for file path..."),
                    new ToolInvocationCardFieldDefinition("Edit text", "EditText", "Waiting for anchored edit text..."),
                    new ToolInvocationCardFieldDefinition("Encoding", "Encoding", "Default utf-8 with safe auto-detection")
                ]);
        }

        public async Task<FerritaToolResult> ExecuteAsync(
            FerritaToolContext context,
            FerritaToolArguments arguments,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var settings = EditFileAdvancedToolSettings.FromConfiguration(context.CurrentToolConfiguration);
            var requestedPath = arguments.GetString("FilePath") ?? string.Empty;
            var rawEditText = arguments.GetString("EditText") ?? string.Empty;
            WriteTargetPath? targetPath = null;

            try
            {
                targetPath = ResolveWriteTargetPath(requestedPath, context.WorkspacePath, settings.PermissionScope);

                if (Directory.Exists(targetPath.ResolvedPath))
                {
                    return FerritaToolResult.Failure(
                        $"Path points to a directory, not a file: {targetPath.ResolvedPath}",
                        BuildData(targetPath, settings, null, null, null, null, null, 0, didWrite: false, blockSummaries: []));
                }

                if (!File.Exists(targetPath.ResolvedPath))
                {
                    return FerritaToolResult.Failure(
                        $"File not found: {targetPath.ResolvedPath}",
                        BuildData(targetPath, settings, null, null, null, null, null, 0, didWrite: false, blockSummaries: []));
                }

                var editText = NormalizeEditTextArgument(rawEditText);
                var editBlocks = ParseEditBlocks(editText);
                var originalBytes = await File.ReadAllBytesAsync(targetPath.ResolvedPath, cancellationToken).ConfigureAwait(false);
                var encodingDecision = ResolveEncodingDecision(originalBytes, GetExplicitEncodingName(arguments));
                var originalContent = DecodeContent(originalBytes, encodingDecision);
                var newline = DetectDominantNewline(originalContent);
                var currentContent = originalContent;
                var blockResults = new List<AppliedEditBlock>(editBlocks.Count);

                foreach (var block in editBlocks)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var application = ApplyEditBlock(currentContent, block, newline);
                    blockResults.Add(application);
                    currentContent = application.UpdatedContent;
                }

                if (string.Equals(currentContent, originalContent, StringComparison.Ordinal))
                {
                    return FerritaToolResult.Success(
                        BuildNoChangeContent(targetPath, settings, encodingDecision, originalBytes.LongLength, originalContent.Length, blockResults),
                        BuildData(
                            targetPath,
                            settings,
                            encodingDecision,
                            originalBytes.LongLength,
                            originalContent.Length,
                            originalBytes.LongLength,
                            currentContent.Length,
                            blockResults.Count,
                            didWrite: false,
                            blockSummaries: blockResults.Select(item => item.Summary).ToArray()));
                }

                var updatedBytes = EncodeContent(currentContent, encodingDecision);
                await File.WriteAllBytesAsync(targetPath.ResolvedPath, updatedBytes, cancellationToken).ConfigureAwait(false);
                var ragSync = await AerialCityRagToolSync.RefreshFileAsync(
                    targetPath.ResolvedPath,
                    context.WorkspacePath,
                    cancellationToken).ConfigureAwait(false);

                return FerritaToolResult.Success(
                    FerritaLineDiffPresentation.BuildContent(originalContent, currentContent),
                    AerialCityRagToolSync.WithSyncData(
                        BuildData(
                            targetPath,
                            settings,
                            encodingDecision,
                            originalBytes.LongLength,
                            originalContent.Length,
                            updatedBytes.LongLength,
                            currentContent.Length,
                            blockResults.Count,
                            didWrite: true,
                            blockSummaries: blockResults.Select(item => item.Summary).ToArray()),
                        ragSync),
                    FerritaToolResultPresentationHints.CreateLineDiff());
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex) when (IsExpectedException(ex))
            {
                return FerritaToolResult.Failure(
                    $"Failed to apply advanced file edit: {ex.Message}",
                    BuildData(targetPath, settings, null, null, null, null, null, 0, didWrite: false, blockSummaries: []));
            }
        }

        private static FerritaToolDefinition BuildDefinition(EditFileAdvancedToolSettings settings)
        {
            return new FerritaToolDefinition(
                ToolName,
                BuildDescription(settings),
                "Script",
                [
                    new FerritaToolParameterDefinition(
                        "FilePath",
                        "目标文件路径。当前工具常见配置是 LateralFileSystemOnly：这种配置下必须使用 LateralFS\\NodeName\\relative\\file.ext，或使用已经位于 LateralFS 虚拟根目录下的实际路径；不要用原始投影源路径。FullAccess 配置下也可使用普通绝对/相对路径。LateralFS 快捷路径会被解析到节点虚拟目录，并阻止 '..' 越界。",
                        FerritaToolParameterType.String,
                        isRequired: true),
                    new FerritaToolParameterDefinition(
                        "EditText",
                        BuildEditTextParameterDescription(),
                        FerritaToolParameterType.String,
                        isRequired: true),
                    new FerritaToolParameterDefinition(
                        "Encoding",
                        "Optional text encoding name. Default is utf-8. When utf-8 is requested, the tool safely auto-detects UTF BOMs plus strong UTF-16/UTF-32 zero-byte patterns. If bytes are not valid UTF-8 and cannot be identified, pass the correct encoding explicitly.",
                        FerritaToolParameterType.String,
                        isRequired: false,
                        defaultValue: "utf-8")
                ],
                defaultToolKitKeys: ["LegacyEditFile"]);
        }

        #pragma warning disable CS0162
        private static string BuildDescription(EditFileAdvancedToolSettings settings)
        {
            var permissionText = settings.PermissionScope == EditFileAdvancedPermissionScope.FullAccess
                ? "Permission: FullAccess, so the tool may write any file path that the process account can access."
                : "Permission: LateralFileSystemOnly, so the tool may write only inside LateralFS virtual folders. In this mode, use LateralFS\\NodeName\\relative\\file.ext or an actual path under a LateralFS virtual root; do not use the original projected source path.";

            return "Advanced existing-file editing tool. EditText is plain text, not a diff. " +
                "Primary protocol: use one or more full-line [Existing Code] ... [Existing Code] blocks. As a relaxed edge form, the first block may start with [BOF] instead of [Existing Code], and the last block may end with [EOF] instead of [Existing Code]. Inside each block, the first real line(s) are unchanged top anchors, the middle is the final edited text, and the last real line(s) are unchanged bottom anchors. [Existing Code] only marks block boundaries; it is never an anchor, and a standalone sentinel line may carry a trailing comment. " +
                "Single-block shorthand: if you omit [Existing Code] entirely, the whole EditText body is treated as one edit block. A one-line shorthand only rewrites an empty or one-line file. " +
                "You may use the literal marker [BOF] as the first line of a block to anchor at the beginning of the file, and/or [EOF] as the last line of a block to anchor at the end of the file. Those marker lines are virtual anchors and are not written into the file. Use [BOF] when editing the first line, [EOF] when editing the last line, and [BOF] plus [EOF] together when rewriting the whole file. " +
                "Prefer 2-5 exact unchanged context lines around the real edit when possible. The tool considers at most 16 anchor lines on each side. " +
                "If EditText may contain '<', '>', or '&' such as XML, HTML, or generic code, wrap the entire EditText value in CDATA in the outer XML tool call; the host/parser unwraps that CDATA automatically, so do not leave literal <![CDATA[ or ]]> markers inside the edit body. " +
                "Few-shot samples: first-line edit => [BOF]\\nnew first line\\nunchanged second line. Last-line edit => unchanged previous line\\nnew last line\\n[EOF]. Relaxed first block => [BOF]\\nnew first line\\nunchanged second line\\n[Existing Code]. Relaxed last block => [Existing Code]\\nunchanged previous line\\nnew last line\\n[EOF]. Multi-edit => [Existing Code]\\nunchanged top\\nnew middle\\nunchanged bottom\\n[Existing Code]\\n\\n[Existing Code]\\nsecond top\\nsecond new middle\\nsecond bottom\\n[Existing Code]. Whole-file rewrite => [BOF]\\nfull final file content\\n[EOF]. " +
                "Text outside complete edit blocks is ignored. The standard multi-block form is [Existing Code] ... [Existing Code], with [BOF] ... [Existing Code] and [Existing Code] ... [EOF] also accepted for the outermost first/last block. If anchors are missing, duplicated, or not unique, the tool fails without writing the file. " +
                "FilePath can be a LateralFS shortcut in the form LateralFS\\NodeName\\relative\\file.ext; the shortcut is resolved to that node's virtual folder before writing, and '..' traversal outside the node is rejected. " +
                permissionText;

            return "主文件编辑工具。用于对已存在文本文件做行级别编辑；不是 diff 工具，不使用 XML 编辑标签。 " +
                "EditText 是纯文本锚点协议：固定哨兵行必须是一整行 [Existing Code]。一个 EditText 可以一次包含一个或多个编辑块；块与块之间只允许空白。 " +
                "每个编辑块的唯一合法形态是：[Existing Code]\\n未修改的上锚点行\\n编辑后的最终内容行...\\n未修改的下锚点行\\n[Existing Code]。 " +
                "注意：[Existing Code] 只是块边界，不是锚点。真正的锚点是块内最前面和最后面的原文行；它们必须仍存在于文件中、逐字符相同、并且本次不能修改或删除。 " +
                "实际改动必须发生在这两侧锚点之间：替换时把中间写成最终文本；删除时省略要删除的行；插入时把新行放在上下锚点之间。 " +
                "为了避免歧义，优先选择更大的范围：在改动前后各放 2-5 行稳定且唯一的未修改原文，重复代码多时继续扩大；工具最多会利用每侧 16 行锚点。 " +
                "不要只给待修改的一行；每个块至少需要两行内容，也就是至少一个上锚点和一个下锚点。不能把会变化的行放在块首或块尾。 " +
                "删除多行时，下锚点必须位于所有被删除行之后；若要删除/修改相隔较远的多处内容，可以在同一个 EditText 中连续写多个 [Existing Code]... [Existing Code] 编辑块。 " +
                "外层调用仍是 XML；只要 EditText 中可能出现 '<'、'>'、'&'、项目文件 XML、HTML 或泛型代码，就把整个 EditText 参数放进 CDATA。 " +
                "推荐调用形态：<EditText><![CDATA[[Existing Code]\\n未修改的上锚点行\\n编辑后的最终内容\\n未修改的下锚点行\\n[Existing Code]\\n\\n[Existing Code]\\n第二处未修改的上锚点行\\n第二处编辑后的最终内容\\n第二处未修改的下锚点行\\n[Existing Code]]]></EditText>。 " +
                "如果锚点缺失、重复、不唯一、空白不一致，工具会失败且不会写文件。 " +
                "FilePath can be a LateralFS shortcut in the form LateralFS\\NodeName\\relative\\file.ext; the shortcut is resolved to that node's virtual folder before writing, and '..' traversal outside the node is rejected. " +
                permissionText;
        }

        private static string BuildEditTextParameterDescription()
        {
            return "Anchored final-text editing parameter. EditText is plain text, not a diff. " +
                "Preferred form: one or more [Existing Code] ... [Existing Code] blocks. Extra prose outside complete blocks is ignored. Relaxed edge forms [BOF] ... [Existing Code] and [Existing Code] ... [EOF] are also accepted for the outermost first/last block. Trailing comments after a standalone sentinel line are allowed. Shorthand fallback: if no [Existing Code] sentinel appears, the entire EditText body becomes one edit block; a one-line shorthand only rewrites an empty or one-line file. " +
                "Within a block, the default form is unchanged top anchor line(s), final edited content, unchanged bottom anchor line(s). To edit the first line, you may use [BOF] as the first line inside the block. To edit the last line, you may use [EOF] as the last line inside the block. [BOF] and [EOF] are virtual file-boundary anchors and are not written into the file. " +
                "Without [BOF] or [EOF], the first and last lines inside the block must be unchanged original lines. Prefer 2-5 exact unchanged context lines when possible; repeated code may require larger anchors. " +
                "Few-shot samples: [BOF]\\nnew first line\\nunchanged second line ; unchanged previous line\\nnew last line\\n[EOF] ; [BOF]\\nnew first line\\nunchanged second line\\n[Existing Code] ; [Existing Code]\\nunchanged previous line\\nnew last line\\n[EOF] ; [BOF]\\nfull final file content\\n[EOF]. " +
                "If the content contains '<', '>', or '&', especially XML, HTML, or generic code, wrap the entire EditText value in CDATA in the outer XML tool call. The host/parser unwraps the outer CDATA automatically.";

            return "锚点式最终文本编辑参数。EditText 是纯文本，不是 diff，不要写 XML 编辑标签。固定哨兵行是一整行 [Existing Code]。 " +
                "一个 EditText 可以一次包含多个编辑块，每个块都是一对 [Existing Code] 哨兵包住若干行最终文本；块外只能有空白。 " +
                "每个块内，第一行必须是未修改的原文上锚点，最后一行必须是未修改的原文下锚点；这两行用于定位，不能编辑、删除或改空白。 " +
                "中间写编辑后的最终内容：替换就写替换后的行，删除就省略被删行，插入就把新增行放在两个锚点之间。 " +
                "不要只提供要修改的单行；至少提供上锚点 + 下锚点。优先在改动前后各包含 2-5 行稳定且唯一的未修改原文，重复代码多时扩大范围。 " +
                "删除多行时，下锚点必须放在所有被删除行之后；多个相距较远的修改请在同一个 EditText 中写多个完整编辑块。 " +
                "外层工具调用是 XML；如果内容包含 '<'、'>' 或 '&'，尤其是 XML/HTML/泛型代码，请把整个 EditText 参数包进 CDATA。";
        }

        #pragma warning restore CS0162
        private static WriteTargetPath ResolveWriteTargetPath(
            string requestedPath,
            string? workspacePath,
            EditFileAdvancedPermissionScope permissionScope)
        {
            ToolFileSystemHelper.LateralFileSystemPathResolution? lateralResolution = null;
            string resolvedPath;

            if (ToolFileSystemHelper.TryResolveLateralFileSystemShortcut(requestedPath, out var shortcutResolution))
            {
                lateralResolution = shortcutResolution;
                resolvedPath = shortcutResolution.ResolvedPath;
            }
            else
            {
                resolvedPath = ToolFileSystemHelper.ResolvePath(requestedPath, workspacePath);
                if (ToolFileSystemHelper.TryGetContainingLateralFileSystemNode(resolvedPath, out var containingResolution))
                {
                    lateralResolution = containingResolution;
                }
            }

            if (permissionScope == EditFileAdvancedPermissionScope.LateralFileSystemOnly &&
                lateralResolution == null)
            {
                throw new InvalidOperationException(
                    "This tool is configured as LateralFileSystemOnly. FilePath must resolve inside a LateralFS virtual folder. Use LateralFS\\NodeName\\relative\\file.ext or an actual path under a LateralFS virtual root; do not use the original projected source path.");
            }

            return new WriteTargetPath(
                resolvedPath,
                lateralResolution?.NodeName,
                lateralResolution?.NodeId,
                lateralResolution?.NodeVirtualRootPath,
                lateralResolution?.RelativePath,
                lateralResolution?.UsedShortcut ?? false);
        }

        private static string NormalizeEditTextArgument(string rawEditText)
        {
            var normalized = rawEditText?.Trim() ?? string.Empty;
            if (normalized.Length == 0)
            {
                throw new InvalidOperationException("EditText cannot be empty.");
            }

            normalized = UnwrapOuterCData(normalized);

            if (TryExtractEditTextElementPayload(normalized, out var extracted))
            {
                return extracted;
            }

            var unescaped = UnescapeCommonXmlEntities(normalized);
            if (!string.Equals(unescaped, normalized, StringComparison.Ordinal) &&
                TryExtractEditTextElementPayload(unescaped, out extracted))
            {
                return extracted;
            }

            return UnwrapOuterCData(unescaped);
        }

        private static IReadOnlyList<EditBlock> ParseEditBlocks(string editText)
        {
            editText = NormalizeRelaxedBoundarySentinels(editText);
            var matches = s_editMarkerPattern.Matches(editText);
            if (matches.Count == 0)
            {
                return ParseSingleBlockFallback(editText);
            }

            if (matches.Count % 2 != 0)
            {
                throw new InvalidOperationException(BuildUnmatchedSentinelMessage(editText));
            }

            var blocks = new List<EditBlock>(matches.Count / 2);
            for (var index = 0; index < matches.Count; index += 2)
            {
                var openMarker = matches[index];
                var closeMarker = matches[index + 1];

                var blockStart = openMarker.Index + openMarker.Length;
                var blockLength = closeMarker.Index - blockStart;
                var blockText = TrimSingleBoundaryLineBreak(editText.Substring(blockStart, blockLength));
                if (string.IsNullOrWhiteSpace(blockText))
                {
                    throw new InvalidOperationException("Edit block cannot be empty.");
                }

                blocks.Add(ParseEditBlock(blocks.Count + 1, blockText));
            }

            return blocks;
        }

        private static string NormalizeRelaxedBoundarySentinels(string editText)
        {
            if (string.IsNullOrEmpty(editText))
            {
                return editText;
            }

            var split = SplitLines(editText);
            if (split.Lines.Count == 0 || !split.LineTexts.Any(IsExistingCodeSentinelLine))
            {
                return editText;
            }

            var firstContentLineIndex = FindFirstNonWhitespaceLineIndex(split.Lines);
            var lastContentLineIndex = FindLastNonWhitespaceLineIndex(split.Lines);
            if (firstContentLineIndex < 0 || lastContentLineIndex < 0)
            {
                return editText;
            }

            var rewriteStart = IsBeginningOfFileMarkerLine(split.Lines[firstContentLineIndex].Text);
            var rewriteEnd = IsEndOfFileMarkerLine(split.Lines[lastContentLineIndex].Text);
            if (!rewriteStart && !rewriteEnd)
            {
                return editText;
            }

            var newline = DetectDominantNewline(editText);
            var builder = new StringBuilder(editText.Length + 32);
            for (var index = 0; index < split.Lines.Count; index++)
            {
                var line = split.Lines[index];
                if (rewriteStart && index == firstContentLineIndex)
                {
                    AppendInsertedExistingCodeSentinel(builder, line.Newline, newline);
                }

                builder.Append(line.Text);
                builder.Append(line.Newline);

                if (rewriteEnd && index == lastContentLineIndex)
                {
                    if (line.Newline.Length == 0)
                    {
                        builder.Append(newline);
                    }

                    builder.Append("[Existing Code]");
                }
            }

            return builder.ToString();
        }

        private static void AppendInsertedExistingCodeSentinel(
            StringBuilder builder,
            string preferredNewline,
            string fallbackNewline)
        {
            builder.Append("[Existing Code]");
            builder.Append(preferredNewline.Length > 0 ? preferredNewline : fallbackNewline);
        }

        private static int FindFirstNonWhitespaceLineIndex(IReadOnlyList<TextLine> lines)
        {
            for (var index = 0; index < lines.Count; index++)
            {
                if (!string.IsNullOrWhiteSpace(lines[index].Text))
                {
                    return index;
                }
            }

            return -1;
        }

        private static int FindLastNonWhitespaceLineIndex(IReadOnlyList<TextLine> lines)
        {
            for (var index = lines.Count - 1; index >= 0; index--)
            {
                if (!string.IsNullOrWhiteSpace(lines[index].Text))
                {
                    return index;
                }
            }

            return -1;
        }

        private static IReadOnlyList<EditBlock> ParseSingleBlockFallback(string editText)
        {
            var blockText = TrimSingleBoundaryLineBreak(editText.Trim());
            if (string.IsNullOrWhiteSpace(blockText))
            {
                throw new InvalidOperationException("EditText cannot be empty.");
            }

            try
            {
                var block = ParseEditBlock(1, blockText);
                ValidateReplacementLineCount(block, SplitLines(block.FinalText).Lines.Count);
                return [block];
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException(BuildFallbackBlockFailureMessage(editText, ex.Message));
            }
        }

        private static AppliedEditBlock ApplyEditBlock(string content, EditBlock block, string newline)
        {
            var fileLines = SplitLines(content);
            var replacementLines = SplitLines(block.FinalText);
            var replacementLineTexts = replacementLines.Lines.Select(line => line.Text).ToArray();
            ValidateReplacementLineCount(block, replacementLines.Lines.Count);
            var match = ResolveUniqueBlockMatch(
                fileLines.LineTexts,
                replacementLineTexts,
                block.AnchoredAtBeginningOfFile,
                block.AnchoredAtEndOfFile,
                block.Index);
            var replacementText = BuildReplacementText(
                fileLines.Lines,
                replacementLines.Lines,
                match,
                newline);
            var startOffset = GetLineStartOffset(fileLines.Lines, match.StartLineIndex);
            var endOffset = GetLineEndOffset(fileLines.Lines, match.EndLineExclusive);
            var originalSpan = BuildTextFromLines(fileLines.Lines, match.StartLineIndex, match.EndLineExclusive);
            replacementText = PreserveOriginalTrailingLineBreak(replacementText, fileLines.Lines, match);
            var updatedContent = content[..startOffset] + replacementText + content[endOffset..];
            var changed = !string.Equals(originalSpan, replacementText, StringComparison.Ordinal);
            var summaryStartLine = fileLines.Lines.Count == 0 ? 0 : match.StartLineIndex + 1;

            return new AppliedEditBlock(
                updatedContent,
                new AppliedEditBlockSummary(
                    block.Index,
                    summaryStartLine,
                    match.EndLineExclusive,
                    match.PrefixLineCount,
                    match.SuffixLineCount,
                    match.EndLineExclusive - match.StartLineIndex,
                    replacementLines.Lines.Count,
                    changed));
        }

        private static BlockMatch ResolveUniqueBlockMatch(
            IReadOnlyList<string> fileLineTexts,
            IReadOnlyList<string> replacementLineTexts,
            bool anchoredAtBeginningOfFile,
            bool anchoredAtEndOfFile,
            int blockIndex)
        {
            if (!anchoredAtBeginningOfFile &&
                !anchoredAtEndOfFile &&
                replacementLineTexts.Count == 1 &&
                fileLineTexts.Count <= 1)
            {
                return new BlockMatch(0, fileLineTexts.Count, 0, 0, false, LineMatchMode.Exact);
            }

            foreach (var lineMatchMode in new[]
                     {
                         LineMatchMode.Exact,
                         LineMatchMode.TrimEnd,
                         LineMatchMode.IndentNormalized
                     })
            {
                var resolution = TryResolveStandardBlockMatch(
                    fileLineTexts,
                    replacementLineTexts,
                    anchoredAtBeginningOfFile,
                    anchoredAtEndOfFile,
                    blockIndex,
                    lineMatchMode);

                if (resolution.Match != null)
                {
                    return resolution.Match;
                }

                if (resolution.IsAmbiguous)
                {
                    throw new InvalidOperationException(
                        BuildAmbiguousBlockMessage(blockIndex, resolution.CandidateMatches, fileLineTexts, lineMatchMode));
                }
            }

            if (TryResolveWholeFileShorthandMatch(fileLineTexts, replacementLineTexts, anchoredAtBeginningOfFile, anchoredAtEndOfFile, out var shorthandMatch))
            {
                return shorthandMatch;
            }

            throw new InvalidOperationException(
                BuildCouldNotLocateBlockMessage(
                    blockIndex,
                    fileLineTexts,
                    replacementLineTexts,
                    anchoredAtBeginningOfFile,
                    anchoredAtEndOfFile));
        }

        private static BlockMatchResolution TryResolveStandardBlockMatch(
            IReadOnlyList<string> fileLineTexts,
            IReadOnlyList<string> replacementLineTexts,
            bool anchoredAtBeginningOfFile,
            bool anchoredAtEndOfFile,
            int blockIndex,
            LineMatchMode lineMatchMode)
        {
            var replacementLineCount = replacementLineTexts.Count;
            var minimumPrefixLines = anchoredAtBeginningOfFile ? 0 : 1;
            var minimumSuffixLines = anchoredAtEndOfFile ? 0 : 1;
            var maximumPrefixLines = Math.Min(MaximumAnchorLines, replacementLineCount - minimumSuffixLines);
            var uniqueCandidatesBySpan = new Dictionary<(int Start, int End), BlockMatch>();
            var ambiguousCandidatesBySpan = new Dictionary<(int Start, int End), BlockMatch>();
            var bestUniqueAnchorLineScore = -1;
            var bestAmbiguousAnchorLineScore = -1;

            for (var prefixLineCount = maximumPrefixLines; prefixLineCount >= minimumPrefixLines; prefixLineCount--)
            {
                var maximumSuffixLines = Math.Min(MaximumAnchorLines, replacementLineCount - prefixLineCount);
                for (var suffixLineCount = maximumSuffixLines; suffixLineCount >= minimumSuffixLines; suffixLineCount--)
                {
                    var matches = FindMatches(
                        fileLineTexts,
                        replacementLineTexts,
                        prefixLineCount,
                        suffixLineCount,
                        anchoredAtBeginningOfFile,
                        anchoredAtEndOfFile,
                        lineMatchMode);

                    if (matches.Count == 1)
                    {
                        var anchorLineScore = prefixLineCount + suffixLineCount;
                        if (anchorLineScore < bestUniqueAnchorLineScore)
                        {
                            continue;
                        }

                        if (anchorLineScore > bestUniqueAnchorLineScore)
                        {
                            uniqueCandidatesBySpan.Clear();
                            bestUniqueAnchorLineScore = anchorLineScore;
                        }

                        var match = matches[0];
                        uniqueCandidatesBySpan.TryAdd(
                            (match.StartLineIndex, match.EndLineExclusive),
                            match with
                            {
                                LineMatchMode = lineMatchMode,
                                PreserveAnchors = true
                            });
                        continue;
                    }

                    if (matches.Count > 1)
                    {
                        var anchorLineScore = prefixLineCount + suffixLineCount;
                        if (anchorLineScore < bestAmbiguousAnchorLineScore)
                        {
                            continue;
                        }

                        if (anchorLineScore > bestAmbiguousAnchorLineScore)
                        {
                            ambiguousCandidatesBySpan.Clear();
                            bestAmbiguousAnchorLineScore = anchorLineScore;
                        }

                        foreach (var match in matches)
                        {
                            ambiguousCandidatesBySpan.TryAdd(
                                (match.StartLineIndex, match.EndLineExclusive),
                                match with
                                {
                                    LineMatchMode = lineMatchMode,
                                    PreserveAnchors = true
                                });
                        }
                    }
                }
            }

            if (uniqueCandidatesBySpan.Count == 1)
            {
                return new BlockMatchResolution(uniqueCandidatesBySpan.Values.Single(), Array.Empty<BlockMatch>());
            }

            if (uniqueCandidatesBySpan.Count > 1)
            {
                return new BlockMatchResolution(null, uniqueCandidatesBySpan.Values.ToArray());
            }

            if (ambiguousCandidatesBySpan.Count > 0)
            {
                return new BlockMatchResolution(null, ambiguousCandidatesBySpan.Values.ToArray());
            }

            return new BlockMatchResolution(null, Array.Empty<BlockMatch>());
        }

        private static bool TryResolveWholeFileShorthandMatch(
            IReadOnlyList<string> fileLineTexts,
            IReadOnlyList<string> replacementLineTexts,
            bool anchoredAtBeginningOfFile,
            bool anchoredAtEndOfFile,
            out BlockMatch match)
        {
            if (anchoredAtBeginningOfFile || anchoredAtEndOfFile || replacementLineTexts.Count != 1)
            {
                match = null!;
                return false;
            }

            if (fileLineTexts.Count > 1)
            {
                match = null!;
                return false;
            }

            match = new BlockMatch(0, fileLineTexts.Count, 0, 0, false, LineMatchMode.Exact);
            return true;
        }

        private static List<BlockMatch> FindMatches(
            IReadOnlyList<string> fileLineTexts,
            IReadOnlyList<string> replacementLineTexts,
            int prefixLineCount,
            int suffixLineCount,
            bool anchoredAtBeginningOfFile,
            bool anchoredAtEndOfFile,
            LineMatchMode lineMatchMode)
        {
            var matches = new List<BlockMatch>();
            var prefix = replacementLineTexts.Take(prefixLineCount).ToArray();
            var suffix = replacementLineTexts.Skip(replacementLineTexts.Count - suffixLineCount).ToArray();

            var maximumStart = anchoredAtBeginningOfFile
                ? 0
                : fileLineTexts.Count - prefixLineCount;

            for (var start = 0; start <= maximumStart; start++)
            {
                if (prefixLineCount > 0 && !SequenceMatches(fileLineTexts, start, prefix, lineMatchMode))
                {
                    continue;
                }

                if (anchoredAtEndOfFile)
                {
                    var suffixStart = fileLineTexts.Count - suffixLineCount;
                    if (suffixStart < start + prefixLineCount)
                    {
                        continue;
                    }

                    if (suffixLineCount > 0 && !SequenceMatches(fileLineTexts, suffixStart, suffix, lineMatchMode))
                    {
                        continue;
                    }

                    matches.Add(new BlockMatch(
                        start,
                        suffixStart + suffixLineCount,
                        prefixLineCount,
                        suffixLineCount,
                        true,
                        lineMatchMode));
                    continue;
                }

                var suffixSearchStart = start + prefixLineCount;
                for (var suffixStart = suffixSearchStart; suffixStart <= fileLineTexts.Count - suffixLineCount; suffixStart++)
                {
                    if (suffixLineCount > 0 && !SequenceMatches(fileLineTexts, suffixStart, suffix, lineMatchMode))
                    {
                        continue;
                    }

                    matches.Add(new BlockMatch(
                        start,
                        suffixStart + suffixLineCount,
                        prefixLineCount,
                        suffixLineCount,
                        true,
                        lineMatchMode));
                }
            }

            return matches;
        }

        private static bool SequenceMatches(
            IReadOnlyList<string> lines,
            int startIndex,
            IReadOnlyList<string> expected,
            LineMatchMode lineMatchMode)
        {
            if (startIndex < 0 || startIndex + expected.Count > lines.Count)
            {
                return false;
            }

            for (var index = 0; index < expected.Count; index++)
            {
                if (!LineMatches(lines[startIndex + index], expected[index], lineMatchMode))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool LineMatches(string source, string expected, LineMatchMode lineMatchMode)
        {
            return lineMatchMode switch
            {
                LineMatchMode.Exact => string.Equals(source, expected, StringComparison.Ordinal),
                LineMatchMode.TrimEnd => string.Equals(source.TrimEnd(), expected.TrimEnd(), StringComparison.Ordinal),
                LineMatchMode.IndentNormalized => string.Equals(
                    NormalizeIndentationKey(source),
                    NormalizeIndentationKey(expected),
                    StringComparison.Ordinal),
                _ => string.Equals(source, expected, StringComparison.Ordinal)
            };
        }

        private static string NormalizeIndentationKey(string line)
        {
            if (line.Length == 0)
            {
                return string.Empty;
            }

            var index = 0;
            var indentationColumns = 0;
            while (index < line.Length)
            {
                var current = line[index];
                if (current == ' ')
                {
                    indentationColumns++;
                    index++;
                    continue;
                }

                if (current == '\t')
                {
                    indentationColumns += DefaultIndentTabWidth - indentationColumns % DefaultIndentTabWidth;
                    index++;
                    continue;
                }

                break;
            }

            return string.Concat(
                indentationColumns.ToString(CultureInfo.InvariantCulture),
                "\u0000",
                line[index..].TrimEnd());
        }

        private static SplitLineResult SplitLines(string text)
        {
            var lines = new List<TextLine>();
            var offset = 0;
            var index = 0;

            while (index < text.Length)
            {
                var lineStart = index;
                while (index < text.Length && text[index] is not ('\r' or '\n'))
                {
                    index++;
                }

                var lineText = text[lineStart..index];
                var newline = string.Empty;
                if (index < text.Length)
                {
                    if (text[index] == '\r' && index + 1 < text.Length && text[index + 1] == '\n')
                    {
                        newline = "\r\n";
                        index += 2;
                    }
                    else
                    {
                        newline = text[index].ToString();
                        index++;
                    }
                }

                lines.Add(new TextLine(lineText, newline, offset));
                offset = index;
            }

            if (text.Length == 0)
            {
                return new SplitLineResult([], []);
            }

            return new SplitLineResult(lines, lines.Select(line => line.Text).ToArray());
        }

        private static string BuildTextFromLines(IReadOnlyList<TextLine> lines, int startLineIndex, int endLineExclusive)
        {
            if (startLineIndex < 0 || endLineExclusive > lines.Count || startLineIndex >= endLineExclusive)
            {
                return string.Empty;
            }

            var startOffset = lines[startLineIndex].StartOffset;
            var endOffset = GetLineEndOffset(lines, endLineExclusive);
            var builder = new StringBuilder(endOffset - startOffset);
            for (var index = startLineIndex; index < endLineExclusive; index++)
            {
                builder.Append(lines[index].Text);
                builder.Append(lines[index].Newline);
            }

            return builder.ToString();
        }

        private static int GetLineStartOffset(IReadOnlyList<TextLine> lines, int startLineIndex)
        {
            if (startLineIndex <= 0 || lines.Count == 0)
            {
                return 0;
            }

            if (startLineIndex >= lines.Count)
            {
                return GetLineEndOffset(lines, lines.Count);
            }

            return lines[startLineIndex].StartOffset;
        }

        private static int GetLineEndOffset(IReadOnlyList<TextLine> lines, int endLineExclusive)
        {
            if (endLineExclusive <= 0)
            {
                return 0;
            }

            var lastLine = lines[endLineExclusive - 1];
            return lastLine.StartOffset + lastLine.Text.Length + lastLine.Newline.Length;
        }

        private static string NormalizeReplacementLineEndings(string text, string newline)
        {
            var split = SplitLines(text);
            if (split.Lines.Count == 0)
            {
                return string.Empty;
            }

            var builder = new StringBuilder(text.Length);
            for (var index = 0; index < split.Lines.Count; index++)
            {
                var line = split.Lines[index];
                builder.Append(line.Text);
                if (line.Newline.Length > 0)
                {
                    builder.Append(newline);
                }
            }

            return builder.ToString();
        }

        private static string PreserveOriginalTrailingLineBreak(
            string replacementText,
            IReadOnlyList<TextLine> fileLines,
            BlockMatch match)
        {
            if (replacementText.Length == 0)
            {
                return replacementText;
            }

            if (match.EndLineExclusive <= 0 || match.EndLineExclusive > fileLines.Count)
            {
                return replacementText;
            }

            var originalTrailingLineBreak = fileLines[match.EndLineExclusive - 1].Newline;
            if (originalTrailingLineBreak.Length == 0 || EndsWithLineBreak(replacementText))
            {
                return replacementText;
            }

            return replacementText + originalTrailingLineBreak;
        }

        private static bool EndsWithLineBreak(string text)
        {
            return text.EndsWith('\n') || text.EndsWith('\r');
        }

        private static string DetectDominantNewline(string text)
        {
            var crlf = 0;
            var lf = 0;
            var cr = 0;

            for (var index = 0; index < text.Length; index++)
            {
                if (text[index] == '\r')
                {
                    if (index + 1 < text.Length && text[index + 1] == '\n')
                    {
                        crlf++;
                        index++;
                    }
                    else
                    {
                        cr++;
                    }
                }
                else if (text[index] == '\n')
                {
                    lf++;
                }
            }

            if (crlf >= lf && crlf >= cr && crlf > 0)
            {
                return "\r\n";
            }

            if (lf >= cr && lf > 0)
            {
                return "\n";
            }

            if (cr > 0)
            {
                return "\r";
            }

            return Environment.NewLine;
        }

        private static string TrimSingleBoundaryLineBreak(string text)
        {
            var result = text;
            if (result.StartsWith("\r\n", StringComparison.Ordinal))
            {
                result = result[2..];
            }
            else if (result.StartsWith('\n') || result.StartsWith('\r'))
            {
                result = result[1..];
            }

            if (result.EndsWith("\r\n", StringComparison.Ordinal))
            {
                result = result[..^2];
            }
            else if (result.EndsWith('\n') || result.EndsWith('\r'))
            {
                result = result[..^1];
            }

            return result;
        }

        private static EditBlock ParseEditBlock(int blockIndex, string blockText)
        {
            var split = SplitLines(blockText);
            var lineTexts = split.LineTexts;
            var anchoredAtBeginningOfFile = lineTexts.Count > 0 && IsBeginningOfFileMarkerLine(lineTexts[0]);
            var anchoredAtEndOfFile = lineTexts.Count > 0 && IsEndOfFileMarkerLine(lineTexts[^1]);

            ValidateBoundaryMarkers(blockIndex, lineTexts, anchoredAtBeginningOfFile, anchoredAtEndOfFile);

            var startLineIndex = anchoredAtBeginningOfFile ? 1 : 0;
            var endLineExclusive = split.Lines.Count - (anchoredAtEndOfFile ? 1 : 0);
            var finalText = BuildBlockFinalText(split.Lines, startLineIndex, endLineExclusive);

            return new EditBlock(blockIndex, finalText, anchoredAtBeginningOfFile, anchoredAtEndOfFile);
        }

        private static void ValidateBoundaryMarkers(
            int blockIndex,
            IReadOnlyList<string> lineTexts,
            bool anchoredAtBeginningOfFile,
            bool anchoredAtEndOfFile)
        {
            for (var index = 0; index < lineTexts.Count; index++)
            {
                var lineText = lineTexts[index];
                if (IsBeginningOfFileMarkerLine(lineText) &&
                    (!anchoredAtBeginningOfFile || index != 0))
                {
                    throw new InvalidOperationException(
                        $"Edit block {blockIndex} uses {BeginningOfFileMarker} in an invalid position. {BeginningOfFileMarker} is only allowed as the first line inside a block.");
                }

                if (IsEndOfFileMarkerLine(lineText) &&
                    (!anchoredAtEndOfFile || index != lineTexts.Count - 1))
                {
                    throw new InvalidOperationException(
                        $"Edit block {blockIndex} uses {EndOfFileMarker} in an invalid position. {EndOfFileMarker} is only allowed as the last line inside a block.");
                }
            }
        }

        private static string BuildBlockFinalText(IReadOnlyList<TextLine> lines, int startLineIndex, int endLineExclusive)
        {
            if (startLineIndex >= endLineExclusive)
            {
                return string.Empty;
            }

            var builder = new StringBuilder();
            for (var index = startLineIndex; index < endLineExclusive; index++)
            {
                builder.Append(lines[index].Text);
                if (index + 1 < endLineExclusive)
                {
                    builder.Append(lines[index].Newline);
                }
            }

            return builder.ToString();
        }

        private static void ValidateReplacementLineCount(EditBlock block, int replacementLineCount)
        {
            if (block.AnchoredAtBeginningOfFile && block.AnchoredAtEndOfFile)
            {
                return;
            }

            if (block.AnchoredAtBeginningOfFile || block.AnchoredAtEndOfFile)
            {
                if (replacementLineCount == 0)
                {
                    throw new InvalidOperationException(
                        $"Edit block {block.Index} uses a file-boundary marker and must still include at least one real line of final text or unchanged anchor text.");
                }

                return;
            }

            if (replacementLineCount == 0)
            {
                throw new InvalidOperationException(
                    $"Edit block {block.Index} cannot be empty. Use {BeginningOfFileMarker} plus {EndOfFileMarker} when intentionally rewriting a whole file to empty content.");
            }
        }

        private static bool StartsWithElement(string text, string elementName)
        {
            var trimmed = text.TrimStart();
            var prefix = $"<{elementName}";
            if (!trimmed.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            return trimmed.Length == prefix.Length || char.IsWhiteSpace(trimmed[prefix.Length]) || trimmed[prefix.Length] is '>' or '/';
        }

        private static bool TryExtractEditTextElementPayload(string text, out string payload)
        {
            payload = string.Empty;
            if (!StartsWithElement(text, "EditText"))
            {
                return false;
            }

            try
            {
                var element = XElement.Parse(text, LoadOptions.PreserveWhitespace);
                if (!string.Equals(element.Name.LocalName, "EditText", StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }

                payload = UnwrapOuterCData(UnescapeCommonXmlEntities(ExtractElementText(element).Trim()));
                return true;
            }
            catch (Exception ex) when (ex is InvalidOperationException or System.Xml.XmlException)
            {
                return false;
            }
        }

        private static string UnescapeCommonXmlEntities(string text)
        {
            if (!ContainsCommonXmlEntity(text))
            {
                return text;
            }

            var current = text;
            for (var pass = 0; pass < 3; pass++)
            {
                var next = current
                    .Replace("&lt;", "<", StringComparison.OrdinalIgnoreCase)
                    .Replace("&gt;", ">", StringComparison.OrdinalIgnoreCase)
                    .Replace("&quot;", "\"", StringComparison.OrdinalIgnoreCase)
                    .Replace("&apos;", "'", StringComparison.OrdinalIgnoreCase)
                    .Replace("&amp;", "&", StringComparison.OrdinalIgnoreCase);

                if (string.Equals(next, current, StringComparison.Ordinal))
                {
                    break;
                }

                current = next;
                if (!ContainsCommonXmlEntity(current))
                {
                    break;
                }
            }

            return current;
        }

        private static bool ContainsCommonXmlEntity(string text)
        {
            return text.Contains("&lt;", StringComparison.OrdinalIgnoreCase) ||
                text.Contains("&gt;", StringComparison.OrdinalIgnoreCase) ||
                text.Contains("&amp;", StringComparison.OrdinalIgnoreCase) ||
                text.Contains("&quot;", StringComparison.OrdinalIgnoreCase) ||
                text.Contains("&apos;", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsMarkerLine(string line, string marker)
        {
            var trimmed = line.Trim();
            if (!trimmed.StartsWith(marker, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            if (trimmed.Length == marker.Length)
            {
                return true;
            }

            var suffix = trimmed[marker.Length..].TrimStart();
            return suffix.StartsWith("//", StringComparison.Ordinal) ||
                suffix.StartsWith("#", StringComparison.Ordinal) ||
                suffix.StartsWith("/*", StringComparison.Ordinal) ||
                suffix.StartsWith("<!--", StringComparison.Ordinal);
        }

        private static bool IsBeginningOfFileMarkerLine(string line)
        {
            return IsMarkerLine(line, BeginningOfFileMarker);
        }

        private static bool IsEndOfFileMarkerLine(string line)
        {
            return IsMarkerLine(line, EndOfFileMarker);
        }

        private static string ExtractElementText(XElement element)
        {
            var builder = new StringBuilder();
            foreach (var node in element.Nodes())
            {
                switch (node)
                {
                    case XCData cdata:
                        builder.Append(cdata.Value);
                        break;
                    case XText text:
                        builder.Append(text.Value);
                        break;
                    default:
                        builder.Append(node.ToString(SaveOptions.DisableFormatting));
                        break;
                }
            }

            return builder.ToString();
        }

        private static string BuildReplacementText(
            IReadOnlyList<TextLine> fileLines,
            IReadOnlyList<TextLine> replacementLines,
            BlockMatch match,
            string newline)
        {
            if (replacementLines.Count == 0)
            {
                return string.Empty;
            }

            var builder = new StringBuilder();
            var suffixReplacementStart = replacementLines.Count - match.SuffixLineCount;
            for (var index = 0; index < replacementLines.Count; index++)
            {
                var lineText = replacementLines[index].Text;
                if (match.PreserveAnchors)
                {
                    if (index < match.PrefixLineCount)
                    {
                        var fileLineIndex = match.StartLineIndex + index;
                        if (fileLineIndex >= 0 && fileLineIndex < fileLines.Count)
                        {
                            lineText = fileLines[fileLineIndex].Text;
                        }
                    }
                    else if (index >= suffixReplacementStart)
                    {
                        var suffixOffset = index - suffixReplacementStart;
                        var fileLineIndex = match.EndLineExclusive - match.SuffixLineCount + suffixOffset;
                        if (fileLineIndex >= 0 && fileLineIndex < fileLines.Count)
                        {
                            lineText = fileLines[fileLineIndex].Text;
                        }
                    }
                }

                builder.Append(lineText);
                if (replacementLines[index].Newline.Length > 0)
                {
                    builder.Append(newline);
                }
            }

            return builder.ToString();
        }

        private static string UnwrapOuterCData(string text)
        {
            var current = text.Trim();
            while (TryUnwrapOuterCData(current, out var innerText))
            {
                current = innerText.Trim();
            }

            return current;
        }

        private static bool TryUnwrapOuterCData(string text, out string innerText)
        {
            const string cdataPrefix = "<![CDATA[";
            const string cdataSuffix = "]]>";

            if (text.StartsWith(cdataPrefix, StringComparison.Ordinal) &&
                text.EndsWith(cdataSuffix, StringComparison.Ordinal) &&
                text.Length >= cdataPrefix.Length + cdataSuffix.Length)
            {
                innerText = text[cdataPrefix.Length..^cdataSuffix.Length];
                return true;
            }

            innerText = string.Empty;
            return false;
        }

        private static bool IsWhitespaceOnly(string text, int startIndex, int endIndex)
        {
            for (var index = startIndex; index < endIndex; index++)
            {
                if (!char.IsWhiteSpace(text[index]))
                {
                    return false;
                }
            }

            return true;
        }

        private static string BuildUnmatchedSentinelMessage(string editText)
        {
            if (ContainsNonStandaloneExistingCodeLine(editText))
            {
                return "EditText contains [Existing Code] on a non-sentinel line. [Existing Code] must appear alone on its own line, but it may carry a trailing comment such as // note. Put [BOF], [EOF], or any real code on the next/previous line instead. Example: [Existing Code]\\n[BOF]\\n...\\n[Existing Code]. If you only need one block, you may also omit [Existing Code] entirely and let the whole EditText body act as the block.";
            }

            return "EditText contains unmatched edit-block sentinels. The standard multi-block form is [Existing Code] ... [Existing Code]. The outermost first block may also start with [BOF], and the outermost last block may also end with [EOF]. Commentary outside complete blocks is ignored, but every sentinel must still be paired. If you only need one block, you may omit [Existing Code] entirely and use the whole EditText body as a single shorthand block.";
        }

        private static string BuildOutsideBlockTextMessage(string editText)
        {
            if (ContainsLiteralCDataMarker(editText))
            {
                return "Text outside complete [Existing Code] blocks is ignored, but literal <![CDATA[ or ]]> markers were found in EditText. CDATA should wrap the outer XML parameter only; do not leave the CDATA markers themselves inside the edit body.";
            }

            if (ContainsNonStandaloneExistingCodeLine(editText))
            {
                return "[Existing Code] must be alone on its own line; do not combine it with [BOF], [EOF], or real code on the same line.";
            }

            return "Text outside complete edit blocks is ignored. Put all source/final text inside complete blocks; the standard multi-block form is [Existing Code] ... [Existing Code], and the outermost first/last block may also use [BOF] ... [Existing Code] or [Existing Code] ... [EOF]. If you only need one block, you may omit [Existing Code] entirely and let the whole EditText body be that block.";
        }

        private static string BuildFallbackBlockFailureMessage(string editText, string reason)
        {
            if (ContainsNonStandaloneExistingCodeLine(editText))
            {
                return $"{reason} [Existing Code] was found, but not as a standalone sentinel line. Put [Existing Code] on its own line, or omit it entirely and use the whole EditText body as one shorthand block.";
            }

            if (ContainsLiteralCDataMarker(editText))
            {
                return $"{reason} Literal <![CDATA[ or ]]> markers were found in EditText. CDATA should wrap the outer XML parameter only; do not keep those markers inside the edit body.";
            }

            return $"{reason} No complete edit block was found, so the whole EditText body was treated as one shorthand block. A one-line shorthand only rewrites an empty or one-line file; for multi-line files, add unchanged anchor lines or use [BOF] / [EOF] for file-boundary edits. For multiple distant edits, wrap each block in matched sentinels: usually [Existing Code] ... [Existing Code], with [BOF] ... [Existing Code] and [Existing Code] ... [EOF] also accepted for the outermost first/last block.";
        }

        private static bool ContainsNonStandaloneExistingCodeLine(string text)
        {
            foreach (var line in SplitLines(text).LineTexts)
            {
                if (line.IndexOf("[Existing Code]", StringComparison.OrdinalIgnoreCase) >= 0 &&
                    !IsExistingCodeSentinelLine(line))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsExistingCodeSentinelLine(string line)
        {
            return IsMarkerLine(line, "[Existing Code]");
        }

        private static bool ContainsLiteralCDataMarker(string text)
        {
            return text.Contains("<![CDATA[", StringComparison.Ordinal) ||
                text.Contains("]]>", StringComparison.Ordinal);
        }

        private static string BuildCouldNotLocateBlockMessage(
            int blockIndex,
            IReadOnlyList<string> fileLineTexts,
            IReadOnlyList<string> replacementLineTexts,
            bool anchoredAtBeginningOfFile,
            bool anchoredAtEndOfFile)
        {
            var builder = new StringBuilder();
            builder.Append($"Edit block {blockIndex} could not be uniquely located. ");
            builder.Append("Tried exact, trim-end, and tab/space-normalized anchor matching. ");

            if (!anchoredAtBeginningOfFile &&
                !anchoredAtEndOfFile &&
                replacementLineTexts.Count == 1 &&
                fileLineTexts.Count > 1)
            {
                builder.Append("This one-line shorthand only rewrites an empty or one-line file. ");
            }

            var closestReport = BuildClosestMatchReport(
                fileLineTexts,
                replacementLineTexts,
                anchoredAtBeginningOfFile,
                anchoredAtEndOfFile);
            if (closestReport.Length > 0)
            {
                builder.Append(closestReport);
                builder.Append(' ');
            }

            builder.Append("Add more exact unchanged context around the real edit and make sure changed or deleted lines stay between the anchors.");
            return builder.ToString();
        }

        private static string BuildAmbiguousBlockMessage(
            int blockIndex,
            IReadOnlyList<BlockMatch> candidateMatches,
            IReadOnlyList<string> fileLineTexts,
            LineMatchMode lineMatchMode)
        {
            var orderedMatches = candidateMatches
                .OrderBy(match => match.StartLineIndex)
                .ThenBy(match => match.EndLineExclusive)
                .Take(5)
                .ToArray();

            var spans = string.Join(
                ", ",
                orderedMatches.Select(match => $"{FormatLineRange(match.StartLineIndex, match.EndLineExclusive)}: {BuildSpanPreview(fileLineTexts, match)}"));
            var moreText = candidateMatches.Count > orderedMatches.Length
                ? $" (+{candidateMatches.Count - orderedMatches.Length} more)"
                : string.Empty;

            return $"Edit block {blockIndex} is ambiguous under {FormatLineMatchMode(lineMatchMode)} matching. Candidate spans: {spans}{moreText}. Add more unchanged context above and below the real edit until the target span is unique.";
        }

        private static string BuildClosestMatchReport(
            IReadOnlyList<string> fileLineTexts,
            IReadOnlyList<string> replacementLineTexts,
            bool anchoredAtBeginningOfFile,
            bool anchoredAtEndOfFile)
        {
            var reports = new List<(int Score, string Message)>(2);
            var sampleLength = Math.Min(MaximumAnchorLines, Math.Min(5, replacementLineTexts.Count));
            if (sampleLength <= 0)
            {
                return string.Empty;
            }

            IReadOnlyList<string>? topSample = null;
            if (!anchoredAtBeginningOfFile)
            {
                topSample = replacementLineTexts.Take(sampleLength).ToArray();
                var topReport = FindClosestSequenceMatch(fileLineTexts, topSample);
                if (topReport != null)
                {
                    reports.Add((topReport.TotalScore, FormatClosestAnchorReport("top", topSample, topReport)));
                }
            }

            if (!anchoredAtEndOfFile)
            {
                var bottomSample = replacementLineTexts.Skip(replacementLineTexts.Count - sampleLength).ToArray();
                if (topSample == null || !topSample.SequenceEqual(bottomSample, StringComparer.Ordinal))
                {
                    var bottomReport = FindClosestSequenceMatch(fileLineTexts, bottomSample);
                    if (bottomReport != null)
                    {
                        reports.Add((bottomReport.TotalScore, FormatClosestAnchorReport("bottom", bottomSample, bottomReport)));
                    }
                }
            }

            if (reports.Count == 0)
            {
                return string.Empty;
            }

            return "Closest candidate: " + string.Join(" | ", reports.OrderBy(report => report.Score).Select(report => report.Message));
        }

        private static string FormatClosestAnchorReport(
            string label,
            IReadOnlyList<string> expectedLines,
            SequenceComparisonResult comparison)
        {
            var range = FormatLineRange(comparison.StartLineIndex, comparison.StartLineIndex + expectedLines.Count);
            if (comparison.FirstDifferenceIndex < 0)
            {
                return $"{label} sample matches file lines {range}";
            }

            var expectedLineNumber = comparison.FirstDifferenceIndex + 1;
            var actualLineNumber = comparison.StartLineIndex + comparison.FirstDifferenceIndex + 1;
            return $"{label} sample at file lines {range}: {comparison.MatchedLineCount}/{expectedLines.Count} lines matched; first difference at sample line {expectedLineNumber} vs file line {actualLineNumber} ({comparison.FirstDifferenceReason}): expected \"{TruncateLineForMessage(comparison.FirstExpectedLine, 80)}\", found \"{TruncateLineForMessage(comparison.FirstActualLine, 80)}\"";
        }

        private static SequenceComparisonResult? FindClosestSequenceMatch(
            IReadOnlyList<string> fileLineTexts,
            IReadOnlyList<string> expectedLines)
        {
            if (expectedLines.Count == 0 || fileLineTexts.Count < expectedLines.Count)
            {
                return null;
            }

            SequenceComparisonResult? bestResult = null;
            for (var start = 0; start <= fileLineTexts.Count - expectedLines.Count; start++)
            {
                var result = CompareSequence(fileLineTexts, start, expectedLines);
                if (bestResult == null ||
                    result.TotalScore < bestResult!.TotalScore ||
                    (result.TotalScore == bestResult.TotalScore && result.MatchedLineCount > bestResult.MatchedLineCount) ||
                    (result.TotalScore == bestResult.TotalScore && result.MatchedLineCount == bestResult.MatchedLineCount && result.StartLineIndex < bestResult.StartLineIndex))
                {
                    bestResult = result;
                }
            }

            return bestResult;
        }

        private static SequenceComparisonResult CompareSequence(
            IReadOnlyList<string> fileLineTexts,
            int startIndex,
            IReadOnlyList<string> expectedLines)
        {
            var totalScore = 0;
            var matchedLineCount = 0;
            var firstDifferenceIndex = -1;
            var firstExpectedLine = string.Empty;
            var firstActualLine = string.Empty;
            var firstDifferenceReason = string.Empty;

            for (var index = 0; index < expectedLines.Count; index++)
            {
                var actualLine = fileLineTexts[startIndex + index];
                var expectedLine = expectedLines[index];
                var comparison = CompareLine(actualLine, expectedLine);
                totalScore += comparison.Score;
                if (comparison.Score <= 2)
                {
                    matchedLineCount++;
                }

                if (comparison.Score > 0 && firstDifferenceIndex < 0)
                {
                    firstDifferenceIndex = index;
                    firstExpectedLine = expectedLine;
                    firstActualLine = actualLine;
                    firstDifferenceReason = comparison.Reason;
                }
            }

            return new SequenceComparisonResult(
                startIndex,
                totalScore,
                matchedLineCount,
                firstDifferenceIndex,
                firstExpectedLine,
                firstActualLine,
                firstDifferenceReason);
        }

        private static LineComparisonResult CompareLine(string actualLine, string expectedLine)
        {
            if (string.Equals(actualLine, expectedLine, StringComparison.Ordinal))
            {
                return new LineComparisonResult(0, "exact text");
            }

            if (string.Equals(actualLine.TrimEnd(), expectedLine.TrimEnd(), StringComparison.Ordinal))
            {
                return new LineComparisonResult(1, "trailing whitespace differs");
            }

            if (string.Equals(NormalizeIndentationKey(actualLine), NormalizeIndentationKey(expectedLine), StringComparison.Ordinal))
            {
                return new LineComparisonResult(2, "indentation differs");
            }

            var distance = CalculateBoundedEditDistance(
                actualLine.TrimEnd(),
                expectedLine.TrimEnd(),
                80);
            return new LineComparisonResult(3 + distance, "text differs");
        }

        private static int CalculateBoundedEditDistance(string left, string right, int maximumDistance)
        {
            if (maximumDistance < 0)
            {
                maximumDistance = int.MaxValue;
            }

            if (string.Equals(left, right, StringComparison.Ordinal))
            {
                return 0;
            }

            if (Math.Abs(left.Length - right.Length) > maximumDistance)
            {
                return maximumDistance + 1;
            }

            var previous = new int[right.Length + 1];
            var current = new int[right.Length + 1];
            for (var index = 0; index <= right.Length; index++)
            {
                previous[index] = index;
            }

            for (var leftIndex = 1; leftIndex <= left.Length; leftIndex++)
            {
                current[0] = leftIndex;
                var rowMinimum = current[0];
                var leftChar = left[leftIndex - 1];

                for (var rightIndex = 1; rightIndex <= right.Length; rightIndex++)
                {
                    var cost = leftChar == right[rightIndex - 1] ? 0 : 1;
                    var deletion = previous[rightIndex] + 1;
                    var insertion = current[rightIndex - 1] + 1;
                    var substitution = previous[rightIndex - 1] + cost;
                    var value = Math.Min(Math.Min(deletion, insertion), substitution);
                    current[rightIndex] = value;
                    if (value < rowMinimum)
                    {
                        rowMinimum = value;
                    }
                }

                if (rowMinimum > maximumDistance)
                {
                    return maximumDistance + 1;
                }

                (previous, current) = (current, previous);
            }

            return previous[right.Length];
        }

        private static string BuildSpanPreview(IReadOnlyList<string> fileLineTexts, BlockMatch match)
        {
            if (fileLineTexts.Count == 0)
            {
                return "<empty file>";
            }

            var start = Math.Max(0, match.StartLineIndex - 1);
            var end = Math.Min(fileLineTexts.Count, match.EndLineExclusive + 1);
            var builder = new StringBuilder();
            for (var index = start; index < end; index++)
            {
                if (builder.Length > 0)
                {
                    builder.Append(" | ");
                }

                var prefix = index >= match.StartLineIndex && index < match.EndLineExclusive ? ">" : " ";
                builder.Append(prefix);
                builder.Append(index + 1);
                builder.Append(": ");
                builder.Append(TruncateLineForMessage(fileLineTexts[index], 64));
            }

            return builder.ToString();
        }

        private static string FormatLineRange(int startLineIndex, int endLineExclusive)
        {
            if (endLineExclusive <= startLineIndex)
            {
                return (startLineIndex + 1).ToString(CultureInfo.InvariantCulture);
            }

            if (endLineExclusive == startLineIndex + 1)
            {
                return (startLineIndex + 1).ToString(CultureInfo.InvariantCulture);
            }

            return $"{startLineIndex + 1}-{endLineExclusive}";
        }

        private static string TruncateLineForMessage(string text, int maxLength)
        {
            var normalized = text.Replace("\r", "\\r", StringComparison.Ordinal).Replace("\n", "\\n", StringComparison.Ordinal);
            if (normalized.Length <= maxLength)
            {
                return normalized;
            }

            if (maxLength <= 3)
            {
                return normalized[..maxLength];
            }

            return normalized[..(maxLength - 3)] + "...";
        }

        private static string FormatLineMatchMode(LineMatchMode lineMatchMode)
        {
            return lineMatchMode switch
            {
                LineMatchMode.Exact => "exact",
                LineMatchMode.TrimEnd => "trim-end",
                LineMatchMode.IndentNormalized => "tab/space-normalized",
                _ => lineMatchMode.ToString()
            };
        }

        private static string BuildSuccessContent(
            WriteTargetPath targetPath,
            EditFileAdvancedToolSettings settings,
            EncodingDecision encodingDecision,
            long bytesBefore,
            long bytesAfter,
            int charactersBefore,
            int charactersAfter,
            IReadOnlyList<AppliedEditBlock> blockResults)
        {
            var builder = new StringBuilder(768);
            AppendCommonContent(builder, targetPath, settings, encodingDecision);
            builder.AppendLine($"BlocksApplied: {blockResults.Count}");
            AppendBlockSummaries(builder, blockResults);
            builder.AppendLine($"CharactersBefore: {charactersBefore}");
            builder.AppendLine($"CharactersAfter: {charactersAfter}");
            builder.AppendLine($"BytesBefore: {bytesBefore}");
            builder.AppendLine($"BytesAfter: {bytesAfter}");
            return builder.ToString().TrimEnd();
        }

        private static string BuildNoChangeContent(
            WriteTargetPath targetPath,
            EditFileAdvancedToolSettings settings,
            EncodingDecision encodingDecision,
            long bytesBefore,
            int charactersBefore,
            IReadOnlyList<AppliedEditBlock> blockResults)
        {
            var builder = new StringBuilder(768);
            AppendCommonContent(builder, targetPath, settings, encodingDecision);
            builder.AppendLine("All edit blocks were located, but the requested final text is identical. File was not changed.");
            builder.AppendLine($"BlocksChecked: {blockResults.Count}");
            AppendBlockSummaries(builder, blockResults);
            builder.AppendLine($"Characters: {charactersBefore}");
            builder.AppendLine($"Bytes: {bytesBefore}");
            return builder.ToString().TrimEnd();
        }

        private static void AppendCommonContent(
            StringBuilder builder,
            WriteTargetPath targetPath,
            EditFileAdvancedToolSettings settings,
            EncodingDecision encodingDecision)
        {
            builder.AppendLine($"Path: {targetPath.ResolvedPath}");
            builder.AppendLine($"PermissionScope: {settings.PermissionScope}");
            if (!string.IsNullOrWhiteSpace(targetPath.LateralNodeName))
            {
                builder.AppendLine($"LateralFSNode: {targetPath.LateralNodeName}");
                builder.AppendLine($"LateralFSRelativePath: {targetPath.LateralRelativePath}");
                builder.AppendLine($"UsedLateralFSShortcut: {targetPath.UsedLateralShortcut}");
            }

            builder.AppendLine($"RequestedEncoding: {encodingDecision.RequestedEncodingDisplayName}");
            builder.AppendLine($"Encoding: {encodingDecision.EffectiveEncodingName}");
            if (encodingDecision.IsAutoDetected)
            {
                builder.AppendLine($"DetectedEncoding: {encodingDecision.DetectedEncodingName}");
                builder.AppendLine($"DetectedEncodingReason: {encodingDecision.DetectionReason}");
            }
        }

        private static void AppendBlockSummaries(StringBuilder builder, IReadOnlyList<AppliedEditBlock> blockResults)
        {
            foreach (var result in blockResults)
            {
                var summary = result.Summary;
                builder.AppendLine(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Block {0}: lines {1}-{2}, anchors {3}+{4}, replaced {5} lines with {6} lines, changed={7}",
                        summary.BlockIndex,
                        summary.StartLine,
                        summary.EndLine,
                        summary.PrefixAnchorLines,
                        summary.SuffixAnchorLines,
                        summary.ReplacedLineCount,
                        summary.ReplacementLineCount,
                        summary.Changed));
            }
        }

        private static IReadOnlyDictionary<string, object?> BuildData(
            WriteTargetPath? targetPath,
            EditFileAdvancedToolSettings settings,
            EncodingDecision? encodingDecision,
            long? bytesBefore,
            int? charactersBefore,
            long? bytesAfter,
            int? charactersAfter,
            int blocksApplied,
            bool didWrite,
            IReadOnlyList<AppliedEditBlockSummary> blockSummaries)
        {
            return new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase)
            {
                ["resolvedPath"] = targetPath?.ResolvedPath,
                ["permissionScope"] = settings.PermissionScope.ToString(),
                ["lateralNodeName"] = targetPath?.LateralNodeName,
                ["lateralNodeId"] = targetPath?.LateralNodeId,
                ["lateralNodeVirtualRootPath"] = targetPath?.LateralNodeVirtualRootPath,
                ["lateralRelativePath"] = targetPath?.LateralRelativePath,
                ["usedLateralShortcut"] = targetPath?.UsedLateralShortcut,
                ["requestedEncoding"] = encodingDecision?.RequestedEncodingName,
                ["requestedEncodingWasDefault"] = encodingDecision?.RequestedEncodingWasDefault,
                ["encoding"] = encodingDecision?.EffectiveEncodingName,
                ["encodingAutoDetected"] = encodingDecision?.IsAutoDetected,
                ["detectedEncoding"] = encodingDecision?.DetectedEncodingName,
                ["detectedEncodingReason"] = encodingDecision?.DetectionReason,
                ["bytesBefore"] = bytesBefore,
                ["charactersBefore"] = charactersBefore,
                ["bytesAfter"] = bytesAfter,
                ["charactersAfter"] = charactersAfter,
                ["blocksApplied"] = blocksApplied,
                ["didWrite"] = didWrite,
                ["blockSummaries"] = blockSummaries.Select(summary => summary.ToString()).ToArray()
            };
        }

        private static string? GetExplicitEncodingName(FerritaToolArguments arguments)
        {
            return arguments.RawArguments.TryGetValue("Encoding", out var rawEncoding) &&
                !string.IsNullOrWhiteSpace(rawEncoding)
                ? rawEncoding.Trim()
                : null;
        }

        private static EncodingDecision ResolveEncodingDecision(byte[] fileBytes, string? explicitEncodingName)
        {
            var requestedEncodingWasDefault = string.IsNullOrWhiteSpace(explicitEncodingName);
            var requestedEncodingName = requestedEncodingWasDefault
                ? "utf-8"
                : explicitEncodingName!.Trim();

            if (ShouldAutoDetectForUtf8Request(requestedEncodingName))
            {
                if (TryDetectUnicodeEncoding(fileBytes, out var detectedEncoding, out var detectedEncodingName, out var detectionReason, out var preamble))
                {
                    return new EncodingDecision(
                        detectedEncoding,
                        detectedEncodingName,
                        requestedEncodingName,
                        requestedEncodingWasDefault,
                        IsAutoDetected: true,
                        detectedEncodingName,
                        detectionReason,
                        preamble);
                }

                if (!TryDecodeStrict(fileBytes, 0, fileBytes.Length, s_utf8WithoutBomStrict, out _))
                {
                    throw new InvalidOperationException(
                        "The file is not valid UTF-8 and no UTF BOM/UTF-16/UTF-32 pattern was detected. Provide the correct Encoding parameter to avoid corrupting the file.");
                }

                return new EncodingDecision(
                    s_utf8WithoutBomStrict,
                    s_utf8WithoutBomStrict.WebName,
                    requestedEncodingName,
                    requestedEncodingWasDefault,
                    IsAutoDetected: false,
                    null,
                    null,
                    Array.Empty<byte>());
            }

            var resolvedEncoding = ResolveStrictEncoding(requestedEncodingName);
            return new EncodingDecision(
                resolvedEncoding,
                resolvedEncoding.WebName,
                requestedEncodingName,
                requestedEncodingWasDefault,
                IsAutoDetected: false,
                null,
                null,
                ResolveMatchingPreamble(fileBytes, resolvedEncoding));
        }

        private static bool ShouldAutoDetectForUtf8Request(string requestedEncodingName)
        {
            return string.Equals(
                ResolveStrictEncoding(requestedEncodingName).WebName,
                s_utf8WithoutBomStrict.WebName,
                StringComparison.OrdinalIgnoreCase);
        }

        private static Encoding ResolveStrictEncoding(string encodingName)
        {
            var normalized = string.IsNullOrWhiteSpace(encodingName)
                ? "utf-8"
                : encodingName.Trim();
            var lower = normalized.ToLowerInvariant();

            return lower switch
            {
                "utf8" or "utf-8" => s_utf8WithoutBomStrict,
                "unicode" or "utf-16" or "utf-16le" or "utf-16-le" => s_utf16LittleEndianStrict,
                "unicodefffe" or "utf-16be" or "utf-16-be" => s_utf16BigEndianStrict,
                "utf-32" or "utf-32le" or "utf-32-le" => s_utf32LittleEndianStrict,
                "utf-32be" or "utf-32-be" => s_utf32BigEndianStrict,
                _ => Encoding.GetEncoding(
                    ToolFileSystemHelper.ResolveEncoding(normalized).CodePage,
                    EncoderFallback.ExceptionFallback,
                    DecoderFallback.ExceptionFallback)
            };
        }

        private static bool TryDetectUnicodeEncoding(
            byte[] fileBytes,
            out Encoding encoding,
            out string encodingName,
            out string detectionReason,
            out byte[] preamble)
        {
            encoding = s_utf8WithoutBomStrict;
            encodingName = s_utf8WithoutBomStrict.WebName;
            detectionReason = string.Empty;
            preamble = Array.Empty<byte>();

            if (fileBytes.Length == 0)
            {
                return false;
            }

            if (HasPrefix(fileBytes, s_utf32BigEndianPreamble))
            {
                encoding = s_utf32BigEndianStrict;
                encodingName = "utf-32-be";
                detectionReason = "byte-order mark 00 00 FE FF";
                preamble = s_utf32BigEndianPreamble;
                return true;
            }

            if (HasPrefix(fileBytes, s_utf32LittleEndianPreamble))
            {
                encoding = s_utf32LittleEndianStrict;
                encodingName = "utf-32-le";
                detectionReason = "byte-order mark FF FE 00 00";
                preamble = s_utf32LittleEndianPreamble;
                return true;
            }

            if (HasPrefix(fileBytes, s_utf8Preamble))
            {
                encoding = s_utf8WithoutBomStrict;
                encodingName = "utf-8";
                detectionReason = "byte-order mark EF BB BF";
                preamble = s_utf8Preamble;
                return true;
            }

            if (HasPrefix(fileBytes, s_utf16LittleEndianPreamble))
            {
                encoding = s_utf16LittleEndianStrict;
                encodingName = "utf-16-le";
                detectionReason = "byte-order mark FF FE";
                preamble = s_utf16LittleEndianPreamble;
                return true;
            }

            if (HasPrefix(fileBytes, s_utf16BigEndianPreamble))
            {
                encoding = s_utf16BigEndianStrict;
                encodingName = "utf-16-be";
                detectionReason = "byte-order mark FE FF";
                preamble = s_utf16BigEndianPreamble;
                return true;
            }

            if (TryDetectUtf32WithoutBom(fileBytes, out encoding, out encodingName, out detectionReason))
            {
                return true;
            }

            if (TryDetectUtf16WithoutBom(fileBytes, out encoding, out encodingName, out detectionReason))
            {
                return true;
            }

            return false;
        }

        private static bool TryDetectUtf32WithoutBom(
            byte[] fileBytes,
            out Encoding encoding,
            out string encodingName,
            out string detectionReason)
        {
            encoding = s_utf8WithoutBomStrict;
            encodingName = s_utf8WithoutBomStrict.WebName;
            detectionReason = string.Empty;

            var sampleLength = Math.Min(fileBytes.Length - (fileBytes.Length % 4), 512);
            var quartetCount = sampleLength / 4;
            if (quartetCount < 4)
            {
                return false;
            }

            var likelyLittleEndian = 0;
            var likelyBigEndian = 0;
            for (var index = 0; index < sampleLength; index += 4)
            {
                if (fileBytes[index] != 0 &&
                    fileBytes[index + 1] == 0 &&
                    fileBytes[index + 2] == 0 &&
                    fileBytes[index + 3] == 0)
                {
                    likelyLittleEndian++;
                }

                if (fileBytes[index] == 0 &&
                    fileBytes[index + 1] == 0 &&
                    fileBytes[index + 2] == 0 &&
                    fileBytes[index + 3] != 0)
                {
                    likelyBigEndian++;
                }
            }

            if (likelyLittleEndian >= quartetCount * 0.6 && likelyBigEndian == 0)
            {
                encoding = s_utf32LittleEndianStrict;
                encodingName = "utf-32-le";
                detectionReason = "strong UTF-32 little-endian zero-byte pattern without BOM";
                return true;
            }

            if (likelyBigEndian >= quartetCount * 0.6 && likelyLittleEndian == 0)
            {
                encoding = s_utf32BigEndianStrict;
                encodingName = "utf-32-be";
                detectionReason = "strong UTF-32 big-endian zero-byte pattern without BOM";
                return true;
            }

            return false;
        }

        private static bool TryDetectUtf16WithoutBom(
            byte[] fileBytes,
            out Encoding encoding,
            out string encodingName,
            out string detectionReason)
        {
            encoding = s_utf8WithoutBomStrict;
            encodingName = s_utf8WithoutBomStrict.WebName;
            detectionReason = string.Empty;

            var sampleLength = Math.Min(fileBytes.Length - (fileBytes.Length % 2), 512);
            var pairCount = sampleLength / 2;
            if (pairCount < 8)
            {
                return false;
            }

            var likelyLittleEndian = 0;
            var likelyBigEndian = 0;
            for (var index = 0; index < sampleLength; index += 2)
            {
                var first = fileBytes[index];
                var second = fileBytes[index + 1];

                if (first != 0 && second == 0)
                {
                    likelyLittleEndian++;
                }
                else if (first == 0 && second != 0)
                {
                    likelyBigEndian++;
                }
            }

            if (likelyLittleEndian >= pairCount * 0.6 && likelyBigEndian <= pairCount * 0.1)
            {
                encoding = s_utf16LittleEndianStrict;
                encodingName = "utf-16-le";
                detectionReason = "strong UTF-16 little-endian zero-byte pattern without BOM";
                return true;
            }

            if (likelyBigEndian >= pairCount * 0.6 && likelyLittleEndian <= pairCount * 0.1)
            {
                encoding = s_utf16BigEndianStrict;
                encodingName = "utf-16-be";
                detectionReason = "strong UTF-16 big-endian zero-byte pattern without BOM";
                return true;
            }

            return false;
        }

        private static byte[] ResolveMatchingPreamble(byte[] fileBytes, Encoding encoding)
        {
            if (encoding.CodePage == s_utf32BigEndianStrict.CodePage && HasPrefix(fileBytes, s_utf32BigEndianPreamble))
            {
                return s_utf32BigEndianPreamble;
            }

            if (encoding.CodePage == s_utf32LittleEndianStrict.CodePage && HasPrefix(fileBytes, s_utf32LittleEndianPreamble))
            {
                return s_utf32LittleEndianPreamble;
            }

            if (encoding.CodePage == s_utf8WithoutBomStrict.CodePage && HasPrefix(fileBytes, s_utf8Preamble))
            {
                return s_utf8Preamble;
            }

            if (encoding.CodePage == s_utf16LittleEndianStrict.CodePage && HasPrefix(fileBytes, s_utf16LittleEndianPreamble))
            {
                return s_utf16LittleEndianPreamble;
            }

            if (encoding.CodePage == s_utf16BigEndianStrict.CodePage && HasPrefix(fileBytes, s_utf16BigEndianPreamble))
            {
                return s_utf16BigEndianPreamble;
            }

            return Array.Empty<byte>();
        }

        private static string DecodeContent(byte[] fileBytes, EncodingDecision encodingDecision)
        {
            var offset = encodingDecision.Preamble.Length;
            return encodingDecision.Encoding.GetString(fileBytes, offset, fileBytes.Length - offset);
        }

        private static bool TryDecodeStrict(
            byte[] fileBytes,
            int offset,
            int count,
            Encoding encoding,
            out string content)
        {
            try
            {
                content = encoding.GetString(fileBytes, offset, count);
                return true;
            }
            catch (DecoderFallbackException)
            {
                content = string.Empty;
                return false;
            }
        }

        private static byte[] EncodeContent(string content, EncodingDecision encodingDecision)
        {
            var encodedContent = encodingDecision.Encoding.GetBytes(content);
            if (encodingDecision.Preamble.Length == 0)
            {
                return encodedContent;
            }

            var bytes = new byte[encodingDecision.Preamble.Length + encodedContent.Length];
            Buffer.BlockCopy(encodingDecision.Preamble, 0, bytes, 0, encodingDecision.Preamble.Length);
            Buffer.BlockCopy(encodedContent, 0, bytes, encodingDecision.Preamble.Length, encodedContent.Length);
            return bytes;
        }

        private static bool HasPrefix(byte[] fileBytes, byte[] prefix)
        {
            if (fileBytes.Length < prefix.Length)
            {
                return false;
            }

            for (var index = 0; index < prefix.Length; index++)
            {
                if (fileBytes[index] != prefix[index])
                {
                    return false;
                }
            }

            return true;
        }

        private static bool IsExpectedException(Exception ex)
        {
            return ex is IOException
                or UnauthorizedAccessException
                or InvalidOperationException
                or ArgumentException
                or NotSupportedException
                or DecoderFallbackException
                or EncoderFallbackException;
        }

        private sealed record WriteTargetPath(
            string ResolvedPath,
            string? LateralNodeName,
            string? LateralNodeId,
            string? LateralNodeVirtualRootPath,
            string? LateralRelativePath,
            bool UsedLateralShortcut);

        private sealed record EncodingDecision(
            Encoding Encoding,
            string EffectiveEncodingName,
            string RequestedEncodingName,
            bool RequestedEncodingWasDefault,
            bool IsAutoDetected,
            string? DetectedEncodingName,
            string? DetectionReason,
            byte[] Preamble)
        {
            public string RequestedEncodingDisplayName => RequestedEncodingWasDefault
                ? $"{RequestedEncodingName} (default)"
                : RequestedEncodingName;
        }

        private sealed record EditBlock(
            int Index,
            string FinalText,
            bool AnchoredAtBeginningOfFile,
            bool AnchoredAtEndOfFile);

        private sealed record TextLine(string Text, string Newline, int StartOffset);

        private sealed record SplitLineResult(IReadOnlyList<TextLine> Lines, IReadOnlyList<string> LineTexts);

        private sealed record BlockMatch(
            int StartLineIndex,
            int EndLineExclusive,
            int PrefixLineCount,
            int SuffixLineCount,
            bool PreserveAnchors,
            LineMatchMode LineMatchMode);

        private sealed record BlockMatchResolution(BlockMatch? Match, IReadOnlyList<BlockMatch> CandidateMatches)
        {
            public bool IsAmbiguous => CandidateMatches.Count > 1;
        }

        private sealed record SequenceComparisonResult(
            int StartLineIndex,
            int TotalScore,
            int MatchedLineCount,
            int FirstDifferenceIndex,
            string FirstExpectedLine,
            string FirstActualLine,
            string FirstDifferenceReason);

        private readonly record struct LineComparisonResult(int Score, string Reason);

        private sealed record AppliedEditBlock(string UpdatedContent, AppliedEditBlockSummary Summary);

        private sealed record AppliedEditBlockSummary(
            int BlockIndex,
            int StartLine,
            int EndLine,
            int PrefixAnchorLines,
            int SuffixAnchorLines,
            int ReplacedLineCount,
            int ReplacementLineCount,
            bool Changed);

        private enum LineMatchMode
        {
            Exact,
            TrimEnd,
            IndentNormalized
        }
    }

    internal enum EditFileAdvancedPermissionScope
    {
        LateralFileSystemOnly,
        FullAccess
    }

    internal sealed class EditFileAdvancedToolSettings
    {
        private const string RootElementName = "EditFileAdvancedSettings";

        public EditFileAdvancedPermissionScope PermissionScope { get; set; } =
            EditFileAdvancedPermissionScope.LateralFileSystemOnly;

        public XElement ToXElement()
        {
            return new XElement(
                RootElementName,
                new XElement("PermissionScope", PermissionScope.ToString()));
        }

        public static EditFileAdvancedToolSettings FromConfiguration(FerritaToolConfigurationState? configuration)
        {
            var payload = configuration?.GetPayload();
            if (payload == null)
            {
                return new EditFileAdvancedToolSettings();
            }

            var root = string.Equals(payload.Name.LocalName, RootElementName, StringComparison.OrdinalIgnoreCase)
                ? payload
                : payload.Element(RootElementName);

            if (root == null)
            {
                return new EditFileAdvancedToolSettings();
            }

            return new EditFileAdvancedToolSettings
            {
                PermissionScope = ParsePermissionScope((string?)root.Element("PermissionScope"))
            };
        }

        public static EditFileAdvancedPermissionScope ParsePermissionScope(string? value)
        {
            var normalized = (value ?? string.Empty).Trim();
            if (normalized.Length == 0)
            {
                return EditFileAdvancedPermissionScope.LateralFileSystemOnly;
            }

            if (string.Equals(normalized, "FullAccess", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(normalized, "FullAuthorization", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(normalized, "Full", StringComparison.OrdinalIgnoreCase))
            {
                return EditFileAdvancedPermissionScope.FullAccess;
            }

            return EditFileAdvancedPermissionScope.LateralFileSystemOnly;
        }
    }

    internal sealed class EditFileAdvancedPermissionOption
    {
        public EditFileAdvancedPermissionOption(
            EditFileAdvancedPermissionScope scope,
            string displayName,
            string description)
        {
            Scope = scope;
            DisplayName = displayName;
            Description = description;
        }

        public EditFileAdvancedPermissionScope Scope { get; }

        public string DisplayName { get; }

        public string Description { get; }
    }

    internal sealed class EditFileAdvancedToolConfigurationViewModel : ObservableObject
    {
        private readonly Action _notifyConfigurationChanged;
        private EditFileAdvancedPermissionOption? _selectedPermission;

        public EditFileAdvancedToolConfigurationViewModel(
            EditFileAdvancedToolSettings settings,
            Action notifyConfigurationChanged)
        {
            _notifyConfigurationChanged = notifyConfigurationChanged ?? throw new ArgumentNullException(nameof(notifyConfigurationChanged));
            PermissionOptions = new ObservableCollection<EditFileAdvancedPermissionOption>
            {
                new(
                    EditFileAdvancedPermissionScope.LateralFileSystemOnly,
                    "LateralFS only",
                    "The model can write only inside LateralFS virtual folders. LateralFS\\NodeName\\... shortcuts are supported and checked."),
                new(
                    EditFileAdvancedPermissionScope.FullAccess,
                    "Full access",
                    "The model can write any file path that the Ferrita process account can access. LateralFS shortcuts still work.")
            };

            _selectedPermission = PermissionOptions.FirstOrDefault(option => option.Scope == settings.PermissionScope)
                ?? PermissionOptions[0];
        }

        public ObservableCollection<EditFileAdvancedPermissionOption> PermissionOptions { get; }

        public EditFileAdvancedPermissionOption? SelectedPermission
        {
            get => _selectedPermission;
            set
            {
                if (SetProperty(ref _selectedPermission, value))
                {
                    OnPropertyChanged(nameof(PermissionDescription));
                    OnPropertyChanged(nameof(PreviewText));
                    _notifyConfigurationChanged();
                }
            }
        }

        public string PermissionDescription => SelectedPermission?.Description ?? string.Empty;

        public string PreviewText
        {
            get
            {
                var scope = SelectedPermission?.Scope ?? EditFileAdvancedPermissionScope.LateralFileSystemOnly;
                return scope == EditFileAdvancedPermissionScope.FullAccess
                    ? "Current permission: FullAccess. Normal absolute/relative paths and LateralFS shortcuts are accepted."
                    : "Current permission: LateralFileSystemOnly. Use LateralFS\\NodeName\\relative\\file.ext or an actual path under a LateralFS virtual root.";
            }
        }

        public EditFileAdvancedToolSettings ToSettings()
        {
            return new EditFileAdvancedToolSettings
            {
                PermissionScope = SelectedPermission?.Scope ?? EditFileAdvancedPermissionScope.LateralFileSystemOnly
            };
        }
    }

    internal sealed class EditFileAdvancedToolConfigurationPresenter : FerritaToolConfigurationPresenter
    {
        private readonly EditFileAdvancedToolConfigurationViewModel _viewModel;
        private readonly FrameworkElement _view;

        public EditFileAdvancedToolConfigurationPresenter(FerritaToolConfigurationEditorContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            var settings = EditFileAdvancedToolSettings.FromConfiguration(context.InitialConfiguration);
            _viewModel = new EditFileAdvancedToolConfigurationViewModel(settings, RaiseConfigurationChanged);
            _view = CreateView(_viewModel);
        }

        public override FrameworkElement View => _view;

        public override bool TryCaptureConfiguration(out XElement? configuration, out string? errorMessage)
        {
            try
            {
                configuration = _viewModel.ToSettings().ToXElement();
                errorMessage = null;
                return true;
            }
            catch (Exception ex)
            {
                configuration = null;
                errorMessage = $"EditFile_Advanced configuration is invalid: {ex.Message}";
                return false;
            }
        }

        private static FrameworkElement CreateView(EditFileAdvancedToolConfigurationViewModel viewModel)
        {
            var panel = new StackPanel
            {
                DataContext = viewModel
            };

            panel.Children.Add(new TextBlock
            {
                Text = LocalizationRuntime.Instance.GetString("ToolConfiguration.Permission", "Permission"),
                FontSize = 13,
                FontWeight = FontWeights.SemiBold,
                Margin = new Thickness(0, 0, 0, 8)
            });

            var comboBox = new ComboBox
            {
                MinWidth = 220,
                DisplayMemberPath = nameof(EditFileAdvancedPermissionOption.DisplayName),
                Margin = new Thickness(0, 0, 0, 10)
            };
            comboBox.SetBinding(
                ItemsControl.ItemsSourceProperty,
                new Binding(nameof(EditFileAdvancedToolConfigurationViewModel.PermissionOptions)));
            comboBox.SetBinding(
                ComboBox.SelectedItemProperty,
                new Binding(nameof(EditFileAdvancedToolConfigurationViewModel.SelectedPermission))
                {
                    Mode = BindingMode.TwoWay,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                });
            panel.Children.Add(comboBox);

            var description = new TextBlock
            {
                TextWrapping = TextWrapping.Wrap,
                Foreground = Brushes.LightCyan,
                Margin = new Thickness(0, 0, 0, 10)
            };
            description.SetBinding(
                TextBlock.TextProperty,
                new Binding(nameof(EditFileAdvancedToolConfigurationViewModel.PermissionDescription)));
            panel.Children.Add(description);

            var preview = new TextBlock
            {
                TextWrapping = TextWrapping.Wrap,
                Foreground = Brushes.White,
                Opacity = 0.72
            };
            preview.SetBinding(
                TextBlock.TextProperty,
                new Binding(nameof(EditFileAdvancedToolConfigurationViewModel.PreviewText)));
            panel.Children.Add(preview);

            return panel;
        }
    }
}
