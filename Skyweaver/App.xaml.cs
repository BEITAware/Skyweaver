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
