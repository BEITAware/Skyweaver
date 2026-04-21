using System.IO;

namespace Skyweaver.Controls.LanguageModelConfigurationControl.Services
{
    public sealed class LanguageModelConfigurationPathProvider
    {
        public string ConfigurationDirectoryPath => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            "Skyweaver",
            "Configuration");

        public string LanguageModelFilePath => Path.Combine(ConfigurationDirectoryPath, "LanguageModel.xml");

        public string CapabilityLayerFilePath => Path.Combine(ConfigurationDirectoryPath, "CapabilityLayer.xml");
    }
}
