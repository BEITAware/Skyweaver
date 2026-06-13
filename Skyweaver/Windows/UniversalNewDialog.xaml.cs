using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using Skyweaver.Infrastructure.Mvvm;

namespace Skyweaver.Windows
{
    public partial class UniversalNewDialog : Window
    {
        public static readonly DependencyProperty MainTitleProperty =
            DependencyProperty.Register(nameof(MainTitle), typeof(string), typeof(UniversalNewDialog), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty MainDescriptionProperty =
            DependencyProperty.Register(nameof(MainDescription), typeof(string), typeof(UniversalNewDialog), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty MainIconSourceProperty =
            DependencyProperty.Register(nameof(MainIconSource), typeof(ImageSource), typeof(UniversalNewDialog), new PropertyMetadata(null));

        public static readonly DependencyProperty MainHintProperty =
            DependencyProperty.Register(nameof(MainHint), typeof(string), typeof(UniversalNewDialog), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty SettingPanelTitleProperty =
            DependencyProperty.Register(nameof(SettingPanelTitle), typeof(string), typeof(UniversalNewDialog), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty SettingPanelDescriptionProperty =
            DependencyProperty.Register(
                nameof(SettingPanelDescription),
                typeof(string),
                typeof(UniversalNewDialog),
                new PropertyMetadata(string.Empty, OnSettingPanelDescriptionChanged));

        public static readonly DependencyProperty HasSettingPanelDescriptionProperty =
            DependencyProperty.Register(nameof(HasSettingPanelDescription), typeof(bool), typeof(UniversalNewDialog), new PropertyMetadata(false));

        public static readonly DependencyProperty CurrentAngleProperty =
            DependencyProperty.RegisterAttached(
                "CurrentAngle",
                typeof(double),
                typeof(UniversalNewDialog),
                new PropertyMetadata(0d, OnCurrentAngleChanged));

        public static double GetCurrentAngle(DependencyObject obj) => (double)obj.GetValue(CurrentAngleProperty);
        public static void SetCurrentAngle(DependencyObject obj, double value) => obj.SetValue(CurrentAngleProperty, value);

        private static void OnCurrentAngleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ContentPresenter presenter)
            {
                var angle = (double)e.NewValue;
                var radians = angle * Math.PI / 180d;
                var sphereCenterX = RingCenterX + (Math.Cos(radians) * RingRadius);
                var sphereCenterY = RingCenterY + (Math.Sin(radians) * RingRadius);
                var left = sphereCenterX - OptionSphereCenterX;
                var top = sphereCenterY - OptionSphereCenterY;

                Canvas.SetLeft(presenter, left);
                Canvas.SetTop(presenter, top);
            }
        }

        private const double RingCenterX = 270;
        private const double RingCenterY = 310;
        private const double RingRadius = 248;
        private const double RingStartAngle = -68;
        private const double RingSweepAngle = 136;
        private const double OptionSphereCenterX = 54;
        private const double OptionSphereCenterY = 62;

        private readonly Duration _ringAnimationDuration = new(TimeSpan.FromMilliseconds(360));
        private readonly IEasingFunction _ringEasing = new CubicEase { EasingMode = EasingMode.EaseOut };
        private bool _allowClose;
        private bool _isCloseAnimationRunning;
        private bool? _pendingDialogResult;
        private double _ringOffset;
        private UniversalNewDialogOption? _expandedOption;
        private bool _isScrollingDown;

        public UniversalNewDialog()
        {
            InitializeComponent();
            SideItems.CollectionChanged += OnSideItemsChanged;
            OptionsItemsControl.ItemContainerGenerator.StatusChanged += OnItemContainerGeneratorStatusChanged;
            Loaded += OnLoaded;

            AddTriggerOption(
                "关闭",
                "关闭这一对话框",
                "/Skyweaver;component/Resources/CrossMark.png",
                _ =>
                {
                    BeginClose(false);
                    return true;
                });
        }

        public ObservableCollection<UniversalNewDialogOption> SideItems { get; } = new();

        public string MainTitle
        {
            get => (string)GetValue(MainTitleProperty);
            set => SetValue(MainTitleProperty, value ?? string.Empty);
        }

        public string MainDescription
        {
            get => (string)GetValue(MainDescriptionProperty);
            set => SetValue(MainDescriptionProperty, value ?? string.Empty);
        }

        public ImageSource? MainIconSource
        {
            get => (ImageSource?)GetValue(MainIconSourceProperty);
            set => SetValue(MainIconSourceProperty, value);
        }

        public string MainHint
        {
            get => (string)GetValue(MainHintProperty);
            private set => SetValue(MainHintProperty, value ?? string.Empty);
        }

        public string SettingPanelTitle
        {
            get => (string)GetValue(SettingPanelTitleProperty);
            private set => SetValue(SettingPanelTitleProperty, value ?? string.Empty);
        }

        public string SettingPanelDescription
        {
            get => (string)GetValue(SettingPanelDescriptionProperty);
            private set => SetValue(SettingPanelDescriptionProperty, value ?? string.Empty);
        }

        public bool HasSettingPanelDescription
        {
            get => (bool)GetValue(HasSettingPanelDescriptionProperty);
            private set => SetValue(HasSettingPanelDescriptionProperty, value);
        }

        public UniversalNewDialogOption AddTriggerOption(
            string title,
            string description,
            string iconUri,
            Func<UniversalNewDialogOption, bool> execute)
        {
            var option = new UniversalNewDialogOption(UniversalNewDialogOptionKind.Trigger)
            {
                Title = title,
                DescriptionText = description,
                IconSource = CreateImageSource(iconUri),
                Execute = execute
            };

            SideItems.Add(option);
            return option;
        }

        public UniversalNewDialogOption AddSettingOption(
            string title,
            string description,
            string iconUri,
            FrameworkElement settingContent,
            Action<UniversalNewDialogOption>? opened = null)
        {
            var option = new UniversalNewDialogOption(UniversalNewDialogOptionKind.Setting)
            {
                Title = title,
                DescriptionText = description,
                IconSource = CreateImageSource(iconUri),
                SettingContent = settingContent,
                SettingOpened = opened
            };

            SideItems.Add(option);
            return option;
        }

        public void SetMainIcon(string iconUri)
        {
            MainIconSource = CreateImageSource(iconUri);
        }

        public void ShowMainHint(string message)
        {
            MainHint = message;
            MainHintHost.BeginAnimation(OpacityProperty, null);
            MainHintHost.Opacity = 0;
            MainHintHost.BeginAnimation(
                OpacityProperty,
                new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(170))
                {
                    EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
                });
        }

        public void ClearMainHint()
        {
            MainHint = string.Empty;
            MainHintHost.BeginAnimation(
                OpacityProperty,
                new DoubleAnimation(MainHintHost.Opacity, 0, TimeSpan.FromMilliseconds(150)));
        }

        public void HighlightOption(UniversalNewDialogOption? option)
        {
            foreach (var item in SideItems)
            {
                item.IsHighlighted = ReferenceEquals(item, option);
            }
        }

        public void CloseWithResult(bool dialogResult)
        {
            BeginClose(dialogResult);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (!_allowClose && IsLoaded)
            {
                e.Cancel = true;
                if (!_isCloseAnimationRunning)
                {
                    BeginClose(_pendingDialogResult);
                }

                return;
            }

            base.OnClosing(e);
        }

        private static void OnSettingPanelDescriptionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is UniversalNewDialog dialog)
            {
                dialog.HasSettingPanelDescription = !string.IsNullOrWhiteSpace(e.NewValue as string);
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            PlayOpenAnimation();
            ApplyRingLayout(false);
        }

