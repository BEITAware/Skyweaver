using System.Diagnostics;
using System.IO;
using System.Linq;
using Skyweaver.Models.LateralFileSystem;

namespace Skyweaver.Services.LateralFileSystem
{
    public sealed class LateralFileSystemStorageAnalyzer
    {
        public LateralFileSystemNodeStorageSummary Analyze(string virtualRootPath)
        {
            var stopwatch = Stopwatch.StartNew();
            LateralFileSystemDebugConsole.Write("Analyzer", $"Analyze start; virtualRootPath='{virtualRootPath}'.");
            if (string.IsNullOrWhiteSpace(virtualRootPath) || !Directory.Exists(virtualRootPath))
            {
                LateralFileSystemDebugConsole.Write("Analyzer", $"Analyze returning empty because virtual root is missing: '{virtualRootPath}'.");
                return new LateralFileSystemNodeStorageSummary();
            }

            long hydratedBytes = 0;
            var hydratedFileCount = 0;
            var hydratedPlaceholderFileCount = 0;
            var fullFileCount = 0;
            var placeholderFileCount = 0;
            var totalFileCount = 0;
            var totalDirectoryCount = 1;
            var usedFallbackEstimation = false;

            foreach (var entryPath in LateralFileSystemSafeEnumeration.EnumerateFileSystemEntriesRecursively(virtualRootPath))
            {
                if (LateralFileSystemSafeEnumeration.IsDirectory(entryPath))
                {
                    totalDirectoryCount++;
                    continue;
                }

                try
                {
                    totalFileCount++;
                    if (totalFileCount <= 20 || totalFileCount % 100 == 0)
                    {
                        LateralFileSystemDebugConsole.Write("Analyzer", $"Analyze file {totalFileCount}: '{entryPath}'.");
                    }

                    var fileInfo = new FileInfo(entryPath);
                    var stateInfo = GetOnDiskState(entryPath, isDirectory: false);

                    if (stateInfo.UsedFallbackEstimation)
                    {
                        usedFallbackEstimation = true;
                    }

                    if (stateInfo.State.HasFlag(LateralFileSystemOnDiskState.Placeholder))
                    {
                        placeholderFileCount++;
                    }

                    if (stateInfo.State.HasFlag(LateralFileSystemOnDiskState.HydratedPlaceholder))
                    {
                        hydratedPlaceholderFileCount++;
                    }

                    if (stateInfo.State.HasFlag(LateralFileSystemOnDiskState.Full))
                    {
                        fullFileCount++;
                    }

                    if (IsHydratedContent(stateInfo.State))
                    {
                        hydratedFileCount++;
                        hydratedBytes += fileInfo.Length;
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    LateralFileSystemDebugConsole.Write("Analyzer", $"Analyze skipped file due to UnauthorizedAccessException: '{entryPath}'.");
                }
                catch (IOException)
                {
                    LateralFileSystemDebugConsole.Write("Analyzer", $"Analyze skipped file due to IOException: '{entryPath}'.");
                }
                catch (NotSupportedException)
                {
                    LateralFileSystemDebugConsole.Write("Analyzer", $"Analyze skipped file due to NotSupportedException: '{entryPath}'.");
                }
            }

            var summary = new LateralFileSystemNodeStorageSummary
            {
                HydratedBytes = hydratedBytes,
                HydratedFileCount = hydratedFileCount,
                HydratedPlaceholderFileCount = hydratedPlaceholderFileCount,
                FullFileCount = fullFileCount,
                PlaceholderFileCount = placeholderFileCount,
                TotalFileCount = totalFileCount,
                TotalDirectoryCount = totalDirectoryCount,
                UsedFallbackEstimation = usedFallbackEstimation
            };
            LateralFileSystemDebugConsole.Write("Analyzer", $"Analyze end; virtualRootPath='{virtualRootPath}'; files={summary.TotalFileCount}; directories={summary.TotalDirectoryCount}; hydratedBytes={summary.HydratedBytes}; elapsedMs={stopwatch.ElapsedMilliseconds}.");
            return summary;
        }

        public IReadOnlyList<LateralFileSystemFileEntryModel> GetEntries(string virtualRootPath, string relativeDirectoryPath)
        {
            var stopwatch = Stopwatch.StartNew();
            LateralFileSystemDebugConsole.Write("Analyzer", $"GetEntries start; virtualRootPath='{virtualRootPath}'; relativeDirectoryPath='{relativeDirectoryPath}'.");
            if (string.IsNullOrWhiteSpace(virtualRootPath) || !Directory.Exists(virtualRootPath))
            {
                LateralFileSystemDebugConsole.Write("Analyzer", $"GetEntries returning empty because virtual root is missing: '{virtualRootPath}'.");
                return Array.Empty<LateralFileSystemFileEntryModel>();
            }

            var normalizedRelativePath = NormalizeRelativePath(relativeDirectoryPath);
            var targetDirectoryPath = string.IsNullOrWhiteSpace(normalizedRelativePath)
                ? virtualRootPath
                : Path.Combine(virtualRootPath, normalizedRelativePath);

            if (!Directory.Exists(targetDirectoryPath))
            {
                LateralFileSystemDebugConsole.Write("Analyzer", $"GetEntries returning empty because target directory is missing: '{targetDirectoryPath}'.");
                return Array.Empty<LateralFileSystemFileEntryModel>();
            }

            var entries = new List<LateralFileSystemFileEntryModel>();

            var childEntryPaths = LateralFileSystemSafeEnumeration.EnumerateImmediateFileSystemEntries(targetDirectoryPath)
                .Select(path => new
                {
                    FullPath = path,
                    IsDirectory = LateralFileSystemSafeEnumeration.IsDirectory(path)
                })
                .OrderBy(entry => entry.IsDirectory ? 0 : 1)
                .ThenBy(entry => Path.GetFileName(entry.FullPath), StringComparer.OrdinalIgnoreCase)
                .ToList();

            foreach (var childEntryPath in childEntryPaths)
            {
                var entry = TryCreateEntryModel(virtualRootPath, childEntryPath.FullPath, childEntryPath.IsDirectory);
                if (entry is not null)
                {
                    entries.Add(entry);
                }
            }

            LateralFileSystemDebugConsole.Write("Analyzer", $"GetEntries end; virtualRootPath='{virtualRootPath}'; relativeDirectoryPath='{relativeDirectoryPath}'; count={entries.Count}; elapsedMs={stopwatch.ElapsedMilliseconds}.");
            return entries;
        }

        private static LateralFileSystemFileEntryModel? TryCreateEntryModel(string virtualRootPath, string fullPath, bool isDirectory)
        {
            try
            {
                return CreateEntryModel(virtualRootPath, fullPath, isDirectory);
            }
            catch (UnauthorizedAccessException)
            {
                return null;
            }
            catch (IOException)
            {
                return null;
            }
            catch (NotSupportedException)
            {
                return null;
            }
        }

        private static LateralFileSystemFileEntryModel CreateEntryModel(string virtualRootPath, string fullPath, bool isDirectory)
        {
            var relativePath = NormalizeRelativePath(Path.GetRelativePath(virtualRootPath, fullPath));
            var stateInfo = GetOnDiskState(fullPath, isDirectory);
            var logicalSizeBytes = 0L;
            var hydratedSizeBytes = 0L;

            if (!isDirectory)
            {
                var info = new FileInfo(fullPath);
                logicalSizeBytes = info.Length;
                hydratedSizeBytes = IsHydratedContent(stateInfo.State) ? info.Length : 0;
            }

            return new LateralFileSystemFileEntryModel
            {
                Name = Path.GetFileName(fullPath),
                FullPath = fullPath,
                RelativePath = relativePath,
                IsDirectory = isDirectory,
                LogicalSizeBytes = logicalSizeBytes,
                HydratedSizeBytes = hydratedSizeBytes,
                OnDiskState = stateInfo.State
            };
        }

        private static (LateralFileSystemOnDiskState State, bool UsedFallbackEstimation) GetOnDiskState(string fullPath, bool isDirectory)
        {
            try
            {
                var hr = ProjFsNative.PrjGetOnDiskFileState(fullPath, out var nativeState);
                if (hr >= 0)
                {
                    return (Map(nativeState), false);
                }
            }
            catch (DllNotFoundException)
            {
            }
            catch (EntryPointNotFoundException)
            {
            }
            catch (BadImageFormatException)
            {
            }

            return isDirectory
                ? (LateralFileSystemOnDiskState.Unknown, true)
                : (LateralFileSystemOnDiskState.Full | LateralFileSystemOnDiskState.Unknown, true);
        }

        private static bool IsHydratedContent(LateralFileSystemOnDiskState state)
        {
            return state.HasFlag(LateralFileSystemOnDiskState.HydratedPlaceholder)
                || state.HasFlag(LateralFileSystemOnDiskState.Full);
        }

        private static LateralFileSystemOnDiskState Map(ProjFsNative.PrjFileState state)
        {
            var mapped = LateralFileSystemOnDiskState.None;

            if (state.HasFlag(ProjFsNative.PrjFileState.Placeholder))
            {
                mapped |= LateralFileSystemOnDiskState.Placeholder;
            }

            if (state.HasFlag(ProjFsNative.PrjFileState.HydratedPlaceholder))
            {
                mapped |= LateralFileSystemOnDiskState.HydratedPlaceholder;
            }

            if (state.HasFlag(ProjFsNative.PrjFileState.DirtyPlaceholder))
            {
                mapped |= LateralFileSystemOnDiskState.DirtyPlaceholder;
            }

            if (state.HasFlag(ProjFsNative.PrjFileState.Full))
            {
                mapped |= LateralFileSystemOnDiskState.Full;
            }

            if (state.HasFlag(ProjFsNative.PrjFileState.Tombstone))
            {
                mapped |= LateralFileSystemOnDiskState.Tombstone;
            }

            return mapped;
        }

        private static string NormalizeRelativePath(string relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath) || relativePath == ".")
            {
                return string.Empty;
            }

            return relativePath
                .Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar)
                .TrimStart(Path.DirectorySeparatorChar);
        }
    }
}
