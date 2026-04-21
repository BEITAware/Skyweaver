using System.Collections.Concurrent;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Skyweaver.Models.LateralFileSystem;

namespace Skyweaver.Services.LateralFileSystem
{
    internal sealed class LateralFileSystemVirtualizationInstance : IDisposable
    {
        private readonly LateralFileSystemTreeRepository _treeRepository;
        private readonly object _syncRoot = new();
        private readonly ConcurrentDictionary<Guid, LateralFileSystemEnumerationSession> _enumerationSessions = new();
        private readonly FileSystemWatcher _sourceWatcher;
        private readonly GCHandle _selfHandle;
        private readonly ProjFsNative.PrjStartDirectoryEnumerationCb _startDirectoryEnumerationCallback;
        private readonly ProjFsNative.PrjEndDirectoryEnumerationCb _endDirectoryEnumerationCallback;
        private readonly ProjFsNative.PrjGetDirectoryEnumerationCb _getDirectoryEnumerationCallback;
        private readonly ProjFsNative.PrjGetPlaceholderInfoCb _getPlaceholderInfoCallback;
        private readonly ProjFsNative.PrjGetFileDataCb _getFileDataCallback;
        private readonly ProjFsNative.PrjNotificationCb _notificationCallback;
        private readonly ProjFsNative.PrjCallbacks _callbacks;
        private readonly ProjFsNative.PrjNotificationMapping[] _notificationMappings;
        private GCHandle _notificationMappingsHandle;
        private IntPtr _namespaceContext;
        private bool _disposed;

        public LateralFileSystemVirtualizationInstance(LateralFileSystemNodeModel node, string workingRootDirectory, string sourceRootDirectory, LateralFileSystemTreeRepository treeRepository)
        {
            Node = node;
            WorkingRootDirectory = workingRootDirectory;
            SourceRootDirectory = sourceRootDirectory;
            _treeRepository = treeRepository;

            _startDirectoryEnumerationCallback = StartDirectoryEnumerationCallback;
            _endDirectoryEnumerationCallback = EndDirectoryEnumerationCallback;
            _getDirectoryEnumerationCallback = GetDirectoryEnumerationCallback;
            _getPlaceholderInfoCallback = GetPlaceholderInfoCallback;
            _getFileDataCallback = GetFileDataCallback;
            _notificationCallback = NotificationCallback;

            _callbacks = new ProjFsNative.PrjCallbacks
            {
                StartDirectoryEnumerationCallback = Marshal.GetFunctionPointerForDelegate(_startDirectoryEnumerationCallback),
                EndDirectoryEnumerationCallback = Marshal.GetFunctionPointerForDelegate(_endDirectoryEnumerationCallback),
                GetDirectoryEnumerationCallback = Marshal.GetFunctionPointerForDelegate(_getDirectoryEnumerationCallback),
                GetPlaceholderInfoCallback = Marshal.GetFunctionPointerForDelegate(_getPlaceholderInfoCallback),
                GetFileDataCallback = Marshal.GetFunctionPointerForDelegate(_getFileDataCallback),
                NotificationCallback = Marshal.GetFunctionPointerForDelegate(_notificationCallback)
            };

            _notificationMappings =
            [
                new ProjFsNative.PrjNotificationMapping
                {
                    NotificationRoot = string.Empty,
                    NotificationBitMask = ProjFsNative.PrjNotification.NewFileCreated
                        | ProjFsNative.PrjNotification.FileOverwritten
                        | ProjFsNative.PrjNotification.PreDelete
                        | ProjFsNative.PrjNotification.PreRename
                        | ProjFsNative.PrjNotification.FileRenamed
                        | ProjFsNative.PrjNotification.FileHandleClosedFileModified
                        | ProjFsNative.PrjNotification.FileHandleClosedFileDeleted
                        | ProjFsNative.PrjNotification.FilePreConvertToFull
                }
            ];

            _notificationMappingsHandle = GCHandle.Alloc(_notificationMappings, GCHandleType.Pinned);
            _selfHandle = GCHandle.Alloc(this);

            _sourceWatcher = new FileSystemWatcher(SourceRootDirectory)
            {
                IncludeSubdirectories = true,
                NotifyFilter = NotifyFilters.DirectoryName | NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.CreationTime
            };
            _sourceWatcher.Created += OnSourceCreated;
            _sourceWatcher.Deleted += OnSourceDeleted;
            _sourceWatcher.Renamed += OnSourceRenamed;
            _sourceWatcher.Changed += OnSourceChanged;
        }

