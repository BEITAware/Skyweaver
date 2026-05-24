using System.Diagnostics;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Forms = System.Windows.Forms;
using Drawing = System.Drawing;
using Application = System.Windows.Application;
using WpfBrushes = System.Windows.Media.Brushes;
using WpfImage = System.Windows.Controls.Image;
using WpfOrientation = System.Windows.Controls.Orientation;

namespace Skylifter
{
    internal sealed class SkylifterDaemon : IDisposable
    {
        private const int ShowWindowRestore = 9;

        private readonly SkylifterMemoryQueue _memoryQueue = new();
        private readonly SkylifterIpcServer _ipcServer;
        private readonly object _shutdownGate = new();
        private Forms.NotifyIcon? _notifyIcon;
        private Drawing.Icon? _trayIcon;
        private ContextMenu? _trayContextMenu;
        private string? _skyweaverExecutablePath;
        private Task? _shutdownTask;
        private bool _isDisposed;

        public SkylifterDaemon(IReadOnlyList<string> args)
        {
            _skyweaverExecutablePath = ParseSkyweaverExecutablePath(args);
            _ipcServer = new SkylifterIpcServer(this, _memoryQueue);
        }

        public void Start()
        {
            _memoryQueue.Start();
            _ipcServer.Start();
            InitializeTrayIcon();
        }

        public void RegisterSkyweaverExecutablePath(string? path)
        {
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
            {
                return;
            }

            _skyweaverExecutablePath = Path.GetFullPath(path);
        }

