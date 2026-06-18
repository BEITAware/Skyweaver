using System.Windows;
using System.IO;
using Ferrita.Controls.ChatSessionControl.Views;
using Ferrita.Services.AerialCityRag;
using Ferrita.Services.FerritaTools;

namespace Ferrita.Tools
{
    public sealed class InitializeAerialCityRagTool :
        IFerritaTool,
        IFerritaToolInvocationPresentationProvider,
        IFerritaToolPromptDescriptionProvider
    {
        public const string ToolName = "InitializeAerialCityRAG";

        private static readonly FerritaToolDefinition s_definition = new(
            ToolName,
            "Initializes AerialCity RAG for a target folder. It creates AerialCity.xml in the configured AerialCity directory when missing, records the target-folder to database-folder mapping, writes ExcludedFiles.xml in the folder's AerialCity database directory, embeds text/code files with the selected embedding model, embeds images when the model supports multimodal embedding, and inserts segments into the AerialCity vector and graph database.",
            "ResourcesLibrary",
            [
                new FerritaToolParameterDefinition(
                    "TargetDirectory",
                    "Target folder to initialize for AerialCity RAG. Relative paths resolve against the current workspace.",
                    FerritaToolParameterType.String,
                    isRequired: true),
                new FerritaToolParameterDefinition(
                    "Mode",
                    "Optional indexing mode. Code is the default and excludes version-control, generated, hidden-dot, dependency-cache and noisy output files. Everything performs no mode-based exclusions.",
                    FerritaToolParameterType.String,
                    isRequired: false,
                    defaultValue: "Code"),
                new FerritaToolParameterDefinition(
                    "MaxFileBytes",
                    "Maximum single file size to embed. Default is 10485760 bytes. Use 0 to disable this guard.",
                    FerritaToolParameterType.Integer,
                    isRequired: false,
                    defaultValue: "10485760")
            ],
            defaultToolKitKeys: ["Investigate"]);

        private readonly AerialCityRagService _ragService = new();

        public FerritaToolDefinition Definition => s_definition;

        public string GetPromptDescription(FerritaToolPromptDescriptionContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            return "Initializes AerialCity RAG for TargetDirectory. This tool is available only when Preferences > Semantic Search has AerialCity RAG enabled and an AerialCity directory is configured. It creates/updates AerialCity.xml, writes ExcludedFiles.xml in the database folder, records the current embedding model, segments code files and text files with AerialCity's dedicated methods, embeds images when the selected model supports multimodal embedding, and stores the resulting vectors plus source metadata in AerialCity. Mode is optional: Code is the default and excludes generated/version-control/cache/noisy files; Everything applies no mode exclusions. MaxFileBytes defaults to 10485760; pass 0 to disable the size guard.";
        }

        public FrameworkElement? CreateInvocationPresentation(FerritaToolInvocationPresentationContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            return ToolInvocationCardFactory.CreateAerialCity(
                context,
                [
                    new ToolInvocationCardFieldDefinition("Target directory", "TargetDirectory", "Waiting for target directory..."),
                    new ToolInvocationCardFieldDefinition("Mode", "Mode", "Code"),
                    new ToolInvocationCardFieldDefinition("Max file bytes", "MaxFileBytes", "Default 10485760")
                ]);
        }

        public async Task<FerritaToolResult> ExecuteAsync(
            FerritaToolContext context,
            FerritaToolArguments arguments,
            CancellationToken cancellationToken = default)
        {
            var targetDirectory = arguments.GetString("TargetDirectory") ?? string.Empty;
            var mode = arguments.GetString("Mode");
            var maxFileBytes = arguments.GetInteger("MaxFileBytes", 10 * 1024 * 1024);

            try
            {
                var result = await _ragService.InitializeAsync(
                    targetDirectory,
                    context.WorkspacePath,
                    mode,
                    maxFileBytes,
                    cancellationToken,
                    context.ReportProgressAsync).ConfigureAwait(false);

                return FerritaToolResult.Success(
                    AerialCityRagService.FormatInitializationResult(result),
                    new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase)
                    {
                        ["targetDirectory"] = result.TargetDirectory,
                        ["aerialCityDirectory"] = result.AerialCityDirectory,
                        ["registryFilePath"] = result.RegistryFilePath,
                        ["databasePath"] = result.DatabasePath,
                        ["databaseFolderName"] = result.DatabaseFolderName,
                        ["mode"] = result.Mode.ToString(),
                        ["excludedFilesPath"] = result.ExcludedFilesPath,
                        ["excludedFileCount"] = result.ExcludedFileCount,
                        ["embeddingModel"] = result.EmbeddingModelDisplayName,
                        ["embeddingModelId"] = result.EmbeddingModelId,
                        ["supportsMultimodalEmbedding"] = result.SupportsMultimodalEmbedding,
                        ["filesVisited"] = result.Statistics.FilesVisited,
                        ["filesExcludedByMode"] = result.Statistics.FilesExcludedByMode,
                        ["segmentsInserted"] = result.Statistics.SegmentsInserted,
                        ["filesFailed"] = result.Statistics.FilesFailed
                    });
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex) when (ex is InvalidOperationException or IOException or UnauthorizedAccessException or ArgumentException or NotSupportedException)
            {
                return FerritaToolResult.Failure($"Failed to initialize AerialCity RAG: {ex.Message}");
            }
        }
    }

    public sealed class SemanticSearchTool :
        IFerritaTool,
        IFerritaToolInvocationPresentationProvider,
        IFerritaToolPromptDescriptionProvider
    {
        public const string ToolName = "SemanticSearch";

        private static readonly FerritaToolDefinition s_definition = new(
            ToolName,
            "Searches initialized AerialCity RAG databases with cosine similarity. SearchPath must be inside a folder initialized with InitializeAerialCityRAG.",
            "Script",
            [
                new FerritaToolParameterDefinition(
                    "SearchPath",
                    "Folder or file path to search. It must be equal to or inside an initialized AerialCity target folder. Relative paths resolve against the current workspace.",
                    FerritaToolParameterType.String,
                    isRequired: true),
                new FerritaToolParameterDefinition(
                    "Query",
                    "Semantic query text to embed and search with cosine similarity.",
                    FerritaToolParameterType.String,
                    isRequired: true),
                new FerritaToolParameterDefinition(
                    "TopK",
                    "Maximum number of results. Default is 10.",
                    FerritaToolParameterType.Integer,
                    isRequired: false,
                    defaultValue: "10"),
                new FerritaToolParameterDefinition(
                    "MinScore",
                    "Optional minimum cosine score.",
                    FerritaToolParameterType.Number,
                    isRequired: false)
            ],
            defaultAgentPermission: FerritaToolDefaultAgentPermission.Allow,
            defaultToolKitKeys: ["Investigate"]);

        private readonly AerialCityRagService _ragService = new();

        public FerritaToolDefinition Definition => s_definition;

        public string GetPromptDescription(FerritaToolPromptDescriptionContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            return "Searches initialized AerialCity RAG databases with cosine similarity. SearchPath is required and must be equal to or inside a folder previously initialized with InitializeAerialCityRAG; if it is outside all initialized folders, the tool reports that initialization is required. Query is embedded using the embedding model recorded for that AerialCity database. TopK defaults to 10.";
        }

        public FrameworkElement? CreateInvocationPresentation(FerritaToolInvocationPresentationContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            return ToolInvocationCardFactory.Create(
                context,
                [
                    new ToolInvocationCardFieldDefinition("Search path", "SearchPath", "Waiting for search path..."),
                    new ToolInvocationCardFieldDefinition("Query", "Query", "Waiting for semantic query..."),
                    new ToolInvocationCardFieldDefinition("Top K", "TopK", "Default 10"),
                    new ToolInvocationCardFieldDefinition("Min score", "MinScore", "Optional")
                ]);
        }

        public async Task<FerritaToolResult> ExecuteAsync(
            FerritaToolContext context,
            FerritaToolArguments arguments,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await _ragService.SemanticSearchAsync(
                    arguments.GetString("SearchPath") ?? string.Empty,
                    arguments.GetString("Query") ?? string.Empty,
                    context.WorkspacePath,
                    arguments.GetInteger("TopK", 10),
                    (float)arguments.GetNumber("MinScore", decimal.MinValue),
                    cancellationToken).ConfigureAwait(false);

                return FerritaToolResult.Success(
                    AerialCityRagService.FormatSearchResult(result),
                    BuildSearchData(result));
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex) when (ex is InvalidOperationException or IOException or UnauthorizedAccessException or ArgumentException or NotSupportedException)
            {
                return FerritaToolResult.Failure($"Semantic search failed: {ex.Message}");
            }
        }

        private static IReadOnlyDictionary<string, object?> BuildSearchData(AerialCityRagSearchResult result)
        {
            return new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase)
            {
                ["searchPath"] = result.SearchPath,
                ["targetDirectory"] = result.TargetDirectory,
                ["databasePath"] = result.DatabasePath,
                ["method"] = result.Method.ToString(),
                ["query"] = result.Query,
                ["topK"] = result.TopK,
                ["resultCount"] = result.Results.Count
            };
        }
    }

    public sealed class UpdateAerialCityDbTool :
        IFerritaTool,
        IFerritaToolInvocationPresentationProvider,
        IFerritaToolPromptDescriptionProvider
    {
        public const string ToolName = "UpdateAerialCityDB";

        private static readonly FerritaToolDefinition s_definition = new(
            ToolName,
            "Updates the initialized AerialCity database that contains the supplied path. It can refresh folders, files, deleted paths, and ExcludedFiles.xml; it follows the database folder's current mode, compares files against hashes recorded in AerialCity.xml and re-embeds changed files.",
            "Refresh",
            [
                new FerritaToolParameterDefinition(
                    "FolderPath",
                    "Folder, file, or deleted path to refresh. Relative paths resolve against the current workspace. The path must be equal to or inside a folder initialized with InitializeAerialCityRAG.",
                    FerritaToolParameterType.String,
                    isRequired: true),
                new FerritaToolParameterDefinition(
                    "MaxFileBytes",
                    "Maximum single file size to embed. Default is 10485760 bytes. Use 0 to disable this guard.",
                    FerritaToolParameterType.Integer,
                    isRequired: false,
                    defaultValue: "10485760")
            ],
            defaultToolKitKeys: ["Investigate"]);

        private readonly AerialCityRagService _ragService = new();

        public FerritaToolDefinition Definition => s_definition;

        public string GetPromptDescription(FerritaToolPromptDescriptionContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            return "Updates an existing AerialCity RAG database for FolderPath. The tool finds the initialized AerialCity DB whose target folder contains FolderPath, follows that database's current mode from ExcludedFiles.xml, honors its excluded file list, and can refresh folders, single files, or deleted paths. The update adds new matching files, re-embeds changed files, removes deleted files, and removes files that no longer satisfy the configured mode. MaxFileBytes defaults to 10485760; pass 0 to disable the size guard.";
        }

        public FrameworkElement? CreateInvocationPresentation(FerritaToolInvocationPresentationContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            return ToolInvocationCardFactory.CreateAerialCity(
                context,
                [
                    new ToolInvocationCardFieldDefinition("Path", "FolderPath", "Waiting for path..."),
                    new ToolInvocationCardFieldDefinition("Max file bytes", "MaxFileBytes", "Default 10485760")
                ]);
        }

        public async Task<FerritaToolResult> ExecuteAsync(
            FerritaToolContext context,
            FerritaToolArguments arguments,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await _ragService.UpdateDatabaseAsync(
                    arguments.GetString("FolderPath") ?? string.Empty,
                    context.WorkspacePath,
                    arguments.GetInteger("MaxFileBytes", 10 * 1024 * 1024),
                    cancellationToken,
                    context.ReportProgressAsync).ConfigureAwait(false);

                return FerritaToolResult.Success(
                    AerialCityRagService.FormatUpdateResult(result),
                    new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase)
                    {
                        ["folderPath"] = result.RequestedDirectory,
                        ["targetDirectory"] = result.TargetDirectory,
                        ["aerialCityDirectory"] = result.AerialCityDirectory,
                        ["registryFilePath"] = result.RegistryFilePath,
                        ["databasePath"] = result.DatabasePath,
                        ["databaseFolderName"] = result.DatabaseFolderName,
                        ["mode"] = result.Mode.ToString(),
                        ["excludedFilesPath"] = result.ExcludedFilesPath,
                        ["excludedFileCount"] = result.ExcludedFileCount,
                        ["embeddingModel"] = result.EmbeddingModelDisplayName,
                        ["embeddingModelId"] = result.EmbeddingModelId,
                        ["filesVisited"] = result.Statistics.FilesVisited,
                        ["filesExcludedByMode"] = result.Statistics.FilesExcludedByMode,
                        ["filesReembedded"] = result.Statistics.FilesReembedded,
                        ["filesRemoved"] = result.Statistics.FilesRemoved,
                        ["segmentsInserted"] = result.Statistics.SegmentsInserted,
                        ["segmentsUpdated"] = result.Statistics.SegmentsUpdated,
                        ["segmentsReused"] = result.Statistics.SegmentsReused,
                        ["segmentsDeleted"] = result.Statistics.SegmentsDeleted,
                        ["filesFailed"] = result.Statistics.FilesFailed
                    });
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex) when (ex is InvalidOperationException or IOException or UnauthorizedAccessException or ArgumentException or NotSupportedException)
            {
                return FerritaToolResult.Failure($"Failed to update AerialCity DB: {ex.Message}");
            }
        }
    }

    public sealed class KeywordSearchTool :
        IFerritaTool,
        IFerritaToolInvocationPresentationProvider,
        IFerritaToolPromptDescriptionProvider
    {
        public const string ToolName = "KeywordSearch";

        private static readonly FerritaToolDefinition s_definition = new(
            ToolName,
            "Searches initialized AerialCity RAG databases with BM25 keyword ranking. This is often better than grep for source/text retrieval because it ranks relevant segments instead of only returning literal line matches.",
            "Script",
            [
                new FerritaToolParameterDefinition(
                    "SearchPath",
                    "Folder or file path to search. It must be equal to or inside an initialized AerialCity target folder. Relative paths resolve against the current workspace.",
                    FerritaToolParameterType.String,
                    isRequired: true),
                new FerritaToolParameterDefinition(
                    "Query",
                    "Keyword query text for BM25 ranking.",
                    FerritaToolParameterType.String,
                    isRequired: true),
                new FerritaToolParameterDefinition(
                    "TopK",
                    "Maximum number of results. Default is 10.",
                    FerritaToolParameterType.Integer,
                    isRequired: false,
                    defaultValue: "10"),
                new FerritaToolParameterDefinition(
                    "MinScore",
                    "Optional minimum BM25 score.",
                    FerritaToolParameterType.Number,
                    isRequired: false)
            ],
            defaultAgentPermission: FerritaToolDefaultAgentPermission.Allow,
            defaultToolKitKeys: ["Investigate"]);

        private readonly AerialCityRagService _ragService = new();

        public FerritaToolDefinition Definition => s_definition;

        public string GetPromptDescription(FerritaToolPromptDescriptionContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            return "Searches initialized AerialCity RAG databases with BM25 keyword ranking. Prefer this over GrepSearch for many codebase investigations: it ranks semantically useful text/code segments and is usually better than grep when you need relevant passages rather than exact line matches. SearchPath must be inside a folder initialized with InitializeAerialCityRAG; otherwise initialize the corresponding folder first. TopK defaults to 10.";
        }

        public FrameworkElement? CreateInvocationPresentation(FerritaToolInvocationPresentationContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            return ToolInvocationCardFactory.Create(
                context,
                [
                    new ToolInvocationCardFieldDefinition("Search path", "SearchPath", "Waiting for search path..."),
                    new ToolInvocationCardFieldDefinition("Query", "Query", "Waiting for keyword query..."),
                    new ToolInvocationCardFieldDefinition("Top K", "TopK", "Default 10"),
                    new ToolInvocationCardFieldDefinition("Min score", "MinScore", "Optional")
                ]);
        }

        public async Task<FerritaToolResult> ExecuteAsync(
            FerritaToolContext context,
            FerritaToolArguments arguments,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await _ragService.KeywordSearchAsync(
                    arguments.GetString("SearchPath") ?? string.Empty,
                    arguments.GetString("Query") ?? string.Empty,
                    context.WorkspacePath,
                    arguments.GetInteger("TopK", 10),
                    (float)arguments.GetNumber("MinScore", decimal.MinValue),
                    cancellationToken).ConfigureAwait(false);

                return FerritaToolResult.Success(
                    AerialCityRagService.FormatSearchResult(result),
                    new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase)
                    {
                        ["searchPath"] = result.SearchPath,
                        ["targetDirectory"] = result.TargetDirectory,
                        ["databasePath"] = result.DatabasePath,
                        ["method"] = result.Method.ToString(),
                        ["query"] = result.Query,
                        ["topK"] = result.TopK,
                        ["resultCount"] = result.Results.Count
                    });
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex) when (ex is InvalidOperationException or IOException or UnauthorizedAccessException or ArgumentException or NotSupportedException)
            {
                return FerritaToolResult.Failure($"Keyword search failed: {ex.Message}");
            }
        }
    }
}
