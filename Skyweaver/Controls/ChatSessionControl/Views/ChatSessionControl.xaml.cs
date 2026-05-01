using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Skyweaver.Controls.ChatSessionControl.ViewModels;

namespace Skyweaver.Controls.ChatSessionControl.Views
{
    public partial class ChatSessionControl : UserControl
    {
        public static readonly DependencyProperty RibbonTimeProperty =
            DependencyProperty.Register(
                nameof(RibbonTime),
                typeof(double),
                typeof(ChatSessionControl),
                new PropertyMetadata(0.0));

        public static readonly DependencyProperty RibbonAspectRatioProperty =
            DependencyProperty.Register(
                nameof(RibbonAspectRatio),
                typeof(double),
                typeof(ChatSessionControl),
                new PropertyMetadata(1.0));

        private readonly Stopwatch _ribbonStopwatch = Stopwatch.StartNew();
        private INotifyCollectionChanged? _trackedMessages;
        private bool _isRenderingHooked;

        public ChatSessionControl()
        {
            InitializeComponent();
            Loaded += ChatSessionControl_Loaded;
            Unloaded += ChatSessionControl_Unloaded;
            DataContextChanged += ChatSessionControl_DataContextChanged;
        }

        private void ChatSessionControl_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateRibbonAspectRatio();
            HookRibbonRendering();
            TrackMessagesCollection();
            ScrollToLatestMessage();
        }

        private void ChatSessionControl_Unloaded(object sender, RoutedEventArgs e)
        {
            UnhookRibbonRendering();
            DetachMessagesCollection();
        }

        private void ChatSessionControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            TrackMessagesCollection();
        }

        private void TrackMessagesCollection()
        {
            var messageCollection = (DataContext as ChatSessionControlViewModel)?.Messages;
            if (ReferenceEquals(_trackedMessages, messageCollection))
            {
                return;
            }

            DetachMessagesCollection();
            _trackedMessages = messageCollection;

            if (_trackedMessages != null)
            {
                _trackedMessages.CollectionChanged += MessagesCollection_CollectionChanged;
            }
        }

        private void DetachMessagesCollection()
        {
            if (_trackedMessages == null)
            {
                return;
            }

            _trackedMessages.CollectionChanged -= MessagesCollection_CollectionChanged;
            _trackedMessages = null;
        }

        private void MessagesCollection_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add
                || e.Action == NotifyCollectionChangedAction.Move
                || e.Action == NotifyCollectionChangedAction.Replace
                || e.Action == NotifyCollectionChangedAction.Reset)
            {
                ScrollToLatestMessage();
            }
        }

        private void ScrollToLatestMessage()
        {
            if (MessagesList.Items.Count == 0)
            {
                return;
            }

            Dispatcher.BeginInvoke(new Action(() =>
            {
                var latestMessage = MessagesList.Items[MessagesList.Items.Count - 1];
                MessagesList.ScrollIntoView(latestMessage);
            }), DispatcherPriority.Background);
        }

        private void ComposerTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Return && e.Key != Key.Enter)
            {
                return;
            }

            var modifiers = Keyboard.Modifiers;
            if ((modifiers & (ModifierKeys.Control | ModifierKeys.Shift)) != ModifierKeys.None)
            {
                return;
            }

            if (modifiers != ModifierKeys.None)
            {
                return;
            }

            if (DataContext is ChatSessionControlViewModel viewModel &&
                viewModel.SendMessageCommand.CanExecute(null))
            {
                viewModel.SendMessageCommand.Execute(null);
            }

            e.Handled = true;
        }

        private void ComposerTextBox_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (!ReferenceEquals(sender, ComposerTextBox) ||
                !ComposerTextBox.IsKeyboardFocusWithin)
            {
                return;
            }

            var image = TryGetPastedImage(e);
            if (image == null)
            {
                return;
            }

            if (DataContext is ChatSessionControlViewModel viewModel &&
                viewModel.AddPastedImage(image))
            {
                e.CancelCommand();
                e.Handled = true;
            }
        }

        private static BitmapSource? TryGetPastedImage(DataObjectPastingEventArgs e)
        {
            if (Clipboard.ContainsImage())
            {
                return Clipboard.GetImage();
            }

            return e.SourceDataObject.GetDataPresent(DataFormats.Bitmap, autoConvert: true)
                ? e.SourceDataObject.GetData(DataFormats.Bitmap, autoConvert: true) as BitmapSource
                : null;
        }

        public double RibbonTime
        {
            get => (double)GetValue(RibbonTimeProperty);
            set => SetValue(RibbonTimeProperty, value);
        }

        public double RibbonAspectRatio
        {
            get => (double)GetValue(RibbonAspectRatioProperty);
            set => SetValue(RibbonAspectRatioProperty, value);
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            UpdateRibbonAspectRatio();
        }

        private void HookRibbonRendering()
        {
            if (_isRenderingHooked)
            {
                return;
            }

            CompositionTarget.Rendering += CompositionTarget_Rendering;
            _isRenderingHooked = true;
        }

        private void UnhookRibbonRendering()
        {
            if (!_isRenderingHooked)
            {
                return;
            }

            CompositionTarget.Rendering -= CompositionTarget_Rendering;
            _isRenderingHooked = false;
        }

        private void CompositionTarget_Rendering(object? sender, EventArgs e)
        {
            RibbonTime = _ribbonStopwatch.Elapsed.TotalSeconds;
        }

        private void UpdateRibbonAspectRatio()
        {
            var height = ActualHeight;
            RibbonAspectRatio = height > 0 ? Math.Max(ActualWidth / height, 0.0001) : 1.0;
        }
    }
}
