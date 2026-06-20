using System.Diagnostics;
using System.IO;
using System.Linq;
using Ferrita.Models.LateralFileSystem;

namespace Ferrita.Services.LateralFileSystem
{
    public sealed class LateralFileSystemService : IDisposable
    {
        private readonly LateralFileSystemTreeRepository _treeRepository;
        private readonly LateralFileSystemProjectionHost _projectionHost;
        private readonly object _operationSyncRoot = new();

        public LateralFileSystemService(LateralFileSystemTreeRepository treeRepository, LateralFileSystemProjectionHost projectionHost)
        {
            _treeRepository = treeRepository;
            _projectionHost = projectionHost;
        }

        public event EventHandler<LateralFileSystemSourceChangedEventArgs>? SourceChanged
        {
            add => _projectionHost.SourceChanged += value;
            remove => _projectionHost.SourceChanged -= value;
        }

        public LateralFileSystemNodeModel CreateProjection(string workingRootDirectory, string virtualFolderName, string owner, string projectionSourcePath)
        {
            LateralFileSystemDebugConsole.Write("Service", $"CreateProjection start; virtualFolderName='{virtualFolderName}'; workingRootDirectory='{workingRootDirectory}'; projectionSourcePath='{projectionSourcePath}'.");
            ArgumentException.ThrowIfNullOrWhiteSpace(workingRootDirectory);
            ArgumentException.ThrowIfNullOrWhiteSpace(virtualFolderName);
            ArgumentException.ThrowIfNullOrWhiteSpace(owner);
            ArgumentException.ThrowIfNullOrWhiteSpace(projectionSourcePath);

            EnsureDirectoryExists(projectionSourcePath, "投影源目录不存在。");
            Directory.CreateDirectory(workingRootDirectory);

            var tree = _treeRepository.Load(workingRootDirectory);
            var virtualRootPath = GetVirtualRootPath(workingRootDirectory, virtualFolderName);
            EnsureVirtualRootIsUnique(tree, virtualRootPath, virtualFolderName);

            Directory.CreateDirectory(virtualRootPath);

            var node = new LateralFileSystemNodeModel
            {
                Name = virtualFolderName,
                VirtualRootPath = virtualRootPath,
                Owner = owner,
                Kind = LateralFileSystemNodeKind.Projection,
                ProjectionSourcePath = projectionSourcePath,
                IsActive = true,
                CreatedAtUtc = DateTime.UtcNow,
                UpdatedAtUtc = DateTime.UtcNow
            };

            node.Properties.Add(new LateralFileSystemNodeProperty { Key = "ProjectionMode", Value = "Direct" });
            tree.Nodes.Add(node);
            _treeRepository.Save(workingRootDirectory, tree);
            _projectionHost.Activate(node, workingRootDirectory, projectionSourcePath);
            LateralFileSystemDebugConsole.Write("Service", $"CreateProjection end; nodeId={node.Id}; virtualRootPath='{node.VirtualRootPath}'.");
            return node;
        }

