using System.IO;
using System.Text;
using System.Windows;
using Skyweaver.Controls.ChatSessionControl.Views;
using Skyweaver.Services.SkyweaverTools;

namespace Skyweaver.Tools
{
    public sealed class ReadTextFileTool : ISkyweaverTool, ISkyweaverToolInvocationPresentationProvider
    {
        public const string ToolName = "ReadTextFile";

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

        private static readonly SkyweaverToolDefinition s_definition = new(
            ToolName,
            "读取文本文件并返回完整内容。Encoding 为可选参数；默认以 UTF-8 为首选，但当文件可被明确探测为非 UTF-8 编码时，会自动改用探测到的编码，并在结果中说明。",
            "Script",
            [
                new SkyweaverToolParameterDefinition(
                    "FilePath",
                    "要读取的文本文件路径。相对路径会相对于当前工作区解析。",
                    SkyweaverToolParameterType.String,
                    isRequired: true),
                new SkyweaverToolParameterDefinition(
                    "Encoding",
                    "读取文件时使用的文本编码名称；省略时默认以 UTF-8 为首选，并在可明确探测到非 UTF-8 编码时自动切换到探测结果。",
                    SkyweaverToolParameterType.String,
                    isRequired: false,
                    defaultValue: "utf-8")
            ]);

        public SkyweaverToolDefinition Definition => s_definition;

        public FrameworkElement? CreateInvocationPresentation(SkyweaverToolInvocationPresentationContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            return ToolInvocationCardFactory.Create(
                context,
                [
                    new ToolInvocationCardFieldDefinition("文件路径", "FilePath", "等待文件路径..."),
                    new ToolInvocationCardFieldDefinition("编码", "Encoding", "默认按 UTF-8 处理")
                ]);
        }

        public async Task<SkyweaverToolResult> ExecuteAsync(
            SkyweaverToolContext context,
            SkyweaverToolArguments arguments,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var requestedPath = arguments.GetString("FilePath") ?? string.Empty;
            string? resolvedPath = null;

            try
            {
                resolvedPath = ToolFileSystemHelper.ResolvePath(requestedPath, context.WorkspacePath);
                var relativePath = ToolFileSystemHelper.TryGetWorkspaceRelativePath(context.WorkspacePath, resolvedPath);

                if (Directory.Exists(resolvedPath))
                {
                    return SkyweaverToolResult.Failure(
                        $"Path points to a directory, not a text file: {resolvedPath}",
                        BuildData(
                            resolvedPath,
                            relativePath,
                            requestedEncodingName: null,
                            requestedEncodingWasDefault: true,
                            effectiveEncodingName: null,
                            isAutoDetected: false,
                            detectedEncodingName: null,
                            detectionReason: null,
                            encodingNotice: null,
                            characterCount: null,
                            lineCount: null,
                            byteCount: null));
                }

                if (!File.Exists(resolvedPath))
                {
                    return SkyweaverToolResult.Failure(
                        $"Text file not found: {resolvedPath}",
                        BuildData(
                            resolvedPath,
                            relativePath,
                            requestedEncodingName: null,
                            requestedEncodingWasDefault: true,
                            effectiveEncodingName: null,
                            isAutoDetected: false,
                            detectedEncodingName: null,
                            detectionReason: null,
                            encodingNotice: null,
                            characterCount: null,
                            lineCount: null,
                            byteCount: null));
                }

                var fileBytes = await File.ReadAllBytesAsync(resolvedPath, cancellationToken).ConfigureAwait(false);
                var encodingDecision = ResolveEncodingDecision(fileBytes, arguments.GetString("Encoding"));
                var content = DecodeContent(fileBytes, encodingDecision.EffectiveEncoding);
                var lineCount = ToolFileSystemHelper.CountLines(content);

                return SkyweaverToolResult.Success(
                    BuildContent(
                        resolvedPath,
                        relativePath,
                        encodingDecision,
                        content,
                        lineCount,
                        fileBytes.LongLength),
                    BuildData(
                        resolvedPath,
                        relativePath,
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
                return SkyweaverToolResult.Failure(
                    $"Failed to read text file: {ex.Message}",
                    BuildData(
                        resolvedPath,
                        ToolFileSystemHelper.TryGetWorkspaceRelativePath(context.WorkspacePath, resolvedPath ?? string.Empty),
                        requestedEncodingName: null,
                        requestedEncodingWasDefault: true,
                        effectiveEncodingName: null,
                        isAutoDetected: false,
                        detectedEncodingName: null,
                        detectionReason: null,
                        encodingNotice: null,
                        characterCount: null,
                        lineCount: null,
                        byteCount: null));
            }
        }

        private static string BuildContent(
            string resolvedPath,
            string? relativePath,
            EncodingDecision encodingDecision,
            string content,
            int lineCount,
            long byteCount)
        {
            var builder = new StringBuilder(Math.Max(content.Length + 512, 768));
            builder.AppendLine($"Path: {resolvedPath}");

            if (!string.IsNullOrWhiteSpace(relativePath))
            {
                builder.AppendLine($"WorkspaceRelativePath: {relativePath}");
            }

            builder.AppendLine($"RequestedEncoding: {encodingDecision.RequestedEncodingDisplayName}");
            builder.AppendLine($"Encoding: {encodingDecision.EffectiveEncodingName}");

            if (encodingDecision.IsAutoDetected)
            {
                builder.AppendLine($"DetectedEncoding: {encodingDecision.DetectedEncodingName}");
                builder.AppendLine($"DetectedEncodingReason: {encodingDecision.DetectionReason}");
                builder.AppendLine($"EncodingNotice: {encodingDecision.EncodingNotice}");
            }

            builder.AppendLine($"Characters: {content.Length}");
            builder.AppendLine($"Lines: {lineCount}");
            builder.AppendLine($"Bytes: {byteCount}");
            builder.AppendLine();
            builder.AppendLine("----- BEGIN FILE CONTENT -----");

            if (content.Length > 0)
            {
                builder.Append(content);
                if (!EndsWithLineBreak(content))
                {
                    builder.AppendLine();
                }
            }

            builder.AppendLine("----- END FILE CONTENT -----");
            return builder.ToString().TrimEnd();
        }

        private static IReadOnlyDictionary<string, object?> BuildData(
            string? resolvedPath,
            string? relativePath,
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
                ["resolvedPath"] = resolvedPath,
                ["workspaceRelativePath"] = relativePath,
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

        private static bool EndsWithLineBreak(string text)
        {
            return text.EndsWith("\r", StringComparison.Ordinal)
                || text.EndsWith("\n", StringComparison.Ordinal);
        }
    }
}
