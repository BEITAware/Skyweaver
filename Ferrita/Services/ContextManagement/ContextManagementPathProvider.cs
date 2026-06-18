using System.IO;
using Ferrita.Services.Directories;

namespace Ferrita.Services.ContextManagement
{
    public sealed class ContextManagementPathProvider
    {
        public string ConfigurationDirectoryPath => FerritaDirectoryRuntime.Instance.ConfigurationDirectoryPath;

        public string ConfigurationFilePath => Path.Combine(ConfigurationDirectoryPath, "ContextManagement.xml");
    }
}
