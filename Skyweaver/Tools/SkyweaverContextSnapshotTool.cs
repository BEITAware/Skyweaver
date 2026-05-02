using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using Skyweaver.Controls.ChatSessionControl.Views;
using Skyweaver.Models.LateralFileSystem;
using Skyweaver.Services.LateralFileSystem;
using Skyweaver.Services.SkyweaverTools;

namespace Skyweaver.Tools
{
    public sealed class SkyweaverContextSnapshotTool : ISkyweaverTool, ISkyweaverToolInvocationPresentationProvider
    {
        public const string ToolName = "SkyweaverContextSnapshot";

        private static readonly SkyweaverToolDefinition s_definition = new(
            ToolName,
            "Reads the current Skyweaver runtime context and returns a concise environment snapshot.",
            "GuideBot",
            [
                new SkyweaverToolParameterDefinition(
                    "IncludeProperties",
                    "Whether to include custom context properties in the snapshot.",
                    SkyweaverToolParameterType.Boolean,
                    isRequired: false,
                    defaultValue: "true"),
                new SkyweaverToolParameterDefinition(
                    "IncludeOperatingSystem",
                    "Whether to include operating system and process runtime information.",
                    SkyweaverToolParameterType.Boolean,
                    isRequired: false,
                    defaultValue: "true"),
                new SkyweaverToolParameterDefinition(
                    "IncludeFileSystem",
                    "Whether to enumerate all mounted filesystem volumes.",
                    SkyweaverToolParameterType.Boolean,
                    isRequired: false,
                    defaultValue: "true"),
                new SkyweaverToolParameterDefinition(
                    "IncludeLateralFS",
                    "Whether to include LateralFS configuration and node information.",
                    SkyweaverToolParameterType.Boolean,
                    isRequired: false,
                    defaultValue: "true"),
                new SkyweaverToolParameterDefinition(
                    "UppercaseLabels",
                    "Whether to render snapshot labels in uppercase.",
                    SkyweaverToolParameterType.Boolean,
                    isRequired: false,
                    defaultValue: "false")
            ]);

        public SkyweaverToolDefinition Definition => s_definition;

        public FrameworkElement? CreateInvocationPresentation(SkyweaverToolInvocationPresentationContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            return ToolInvocationCardFactory.Create(
                context,
                [
                    new ToolInvocationCardFieldDefinition("Properties", "IncludeProperties", "Default: included"),
                    new ToolInvocationCardFieldDefinition("OS", "IncludeOperatingSystem", "Default: included"),
                    new ToolInvocationCardFieldDefinition("Volumes", "IncludeFileSystem", "Default: included"),
                    new ToolInvocationCardFieldDefinition("LateralFS", "IncludeLateralFS", "Default: included"),
                    new ToolInvocationCardFieldDefinition("Uppercase", "UppercaseLabels", "Default: normal")
                ]);
        }

