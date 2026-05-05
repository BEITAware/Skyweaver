using System.IO;

namespace Skyweaver.Services.LateralFileSystem
{
    public sealed class LateralFileSystemPathProvider
    {
        public string ConfigurationDirectoryPath => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            "Skyweaver",
            "Configuration");

        public string ConfigurationFilePath => Path.Combine(ConfigurationDirectoryPath, "LateralFileSystem.xml");

        public static string GetTreeFilePath(string workingRootDirectory)
        {
            return Path.Combine(workingRootDirectory, "LateralFileSystemTree.xml");
        }
    }
}
