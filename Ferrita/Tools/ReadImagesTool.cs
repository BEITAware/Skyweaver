using System.Security;
using System.Text;
using Ferrita.Services.FerritaTools;

namespace Ferrita.Tools
{
    public sealed class ReadImagesTool : IFerritaTool
    {
        public const string ToolName = "ReadImages";

        private static readonly FerritaToolDefinition s_definition = new(
            ToolName,
            "Reads one or more images and embeds the validated image files into the tool return as preserved resources. Paths must point to real decodable image files; missing or invalid files are reported as warnings or failures.",
            "Image",
            [
                new FerritaToolParameterDefinition(
                    "Paths",
                    "A pipe-separated (|) list of image file paths to read.",
                    FerritaToolParameterType.String,
                    isRequired: true)
            ],
            defaultAgentPermission: FerritaToolDefaultAgentPermission.Allow,
            defaultToolKitKeys: ["multimodal"]);

        public FerritaToolDefinition Definition => s_definition;

        public Task<FerritaToolResult> ExecuteAsync(
            FerritaToolContext context,
            FerritaToolArguments arguments,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var rawPaths = arguments.GetString("Paths");
            if (string.IsNullOrWhiteSpace(rawPaths))
            {
                return Task.FromResult(FerritaToolResult.Failure(
                    "Paths parameter is required and cannot be empty.",
                    BuildData([], ["Paths parameter is required and cannot be empty."])));
            }

            var paths = rawPaths.Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (paths.Length == 0)
            {
                return Task.FromResult(FerritaToolResult.Failure(
                    "At least one image path is required.",
                    BuildData([], ["At least one image path is required."])));
            }

            var errors = new List<string>();
            var validatedImages = ToolImageSupport.ResolveAndValidateImagePaths(
                paths,
                context.WorkspacePath,
                cancellationToken,
                errors);

            if (validatedImages.Count == 0)
            {
                return Task.FromResult(FerritaToolResult.Failure(
                    $"Failed to read any valid images. Errors:{Environment.NewLine}{string.Join(Environment.NewLine, errors)}",
                    BuildData([], errors)));
            }

            var builder = new StringBuilder();
            foreach (var image in validatedImages)
            {
                builder.AppendLine($"<FerritaPreservedContent><Image Path=\"{SecurityElement.Escape(image.ResolvedPath)}\" /></FerritaPreservedContent>");
            }

            if (errors.Count > 0)
            {
                builder.AppendLine();
                builder.AppendLine("Note: Some images could not be read:");
                foreach (var error in errors)
                {
                    builder.AppendLine(error);
                }
            }

            return Task.FromResult(FerritaToolResult.Success(
                builder.ToString().TrimEnd(),
                BuildData(validatedImages, errors)));
        }

        private static IReadOnlyDictionary<string, object?> BuildData(
            IReadOnlyList<ValidatedImagePath> validatedImages,
            IReadOnlyList<string> errors)
        {
            return new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase)
            {
                ["imageCount"] = validatedImages.Count,
                ["validatedPaths"] = validatedImages.Select(image => image.ResolvedPath).ToArray(),
                ["invalidCount"] = errors.Count,
                ["errors"] = errors.ToArray()
            };
        }
    }
}
