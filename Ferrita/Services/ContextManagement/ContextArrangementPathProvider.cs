using System.IO;
using Ferrita.Services.Directories;

namespace Ferrita.Services.ContextManagement
{
    public sealed class ContextArrangementPathProvider
    {
        public string ConfigurationDirectoryPath => FerritaDirectoryRuntime.Instance.ConfigurationDirectoryPath;

        public string ConfigurationFilePath => Path.Combine(ConfigurationDirectoryPath, "ContextArrangement.xml");
    }
}
