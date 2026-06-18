using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using Ferrita.Controls.ChatSessionControl.Views;
using Ferrita.Services.FerritaTools;

namespace Ferrita.Tools
{
    public sealed class CalculateFileHashTool :
        IFerritaTool,
        IFerritaToolInvocationPresentationProvider,
        IFerritaToolPromptDescriptionProvider
    {
        public const string ToolName = "CalculateFileHash";

        private static readonly FerritaToolDefinition s_definition = new(
            ToolName,
            "Calculates the cryptographic hash of a file. Supports MD5, SHA1, SHA256, SHA384, and SHA512 algorithms. FilePath may be an absolute or relative path, or a LateralFS\\NodeName\\relative\\file.ext shortcut.",
            "Script",
            [
                new FerritaToolParameterDefinition(
                    "FilePath",
                    "Path of the file to hash. Relative paths resolve against the current workspace.",
                    FerritaToolParameterType.String,
                    isRequired: true),
                new FerritaToolParameterDefinition(
                    "Algorithm",
                    "Hash algorithm to use (MD5, SHA1, SHA256, SHA384, SHA512). Default is SHA256.",
                    FerritaToolParameterType.String,
                    isRequired: false,
                    defaultValue: "SHA256")
            ],
            defaultAgentPermission: FerritaToolDefaultAgentPermission.Allow);

        public FerritaToolDefinition Definition => s_definition;

        public string GetPromptDescription(FerritaToolPromptDescriptionContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            return "Calculates the cryptographic hash of a file. FilePath may be a normal absolute or relative path, or a LateralFS\\NodeName\\relative\\file.ext shortcut. Algorithm is optional and defaults to SHA256 (supports MD5, SHA1, SHA256, SHA384, SHA512). Returns the hash as a hexadecimal string.";
        }

        public FrameworkElement? CreateInvocationPresentation(FerritaToolInvocationPresentationContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            return ToolInvocationCardFactory.Create(
                context,
                [
                    new ToolInvocationCardFieldDefinition("File path", "FilePath", "Waiting for file path..."),
                    new ToolInvocationCardFieldDefinition("Algorithm", "Algorithm", "Default SHA256")
                ]);
        }

        public async Task<FerritaToolResult> ExecuteAsync(
            FerritaToolContext context,
            FerritaToolArguments arguments,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var requestedPath = arguments.GetString("FilePath") ?? string.Empty;
            var algorithmName = arguments.GetString("Algorithm") ?? "SHA256";
            string resolvedPath;

            try
            {
                resolvedPath = ToolFileSystemHelper.ResolvePath(requestedPath, context.WorkspacePath);
            }
            catch (Exception ex)
            {
                return FerritaToolResult.Failure($"Invalid path: {ex.Message}");
            }

            if (Directory.Exists(resolvedPath))
            {
                return FerritaToolResult.Failure($"Path points to a directory, not a file: {resolvedPath}");
            }

            if (!File.Exists(resolvedPath))
            {
                return FerritaToolResult.Failure($"File not found: {resolvedPath}");
            }

            try
            {
                using var stream = File.OpenRead(resolvedPath);
                using var hashAlgorithm = CreateHashAlgorithm(algorithmName);
                if (hashAlgorithm == null)
                {
                    return FerritaToolResult.Failure($"Unsupported hash algorithm: {algorithmName}. Supported algorithms are MD5, SHA1, SHA256, SHA384, SHA512.");
                }

                var hashBytes = await hashAlgorithm.ComputeHashAsync(stream, cancellationToken).ConfigureAwait(false);
                var hashHex = Convert.ToHexString(hashBytes).ToLowerInvariant();

                return FerritaToolResult.Success(BuildContent(resolvedPath, algorithmName, hashHex));
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                return FerritaToolResult.Failure($"Failed to read file for hashing: {ex.Message}");
            }
        }

        private static HashAlgorithm? CreateHashAlgorithm(string name)
        {
            return name.Trim().ToUpperInvariant() switch
            {
                "MD5" => MD5.Create(),
                "SHA1" => SHA1.Create(),
                "SHA256" => SHA256.Create(),
                "SHA384" => SHA384.Create(),
                "SHA512" => SHA512.Create(),
                _ => null
            };
        }

        private static string BuildContent(string resolvedPath, string algorithm, string hashHex)
        {
            var builder = new StringBuilder(256);
            builder.AppendLine($"Path: {resolvedPath}");
            builder.AppendLine($"Algorithm: {algorithm.Trim().ToUpperInvariant()}");
            builder.AppendLine($"Hash: {hashHex}");
            return builder.ToString().TrimEnd();
        }
    }
}
