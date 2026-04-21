using Skyweaver.Infrastructure.Mvvm;
using Skyweaver.Panels.DocumentWorkspace.Contracts;

namespace Skyweaver.Panels.MultiFunctionArea.ViewModels
{
    public sealed class PlaceholderPanelViewModel : ObservableObject, IWorkspaceTabAware
    {
        private IWorkspaceTabController? _controller;

        public string Title { get; }

        public string Description { get; }

        public string Hint { get; }

        public PlaceholderPanelViewModel(string title, string description, string hint)
        {
            Title = title;
            Description = description;
            Hint = hint;
        }

        public void AttachToWorkspaceTab(IWorkspaceTabController controller)
        {
            _controller = controller;
        }

        public void CloseSelf()
        {
            _controller?.CloseSelf();
        }
    }
}
