using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using Skyweaver.Rendering;
using Skyweaver.ViewModels;

namespace Skyweaver
{
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _viewModel;
        private int _oldSelectedIndex = 0;
        private Guid _currentTransitionToken;

        public MainWindow()
        {
            InitializeComponent();
            _viewModel = new MainViewModel();
            DataContext = _viewModel;
            WarmUpTilePageForStartup();
            Closing += MainWindow_Closing;
            KeyDown += MainWindow_KeyDown;
        }

        private void WarmUpTilePageForStartup()
        {
            try
            {
                DirectXResourcePreloader.PreloadAll();

                var tilePageView = MainPageContentHost.GetOrCreateCachedView(_viewModel.TilesPage);
                if (tilePageView is not UIElement tilePageElement)
                {
                    return;
                }

                PageWarmupHost.Content = tilePageView;
                PageWarmupHost.Visibility = Visibility.Visible;

                double warmupWidth = Math.Max(ActualWidth, 1280);
                double warmupHeight = Math.Max(MainPageContentHost.ActualHeight, 720);
                var warmupSize = new Size(warmupWidth, warmupHeight);

                PageWarmupHost.Width = warmupWidth;
                PageWarmupHost.Height = warmupHeight;
                tilePageElement.Measure(warmupSize);
                tilePageElement.Arrange(new Rect(warmupSize));
                tilePageElement.UpdateLayout();
                PageWarmupHost.Measure(warmupSize);
                PageWarmupHost.Arrange(new Rect(warmupSize));
                PageWarmupHost.UpdateLayout();
                RenderWarmupVisual(tilePageElement, warmupSize);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Tile page warmup failed: {ex}");
            }
            finally
            {
                PageWarmupHost.Content = null;
                PageWarmupHost.Visibility = Visibility.Collapsed;
            }
        }

        private static void RenderWarmupVisual(UIElement element, Size size)
        {
            if (size.Width <= 0 || size.Height <= 0)
            {
                return;
            }

            int pixelWidth = Math.Max(1, (int)Math.Ceiling(size.Width));
            int pixelHeight = Math.Max(1, (int)Math.Ceiling(size.Height));
            var bitmap = new RenderTargetBitmap(
                pixelWidth,
                pixelHeight,
                96,
                96,
                PixelFormats.Pbgra32);
            bitmap.Render(element);
            bitmap.Freeze();
        }

        private void MainWindow_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            // 按 Ctrl + Shift + C 快捷键直接弹出 Shell 聊天窗口
            if (e.Key == System.Windows.Input.Key.C && 
                (System.Windows.Input.Keyboard.Modifiers & (System.Windows.Input.ModifierKeys.Control | System.Windows.Input.ModifierKeys.Shift)) == (System.Windows.Input.ModifierKeys.Control | System.Windows.Input.ModifierKeys.Shift))
            {
                var shellWindow = new Skyweaver.Windows.ShellChatWindow();
                shellWindow.Owner = this;
                shellWindow.Show();
            }

