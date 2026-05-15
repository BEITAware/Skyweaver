using System.IO;
using Skyweaver.Services.Directories;

namespace Skyweaver.Controls.WorkflowEditorControl.Services
{
    public sealed class SessionFlowPathProvider
    {
        public string ConfigurationDirectoryPath => SkyweaverDirectoryRuntime.Instance.SessionFlowsDirectoryPath;
    }
}
