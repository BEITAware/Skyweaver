using System.IO;

namespace Skyweaver.Services.LateralFileSystem
{
    internal static class LateralFileSystemSafeEnumeration
    {
        private static readonly EnumerationOptions ImmediateEnumerationOptions = new()
        {
            AttributesToSkip = (FileAttributes)0,
            IgnoreInaccessible = true,
            RecurseSubdirectories = false,
            ReturnSpecialDirectories = false
        };

        public static IEnumerable<string> EnumerateImmediateFileSystemEntries(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                yield break;
            }

            LateralFileSystemDebugConsole.Write("Enum", $"Immediate enumeration start: {directoryPath}");

            IEnumerator<string> enumerator;
            try
            {
                enumerator = Directory.EnumerateFileSystemEntries(directoryPath, "*", ImmediateEnumerationOptions).GetEnumerator();
            }
            catch (Exception ex) when (IsSkippableEnumerationException(ex))
            {
                LateralFileSystemDebugConsole.WriteException("Enum", ex, $"Immediate enumeration initialization skipped for '{directoryPath}'");
                yield break;
            }

            var count = 0;
            using (enumerator)
            {
                while (true)
                {
                    string currentPath;
                    try
                    {
                        if (!enumerator.MoveNext())
                        {
                            LateralFileSystemDebugConsole.Write("Enum", $"Immediate enumeration end: {directoryPath}; count={count}");
                            yield break;
                        }

                        currentPath = enumerator.Current;
                    }
                    catch (Exception ex) when (IsSkippableEnumerationException(ex))
                    {
                        LateralFileSystemDebugConsole.WriteException("Enum", ex, $"Immediate enumeration aborted for '{directoryPath}' after {count} entries");
                        yield break;
                    }

                    count++;
                    if (count <= 20 || count % 100 == 0)
                    {
                        LateralFileSystemDebugConsole.Write("Enum", $"Immediate enumeration item {count}: {currentPath}");
                    }

                    yield return currentPath;
                }
            }
        }

        public static IEnumerable<string> EnumerateImmediateDirectories(string directoryPath)
        {
            foreach (var entryPath in EnumerateImmediateFileSystemEntries(directoryPath))
            {
                if (IsDirectory(entryPath))
                {
                    yield return entryPath;
                }
            }
        }

        public static IEnumerable<string> EnumerateImmediateFiles(string directoryPath)
        {
            foreach (var entryPath in EnumerateImmediateFileSystemEntries(directoryPath))
            {
                if (!IsDirectory(entryPath))
                {
                    yield return entryPath;
                }
            }
        }

        public static IEnumerable<string> EnumerateFileSystemEntriesRecursively(string rootPath, bool descendIntoReparsePoints = false)
        {
            if (!Directory.Exists(rootPath))
            {
                yield break;
            }

            LateralFileSystemDebugConsole.Write("Enum", $"Recursive enumeration start: {rootPath}; descendIntoReparsePoints={descendIntoReparsePoints}");

            var pendingDirectories = new Stack<string>();
            pendingDirectories.Push(rootPath);
            var visitedDirectoryCount = 0;
            var yieldedEntryCount = 0;

            while (pendingDirectories.Count > 0)
            {
                var currentDirectoryPath = pendingDirectories.Pop();
                visitedDirectoryCount++;
                LateralFileSystemDebugConsole.Write("Enum", $"Recursive enumeration visiting directory {visitedDirectoryCount}: {currentDirectoryPath}");

                foreach (var entryPath in EnumerateImmediateFileSystemEntries(currentDirectoryPath))
                {
                    yieldedEntryCount++;
                    if (yieldedEntryCount <= 20 || yieldedEntryCount % 100 == 0)
                    {
                        LateralFileSystemDebugConsole.Write("Enum", $"Recursive enumeration item {yieldedEntryCount}: {entryPath}");
                    }

                    yield return entryPath;

                    if (ShouldDescendIntoDirectory(entryPath, descendIntoReparsePoints))
                    {
                        LateralFileSystemDebugConsole.Write("Enum", $"Recursive enumeration descending into: {entryPath}");
                        pendingDirectories.Push(entryPath);
                    }
                }
            }

            LateralFileSystemDebugConsole.Write("Enum", $"Recursive enumeration end: {rootPath}; directories={visitedDirectoryCount}; entries={yieldedEntryCount}");
        }

        public static IEnumerable<string> EnumerateDirectoriesRecursively(string rootPath, bool descendIntoReparsePoints = false)
        {
            foreach (var entryPath in EnumerateFileSystemEntriesRecursively(rootPath, descendIntoReparsePoints))
            {
                if (IsDirectory(entryPath))
                {
                    yield return entryPath;
                }
            }
        }

        public static IEnumerable<string> EnumerateFilesRecursively(string rootPath, bool descendIntoReparsePoints = false)
        {
            foreach (var entryPath in EnumerateFileSystemEntriesRecursively(rootPath, descendIntoReparsePoints))
            {
                if (!IsDirectory(entryPath))
                {
                    yield return entryPath;
                }
            }
        }

        public static bool HasAnyEntries(string directoryPath)
        {
            using var enumerator = EnumerateImmediateFileSystemEntries(directoryPath).GetEnumerator();
            return enumerator.MoveNext();
        }

        public static bool IsDirectory(string path)
        {
            return TryGetAttributes(path, out var attributes)
                && attributes.HasFlag(FileAttributes.Directory);
        }

        public static bool ShouldDescendIntoDirectory(string path, bool descendIntoReparsePoints = false)
        {
            if (!TryGetAttributes(path, out var attributes) || !attributes.HasFlag(FileAttributes.Directory))
            {
                return false;
            }

            return descendIntoReparsePoints || !attributes.HasFlag(FileAttributes.ReparsePoint);
        }

        private static bool TryGetAttributes(string path, out FileAttributes attributes)
        {
            try
            {
                attributes = File.GetAttributes(path);
                return true;
            }
            catch (Exception ex) when (IsSkippableEnumerationException(ex))
            {
                attributes = default;
                return false;
            }
        }

        private static bool IsSkippableEnumerationException(Exception ex)
        {
            return ex is UnauthorizedAccessException or IOException or NotSupportedException;
        }
    }
}