            // 按 Ctrl + Shift + A 快捷键直接弹出 语音智能助手球 窗口进行设计演示和测试
            if (e.Key == System.Windows.Input.Key.A && 
                (System.Windows.Input.Keyboard.Modifiers & (System.Windows.Input.ModifierKeys.Control | System.Windows.Input.ModifierKeys.Shift)) == (System.Windows.Input.ModifierKeys.Control | System.Windows.Input.ModifierKeys.Shift))
            {
                var assistantWindow = new Skyweaver.Windows.AssistantBallWindow();
                assistantWindow.Owner = this;
                assistantWindow.Show();
            }
        }

        private void SessionListPanelView_Loaded()
        {

        }

        private void MainWindow_Closing(object? sender, CancelEventArgs e)
        {
            var app = Application.Current as App;
            if (app != null && app.IsShuttingDown)
            {
                return;
            }

            e.Cancel = true;
            Hide();

            _ = Task.Run(async () =>
            {
                try
                {
                    await _viewModel.HandleGuiClosingAsync();
                }
                catch
                {
                    // Protection against unhandled exceptions on background thread
                }
            });
        }

        private void RibbonTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source != RibbonTabControl)
                return;

            int newIndex = RibbonTabControl.SelectedIndex;
            int oldIndex = _oldSelectedIndex;
            _oldSelectedIndex = newIndex;

            // 如果是第一次加载，或者 index 没有变，或者 oldIndex 不合法，我们不播放动画
            if (oldIndex < 0 || oldIndex == newIndex || 
                oldIndex >= RibbonTabControl.Items.Count || 
                newIndex < 0 || newIndex >= RibbonTabControl.Items.Count)
            {
                return;
            }

            PlayRibbonTransitionAnimation(oldIndex, newIndex);
        }

        private void PlayRibbonTransitionAnimation(int oldIndex, int newIndex)
        {
            var oldTabItem = RibbonTabControl.Items[oldIndex] as TabItem;
            var newTabItem = RibbonTabControl.Items[newIndex] as TabItem;

            if (oldTabItem == null || newTabItem == null) return;

            var oldContent = oldTabItem.Content as FrameworkElement;
            if (oldContent == null) return;

            // 寻找模板中的命名元素
            var oldHost = RibbonTabControl.Template.FindName("OldContentVisualHost", RibbonTabControl) as Border;
            var newHost = RibbonTabControl.Template.FindName("PART_SelectedContentHost", RibbonTabControl) as FrameworkElement;

            if (oldHost == null || newHost == null) return;

            // 用唯一的 Token 解决多重/快速连续动画触发时的状态清理冲突问题
            var myToken = Guid.NewGuid();
            _currentTransitionToken = myToken;

            // 使用旧内容渲染 VisualBrush 并绑定到 host 上
            var visualBrush = new VisualBrush(oldContent)
            {
                Stretch = Stretch.Fill,
                AlignmentX = AlignmentX.Left,
                AlignmentY = AlignmentY.Top
            };
            oldHost.Background = visualBrush;
            oldHost.Width = newHost.ActualWidth;
            oldHost.Height = newHost.ActualHeight;
            oldHost.Visibility = Visibility.Visible;
            oldHost.Opacity = 1.0;

            // 判定切换方向
            bool isGoingRight = newIndex > oldIndex;
            double offset = 200.0;
            double slideOutDestination = isGoingRight ? -offset : offset;
            double slideInStart = isGoingRight ? offset : -offset;

            // 初始化变换
            var oldTransform = new TranslateTransform(0, 0);
            oldHost.RenderTransform = oldTransform;

            var newTransform = new TranslateTransform(slideInStart, 0);
            newHost.RenderTransform = newTransform;
            newHost.Opacity = 0.0;

            var duration = TimeSpan.FromSeconds(0.3);

            // --- 旧页面滑出 & 高速淡出动画 ---
            var oldOpacityAnimation = new DoubleAnimation
            {
                To = 0.0,
                Duration = TimeSpan.FromSeconds(0.18), // 中途高速变得透明
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn }
            };

            var oldSlideAnimation = new DoubleAnimation
            {
                To = slideOutDestination,
                Duration = duration,
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };

            // --- 新页面滑入 & 渐显动画 ---
            var newOpacityAnimation = new DoubleAnimation
            {
                To = 1.0,
                Duration = duration,
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };

            var newSlideAnimation = new DoubleAnimation
            {
                To = 0.0,
                Duration = duration,
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };

            // 清理事件处理
            oldSlideAnimation.Completed += (s, ev) =>
            {
                if (_currentTransitionToken == myToken)
                {
                    oldHost.Visibility = Visibility.Collapsed;
                    oldHost.Background = null;
                }
            };

            // 播放动画
            oldTransform.BeginAnimation(TranslateTransform.XProperty, oldSlideAnimation);
            oldHost.BeginAnimation(UIElement.OpacityProperty, oldOpacityAnimation);

            newTransform.BeginAnimation(TranslateTransform.XProperty, newSlideAnimation);
            newHost.BeginAnimation(UIElement.OpacityProperty, newOpacityAnimation);
        }
    }
}
