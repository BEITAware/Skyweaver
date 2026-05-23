using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Skyweaver.Controls.ShellChatSessionControl.ViewModels;

namespace Skyweaver.Controls.ShellChatSessionControl.Views
{
    public partial class ShellChatSessionControl : UserControl
    {
        private INotifyCollectionChanged? _trackedMessages;
        private ScrollViewer? _messagesScrollViewer;
        private bool _autoScrollToLatestMessage = true;
        private bool _isScrollingToLatestMessage;
        private bool _isScrollToLatestMessageQueued;

        public ShellChatSessionControl()
        {
            InitializeComponent();
            Loaded += ShellChatSessionControl_Loaded;
            Unloaded += ShellChatSessionControl_Unloaded;
            DataContextChanged += ShellChatSessionControl_DataContextChanged;
            MessagesListBox.PreviewMouseDown += MessagesListBox_PreviewMouseDown;
        }

        public void FocusComposer()
        {
            InputTextBox.Focus();
            Keyboard.Focus(InputTextBox);
        }

        private void ShellChatSessionControl_Loaded(object sender, RoutedEventArgs e)
        {
            HookMessagesScrollViewer();
            TrackMessagesCollection();
            _autoScrollToLatestMessage = true;
            ScrollToBottom(force: true);
        }

        private void ShellChatSessionControl_Unloaded(object sender, RoutedEventArgs e)
        {
            DetachMessagesScrollViewer();
            DetachMessagesCollection();
        }

        private void ShellChatSessionControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is ShellChatSessionControlViewModel oldViewModel)
            {
                oldViewModel.RequestClose -= OnRequestClose;
            }

            if (e.NewValue is ShellChatSessionControlViewModel newViewModel)
            {
                newViewModel.RequestClose += OnRequestClose;
                _autoScrollToLatestMessage = true;
                ScrollToBottom(force: true);
            }

            TrackMessagesCollection();
        }

        private void TrackMessagesCollection()
        {
            var messageCollection = (DataContext as ShellChatSessionControlViewModel)?.Messages;
            if (ReferenceEquals(_trackedMessages, messageCollection))
            {
                return;
            }

            DetachMessagesCollection();
            _trackedMessages = messageCollection;
            if (_trackedMessages != null)
            {
                _trackedMessages.CollectionChanged += Messages_CollectionChanged;
            }
        }

        private void DetachMessagesCollection()
        {
            if (_trackedMessages == null)
            {
                return;
            }

            _trackedMessages.CollectionChanged -= Messages_CollectionChanged;
            _trackedMessages = null;
        }

        private void Messages_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (_autoScrollToLatestMessage || IsMessagesScrollViewerAtBottom())
            {
                ScrollToBottom();
            }
        }

        private void MessagesListBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Middle)
            {
                return;
            }

            _autoScrollToLatestMessage = !_autoScrollToLatestMessage;
            if (_autoScrollToLatestMessage)
            {
                ScrollToBottom();
            }

            e.Handled = true;
        }

        private void OnRequestClose()
        {
            Window.GetWindow(this)?.Close();
        }

        private void HookMessagesScrollViewer()
        {
            if (!IsLoaded || _messagesScrollViewer != null)
            {
                return;
            }

            MessagesListBox.ApplyTemplate();
            _messagesScrollViewer = FindDescendant<ScrollViewer>(MessagesListBox);
            if (_messagesScrollViewer != null)
            {
                _messagesScrollViewer.ScrollChanged += MessagesScrollViewer_ScrollChanged;
                return;
            }

            Dispatcher.BeginInvoke(new Action(HookMessagesScrollViewer), DispatcherPriority.Loaded);
        }

        private void DetachMessagesScrollViewer()
        {
            if (_messagesScrollViewer == null)
            {
                return;
            }

            _messagesScrollViewer.ScrollChanged -= MessagesScrollViewer_ScrollChanged;
            _messagesScrollViewer = null;
        }

        private void MessagesScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (_isScrollingToLatestMessage)
            {
                return;
            }

            if (e.ExtentHeightChange > 0.0 && _autoScrollToLatestMessage)
            {
                ScrollToBottom();
                return;
            }

            if (Math.Abs(e.VerticalChange) > 0.0 ||
                Math.Abs(e.ViewportHeightChange) > 0.0)
            {
                _autoScrollToLatestMessage = IsMessagesScrollViewerAtBottom();
            }
        }

        private void ScrollToBottom(bool force = false)
        {
            if (!force && !_autoScrollToLatestMessage)
            {
                return;
            }

            if (MessagesListBox.Items.Count == 0)
            {
                return;
            }

            if (_isScrollToLatestMessageQueued)
            {
                return;
            }

            _isScrollToLatestMessageQueued = true;
            Dispatcher.BeginInvoke(new Action(() =>
            {
                _isScrollToLatestMessageQueued = false;
                if (!force && !_autoScrollToLatestMessage)
                {
                    return;
                }

                if (MessagesListBox.Items.Count == 0)
                {
                    return;
                }

                _isScrollingToLatestMessage = true;
                try
                {
                    MessagesListBox.ScrollIntoView(MessagesListBox.Items[^1]);
                    _messagesScrollViewer?.ScrollToEnd();
                    _autoScrollToLatestMessage = true;
                }
                finally
                {
                    _isScrollingToLatestMessage = false;
                }
            }), DispatcherPriority.Background);
        }

        private bool IsMessagesScrollViewerAtBottom()
        {
            if (_messagesScrollViewer == null)
            {
                return true;
            }

            return _messagesScrollViewer.ScrollableHeight <= 0.0 ||
                   _messagesScrollViewer.VerticalOffset >= _messagesScrollViewer.ScrollableHeight - 1.0;
        }

        private static T? FindDescendant<T>(DependencyObject root)
            where T : DependencyObject
        {
            var count = VisualTreeHelper.GetChildrenCount(root);
            for (var index = 0; index < count; index++)
            {
                var child = VisualTreeHelper.GetChild(root, index);
                if (child is T typedChild)
                {
                    return typedChild;
                }

                var descendant = FindDescendant<T>(child);
                if (descendant != null)
                {
                    return descendant;
                }
            }

            return null;
        }

        private void InputTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
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

            if (DataContext is ShellChatSessionControlViewModel viewModel &&
                viewModel.SendCommand.CanExecute(null))
            {
                viewModel.SendCommand.Execute(null);
            }

            e.Handled = true;
        }
    }
}
