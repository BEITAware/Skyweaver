using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Ferrita.Controls.ChatSessionControl.Views;
using Ferrita.Services.FerritaTools;

namespace Ferrita.Tools
{
    /// <summary>
    /// 读取文本文件，并为每一行加上前导哈希前缀的工具。
    /// </summary>
    public sealed class ReadTextFileHashlineTool :
        IFerritaTool,
        IFerritaToolInvocationPresentationProvider,
        IFerritaToolPromptDescriptionProvider
    {
        public const string ToolName = "ReadTextFileHashline";

        private sealed record ReadTargetPath(
            string ResolvedPath,
            string? WorkspaceRelativePath,
            string? LateralNodeName,
            string? LateralNodeId,
            string? LateralNodeVirtualRootPath,
            string? LateralRelativePath,
            bool UsedLateralShortcut);

        private sealed record EncodingDecision(
            Encoding EffectiveEncoding,
            string EffectiveEncodingName,
            string RequestedEncodingName,
            bool RequestedEncodingWasDefault,
            bool IsAutoDetected,
            string? DetectedEncodingName,
            string? DetectionReason)
        {
            public string RequestedEncodingDisplayName => RequestedEncodingWasDefault
                ? $"{RequestedEncodingName} (default)"
                : RequestedEncodingName;

            public string? EncodingNotice => IsAutoDetected && !string.IsNullOrWhiteSpace(DetectedEncodingName)
                ? $"This file is not UTF-8. The tool detected {DetectedEncodingName} ({DetectionReason}) and opened it using that encoding. Keep future edits in {DetectedEncodingName} unless you intentionally convert the file."
                : null;
        }

        private static readonly UTF8Encoding s_utf8WithoutBom = new(false);
        private static readonly UTF32Encoding s_utf32LittleEndian = new(false, true);
        private static readonly UTF32Encoding s_utf32BigEndian = new(true, true);
        private const long MaximumWholeTextFileBytes = 10L * 1024 * 1024;
        private static readonly HashSet<string> s_nonTextExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".exe", ".dll", ".sys", ".msi", ".com", ".scr", ".bin", ".dat",
            ".iso", ".img", ".zip", ".7z", ".rar", ".tar", ".gz", ".bz2", ".xz",
            ".png", ".jpg", ".jpeg", ".gif", ".bmp", ".webp", ".ico",
            ".mp3", ".wav", ".m4a", ".ogg", ".flac", ".mp4", ".mov", ".avi", ".mkv",
            ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx"
        };

        private static readonly FerritaToolDefinition s_definition = new(
            ToolName,
            "Reads a text file and returns the content with a 4-character leading hash prefixed to each line in the format '|hash|line_content'. FilePath may be a normal absolute or relative path, or a LateralFS\\NodeName\\relative\\file.ext shortcut. Encoding is optional; utf-8 is preferred by default.",
            "Script",
            [
                new FerritaToolParameterDefinition(
                    "FilePath",
                    "Path of the text file to read. Relative paths resolve against the current workspace. You may also use LateralFS\\NodeName\\relative\\file.ext.",
                    FerritaToolParameterType.String,
                    isRequired: true),
                new FerritaToolParameterDefinition(
                    "Encoding",
                    "Text encoding name to use while reading. Default is utf-8.",
                    FerritaToolParameterType.String,
                    isRequired: false,
                    defaultValue: "utf-8")
            ],
            defaultAgentPermission: FerritaToolDefaultAgentPermission.Allow);

        public FerritaToolDefinition Definition => s_definition;

        public string GetPromptDescription(FerritaToolPromptDescriptionContext context)
        {
            ArgumentNullException.ThrowIfNull(context);
            return "Reads a text file and returns each line prefixed with its unique 4-character context hash (e.g., |a0z1|line content). Each line's hash is computed based on its 21-line local window (10 lines before, current line, 10 lines after) to prevent duplication issues. FilePath may be absolute, relative, or a LateralFS shortcut.";
        }

        public FrameworkElement? CreateInvocationPresentation(FerritaToolInvocationPresentationContext context)
        {
            ArgumentNullException.ThrowIfNull(context);
            return ToolInvocationCardFactory.Create(
                context,
                [
                    new ToolInvocationCardFieldDefinition("File path", "FilePath", "Waiting for file path..."),
                    new ToolInvocationCardFieldDefinition("Encoding", "Encoding", "Default utf-8 with safe auto-detection")
                ]);
        }

