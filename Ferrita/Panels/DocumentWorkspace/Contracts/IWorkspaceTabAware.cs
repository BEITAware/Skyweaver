namespace Ferrita.Panels.DocumentWorkspace.Contracts
{
    public interface IWorkspaceTabAware
    {
        void AttachToWorkspaceTab(IWorkspaceTabController controller);
    }
}
