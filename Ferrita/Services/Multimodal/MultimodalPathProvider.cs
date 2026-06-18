using System.IO;
using Ferrita.Services.Directories;

namespace Ferrita.Services.Multimodal
{
    public sealed class MultimodalPathProvider
    {
        public string ConfigurationDirectoryPath => FerritaDirectoryRuntime.Instance.ConfigurationDirectoryPath;

        public string ConfigurationFilePath => Path.Combine(ConfigurationDirectoryPath, "Multimodal.xml");
    }
}
