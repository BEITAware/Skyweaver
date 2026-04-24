using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Skyweaver.Controls.LateralFileSystemTreeControl.ViewModels;

namespace Skyweaver.Controls.LateralFileSystemTreeControl.Views
{
    public partial class LateralFileSystemTreeControl : UserControl
    {
        // 用于追踪当次拖动是否为真实拖动（位移超出点击容差）
        private double _dragAccumX;
        private double _dragAccumY;
        private bool _isDragging;
        private LateralFileSystemNodeViewModel? _pendingSelectVm;

        public LateralFileSystemTreeControl()
        {
            InitializeComponent();
        }

        private void Node_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is LateralFileSystemNodeViewModel nodeVm)
            {
                _dragAccumX += e.HorizontalChange;
                _dragAccumY += e.VerticalChange;

                // 超过 4px 才认为是真实拖动，取消点击选中
                if (System.Math.Abs(_dragAccumX) > 4 || System.Math.Abs(_dragAccumY) > 4)
                {
                    _isDragging = true;
                    _pendingSelectVm = null;
                }

                nodeVm.X += e.HorizontalChange;
                nodeVm.Y += e.VerticalChange;
            }
        }

        // 使用 PreviewMouseLeftButtonDown（Tunnel 阶段）在 Thumb 捕获鼠标前拦截
        private void Node_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _dragAccumX = 0;
            _dragAccumY = 0;
            _isDragging = false;

            if (sender is FrameworkElement element && element.DataContext is LateralFileSystemNodeViewModel nodeVm)
            {
                _pendingSelectVm = nodeVm;
            }
        }

        private void Node_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            if (sender is FrameworkElement element
                && element.DataContext is LateralFileSystemNodeViewModel nodeVm
                && DataContext is LateralFileSystemTreeControlViewModel treeVm)
            {
                treeVm.PersistNodeVisualPosition(nodeVm);
            }

            // 仅在没有真实拖动时（即纯点击）提交选中
            if (!_isDragging && _pendingSelectVm != null)
            {
                if (DataContext is LateralFileSystemTreeControlViewModel vm)
                {
                    vm.SelectedNode = _pendingSelectVm;
                }
            }
            _pendingSelectVm = null;
            _isDragging = false;
        }

        private void OnCanvasMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is Canvas || e.OriginalSource is Grid)
            {
                if (DataContext is LateralFileSystemTreeControlViewModel vm)
                {
                    vm.SelectedNode = null;
                }
            }
        }
    }
}