        public async Task<FerritaToolResult> ExecuteAsync(
            FerritaToolContext context,
            FerritaToolArguments arguments,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var requestedPath = arguments.GetString("FilePath") ?? string.Empty;
            ReadTargetPath? targetPath = null;

            try
            {
                targetPath = ResolveReadTargetPath(requestedPath, context.WorkspacePath);

                if (Directory.Exists(targetPath.ResolvedPath))
                {
                    return FerritaToolResult.Failure($"Path points to a directory, not a text file: {targetPath.ResolvedPath}");
                }

                if (!File.Exists(targetPath.ResolvedPath))
                {
                    return FerritaToolResult.Failure($"Text file not found: {targetPath.ResolvedPath}");
                }

                var textFileValidationError = ValidateWholeTextFileCandidate(targetPath.ResolvedPath, out var candidateByteCount);
                if (!string.IsNullOrWhiteSpace(textFileValidationError))
                {
                    return FerritaToolResult.Failure(textFileValidationError);
                }

                var fileBytes = await File.ReadAllBytesAsync(targetPath.ResolvedPath, cancellationToken).ConfigureAwait(false);
                var encodingDecision = ResolveEncodingDecision(fileBytes, arguments.GetString("Encoding"));
                var content = DecodeContent(fileBytes, encodingDecision.EffectiveEncoding);

                // 解析行结构并计算每一行的哈希
                var fileLines = HashlineHelper.ParseFileLines(content);
                HashlineHelper.CalculateHashes(fileLines);

                var lineCount = fileLines.Count;

                return FerritaToolResult.Success(
                    BuildContent(
                        targetPath,
                        encodingDecision,
                        fileLines,
                        lineCount,
                        fileBytes.LongLength),
                    BuildData(
                        targetPath,
                        encodingDecision.RequestedEncodingName,
                        encodingDecision.RequestedEncodingWasDefault,
                        encodingDecision.EffectiveEncodingName,
                        encodingDecision.IsAutoDetected,
                        encodingDecision.DetectedEncodingName,
                        encodingDecision.DetectionReason,
                        encodingDecision.EncodingNotice,
                        content.Length,
                        lineCount,
                        fileBytes.LongLength));
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or InvalidOperationException or ArgumentException or DecoderFallbackException or NotSupportedException)
            {
                return FerritaToolResult.Failure($"Failed to read text file with Hashline: {ex.Message}");
            }
        }

        private static string BuildContent(
            ReadTargetPath targetPath,
            EncodingDecision encodingDecision,
            List<HashlineHelper.FileLine> fileLines,
            int lineCount,
            long byteCount)
        {
            var builder = new StringBuilder(Math.Max(lineCount * 80 + 512, 768));
            builder.AppendLine($"Path: {targetPath.ResolvedPath}");

            if (!string.IsNullOrWhiteSpace(targetPath.WorkspaceRelativePath))
            {
                builder.AppendLine($"WorkspaceRelativePath: {targetPath.WorkspaceRelativePath}");
            }

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
                builder.AppendLine($"EncodingNotice: {encodingDecision.EncodingNotice}");
            }

            builder.AppendLine($"Lines: {lineCount}");
            builder.AppendLine($"Bytes: {byteCount}");
            builder.AppendLine();
            builder.AppendLine("----- BEGIN FILE CONTENT -----");

            foreach (var line in fileLines)
            {
                builder.AppendLine($"|{line.Hash}|{line.Text}");
            }

