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
            return node;
        }

        public LateralFileSystemNodeModel CreateInheritance(string workingRootDirectory, string virtualFolderName, string owner, string parentNodeId)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(workingRootDirectory);
            ArgumentException.ThrowIfNullOrWhiteSpace(virtualFolderName);
            ArgumentException.ThrowIfNullOrWhiteSpace(owner);
            ArgumentException.ThrowIfNullOrWhiteSpace(parentNodeId);

            Directory.CreateDirectory(workingRootDirectory);

            var tree = _treeRepository.Load(workingRootDirectory);
            var parentNode = tree.Nodes.FirstOrDefault(node => string.Equals(node.Id, parentNodeId, StringComparison.OrdinalIgnoreCase))
                ?? throw new InvalidOperationException("继承源侧向文件系统虚拟文件夹不存在。");

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
                ProjectionSourcePath = parentNode.VirtualRootPath,
                IsActive = true,
                CreatedAtUtc = DateTime.UtcNow,
                UpdatedAtUtc = DateTime.UtcNow
            };

            node.Properties.Add(new LateralFileSystemNodeProperty { Key = "ProjectionMode", Value = "Inherited" });
            tree.Nodes.Add(node);
            _treeRepository.Save(workingRootDirectory, tree);
            _projectionHost.Activate(node, workingRootDirectory, parentNode.VirtualRootPath);
            return node;
        }

        public void DeleteVirtualRoot(string workingRootDirectory, string nodeId)
        {
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
        }

        public IReadOnlyList<LateralFileSystemNodeModel> GetNodes(string workingRootDirectory)
        {
            return _treeRepository.Load(workingRootDirectory).Nodes.ToList();
        }

        public void ActivateAll(string workingRootDirectory)
        {
            if (string.IsNullOrWhiteSpace(workingRootDirectory) || !Directory.Exists(workingRootDirectory))
            {
                return;
            }

            var tree = _treeRepository.Load(workingRootDirectory);
            foreach (var node in tree.Nodes)
            {
                var sourceRootDirectory = ResolveSourceRoot(tree, node);
                _projectionHost.Activate(node, workingRootDirectory, sourceRootDirectory);
            }
        }

        public void SyncNode(string workingRootDirectory, string nodeId)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(workingRootDirectory);
            ArgumentException.ThrowIfNullOrWhiteSpace(nodeId);

            _projectionHost.Sync(nodeId);
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
            if (node.Kind == LateralFileSystemNodeKind.Projection)
            {
                return node.ProjectionSourcePath ?? throw new InvalidOperationException("投影节点缺少投影源路径。");
            }

            var parentNode = tree.Nodes.FirstOrDefault(candidate => string.Equals(candidate.Id, node.ParentNodeId, StringComparison.OrdinalIgnoreCase))
                ?? throw new InvalidOperationException("继承节点缺少父节点。") ;
            return parentNode.VirtualRootPath;
        }
    }
}
