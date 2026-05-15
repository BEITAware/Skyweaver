using System.IO;
using Skyweaver.Services.Directories;

namespace Skyweaver.Controls.LanguageModelConfigurationControl.Services
{
    public sealed class LanguageModelConfigurationPathProvider
    {
        public string ConfigurationDirectoryPath => SkyweaverDirectoryRuntime.Instance.ConfigurationDirectoryPath;

        public string LanguageModelFilePath => Path.Combine(ConfigurationDirectoryPath, "LanguageModel.xml");

        public string CapabilityLayerFilePath => Path.Combine(ConfigurationDirectoryPath, "CapabilityLayer.xml");
    }
}