        public LateralFileSystemNodeModel CreateInheritance(string workingRootDirectory, string virtualFolderName, string owner, string parentNodeId, string? projectionSourcePath = null)
        {
            LateralFileSystemDebugConsole.Write("Service", $"CreateInheritance start; virtualFolderName='{virtualFolderName}'; parentNodeId='{parentNodeId}'; workingRootDirectory='{workingRootDirectory}'; projectionSourcePath='{projectionSourcePath}'.");
            ArgumentException.ThrowIfNullOrWhiteSpace(workingRootDirectory);
            ArgumentException.ThrowIfNullOrWhiteSpace(virtualFolderName);
            ArgumentException.ThrowIfNullOrWhiteSpace(owner);
            ArgumentException.ThrowIfNullOrWhiteSpace(parentNodeId);

            Directory.CreateDirectory(workingRootDirectory);

            var tree = _treeRepository.Load(workingRootDirectory);
            var parentNode = tree.Nodes.FirstOrDefault(node => string.Equals(node.Id, parentNodeId, StringComparison.OrdinalIgnoreCase))
                ?? throw new InvalidOperationException("继承源侧向文件系统虚拟文件夹不存在。");

            var resolvedProjectionSourcePath = string.IsNullOrWhiteSpace(projectionSourcePath)
                ? parentNode.VirtualRootPath
                : projectionSourcePath.Trim();

            EnsureDirectoryExists(resolvedProjectionSourcePath, "投影源目录不存在。");

            var virtualRootPath = GetVirtualRootPath(workingRootDirectory, virtualFolderName);
            EnsureVirtualRootIsUnique(tree, virtualRootPath, virtualFolderName);
            Directory.CreateDirectory(virtualRootPath);

            var node = new LateralFileSystemNodeModel
            {
                Name = virtualFolderName,
                VirtualRootPath = virtualRootPath,
                Owner = owner,
                Kind = LateralFileSystemNodeKind.Inheritance,
                ParentNodeId = parentNode.Id,
                ProjectionSourcePath = resolvedProjectionSourcePath,
                IsActive = true,
                CreatedAtUtc = DateTime.UtcNow,
                UpdatedAtUtc = DateTime.UtcNow
            };

            node.Properties.Add(new LateralFileSystemNodeProperty { Key = "ProjectionMode", Value = "Inherited" });
            tree.Nodes.Add(node);
            _treeRepository.Save(workingRootDirectory, tree);
            _projectionHost.Activate(node, workingRootDirectory, resolvedProjectionSourcePath);
            LateralFileSystemDebugConsole.Write("Service", $"CreateInheritance end; nodeId={node.Id}; virtualRootPath='{node.VirtualRootPath}'; sourceRootDirectory='{resolvedProjectionSourcePath}'.");
            return node;
        }

        public void DeleteVirtualRoot(string workingRootDirectory, string nodeId)
        {
            LateralFileSystemDebugConsole.Write("Service", $"DeleteVirtualRoot start; workingRootDirectory='{workingRootDirectory}'; nodeId={nodeId}.");
            ArgumentException.ThrowIfNullOrWhiteSpace(workingRootDirectory);
            ArgumentException.ThrowIfNullOrWhiteSpace(nodeId);

            var tree = _treeRepository.Load(workingRootDirectory);
            var node = tree.Nodes.FirstOrDefault(candidate => string.Equals(candidate.Id, nodeId, StringComparison.OrdinalIgnoreCase))
                ?? throw new InvalidOperationException("待删除的侧向文件系统虚拟文件夹不存在。");

            var hasChildren = tree.Nodes.Any(candidate => string.Equals(candidate.ParentNodeId, node.Id, StringComparison.OrdinalIgnoreCase));
            if (hasChildren)
            {
                throw new InvalidOperationException("该侧向文件系统虚拟文件夹已有继承子项，不能直接删除。");
            }

            _projectionHost.Deactivate(node.Id);
            tree.Nodes.Remove(node);
            _treeRepository.Save(workingRootDirectory, tree);

            if (Directory.Exists(node.VirtualRootPath))
            {
                Directory.Delete(node.VirtualRootPath, recursive: true);
            }

            LateralFileSystemDebugConsole.Write("Service", $"DeleteVirtualRoot end; nodeId={nodeId}; virtualRootPath='{node.VirtualRootPath}'.");
        }

        public LateralFileSystemMergeResult MergeVirtualRoot(string workingRootDirectory, string nodeId)
        {
            lock (_operationSyncRoot)
            {
                return MergeVirtualRootCore(workingRootDirectory, nodeId);
            }
        }

        public LateralFileSystemMergeResult OverwriteVirtualRoot(string workingRootDirectory, string nodeId)
        {
            lock (_operationSyncRoot)
            {
                return OverwriteVirtualRootCore(workingRootDirectory, nodeId);
            }
        }

