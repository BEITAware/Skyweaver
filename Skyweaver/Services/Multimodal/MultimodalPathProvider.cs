using System.IO;
using Skyweaver.Services.Directories;

namespace Skyweaver.Services.Multimodal
{
    public sealed class MultimodalPathProvider
    {
        public string ConfigurationDirectoryPath => SkyweaverDirectoryRuntime.Instance.ConfigurationDirectoryPath;

        public string ConfigurationFilePath => Path.Combine(ConfigurationDirectoryPath, "Multimodal.xml");
    }
}
