using System.IO;
using Skyweaver.Services.Directories;

namespace Skyweaver.Controls.AgentConfigurationControl.Services
{
    public sealed class AgentConfigurationPathProvider
    {
        public string ConfigurationDirectoryPath => SkyweaverDirectoryRuntime.Instance.ConfigurationDirectoryPath;

        public string ConfigurationFilePath => Path.Combine(ConfigurationDirectoryPath, "AgentConfiguration.xml");
    }
}