        public LateralFileSystemNodeModel Node { get; }

        public string WorkingRootDirectory { get; }

        public string SourceRootDirectory { get; }

        public event EventHandler<LateralFileSystemSourceChangedEventArgs>? SourceChanged;

        public void Start()
        {
            lock (_syncRoot)
            {
                ThrowIfDisposed();

                Directory.CreateDirectory(Node.VirtualRootPath);
                var instanceId = Guid.Parse(Node.ProviderInstanceId);
                var markResult = ProjFsNative.PrjMarkDirectoryAsPlaceholder(Node.VirtualRootPath, null, IntPtr.Zero, in instanceId);
                if (markResult < 0 && markResult != unchecked((int)0x800700b7))
                {
                    Marshal.ThrowExceptionForHR(markResult);
                }

                var options = new ProjFsNative.PrjStartVirtualizingOptions
                {
                    Flags = 0,
                    PoolThreadCount = 4,
                    ConcurrentThreadCount = 4,
                    NotificationMappings = _notificationMappingsHandle.AddrOfPinnedObject(),
                    NotificationMappingsCount = (uint)_notificationMappings.Length
                };

                var instanceContext = GCHandle.ToIntPtr(_selfHandle);
                var hr = ProjFsNative.PrjStartVirtualizing(Node.VirtualRootPath, in _callbacks, instanceContext, in options, out _namespaceContext);
                if (hr < 0)
                {
                    Marshal.ThrowExceptionForHR(hr);
                }

                Node.IsActive = true;
                Node.UpdatedAtUtc = DateTime.UtcNow;
                PersistNode();
                SyncAllPlaceholders();
                _sourceWatcher.EnableRaisingEvents = true;
            }
        }

        public void Stop()
        {
            lock (_syncRoot)
            {
                if (_disposed)
                {
                    return;
                }

                _sourceWatcher.EnableRaisingEvents = false;
                if (_namespaceContext != IntPtr.Zero)
                {
                    ProjFsNative.PrjStopVirtualizing(_namespaceContext);
                    _namespaceContext = IntPtr.Zero;
                }

                Node.IsActive = false;
                Node.UpdatedAtUtc = DateTime.UtcNow;
                PersistNode();
            }
        }

