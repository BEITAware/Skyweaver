using System.IO;
using Ferrita.Services.Directories;

namespace Ferrita.Services.Localization
{
    public sealed class LocalizationPathProvider
    {
        public string ConfigurationDirectoryPath => FerritaDirectoryRuntime.Instance.ConfigurationDirectoryPath;

        public string ConfigurationFilePath => Path.Combine(ConfigurationDirectoryPath, "Localization.xml");
    }
}
