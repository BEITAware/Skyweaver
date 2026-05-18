using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Xml.Linq;
using Skyweaver.Infrastructure.Mvvm;
using Skyweaver.Services.Localization;
using Skyweaver.Services.SkyweaverTools;

namespace Skyweaver.Tools
{
    internal enum ToolFileSystemPermissionScope
    {
        LateralFileSystemOnly,
        FullAccess
    }

    internal sealed record ToolResolvedPathInfo(
        string ResolvedPath,
        string? WorkspaceRelativePath,
        string? LateralNodeName,
        string? LateralNodeId,
        string? LateralNodeVirtualRootPath,
        string? LateralRelativePath,
        bool UsedLateralShortcut)
    {
        public bool IsInsideLateralFileSystem => !string.IsNullOrWhiteSpace(LateralNodeName);
    }

    internal static class ToolFileSystemMutationSupport
    {
        public static ToolResolvedPathInfo ResolvePathInfo(string requestedPath, string? workspacePath)
        {
            ToolFileSystemHelper.LateralFileSystemPathResolution? lateralResolution = null;
            string resolvedPath;

            if (ToolFileSystemHelper.TryResolveLateralFileSystemShortcut(requestedPath, out var shortcutResolution))
            {
                lateralResolution = shortcutResolution;
                resolvedPath = shortcutResolution.ResolvedPath;
            }
            else
            {
                resolvedPath = ToolFileSystemHelper.ResolvePath(requestedPath, workspacePath);
                if (ToolFileSystemHelper.TryGetContainingLateralFileSystemNode(resolvedPath, out var containingResolution))
                {
                    lateralResolution = containingResolution;
                }
            }

            return new ToolResolvedPathInfo(
                resolvedPath,
                ToolFileSystemHelper.TryGetWorkspaceRelativePath(workspacePath, resolvedPath),
                lateralResolution?.NodeName,
                lateralResolution?.NodeId,
                lateralResolution?.NodeVirtualRootPath,
                lateralResolution?.RelativePath,
                lateralResolution?.UsedShortcut ?? false);
        }

        public static ToolResolvedPathInfo ResolveAuthorizedPath(
            string requestedPath,
            string? workspacePath,
            ToolFileSystemPermissionScope permissionScope)
        {
            var pathInfo = ResolvePathInfo(requestedPath, workspacePath);
            if (permissionScope == ToolFileSystemPermissionScope.LateralFileSystemOnly &&
                !pathInfo.IsInsideLateralFileSystem)
            {
                throw new InvalidOperationException(
                    "This tool is configured as LateralFileSystemOnly. The path must resolve inside a LateralFS virtual folder. Prefer LateralFS\\NodeName\\relative\\path.");
            }

            return pathInfo;
        }

        public static void AppendPathInfo(StringBuilder builder, ToolResolvedPathInfo pathInfo)
        {
            ArgumentNullException.ThrowIfNull(builder);
            ArgumentNullException.ThrowIfNull(pathInfo);

            builder.AppendLine($"Path: {pathInfo.ResolvedPath}");
            if (!string.IsNullOrWhiteSpace(pathInfo.WorkspaceRelativePath))
            {
                builder.AppendLine($"WorkspaceRelativePath: {pathInfo.WorkspaceRelativePath}");
            }

            if (pathInfo.IsInsideLateralFileSystem)
            {
                builder.AppendLine($"LateralFSNode: {pathInfo.LateralNodeName}");
                builder.AppendLine($"LateralFSRelativePath: {pathInfo.LateralRelativePath}");
                builder.AppendLine($"UsedLateralFSShortcut: {pathInfo.UsedLateralShortcut}");
            }
        }

        public static string BuildPermissionSentence(ToolFileSystemPermissionScope permissionScope)
        {
            return permissionScope == ToolFileSystemPermissionScope.FullAccess
                ? "Permission: FullAccess, so the tool may modify any file or directory path that the process account can access."
                : "Permission: LateralFileSystemOnly, so the tool may modify only paths that resolve inside LateralFS virtual folders.";
        }

        public static string BuildPermissionDescription(ToolFileSystemPermissionScope permissionScope)
        {
            return permissionScope == ToolFileSystemPermissionScope.FullAccess
                ? "The model can modify any file or directory path that the Skyweaver process account can access. LateralFS shortcuts still work."
                : "The model can modify only files and directories that resolve inside LateralFS virtual folders. LateralFS\\NodeName\\... shortcuts are supported and checked.";
        }

