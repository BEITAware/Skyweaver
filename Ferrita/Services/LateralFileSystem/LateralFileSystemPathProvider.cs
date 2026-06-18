using System.IO;
using Ferrita.Services.Directories;

namespace Ferrita.Services.LateralFileSystem
{
    public sealed class LateralFileSystemPathProvider
    {
        public string ConfigurationDirectoryPath => FerritaDirectoryRuntime.Instance.ConfigurationDirectoryPath;

        public string ConfigurationFilePath => Path.Combine(ConfigurationDirectoryPath, "LateralFileSystem.xml");

        public static string GetTreeFilePath(string workingRootDirectory)
        {
            return Path.Combine(workingRootDirectory, "LateralFileSystemTree.xml");
        }
    }
}
