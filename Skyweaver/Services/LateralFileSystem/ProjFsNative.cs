using System.Runtime.InteropServices;

namespace Skyweaver.Services.LateralFileSystem
{
    internal static class ProjFsNative
    {
        public const int PlaceholderIdLength = 128;
        public const int ErrorInsufficientBuffer = unchecked((int)0x8007007A);
        public const int ErrorFileNotFound = unchecked((int)0x80070002);
        public const int ErrorPathNotFound = unchecked((int)0x80070003);
        public const int ErrorAccessDenied = unchecked((int)0x80070005);

        public static bool TryGetAvailability(out string? unavailableReason)
        {
            IntPtr handle = IntPtr.Zero;

            try
            {
                if (NativeLibrary.TryLoad("ProjectedFSLib.dll", typeof(ProjFsNative).Assembly, searchPath: null, out handle))
                {
                    unavailableReason = null;
                    return true;
                }

                unavailableReason = BuildAvailabilityErrorMessage(exception: null);
                return false;
            }
            catch (Exception ex) when (IsAvailabilityException(ex))
            {
                unavailableReason = BuildAvailabilityErrorMessage(ex);
                return false;
            }
            finally
            {
                if (handle != IntPtr.Zero)
                {
                    NativeLibrary.Free(handle);
                }
            }
        }

        public static bool IsAvailabilityException(Exception exception)
        {
            ArgumentNullException.ThrowIfNull(exception);
            return exception is DllNotFoundException or EntryPointNotFoundException or BadImageFormatException;
        }

        public static string BuildAvailabilityErrorMessage(Exception? exception)
        {
            return exception switch
            {
                BadImageFormatException => "侧向文件系统原生后端不可用：ProjectedFSLib.dll 或其依赖项的位数不匹配，无法加载。",
                EntryPointNotFoundException => "侧向文件系统原生后端不可用：ProjectedFSLib.dll 已找到，但缺少所需的 ProjFS 导出函数。",
                DllNotFoundException => "侧向文件系统原生后端不可用：无法加载 ProjectedFSLib.dll 或其依赖项。通常是系统未启用 Projected File System，或相关组件缺失。",
                _ => "侧向文件系统原生后端不可用：无法加载 ProjectedFSLib.dll。通常是系统未启用 Projected File System，或相关组件缺失。"
            };
        }

        [Flags]
        public enum PrjFileState : uint
        {
            None = 0,
            Placeholder = 0x00000001,
            HydratedPlaceholder = 0x00000002,
            DirtyPlaceholder = 0x00000004,
            Full = 0x00000008,
            Tombstone = 0x00000010
        }

        [Flags]
        public enum PrjNotification : uint
        {
            SuppressNotifications = 0x00000001,
            FileOpened = 0x00000002,
            NewFileCreated = 0x00000004,
            FileOverwritten = 0x00000008,
            PreDelete = 0x00000010,
            PreRename = 0x00000020,
            PreSetHardlink = 0x00000040,
            FileRenamed = 0x00000080,
            HardlinkCreated = 0x00000100,
            FileHandleClosedNoModification = 0x00000200,
            FileHandleClosedFileModified = 0x00000400,
            FileHandleClosedFileDeleted = 0x00000800,
            FilePreConvertToFull = 0x00001000,
            UseExistingMask = 0xffffffff
        }

        [Flags]
        public enum PrjCallbackDataFlags : uint
        {
            None = 0,
            EnumerationRestartScan = 0x00000001
        }

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        public delegate int PrjStartDirectoryEnumerationCb(in PrjCallbackData callbackData, in Guid enumerationId);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        public delegate int PrjEndDirectoryEnumerationCb(in PrjCallbackData callbackData, in Guid enumerationId);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        public delegate int PrjGetDirectoryEnumerationCb(in PrjCallbackData callbackData, in Guid enumerationId, string? searchExpression, IntPtr dirEntryBufferHandle);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        public delegate int PrjGetPlaceholderInfoCb(in PrjCallbackData callbackData);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        public delegate int PrjGetFileDataCb(in PrjCallbackData callbackData, ulong byteOffset, uint length);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        public delegate int PrjQueryFileNameCb(in PrjCallbackData callbackData);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        public delegate int PrjNotificationCb(in PrjCallbackData callbackData, [MarshalAs(UnmanagedType.Bool)] bool isDirectory, PrjNotification notification, string? destinationFileName, ref PrjNotificationParameters operationParameters);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        public delegate void PrjCancelCommandCb(in PrjCallbackData callbackData);