        public static string BuildPermissionPreviewText(ToolFileSystemPermissionScope permissionScope)
        {
            return permissionScope == ToolFileSystemPermissionScope.FullAccess
                ? "Current permission: FullAccess. Normal absolute or relative paths and LateralFS shortcuts are accepted."
                : "Current permission: LateralFileSystemOnly. Use LateralFS\\NodeName\\relative\\path or an actual path under a LateralFS virtual root.";
        }

        public static string BuildPromptDescription(string operationDescription, ToolFileSystemPermissionScope permissionScope)
        {
            return $"{operationDescription} The path may be a normal absolute or relative path, or a LateralFS\\NodeName\\relative\\path shortcut; the shortcut resolves to that node's virtual folder and rejects '..' traversal outside the node. {BuildPermissionSentence(permissionScope)}";
        }

        public static string NormalizeComparisonPath(string path)
        {
            return Path.TrimEndingDirectorySeparator(Path.GetFullPath(path));
        }

        public static bool AreSamePath(string left, string right)
        {
            return string.Equals(
                NormalizeComparisonPath(left),
                NormalizeComparisonPath(right),
                StringComparison.OrdinalIgnoreCase);
        }
    }

    internal sealed class ToolFileSystemPermissionSettings
    {
        public ToolFileSystemPermissionScope PermissionScope { get; set; } =
            ToolFileSystemPermissionScope.LateralFileSystemOnly;

        public XElement ToXElement(string rootElementName)
        {
            if (string.IsNullOrWhiteSpace(rootElementName))
            {
                throw new ArgumentException("Root element name cannot be empty.", nameof(rootElementName));
            }

            return new XElement(
                rootElementName.Trim(),
                new XElement("PermissionScope", PermissionScope.ToString()));
        }

        public static ToolFileSystemPermissionSettings FromConfiguration(
            SkyweaverToolConfigurationState? configuration,
            string rootElementName)
        {
            var payload = configuration?.GetPayload();
            if (payload == null || string.IsNullOrWhiteSpace(rootElementName))
            {
                return new ToolFileSystemPermissionSettings();
            }

            var root = string.Equals(payload.Name.LocalName, rootElementName, StringComparison.OrdinalIgnoreCase)
                ? payload
                : payload.Element(rootElementName);

            if (root == null)
            {
                return new ToolFileSystemPermissionSettings();
            }

            return new ToolFileSystemPermissionSettings
            {
                PermissionScope = ParsePermissionScope((string?)root.Element("PermissionScope"))
            };
        }

