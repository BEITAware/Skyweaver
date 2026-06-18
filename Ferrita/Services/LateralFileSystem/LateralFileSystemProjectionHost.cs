using Ferrita.Models.LateralFileSystem;

namespace Ferrita.Services.LateralFileSystem
{
    public sealed class LateralFileSystemProjectionHost : IDisposable
    {
        private readonly Dictionary<string, LateralFileSystemVirtualizationInstance> _instances = new(StringComparer.OrdinalIgnoreCase);
        private readonly LateralFileSystemTreeRepository _treeRepository;
        private readonly object _syncRoot = new();
        private bool _disposed;

        public LateralFileSystemProjectionHost(LateralFileSystemTreeRepository treeRepository)
        {
            _treeRepository = treeRepository;
        }

        public event EventHandler<LateralFileSystemSourceChangedEventArgs>? SourceChanged;

        public void Activate(LateralFileSystemNodeModel node, string workingRootDirectory, string sourceRootDirectory)
        {
            ArgumentNullException.ThrowIfNull(node);
            ArgumentException.ThrowIfNullOrWhiteSpace(workingRootDirectory);
            ArgumentException.ThrowIfNullOrWhiteSpace(sourceRootDirectory);
            LateralFileSystemDebugConsole.Write("Host", $"Activate requested; node='{node.Name}' ({node.Id}); workingRootDirectory='{workingRootDirectory}'; sourceRootDirectory='{sourceRootDirectory}'.");

            lock (_syncRoot)
            {
                ThrowIfDisposed();

                if (_instances.ContainsKey(node.Id))
                {
                    LateralFileSystemDebugConsole.Write("Host", $"Activate skipped because node '{node.Name}' ({node.Id}) is already active.");
                    return;
                }

                var instance = new LateralFileSystemVirtualizationInstance(node, workingRootDirectory, sourceRootDirectory, _treeRepository);
                instance.SourceChanged += OnSourceChanged;
                try
                {
                    instance.Start();
                    _instances.Add(node.Id, instance);
                }
                catch
                {
                    instance.SourceChanged -= OnSourceChanged;
                    try
                    {
                        instance.Dispose();
                    }
                    catch
                    {
                    }

                    LateralFileSystemDebugConsole.Write("Host", $"Activate failed; node='{node.Name}' ({node.Id}).");
                    throw;
                }

                LateralFileSystemDebugConsole.Write("Host", $"Activate completed; node='{node.Name}' ({node.Id}).");
            }
        }

        public void Deactivate(string nodeId)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(nodeId);
            LateralFileSystemDebugConsole.Write("Host", $"Deactivate requested; nodeId={nodeId}.");

            lock (_syncRoot)
            {
                if (!_instances.Remove(nodeId, out var instance))
                {
                    LateralFileSystemDebugConsole.Write("Host", $"Deactivate skipped because nodeId={nodeId} is not active.");
                    return;
                }

                instance.SourceChanged -= OnSourceChanged;
                instance.Dispose();
                LateralFileSystemDebugConsole.Write("Host", $"Deactivate completed; nodeId={nodeId}.");
            }
        }

        public void Sync(string nodeId)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(nodeId);
            LateralFileSystemDebugConsole.Write("Host", $"Sync requested; nodeId={nodeId}.");

            lock (_syncRoot)
            {
                if (_instances.TryGetValue(nodeId, out var instance))
                {
                    instance.SyncAllPlaceholders();
                    LateralFileSystemDebugConsole.Write("Host", $"Sync completed; nodeId={nodeId}.");
                }
                else
                {
                    LateralFileSystemDebugConsole.Write("Host", $"Sync skipped because nodeId={nodeId} is not active.");
                }
            }
        }

        public void Dispose()
        {
            lock (_syncRoot)
            {
                if (_disposed)
                {
                    return;
                }

                foreach (var instance in _instances.Values)
                {
                    instance.SourceChanged -= OnSourceChanged;
                    instance.Dispose();
                }

                _instances.Clear();
                _disposed = true;
            }
        }

        private void OnSourceChanged(object? sender, LateralFileSystemSourceChangedEventArgs e)
        {
            SourceChanged?.Invoke(this, e);
        }

        private void ThrowIfDisposed()
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
        }
    }
}
