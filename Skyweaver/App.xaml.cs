using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using Skyweaver.Controls.SkyweaverPreferencesControl.Services;
using Skyweaver.Services.Localization;
using Skyweaver.Services.ShellIntegration;
using Skyweaver.Services.Skylifter;
using Skyweaver.Windows;

namespace Skyweaver
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var skyweaverExecutablePath = SkylifterLauncher.GetCurrentSkyweaverExecutablePath();
            if (SkylifterLauncher.IsDaemonOnlyStartup(e.Args))
            {
                SkylifterLauncher.EnsureStarted(skyweaverExecutablePath);
                Shutdown();
                return;
            }

            LocalizationRuntime.Instance.ApplyConfiguredLanguage();
            SkyweaverPreferencesRegistration.EnsureRegistered();
            Skyweaver.Services.Notifications.NotificationService.Instance.ClearTransient();

            var shellChatStartupContext = ShellIntegrationCommandLine.ParseShellChatStartup(e.Args);
            if (shellChatStartupContext != null)
            {
                SkylifterLauncher.EnsureStarted(skyweaverExecutablePath);
                _ = MonitorSkylifterLifecycleAsync();
                if (ShouldAggregateShellStartup(shellChatStartupContext))
                {
                    _ = ShowShellChatWindowAfterStartupAggregationAsync(shellChatStartupContext);
                }
                else
                {
                    ShowShellChatWindow(shellChatStartupContext);
                }

                return;
            }

            _ = ShellIntegrationRuntime.Instance.ApplyConfiguredRegistration();
            SkylifterLauncher.EnsureStarted(skyweaverExecutablePath);
            _ = SkylifterIpcClient.TryRegisterSkyweaverPathAsync(skyweaverExecutablePath);
            _ = MonitorSkylifterLifecycleAsync();

            var splashWindow = new SplashWindow();
            splashWindow.Show();

            MainWindow mainWindow = new MainWindow();
            MainWindow = mainWindow;

            bool mainWindowShown = false;

            Action showMainWindow = () =>
            {
                if (mainWindowShown)
                {
                    return;
                }

                Debug.WriteLine("Show main window for the first time.");
                mainWindowShown = true;

                Dispatcher.Invoke(() =>
                {
                    mainWindow.Show();
                    splashWindow.Close();
                });
            };

            System.Threading.Tasks.Task.Delay(500).ContinueWith(_ => showMainWindow());
        }

        private static void ShowShellChatWindow(ShellChatStartupContext startupContext)
        {
            var shellWindow = new ShellChatWindow(startupContext);
            Current.MainWindow = shellWindow;
            shellWindow.Show();
            shellWindow.Activate();
        }

        /// <summary>
        /// 后台监控 Skylifter 进程生命周期。若 Skylifter 被关闭，则当前应用的所有窗口也随之关闭并退出。
        /// </summary>
        private async Task MonitorSkylifterLifecycleAsync()
        {
            // 给 Skylifter 启动预留 2 秒的缓冲时间
            await Task.Delay(2000).ConfigureAwait(false);

            while (true)
            {
                await Task.Delay(1000).ConfigureAwait(false);

                try
                {
                    var isRunning = Process.GetProcessesByName("Skylifter")
                        .Any(p =>
                        {
                            try
                            {
                                return !p.HasExited;
                            }
                            catch
                            {
                                return false;
                            }
                        });

                    if (!isRunning)
                    {
                        // 发现 Skylifter 已退出，在 UI 线程上关闭当前应用的所有窗口并关闭整个程序
                        Current.Dispatcher.Invoke(() =>
                        {
                            try
                            {
                                Current.Shutdown();
                            }
                            catch
                            {
                                Environment.Exit(0);
                            }
                        });
                        break;
                    }
                }
                catch
                {
                    // 忽略异常，继续轮询监测
                }
            }
        }

        private static bool ShouldAggregateShellStartup(ShellChatStartupContext startupContext)
        {
            return startupContext.SelectedPaths.Any(path =>
                !string.IsNullOrWhiteSpace(path) &&
                !Directory.Exists(path.Trim()));
        }

        private async Task ShowShellChatWindowAfterStartupAggregationAsync(
            ShellChatStartupContext startupContext)
        {
            var aggregatedContext = await ShellChatStartupCoordinator
                .TryAggregateOrForwardAsync(startupContext)
                .ConfigureAwait(true);
            if (aggregatedContext == null)
            {
                Shutdown();
                return;
            }

            ShowShellChatWindow(aggregatedContext);
        }
    }
}
