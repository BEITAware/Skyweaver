using System.IO;
using Skyweaver.Services.Directories;

namespace Skyweaver.Services.Localization
{
    public sealed class LocalizationPathProvider
    {
        public string ConfigurationDirectoryPath => SkyweaverDirectoryRuntime.Instance.ConfigurationDirectoryPath;

        public string ConfigurationFilePath => Path.Combine(ConfigurationDirectoryPath, "Localization.xml");
    }
}
