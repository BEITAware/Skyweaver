using System.IO;

namespace Skyweaver.Controls.AgentConfigurationControl.Services
{
    public sealed class AgentConfigurationPathProvider
    {
        public string ConfigurationDirectoryPath => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            "Skyweaver",
            "Configuration");

        public string ConfigurationFilePath => Path.Combine(ConfigurationDirectoryPath, "AgentConfiguration.xml");
    }
}