            builder.AppendLine("----- END FILE CONTENT -----");
            return builder.ToString().TrimEnd();
        }

        private static IReadOnlyDictionary<string, object?> BuildData(
            ReadTargetPath? targetPath,
            string? requestedEncodingName,
            bool requestedEncodingWasDefault,
            string? effectiveEncodingName,
            bool isAutoDetected,
            string? detectedEncodingName,
            string? detectionReason,
            string? encodingNotice,
            int? characterCount,
            int? lineCount,
            long? byteCount)
        {
            return new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase)
            {
                ["resolvedPath"] = targetPath?.ResolvedPath,
                ["workspaceRelativePath"] = targetPath?.WorkspaceRelativePath,
                ["lateralNodeName"] = targetPath?.LateralNodeName,
                ["lateralNodeId"] = targetPath?.LateralNodeId,
                ["lateralNodeVirtualRootPath"] = targetPath?.LateralNodeVirtualRootPath,
                ["lateralRelativePath"] = targetPath?.LateralRelativePath,
                ["usedLateralShortcut"] = targetPath?.UsedLateralShortcut,
                ["requestedEncoding"] = requestedEncodingName,
                ["requestedEncodingWasDefault"] = requestedEncodingWasDefault,
                ["encoding"] = effectiveEncodingName,
                ["effectiveEncoding"] = effectiveEncodingName,
                ["encodingAutoDetected"] = isAutoDetected,
                ["detectedEncoding"] = detectedEncodingName,
                ["detectedEncodingReason"] = detectionReason,
                ["encodingNotice"] = encodingNotice,
                ["characterCount"] = characterCount,
                ["lineCount"] = lineCount,
                ["byteCount"] = byteCount
            };
        }

        private static string? ValidateWholeTextFileCandidate(string resolvedPath, out long? byteCount)
        {
            byteCount = null;
            var extension = Path.GetExtension(resolvedPath);
            if (s_nonTextExtensions.Contains(extension))
            {
                return $"Refusing to read a non-text file as text: {resolvedPath}. Pass the path only or use a purpose-built tool for this file type.";
            }

            try
            {
                var fileInfo = new FileInfo(resolvedPath);
                byteCount = fileInfo.Length;
                if (fileInfo.Length > MaximumWholeTextFileBytes)
                {
                    return $"Refusing to read a large file into the chat transcript: {resolvedPath} ({fileInfo.Length} bytes; limit {MaximumWholeTextFileBytes} bytes).";
                }
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or ArgumentException or NotSupportedException)
            {
                return $"Failed to inspect text file before reading: {ex.Message}";
            }

            return null;
        }

        private static ReadTargetPath ResolveReadTargetPath(string requestedPath, string? workspacePath)
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

            return new ReadTargetPath(
                resolvedPath,
                ToolFileSystemHelper.TryGetWorkspaceRelativePath(workspacePath, resolvedPath),
                lateralResolution?.NodeName,
                lateralResolution?.NodeId,
                lateralResolution?.NodeVirtualRootPath,
                lateralResolution?.RelativePath,
                lateralResolution?.UsedShortcut ?? false);
        }

        private static EncodingDecision ResolveEncodingDecision(byte[] fileBytes, string? requestedEncodingName)
        {
            var requestedEncodingWasDefault = string.IsNullOrWhiteSpace(requestedEncodingName);
            var normalizedRequestedEncoding = requestedEncodingWasDefault
                ? "utf-8"
                : requestedEncodingName!.Trim();

            if (ShouldAutoDetectForUtf8Request(normalizedRequestedEncoding) &&
                TryDetectNonUtf8Encoding(fileBytes, out var detectedEncoding, out var detectedEncodingName, out var detectionReason))
            {
                return new EncodingDecision(
                    detectedEncoding,
                    detectedEncodingName,
                    normalizedRequestedEncoding,
                    requestedEncodingWasDefault,
                    true,
                    detectedEncodingName,
                    detectionReason);
            }

            var resolvedEncoding = ToolFileSystemHelper.ResolveEncoding(normalizedRequestedEncoding);
            return new EncodingDecision(
                resolvedEncoding,
                resolvedEncoding.WebName,
                normalizedRequestedEncoding,
                requestedEncodingWasDefault,
                false,
                null,
                null);
        }

        private static bool ShouldAutoDetectForUtf8Request(string requestedEncodingName)
        {
            return string.Equals(
                ToolFileSystemHelper.ResolveEncoding(requestedEncodingName).WebName,
                s_utf8WithoutBom.WebName,
                StringComparison.OrdinalIgnoreCase);
        }

        private static bool TryDetectNonUtf8Encoding(
            byte[] fileBytes,
            out Encoding encoding,
            out string encodingName,
            out string detectionReason)
        {
            encoding = s_utf8WithoutBom;
            encodingName = s_utf8WithoutBom.WebName;
            detectionReason = string.Empty;

            if (fileBytes.Length == 0)
            {
                return false;
            }

            if (HasPrefix(fileBytes, 0x00, 0x00, 0xFE, 0xFF))
            {
                encoding = s_utf32BigEndian;
                encodingName = "utf-32-be";
                detectionReason = "byte-order mark 00 00 FE FF";
                return true;
            }

            if (HasPrefix(fileBytes, 0xFF, 0xFE, 0x00, 0x00))
            {
                encoding = s_utf32LittleEndian;
                encodingName = "utf-32-le";
                detectionReason = "byte-order mark FF FE 00 00";
                return true;
            }

            if (HasPrefix(fileBytes, 0xFF, 0xFE))
            {
                encoding = Encoding.Unicode;
                encodingName = "utf-16-le";
                detectionReason = "byte-order mark FF FE";
                return true;
            }

            if (HasPrefix(fileBytes, 0xFE, 0xFF))
            {
                encoding = Encoding.BigEndianUnicode;
                encodingName = "utf-16-be";
                detectionReason = "byte-order mark FE FF";
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
            encoding = s_utf8WithoutBom;
            encodingName = s_utf8WithoutBom.WebName;
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
                encoding = s_utf32LittleEndian;
                encodingName = "utf-32-le";
                detectionReason = "strong UTF-32 little-endian zero-byte pattern without BOM";
                return true;
            }

            if (likelyBigEndian >= quartetCount * 0.6 && likelyLittleEndian == 0)
            {
                encoding = s_utf32BigEndian;
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
            encoding = s_utf8WithoutBom;
            encodingName = s_utf8WithoutBom.WebName;
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
                encoding = Encoding.Unicode;
                encodingName = "utf-16-le";
                detectionReason = "strong UTF-16 little-endian zero-byte pattern without BOM";
                return true;
            }

            if (likelyBigEndian >= pairCount * 0.6 && likelyLittleEndian <= pairCount * 0.1)
            {
                encoding = Encoding.BigEndianUnicode;
                encodingName = "utf-16-be";
                detectionReason = "strong UTF-16 big-endian zero-byte pattern without BOM";
                return true;
            }

            return false;
        }

        private static string DecodeContent(byte[] fileBytes, Encoding encoding)
        {
            using var stream = new MemoryStream(fileBytes, writable: false);
            using var reader = new StreamReader(
                stream,
                encoding,
                detectEncodingFromByteOrderMarks: true,
                bufferSize: 1024,
                leaveOpen: false);
            return reader.ReadToEnd();
        }

        private static bool HasPrefix(byte[] fileBytes, params byte[] prefix)
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
    }
}