        private LateralFileSystemMergeResult MergeVirtualRootCore(string workingRootDirectory, string nodeId)
        {
            LateralFileSystemDebugConsole.Write("Service", $"MergeVirtualRoot start; workingRootDirectory='{workingRootDirectory}'; nodeId={nodeId}.");
            ArgumentException.ThrowIfNullOrWhiteSpace(workingRootDirectory);
            ArgumentException.ThrowIfNullOrWhiteSpace(nodeId);

            var tree = _treeRepository.Load(workingRootDirectory);
            var node = tree.Nodes.FirstOrDefault(candidate => string.Equals(candidate.Id, nodeId, StringComparison.OrdinalIgnoreCase))
                ?? throw new InvalidOperationException("待合并的侧向文件系统虚拟文件夹不存在。");

            EnsureNoDependentNodes(tree, node);

            var sourceRootDirectory = ResolveSourceRoot(tree, node);
            EnsureDirectoryExists(node.VirtualRootPath, "侧向文件夹投影目录不存在。");
            EnsureDirectoryExists(sourceRootDirectory, "投影源目录不存在。");
            EnsureMergePathsDoNotOverlap(sourceRootDirectory, node.VirtualRootPath);

            var mergeOperationRoot = CreateMergeOperationRoot(workingRootDirectory, node.Id);
            var projectedSnapshotPath = Path.Combine(mergeOperationRoot, "ProjectedSnapshot");
            var sourceBackupPath = Path.Combine(mergeOperationRoot, "SourceBackup");
            var mergedResultPath = Path.Combine(mergeOperationRoot, "MergedResult");

            try
            {
                Directory.CreateDirectory(projectedSnapshotPath);
                Directory.CreateDirectory(sourceBackupPath);
                Directory.CreateDirectory(mergedResultPath);

                var projectedSnapshotSummary = LateralFileSystemDirectoryMirror.CopyContents(
                    node.VirtualRootPath,
                    projectedSnapshotPath,
                    overwrite: true);
                LateralFileSystemDebugConsole.Write("Service", $"MergeVirtualRoot snapshot captured; nodeId={node.Id}; files={projectedSnapshotSummary.FileCount}; directories={projectedSnapshotSummary.DirectoryCount}.");

                _ = LateralFileSystemDirectoryMirror.CopyContents(
                    sourceRootDirectory,
                    sourceBackupPath,
                    overwrite: true);
                LateralFileSystemDebugConsole.Write("Service", $"MergeVirtualRoot source backup captured; nodeId={node.Id}; sourceRootDirectory='{sourceRootDirectory}'.");

                var allRelativePaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                var vFiles = GetAllRelativeFilePaths(projectedSnapshotPath);
                var sFiles = GetAllRelativeFilePaths(sourceBackupPath);
                allRelativePaths.UnionWith(vFiles);
                allRelativePaths.UnionWith(sFiles);

                var conflicts = new List<string>();
                var baseTimeLimit = node.CreatedAtUtc.AddSeconds(2);

                foreach (var relPath in allRelativePaths)
                {
                    bool inV = vFiles.Contains(relPath);
                    bool inS = sFiles.Contains(relPath);

                    if (inV && !inS)
                    {
                        var targetPath = Path.Combine(mergedResultPath, relPath);
                        CopyFileWithDirectories(Path.Combine(projectedSnapshotPath, relPath), targetPath);
                    }
                    else if (!inV && inS)
                    {
                        var sFilePath = Path.Combine(sourceBackupPath, relPath);
                        var creationTime = File.GetCreationTimeUtc(sFilePath);
                        var lastWriteTime = File.GetLastWriteTimeUtc(sFilePath);

                        bool isSourceNew = creationTime > baseTimeLimit;

                        if (isSourceNew)
                        {
                            var targetPath = Path.Combine(mergedResultPath, relPath);
                            CopyFileWithDirectories(sFilePath, targetPath);
                        }
                        else
                        {
                            bool isSourceModified = lastWriteTime > baseTimeLimit;
                            if (isSourceModified)
                            {
                                conflicts.Add($"[修改/删除冲突] 文件 '{relPath}' 在源目录中被修改，但在虚拟文件夹中被删除。");
                            }
                        }
                    }
                    else
                    {
                        bool isVirtualEdited = node.EditedRelativePaths.Any(p => string.Equals(p, relPath, StringComparison.OrdinalIgnoreCase))
                                               || node.LocalOnlyRelativePaths.Any(p => string.Equals(p, relPath, StringComparison.OrdinalIgnoreCase));

                        var sFilePath = Path.Combine(sourceBackupPath, relPath);
                        var vFilePath = Path.Combine(projectedSnapshotPath, relPath);

                        if (!isVirtualEdited)
                        {
                            var targetPath = Path.Combine(mergedResultPath, relPath);
                            CopyFileWithDirectories(sFilePath, targetPath);
                        }
                        else
                        {
                            var lastWriteTime = File.GetLastWriteTimeUtc(sFilePath);
                            bool isSourceModified = lastWriteTime > baseTimeLimit;

                            if (!isSourceModified)
                            {
                                var targetPath = Path.Combine(mergedResultPath, relPath);
                                CopyFileWithDirectories(vFilePath, targetPath);
                            }
                            else
                            {
                                if (FilesAreEqual(sFilePath, vFilePath))
                                {
                                    var targetPath = Path.Combine(mergedResultPath, relPath);
                                    CopyFileWithDirectories(sFilePath, targetPath);
                                }
                                else
                                {
                                    conflicts.Add($"[修改/修改冲突] 文件 '{relPath}' 在源目录和虚拟文件夹中都被修改，且内容不一致。");
                                }
                            }
                        }
                    }
                }

                if (conflicts.Count > 0)
                {
                    throw new InvalidOperationException("合并冲突：\n" + string.Join("\n", conflicts));
                }

                var replaceSummary = LateralFileSystemDirectoryMirror.ReplaceContentsWithRollback(
                    sourceRootDirectory,
                    mergedResultPath,
                    sourceBackupPath);
                LateralFileSystemDebugConsole.Write("Service", $"MergeVirtualRoot source replaced; nodeId={node.Id}; removedFiles={replaceSummary.Removed.FileCount}; removedDirectories={replaceSummary.Removed.DirectoryCount}; restoredFiles={replaceSummary.Restored.FileCount}; restoredDirectories={replaceSummary.Restored.DirectoryCount}.");

                DeleteEmptyDirectories(sourceRootDirectory);

                _projectionHost.Deactivate(node.Id);
                tree.Nodes.Remove(node);
                _treeRepository.Save(workingRootDirectory, tree);

                if (Directory.Exists(node.VirtualRootPath))
                {
                    LateralFileSystemDirectoryMirror.DeleteDirectoryIfExists(node.VirtualRootPath);
                }

                var result = new LateralFileSystemMergeResult
                {
                    NodeId = node.Id,
                    NodeName = node.Name,
                    VirtualRootPath = node.VirtualRootPath,
                    ProjectionSourcePath = sourceRootDirectory,
                    SnapshotFileCount = projectedSnapshotSummary.FileCount,
                    SnapshotDirectoryCount = projectedSnapshotSummary.DirectoryCount,
                    RemovedSourceFileCount = replaceSummary.Removed.FileCount,
                    RemovedSourceDirectoryCount = replaceSummary.Removed.DirectoryCount,
                    RestoredSourceFileCount = replaceSummary.Restored.FileCount,
                    RestoredSourceDirectoryCount = replaceSummary.Restored.DirectoryCount
                };

                LateralFileSystemDebugConsole.Write("Service", $"MergeVirtualRoot end; nodeId={nodeId}; sourceRootDirectory='{sourceRootDirectory}'.");
                return result;
            }
            finally
            {
                TryDeleteMergeOperationRoot(mergeOperationRoot);
            }
        }

