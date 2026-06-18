using System.IO;
using Ferrita.Services.Directories;

namespace Ferrita.Controls.AgentConfigurationControl.Services
{
    public sealed class AgentConfigurationPathProvider
    {
        public string ConfigurationDirectoryPath => FerritaDirectoryRuntime.Instance.ConfigurationDirectoryPath;

        public string ConfigurationFilePath => Path.Combine(ConfigurationDirectoryPath, "AgentConfiguration.xml");
    }
}
