using System.IO;
using Ferrita.Services.Directories;

namespace Ferrita.Services.PresentationUI
{
    public sealed class PresentationUIPathProvider
    {
        public string ConfigurationDirectoryPath => FerritaDirectoryRuntime.Instance.ConfigurationDirectoryPath;

        public string ConfigurationFilePath => Path.Combine(ConfigurationDirectoryPath, "PresentationUI.xml");
    }
}
