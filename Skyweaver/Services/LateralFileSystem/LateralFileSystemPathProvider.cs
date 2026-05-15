using System.IO;
using Skyweaver.Services.Directories;

namespace Skyweaver.Services.LateralFileSystem
{
    public sealed class LateralFileSystemPathProvider
    {
        public string ConfigurationDirectoryPath => SkyweaverDirectoryRuntime.Instance.ConfigurationDirectoryPath;

        public string ConfigurationFilePath => Path.Combine(ConfigurationDirectoryPath, "LateralFileSystem.xml");

        public static string GetTreeFilePath(string workingRootDirectory)
        {
            return Path.Combine(workingRootDirectory, "LateralFileSystemTree.xml");
        }
    }
}
