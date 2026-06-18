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
    /// SearchReplaceInFileName 工具，允许在指定的目录下并行地查找文件名和目录名中的 SearchString 并替换为 TargetString。
    /// </summary>
    public sealed class SearchReplaceInFileNameTool :
        IFerritaTool,
        IFerritaToolConfigurationProvider,
        IFerritaToolInvocationPresentationProvider,
        IFerritaToolPromptDescriptionProvider
    {
        public const string ToolName = "SearchReplaceInFileName";

        private static readonly FerritaToolDefinition s_definition = BuildDefinition(new ToolFileSystemPermissionSettings());

        public FerritaToolDefinition Definition => s_definition;

        public FerritaToolDefinition GetEffectiveDefinition(FerritaToolConfigurationState configuration)
        {
            return BuildDefinition(ToolFileSystemPermissionSettings.FromConfiguration(configuration, "SearchReplaceInFileNameSettings"));
        }

        public FerritaToolConfigurationPresenter? CreateConfigurationPresenter(FerritaToolConfigurationEditorContext context)
        {
            return new ToolFileSystemPermissionConfigurationPresenter(context, "SearchReplaceInFileNameSettings", ToolName);
        }

        public string GetPromptDescription(FerritaToolPromptDescriptionContext context)
        {
            var settings = ToolFileSystemPermissionSettings.FromConfiguration(context.ConfigurationState, "SearchReplaceInFileNameSettings");
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
                    new ToolInvocationCardFieldDefinition("TargetString", "TargetString", "Waiting for replacement text...")
                ]);
        }

        public async Task<FerritaToolResult> ExecuteAsync(
            FerritaToolContext context,
            FerritaToolArguments arguments,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var settings = ToolFileSystemPermissionSettings.FromConfiguration(context.CurrentToolConfiguration, "SearchReplaceInFileNameSettings");
            var dirsXml = arguments.GetString("Directories") ?? string.Empty;
            var searchString = arguments.GetString("SearchString") ?? string.Empty;
            var targetString = arguments.GetString("TargetString") ?? string.Empty;

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

            // 2. 并行递归收集所有待处理的文件和文件夹
            var allPathsToRename = new ConcurrentDictionary<string, byte>(StringComparer.OrdinalIgnoreCase);
            if (resolvedDirs.Count > 0)
            {
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
                    allPathsToRename.TryAdd(file, 0);
                }
                foreach (var subDir in subDirs)
                {
                    allPathsToRename.TryAdd(subDir, 0);
                }
            }

            if (allPathsToRename.Count == 0)
            {
                return FerritaToolResult.Success($"No files or directories found under the target paths to rename.");
            }

            // 3. 按照物理路径深度降序分组
            var pathsByDepth = allPathsToRename.Keys
                .GroupBy(GetPathDepth)
                .OrderByDescending(g => g.Key)
                .ToList();

            var finalPaths = new ConcurrentDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var lockObj = new object();
            var renameFailures = new ConcurrentQueue<string>();
            var renamedCount = 0;

            // 4. 自底向上，层级间串行，层级内并行重命名
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
                        renameFailures.Enqueue($"Skipped '{path}': New name '{newName}' contains invalid directory separators.");
                        return;
                    }

                    var newPath = Path.Combine(parentDir, newName);

                    if (File.Exists(newPath) || Directory.Exists(newPath))
                    {
                        renameFailures.Enqueue($"Skipped '{path}': Target path '{newPath}' already exists.");
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

            // 5. 对最终所有的已重命名的文件，进行 RAG 刷新 (并行)
            var filesToRefresh = finalPaths.Values.Where(File.Exists).ToList();
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
              builder.AppendLine("=== Search and Replace FileNames Summary ===");
              builder.AppendLine($"Successfully renamed: {renamedCount}");

              if (errors.Count > 0)
              {
                  builder.AppendLine("\nResolution Errors:");
                  foreach (var err in errors)
                  {
                      builder.AppendLine($"- {err}");
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
                  builder.AppendLine("\nFailures or Skips:");
                  foreach (var fail in renameFailures)
                  {
                      builder.AppendLine($"- {fail}");
                  }
              }

              var responseData = new Dictionary<string, object?>
              {
                  ["renamedCount"] = renamedCount,
                  ["errorCount"] = errors.Count + renameFailures.Count,
                  ["renamedMappings"] = finalPaths.ToDictionary(kv => kv.Key, kv => kv.Value, StringComparer.OrdinalIgnoreCase),
                  ["failures"] = renameFailures.ToArray()
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
                          isRequired: true)
                  ],
                  defaultToolKitKeys: ["Refactor"]);
          }

          private static string BuildDescription(ToolFileSystemPermissionSettings settings)
          {
              return "Parallel search and replace for file and directory names. " +
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
      }
  }
