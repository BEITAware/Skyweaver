using System.IO;
using Skyweaver.Models.LateralFileSystem;

namespace Skyweaver.Services.LateralFileSystem
{
    public sealed class LateralFileSystemSourceChangedEventArgs : EventArgs
    {
        public LateralFileSystemSourceChangedEventArgs(LateralFileSystemNodeModel node, WatcherChangeTypes changeType, string fullPath, string? oldFullPath)
        {
            Node = node;
            ChangeType = changeType;
            FullPath = fullPath;
            OldFullPath = oldFullPath;
        }

        public LateralFileSystemNodeModel Node { get; }

        public WatcherChangeTypes ChangeType { get; }

        public string FullPath { get; }

        public string? OldFullPath { get; }
    }
}