        [StructLayout(LayoutKind.Sequential)]
        public struct PrjCallbacks
        {
            public IntPtr StartDirectoryEnumerationCallback;
            public IntPtr EndDirectoryEnumerationCallback;
            public IntPtr GetDirectoryEnumerationCallback;
            public IntPtr GetPlaceholderInfoCallback;
            public IntPtr GetFileDataCallback;
            public IntPtr QueryFileNameCallback;
            public IntPtr NotificationCallback;
            public IntPtr CancelCommandCallback;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PrjNotificationMapping
        {
            public PrjNotification NotificationBitMask;
            public IntPtr NotificationRoot;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PrjStartVirtualizingOptions
        {
            public uint Flags;
            public uint PoolThreadCount;
            public uint ConcurrentThreadCount;
            public IntPtr NotificationMappings;
            public uint NotificationMappingsCount;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PrjPlaceholderVersionInfo
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = PlaceholderIdLength)]
            public byte[] ProviderId;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = PlaceholderIdLength)]
            public byte[] ContentId;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PrjFileBasicInfo
        {
            [MarshalAs(UnmanagedType.I1)]
            public bool IsDirectory;
            public long FileSize;
            public long CreationTime;
            public long LastAccessTime;
            public long LastWriteTime;
            public long ChangeTime;
            public uint FileAttributes;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PrjInfoStruct
        {
            public uint BufferSize;
            public uint Offset;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PrjPlaceholderInfo
        {
            public PrjFileBasicInfo FileBasicInfo;
            public PrjInfoStruct EaInformation;
            public PrjInfoStruct SecurityInformation;
            public PrjInfoStruct StreamsInformation;
            public PrjPlaceholderVersionInfo VersionInfo;
            public byte VariableData;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PrjCallbackData
        {
            public uint Size;
            public PrjCallbackDataFlags Flags;
            public IntPtr NamespaceVirtualizationContext;
            public int CommandId;
            public Guid FileId;
            public Guid DataStreamId;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string FilePathName;
            public IntPtr VersionInfo;
            public uint TriggeringProcessId;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string? TriggeringProcessImageFileName;
            public IntPtr InstanceContext;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NotificationMaskInfo
        {
            public PrjNotification NotificationMask;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct FileDeletedOnHandleCloseInfo
        {
            [MarshalAs(UnmanagedType.I1)]
            public bool IsFileModified;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct PrjNotificationParameters
        {
            [FieldOffset(0)]
            public NotificationMaskInfo PostCreate;

            [FieldOffset(0)]
            public NotificationMaskInfo FileRenamed;

            [FieldOffset(0)]
            public FileDeletedOnHandleCloseInfo FileDeletedOnHandleClose;
        }

        [DllImport("ProjectedFSLib.dll", CharSet = CharSet.Unicode, PreserveSig = true)]
        public static extern int PrjStartVirtualizing(
            string virtualizationRootPath,
            in PrjCallbacks callbacks,
            IntPtr instanceContext,
            in PrjStartVirtualizingOptions options,
            out IntPtr namespaceVirtualizationContext);

        [DllImport("ProjectedFSLib.dll", CharSet = CharSet.Unicode, PreserveSig = true)]
        public static extern void PrjStopVirtualizing(IntPtr namespaceVirtualizationContext);

        [DllImport("ProjectedFSLib.dll", CharSet = CharSet.Unicode, PreserveSig = true)]
        public static extern int PrjMarkDirectoryAsPlaceholder(string rootPathName, string? targetPathName, IntPtr versionInfo, in Guid virtualizationInstanceId);

        [DllImport("ProjectedFSLib.dll", CharSet = CharSet.Unicode, PreserveSig = true)]
        public static extern int PrjWritePlaceholderInfo(IntPtr namespaceVirtualizationContext, string destinationFileName, in PrjPlaceholderInfo placeholderInfo, uint placeholderInfoSize);

        [DllImport("ProjectedFSLib.dll", CharSet = CharSet.Unicode, PreserveSig = true)]
        public static extern int PrjFillDirEntryBuffer(string fileName, in PrjFileBasicInfo fileBasicInfo, IntPtr dirEntryBufferHandle);

        [DllImport("ProjectedFSLib.dll", CharSet = CharSet.Unicode, PreserveSig = true)]
        public static extern int PrjGetOnDiskFileState(string destinationFileName, out PrjFileState fileState);

        [DllImport("ProjectedFSLib.dll", CharSet = CharSet.Unicode, PreserveSig = true)]
        public static extern int PrjWriteFileData(IntPtr namespaceVirtualizationContext, in Guid dataStreamId, IntPtr buffer, ulong byteOffset, uint length);

        [DllImport("ProjectedFSLib.dll", CharSet = CharSet.Unicode, PreserveSig = true)]
        public static extern IntPtr PrjAllocateAlignedBuffer(IntPtr namespaceVirtualizationContext, nuint size);

        [DllImport("ProjectedFSLib.dll", CharSet = CharSet.Unicode, PreserveSig = true)]
        public static extern void PrjFreeAlignedBuffer(IntPtr buffer);
    }
}