        private LateralFileSystemMergeResult OverwriteVirtualRootCore(string workingRootDirectory, string nodeId)
        {
            LateralFileSystemDebugConsole.Write("Service", $"OverwriteVirtualRoot start; workingRootDirectory='{workingRootDirectory}'; nodeId={nodeId}.");
            ArgumentException.ThrowIfNullOrWhiteSpace(workingRootDirectory);
            ArgumentException.ThrowIfNullOrWhiteSpace(nodeId);

            var tree = _treeRepository.Load(workingRootDirectory);
            var node = tree.Nodes.FirstOrDefault(candidate => string.Equals(candidate.Id, nodeId, StringComparison.OrdinalIgnoreCase))
                ?? throw new InvalidOperationException("待合并的侧向文件系统虚拟文件夹不存在。");

            EnsureNoDependentNodes(tree, node);

            var sourceRootDirectory = ResolveSourceRoot(tree, node);
            EnsureDirectoryExists(node.VirtualRootPath, "侧向文件夹投影目录不存在。");
            EnsureDirectoryExists(sourceRootDirectory, "投影源目录不存在。");
            EnsureMergePathsDoNotOverlap(sourceRootDirectory, node.VirtualRootPath);

            var mergeOperationRoot = CreateMergeOperationRoot(workingRootDirectory, node.Id);
            var projectedSnapshotPath = Path.Combine(mergeOperationRoot, "ProjectedSnapshot");
            var sourceBackupPath = Path.Combine(mergeOperationRoot, "SourceBackup");

            try
            {
                Directory.CreateDirectory(projectedSnapshotPath);
                Directory.CreateDirectory(sourceBackupPath);

                var projectedSnapshotSummary = LateralFileSystemDirectoryMirror.CopyContents(
                    node.VirtualRootPath,
                    projectedSnapshotPath,
                    overwrite: true);
                LateralFileSystemDebugConsole.Write("Service", $"OverwriteVirtualRoot snapshot captured; nodeId={node.Id}; files={projectedSnapshotSummary.FileCount}; directories={projectedSnapshotSummary.DirectoryCount}.");

                _ = LateralFileSystemDirectoryMirror.CopyContents(
                    sourceRootDirectory,
                    sourceBackupPath,
                    overwrite: true);
                LateralFileSystemDebugConsole.Write("Service", $"OverwriteVirtualRoot source backup captured; nodeId={node.Id}; sourceRootDirectory='{sourceRootDirectory}'.");

                var replaceSummary = LateralFileSystemDirectoryMirror.ReplaceContentsWithRollback(
                    sourceRootDirectory,
                    projectedSnapshotPath,
                    sourceBackupPath);
                LateralFileSystemDebugConsole.Write("Service", $"OverwriteVirtualRoot source replaced; nodeId={node.Id}; removedFiles={replaceSummary.Removed.FileCount}; removedDirectories={replaceSummary.Removed.DirectoryCount}; restoredFiles={replaceSummary.Restored.FileCount}; restoredDirectories={replaceSummary.Restored.DirectoryCount}.");

                _projectionHost.Deactivate(node.Id);
                tree.Nodes.Remove(node);
                _treeRepository.Save(workingRootDirectory, tree);

                if (Directory.Exists(node.VirtualRootPath))
                {
                    LateralFileSystemDirectoryMirror.DeleteDirectoryIfExists(node.VirtualRootPath);
                }

                var result = new LateralFileSystemMergeResult
                {
                    NodeId = node.Id,
                    NodeName = node.Name,
                    VirtualRootPath = node.VirtualRootPath,
                    ProjectionSourcePath = sourceRootDirectory,
                    SnapshotFileCount = projectedSnapshotSummary.FileCount,
                    SnapshotDirectoryCount = projectedSnapshotSummary.DirectoryCount,
                    RemovedSourceFileCount = replaceSummary.Removed.FileCount,
                    RemovedSourceDirectoryCount = replaceSummary.Removed.DirectoryCount,
                    RestoredSourceFileCount = replaceSummary.Restored.FileCount,
                    RestoredSourceDirectoryCount = replaceSummary.Restored.DirectoryCount
                };

                LateralFileSystemDebugConsole.Write("Service", $"OverwriteVirtualRoot end; nodeId={nodeId}; sourceRootDirectory='{sourceRootDirectory}'.");
                return result;
            }
            finally
            {
                TryDeleteMergeOperationRoot(mergeOperationRoot);
            }
        }