        public Task<SkyweaverToolResult> ExecuteAsync(
            SkyweaverToolContext context,
            SkyweaverToolArguments arguments,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var includeProperties = arguments.GetBoolean("IncludeProperties", true);
            var includeOperatingSystem = arguments.GetBoolean("IncludeOperatingSystem", true);
            var includeFileSystem = arguments.GetBoolean("IncludeFileSystem", true);
            var includeLateralFileSystem = arguments.GetBoolean("IncludeLateralFS", true);
            var uppercaseLabels = arguments.GetBoolean("UppercaseLabels");

            string Label(string value) => uppercaseLabels ? value.ToUpperInvariant() : value;

            var operatingSystem = includeOperatingSystem ? BuildOperatingSystemSnapshot() : null;
            var volumes = includeFileSystem ? BuildFileSystemVolumeSnapshots() : Array.Empty<FileSystemVolumeSnapshot>();
            var lateralFileSystem = includeLateralFileSystem ? BuildLateralFileSystemSnapshot() : null;

            var lines = new List<string>
            {
                $"{Label("Application")}: {context.ApplicationName}",
                $"{Label("Timestamp")}: {context.Timestamp:yyyy-MM-dd HH:mm:ss zzz}",
                $"{Label("Session")}: {(string.IsNullOrWhiteSpace(context.SessionTitle) ? "n/a" : context.SessionTitle)}",
                $"{Label("Workspace")}: {(string.IsNullOrWhiteSpace(context.WorkspacePath) ? "n/a" : context.WorkspacePath)}"
            };

            if (includeOperatingSystem && operatingSystem != null)
            {
                lines.Add($"{Label("OperatingSystem")}:");
                lines.Add($"- Description: {operatingSystem.Description}");
                lines.Add($"- Version: {operatingSystem.Version}");
                lines.Add($"- Platform: {operatingSystem.Platform}");
                lines.Add($"- OSArchitecture: {operatingSystem.OSArchitecture}");
                lines.Add($"- ProcessArchitecture: {operatingSystem.ProcessArchitecture}");
                lines.Add($"- FrameworkDescription: {operatingSystem.FrameworkDescription}");
                lines.Add($"- MachineName: {operatingSystem.MachineName}");
                lines.Add($"- UserName: {operatingSystem.UserName}");
                lines.Add($"- ProcessorCount: {operatingSystem.ProcessorCount}");
                lines.Add($"- CurrentDirectory: {operatingSystem.CurrentDirectory}");
                lines.Add($"- SystemDirectory: {operatingSystem.SystemDirectory}");
            }

            if (includeFileSystem)
            {
                lines.Add($"{Label("FileSystemVolumes")}: {volumes.Count}");
                foreach (var volume in volumes)
                {
                    lines.Add($"- {volume.Name} [{volume.DriveType}] Ready={volume.IsReady}; Format={NullToNotAvailable(volume.DriveFormat)}; Label={NullToNotAvailable(volume.VolumeLabel)}; Root={volume.RootDirectory}");
                    if (volume.IsReady)
                    {
                        lines.Add($"  TotalSize: {FormatBytes(volume.TotalSize)}");
                        lines.Add($"  AvailableFreeSpace: {FormatBytes(volume.AvailableFreeSpace)}");
                        lines.Add($"  TotalFreeSpace: {FormatBytes(volume.TotalFreeSpace)}");
                    }

                    if (!string.IsNullOrWhiteSpace(volume.Error))
                    {
                        lines.Add($"  Error: {volume.Error}");
                    }
                }
            }

            if (includeLateralFileSystem && lateralFileSystem != null)
            {
                lines.Add($"{Label("LateralFS")}:");
                lines.Add($"- IsEnabled: {lateralFileSystem.IsEnabled}");
                lines.Add($"- WorkingRootDirectory: {NullToNotAvailable(lateralFileSystem.WorkingRootDirectory)}");
                lines.Add($"- ConfigurationFilePath: {NullToNotAvailable(lateralFileSystem.ConfigurationFilePath)}");
                lines.Add($"- IsVirtualizationBackendAvailable: {lateralFileSystem.IsVirtualizationBackendAvailable}");
                lines.Add($"- VirtualizationBackendStatusMessage: {NullToNotAvailable(lateralFileSystem.VirtualizationBackendStatusMessage)}");
                lines.Add($"- NodeCount: {lateralFileSystem.Nodes.Count}");
                foreach (var node in lateralFileSystem.Nodes)
                {
                    lines.Add($"  - {node.Name} ({node.Kind})");
                    lines.Add($"    Id: {node.Id}");
                    lines.Add($"    Owner: {NullToNotAvailable(node.Owner)}");
                    lines.Add($"    IsActive: {node.IsActive}");
                    lines.Add($"    VirtualRootPath: {NullToNotAvailable(node.VirtualRootPath)}");
                    lines.Add($"    ProjectionSourcePath: {NullToNotAvailable(node.ProjectionSourcePath)}");
                    lines.Add($"    ParentNodeId: {NullToNotAvailable(node.ParentNodeId)}");
                    lines.Add($"    EditedRelativePathCount: {node.EditedRelativePathCount}");
                    lines.Add($"    LocalOnlyRelativePathCount: {node.LocalOnlyRelativePathCount}");
                    lines.Add($"    CreatedAtUtc: {node.CreatedAtUtc:O}");
                    lines.Add($"    UpdatedAtUtc: {node.UpdatedAtUtc:O}");
                    if (node.Properties.Count > 0)
                    {
                        lines.Add("    Properties:");
                        foreach (var property in node.Properties.OrderBy(item => item.Key, StringComparer.OrdinalIgnoreCase))
                        {
                            lines.Add($"    - {property.Key}: {property.Value}");
                        }
                    }
                }
            }

            if (includeProperties)
            {
                if (context.Properties.Count == 0)
                {
                    lines.Add($"{Label("Properties")}: none");
                }
                else
                {
                    lines.Add($"{Label("Properties")}:");
                    foreach (var property in context.Properties.OrderBy(item => item.Key, StringComparer.OrdinalIgnoreCase))
                    {
                        lines.Add($"- {property.Key}: {property.Value}");
                    }
                }
            }

            return Task.FromResult(SkyweaverToolResult.Success(
                string.Join(Environment.NewLine, lines),
                new Dictionary<string, object?>
                {
                    ["applicationName"] = context.ApplicationName,
                    ["sessionTitle"] = context.SessionTitle,
                    ["workspacePath"] = context.WorkspacePath,
                    ["timestamp"] = context.Timestamp,
                    ["operatingSystem"] = operatingSystem,
                    ["fileSystemVolumes"] = volumes,
                    ["lateralFileSystem"] = lateralFileSystem
                }));
        }

