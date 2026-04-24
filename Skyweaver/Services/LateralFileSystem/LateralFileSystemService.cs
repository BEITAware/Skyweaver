using System.Diagnostics;
using System.IO;
using System.Linq;
using Skyweaver.Models.LateralFileSystem;

namespace Skyweaver.Services.LateralFileSystem
{
    public sealed class LateralFileSystemService : IDisposable
    {
        private readonly LateralFileSystemTreeRepository _treeRepository;
        private readonly LateralFileSystemProjectionHost _projectionHost;

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

        public IReadOnlyList<LateralFileSystemNodeModel> GetNodes(string workingRootDirectory)
        {
            return _treeRepository.Load(workingRootDirectory).Nodes.ToList();
        }

        public void ActivateAll(string workingRootDirectory)
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
    }
}