        public static ToolFileSystemPermissionScope ParsePermissionScope(string? value)
        {
            var normalized = (value ?? string.Empty).Trim();
            if (normalized.Length == 0)
            {
                return ToolFileSystemPermissionScope.LateralFileSystemOnly;
            }

            if (string.Equals(normalized, "FullAccess", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(normalized, "FullAuthorization", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(normalized, "Full", StringComparison.OrdinalIgnoreCase))
            {
                return ToolFileSystemPermissionScope.FullAccess;
            }

            return ToolFileSystemPermissionScope.LateralFileSystemOnly;
        }
    }

    internal sealed class ToolFileSystemPermissionOption
    {
        public ToolFileSystemPermissionOption(
            ToolFileSystemPermissionScope scope,
            string displayName,
            string description)
        {
            Scope = scope;
            DisplayName = displayName;
            Description = description;
        }

        public ToolFileSystemPermissionScope Scope { get; }

        public string DisplayName { get; }

        public string Description { get; }
    }

    internal sealed class ToolFileSystemPermissionConfigurationViewModel : ObservableObject
    {
        private readonly Action _notifyConfigurationChanged;
        private ToolFileSystemPermissionOption? _selectedPermission;

        public ToolFileSystemPermissionConfigurationViewModel(
            ToolFileSystemPermissionSettings settings,
            Action notifyConfigurationChanged)
        {
            _notifyConfigurationChanged = notifyConfigurationChanged ?? throw new ArgumentNullException(nameof(notifyConfigurationChanged));
            PermissionOptions = new ObservableCollection<ToolFileSystemPermissionOption>
            {
                new(
                    ToolFileSystemPermissionScope.LateralFileSystemOnly,
                    "LateralFS only",
                    ToolFileSystemMutationSupport.BuildPermissionDescription(ToolFileSystemPermissionScope.LateralFileSystemOnly)),
                new(
                    ToolFileSystemPermissionScope.FullAccess,
                    "Full access",
                    ToolFileSystemMutationSupport.BuildPermissionDescription(ToolFileSystemPermissionScope.FullAccess))
            };

            _selectedPermission = PermissionOptions.FirstOrDefault(option => option.Scope == settings.PermissionScope)
                ?? PermissionOptions[0];
        }

        public ObservableCollection<ToolFileSystemPermissionOption> PermissionOptions { get; }

        public ToolFileSystemPermissionOption? SelectedPermission
        {
            get => _selectedPermission;
            set
            {
                if (SetProperty(ref _selectedPermission, value))
                {
                    OnPropertyChanged(nameof(PermissionDescription));
                    OnPropertyChanged(nameof(PreviewText));
                    _notifyConfigurationChanged();
                }
            }
        }

        public string PermissionDescription => SelectedPermission?.Description ?? string.Empty;

        public string PreviewText => ToolFileSystemMutationSupport.BuildPermissionPreviewText(
            SelectedPermission?.Scope ?? ToolFileSystemPermissionScope.LateralFileSystemOnly);

        public ToolFileSystemPermissionSettings ToSettings()
        {
            return new ToolFileSystemPermissionSettings
            {
                PermissionScope = SelectedPermission?.Scope ?? ToolFileSystemPermissionScope.LateralFileSystemOnly
            };
        }
    }

    internal sealed class ToolFileSystemPermissionConfigurationPresenter : SkyweaverToolConfigurationPresenter
    {
        private readonly ToolFileSystemPermissionConfigurationViewModel _viewModel;
        private readonly FrameworkElement _view;
        private readonly string _rootElementName;
        private readonly string _toolName;

        public ToolFileSystemPermissionConfigurationPresenter(
            SkyweaverToolConfigurationEditorContext context,
            string rootElementName,
            string toolName)
        {
            ArgumentNullException.ThrowIfNull(context);

            _rootElementName = string.IsNullOrWhiteSpace(rootElementName)
                ? throw new ArgumentException("Root element name cannot be empty.", nameof(rootElementName))
                : rootElementName.Trim();
            _toolName = string.IsNullOrWhiteSpace(toolName)
                ? context.ToolName
                : toolName.Trim();

            var settings = ToolFileSystemPermissionSettings.FromConfiguration(context.InitialConfiguration, _rootElementName);
            _viewModel = new ToolFileSystemPermissionConfigurationViewModel(settings, RaiseConfigurationChanged);
            _view = CreateView(_viewModel);
        }

        public override FrameworkElement View => _view;

        public override bool TryCaptureConfiguration(out XElement? configuration, out string? errorMessage)
        {
            try
            {
                configuration = _viewModel.ToSettings().ToXElement(_rootElementName);
                errorMessage = null;
                return true;
            }
            catch (Exception ex)
            {
                configuration = null;
                errorMessage = $"{_toolName} configuration is invalid: {ex.Message}";
                return false;
            }
        }

        private static FrameworkElement CreateView(ToolFileSystemPermissionConfigurationViewModel viewModel)
        {
            var panel = new StackPanel
            {
                DataContext = viewModel
            };

            panel.Children.Add(new TextBlock
            {
                Text = LocalizationRuntime.Instance.GetString("ToolConfiguration.Permission", "Permission"),
                FontSize = 13,
                FontWeight = FontWeights.SemiBold,
                Margin = new Thickness(0, 0, 0, 8)
            });

            var comboBox = new ComboBox
            {
                MinWidth = 220,
                DisplayMemberPath = nameof(ToolFileSystemPermissionOption.DisplayName),
                Margin = new Thickness(0, 0, 0, 10)
            };
            comboBox.SetBinding(
                ItemsControl.ItemsSourceProperty,
                new Binding(nameof(ToolFileSystemPermissionConfigurationViewModel.PermissionOptions)));
            comboBox.SetBinding(
                ComboBox.SelectedItemProperty,
                new Binding(nameof(ToolFileSystemPermissionConfigurationViewModel.SelectedPermission))
                {
                    Mode = BindingMode.TwoWay,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                });
            panel.Children.Add(comboBox);

            var description = new TextBlock
            {
                TextWrapping = TextWrapping.Wrap,
                Foreground = Brushes.LightCyan,
                Margin = new Thickness(0, 0, 0, 10)
            };
            description.SetBinding(
                TextBlock.TextProperty,
                new Binding(nameof(ToolFileSystemPermissionConfigurationViewModel.PermissionDescription)));
            panel.Children.Add(description);

            var preview = new TextBlock
            {
                TextWrapping = TextWrapping.Wrap,
                Foreground = Brushes.White,
                Opacity = 0.72
            };
            preview.SetBinding(
                TextBlock.TextProperty,
                new Binding(nameof(ToolFileSystemPermissionConfigurationViewModel.PreviewText)));
            panel.Children.Add(preview);

            return panel;
        }
    }
}
