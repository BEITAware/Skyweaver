using System.IO;
using Skyweaver.Services.Directories;

namespace Skyweaver.Services.ContextManagement
{
    public sealed class ContextArrangementPathProvider
    {
        public string ConfigurationDirectoryPath => SkyweaverDirectoryRuntime.Instance.ConfigurationDirectoryPath;

        public string ConfigurationFilePath => Path.Combine(ConfigurationDirectoryPath, "ContextArrangement.xml");
    }
}
