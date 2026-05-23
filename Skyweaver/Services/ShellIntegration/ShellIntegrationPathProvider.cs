using System.IO;
using Skyweaver.Services.Directories;

namespace Skyweaver.Services.ShellIntegration
{
    public sealed class ShellIntegrationPathProvider
    {
        public string ConfigurationDirectoryPath => SkyweaverDirectoryRuntime.Instance.ConfigurationDirectoryPath;

        public string ConfigurationFilePath => Path.Combine(ConfigurationDirectoryPath, "ShellIntegration.xml");
    }
}
