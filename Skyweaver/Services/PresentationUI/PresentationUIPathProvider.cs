using System.IO;
using Skyweaver.Services.Directories;

namespace Skyweaver.Services.PresentationUI
{
    public sealed class PresentationUIPathProvider
    {
        public string ConfigurationDirectoryPath => SkyweaverDirectoryRuntime.Instance.ConfigurationDirectoryPath;

        public string ConfigurationFilePath => Path.Combine(ConfigurationDirectoryPath, "PresentationUI.xml");
    }
}
