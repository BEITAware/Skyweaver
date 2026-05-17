using System.IO;
using System.Xml.Linq;

namespace Skyweaver.Services.AerialCityRag
{
    public enum AerialCityRagIndexMode
    {
        Code,
        Everything
    }

    internal static class AerialCityRagIndexModeSupport
    {
        public const AerialCityRagIndexMode DefaultMode = AerialCityRagIndexMode.Code;

        public static AerialCityRagIndexMode Parse(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return DefaultMode;
            }

            if (TryParse(value, out var mode))
            {
                return mode;
            }

            throw new InvalidOperationException(
                $"Unsupported AerialCity RAG mode: {value}. Supported modes: Code, Everything.");
        }

        public static bool TryParse(string? value, out AerialCityRagIndexMode mode)
        {
            mode = DefaultMode;

            return Enum.TryParse(value?.Trim(), ignoreCase: true, out mode) &&
                Enum.IsDefined(typeof(AerialCityRagIndexMode), mode);
        }
    }

    internal sealed class AerialCityRagExcludedFilesStore
    {
        public const string FileName = "ExcludedFiles.xml";

        public string GetExcludedFilesPath(string databasePath)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(databasePath);
            return Path.Combine(Path.GetFullPath(databasePath), FileName);
        }

        public AerialCityRagExcludedFiles Load(string databasePath)
        {
            var filePath = GetExcludedFilesPath(databasePath);
            if (!File.Exists(filePath))
            {
                return AerialCityRagExcludedFiles.Empty(AerialCityRagIndexModeSupport.DefaultMode);
            }

            var document = XDocument.Load(filePath);
            var root = document.Root ?? throw new InvalidDataException($"{FileName} is missing its root element.");
            var modeText = ((string?)root.Element("CurrentMode")?.Attribute("Value") ??
                (string?)root.Element("CurrentMode") ??
                (string?)root.Attribute("Mode") ??
                string.Empty).Trim();
            var mode = AerialCityRagIndexModeSupport.TryParse(modeText, out var parsedMode)
                ? parsedMode
                : AerialCityRagIndexModeSupport.DefaultMode;

            var files = root.Element("Files")?.Elements("File")
                .Select(ParseExcludedFile)
                .Where(file => file != null)
                .Cast<AerialCityRagExcludedFile>()
                .GroupBy(file => file.RelativePath, StringComparer.OrdinalIgnoreCase)
                .Select(group => group.First())
                .OrderBy(file => file.RelativePath, StringComparer.OrdinalIgnoreCase)
                .ToArray() ?? [];

            return new AerialCityRagExcludedFiles
            {
                Mode = mode,
                GeneratedAtUtc = ParseDate((string?)root.Attribute("GeneratedAtUtc")),
                Files = files
            };
        }

        public void Save(string databasePath, AerialCityRagExcludedFiles excludedFiles)
        {
            ArgumentNullException.ThrowIfNull(excludedFiles);

            var filePath = GetExcludedFilesPath(databasePath);
            Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

            var document = new XDocument(
                new XElement("ExcludedFiles",
                    new XAttribute("SchemaVersion", 1),
                    new XAttribute("Mode", excludedFiles.Mode.ToString()),
                    new XAttribute("GeneratedAtUtc", DateTimeOffset.UtcNow.ToString("O")),
                    new XElement("CurrentMode",
                        new XAttribute("Value", excludedFiles.Mode.ToString())),
                    new XElement("Files",
                        excludedFiles.Files
                            .OrderBy(file => file.RelativePath, StringComparer.OrdinalIgnoreCase)
                            .Select(CreateExcludedFileElement))));

            document.Save(filePath);
        }

        private static AerialCityRagExcludedFile? ParseExcludedFile(XElement element)
        {
            var relativePath = ((string?)element.Attribute("RelativePath") ?? string.Empty).Trim();
            if (relativePath.Length == 0)
            {
                return null;
            }

            return new AerialCityRagExcludedFile
            {
                RelativePath = relativePath.Replace('\\', '/'),
                Reason = ((string?)element.Attribute("Reason") ?? string.Empty).Trim()
            };
        }

        private static XElement CreateExcludedFileElement(AerialCityRagExcludedFile file)
        {
            return new XElement("File",
                new XAttribute("RelativePath", file.RelativePath),
                new XAttribute("Reason", file.Reason));
        }

        private static DateTimeOffset ParseDate(string? value)
        {
            return DateTimeOffset.TryParse(value, out var parsed) ? parsed : DateTimeOffset.MinValue;
        }
    }

    internal sealed class AerialCityRagExcludedFiles
    {
        public AerialCityRagIndexMode Mode { get; init; }

        public DateTimeOffset GeneratedAtUtc { get; init; }

        public IReadOnlyList<AerialCityRagExcludedFile> Files { get; init; } = [];

        public static AerialCityRagExcludedFiles Empty(AerialCityRagIndexMode mode) =>
            new()
            {
                Mode = mode,
                GeneratedAtUtc = DateTimeOffset.UtcNow,
                Files = []
            };

        public bool Contains(string relativePath)
        {
            if (Mode == AerialCityRagIndexMode.Everything)
            {
                return false;
            }

            return Files.Any(file =>
                string.Equals(file.RelativePath, relativePath.Replace('\\', '/'), StringComparison.OrdinalIgnoreCase));
        }
    }

    internal sealed class AerialCityRagExcludedFile
    {
        public required string RelativePath { get; init; }

        public string Reason { get; init; } = string.Empty;
    }
}
