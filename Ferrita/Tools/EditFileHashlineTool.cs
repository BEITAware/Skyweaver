using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
    /// <summary>
    /// 利用 Hashline 上下文哈希进行文件局部编辑的工具。
    /// 默认采用 LateralFS Only 安全限制，支持在设置面板中切换权限范围。
    /// </summary>
    public sealed class EditFileHashlineTool :
        IFerritaTool,
        IFerritaToolConfigurationProvider,
        IFerritaToolInvocationPresentationProvider,
        IFerritaToolPromptDescriptionProvider
    {
        public const string ToolName = "EditFileHashline";

        private static readonly FerritaToolDefinition s_definition = BuildDefinition(new EditFileHashlineToolSettings());

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
            return BuildDefinition(EditFileHashlineToolSettings.FromConfiguration(configuration));
        }

        public FerritaToolConfigurationPresenter? CreateConfigurationPresenter(FerritaToolConfigurationEditorContext context)
        {
            return new EditFileHashlineToolConfigurationPresenter(context);
        }

        public string GetPromptDescription(FerritaToolPromptDescriptionContext context)
        {
            return BuildDescription(EditFileHashlineToolSettings.FromConfiguration(context.ConfigurationState));
        }

        public FrameworkElement? CreateInvocationPresentation(FerritaToolInvocationPresentationContext context)
        {
            ArgumentNullException.ThrowIfNull(context);
            return ToolInvocationCardFactory.Create(
                context,
                [
                    new ToolInvocationCardFieldDefinition("File path", "FilePath", "Waiting for file path..."),
                    new ToolInvocationCardFieldDefinition("Start line hash", "StartLineHash", "Waiting for start line hash..."),
                    new ToolInvocationCardFieldDefinition("End line hash", "EndLineHash", "Waiting for end line hash..."),
                    new ToolInvocationCardFieldDefinition("Replace content", "ReplaceContent", "Waiting for replacement content..."),
                    new ToolInvocationCardFieldDefinition("Encoding", "Encoding", "Default utf-8 with safe auto-detection")
                ]);
        }

        public async Task<FerritaToolResult> ExecuteAsync(
            FerritaToolContext context,
            FerritaToolArguments arguments,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var settings = EditFileHashlineToolSettings.FromConfiguration(context.CurrentToolConfiguration);
            var requestedPath = arguments.GetString("FilePath") ?? string.Empty;
            var startLineHash = (arguments.GetString("StartLineHash") ?? string.Empty).Trim().ToLowerInvariant();
            var endLineHash = (arguments.GetString("EndLineHash") ?? string.Empty).Trim().ToLowerInvariant();
            var replaceContent = arguments.GetString("ReplaceContent");
            WriteTargetPath? targetPath = null;

            try
            {
                if (string.IsNullOrEmpty(startLineHash))
                {
                    return FerritaToolResult.Failure("Start line hash cannot be empty.");
                }
                if (string.IsNullOrEmpty(endLineHash))
                {
                    return FerritaToolResult.Failure("End line hash cannot be empty.");
                }

                targetPath = ResolveWriteTargetPath(requestedPath, context.WorkspacePath);

                // 检验权限范围限制
                if (settings.PermissionScope == EditFileHashlinePermissionScope.LateralFileSystemOnly &&
                    targetPath.LateralNodeName == null)
                {
                    return FerritaToolResult.Failure(
                        "This tool is configured as LateralFileSystemOnly. FilePath must resolve inside a LateralFS virtual folder. Prefer LateralFS\\NodeName\\relative\\file.ext.",
                        BuildData(targetPath, settings, null, null, null, didWrite: false));
                }

                if (Directory.Exists(targetPath.ResolvedPath))
                {
                    return FerritaToolResult.Failure(
                        $"Path points to a directory, not a file: {targetPath.ResolvedPath}",
                        BuildData(targetPath, settings, null, null, null, didWrite: false));
                }

                if (!File.Exists(targetPath.ResolvedPath))
                {
                    return FerritaToolResult.Failure(
                        $"File not found: {targetPath.ResolvedPath}",
                        BuildData(targetPath, settings, null, null, null, didWrite: false));
                }

                var originalBytes = await File.ReadAllBytesAsync(targetPath.ResolvedPath, cancellationToken).ConfigureAwait(false);
                var encodingDecision = ResolveEncodingDecision(originalBytes, GetExplicitEncodingName(arguments));
                var originalContent = DecodeContent(originalBytes, encodingDecision);

                // 解析文件并计算行哈希
                var originalLines = HashlineHelper.ParseFileLines(originalContent);
                HashlineHelper.CalculateHashes(originalLines);

                // 寻找起始与终止哈希行
                var startMatches = originalLines
                    .Select((line, index) => new { line, index })
                    .Where(x => string.Equals(x.line.Hash, startLineHash, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                var endMatches = originalLines
                    .Select((line, index) => new { line, index })
                    .Where(x => string.Equals(x.line.Hash, endLineHash, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                if (startMatches.Count == 0)
                {
                    return FerritaToolResult.Failure(
                        $"Start line hash '{startLineHash}' was not found in the file.",
                        BuildData(targetPath, settings, encodingDecision, originalBytes.LongLength, null, didWrite: false));
                }
                if (startMatches.Count > 1)
                {
                    return FerritaToolResult.Failure(
                        $"Multiple lines matched the start line hash '{startLineHash}'. Aborting for safety.",
                        BuildData(targetPath, settings, encodingDecision, originalBytes.LongLength, null, didWrite: false));
                }

                if (endMatches.Count == 0)
                {
                    return FerritaToolResult.Failure(
                        $"End line hash '{endLineHash}' was not found in the file.",
                        BuildData(targetPath, settings, encodingDecision, originalBytes.LongLength, null, didWrite: false));
                }
                if (endMatches.Count > 1)
                {
                    return FerritaToolResult.Failure(
                        $"Multiple lines matched the end line hash '{endLineHash}'. Aborting for safety.",
                        BuildData(targetPath, settings, encodingDecision, originalBytes.LongLength, null, didWrite: false));
                }

                int startIndex = startMatches[0].index;
                int endIndex = endMatches[0].index;

                if (startIndex > endIndex)
                {
                    return FerritaToolResult.Failure(
                        $"Start line hash '{startLineHash}' (line {startIndex + 1}) occurs after end line hash '{endLineHash}' (line {endIndex + 1}) in the file.",
                        BuildData(targetPath, settings, encodingDecision, originalBytes.LongLength, null, didWrite: false));
                }

                // 进行替换或删除
                var newLines = new List<HashlineHelper.FileLine>();
                if (string.IsNullOrEmpty(replaceContent))
                {
                    // 如果 replaceContent 为空，则删除起始哈希与终止哈希之间的行，保留这两个锚行本身
                    newLines.AddRange(originalLines.Take(startIndex + 1));
                    if (endIndex > startIndex)
                    {
                        newLines.AddRange(originalLines.Skip(endIndex));
                    }
                    else
                    {
                        newLines.AddRange(originalLines.Skip(startIndex + 1));
                    }
                }
                else
                {
                    // 否则，用新内容替换整个块（包含起始与终止行）
                    var replacementLines = HashlineHelper.ParseFileLines(replaceContent);
                    if (replacementLines.Count > 0)
                    {
                        var lastReplacementLine = replacementLines[^1];
                        if (string.IsNullOrEmpty(lastReplacementLine.LineBreak))
                        {
                            lastReplacementLine.LineBreak = originalLines[endIndex].LineBreak;
                        }
                    }

                    newLines.AddRange(originalLines.Take(startIndex));
                    newLines.AddRange(replacementLines);
                    newLines.AddRange(originalLines.Skip(endIndex + 1));
                }

                string updatedContent = string.Concat(newLines.Select(l => l.Text + l.LineBreak));

                if (string.Equals(updatedContent, originalContent, StringComparison.Ordinal))
                {
                    return FerritaToolResult.Success(
                        "Replacement content is identical to the selected block. File was not changed.",
                        BuildData(targetPath, settings, encodingDecision, originalBytes.LongLength, originalBytes.LongLength, didWrite: false));
                }

                var updatedBytes = EncodeContent(updatedContent, encodingDecision);
                await File.WriteAllBytesAsync(targetPath.ResolvedPath, updatedBytes, cancellationToken).ConfigureAwait(false);

                // 刷新 RAG 索引
                var ragSync = await AerialCityRagToolSync.RefreshFileAsync(
                    targetPath.ResolvedPath,
                    context.WorkspacePath,
                    cancellationToken).ConfigureAwait(false);

                return FerritaToolResult.Success(
                    FerritaLineDiffPresentation.BuildContent(originalContent, updatedContent),
                    AerialCityRagToolSync.WithSyncData(
                        BuildData(targetPath, settings, encodingDecision, originalBytes.LongLength, updatedBytes.LongLength, didWrite: true),
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
                    $"Failed to edit file using Hashline: {ex.Message}",
                    BuildData(targetPath, settings, null, null, null, didWrite: false));
            }
        }

        private static FerritaToolDefinition BuildDefinition(EditFileHashlineToolSettings settings)
        {
            return new FerritaToolDefinition(
                ToolName,
                BuildDescription(settings),
                "Script",
                [
                    new FerritaToolParameterDefinition(
                        "FilePath",
                        "Path of the text file to edit.",
                        FerritaToolParameterType.String,
                        isRequired: true),
                    new FerritaToolParameterDefinition(
                        "StartLineHash",
                        "The 4-character hash of the starting line of the block to replace.",
                        FerritaToolParameterType.String,
                        isRequired: true),
                    new FerritaToolParameterDefinition(
                        "EndLineHash",
                        "The 4-character hash of the ending line of the block to replace.",
                        FerritaToolParameterType.String,
                        isRequired: true),
                    new FerritaToolParameterDefinition(
                        "ReplaceContent",
                        "The new content to replace the target block (inclusive). If null or empty, the lines strictly between start and end hashes are deleted, and the start/end lines themselves are kept.",
                        FerritaToolParameterType.String,
                        isRequired: false,
                        defaultValue: null),
                    new FerritaToolParameterDefinition(
                        "Encoding",
                        "Optional text encoding name. Default is utf-8.",
                        FerritaToolParameterType.String,
                        isRequired: false,
                        defaultValue: "utf-8")
                ],
                defaultAgentPermission: FerritaToolDefaultAgentPermission.RequireConfirmation);
        }

        private static string BuildDescription(EditFileHashlineToolSettings settings)
        {
            var permissionText = settings.PermissionScope == EditFileHashlinePermissionScope.LateralFileSystemOnly
                ? "Permission: LateralFileSystemOnly, so the tool may write only inside LateralFS virtual folders."
                : "Permission: FullAccess, so the tool may write any file path that the Ferrita process account can access.";

            return "Edits a file by replacing a block of text defined by start and end line hashes (inclusive) with the ReplaceContent. " +
                "FilePath may be a normal path, or a LateralFS shortcut in the form LateralFS\\NodeName\\relative\\file.ext. " +
                permissionText;
        }

        private static string? GetExplicitEncodingName(FerritaToolArguments arguments)
        {
            return arguments.RawArguments.TryGetValue("Encoding", out var rawEncoding) &&
                !string.IsNullOrWhiteSpace(rawEncoding)
                ? rawEncoding.Trim()
                : null;
        }

        private static WriteTargetPath ResolveWriteTargetPath(string requestedPath, string? workspacePath)
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

            return new WriteTargetPath(
                resolvedPath,
                lateralResolution?.NodeName,
                lateralResolution?.NodeId,
                lateralResolution?.NodeVirtualRootPath,
                lateralResolution?.RelativePath,
                lateralResolution?.UsedShortcut ?? false);
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

        private static IReadOnlyDictionary<string, object?> BuildData(
            WriteTargetPath? targetPath,
            EditFileHashlineToolSettings settings,
            EncodingDecision? encodingDecision,
            long? bytesBefore,
            long? bytesAfter,
            bool didWrite)
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
                ["encoding"] = encodingDecision?.Encoding.WebName,
                ["encodingAutoDetected"] = encodingDecision?.IsAutoDetected,
                ["detectedEncoding"] = encodingDecision?.DetectedEncodingName,
                ["detectedEncodingReason"] = encodingDecision?.DetectionReason,
                ["bytesBefore"] = bytesBefore,
                ["bytesAfter"] = bytesAfter,
                ["didWrite"] = didWrite
            };
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
    }

    internal enum EditFileHashlinePermissionScope
    {
        LateralFileSystemOnly,
        FullAccess
    }

    internal sealed class EditFileHashlineToolSettings
    {
        private const string RootElementName = "EditFileHashlineSettings";

        public EditFileHashlinePermissionScope PermissionScope { get; set; } =
            EditFileHashlinePermissionScope.LateralFileSystemOnly;

        public XElement ToXElement()
        {
            return new XElement(
                RootElementName,
                new XElement("PermissionScope", PermissionScope.ToString()));
        }

        public static EditFileHashlineToolSettings FromConfiguration(FerritaToolConfigurationState? configuration)
        {
            var payload = configuration?.GetPayload();
            if (payload == null)
            {
                return new EditFileHashlineToolSettings();
            }

            var root = string.Equals(payload.Name.LocalName, RootElementName, StringComparison.OrdinalIgnoreCase)
                ? payload
                : payload.Element(RootElementName);

            if (root == null)
            {
                return new EditFileHashlineToolSettings();
            }

            return new EditFileHashlineToolSettings
            {
                PermissionScope = ParsePermissionScope((string?)root.Element("PermissionScope"))
            };
        }

        public static EditFileHashlinePermissionScope ParsePermissionScope(string? value)
        {
            var normalized = (value ?? string.Empty).Trim();
            if (normalized.Length == 0)
            {
                return EditFileHashlinePermissionScope.LateralFileSystemOnly;
            }

            if (string.Equals(normalized, "FullAccess", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(normalized, "FullAuthorization", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(normalized, "Full", StringComparison.OrdinalIgnoreCase))
            {
                return EditFileHashlinePermissionScope.FullAccess;
            }

            return EditFileHashlinePermissionScope.LateralFileSystemOnly;
        }
    }

    internal sealed class EditFileHashlinePermissionOption
    {
        public EditFileHashlinePermissionOption(
            EditFileHashlinePermissionScope scope,
            string displayName,
            string description)
        {
            Scope = scope;
            DisplayName = displayName;
            Description = description;
        }

        public EditFileHashlinePermissionScope Scope { get; }

        public string DisplayName { get; }

        public string Description { get; }
    }

    internal sealed class EditFileHashlineToolConfigurationViewModel : ObservableObject
    {
        private readonly Action _notifyConfigurationChanged;
        private EditFileHashlinePermissionOption? _selectedPermission;

        public EditFileHashlineToolConfigurationViewModel(
            EditFileHashlineToolSettings settings,
            Action notifyConfigurationChanged)
        {
            _notifyConfigurationChanged = notifyConfigurationChanged ?? throw new ArgumentNullException(nameof(notifyConfigurationChanged));
            PermissionOptions = new ObservableCollection<EditFileHashlinePermissionOption>
            {
                new(
                    EditFileHashlinePermissionScope.LateralFileSystemOnly,
                    "LateralFS only",
                    "The model can write only inside LateralFS virtual folders. LateralFS\\NodeName\\... shortcuts are supported and checked."),
                new(
                    EditFileHashlinePermissionScope.FullAccess,
                    "Full access",
                    "The model can write any file path that the Ferrita process account can access. LateralFS shortcuts still work.")
            };

            _selectedPermission = PermissionOptions.FirstOrDefault(option => option.Scope == settings.PermissionScope)
                ?? PermissionOptions[0];
        }

        public ObservableCollection<EditFileHashlinePermissionOption> PermissionOptions { get; }

        public EditFileHashlinePermissionOption? SelectedPermission
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
                var scope = SelectedPermission?.Scope ?? EditFileHashlinePermissionScope.LateralFileSystemOnly;
                return scope == EditFileHashlinePermissionScope.FullAccess
                    ? "Current permission: FullAccess. Normal absolute/relative paths and LateralFS shortcuts are accepted."
                    : "Current permission: LateralFileSystemOnly. Use LateralFS\\NodeName\\relative\\file.ext or an actual path under a LateralFS virtual root.";
            }
        }

        public EditFileHashlineToolSettings ToSettings()
        {
            return new EditFileHashlineToolSettings
            {
                PermissionScope = SelectedPermission?.Scope ?? EditFileHashlinePermissionScope.LateralFileSystemOnly
            };
        }
    }

    internal sealed class EditFileHashlineToolConfigurationPresenter : FerritaToolConfigurationPresenter
    {
        private readonly EditFileHashlineToolConfigurationViewModel _viewModel;
        private readonly FrameworkElement _view;

        public EditFileHashlineToolConfigurationPresenter(FerritaToolConfigurationEditorContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            var settings = EditFileHashlineToolSettings.FromConfiguration(context.InitialConfiguration);
            _viewModel = new EditFileHashlineToolConfigurationViewModel(settings, RaiseConfigurationChanged);
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
                errorMessage = $"EditFileHashline configuration is invalid: {ex.Message}";
                return false;
            }
        }

        private static FrameworkElement CreateView(EditFileHashlineToolConfigurationViewModel viewModel)
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
                DisplayMemberPath = nameof(EditFileHashlinePermissionOption.DisplayName),
                Margin = new Thickness(0, 0, 0, 10)
            };
            comboBox.SetBinding(
                ItemsControl.ItemsSourceProperty,
                new Binding(nameof(EditFileHashlineToolConfigurationViewModel.PermissionOptions)));
            comboBox.SetBinding(
                ComboBox.SelectedItemProperty,
                new Binding(nameof(EditFileHashlineToolConfigurationViewModel.SelectedPermission))
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
                new Binding(nameof(EditFileHashlineToolConfigurationViewModel.PermissionDescription)));
            panel.Children.Add(description);

            var preview = new TextBlock
            {
                TextWrapping = TextWrapping.Wrap,
                Foreground = Brushes.White,
                Opacity = 0.72
            };
            preview.SetBinding(
                TextBlock.TextProperty,
                new Binding(nameof(EditFileHashlineToolConfigurationViewModel.PreviewText)));
            panel.Children.Add(preview);

            return panel;
        }
    }
}
