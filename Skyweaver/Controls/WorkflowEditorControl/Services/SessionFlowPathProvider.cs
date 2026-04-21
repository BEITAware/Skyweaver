using System.IO;

namespace Skyweaver.Controls.WorkflowEditorControl.Services
{
    public sealed class SessionFlowPathProvider
    {
        public string ConfigurationDirectoryPath => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            "Skyweaver",
            "Nodegraphs");
    }
}
