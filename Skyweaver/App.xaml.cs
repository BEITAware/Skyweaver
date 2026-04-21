using System;
using System.Diagnostics;
using System.Windows;
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
            // 显示启动窗口
            var splashWindow = new SplashWindow();
            splashWindow.Show();

            // 加载主窗口（不自动显示）
            MainWindow mainWindow = new MainWindow();

            // 重要：设置为应用程序的主窗口，这样Application.Current.MainWindow就不会为null
            this.MainWindow = mainWindow;

            // 设置关闭启动窗口的标志和方法
            bool mainWindowShown = false;

            // 创建显示主窗口的方法（确保只执行一次）
            Action showMainWindow = () =>
            {
                if (!mainWindowShown)
                {
                    Debug.WriteLine("显示主窗口 - 首次调用");
                    mainWindowShown = true;

                    // 在UI线程上显示主窗口并关闭启动窗口
                    this.Dispatcher.Invoke(() =>
                    {
                        mainWindow.Show();
                        splashWindow.Close();
                    });
                }
            };

            // 模拟后台加载，完成后显示主窗口
            System.Threading.Tasks.Task.Delay(500).ContinueWith(t => showMainWindow());
        }
    }
}
