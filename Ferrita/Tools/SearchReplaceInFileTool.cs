using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;
using Ferrita.Controls.ChatSessionControl.Views;
using Ferrita.Services.FerritaTools;

namespace Ferrita.Tools
{
    /// <summary>
    /// SearchReplaceInFile 工具，允许在指定的文件或文件夹内的所有文本文件中并行地查找并替换指定的字符串。
    /// </summary>
    public sealed class SearchReplaceInFileTool :
        IFerritaTool,
        IFerritaToolConfigurationProvider,
        IFerritaToolInvocationPresentationProvider,
        IFerritaToolPromptDescriptionProvider
    {
        public const string ToolName = "SearchReplaceInFile";

        private static readonly FerritaToolDefinition s_definition = BuildDefinition(new ToolFileSystemPermissionSettings());

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
            return BuildDefinition(ToolFileSystemPermissionSettings.FromConfiguration(configuration, "SearchReplaceInFileSettings"));
        }

        public FerritaToolConfigurationPresenter? CreateConfigurationPresenter(FerritaToolConfigurationEditorContext context)
        {
            return new ToolFileSystemPermissionConfigurationPresenter(context, "SearchReplaceInFileSettings", ToolName);
        }

        public string GetPromptDescription(FerritaToolPromptDescriptionContext context)
        {
            var settings = ToolFileSystemPermissionSettings.FromConfiguration(context.ConfigurationState, "SearchReplaceInFileSettings");
            return BuildDescription(settings);
        }

        public FrameworkElement? CreateInvocationPresentation(FerritaToolInvocationPresentationContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            return ToolInvocationCardFactory.Create(
                context,
                [
                    new ToolInvocationCardFieldDefinition("Files", "Files", "Waiting for files XML..."),
                    new ToolInvocationCardFieldDefinition("SearchString", "SearchString", "Waiting for search text..."),
                    new ToolInvocationCardFieldDefinition("TargetString", "TargetString", "Waiting for replacement text..."),
                    new ToolInvocationCardFieldDefinition("Encoding", "Encoding", "Default utf-8")
                ]);
        }