        private static HashSet<string> GetAllRelativeFilePaths(string rootDirectory)
        {
            var paths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (!Directory.Exists(rootDirectory))
            {
                return paths;
            }
            foreach (var file in Directory.EnumerateFiles(rootDirectory, "*", SearchOption.AllDirectories))
            {
                var relative = Path.GetRelativePath(rootDirectory, file);
                paths.Add(relative);
            }
            return paths;
        }

        private static void CopyFileWithDirectories(string source, string destination)
        {
            var destDir = Path.GetDirectoryName(destination);
            if (!string.IsNullOrWhiteSpace(destDir))
            {
                Directory.CreateDirectory(destDir);
            }
            File.Copy(source, destination, overwrite: true);
        }

        private static bool FilesAreEqual(string path1, string path2)
        {
            var info1 = new FileInfo(path1);
            var info2 = new FileInfo(path2);
            if (info1.Length != info2.Length)
            {
                return false;
            }

            using var fs1 = info1.OpenRead();
            using var fs2 = info2.OpenRead();
            const int bufferSize = 4096;
            var buffer1 = new byte[bufferSize];
            var buffer2 = new byte[bufferSize];
            while (true)
            {
                int count1 = fs1.Read(buffer1, 0, bufferSize);
                int count2 = fs2.Read(buffer2, 0, bufferSize);
                if (count1 != count2)
                {
                    return false;
                }
                if (count1 == 0)
                {
                    return true;
                }
                for (int i = 0; i < count1; i++)
                {
                    if (buffer1[i] != buffer2[i])
                    {
                        return false;
                    }
                }
            }
        }

