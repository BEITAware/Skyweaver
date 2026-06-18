using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Threading;

namespace Ferrita.Windows
{
    public partial class AssistantBallWindow : Window
    {
        private readonly DispatcherTimer _volumeTimer;
        private readonly DispatcherTimer _countdownTimer;
        private int _secondsRemaining = 5;
        private bool _isListening = false;
        private readonly Random _random = new Random();

        // 颜色定义
        private static readonly Color IdleShadowColor = Colors.Black;
        private static readonly Color ListeningOrangeColor = Color.FromRgb(255, 127, 0);
        private static readonly Color IdleGlowColor = Color.FromRgb(0, 168, 255);

        private Storyboard? _breathStoryboard;

        public AssistantBallWindow()
        {
            InitializeComponent();

            // 1. 初始化模拟音量波动的计时器
            _volumeTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(70)
            };
            _volumeTimer.Tick += VolumeTimer_Tick;

            // 2. 初始化5s倒计时计时器
            _countdownTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _countdownTimer.Tick += CountdownTimer_Tick;

            Loaded += AssistantBallWindow_Loaded;
        }

        private void AssistantBallWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // 初始化呼吸动画
            InitializeBreathAnimation();
            
            // 默认闲置状态，音量条设为最低
            VolumeFill.Height = 8;
        }

        private void InitializeBreathAnimation()
        {
            // 创建正在聆听时的球体呼吸光环动画
            _breathStoryboard = new Storyboard();

            // 光环大小变化
            var scaleXAnim = new DoubleAnimation
            {
                From = 0.96,
                To = 1.05,
                Duration = TimeSpan.FromSeconds(1.2),
                AutoReverse = true,
                RepeatBehavior = RepeatBehavior.Forever
            };
            Storyboard.SetTarget(scaleXAnim, ListeningBreathRing);
            Storyboard.SetTargetProperty(scaleXAnim, new PropertyPath("RenderTransform.ScaleX"));

            var scaleYAnim = new DoubleAnimation
            {
                From = 0.96,
                To = 1.05,
                Duration = TimeSpan.FromSeconds(1.2),
                AutoReverse = true,
                RepeatBehavior = RepeatBehavior.Forever
            };
            Storyboard.SetTarget(scaleYAnim, ListeningBreathRing);
            Storyboard.SetTargetProperty(scaleYAnim, new PropertyPath("RenderTransform.ScaleY"));

            // 光环透明度变化
            var opacityAnim = new DoubleAnimation
            {
                From = 0.2,
                To = 0.85,
                Duration = TimeSpan.FromSeconds(1.2),
                AutoReverse = true,
                RepeatBehavior = RepeatBehavior.Forever
            };
            Storyboard.SetTarget(opacityAnim, ListeningBreathRing);
            Storyboard.SetTargetProperty(opacityAnim, new PropertyPath(UIElement.OpacityProperty));

            // 给 Ring 设置 TransformOrigin
            ListeningBreathRing.RenderTransformOrigin = new Point(0.5, 0.5);
            ListeningBreathRing.RenderTransform = new ScaleTransform(1, 1);

            _breathStoryboard.Children.Add(scaleXAnim);
            _breathStoryboard.Children.Add(scaleYAnim);
            _breathStoryboard.Children.Add(opacityAnim);
        }

        private void BallButton_Click(object sender, RoutedEventArgs e)
        {
            if (!_isListening)
            {
                StartListening();
            }
            else
            {
                StopListening(isInterrupted: true);
            }
        }

        private void StartListening()
        {
            _isListening = true;
            _secondsRemaining = 5;

            // 1. 改变文字内容与可见性
            StateValueText.Text = "正在聆听...";
            StateTimerText.Text = $"等待 {_secondsRemaining}s 后停止";
            StateTimerText.Visibility = Visibility.Visible;
            ListeningGlowOverlay.Visibility = Visibility.Visible;

            // 2. 状态框、图标、球体的炫酷颜色和光效过渡动画
            var duration = TimeSpan.FromSeconds(0.35);
            var ease = new CubicEase { EasingMode = EasingMode.EaseOut };

            // 状态框内阴影 (黑色 -> 橘黄色)
            var shadowColorAnim = new ColorAnimation(IdleShadowColor, ListeningOrangeColor, duration) { EasingFunction = ease };
            InnerShadowBrush.BeginAnimation(SolidColorBrush.ColorProperty, shadowColorAnim);
            InnerShadowEffect.BeginAnimation(DropShadowEffect.ColorProperty, shadowColorAnim);

            // 状态文字发光 (蓝色 -> 橘黄色)
            var textGlowColorAnim = new ColorAnimation(IdleGlowColor, ListeningOrangeColor, duration) { EasingFunction = ease };
            StateTextGlow.BeginAnimation(DropShadowEffect.ColorProperty, textGlowColorAnim);

            // 麦克风图标发光 (蓝色 -> 橘黄色)
            MicGlow.BeginAnimation(DropShadowEffect.ColorProperty, textGlowColorAnim);

            // 球体底部彩虹光斑 (淡蓝色 -> 橘黄色)
            var ballGlowAnim = new ColorAnimation(Color.FromRgb(0x41, 0xB8, 0xFF), ListeningOrangeColor, duration) { EasingFunction = ease };
            BlueGlowStop.BeginAnimation(GradientStop.ColorProperty, ballGlowAnim);

            // 状态框发光层淡入
            var glowOverlayOpacityAnim = new DoubleAnimation(0, 0.9, duration) { EasingFunction = ease };
            ListeningGlowOverlay.BeginAnimation(UIElement.OpacityProperty, glowOverlayOpacityAnim);

            // 3. 启动球体呼吸圈动画
            _breathStoryboard?.Begin();

            // 4. 启动模拟音量计时器和倒计时计时器
            _volumeTimer.Start();
            _countdownTimer.Start();
        }

        private void StopListening(bool isInterrupted = false)
        {
            _isListening = false;
            _volumeTimer.Stop();
            _countdownTimer.Stop();
            _breathStoryboard?.Stop();

            // 1. 恢复文字
            StateValueText.Text = "闲置";
            StateTimerText.Visibility = Visibility.Collapsed;

            var duration = TimeSpan.FromSeconds(0.4);
            var ease = new CubicEase { EasingMode = EasingMode.EaseOut };

            // 2. 还原所有颜色和发光
            // 状态框内阴影还原为黑色
            var shadowColorAnim = new ColorAnimation(ListeningOrangeColor, IdleShadowColor, duration) { EasingFunction = ease };
            InnerShadowBrush.BeginAnimation(SolidColorBrush.ColorProperty, shadowColorAnim);
            InnerShadowEffect.BeginAnimation(DropShadowEffect.ColorProperty, shadowColorAnim);

            // 状态文字发光还原为蓝色
            var textGlowColorAnim = new ColorAnimation(ListeningOrangeColor, IdleGlowColor, duration) { EasingFunction = ease };
            StateTextGlow.BeginAnimation(DropShadowEffect.ColorProperty, textGlowColorAnim);

            // 麦克风发光还原为蓝色
            MicGlow.BeginAnimation(DropShadowEffect.ColorProperty, textGlowColorAnim);

            // 球体底部彩虹光斑还原为淡蓝色
            var ballGlowAnim = new ColorAnimation(ListeningOrangeColor, Color.FromRgb(0x41, 0xB8, 0xFF), duration) { EasingFunction = ease };
            BlueGlowStop.BeginAnimation(GradientStop.ColorProperty, ballGlowAnim);

            // 状态框发光层淡出
            var glowOverlayOpacityAnim = new DoubleAnimation(0, duration) { EasingFunction = ease };
            glowOverlayOpacityAnim.Completed += (s, e) => {
                if (!_isListening) ListeningGlowOverlay.Visibility = Visibility.Collapsed;
            };
            ListeningGlowOverlay.BeginAnimation(UIElement.OpacityProperty, glowOverlayOpacityAnim);

            // 呼吸光圈还原/淡出
            var ringOpacityAnim = new DoubleAnimation(0, duration) { EasingFunction = ease };
            ListeningBreathRing.BeginAnimation(UIElement.OpacityProperty, ringOpacityAnim);

            // 3. 将音量条高度平滑过渡回最低高度 (8px)
            var volumeHeightAnim = new DoubleAnimation(VolumeFill.Height, 8, TimeSpan.FromSeconds(0.3))
            {
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn }
            };
            VolumeFill.BeginAnimation(HeightProperty, volumeHeightAnim);
        }

        private void VolumeTimer_Tick(object? sender, EventArgs e)
        {
            // 正在聆听时模拟随机起伏的音量电平。音量条最高高度为 195
            double targetHeight = _random.Next(20, 185);

            var anim = new DoubleAnimation
            {
                To = targetHeight,
                Duration = TimeSpan.FromMilliseconds(70),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };
            VolumeFill.BeginAnimation(HeightProperty, anim);
        }

        private void CountdownTimer_Tick(object? sender, EventArgs e)
        {
            _secondsRemaining--;

            if (_secondsRemaining <= 0)
            {
                StopListening();
            }
            else
            {
                StateTimerText.Text = $"等待 {_secondsRemaining}s 后停止";
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // 拖动整个窗口
            if (e.ButtonState == MouseButtonState.Pressed && 
                e.OriginalSource is not Button && 
                e.OriginalSource is not System.Windows.Shapes.Path)
            {
                DragMove();
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            _volumeTimer.Stop();
            _countdownTimer.Stop();
            Close();
        }

        // ================= BALL INTERACTIVE EFFECTS (球体微交互特效) =================
        private void BallButton_MouseEnter(object sender, MouseEventArgs e)
        {
            // 鼠标移入：微放大 1.03 倍，平滑过渡
            var anim = new DoubleAnimation(1.03, TimeSpan.FromSeconds(0.18))
            {
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };
            BallScale.BeginAnimation(ScaleTransform.ScaleXProperty, anim);
            BallScale.BeginAnimation(ScaleTransform.ScaleYProperty, anim);
        }

        private void BallButton_MouseLeave(object sender, MouseEventArgs e)
        {
            // 鼠标移出：还原 1.0 倍
            var anim = new DoubleAnimation(1.0, TimeSpan.FromSeconds(0.18))
            {
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };
            BallScale.BeginAnimation(ScaleTransform.ScaleXProperty, anim);
            BallScale.BeginAnimation(ScaleTransform.ScaleYProperty, anim);
        }

        private void BallButton_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // 按下：微缩 0.96 倍，点击反馈
            var anim = new DoubleAnimation(0.96, TimeSpan.FromSeconds(0.08))
            {
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn }
            };
            BallScale.BeginAnimation(ScaleTransform.ScaleXProperty, anim);
            BallScale.BeginAnimation(ScaleTransform.ScaleYProperty, anim);
        }

        private void BallButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // 松开：恢复 Hover 大小 1.03 倍
            var anim = new DoubleAnimation(1.03, TimeSpan.FromSeconds(0.12))
            {
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };
            BallScale.BeginAnimation(ScaleTransform.ScaleXProperty, anim);
            BallScale.BeginAnimation(ScaleTransform.ScaleYProperty, anim);
        }
    }
}
