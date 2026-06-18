using System.IO;
using Ferrita.Services.Directories;

namespace Ferrita.Controls.WorkflowEditorControl.Services
{
    public sealed class SessionFlowPathProvider
    {
        public string ConfigurationDirectoryPath => FerritaDirectoryRuntime.Instance.SessionFlowsDirectoryPath;
    }
}