        private static void DeleteEmptyDirectories(string startLocation)
        {
            if (!Directory.Exists(startLocation))
            {
                return;
            }
            foreach (var directory in Directory.GetDirectories(startLocation))
            {
                DeleteEmptyDirectories(directory);
                if (!Directory.EnumerateFileSystemEntries(directory).Any())
                {
                    try
                    {
                        Directory.Delete(directory, false);
                    }
                    catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or NotSupportedException)
                    {
                    }
                }
            }
        }

        public IReadOnlyList<LateralFileSystemNodeModel> GetNodes(string workingRootDirectory)
        {
            return _treeRepository.Load(workingRootDirectory).Nodes.ToList();
        }

        public void ActivateAll(string workingRootDirectory)
        {
            lock (_operationSyncRoot)
            {
                ActivateAllCore(workingRootDirectory);
            }
        }

        private void ActivateAllCore(string workingRootDirectory)
        {
            LateralFileSystemDebugConsole.Write("Service", $"ActivateAll start; workingRootDirectory='{workingRootDirectory}'.");
            if (string.IsNullOrWhiteSpace(workingRootDirectory) || !Directory.Exists(workingRootDirectory))
            {
                LateralFileSystemDebugConsole.Write("Service", "ActivateAll skipped because working root is empty or missing.");
                return;
            }

            var tree = _treeRepository.Load(workingRootDirectory);
            LateralFileSystemDebugConsole.Write("Service", $"ActivateAll loaded {tree.Nodes.Count} node(s).");
            foreach (var node in tree.Nodes)
            {
                try
                {
                    var sourceRootDirectory = ResolveSourceRoot(tree, node);
                    LateralFileSystemDebugConsole.Write("Service", $"ActivateAll activating node '{node.Name}' ({node.Id}); sourceRootDirectory='{sourceRootDirectory}'; virtualRootPath='{node.VirtualRootPath}'.");
                    _projectionHost.Activate(node, workingRootDirectory, sourceRootDirectory);
                    LateralFileSystemDebugConsole.Write("Service", $"ActivateAll activated node '{node.Name}' ({node.Id}).");
                }
                catch (Exception ex)
                {
                    LateralFileSystemDebugConsole.WriteException("Service", ex, $"ActivateAll failed for node '{node.Name}' ({node.Id})");
                    Debug.WriteLine($"Failed to activate LateralFS node '{node.Name}' ({node.Id}): {ex}");
                }
            }

            LateralFileSystemDebugConsole.Write("Service", "ActivateAll end.");
        }

