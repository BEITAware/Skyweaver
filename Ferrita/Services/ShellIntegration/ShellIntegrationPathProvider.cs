using System.IO;
using Ferrita.Services.Directories;

namespace Ferrita.Services.ShellIntegration
{
    public sealed class ShellIntegrationPathProvider
    {
        public string ConfigurationDirectoryPath => FerritaDirectoryRuntime.Instance.ConfigurationDirectoryPath;

        public string ConfigurationFilePath => Path.Combine(ConfigurationDirectoryPath, "ShellIntegration.xml");
    }
}
