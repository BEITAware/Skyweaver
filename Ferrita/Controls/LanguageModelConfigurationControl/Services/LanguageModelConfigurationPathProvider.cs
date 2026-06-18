using System.IO;
using Ferrita.Services.Directories;

namespace Ferrita.Controls.LanguageModelConfigurationControl.Services
{
    public sealed class LanguageModelConfigurationPathProvider
    {
        public string ConfigurationDirectoryPath => FerritaDirectoryRuntime.Instance.ConfigurationDirectoryPath;

        public string LanguageModelFilePath => Path.Combine(ConfigurationDirectoryPath, "LanguageModel.xml");

        public string CapabilityLayerFilePath => Path.Combine(ConfigurationDirectoryPath, "CapabilityLayer.xml");
    }
}