        private static OperatingSystemSnapshot BuildOperatingSystemSnapshot()
        {
            return new OperatingSystemSnapshot(
                RuntimeInformation.OSDescription,
                Environment.OSVersion.VersionString,
                Environment.OSVersion.Platform.ToString(),
                RuntimeInformation.OSArchitecture.ToString(),
                RuntimeInformation.ProcessArchitecture.ToString(),
                RuntimeInformation.FrameworkDescription,
                Environment.MachineName,
                Environment.UserName,
                Environment.ProcessorCount,
                Environment.CurrentDirectory,
                Environment.SystemDirectory);
        }

        private static IReadOnlyList<FileSystemVolumeSnapshot> BuildFileSystemVolumeSnapshots()
        {
            try
            {
                return DriveInfo.GetDrives()
                    .OrderBy(drive => drive.Name, StringComparer.OrdinalIgnoreCase)
                    .Select(BuildFileSystemVolumeSnapshot)
                    .ToArray();
            }
            catch (Exception ex) when (IsExpectedFileSystemException(ex))
            {
                return
                [
                    new FileSystemVolumeSnapshot(
                        "n/a",
                        "Unknown",
                        false,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        ex.Message)
                ];
            }
        }

        private static FileSystemVolumeSnapshot BuildFileSystemVolumeSnapshot(DriveInfo drive)
        {
            try
            {
                if (!drive.IsReady)
                {
                    return new FileSystemVolumeSnapshot(
                        drive.Name,
                        drive.DriveType.ToString(),
                        false,
                        null,
                        null,
                        drive.RootDirectory.FullName,
                        null,
                        null,
                        null,
                        null);
                }

                return new FileSystemVolumeSnapshot(
                    drive.Name,
                    drive.DriveType.ToString(),
                    true,
                    drive.DriveFormat,
                    drive.VolumeLabel,
                    drive.RootDirectory.FullName,
                    drive.TotalSize,
                    drive.AvailableFreeSpace,
                    drive.TotalFreeSpace,
                    null);
            }
            catch (Exception ex) when (IsExpectedFileSystemException(ex))
            {
                return new FileSystemVolumeSnapshot(
                    drive.Name,
                    drive.DriveType.ToString(),
                    false,
                    null,
                    null,
                    SafeGetRootDirectory(drive),
                    null,
                    null,
                    null,
                    ex.Message);
            }
        }

        private static LateralFileSystemSnapshot BuildLateralFileSystemSnapshot()
        {
            try
            {
                var runtime = LateralFileSystemRuntime.Instance;
                var configuration = runtime.GetConfiguration();
                var nodes = runtime.GetNodes()
                    .OrderBy(node => node.CreatedAtUtc)
                    .ThenBy(node => node.Name, StringComparer.OrdinalIgnoreCase)
                    .Select(BuildLateralFileSystemNodeSnapshot)
                    .ToArray();

                return new LateralFileSystemSnapshot(
                    configuration.IsEnabled,
                    configuration.WorkingRootDirectory,
                    runtime.ConfigurationFilePath,
                    runtime.IsVirtualizationBackendAvailable,
                    runtime.VirtualizationBackendStatusMessage,
                    nodes,
                    null);
            }
            catch (Exception ex) when (IsExpectedLateralFileSystemException(ex))
            {
                return new LateralFileSystemSnapshot(
                    false,
                    null,
                    null,
                    false,
                    null,
                    Array.Empty<LateralFileSystemNodeSnapshot>(),
                    ex.Message);
            }
        }