        public async Task<FerritaToolResult> ExecuteAsync(
            FerritaToolContext context,
            FerritaToolArguments arguments,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var settings = ToolFileSystemPermissionSettings.FromConfiguration(context.CurrentToolConfiguration, "SearchReplaceInFileSettings");
            var filesXml = arguments.GetString("Files") ?? string.Empty;
            var searchString = arguments.GetString("SearchString") ?? string.Empty;
            var targetString = arguments.GetString("TargetString") ?? string.Empty;
            var explicitEncoding = GetExplicitEncodingName(arguments);

            if (string.IsNullOrEmpty(searchString))
            {
                return FerritaToolResult.Failure("SearchString cannot be empty.");
            }

            if (string.IsNullOrWhiteSpace(filesXml))
            {
                return FerritaToolResult.Failure("Files parameter cannot be empty.");
            }

            XElement rootElement;
            try
            {
                rootElement = XElement.Parse(filesXml);
            }
            catch (Exception ex)
            {
                return FerritaToolResult.Failure($"Failed to parse 'Files' XML: {ex.Message}");
            }

            var requestedPaths = rootElement.Elements("File")
                .Select(e => e.Value.Trim())
                .Where(v => v.Length > 0)
                .ToList();

            if (requestedPaths.Count == 0)
            {
                return FerritaToolResult.Failure("No non-empty <File> elements found in 'Files'.");
            }

            var filesToProcess = new ConcurrentBag<string>();
            var errors = new ConcurrentQueue<string>();
            var rootDirsToCrawl = new ConcurrentBag<string>();

            var parallelOptions = new ParallelOptions
            {
                MaxDegreeOfParallelism = Environment.ProcessorCount,
                CancellationToken = cancellationToken
            };

            // 1. 并行解析和验证路径
            await Parallel.ForEachAsync(requestedPaths, parallelOptions, async (requestedPath, ct) =>
            {
                try
                {
                    var pathInfo = ToolFileSystemMutationSupport.ResolveAuthorizedPath(requestedPath, context.WorkspacePath, settings.PermissionScope);
                    var resolvedPath = pathInfo.ResolvedPath;

                    if (Directory.Exists(resolvedPath))
                    {
                        rootDirsToCrawl.Add(resolvedPath);
                    }
                    else if (File.Exists(resolvedPath))
                    {
                        filesToProcess.Add(resolvedPath);
                    }
                    else
                    {
                        errors.Enqueue($"Path not found: {requestedPath} (resolved to: {resolvedPath})");
                    }
                }
                catch (Exception ex)
                {
                    errors.Enqueue($"Failed to resolve path '{requestedPath}': {ex.Message}");
                }
            }).ConfigureAwait(false);

            // 2. 并行爬取所有指定的根目录下的文件
            if (rootDirsToCrawl.Count > 0)
            {
                var dummyDirs = new ConcurrentBag<string>();
                await ToolFileSystemHelper.CrawlDirectoriesAsync(
                    rootDirsToCrawl,
                    filesToProcess,
                    dummyDirs,
                    path =>
                    {
                        var lower = path.ToLowerInvariant();
                        return lower.Contains(@"\.git\") ||
                               lower.Contains(@"\.vs\") ||
                               lower.Contains(@"\bin\") ||
                               lower.Contains(@"\obj\") ||
                               lower.Contains(@"\.dotnet\");
                    },
                    parallelOptions).ConfigureAwait(false);
            }

            if (filesToProcess.Count == 0)
            {
                var errorSummary = errors.Count > 0
                    ? string.Join("; ", errors)
                    : "No files found to process.";
                return FerritaToolResult.Failure($"No files were processed. Errors: {errorSummary}");
            }

            // 3. 并行且编码安全地处理文件
            var processedCount = 0;
            var modifiedCount = 0;
            var fileResults = new ConcurrentQueue<FileProcessResult>();

            await Parallel.ForEachAsync(filesToProcess, parallelOptions, async (filePath, ct) =>
            {
                try
                {
                    Interlocked.Increment(ref processedCount);

                    var originalBytes = await File.ReadAllBytesAsync(filePath, ct).ConfigureAwait(false);
                    if (originalBytes.Length == 0)
                    {
                        return; // 安全跳过空文件
                    }

                    if (!TryResolveEncodingDecision(originalBytes, explicitEncoding, out var encodingDecision, out var decodeError))
                    {
                        fileResults.Enqueue(new FileProcessResult(filePath, isSuccess: false, message: $"Encoding detection failed: {decodeError}"));
                        return;
                    }

                    var originalContent = DecodeContent(originalBytes, encodingDecision!);
                    if (!originalContent.Contains(searchString, StringComparison.Ordinal))
                    {
                        return; // 不含搜索词，跳过
                    }

                    var occurrenceCount = CountOccurrences(originalContent, searchString);
                    var updatedContent = originalContent.Replace(searchString, targetString, StringComparison.Ordinal);

                    var updatedBytes = EncodeContent(updatedContent, encodingDecision!);
                    await File.WriteAllBytesAsync(filePath, updatedBytes, ct).ConfigureAwait(false);

                    Interlocked.Increment(ref modifiedCount);

                    try
                    {
                        await AerialCityRagToolSync.RefreshFileAsync(filePath, context.WorkspacePath, ct).ConfigureAwait(false);
                    }
                    catch
                    {
                        // 忽略 RAG 刷新时的错误，防止中断重构
                    }

                    fileResults.Enqueue(new FileProcessResult(filePath, isSuccess: true, occurrenceCount: occurrenceCount));
                }
                catch (Exception ex)
                {
                    fileResults.Enqueue(new FileProcessResult(filePath, isSuccess: false, message: ex.Message));
                }
            }).ConfigureAwait(false);

            // 3. 构建结果报告
            var builder = new StringBuilder();
            builder.AppendLine($"=== Search and Replace Summary ===");
            builder.AppendLine($"Total files scanned: {processedCount}");
            builder.AppendLine($"Files modified: {modifiedCount}");

            if (errors.Count > 0)
            {
                builder.AppendLine("\nResolution Errors:");
                foreach (var err in errors)
                {
                    builder.AppendLine($"- {err}");
                }
            }

            var successRuns = fileResults.Where(r => r.IsSuccess).ToList();
            var failedRuns = fileResults.Where(r => !r.IsSuccess).ToList();

            if (successRuns.Count > 0)
            {
                builder.AppendLine("\nModified Files Details:");
                foreach (var run in successRuns)
                {
                    builder.AppendLine($"- {Path.GetFileName(run.FilePath)} ({run.OccurrenceCount} replacements) at {run.FilePath}");
                }
            }

            if (failedRuns.Count > 0)
            {
                builder.AppendLine("\nProcessing Failures (Skipped for safety):");
                foreach (var run in failedRuns)
                {
                    builder.AppendLine($"- {run.FilePath}: {run.Message}");
                }
            }

            var responseData = new Dictionary<string, object?>
            {
                ["scannedCount"] = processedCount,
                ["modifiedCount"] = modifiedCount,
                ["errorCount"] = errors.Count + failedRuns.Count,
                ["modifiedFiles"] = successRuns.Select(r => r.FilePath).ToArray(),
                ["failedFiles"] = failedRuns.Select(r => r.FilePath).ToArray()
            };

            return FerritaToolResult.Success(builder.ToString().TrimEnd(), responseData);
        }

        private static FerritaToolDefinition BuildDefinition(ToolFileSystemPermissionSettings settings)
        {
            return new FerritaToolDefinition(
                ToolName,
                BuildDescription(settings),
                "Script",
                [
                    new FerritaToolParameterDefinition(
                        "Files",
                        "XML string containing multiple File elements. E.g., <Files><File>path1</File><File>path2</File></Files>",
                        FerritaToolParameterType.String,
                        isRequired: true),
                    new FerritaToolParameterDefinition(
                        "SearchString",
                        "Exact text to find. Matching is ordinal and case-sensitive.",
                        FerritaToolParameterType.String,
                        isRequired: true),
                    new FerritaToolParameterDefinition(
                        "TargetString",
                        "Replacement text.",
                        FerritaToolParameterType.String,
                        isRequired: true),
                    new FerritaToolParameterDefinition(
                        "Encoding",
                        "Optional text encoding name. Default is utf-8.",
                        FerritaToolParameterType.String,
                        isRequired: false,
                        defaultValue: "utf-8")
                ],
                defaultToolKitKeys: ["Refactor"]);
        }

        private static string BuildDescription(ToolFileSystemPermissionSettings settings)
        {
            return "Parallel and encoding-safe search and replace within specified files and folders. " +
                ToolFileSystemMutationSupport.BuildPermissionSentence(settings.PermissionScope);
        }

        private sealed class FileProcessResult
        {
            public FileProcessResult(string filePath, bool isSuccess, int occurrenceCount = 0, string? message = null)
            {
                FilePath = filePath;
                IsSuccess = isSuccess;
                OccurrenceCount = occurrenceCount;
                Message = message;
            }

            public string FilePath { get; }
            public bool IsSuccess { get; }
            public int OccurrenceCount { get; }
            public string? Message { get; }
        }

        #region Encoding Safe Utilities

        private static string? GetExplicitEncodingName(FerritaToolArguments arguments)
        {
            return arguments.RawArguments.TryGetValue("Encoding", out var rawEncoding) &&
                !string.IsNullOrWhiteSpace(rawEncoding)
                ? rawEncoding.Trim()
                : null;
        }

        private static bool TryResolveEncodingDecision(
            byte[] fileBytes,
            string? explicitEncodingName,
            out EncodingDecision? decision,
            out string? errorMessage)
        {
            decision = null;
            errorMessage = null;

            var requestedEncodingWasDefault = string.IsNullOrWhiteSpace(explicitEncodingName);
            var requestedEncodingName = requestedEncodingWasDefault
                ? "utf-8"
                : explicitEncodingName!.Trim();

            try
            {
                if (string.Equals(requestedEncodingName, "utf-8", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(requestedEncodingName, "utf8", StringComparison.OrdinalIgnoreCase))
                {
                    if (TryDetectUnicodeEncoding(fileBytes, out var detectedEncoding, out var preamble))
                    {
                        decision = new EncodingDecision(detectedEncoding, detectedEncoding.WebName, preamble);
                        return true;
                    }

                    if (!TryDecodeStrict(fileBytes, 0, fileBytes.Length, s_utf8WithoutBomStrict, out _))
                    {
                        errorMessage = "The file is not valid UTF-8 and no UTF BOM/UTF-16/UTF-32 pattern was detected. Decode fallback.";
                        return false;
                    }

                    decision = new EncodingDecision(s_utf8WithoutBomStrict, s_utf8WithoutBomStrict.WebName, Array.Empty<byte>());
                    return true;
                }

                var resolvedEncoding = ResolveStrictEncoding(requestedEncodingName);
                var resolvedPreamble = ResolveMatchingPreamble(fileBytes, resolvedEncoding);
                var offset = resolvedPreamble.Length;

                if (!TryDecodeStrict(fileBytes, offset, fileBytes.Length - offset, resolvedEncoding, out _))
                {
                    errorMessage = $"The file is not valid under specified encoding '{requestedEncodingName}'. Decode fallback.";
                    return false;
                }

                decision = new EncodingDecision(resolvedEncoding, resolvedEncoding.WebName, resolvedPreamble);
                return true;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
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
            out byte[] preamble)
        {
            encoding = s_utf8WithoutBomStrict;
            preamble = Array.Empty<byte>();

            if (fileBytes.Length == 0)
            {
                return false;
            }

            if (HasPrefix(fileBytes, s_utf32BigEndianPreamble))
            {
                encoding = s_utf32BigEndianStrict;
                preamble = s_utf32BigEndianPreamble;
                return true;
            }

            if (HasPrefix(fileBytes, s_utf32LittleEndianPreamble))
            {
                encoding = s_utf32LittleEndianStrict;
                preamble = s_utf32LittleEndianPreamble;
                return true;
            }

            if (HasPrefix(fileBytes, s_utf8Preamble))
            {
                encoding = s_utf8WithoutBomStrict;
                preamble = s_utf8Preamble;
                return true;
            }

            if (HasPrefix(fileBytes, s_utf16LittleEndianPreamble))
            {
                encoding = s_utf16LittleEndianStrict;
                preamble = s_utf16LittleEndianPreamble;
                return true;
            }

            if (HasPrefix(fileBytes, s_utf16BigEndianPreamble))
            {
                encoding = s_utf16BigEndianStrict;
                preamble = s_utf16BigEndianPreamble;
                return true;
            }

            if (TryDetectUtf32WithoutBom(fileBytes, out encoding))
            {
                return true;
            }

            if (TryDetectUtf16WithoutBom(fileBytes, out encoding))
            {
                return true;
            }

            return false;
        }

        private static bool TryDetectUtf32WithoutBom(byte[] fileBytes, out Encoding encoding)
        {
            encoding = s_utf8WithoutBomStrict;
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
                return true;
            }

            if (likelyBigEndian >= quartetCount * 0.6 && likelyLittleEndian == 0)
            {
                encoding = s_utf32BigEndianStrict;
                return true;
            }

            return false;
        }

        private static bool TryDetectUtf16WithoutBom(byte[] fileBytes, out Encoding encoding)
        {
            encoding = s_utf8WithoutBomStrict;
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
                return true;
            }

            if (likelyBigEndian >= pairCount * 0.6 && likelyLittleEndian <= pairCount * 0.1)
            {
                encoding = s_utf16BigEndianStrict;
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

        private static int CountOccurrences(string text, string search)
        {
            if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(search))
            {
                return 0;
            }

            var count = 0;
            var index = 0;
            while (index < text.Length)
            {
                var foundIndex = text.IndexOf(search, index, StringComparison.Ordinal);
                if (foundIndex < 0)
                {
                    break;
                }

                count++;
                index = foundIndex + search.Length;
            }

            return count;
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

        private sealed record EncodingDecision(
            Encoding Encoding,
            string EffectiveEncodingName,
            byte[] Preamble);

        #endregion
    }
}