        public void OpenOrFocusSkyweaver()
        {
            if (TryActivateExistingSkyweaverWindow())
            {
                return;
            }

            var executablePath = LocateSkyweaverExecutablePath();
            if (string.IsNullOrWhiteSpace(executablePath) || !File.Exists(executablePath))
            {
                return;
            }

            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = executablePath,
                    WorkingDirectory = Path.GetDirectoryName(executablePath) ?? AppContext.BaseDirectory,
                    UseShellExecute = true
                });
            }
            catch
            {
                // The tray stays alive even if the GUI executable cannot be launched.
            }
        }

        public void ShutdownApplication()
        {
            lock (_shutdownGate)
            {
                if (_shutdownTask != null)
                {
                    return;
                }

                _shutdownTask = Task.CompletedTask;
            }

            ForceShutdownApplication();
        }

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _isDisposed = true;
            _trayContextMenu?.SetCurrentValue(ContextMenu.IsOpenProperty, false);
            if (_notifyIcon != null)
            {
                _notifyIcon.Visible = false;
            }

            _notifyIcon?.Dispose();
            _trayIcon?.Dispose();
            _ipcServer.Dispose();
            _memoryQueue.Dispose();
        }

        private void InitializeTrayIcon()
        {
            _trayIcon = LoadTrayIcon();
            _notifyIcon = new Forms.NotifyIcon
            {
                Icon = _trayIcon,
                Text = "Skylifter - Skyweaver Daemon",
                Visible = true
            };

            _notifyIcon.MouseUp += OnNotifyIconMouseUp;
        }

        private void OnNotifyIconMouseUp(object? sender, Forms.MouseEventArgs e)
        {
            if (e.Button == Forms.MouseButtons.Left)
            {
                OpenOrFocusSkyweaver();
                return;
            }

            if (e.Button == Forms.MouseButtons.Right)
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(ShowTrayMenu));
            }
        }

        private void ShowTrayMenu()
        {
            _trayContextMenu ??= BuildTrayMenu();
            _trayContextMenu.IsOpen = false;
            _trayContextMenu.Placement = PlacementMode.MousePoint;
            _trayContextMenu.IsOpen = true;
        }

        private ContextMenu BuildTrayMenu()
        {
            var menu = new ContextMenu
            {
                Placement = PlacementMode.MousePoint,
                StaysOpen = false
            };

            if (Application.Current.TryFindResource("AnimatedContextMenuStyle") is Style menuStyle)
            {
                menu.Style = menuStyle;
            }

            menu.Items.Add(CreateMenuItem(
                "打开 Skyweaver",
                "打开或聚焦主窗口",
                "pack://application:,,,/Skyweaver;component/Resources/SkyweaverAppLogo.png",
                (_, _) =>
                {
                    menu.IsOpen = false;
                    OpenOrFocusSkyweaver();
                }));

            menu.Items.Add(CreateMenuItem(
                "关闭应用程序",
                "关闭 Skyweaver 与 Skylifter",
                "pack://application:,,,/Skyweaver;component/Resources/CrossMark.png",
                (_, _) =>
                {
                    menu.IsOpen = false;
                    ShutdownApplication();
                }));

            return menu;
        }

        private static MenuItem CreateMenuItem(
            string title,
            string description,
            string iconUri,
            RoutedEventHandler clickHandler)
        {
            var item = new MenuItem
            {
                Header = CreateMenuHeader(title, description, iconUri)
            };

            if (Application.Current.TryFindResource("AnimatedMenuItemStyle") is Style itemStyle)
            {
                item.Style = itemStyle;
            }

            item.Click += clickHandler;
            return item;
        }

        private static Grid CreateMenuHeader(string title, string description, string iconUri)
        {
            var grid = new Grid
            {
                Width = 248,
                Margin = new Thickness(0, 2, 0, 2)
            };

            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(36) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            var icon = new WpfImage
            {
                Width = 24,
                Height = 24,
                Margin = new Thickness(2, 0, 8, 0),
                Stretch = Stretch.Uniform,
                Source = new BitmapImage(new Uri(iconUri, UriKind.Absolute))
            };
            Grid.SetColumn(icon, 0);
            grid.Children.Add(icon);

            var textStack = new StackPanel
            {
                Orientation = WpfOrientation.Vertical,
                VerticalAlignment = VerticalAlignment.Center
            };
            Grid.SetColumn(textStack, 1);
            grid.Children.Add(textStack);

            textStack.Children.Add(new TextBlock
            {
                Text = title,
                FontSize = 13,
                FontWeight = FontWeights.SemiBold,
                Foreground = WpfBrushes.White
            });
            textStack.Children.Add(new TextBlock
            {
                Text = description,
                FontSize = 11,
                Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(210, 255, 255, 255)),
                TextWrapping = TextWrapping.NoWrap,
                TextTrimming = TextTrimming.CharacterEllipsis
            });

            return grid;
        }

        private static Drawing.Icon LoadTrayIcon()
        {
            try
            {
                var resource = Application.GetResourceStream(
                    new Uri("pack://application:,,,/Skyweaver;component/Resources/Skyweaver.ico", UriKind.Absolute));
                if (resource?.Stream != null)
                {
                    using var stream = resource.Stream;
                    return new Drawing.Icon(stream);
                }
            }
            catch
            {
                // Fall back to the default application icon below.
            }

            return Drawing.SystemIcons.Application;
        }

        private bool TryActivateExistingSkyweaverWindow()
        {
            var foundSkyweaverProcess = false;
            foreach (var process in EnumerateSkyweaverProcesses())
            {
                foundSkyweaverProcess = true;
                var windowHandle = IntPtr.Zero;

                try
                {
                    windowHandle = process.MainWindowHandle;
                }
                catch
                {
                    // Ignore processes that disappear while enumerating.
                }

                if (windowHandle == IntPtr.Zero)
                {
                    continue;
                }

                ShowWindow(windowHandle, ShowWindowRestore);
                SetForegroundWindow(windowHandle);
                return true;
            }

            return foundSkyweaverProcess;
        }

        private static IEnumerable<Process> EnumerateSkyweaverProcesses()
        {
            try
            {
                return Process.GetProcessesByName("Skyweaver")
                    .Where(process =>
                    {
                        try
                        {
                            return !process.HasExited;
                        }
                        catch
                        {
                            return false;
                        }
                    })
                    .ToArray();
            }
            catch
            {
                return Array.Empty<Process>();
            }
        }

        private void ForceShutdownApplication()
        {
            try
            {
                HideTraySurface();
                _memoryQueue.BeginShutdown();
                
                // 优先杀掉 Skyweaver 的所有进程，确保所有主窗口和 Shell Chat 窗口等全部被关闭
                ForceTerminateProcesses(EnumerateSkyweaverProcesses());

                // 作为额外保障，使用 taskkill 强杀 Skyweaver.exe
                StartTaskKillForProcessName("Skyweaver.exe");
            }
            catch
            {
                // 即使 Skyweaver 清理失败，当前的 Skylifter 进程也必须终止
            }

            // 最后安全退出当前 Skylifter 进程自身
            TerminateCurrentProcess();
        }

        private void HideTraySurface()
        {
            try
            {
                _trayContextMenu?.SetCurrentValue(ContextMenu.IsOpenProperty, false);
                if (_notifyIcon != null)
                {
                    _notifyIcon.Visible = false;
                }
            }
            catch
            {
                // Tray cleanup is best effort; hard process termination follows immediately.
            }
        }

        private static void ForceTerminateProcesses(IEnumerable<Process> processes)
        {
            foreach (var process in processes)
            {
                try
                {
                    if (!process.HasExited)
                    {
                        process.Kill(entireProcessTree: true);
                    }
                }
                catch
                {
                    // Continue with the current Skylifter process termination.
                }
                finally
                {
                    process.Dispose();
                }
            }
        }

        private static void TerminateCurrentProcess()
        {
            try
            {
                using var currentProcess = Process.GetCurrentProcess();
                currentProcess.Kill(entireProcessTree: true);
            }
            catch
            {
                Environment.Exit(0);
            }
        }

        private static void StartTaskKillForProcessName(string imageName)
        {
            if (string.IsNullOrWhiteSpace(imageName))
            {
                return;
            }

            StartTaskKill($"/IM {QuoteTaskKillArgument(imageName)} /T /F");
        }

        private static void StartTaskKillForProcessId(int processId)
        {
            if (processId <= 0)
            {
                return;
            }

            StartTaskKill($"/PID {processId} /T /F");
        }

        private static void StartTaskKill(string arguments)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.System),
                        "taskkill.exe"),
                    Arguments = arguments,
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    WindowStyle = ProcessWindowStyle.Hidden
                });
            }
            catch
            {
                // The in-process kill path below is still attempted.
            }
        }

        private static string QuoteTaskKillArgument(string value)
        {
            var builder = new StringBuilder(value.Length + 2);
            builder.Append('"');
            foreach (var ch in value)
            {
                if (ch == '"')
                {
                    builder.Append('\\');
                }

                builder.Append(ch);
            }

            builder.Append('"');
            return builder.ToString();
        }

        private string? LocateSkyweaverExecutablePath()
        {
            if (!string.IsNullOrWhiteSpace(_skyweaverExecutablePath) && File.Exists(_skyweaverExecutablePath))
            {
                return _skyweaverExecutablePath;
            }

            foreach (var candidate in BuildSkyweaverCandidates(Path.GetFullPath(AppContext.BaseDirectory)))
            {
                if (File.Exists(candidate))
                {
                    _skyweaverExecutablePath = candidate;
                    return candidate;
                }
            }

            return null;
        }

        private static IEnumerable<string> BuildSkyweaverCandidates(string baseDirectory)
        {
            yield return Path.Combine(baseDirectory, "Skyweaver.exe");

            var netDirectory = Directory.GetParent(baseDirectory);
            var configurationDirectory = netDirectory?.Parent;
            var binDirectory = configurationDirectory?.Parent;
            var skylifterProjectDirectory = binDirectory?.Parent;
            var solutionDirectory = skylifterProjectDirectory?.Parent;

            if (configurationDirectory == null || solutionDirectory == null)
            {
                yield break;
            }

            var configurationName = configurationDirectory.Name;
            yield return Path.Combine(
                solutionDirectory.FullName,
                "Skyweaver",
                "bin",
                configurationName,
                "net8.0-windows",
                "Skyweaver.exe");

            yield return Path.Combine(
                solutionDirectory.FullName,
                "Skyweaver",
                "bin",
                "Debug",
                "net8.0-windows",
                "Skyweaver.exe");

            yield return Path.Combine(
                solutionDirectory.FullName,
                "Skyweaver",
                "bin",
                "Release",
                "net8.0-windows",
                "Skyweaver.exe");
        }

        private static string? ParseSkyweaverExecutablePath(IReadOnlyList<string> args)
        {
            for (var index = 0; index < args.Count; index++)
            {
                var arg = args[index];
                const string key = "--skyweaver-exe";
                if (string.Equals(arg, key, StringComparison.OrdinalIgnoreCase) &&
                    index + 1 < args.Count)
                {
                    return args[index + 1];
                }

                if (arg.StartsWith(key + "=", StringComparison.OrdinalIgnoreCase))
                {
                    return arg[(key.Length + 1)..].Trim('"');
                }
            }

            return null;
        }

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
    }
}