        private void OnSideItemsChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (UniversalNewDialogOption option in e.OldItems)
                {
                    option.PropertyChanged -= OnOptionPropertyChanged;
                }
            }

            if (e.NewItems != null)
            {
                foreach (UniversalNewDialogOption option in e.NewItems)
                {
                    option.PropertyChanged += OnOptionPropertyChanged;
                }
            }

            Dispatcher.BeginInvoke(() => ApplyRingLayout(false));
        }

        private void OnOptionPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(UniversalNewDialogOption.IsExpanded))
            {
                ApplyRingLayout(false);
            }
            else if (e.PropertyName == nameof(UniversalNewDialogOption.IsHovered))
            {
                RefreshOptionZIndexes();
            }
        }

        private void OnItemContainerGeneratorStatusChanged(object? sender, EventArgs e)
        {
            if (OptionsItemsControl.ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
            {
                ApplyRingLayout(false);
            }
        }

        private void PlayOpenAnimation()
        {
            RootShell.BeginAnimation(OpacityProperty, new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(220))
            {
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            });
            RootScale.BeginAnimation(ScaleTransform.ScaleXProperty, new DoubleAnimation(0.96, 1, TimeSpan.FromMilliseconds(260))
            {
                EasingFunction = new BackEase { EasingMode = EasingMode.EaseOut, Amplitude = 0.18 }
            });
            RootScale.BeginAnimation(ScaleTransform.ScaleYProperty, new DoubleAnimation(0.96, 1, TimeSpan.FromMilliseconds(260))
            {
                EasingFunction = new BackEase { EasingMode = EasingMode.EaseOut, Amplitude = 0.18 }
            });
        }

        private void BeginClose(bool? dialogResult)
        {
            if (_isCloseAnimationRunning)
            {
                _pendingDialogResult = dialogResult ?? _pendingDialogResult;
                return;
            }

            _pendingDialogResult = dialogResult;
            _isCloseAnimationRunning = true;

            var closeAnimation = new DoubleAnimation(RootShell.Opacity, 0, TimeSpan.FromMilliseconds(170))
            {
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseIn }
            };
            closeAnimation.Completed += (_, _) =>
            {
                _allowClose = true;
                _isCloseAnimationRunning = false;
                if (_pendingDialogResult.HasValue)
                {
                    try
                    {
                        DialogResult = _pendingDialogResult.Value;
                    }
                    catch (InvalidOperationException)
                    {
                        Close();
                    }
                }
                else
                {
                    Close();
                }
            };

            RootShell.BeginAnimation(OpacityProperty, closeAnimation);
            RootScale.BeginAnimation(ScaleTransform.ScaleXProperty, new DoubleAnimation(1, 0.96, TimeSpan.FromMilliseconds(170))
            {
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseIn }
            });
            RootScale.BeginAnimation(ScaleTransform.ScaleYProperty, new DoubleAnimation(1, 0.96, TimeSpan.FromMilliseconds(170))
            {
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseIn }
            });
        }

        private void ApplyRingLayout(bool animate)
        {
            var count = SideItems.Count;
            if (count == 0 || OptionsItemsControl == null)
            {
                return;
            }

            for (var index = 0; index < count; index++)
            {
                var option = SideItems[index];
                var slot = GetWrappedSlot(index, count);
                var angle = count == 1
                    ? 0
                    : RingStartAngle + (RingSweepAngle * slot / (count - 1));

                option.ZIndex = option.IsActiveVisual ? 160 : 90 - (int)Math.Abs(angle);

                if (OptionsItemsControl.ItemContainerGenerator.ContainerFromItem(option) is ContentPresenter presenter)
                {
                    MovePresenter(presenter, angle, animate);
                }
            }
        }

        private void RefreshOptionZIndexes()
        {
            var count = SideItems.Count;
            for (var index = 0; index < count; index++)
            {
                var option = SideItems[index];
                var slot = GetWrappedSlot(index, count);
                var angle = count == 1
                    ? 0
                    : RingStartAngle + (RingSweepAngle * slot / (count - 1));

                option.ZIndex = option.IsActiveVisual ? 160 : 90 - (int)Math.Abs(angle);
            }
        }

        private double GetWrappedSlot(int index, int count)
        {
            if (count <= 1)
            {
                return 0;
            }

            var slot = (index - _ringOffset) % count;
            if (slot < 0)
            {
                slot += count;
            }

            return slot;
        }

        private void MovePresenter(ContentPresenter presenter, double angle, bool animate)
        {
            var currentLeft = Canvas.GetLeft(presenter);
            var currentTop = Canvas.GetTop(presenter);

            if (double.IsNaN(currentLeft) || double.IsNaN(currentTop) || !animate)
            {
                presenter.BeginAnimation(CurrentAngleProperty, null);
                
                // 将 angle 标准化到 [-180, 180] 之间
                var normalizedAngle = angle;
                while (normalizedAngle > 180) normalizedAngle -= 360;
                while (normalizedAngle <= -180) normalizedAngle += 360;
                
                presenter.SetValue(CurrentAngleProperty, normalizedAngle);
                presenter.BeginAnimation(OpacityProperty, null);
                presenter.Opacity = 1;
                return;
            }

            var currentAngle = (double)presenter.GetValue(CurrentAngleProperty);
            
            // 调整 targetAngle 以确保它与滚轮滚动方向一致
            var targetAngle = angle;
            if (_isScrollingDown)
            {
                while (targetAngle >= currentAngle)
                {
                    targetAngle -= 360;
                }
                while (currentAngle - targetAngle > 360)
                {
                    targetAngle += 360;
                }
            }
            else
            {
                while (targetAngle <= currentAngle)
                {
                    targetAngle += 360;
                }
                while (targetAngle - currentAngle > 360)
                {
                    targetAngle -= 360;
                }
            }

            if (Math.Abs(currentAngle - targetAngle) < 0.1)
            {
                presenter.BeginAnimation(OpacityProperty, null);
                presenter.Opacity = 1;
                return;
            }

            presenter.BeginAnimation(
                CurrentAngleProperty,
                new DoubleAnimation(currentAngle, targetAngle, _ringAnimationDuration)
                {
                    EasingFunction = _ringEasing
                });
            presenter.BeginAnimation(OpacityProperty, null);
            presenter.Opacity = 1;
        }

        private void ExpandOption(UniversalNewDialogOption option)
        {
            if (_expandedOption != null && !ReferenceEquals(_expandedOption, option))
            {
                _expandedOption.IsExpanded = false;
            }

            _expandedOption = option;
            option.IsExpanded = true;
            SettingPanelTitle = option.Title;
            SettingPanelDescription = option.DescriptionText;
            SettingContentHost.Content = option.SettingContent;
            option.SettingOpened?.Invoke(option);
            ShowSettingPanel(option);
            ClearMainHint();
            HighlightOption(null);
        }

        private void ShowSettingPanel(UniversalNewDialogOption option)
        {
            var panelTop = 246d;
            if (OptionsItemsControl.ItemContainerGenerator.ContainerFromItem(option) is ContentPresenter presenter)
            {
                var top = Canvas.GetTop(presenter);
                if (!double.IsNaN(top))
                {
                    panelTop = Math.Max(112, Math.Min(430, top + 94));
                }
            }

            Canvas.SetLeft(SettingPanel, 644);
            Canvas.SetTop(SettingPanel, panelTop);
            SettingPanel.Visibility = Visibility.Visible;

            if (SettingPanel.RenderTransform is not TranslateTransform translate)
            {
                translate = new TranslateTransform();
                SettingPanel.RenderTransform = translate;
            }

            translate.X = 18;
            SettingPanel.BeginAnimation(OpacityProperty, null);
            SettingPanel.Opacity = 0;
            SettingPanel.BeginAnimation(OpacityProperty, new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(190))
            {
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            });
            translate.BeginAnimation(TranslateTransform.XProperty, new DoubleAnimation(18, 0, TimeSpan.FromMilliseconds(220))
            {
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            });
        }

        private void HideSettingPanel()
        {
            if (_expandedOption != null)
            {
                _expandedOption.IsExpanded = false;
                _expandedOption = null;
            }

            var fade = new DoubleAnimation(SettingPanel.Opacity, 0, TimeSpan.FromMilliseconds(140));
            fade.Completed += (_, _) =>
            {
                SettingPanel.Visibility = Visibility.Collapsed;
                SettingContentHost.Content = null;
            };
            SettingPanel.BeginAnimation(OpacityProperty, fade);
        }

        private void ActivateOption(UniversalNewDialogOption option)
        {
            if (option.Kind == UniversalNewDialogOptionKind.Setting)
            {
                ExpandOption(option);
                return;
            }

            option.Execute?.Invoke(option);
        }

        private static ImageSource? CreateImageSource(string iconUri)
        {
            if (string.IsNullOrWhiteSpace(iconUri))
            {
                return null;
            }

            var normalizedUri = iconUri.Trim().Replace('\\', '/');
            Uri uri;
            if (normalizedUri.StartsWith("pack://", StringComparison.OrdinalIgnoreCase))
            {
                uri = new Uri(normalizedUri, UriKind.Absolute);
            }
            else if (normalizedUri.StartsWith("/", StringComparison.Ordinal))
            {
                uri = new Uri("pack://application:,,," + normalizedUri, UriKind.Absolute);
            }
            else if (normalizedUri.Contains(";component/", StringComparison.OrdinalIgnoreCase))
            {
                uri = new Uri("pack://application:,,,/" + normalizedUri.TrimStart('/'), UriKind.Absolute);
            }
            else
            {
                uri = new Uri(normalizedUri, UriKind.RelativeOrAbsolute);
            }

            var image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.UriSource = uri;
            image.EndInit();
            image.Freeze();
            return image;
        }

        private void OptionsRing_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (SideItems.Count <= 1)
            {
                return;
            }

            _isScrollingDown = e.Delta < 0;
            _ringOffset += _isScrollingDown ? 1 : -1;
            HideSettingPanel(); // 滚动时关闭设置面板
            ApplyRingLayout(true);
            e.Handled = true;
        }

        private void OptionItem_MouseEnter(object sender, MouseEventArgs e)
        {
            if ((sender as FrameworkElement)?.DataContext is UniversalNewDialogOption option)
            {
                option.IsHovered = true;
            }
        }

        private void OptionItem_MouseLeave(object sender, MouseEventArgs e)
        {
            if ((sender as FrameworkElement)?.DataContext is UniversalNewDialogOption option)
            {
                option.IsHovered = false;
            }
        }

        private void OptionItem_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if ((sender as FrameworkElement)?.DataContext is UniversalNewDialogOption option)
            {
                ActivateOption(option);
                e.Handled = true;
            }
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                BeginClose(false);
                e.Handled = true;
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left || e.ClickCount != 1)
            {
                return;
            }

            if (e.OriginalSource is DependencyObject source &&
                (IsDescendantOf(OptionsItemsControl, source) || IsDescendantOf(SettingPanel, source)))
            {
                return;
            }

            try
            {
                DragMove();
            }
            catch (InvalidOperationException)
            {
            }
        }

        private static bool IsDescendantOf(DependencyObject ancestor, DependencyObject source)
        {
            var current = source;
            while (current != null)
            {
                if (ReferenceEquals(current, ancestor))
                {
                    return true;
                }

                current = VisualTreeHelper.GetParent(current);
            }

            return false;
        }

        private void ClipElementToEllipse(object sender, SizeChangedEventArgs e)
        {
            if (sender is FrameworkElement element && element.ActualWidth > 0 && element.ActualHeight > 0)
            {
                element.Clip = new EllipseGeometry(new Rect(0, 0, element.ActualWidth, element.ActualHeight));
            }
        }
    }

    public enum UniversalNewDialogOptionKind
    {
        Trigger,
        Setting
    }

    public sealed class UniversalNewDialogOption : ObservableObject
    {
        private string _title = string.Empty;
        private string _descriptionText = string.Empty;
        private string _contentText = string.Empty;
        private bool _isHovered;
        private bool _isExpanded;
        private bool _isHighlighted;
        private int _zIndex;

        public UniversalNewDialogOption(UniversalNewDialogOptionKind kind)
        {
            Kind = kind;
        }

        public UniversalNewDialogOptionKind Kind { get; }

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value ?? string.Empty);
        }

        public string DescriptionText
        {
            get => _descriptionText;
            set
            {
                if (SetProperty(ref _descriptionText, value ?? string.Empty))
                {
                    RefreshAuxiliaryText();
                }
            }
        }

        public string ContentText
        {
            get => _contentText;
            set
            {
                if (SetProperty(ref _contentText, value ?? string.Empty))
                {
                    RefreshAuxiliaryText();
                }
            }
        }

        public string AuxiliaryText => string.IsNullOrWhiteSpace(ContentText) ? DescriptionText : ContentText;

        public bool HasAuxiliaryText => !string.IsNullOrWhiteSpace(AuxiliaryText);

        public bool HasContentText => !string.IsNullOrWhiteSpace(ContentText);

        public ImageSource? IconSource { get; init; }

        public FrameworkElement? SettingContent { get; init; }

        public Func<UniversalNewDialogOption, bool>? Execute { get; init; }

        public Action<UniversalNewDialogOption>? SettingOpened { get; init; }

        public bool IsHovered
        {
            get => _isHovered;
            set
            {
                if (SetProperty(ref _isHovered, value))
                {
                    OnPropertyChanged(nameof(IsActiveVisual));
                }
            }
        }

        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                if (SetProperty(ref _isExpanded, value))
                {
                    OnPropertyChanged(nameof(IsActiveVisual));
                }
            }
        }

        public bool IsHighlighted
        {
            get => _isHighlighted;
            set => SetProperty(ref _isHighlighted, value);
        }

        public bool IsActiveVisual => IsHovered || IsExpanded;

        public int ZIndex
        {
            get => _zIndex;
            set => SetProperty(ref _zIndex, value);
        }

        private void RefreshAuxiliaryText()
        {
            OnPropertyChanged(nameof(AuxiliaryText));
            OnPropertyChanged(nameof(HasAuxiliaryText));
            OnPropertyChanged(nameof(HasContentText));
        }
    }
}