        public void SyncAllPlaceholders()
        {
            lock (_syncRoot)
            {
                ThrowIfDisposed();

                var sourceEntries = EnumerateEntries(string.Empty).ToDictionary(entry => entry.RelativePath, StringComparer.OrdinalIgnoreCase);
                foreach (var entry in sourceEntries.Values.OrderBy(entry => entry.RelativePath.Count(static c => c == Path.DirectorySeparatorChar || c == Path.AltDirectorySeparatorChar)))
                {
                    EnsurePlaceholder(entry);
                }

                var localPaths = Directory.Exists(Node.VirtualRootPath)
                    ? Directory.EnumerateFileSystemEntries(Node.VirtualRootPath, "*", SearchOption.AllDirectories)
                        .Select(path => NormalizeRelativePath(Path.GetRelativePath(Node.VirtualRootPath, path)))
                        .Where(path => !string.IsNullOrWhiteSpace(path))
                        .OrderByDescending(path => path.Length)
                        .ToList()
                    : new List<string>();

                foreach (var localPath in localPaths)
                {
                    if (sourceEntries.ContainsKey(localPath))
                    {
                        continue;
                    }

                    if (IsEdited(localPath))
                    {
                        continue;
                    }

                    RemoveLocalPath(localPath);
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

                Stop();
                _sourceWatcher.Dispose();
                if (_notificationMappingsHandle.IsAllocated)
                {
                    _notificationMappingsHandle.Free();
                }

                if (_selfHandle.IsAllocated)
                {
                    _selfHandle.Free();
                }

                _disposed = true;
            }
        }

        private int StartDirectoryEnumerationCallback(in ProjFsNative.PrjCallbackData callbackData, in Guid enumerationId)
        {
            try
            {
                var session = new LateralFileSystemEnumerationSession();
                session.Entries.AddRange(EnumerateEntries(callbackData.FilePathName).OrderBy(entry => entry.Name, StringComparer.OrdinalIgnoreCase));
                _enumerationSessions[enumerationId] = session;
                return 0;
            }
            catch (DirectoryNotFoundException)
            {
                return ProjFsNative.ErrorPathNotFound;
            }
            catch (FileNotFoundException)
            {
                return ProjFsNative.ErrorFileNotFound;
            }
            catch (Exception ex)
            {
                return Marshal.GetHRForException(ex);
            }
        }

        private int EndDirectoryEnumerationCallback(in ProjFsNative.PrjCallbackData callbackData, in Guid enumerationId)
        {
            _enumerationSessions.TryRemove(enumerationId, out _);
            return 0;
        }

        private int GetDirectoryEnumerationCallback(in ProjFsNative.PrjCallbackData callbackData, in Guid enumerationId, string? searchExpression, IntPtr dirEntryBufferHandle)
        {
            try
            {
                if (!_enumerationSessions.TryGetValue(enumerationId, out var session))
                {
                    return Marshal.GetHRForException(new InvalidOperationException("目录枚举会话不存在。"));
                }

                if ((callbackData.Flags & ProjFsNative.PrjCallbackDataFlags.EnumerationRestartScan) != 0)
                {
                    session.NextIndex = 0;
                    session.SearchExpression = NormalizeSearchExpression(searchExpression);
                }
                else if (session.NextIndex == 0)
                {
                    session.SearchExpression = NormalizeSearchExpression(searchExpression);
                }

                var entriesAdded = 0;
                while (session.NextIndex < session.Entries.Count)
                {
                    var entry = session.Entries[session.NextIndex];
                    if (!MatchesSearchExpression(session.SearchExpression, entry.Name))
                    {
                        session.NextIndex++;
                        continue;
                    }

                    var basicInfo = CreateFileBasicInfo(entry);
                    var hr = ProjFsNative.PrjFillDirEntryBuffer(entry.Name, in basicInfo, dirEntryBufferHandle);
                    if (hr == ProjFsNative.ErrorInsufficientBuffer)
                    {
                        return entriesAdded == 0 ? hr : 0;
                    }

                    if (hr < 0)
                    {
                        return hr;
                    }

                    entriesAdded++;
                    session.NextIndex++;
                }

                return 0;
            }
            catch (Exception ex)
            {
                return Marshal.GetHRForException(ex);
            }
        }

        private int GetPlaceholderInfoCallback(in ProjFsNative.PrjCallbackData callbackData)
        {
            try
            {
                var entry = GetEntry(callbackData.FilePathName);
                if (entry is null)
                {
                    return ProjFsNative.ErrorFileNotFound;
                }

                var placeholderInfo = CreatePlaceholderInfo(entry);
                var hr = ProjFsNative.PrjWritePlaceholderInfo(callbackData.NamespaceVirtualizationContext, callbackData.FilePathName, in placeholderInfo, (uint)Marshal.SizeOf<ProjFsNative.PrjPlaceholderInfo>());
                return hr;
            }
            catch (Exception ex)
            {
                return Marshal.GetHRForException(ex);
            }
        }

        private int GetFileDataCallback(in ProjFsNative.PrjCallbackData callbackData, ulong byteOffset, uint length)
        {
            try
            {
                var entry = GetEntry(callbackData.FilePathName);
                if (entry is null || entry.IsDirectory)
                {
                    return ProjFsNative.ErrorFileNotFound;
                }

                using var stream = new FileStream(entry.FullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
                if (byteOffset > (ulong)stream.Length)
                {
                    return ProjFsNative.ErrorFileNotFound;
                }

                var remaining = (int)Math.Min(length, (uint)Math.Max(0, stream.Length - (long)byteOffset));
                if (remaining == 0)
                {
                    return 0;
                }

                stream.Position = (long)byteOffset;
                var buffer = ProjFsNative.PrjAllocateAlignedBuffer(callbackData.NamespaceVirtualizationContext, (nuint)remaining);
                if (buffer == IntPtr.Zero)
                {
                    throw new OutOfMemoryException();
                }

                try
                {
                    var managedBuffer = new byte[remaining];
                    var read = stream.Read(managedBuffer, 0, managedBuffer.Length);
                    Marshal.Copy(managedBuffer, 0, buffer, read);
                    return ProjFsNative.PrjWriteFileData(callbackData.NamespaceVirtualizationContext, in callbackData.DataStreamId, buffer, byteOffset, (uint)read);
                }
                finally
                {
                    ProjFsNative.PrjFreeAlignedBuffer(buffer);
                }
            }
            catch (Exception ex)
            {
                return Marshal.GetHRForException(ex);
            }
        }

        private int NotificationCallback(in ProjFsNative.PrjCallbackData callbackData, bool isDirectory, ProjFsNative.PrjNotification notification, string? destinationFileName, ref ProjFsNative.PrjNotificationParameters operationParameters)
        {
            try
            {
                var relativePath = NormalizeRelativePath(callbackData.FilePathName);
                var destinationRelativePath = NormalizeRelativePath(destinationFileName ?? string.Empty);

                switch (notification)
                {
                    case ProjFsNative.PrjNotification.NewFileCreated:
                        MarkLocalOnly(relativePath);
                        operationParameters.PostCreate.NotificationMask = ProjFsNative.PrjNotification.UseExistingMask;
                        break;

                    case ProjFsNative.PrjNotification.FileOverwritten:
                    case ProjFsNative.PrjNotification.FilePreConvertToFull:
                    case ProjFsNative.PrjNotification.FileHandleClosedFileModified:
                        MarkEdited(relativePath);
                        break;

                    case ProjFsNative.PrjNotification.FileHandleClosedFileDeleted:
                        UnmarkEdited(relativePath);
                        UnmarkLocalOnly(relativePath);
                        break;

                    case ProjFsNative.PrjNotification.PreDelete:
                        if (string.IsNullOrWhiteSpace(relativePath))
                        {
                            return ProjFsNative.ErrorAccessDenied;
                        }
                        break;

                    case ProjFsNative.PrjNotification.PreRename:
                        if (string.IsNullOrWhiteSpace(relativePath))
                        {
                            return ProjFsNative.ErrorAccessDenied;
                        }
                        break;

                    case ProjFsNative.PrjNotification.FileRenamed:
                        RenameTrackedPath(relativePath, destinationRelativePath);
                        operationParameters.FileRenamed.NotificationMask = ProjFsNative.PrjNotification.UseExistingMask;
                        break;
                }

                return 0;
            }
            catch (Exception ex)
            {
                return Marshal.GetHRForException(ex);
            }
        }

        private void OnSourceCreated(object sender, FileSystemEventArgs e)
        {
            try
            {
                lock (_syncRoot)
                {
                    if (_disposed)
                    {
                        return;
                    }

                    var entry = GetEntryFromSourcePath(e.FullPath);
                    if (entry is not null)
                    {
                        EnsurePlaceholder(entry);
                    }
                }

                SourceChanged?.Invoke(this, new LateralFileSystemSourceChangedEventArgs(Node, e.ChangeType, e.FullPath, null));
            }
            catch
            {
            }
        }

        private void OnSourceDeleted(object sender, FileSystemEventArgs e)
        {
            try
            {
                lock (_syncRoot)
                {
                    if (_disposed)
                    {
                        return;
                    }

                    var relativePath = NormalizeRelativePath(Path.GetRelativePath(SourceRootDirectory, e.FullPath));
                    RemoveSourceRemovedPath(relativePath);
                }

                SourceChanged?.Invoke(this, new LateralFileSystemSourceChangedEventArgs(Node, e.ChangeType, e.FullPath, null));
            }
            catch
            {
            }
        }

        private void OnSourceRenamed(object sender, RenamedEventArgs e)
        {
            try
            {
                lock (_syncRoot)
                {
                    if (_disposed)
                    {
                        return;
                    }

                    var oldRelativePath = NormalizeRelativePath(Path.GetRelativePath(SourceRootDirectory, e.OldFullPath));
                    var newRelativePath = NormalizeRelativePath(Path.GetRelativePath(SourceRootDirectory, e.FullPath));
                    RemoveSourceRemovedPath(oldRelativePath);
                    RenameTrackedPath(oldRelativePath, newRelativePath);
                    var entry = GetEntryFromSourcePath(e.FullPath);
                    if (entry is not null)
                    {
                        EnsurePlaceholder(entry);
                    }
                }

                SourceChanged?.Invoke(this, new LateralFileSystemSourceChangedEventArgs(Node, e.ChangeType, e.FullPath, e.OldFullPath));
            }
            catch
            {
            }
        }

        private void OnSourceChanged(object sender, FileSystemEventArgs e)
        {
            SourceChanged?.Invoke(this, new LateralFileSystemSourceChangedEventArgs(Node, e.ChangeType, e.FullPath, null));
        }

        private void EnsurePlaceholder(LateralFileSystemEntry entry)
        {
            if (_namespaceContext == IntPtr.Zero)
            {
                return;
            }

            var localPath = Path.Combine(Node.VirtualRootPath, entry.RelativePath);
            var localDirectory = Path.GetDirectoryName(localPath);
            if (!string.IsNullOrWhiteSpace(localDirectory))
            {
                Directory.CreateDirectory(localDirectory);
            }

            var placeholderInfo = CreatePlaceholderInfo(entry);
            var hr = ProjFsNative.PrjWritePlaceholderInfo(_namespaceContext, entry.RelativePath, in placeholderInfo, (uint)Marshal.SizeOf<ProjFsNative.PrjPlaceholderInfo>());
            if (hr < 0 && hr != unchecked((int)0x800700b7))
            {
                Marshal.ThrowExceptionForHR(hr);
            }
        }

        private void RemoveSourceRemovedPath(string relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
            {
                return;
            }

            var localPath = Path.Combine(Node.VirtualRootPath, relativePath);
            if (IsEdited(relativePath))
            {
                return;
            }

            if (File.Exists(localPath))
            {
                File.Delete(localPath);
                return;
            }

            if (Directory.Exists(localPath))
            {
                var descendants = Directory.EnumerateFileSystemEntries(localPath, "*", SearchOption.AllDirectories)
                    .Select(path => NormalizeRelativePath(Path.GetRelativePath(Node.VirtualRootPath, path)))
                    .OrderByDescending(path => path.Length)
                    .ToList();

                foreach (var descendant in descendants)
                {
                    if (IsEdited(descendant))
                    {
                        continue;
                    }

                    RemoveLocalPath(descendant);
                }

                if (!Directory.EnumerateFileSystemEntries(localPath).Any())
                {
                    Directory.Delete(localPath, recursive: false);
                }
            }
        }

        private void RemoveLocalPath(string relativePath)
        {
            var localPath = Path.Combine(Node.VirtualRootPath, relativePath);
            if (File.Exists(localPath))
            {
                File.Delete(localPath);
                return;
            }

            if (Directory.Exists(localPath) && !Directory.EnumerateFileSystemEntries(localPath).Any())
            {
                Directory.Delete(localPath, recursive: false);
            }
        }

        private IReadOnlyList<LateralFileSystemEntry> EnumerateEntries(string relativeDirectoryPath)
        {
            var sourceDirectoryPath = GetSourcePath(relativeDirectoryPath);
            if (!Directory.Exists(sourceDirectoryPath))
            {
                throw new DirectoryNotFoundException(sourceDirectoryPath);
            }

            var entries = new List<LateralFileSystemEntry>();
            foreach (var entryPath in Directory.EnumerateFileSystemEntries(sourceDirectoryPath))
            {
                var entry = GetEntryFromSourcePath(entryPath);
                if (entry is not null)
                {
                    entries.Add(entry);
                }
            }

            return entries;
        }

        private LateralFileSystemEntry? GetEntry(string relativePath)
        {
            relativePath = NormalizeRelativePath(relativePath);
            if (string.IsNullOrWhiteSpace(relativePath))
            {
                return new LateralFileSystemEntry
                {
                    RelativePath = string.Empty,
                    Name = Path.GetFileName(Node.VirtualRootPath),
                    FullPath = SourceRootDirectory,
                    IsDirectory = true,
                    CreationTimeUtc = Directory.GetCreationTimeUtc(SourceRootDirectory),
                    LastAccessTimeUtc = Directory.GetLastAccessTimeUtc(SourceRootDirectory),
                    LastWriteTimeUtc = Directory.GetLastWriteTimeUtc(SourceRootDirectory),
                    ChangeTimeUtc = Directory.GetLastWriteTimeUtc(SourceRootDirectory),
                    FileAttributes = (uint)FileAttributes.Directory
                };
            }

            var sourcePath = GetSourcePath(relativePath);
            return GetEntryFromSourcePath(sourcePath);
        }

        private LateralFileSystemEntry? GetEntryFromSourcePath(string sourcePath)
        {
            if (Directory.Exists(sourcePath))
            {
                var info = new DirectoryInfo(sourcePath);
                return new LateralFileSystemEntry
                {
                    RelativePath = NormalizeRelativePath(Path.GetRelativePath(SourceRootDirectory, sourcePath)),
                    Name = info.Name,
                    FullPath = info.FullName,
                    IsDirectory = true,
                    CreationTimeUtc = info.CreationTimeUtc,
                    LastAccessTimeUtc = info.LastAccessTimeUtc,
                    LastWriteTimeUtc = info.LastWriteTimeUtc,
                    ChangeTimeUtc = info.LastWriteTimeUtc,
                    FileAttributes = (uint)info.Attributes
                };
            }

            if (File.Exists(sourcePath))
            {
                var info = new FileInfo(sourcePath);
                return new LateralFileSystemEntry
                {
                    RelativePath = NormalizeRelativePath(Path.GetRelativePath(SourceRootDirectory, sourcePath)),
                    Name = info.Name,
                    FullPath = info.FullName,
                    IsDirectory = false,
                    FileSize = info.Length,
                    CreationTimeUtc = info.CreationTimeUtc,
                    LastAccessTimeUtc = info.LastAccessTimeUtc,
                    LastWriteTimeUtc = info.LastWriteTimeUtc,
                    ChangeTimeUtc = info.LastWriteTimeUtc,
                    FileAttributes = (uint)info.Attributes
                };
            }

            return null;
        }

        private string GetSourcePath(string relativePath)
        {
            relativePath = NormalizeRelativePath(relativePath);
            return string.IsNullOrWhiteSpace(relativePath)
                ? SourceRootDirectory
                : Path.Combine(SourceRootDirectory, relativePath);
        }

        private bool IsEdited(string relativePath)
        {
            relativePath = NormalizeRelativePath(relativePath);
            if (string.IsNullOrWhiteSpace(relativePath))
            {
                return false;
            }

            return Node.EditedRelativePaths.Any(path => string.Equals(path, relativePath, StringComparison.OrdinalIgnoreCase)
                || path.StartsWith(relativePath + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase)
                || path.StartsWith(relativePath + Path.AltDirectorySeparatorChar, StringComparison.OrdinalIgnoreCase));
        }

        private void MarkEdited(string relativePath)
        {
            relativePath = NormalizeRelativePath(relativePath);
            if (string.IsNullOrWhiteSpace(relativePath))
            {
                return;
            }

            if (!Node.EditedRelativePaths.Any(path => string.Equals(path, relativePath, StringComparison.OrdinalIgnoreCase)))
            {
                Node.EditedRelativePaths.Add(relativePath);
                Node.UpdatedAtUtc = DateTime.UtcNow;
                PersistNode();
            }
        }

        private void UnmarkEdited(string relativePath)
        {
            relativePath = NormalizeRelativePath(relativePath);
            RemoveTrackedPath(Node.EditedRelativePaths, relativePath);
        }

        private void MarkLocalOnly(string relativePath)
        {
            relativePath = NormalizeRelativePath(relativePath);
            if (string.IsNullOrWhiteSpace(relativePath))
            {
                return;
            }

            if (!Node.LocalOnlyRelativePaths.Any(path => string.Equals(path, relativePath, StringComparison.OrdinalIgnoreCase)))
            {
                Node.LocalOnlyRelativePaths.Add(relativePath);
                Node.UpdatedAtUtc = DateTime.UtcNow;
                PersistNode();
            }
        }

        private void UnmarkLocalOnly(string relativePath)
        {
            relativePath = NormalizeRelativePath(relativePath);
            RemoveTrackedPath(Node.LocalOnlyRelativePaths, relativePath);
        }

        private void RenameTrackedPath(string sourceRelativePath, string destinationRelativePath)
        {
            sourceRelativePath = NormalizeRelativePath(sourceRelativePath);
            destinationRelativePath = NormalizeRelativePath(destinationRelativePath);
            RenameTrackedCollection(Node.EditedRelativePaths, sourceRelativePath, destinationRelativePath);
            RenameTrackedCollection(Node.LocalOnlyRelativePaths, sourceRelativePath, destinationRelativePath);
            Node.UpdatedAtUtc = DateTime.UtcNow;
            PersistNode();
        }

        private void RemoveTrackedPath(ICollection<string> collection, string relativePath)
        {
            var removed = collection.Where(path => string.Equals(path, relativePath, StringComparison.OrdinalIgnoreCase)
                || path.StartsWith(relativePath + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase)
                || path.StartsWith(relativePath + Path.AltDirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (removed.Count == 0)
            {
                return;
            }

            foreach (var item in removed)
            {
                collection.Remove(item);
            }

            Node.UpdatedAtUtc = DateTime.UtcNow;
            PersistNode();
        }

        private static void RenameTrackedCollection(ICollection<string> collection, string sourceRelativePath, string destinationRelativePath)
        {
            if (string.IsNullOrWhiteSpace(sourceRelativePath) || string.IsNullOrWhiteSpace(destinationRelativePath))
            {
                return;
            }

            var matches = collection.Where(path => string.Equals(path, sourceRelativePath, StringComparison.OrdinalIgnoreCase)
                || path.StartsWith(sourceRelativePath + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase)
                || path.StartsWith(sourceRelativePath + Path.AltDirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
                .ToList();

            foreach (var match in matches)
            {
                collection.Remove(match);
                var suffix = match.Length == sourceRelativePath.Length ? string.Empty : match[sourceRelativePath.Length..];
                collection.Add(destinationRelativePath + suffix);
            }
        }

        private void PersistNode()
        {
            var tree = _treeRepository.Load(WorkingRootDirectory);
            var existingNode = tree.Nodes.FirstOrDefault(candidate => string.Equals(candidate.Id, Node.Id, StringComparison.OrdinalIgnoreCase));
            if (existingNode is null)
            {
                return;
            }

            CopyNode(existingNode, Node);
            _treeRepository.Save(WorkingRootDirectory, tree);
        }

        private static void CopyNode(LateralFileSystemNodeModel target, LateralFileSystemNodeModel source)
        {
            target.Name = source.Name;
            target.VirtualRootPath = source.VirtualRootPath;
            target.Owner = source.Owner;
            target.Kind = source.Kind;
            target.ProjectionSourcePath = source.ProjectionSourcePath;
            target.ParentNodeId = source.ParentNodeId;
            target.IsActive = source.IsActive;
            target.ProviderInstanceId = source.ProviderInstanceId;
            target.ContentVersion = source.ContentVersion;
            target.CreatedAtUtc = source.CreatedAtUtc;
            target.UpdatedAtUtc = source.UpdatedAtUtc;
            target.SchemaVersion = source.SchemaVersion;
            target.EditedRelativePaths.Clear();
            foreach (var path in source.EditedRelativePaths)
            {
                target.EditedRelativePaths.Add(path);
            }

            target.LocalOnlyRelativePaths.Clear();
            foreach (var path in source.LocalOnlyRelativePaths)
            {
                target.LocalOnlyRelativePaths.Add(path);
            }

            target.Properties.Clear();
            foreach (var property in source.Properties)
            {
                target.Properties.Add(new LateralFileSystemNodeProperty { Key = property.Key, Value = property.Value });
            }
        }

        private static ProjFsNative.PrjPlaceholderInfo CreatePlaceholderInfo(LateralFileSystemEntry entry)
        {
            return new ProjFsNative.PrjPlaceholderInfo
            {
                FileBasicInfo = CreateFileBasicInfo(entry),
                EaInformation = default,
                SecurityInformation = default,
                StreamsInformation = default,
                VersionInfo = CreateVersionInfo(entry.RelativePath)
            };
        }

        private static ProjFsNative.PrjFileBasicInfo CreateFileBasicInfo(LateralFileSystemEntry entry)
        {
            return new ProjFsNative.PrjFileBasicInfo
            {
                IsDirectory = entry.IsDirectory,
                FileSize = entry.IsDirectory ? 0 : entry.FileSize,
                CreationTime = entry.CreationTimeUtc.ToFileTimeUtc(),
                LastAccessTime = entry.LastAccessTimeUtc.ToFileTimeUtc(),
                LastWriteTime = entry.LastWriteTimeUtc.ToFileTimeUtc(),
                ChangeTime = entry.ChangeTimeUtc.ToFileTimeUtc(),
                FileAttributes = entry.FileAttributes
            };
        }

        private static ProjFsNative.PrjPlaceholderVersionInfo CreateVersionInfo(string relativePath)
        {
            var providerId = new byte[ProjFsNative.PlaceholderIdLength];
            var contentId = new byte[ProjFsNative.PlaceholderIdLength];
            var providerBytes = System.Text.Encoding.UTF8.GetBytes("LateralFS");
            var contentBytes = System.Text.Encoding.UTF8.GetBytes(relativePath);
            Array.Copy(providerBytes, providerId, Math.Min(providerBytes.Length, providerId.Length));
            Array.Copy(contentBytes, contentId, Math.Min(contentBytes.Length, contentId.Length));
            return new ProjFsNative.PrjPlaceholderVersionInfo
            {
                ProviderId = providerId,
                ContentId = contentId
            };
        }

        private static string NormalizeRelativePath(string relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath) || relativePath == ".")
            {
                return string.Empty;
            }

            return relativePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar).TrimStart(Path.DirectorySeparatorChar);
        }

        private static string NormalizeSearchExpression(string? searchExpression)
        {
            return string.IsNullOrWhiteSpace(searchExpression) ? "*" : searchExpression;
        }

        private static bool MatchesSearchExpression(string searchExpression, string fileName)
        {
            var pattern = "^" + Regex.Escape(searchExpression)
                .Replace("\\*", ".*")
                .Replace("\\?", ".") + "$";
            return Regex.IsMatch(fileName, pattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        }

        private void ThrowIfDisposed()
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
        }
    }
}
