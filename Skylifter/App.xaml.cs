using System.Threading;
using Skyweaver.Services.Localization;
using WpfApplication = System.Windows.Application;

namespace Skylifter
{
    public partial class App : WpfApplication
    {
        private Mutex? _singleInstanceMutex;
        private SkylifterDaemon? _daemon;

        private void Application_Startup(object sender, System.Windows.StartupEventArgs e)
        {
            _singleInstanceMutex = new Mutex(true, "Skyweaver.Skylifter.SingleInstance", out var createdNew);
            if (!createdNew)
            {
                Shutdown();
                return;
            }

            LocalizationRuntime.Instance.ApplyConfiguredLanguage();

            _daemon = new SkylifterDaemon(e.Args);
            _daemon.Start();
        }

        protected override void OnExit(System.Windows.ExitEventArgs e)
        {
            _daemon?.Dispose();
            _daemon = null;

            try
            {
                _singleInstanceMutex?.ReleaseMutex();
            }
            catch
            {
                // The mutex can already be released during abnormal startup shutdown.
            }

            _singleInstanceMutex?.Dispose();
            base.OnExit(e);
        }
    }
}
