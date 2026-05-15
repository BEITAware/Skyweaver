using System.IO;
using Skyweaver.Services.Directories;

namespace Skyweaver.Services.ContextManagement
{
    public sealed class ContextManagementPathProvider
    {
        public string ConfigurationDirectoryPath => SkyweaverDirectoryRuntime.Instance.ConfigurationDirectoryPath;

        public string ConfigurationFilePath => Path.Combine(ConfigurationDirectoryPath, "ContextManagement.xml");
    }
}
