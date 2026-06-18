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
    /// SearchReplaceEverything 工具，允许在指定的目录及其子项中，
    /// 并行、编码安全地在“目录名”、“文件名”以及“文件内容”中进行查找与替换。
    /// 它也同时作用于传入的这些目录的目录名本身。
    /// </summary>
    public sealed class SearchReplaceEverythingTool :
        IFerritaTool,
        IFerritaToolConfigurationProvider,
        IFerritaToolInvocationPresentationProvider,
        IFerritaToolPromptDescriptionProvider
    {
        public const string ToolName = "SearchReplaceEverything";

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
            return BuildDefinition(ToolFileSystemPermissionSettings.FromConfiguration(configuration, "SearchReplaceEverythingSettings"));
        }

        public FerritaToolConfigurationPresenter? CreateConfigurationPresenter(FerritaToolConfigurationEditorContext context)
        {
            return new ToolFileSystemPermissionConfigurationPresenter(context, "SearchReplaceEverythingSettings", ToolName);
        }

        public string GetPromptDescription(FerritaToolPromptDescriptionContext context)
        {
            var settings = ToolFileSystemPermissionSettings.FromConfiguration(context.ConfigurationState, "SearchReplaceEverythingSettings");
            return BuildDescription(settings);
        }

        public FrameworkElement? CreateInvocationPresentation(FerritaToolInvocationPresentationContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            return ToolInvocationCardFactory.Create(
                context,
                [
                    new ToolInvocationCardFieldDefinition("Directories", "Directories", "Waiting for directories XML..."),
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

            var settings = ToolFileSystemPermissionSettings.FromConfiguration(context.CurrentToolConfiguration, "SearchReplaceEverythingSettings");
            var dirsXml = arguments.GetString("Directories") ?? string.Empty;
            var searchString = arguments.GetString("SearchString") ?? string.Empty;
            var targetString = arguments.GetString("TargetString") ?? string.Empty;
            var explicitEncoding = GetExplicitEncodingName(arguments);

            if (string.IsNullOrEmpty(searchString))
            {
                return FerritaToolResult.Failure("SearchString cannot be empty.");
            }

            if (string.IsNullOrWhiteSpace(dirsXml))
            {
                return FerritaToolResult.Failure("Directories parameter cannot be empty.");
            }

            XElement rootElement;
            try
            {
                rootElement = XElement.Parse(dirsXml);
            }
            catch (Exception ex)
            {
                return FerritaToolResult.Failure($"Failed to parse 'Directories' XML: {ex.Message}");
            }

            var requestedPaths = rootElement.Elements("Directory")
                .Select(e => e.Value.Trim())
                .Where(v => v.Length > 0)
                .ToList();

            if (requestedPaths.Count == 0)
            {
                return FerritaToolResult.Failure("No non-empty <Directory> elements found in 'Directories'.");
            }

            var resolvedDirs = new ConcurrentDictionary<string, byte>(StringComparer.OrdinalIgnoreCase);
            var errors = new ConcurrentQueue<string>();

            var parallelOptions = new ParallelOptions
            {
                MaxDegreeOfParallelism = Environment.ProcessorCount,
                CancellationToken = cancellationToken
            };

            // 1. 并行解析和验证目录路径
            await Parallel.ForEachAsync(requestedPaths, parallelOptions, async (requestedPath, ct) =>
            {
                try
                {
                    var pathInfo = ToolFileSystemMutationSupport.ResolveAuthorizedPath(requestedPath, context.WorkspacePath, settings.PermissionScope);
                    var resolvedPath = pathInfo.ResolvedPath;
                    if (Directory.Exists(resolvedPath))
                    {
                        resolvedDirs.TryAdd(resolvedPath, 0);
                    }
                    else
                    {
                        errors.Enqueue($"Directory not found: {requestedPath} (resolved to: {resolvedPath})");
                    }
                }
                catch (Exception ex)
                {
                    errors.Enqueue($"Failed to resolve directory '{requestedPath}': {ex.Message}");
                }
            }).ConfigureAwait(false);

            if (resolvedDirs.Count == 0)
            {
                var errorSummary = errors.Count > 0 ? string.Join("; ", errors) : "No valid directories found.";
                return FerritaToolResult.Failure($"No directories were processed. Errors: {errorSummary}");
            }

            // 2. 并行递归收集所有待处理的文件和文件夹，以便后续处理
            var allFiles = new ConcurrentDictionary<string, byte>(StringComparer.OrdinalIgnoreCase);
            var allPathsToRename = new ConcurrentDictionary<string, byte>(StringComparer.OrdinalIgnoreCase);

            if (resolvedDirs.Count > 0)
            {
                // 首先将传入的根目录本身加入待重命名的集合
                foreach (var dir in resolvedDirs.Keys)
                {
                    allPathsToRename.TryAdd(dir, 0);
                }

                var files = new ConcurrentBag<string>();
                var subDirs = new ConcurrentBag<string>();

                await ToolFileSystemHelper.CrawlDirectoriesAsync(
                    resolvedDirs.Keys,
                    files,
                    subDirs,
                    path =>
                    {
                        var lower = path.ToLowerInvariant();
                        return lower.Contains(@"\.git\") ||
                               lower.Contains(@"\.vs\") ||
                               lower.Contains(@"\bin\") ||
                               lower.Contains(@"\obj\");
                    },
                    parallelOptions).ConfigureAwait(false);

                foreach (var file in files)
                {
                    var lower = file.ToLowerInvariant();
                    if (lower.Contains(@"\.dotnet\"))
                    {
                        continue;
                    }
                    allFiles.TryAdd(file, 0);
                    allPathsToRename.TryAdd(file, 0);
                }
                foreach (var subDir in subDirs)
                {
                    allPathsToRename.TryAdd(subDir, 0);
                }
            }

            // 3. 执行第一步：修改文件内容 (并行且编码安全)
            var contentProcessedCount = 0;
            var contentModifiedCount = 0;
            var contentFailures = new ConcurrentQueue<string>();

            if (allFiles.Count > 0)
            {
                await Parallel.ForEachAsync(allFiles.Keys, parallelOptions, async (filePath, ct) =>
                {
                    try
                    {
                        Interlocked.Increment(ref contentProcessedCount);

                        var originalBytes = await File.ReadAllBytesAsync(filePath, ct).ConfigureAwait(false);
                        if (originalBytes.Length == 0)
                        {
                            return; // 安全跳过空文件
                        }

                        if (!TryResolveEncodingDecision(originalBytes, explicitEncoding, out var encodingDecision, out var decodeError))
                        {
                            contentFailures.Enqueue($"{filePath}: Encoding detection failed: {decodeError}");
                            return;
                        }

                        var originalContent = DecodeContent(originalBytes, encodingDecision!);
                        if (!originalContent.Contains(searchString, StringComparison.Ordinal))
                        {
                            return; // 不含搜索词，跳过
                        }

                        var updatedContent = originalContent.Replace(searchString, targetString, StringComparison.Ordinal);
                        var updatedBytes = EncodeContent(updatedContent, encodingDecision!);
                        await File.WriteAllBytesAsync(filePath, updatedBytes, ct).ConfigureAwait(false);

                        Interlocked.Increment(ref contentModifiedCount);
                    }
                    catch (Exception ex)
                    {
                        contentFailures.Enqueue($"{filePath}: {ex.Message}");
                    }
                }).ConfigureAwait(false);
            }

            // 4. 执行第二步：自底向上重命名文件和文件夹 (包括传入的目录名本身)
            var pathsByDepth = allPathsToRename.Keys
                .GroupBy(GetPathDepth)
                .OrderByDescending(g => g.Key)
                .ToList();

            var finalPaths = new ConcurrentDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var lockObj = new object();
            var renameFailures = new ConcurrentQueue<string>();
            var renamedCount = 0;

            foreach (var depthGroup in pathsByDepth)
            {
                cancellationToken.ThrowIfCancellationRequested();

                await Parallel.ForEachAsync(depthGroup, parallelOptions, async (path, ct) =>
                {
                    var isDir = Directory.Exists(path);
                    var isFile = File.Exists(path);

                    if (!isDir && !isFile)
                    {
                        return;
                    }

                    var parentDir = Path.GetDirectoryName(path);
                    if (string.IsNullOrEmpty(parentDir))
                    {
                        // 根目录无父目录，不能做通常重命名，或者由父路径处理
                        return;
                    }

                    var currentName = Path.GetFileName(path);
                    if (!currentName.Contains(searchString, StringComparison.Ordinal))
                    {
                        return;
                    }

                    var newName = currentName.Replace(searchString, targetString, StringComparison.Ordinal);
                    if (newName.Contains(Path.DirectorySeparatorChar) || newName.Contains(Path.AltDirectorySeparatorChar))
                    {
                        renameFailures.Enqueue($"Skipped rename for '{path}': New name '{newName}' contains invalid directory separators.");
                        return;
                    }

                    var newPath = Path.Combine(parentDir, newName);

                    if (File.Exists(newPath) || Directory.Exists(newPath))
                    {
                        renameFailures.Enqueue($"Skipped rename for '{path}': Target path '{newPath}' already exists.");
                        return;
                    }

                    try
                    {
                        if (isDir)
                        {
                            try
                            {
                                Directory.Move(path, newPath);
                            }
                            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or NotSupportedException)
                            {
                                MoveDirectoryFallback(path, newPath);
                            }
                            
                            lock (lockObj)
                            {
                                var oldPrefix = path + Path.DirectorySeparatorChar;
                                var newPrefix = newPath + Path.DirectorySeparatorChar;

                                var keysToUpdate = finalPaths.Where(kv => kv.Value.StartsWith(oldPrefix, StringComparison.OrdinalIgnoreCase)).Select(kv => kv.Key).ToList();
                                foreach (var key in keysToUpdate)
                                {
                                    var currentVal = finalPaths[key];
                                    var relative = currentVal.Substring(oldPrefix.Length);
                                    finalPaths[key] = newPrefix + relative;
                                }

                                finalPaths[path] = newPath;
                            }
                        }
                        else
                        {
                            File.Move(path, newPath);
                            lock (lockObj)
                            {
                                finalPaths[path] = newPath;
                            }
                        }

                        Interlocked.Increment(ref renamedCount);
                    }
                    catch (Exception ex)
                    {
                        renameFailures.Enqueue($"Failed to rename '{path}' to '{newName}': {ex.Message}");
                    }
                }).ConfigureAwait(false);
            }

            // 5. 对受影响的文件，进行 RAG 刷新 (并行)
            // 收集所有内容被修改过的文件路径，在经历了重命名之后，它们的最新位置是多少
            var filesToRefresh = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            // 首先，加入成功重命名的所有文件（如果它们现在存在）
            foreach (var kv in finalPaths)
            {
                if (File.Exists(kv.Value))
                {
                    filesToRefresh.Add(kv.Value);
                }
            }

            // 其次，加入那些内容被修改过但名字没变的文件（它们的祖先目录可能被重命名了）
            foreach (var file in allFiles.Keys)
            {
                // 查找该文件经历重命名映射后的最新路径
                var latestPath = file;
                if (finalPaths.TryGetValue(file, out var mappedPath))
                {
                    latestPath = mappedPath;
                }
                else
                {
                    // 检查是否有父目录被重命名的映射
                    foreach (var kv in finalPaths)
                    {
                        var parentPrefix = kv.Key + Path.DirectorySeparatorChar;
                        if (file.StartsWith(parentPrefix, StringComparison.OrdinalIgnoreCase))
                        {
                            var relative = file.Substring(parentPrefix.Length);
                            latestPath = kv.Value + Path.DirectorySeparatorChar + relative;
                            break;
                        }
                    }
                }

                if (File.Exists(latestPath))
                {
                    filesToRefresh.Add(latestPath);
                }
            }

            if (filesToRefresh.Count > 0)
            {
                await Parallel.ForEachAsync(filesToRefresh, parallelOptions, async (filePath, ct) =>
                {
                    try
                    {
                        await AerialCityRagToolSync.RefreshFileAsync(filePath, context.WorkspacePath, ct).ConfigureAwait(false);
                    }
                    catch
                    {
                        // 忽略错误以保障整体执行
                    }
                }).ConfigureAwait(false);
            }

            // 6. 生成报告
            var builder = new StringBuilder();
            builder.AppendLine("=== Search and Replace Everything Summary ===");
            builder.AppendLine($"Files content scanned: {contentProcessedCount}");
            builder.AppendLine($"Files content modified: {contentModifiedCount}");
            builder.AppendLine($"Successfully renamed: {renamedCount}");

            if (errors.Count > 0)
            {
                builder.AppendLine("\nResolution Errors:");
                foreach (var err in errors)
                {
                    builder.AppendLine($"- {err}");
                }
            }

            if (contentFailures.Count > 0)
            {
                builder.AppendLine("\nContent Processing Failures (Skipped for safety):");
                foreach (var fail in contentFailures)
                {
                    builder.AppendLine($"- {fail}");
                }
            }

            if (finalPaths.Count > 0)
            {
                builder.AppendLine("\nRenamed Paths Details:");
                foreach (var kv in finalPaths)
                {
                    builder.AppendLine($"- '{kv.Key}' -> '{kv.Value}'");
                }
            }

            if (renameFailures.Count > 0)
            {
                builder.AppendLine("\nRename Failures or Skips:");
                foreach (var fail in renameFailures)
                {
                    builder.AppendLine($"- {fail}");
                }
            }

            var responseData = new Dictionary<string, object?>
            {
                ["contentScannedCount"] = contentProcessedCount,
                ["contentModifiedCount"] = contentModifiedCount,
                ["renamedCount"] = renamedCount,
                ["errorCount"] = errors.Count + contentFailures.Count + renameFailures.Count,
                ["renamedMappings"] = finalPaths.ToDictionary(kv => kv.Key, kv => kv.Value, StringComparer.OrdinalIgnoreCase),
                ["failures"] = renameFailures.Concat(contentFailures).ToArray()
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
                        "Directories",
                        "XML string containing multiple Directory elements. E.g., <Directories><Directory>path1</Directory></Directories>",
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
            return "Parallel search and replace for directory names, file names, and file contents recursively (including the source directories themselves). " +
                ToolFileSystemMutationSupport.BuildPermissionSentence(settings.PermissionScope);
        }

        private static int GetPathDepth(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return 0;
            }
            return path.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar).Length;
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

        private static void MoveDirectoryFallback(string sourceDir, string destDir)
        {
            CopyDirectoryRecursively(sourceDir, destDir);
            Directory.Delete(sourceDir, true);
        }

        private static void CopyDirectoryRecursively(string sourceDir, string destDir)
        {
            Directory.CreateDirectory(destDir);

            foreach (var file in Directory.GetFiles(sourceDir))
            {
                var destFile = Path.Combine(destDir, Path.GetFileName(file));
                File.Copy(file, destFile, true);
            }

            foreach (var subDir in Directory.GetDirectories(sourceDir))
            {
                var destSubDir = Path.Combine(destDir, Path.GetFileName(subDir));
                CopyDirectoryRecursively(subDir, destSubDir);
            }
        }

        private sealed record EncodingDecision(
            Encoding Encoding,
            string EffectiveEncodingName,
            byte[] Preamble);

        #endregion
    }
}