        public void SyncNode(string workingRootDirectory, string nodeId)
        {
            LateralFileSystemDebugConsole.Write("Service", $"SyncNode start; workingRootDirectory='{workingRootDirectory}'; nodeId={nodeId}.");
            ArgumentException.ThrowIfNullOrWhiteSpace(workingRootDirectory);
            ArgumentException.ThrowIfNullOrWhiteSpace(nodeId);

            _projectionHost.Sync(nodeId);
            LateralFileSystemDebugConsole.Write("Service", $"SyncNode end; nodeId={nodeId}.");
        }

        public void Dispose()
        {
            _projectionHost.Dispose();
        }

        private static void EnsureDirectoryExists(string path, string message)
        {
            if (!Directory.Exists(path))
            {
                throw new InvalidOperationException(message);
            }
        }

        private static string GetVirtualRootPath(string workingRootDirectory, string virtualFolderName)
        {
            var sanitizedFolderName = virtualFolderName.Trim();
            if (string.IsNullOrWhiteSpace(sanitizedFolderName))
            {
                throw new InvalidOperationException("侧向文件系统虚拟文件夹名称不能为空。");
            }

            if (sanitizedFolderName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
            {
                throw new InvalidOperationException("侧向文件系统虚拟文件夹名称包含无效文件名字符。");
            }

            return Path.Combine(workingRootDirectory, sanitizedFolderName);
        }

        private static void EnsureVirtualRootIsUnique(LateralFileSystemTreeModel tree, string virtualRootPath, string virtualFolderName)
        {
            if (tree.Nodes.Any(node => string.Equals(node.VirtualRootPath, virtualRootPath, StringComparison.OrdinalIgnoreCase)
                || string.Equals(node.Name, virtualFolderName, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException("同名的侧向文件系统虚拟文件夹已存在。");
            }

            if (File.Exists(virtualRootPath) || Directory.Exists(virtualRootPath))
            {
                throw new InvalidOperationException("目标虚拟文件夹路径已存在，不能重复建立。");
            }
        }

        private static string ResolveSourceRoot(LateralFileSystemTreeModel tree, LateralFileSystemNodeModel node)
        {
            if (!string.IsNullOrWhiteSpace(node.ProjectionSourcePath))
            {
                return node.ProjectionSourcePath;
            }

            var parentNode = tree.Nodes.FirstOrDefault(candidate => string.Equals(candidate.Id, node.ParentNodeId, StringComparison.OrdinalIgnoreCase))
                ?? throw new InvalidOperationException("继承节点缺少父节点。");
            return parentNode.VirtualRootPath;
        }

        private static void EnsureNoDependentNodes(LateralFileSystemTreeModel tree, LateralFileSystemNodeModel node)
        {
            var nodeVirtualRootPath = Path.GetFullPath(node.VirtualRootPath);
            var dependentNodes = tree.Nodes
                .Where(candidate => !string.Equals(candidate.Id, node.Id, StringComparison.OrdinalIgnoreCase))
                .Where(candidate => string.Equals(candidate.ParentNodeId, node.Id, StringComparison.OrdinalIgnoreCase)
                    || IsSourceInsideNodeVirtualRoot(tree, candidate, nodeVirtualRootPath))
                .ToList();

            if (dependentNodes.Count == 0)
            {
                return;
            }

            var displayedNames = string.Join("、", dependentNodes.Take(5).Select(candidate => $"“{candidate.Name}”"));
            var overflowText = dependentNodes.Count > 5 ? $"等 {dependentNodes.Count} 个节点" : string.Empty;
            throw new InvalidOperationException($"该侧向文件夹仍被 {displayedNames}{overflowText} 作为继承来源或投影源使用，请先合并或删除这些依赖节点。");
        }

        private static bool IsSourceInsideNodeVirtualRoot(
            LateralFileSystemTreeModel tree,
            LateralFileSystemNodeModel candidate,
            string nodeVirtualRootPath)
        {
            try
            {
                var candidateSourceRoot = ResolveSourceRoot(tree, candidate);
                return IsPathInsideOrSame(candidateSourceRoot, nodeVirtualRootPath);
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }

        private static void EnsureMergePathsDoNotOverlap(string sourceRootDirectory, string virtualRootPath)
        {
            if (IsPathInsideOrSame(sourceRootDirectory, virtualRootPath)
                || IsPathInsideOrSame(virtualRootPath, sourceRootDirectory))
            {
                throw new InvalidOperationException("该侧向文件夹的投影目录与源目录存在包含关系，不能安全执行 Merge。");
            }
        }

        private static string CreateMergeOperationRoot(string workingRootDirectory, string nodeId)
        {
            var mergeRootDirectory = Path.Combine(workingRootDirectory, ".LateralFSMerge");
            Directory.CreateDirectory(mergeRootDirectory);
            TrySetHidden(mergeRootDirectory);

            var nodeIdPrefix = nodeId.Length > 8 ? nodeId[..8] : nodeId;
            var operationDirectoryName = $"{DateTime.UtcNow:yyyyMMddHHmmssfff}-{nodeIdPrefix}-{Guid.NewGuid():N}";
            var operationRoot = Path.Combine(mergeRootDirectory, operationDirectoryName);
            Directory.CreateDirectory(operationRoot);
            return operationRoot;
        }

        private static void TryDeleteMergeOperationRoot(string mergeOperationRoot)
        {
            if (string.IsNullOrWhiteSpace(mergeOperationRoot))
            {
                return;
            }

            try
            {
                LateralFileSystemDirectoryMirror.DeleteDirectoryIfExists(mergeOperationRoot);

                var parentDirectory = Directory.GetParent(mergeOperationRoot)?.FullName;
                if (!string.IsNullOrWhiteSpace(parentDirectory)
                    && Directory.Exists(parentDirectory)
                    && !LateralFileSystemSafeEnumeration.HasAnyEntries(parentDirectory))
                {
                    Directory.Delete(parentDirectory, recursive: false);
                }
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or NotSupportedException)
            {
                LateralFileSystemDebugConsole.WriteException("Service", ex, $"Failed to delete LateralFS merge operation directory '{mergeOperationRoot}'");
            }
        }

        private static void TrySetHidden(string directoryPath)
        {
            try
            {
                var attributes = File.GetAttributes(directoryPath);
                File.SetAttributes(directoryPath, attributes | FileAttributes.Hidden);
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or NotSupportedException)
            {
            }
        }

        private static bool IsPathInsideOrSame(string candidatePath, string rootPath)
        {
            if (string.IsNullOrWhiteSpace(candidatePath) || string.IsNullOrWhiteSpace(rootPath))
            {
                return false;
            }

            var normalizedCandidate = Path.GetFullPath(candidatePath);
            var normalizedRoot = Path.GetFullPath(rootPath);

            if (string.Equals(normalizedCandidate, normalizedRoot, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            var rootWithSeparator = normalizedRoot.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                + Path.DirectorySeparatorChar;
            return normalizedCandidate.StartsWith(rootWithSeparator, StringComparison.OrdinalIgnoreCase);
        }
    }
}
