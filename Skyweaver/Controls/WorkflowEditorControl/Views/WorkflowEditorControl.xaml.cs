using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using Skyweaver.Controls.WorkflowEditorControl.Models;
using Skyweaver.Controls.WorkflowEditorControl.ViewModels;

namespace Skyweaver.Controls.WorkflowEditorControl.Views
{
    public partial class WorkflowEditorControl : UserControl
    {
        private SessionFlowNodeModel? _draggingNode;
        private Point _draggingOffset;
        private bool _isDragging;

        public WorkflowEditorControl()
        {
            InitializeComponent();
        }

        private WorkflowEditorControlViewModel? ViewModel => DataContext as WorkflowEditorControlViewModel;

        private void AddNodeMenuButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button || button.ContextMenu == null)
            {
                return;
            }

            ViewModel?.RefreshAgentCatalog();

            var contextMenu = button.ContextMenu;
            contextMenu.DataContext = button.DataContext;
            contextMenu.PlacementTarget = button;
            contextMenu.Placement = PlacementMode.Bottom;
            contextMenu.IsOpen = true;
            e.Handled = true;
        }

        private void NodeCard_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not FrameworkElement element ||
                element.DataContext is not SessionFlowNodeModel node ||
                ViewModel == null)
            {
                return;
            }

            if (FindNodeDataContext(e.OriginalSource as DependencyObject) is SessionFlowNodeModel originalNode &&
                !ReferenceEquals(originalNode, node))
            {
                return;
            }

            ViewModel.SelectNode(node);

            _draggingNode = node;
            _isDragging = true;

            var currentPosition = e.GetPosition(EditorSurface);
            _draggingOffset = new Point(currentPosition.X - node.X, currentPosition.Y - node.Y);

            element.CaptureMouse();
            PlayMousePressAnimation(element, true);
            e.Handled = true;
        }

        private void NodeCard_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_isDragging || _draggingNode == null || ViewModel == null)
            {
                return;
            }

            if (e.LeftButton != MouseButtonState.Pressed)
            {
                _isDragging = false;
                _draggingNode = null;
                if (sender is UIElement releasedElement)
                {
                    releasedElement.ReleaseMouseCapture();
                    PlayMousePressAnimation(releasedElement as FrameworkElement, false);
                }

                return;
            }

            var position = e.GetPosition(EditorSurface);
            ViewModel.MoveNode(_draggingNode, position.X - _draggingOffset.X, position.Y - _draggingOffset.Y);
            e.Handled = true;
        }

        private void NodeCard_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!_isDragging || _draggingNode == null || ViewModel == null)
            {
                return;
            }

            _isDragging = false;
            ViewModel.CommitNodeMove(_draggingNode);
            _draggingNode = null;

            if (sender is UIElement element)
            {
                element.ReleaseMouseCapture();
                PlayMousePressAnimation(element as FrameworkElement, false);
            }

            e.Handled = true;
        }

        private void PortAnchor_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not FrameworkElement element ||
                element.DataContext is not SessionFlowPortModel port ||
                ViewModel == null)
            {
                return;
            }

            var node = FindNodeDataContext(element);
            if (node == null)
            {
                return;
            }

            ViewModel.HandlePortClick(node, port);
            e.Handled = true;
        }

        private void SurfaceBackground_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (ViewModel == null)
            {
                return;
            }

            if (FindNodeDataContext(e.OriginalSource as DependencyObject) != null)
            {
                return;
            }

            if (ViewModel.ClearSelectionCommand.CanExecute(null))
            {
                ViewModel.ClearSelectionCommand.Execute(null);
            }
        }

        private void NodeGraphListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ViewModel?.OpenSelectedNodeGraphCommand.CanExecute(null) != true)
            {
                return;
            }

            ViewModel.OpenSelectedNodeGraphCommand.Execute(null);
            e.Handled = true;
        }

        private void PlayMousePressAnimation(FrameworkElement? element, bool isPressed)
        {
            if (element == null) return;

            if (element.RenderTransformOrigin != new Point(0.5, 0.5))
            {
                element.RenderTransformOrigin = new Point(0.5, 0.5);
            }

            ScaleTransform? scaleTransform = element.RenderTransform as ScaleTransform;
            if (scaleTransform == null && element.RenderTransform is TransformGroup group)
            {
                foreach (var child in group.Children)
                {
                    if (child is ScaleTransform st)
                    {
                        scaleTransform = st;
                        break;
                    }
                }
            }

            if (scaleTransform == null)
            {
                scaleTransform = new ScaleTransform(1.0, 1.0);
                if (element.RenderTransform is TransformGroup tg)
                {
                    tg.Children.Add(scaleTransform);
                }
                else
                {
                    element.RenderTransform = scaleTransform;
                }
            }

            var storyboard = new System.Windows.Media.Animation.Storyboard();
            double toValue = isPressed ? 0.95 : 1.0;

            var scaleXAnimation = new System.Windows.Media.Animation.DoubleAnimation
            {
                To = toValue,
                Duration = TimeSpan.FromMilliseconds(isPressed ? 100 : 200)
            };

            if (!isPressed)
            {
                scaleXAnimation.EasingFunction = new System.Windows.Media.Animation.ElasticEase
                {
                    EasingMode = System.Windows.Media.Animation.EasingMode.EaseOut,
                    Oscillations = 1,
                    Springiness = 2
                };
            }
            else
            {
                scaleXAnimation.EasingFunction = new System.Windows.Media.Animation.QuadraticEase
                {
                    EasingMode = System.Windows.Media.Animation.EasingMode.EaseOut
                };
            }

            System.Windows.Media.Animation.Storyboard.SetTarget(scaleXAnimation, element);
            System.Windows.Media.Animation.Storyboard.SetTargetProperty(scaleXAnimation, new PropertyPath("RenderTransform.ScaleX"));
            storyboard.Children.Add(scaleXAnimation);

            var scaleYAnimation = scaleXAnimation.Clone();
            System.Windows.Media.Animation.Storyboard.SetTarget(scaleYAnimation, element);
            System.Windows.Media.Animation.Storyboard.SetTargetProperty(scaleYAnimation, new PropertyPath("RenderTransform.ScaleY"));
            storyboard.Children.Add(scaleYAnimation);

            storyboard.Begin();
        }

        private static SessionFlowNodeModel? FindNodeDataContext(DependencyObject? current)
        {
            while (current != null)
            {
                if (current is FrameworkElement frameworkElement &&
                    frameworkElement.DataContext is SessionFlowNodeModel node)
                {
                    return node;
                }

                current = VisualTreeHelper.GetParent(current);
            }

            return null;
        }
    }
}