        private static LateralFileSystemNodeSnapshot BuildLateralFileSystemNodeSnapshot(LateralFileSystemNodeModel node)
        {
            return new LateralFileSystemNodeSnapshot(
                node.Id,
                node.Name,
                node.Owner,
                node.Kind.ToString(),
                node.IsActive,
                node.VirtualRootPath,
                node.ProjectionSourcePath,
                node.ParentNodeId,
                node.ProviderInstanceId,
                node.ContentVersion,
                node.CreatedAtUtc,
                node.UpdatedAtUtc,
                node.SchemaVersion,
                node.EditedRelativePaths.Count,
                node.LocalOnlyRelativePaths.Count,
                node.Properties
                    .OrderBy(property => property.Key, StringComparer.OrdinalIgnoreCase)
                    .ToDictionary(property => property.Key, property => property.Value, StringComparer.OrdinalIgnoreCase));
        }

        private static string SafeGetRootDirectory(DriveInfo drive)
        {
            try
            {
                return drive.RootDirectory.FullName;
            }
            catch (Exception ex) when (IsExpectedFileSystemException(ex))
            {
                return string.Empty;
            }
        }

        private static string NullToNotAvailable(string? value)
        {
            return string.IsNullOrWhiteSpace(value) ? "n/a" : value;
        }

        private static string FormatBytes(long? bytes)
        {
            if (!bytes.HasValue)
            {
                return "n/a";
            }

            var value = (double)bytes.Value;
            string[] units = ["B", "KiB", "MiB", "GiB", "TiB", "PiB"];
            var unitIndex = 0;
            while (value >= 1024 && unitIndex < units.Length - 1)
            {
                value /= 1024;
                unitIndex++;
            }

            return unitIndex == 0
                ? $"{bytes.Value} {units[unitIndex]}"
                : $"{value:0.##} {units[unitIndex]} ({bytes.Value} B)";
        }

        private static bool IsExpectedFileSystemException(Exception ex)
        {
            return ex is IOException
                or UnauthorizedAccessException
                or NotSupportedException
                or System.Security.SecurityException;
        }

        private static bool IsExpectedLateralFileSystemException(Exception ex)
        {
            return ex is IOException
                or UnauthorizedAccessException
                or InvalidOperationException
                or ArgumentException
                or NotSupportedException
                or ObjectDisposedException;
        }

        private sealed record OperatingSystemSnapshot(
            string Description,
            string Version,
            string Platform,
            string OSArchitecture,
            string ProcessArchitecture,
            string FrameworkDescription,
            string MachineName,
            string UserName,
            int ProcessorCount,
            string CurrentDirectory,
            string SystemDirectory);

        private sealed record FileSystemVolumeSnapshot(
            string Name,
            string DriveType,
            bool IsReady,
            string? DriveFormat,
            string? VolumeLabel,
            string? RootDirectory,
            long? TotalSize,
            long? AvailableFreeSpace,
            long? TotalFreeSpace,
            string? Error);

        private sealed record LateralFileSystemSnapshot(
            bool IsEnabled,
            string? WorkingRootDirectory,
            string? ConfigurationFilePath,
            bool IsVirtualizationBackendAvailable,
            string? VirtualizationBackendStatusMessage,
            IReadOnlyList<LateralFileSystemNodeSnapshot> Nodes,
            string? Error);

        private sealed record LateralFileSystemNodeSnapshot(
            string Id,
            string Name,
            string Owner,
            string Kind,
            bool IsActive,
            string VirtualRootPath,
            string? ProjectionSourcePath,
            string? ParentNodeId,
            string ProviderInstanceId,
            string ContentVersion,
            DateTime CreatedAtUtc,
            DateTime UpdatedAtUtc,
            int SchemaVersion,
            int EditedRelativePathCount,
            int LocalOnlyRelativePathCount,
            IReadOnlyDictionary<string, string> Properties);
    }
}
