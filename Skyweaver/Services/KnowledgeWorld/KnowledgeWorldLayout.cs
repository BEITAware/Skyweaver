using System.IO;
using Skyweaver.Services.Directories;

namespace Skyweaver.Services.KnowledgeWorld
{
    public sealed class KnowledgeWorldLayout
    {
        public const string RootFolderName = "KnowledgeWorld";
        public const string DatabaseFolderName = "Database";
        public const string KnowledgeDatabaseName = "Knowledge";
        public const string QueriesDatabaseName = "Queries";
        public const string RawFolderName = "Raw";
        public const string QueriesFolderName = "Queries";

        public KnowledgeWorldLayout(string rootPath)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(rootPath);

            RootPath = Path.GetFullPath(rootPath);
            DatabasePath = Path.Combine(RootPath, DatabaseFolderName);
            KnowledgeDatabasePath = Path.Combine(DatabasePath, KnowledgeDatabaseName);
            QueriesDatabasePath = Path.Combine(DatabasePath, QueriesDatabaseName);
            RawPath = Path.Combine(RootPath, RawFolderName);
            QueriesPath = Path.Combine(RootPath, QueriesFolderName);
        }

        public string RootPath { get; }

        public string DatabasePath { get; }

        public string KnowledgeDatabasePath { get; }

        public string QueriesDatabasePath { get; }

        public string RawPath { get; }

        public string QueriesPath { get; }

        public void EnsureCreated()
        {
            Directory.CreateDirectory(RootPath);
            Directory.CreateDirectory(DatabasePath);
            Directory.CreateDirectory(RawPath);
            Directory.CreateDirectory(QueriesPath);
        }

        public static KnowledgeWorldLayout CreateDefault()
        {
            return new KnowledgeWorldLayout(
                Path.Combine(
                    SkyweaverDirectoryRuntime.Instance.AerialCityDirectoryPath,
                    RootFolderName));
        }
    }
}
