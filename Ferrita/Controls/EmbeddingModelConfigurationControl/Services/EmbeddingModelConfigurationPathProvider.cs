using System.IO;
using Ferrita.Services.Directories;

namespace Ferrita.Controls.EmbeddingModelConfigurationControl.Services
{
    public sealed class EmbeddingModelConfigurationPathProvider
    {
        public string ConfigurationDirectoryPath => FerritaDirectoryRuntime.Instance.ConfigurationDirectoryPath;

        public string EmbeddingModelFilePath => Path.Combine(ConfigurationDirectoryPath, "EmbeddingModel.xml");
    }
}
