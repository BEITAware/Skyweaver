using System.Globalization;
using System.IO;
using System.Text;
using System.Text.Json;
using AerialCity;
using AerialCity.Core.Exceptions;
using AerialCity.Core.Primitives;
using AerialCity.Core.Storage;
using AerialCity.Database;
using AerialCity.Delegates;
using AerialCity.Embedding;
using AerialCity.Retrieval;
using AerialCity.Segmentation;
using Skyweaver.Controls.EmbeddingModelConfigurationControl.Models;
using Skyweaver.Controls.EmbeddingModelConfigurationControl.Services;
using Skyweaver.Models.AerialCityRag;
using Skyweaver.Services.Directories;
using Skyweaver.Services.SkyweaverTools;

namespace Skyweaver.Services.AerialCityRag
{
    public sealed class AerialCityRagService
    {
        private const int DefaultTopK = 10;
        private const int MaximumTopK = 50;
        private const int DefaultMaximumFileBytes = 10 * 1024 * 1024;
        private const int MaximumRecordedFailures = 20;
        private const int MaximumProgressActiveItems = 8;
        private const string FileHashAlgorithm = "fnv1a64";
        private const double DefaultSegmentationBudgetRatio = 0.75d;

        private static readonly HashSet<string> s_codeExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".bat", ".c", ".cc", ".clj", ".cpp", ".cs", ".cshtml", ".csproj", ".css", ".fs", ".fsx",
            ".go", ".h", ".hpp", ".html", ".java", ".js", ".json", ".jsx", ".kt", ".kts", ".lua",
            ".php", ".ps1", ".py", ".razor", ".rb", ".rs", ".sh", ".sql", ".swift", ".ts", ".tsx",
            ".vb", ".xaml", ".xml", ".yaml", ".yml"
        };

        private static readonly HashSet<string> s_imageExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".bmp", ".gif", ".jpeg", ".jpg", ".png", ".tif", ".tiff", ".webp"
        };

        private static readonly HashSet<string> s_codeModeExcludedDirectoryNames = new(StringComparer.OrdinalIgnoreCase)
        {
            ".git", ".hg", ".svn", "CVS", "bin", "obj", "node_modules", "bower_components",
            "packages", "artifacts", "dist", "build", "out", "target", "coverage",
            "TestResults", "__pycache__", "venv", "env", "site-packages", "DerivedData"
        };

        private static readonly HashSet<string> s_codeModeExcludedFileNames = new(StringComparer.OrdinalIgnoreCase)
        {
            ".DS_Store", "Thumbs.db", "ehthumbs.db", "desktop.ini", "npm-debug.log",
            "yarn-error.log", "pnpm-debug.log"
        };

        private static readonly HashSet<string> s_codeModeExcludedFileExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".tmp", ".temp", ".log", ".binlog", ".cache", ".pdb", ".dll", ".exe",
            ".obj", ".o", ".so", ".dylib", ".class", ".pyc", ".pyo", ".ilk",
            ".ipch", ".pch", ".ncb", ".suo", ".user", ".aps", ".bak", ".orig",
            ".rej", ".swp", ".swo", ".zip", ".7z", ".rar", ".tar", ".gz",
            ".tgz", ".bz2", ".xz", ".nupkg", ".snupkg", ".jar", ".war", ".ear",
            ".db", ".sqlite", ".sqlite3", ".map"
        };

        private readonly AerialCityRagConfigurationRepository _configurationRepository;
        private readonly AerialCityRagRegistry _registry;
        private readonly AerialCityRagExcludedFilesStore _excludedFilesStore;
        private readonly EmbeddingModelConfigurationRepository _embeddingModelRepository;
        private readonly EmbeddingModelService _embeddingModelService;

        public AerialCityRagService()
            : this(
                new AerialCityRagConfigurationRepository(),
                new AerialCityRagRegistry(),
                new AerialCityRagExcludedFilesStore(),
                new EmbeddingModelConfigurationRepository(new EmbeddingModelConfigurationPathProvider()),
                new EmbeddingModelService())
        {
        }

        internal AerialCityRagService(
            AerialCityRagConfigurationRepository configurationRepository,
            AerialCityRagRegistry registry,
            AerialCityRagExcludedFilesStore excludedFilesStore,
            EmbeddingModelConfigurationRepository embeddingModelRepository,
            EmbeddingModelService embeddingModelService)
        {
            _configurationRepository = configurationRepository ?? throw new ArgumentNullException(nameof(configurationRepository));
            _registry = registry ?? throw new ArgumentNullException(nameof(registry));
            _excludedFilesStore = excludedFilesStore ?? throw new ArgumentNullException(nameof(excludedFilesStore));
            _embeddingModelRepository = embeddingModelRepository ?? throw new ArgumentNullException(nameof(embeddingModelRepository));
            _embeddingModelService = embeddingModelService ?? throw new ArgumentNullException(nameof(embeddingModelService));
        }

        public async Task<AerialCityRagInitializationResult> InitializeAsync(
            string requestedTargetDirectory,
            string? workspacePath,
            string? mode = null,
            int maximumFileBytes = DefaultMaximumFileBytes,
            CancellationToken cancellationToken = default,
            Func<SkyweaverToolProgressUpdate, CancellationToken, ValueTask>? progressReporter = null)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var configuration = RequireEnabledConfiguration();
            var model = ResolveSelectedEmbeddingModel(configuration);
            var indexMode = AerialCityRagIndexModeSupport.Parse(mode);
            var targetDirectory = ResolveExistingDirectory(requestedTargetDirectory, workspacePath);
            var aerialCityDirectory = SkyweaverDirectoryRuntime.Instance.AerialCityDirectoryPath;
            Directory.CreateDirectory(aerialCityDirectory);

            maximumFileBytes = maximumFileBytes <= 0 ? int.MaxValue : maximumFileBytes;

            var databaseFolderName = AerialCityRagRegistry.CreateDatabaseFolderName(targetDirectory);
            var databasePath = Path.Combine(aerialCityDirectory, databaseFolderName);
            var now = DateTimeOffset.UtcNow;
            var existingMapping = _registry.Load(aerialCityDirectory).FirstOrDefault(mapping =>
                string.Equals(
                    AerialCityRagRegistry.NormalizePath(mapping.TargetPath),
                    targetDirectory,
                    StringComparison.OrdinalIgnoreCase));

            var mapping = new AerialCityRagFolderMapping
            {
                TargetPath = targetDirectory,
                DatabaseFolderName = databaseFolderName,
                DatabasePath = databasePath,
                EmbeddingModelKey = model.Key,
                EmbeddingModelDisplayName = model.DisplayName,
                EmbeddingModelId = model.SummaryModelId,
                EmbeddingInterfaceType = model.InterfaceType,
                EmbeddingDimensions = model.Dimensions,
                SupportsMultimodalEmbedding = model.SupportsMultimodalEmbedding,
                InitializedAtUtc = existingMapping?.InitializedAtUtc == default || existingMapping == null
                    ? now
                    : existingMapping.InitializedAtUtc,
                UpdatedAtUtc = now,
                FileSnapshots = existingMapping?.FileSnapshots ?? []
            };

            _registry.Upsert(aerialCityDirectory, mapping);

            var updateResult = await UpdateMappingScopeAsync(
                aerialCityDirectory,
                mapping,
                targetDirectory,
                model,
                indexMode,
                configuration.EmbeddingConcurrency,
                maximumFileBytes,
                cancellationToken,
                progressReporter).ConfigureAwait(false);

            return new AerialCityRagInitializationResult
            {
                TargetDirectory = targetDirectory,
                AerialCityDirectory = aerialCityDirectory,
                RegistryFilePath = _registry.GetRegistryFilePath(aerialCityDirectory),
                DatabasePath = databasePath,
                DatabaseFolderName = databaseFolderName,
                Mode = indexMode,
                ExcludedFilesPath = _excludedFilesStore.GetExcludedFilesPath(databasePath),
                ExcludedFileCount = updateResult.ExcludedFileCount,
                EmbeddingModelDisplayName = model.DisplayName,
                EmbeddingModelId = model.SummaryModelId,
                SupportsMultimodalEmbedding = model.SupportsMultimodalEmbedding,
                Statistics = AerialCityRagInitializationStatistics.FromUpdateStatistics(updateResult.Statistics)
            };
        }

        public Task<AerialCityRagSearchResult> SemanticSearchAsync(
            string requestedSearchPath,
            string query,
            string? workspacePath,
            int topK = DefaultTopK,
            float minScore = float.NegativeInfinity,
            CancellationToken cancellationToken = default)
        {
            return SearchAsync(
                requestedSearchPath,
                query,
                workspacePath,
                RetrievalMethod.Cosine,
                topK,
                minScore,
                cancellationToken);
        }

        public Task<AerialCityRagSearchResult> KeywordSearchAsync(
            string requestedSearchPath,
            string query,
            string? workspacePath,
            int topK = DefaultTopK,
            float minScore = float.NegativeInfinity,
            CancellationToken cancellationToken = default)
        {
            return SearchAsync(
                requestedSearchPath,
                query,
                workspacePath,
                RetrievalMethod.BM25,
                topK,
                minScore,
                cancellationToken);
        }

        public async Task<AerialCityRagUpdateResult> UpdateDatabaseAsync(
            string requestedFolderPath,
            string? workspacePath,
            int maximumFileBytes = DefaultMaximumFileBytes,
            CancellationToken cancellationToken = default,
            Func<SkyweaverToolProgressUpdate, CancellationToken, ValueTask>? progressReporter = null)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var configuration = RequireEnabledConfiguration();

            var requestedPath = AerialCityRagRegistry.NormalizePath(ResolvePath(requestedFolderPath, workspacePath));
            var aerialCityDirectory = SkyweaverDirectoryRuntime.Instance.AerialCityDirectoryPath;
            var mapping = _registry.FindBestMappingForPath(aerialCityDirectory, requestedPath);
            if (mapping == null)
            {
                throw new InvalidOperationException(
                    $"FolderPath is not inside any initialized AerialCity folder: {requestedPath}. Run InitializeAerialCityRAG for the corresponding folder first.");
            }

            var model = ResolveEmbeddingModelForMapping(mapping);
            var excludedFiles = _excludedFilesStore.Load(mapping.DatabasePath);
            var indexMode = excludedFiles.Mode;
            maximumFileBytes = maximumFileBytes <= 0 ? int.MaxValue : maximumFileBytes;

            if (File.Exists(requestedPath))
            {
                return await UpdateSingleFileWithModeAsync(
                    aerialCityDirectory,
                    mapping,
                    requestedPath,
                    model,
                    indexMode,
                    configuration.EmbeddingConcurrency,
                    maximumFileBytes,
                    cancellationToken,
                    progressReporter).ConfigureAwait(false);
            }

            return await UpdateMappingScopeAsync(
                aerialCityDirectory,
                mapping,
                requestedPath,
                model,
                indexMode,
                configuration.EmbeddingConcurrency,
                maximumFileBytes,
                cancellationToken,
                progressReporter).ConfigureAwait(false);
        }

        public async Task<AerialCityRagFileSyncResult> RefreshFileAfterMutationAsync(
            string requestedFilePath,
            string? workspacePath,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var configuration = _configurationRepository.Load();
            if (!configuration.IsEnabled)
            {
                return AerialCityRagFileSyncResult.NoOp("AerialCity RAG is disabled.");
            }

            var aerialCityDirectory = SkyweaverDirectoryRuntime.Instance.AerialCityDirectoryPath;
            if (string.IsNullOrWhiteSpace(aerialCityDirectory))
            {
                return AerialCityRagFileSyncResult.NoOp("AerialCity directory is not configured.");
            }

            var filePath = AerialCityRagRegistry.NormalizePath(ResolvePath(requestedFilePath, workspacePath));
            var mapping = _registry.FindBestMappingForPath(aerialCityDirectory, filePath);
            if (mapping == null)
            {
                return AerialCityRagFileSyncResult.NoOp("File is not inside an initialized AerialCity folder.");
            }

            var excludedFiles = _excludedFilesStore.Load(mapping.DatabasePath);
            _excludedFilesStore.Save(mapping.DatabasePath, excludedFiles);
            var relativePath = GetRelativePath(mapping.TargetPath, filePath);
            var exclusion = GetModeExclusionForFile(filePath, mapping.TargetPath, excludedFiles.Mode);
            if (excludedFiles.Contains(relativePath) || exclusion.Excluded)
            {
                if (exclusion.Excluded && File.Exists(filePath))
                {
                    _excludedFilesStore.Save(
                        mapping.DatabasePath,
                        AddExcludedFile(excludedFiles, relativePath, exclusion.Reason));
                }

                try
                {
                    var model = ResolveEmbeddingModelForMapping(mapping);
                    var result = await UpdateSingleFileAsync(
                        aerialCityDirectory,
                        mapping,
                        filePath,
                        model,
                        configuration.EmbeddingConcurrency,
                        DefaultMaximumFileBytes,
                        cancellationToken,
                        removeFromIndex: true).ConfigureAwait(false);

                    return AerialCityRagFileSyncResult.FromUpdate(filePath, result);
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception ex) when (ex is InvalidOperationException or IOException or UnauthorizedAccessException or ArgumentException or NotSupportedException or AerialCityException)
                {
                    return AerialCityRagFileSyncResult.Failed(filePath, mapping.DatabasePath, ex.Message);
                }
            }

            try
            {
                var model = ResolveEmbeddingModelForMapping(mapping);
                var result = await UpdateSingleFileAsync(
                    aerialCityDirectory,
                    mapping,
                    filePath,
                    model,
                    configuration.EmbeddingConcurrency,
                    DefaultMaximumFileBytes,
                    cancellationToken).ConfigureAwait(false);

                return AerialCityRagFileSyncResult.FromUpdate(filePath, result);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex) when (ex is InvalidOperationException or IOException or UnauthorizedAccessException or ArgumentException or NotSupportedException or AerialCityException)
            {
                return AerialCityRagFileSyncResult.Failed(filePath, mapping.DatabasePath, ex.Message);
            }
        }

        private async Task<AerialCityRagSearchResult> SearchAsync(
            string requestedSearchPath,
            string query,
            string? workspacePath,
            RetrievalMethod method,
            int topK,
            float minScore,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            RequireEnabledConfiguration();

            if (string.IsNullOrWhiteSpace(query))
            {
                throw new InvalidOperationException("Search query cannot be empty.");
            }

            var searchPath = ResolveExistingPath(requestedSearchPath, workspacePath);
            var aerialCityDirectory = SkyweaverDirectoryRuntime.Instance.AerialCityDirectoryPath;
            var mapping = _registry.FindBestMappingForPath(aerialCityDirectory, searchPath);
            if (mapping == null)
            {
                throw new InvalidOperationException(
                    $"SearchPath is not inside any initialized AerialCity folder: {searchPath}. Run InitializeAerialCityRAG for the corresponding folder first.");
            }

            var normalizedTopK = Math.Clamp(topK, 1, MaximumTopK);
            var candidateLimit = Math.Min(500, Math.Max(normalizedTopK * 10, normalizedTopK));
            var model = method == RetrievalMethod.BM25 ? null : ResolveEmbeddingModelForMapping(mapping);
            var request = CreateRetrievalRequest(mapping, model, method, query, candidateLimit, minScore);
            var retrieve = new ApiRetrievalService().CreateRetrievalDelegate();
            var rawResults = await retrieve(request, cancellationToken).ConfigureAwait(false);
            var filteredResults = rawResults
                .Where(result => IsResultInsideSearchPath(result, searchPath))
                .Take(normalizedTopK)
                .ToArray();

            return new AerialCityRagSearchResult
            {
                SearchPath = searchPath,
                TargetDirectory = mapping.TargetPath,
                DatabasePath = mapping.DatabasePath,
                Method = method,
                Query = query.Trim(),
                TopK = normalizedTopK,
                Results = filteredResults
            };
        }

        private async Task<AerialCityRagUpdateResult> UpdateMappingScopeAsync(
            string aerialCityDirectory,
            AerialCityRagFolderMapping mapping,
            string scopeDirectory,
            EmbeddingModelDefinition model,
            AerialCityRagIndexMode indexMode,
            int embeddingConcurrency,
            int maximumFileBytes,
            CancellationToken cancellationToken,
            Func<SkyweaverToolProgressUpdate, CancellationToken, ValueTask>? progressReporter = null)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using var engine = new AerialCityBuilder().Build();
            using var updateContext = await CreateUpdateContextAsync(
                engine,
                aerialCityDirectory,
                mapping,
                model,
                embeddingConcurrency,
                cancellationToken).ConfigureAwait(false);

            await using var database = updateContext.Database;
            var snapshots = mapping.FileSnapshots.ToDictionary(
                snapshot => snapshot.RelativePath,
                StringComparer.OrdinalIgnoreCase);
            var updatedSnapshots = new Dictionary<string, AerialCityRagFileSnapshot>(snapshots, StringComparer.OrdinalIgnoreCase);
            var seenRelativePaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var statistics = new AerialCityRagUpdateStatistics();
            var existingExcludedFiles = _excludedFilesStore.Load(mapping.DatabasePath);
            var enumeration = EnumerateFiles(scopeDirectory, mapping.TargetPath, indexMode, cancellationToken);
            var shouldHonorStoredExclusions = indexMode != AerialCityRagIndexMode.Everything &&
                existingExcludedFiles.Mode == indexMode;
            var excludedByStoredList = enumeration.IncludedFilePaths
                .Where(filePath => shouldHonorStoredExclusions &&
                    existingExcludedFiles.Contains(GetRelativePath(mapping.TargetPath, filePath)))
                .ToArray();
            var excludedByStoredRelativePaths = new HashSet<string>(
                excludedByStoredList.Select(filePath => GetRelativePath(mapping.TargetPath, filePath)),
                StringComparer.OrdinalIgnoreCase);
            var filePaths = enumeration.IncludedFilePaths
                .Where(filePath => !excludedByStoredRelativePaths.Contains(GetRelativePath(mapping.TargetPath, filePath)))
                .ToArray();
            var updatedExcludedFiles = MergeExcludedFiles(
                existingExcludedFiles,
                indexMode,
                mapping.TargetPath,
                scopeDirectory,
                enumeration.ExcludedFiles);
            _excludedFilesStore.Save(mapping.DatabasePath, updatedExcludedFiles);
            statistics.FilesExcludedByMode = enumeration.ExcludedFiles.Count + excludedByStoredList.Length;

            var fileOutcomes = await ProcessScopeFilesAsync(
                updateContext,
                snapshots,
                filePaths,
                maximumFileBytes,
                progressReporter,
                cancellationToken).ConfigureAwait(false);

            foreach (var outcome in fileOutcomes)
            {
                cancellationToken.ThrowIfCancellationRequested();

                seenRelativePaths.Add(outcome.RelativePath);
                MergeUpdateStatistics(statistics, outcome.Statistics);

                if (outcome.Snapshot != null)
                {
                    updatedSnapshots[outcome.RelativePath] = outcome.Snapshot;
                }
            }

            foreach (var staleSnapshot in snapshots.Values.Where(snapshot =>
                         !seenRelativePaths.Contains(snapshot.RelativePath) &&
                         IsSnapshotInsideScope(mapping.TargetPath, snapshot.RelativePath, scopeDirectory)).ToArray())
            {
                cancellationToken.ThrowIfCancellationRequested();
                var deletedPath = Path.Combine(mapping.TargetPath, staleSnapshot.RelativePath.Replace('/', Path.DirectorySeparatorChar));
                var deletedSegments = await DeleteSegmentsForSourceAsync(
                    updateContext,
                    deletedPath,
                    cancellationToken).ConfigureAwait(false);

                updatedSnapshots.Remove(staleSnapshot.RelativePath);
                statistics.FilesRemoved++;
                statistics.SegmentsDeleted += deletedSegments;
            }

            var updatedMapping = CloneMappingWithSnapshots(
                mapping,
                updatedSnapshots.Values
                    .OrderBy(snapshot => snapshot.RelativePath, StringComparer.OrdinalIgnoreCase)
                    .ToArray(),
                DateTimeOffset.UtcNow);

            _registry.Upsert(aerialCityDirectory, updatedMapping);

            return new AerialCityRagUpdateResult
            {
                RequestedDirectory = scopeDirectory,
                TargetDirectory = mapping.TargetPath,
                AerialCityDirectory = aerialCityDirectory,
                RegistryFilePath = _registry.GetRegistryFilePath(aerialCityDirectory),
                DatabasePath = mapping.DatabasePath,
                DatabaseFolderName = mapping.DatabaseFolderName,
                Mode = indexMode,
                ExcludedFilesPath = _excludedFilesStore.GetExcludedFilesPath(mapping.DatabasePath),
                ExcludedFileCount = updatedExcludedFiles.Files.Count,
                EmbeddingModelDisplayName = mapping.EmbeddingModelDisplayName,
                EmbeddingModelId = mapping.EmbeddingModelId,
                SupportsMultimodalEmbedding = mapping.SupportsMultimodalEmbedding,
                Statistics = statistics
            };
        }

        private static async Task<IReadOnlyList<AerialCityRagFileUpdateOutcome>> ProcessScopeFilesAsync(
            AerialCityRagDatabaseUpdateContext context,
            IReadOnlyDictionary<string, AerialCityRagFileSnapshot> snapshots,
            IReadOnlyList<string> filePaths,
            int maximumFileBytes,
            Func<SkyweaverToolProgressUpdate, CancellationToken, ValueTask>? progressReporter,
            CancellationToken cancellationToken)
        {
            await ReportToolProgressAsync(
                progressReporter,
                "AerialCity 嵌入",
                filePaths.Count == 0 ? "没有需要嵌入的文件。" : "准备嵌入文件。",
                completedItems: 0,
                totalItems: filePaths.Count,
                activeItems: Array.Empty<string>(),
                isCompleted: filePaths.Count == 0,
                cancellationToken).ConfigureAwait(false);

            if (filePaths.Count == 0)
            {
                return [];
            }

            var activeFiles = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var activeFilesLock = new object();
            var completedFiles = 0;

            if (context.EmbeddingConcurrency <= 1 || filePaths.Count == 1)
            {
                var sequential = new List<AerialCityRagFileUpdateOutcome>(filePaths.Count);
                foreach (var filePath in filePaths)
                {
                    sequential.Add(await ProcessScopeFileWithProgressAsync(
                        context,
                        snapshots,
                        filePath,
                        maximumFileBytes,
                        cancellationToken).ConfigureAwait(false));
                }

                return sequential;
            }

            var outcomes = new AerialCityRagFileUpdateOutcome[filePaths.Count];
            var workerCount = Math.Min(context.EmbeddingConcurrency, filePaths.Count);
            var nextIndex = -1;
            var workers = Enumerable.Range(0, workerCount).Select(_ => ProcessWorkerAsync()).ToArray();
            await Task.WhenAll(workers).ConfigureAwait(false);
            return outcomes;

            async Task ProcessWorkerAsync()
            {
                while (true)
                {
                    var fileIndex = Interlocked.Increment(ref nextIndex);
                    if (fileIndex >= filePaths.Count)
                    {
                        return;
                    }

                    outcomes[fileIndex] = await ProcessScopeFileWithProgressAsync(
                        context,
                        snapshots,
                        filePaths[fileIndex],
                        maximumFileBytes,
                        cancellationToken).ConfigureAwait(false);
                }
            }

            async Task<AerialCityRagFileUpdateOutcome> ProcessScopeFileWithProgressAsync(
                AerialCityRagDatabaseUpdateContext updateContext,
                IReadOnlyDictionary<string, AerialCityRagFileSnapshot> updateSnapshots,
                string filePath,
                int fileMaximumFileBytes,
                CancellationToken token)
            {
                var relativePath = GetRelativePath(updateContext.Mapping.TargetPath, filePath);
                string[] activeSnapshot;
                lock (activeFilesLock)
                {
                    activeFiles[filePath] = relativePath;
                    activeSnapshot = activeFiles.Values
                        .OrderBy(item => item, StringComparer.OrdinalIgnoreCase)
                        .Take(MaximumProgressActiveItems)
                        .ToArray();
                }

                await ReportToolProgressAsync(
                    progressReporter,
                    "AerialCity 嵌入",
                    "正在嵌入文件。",
                    completedFiles,
                    filePaths.Count,
                    activeSnapshot,
                    isCompleted: false,
                    token).ConfigureAwait(false);

                try
                {
                    return await ProcessScopeFileAsync(
                        updateContext,
                        updateSnapshots,
                        filePath,
                        fileMaximumFileBytes,
                        token).ConfigureAwait(false);
                }
                finally
                {
                    var completed = Interlocked.Increment(ref completedFiles);
                    lock (activeFilesLock)
                    {
                        activeFiles.Remove(filePath);
                        activeSnapshot = activeFiles.Values
                            .OrderBy(item => item, StringComparer.OrdinalIgnoreCase)
                            .Take(MaximumProgressActiveItems)
                            .ToArray();
                    }

                    await ReportToolProgressAsync(
                        progressReporter,
                        "AerialCity 嵌入",
                        completed >= filePaths.Count ? "文件嵌入完成。" : "正在嵌入文件。",
                        completed,
                        filePaths.Count,
                        activeSnapshot,
                        isCompleted: completed >= filePaths.Count,
                        token).ConfigureAwait(false);
                }
            }
        }

        private static async Task<AerialCityRagFileUpdateOutcome> ProcessScopeFileAsync(
            AerialCityRagDatabaseUpdateContext context,
            IReadOnlyDictionary<string, AerialCityRagFileSnapshot> snapshots,
            string filePath,
            int maximumFileBytes,
            CancellationToken cancellationToken)
        {
            var statistics = new AerialCityRagUpdateStatistics
            {
                FilesVisited = 1
            };
            var relativePath = GetRelativePath(context.Mapping.TargetPath, filePath);

            try
            {
                var currentSnapshot = await CreateFileSnapshotAsync(
                    filePath,
                    context.Mapping.TargetPath,
                    embedded: false,
                    segmentCount: 0,
                    sourceType: string.Empty,
                    cancellationToken).ConfigureAwait(false);

                snapshots.TryGetValue(relativePath, out var previousSnapshot);
                if (IsSnapshotCurrent(previousSnapshot, currentSnapshot))
                {
                    statistics.FilesSkippedAlreadyIndexed++;
                    return new AerialCityRagFileUpdateOutcome(
                        relativePath,
                        CarryForwardEmbeddingState(currentSnapshot, previousSnapshot!),
                        statistics);
                }

                var refreshedSnapshot = await RefreshExistingFileAsync(
                    context,
                    filePath,
                    maximumFileBytes,
                    statistics,
                    cancellationToken).ConfigureAwait(false);

                statistics.FilesReembedded++;
                return new AerialCityRagFileUpdateOutcome(relativePath, refreshedSnapshot, statistics);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or InvalidOperationException or ArgumentException or NotSupportedException or AerialCityException)
            {
                statistics.FilesFailed++;
                if (statistics.Failures.Count < MaximumRecordedFailures)
                {
                    statistics.Failures.Add($"{filePath}: {ex.Message}");
                }

                return new AerialCityRagFileUpdateOutcome(relativePath, Snapshot: null, statistics);
            }
        }

        private async Task<AerialCityRagUpdateResult> UpdateSingleFileWithModeAsync(
            string aerialCityDirectory,
            AerialCityRagFolderMapping mapping,
            string filePath,
            EmbeddingModelDefinition model,
            AerialCityRagIndexMode indexMode,
            int embeddingConcurrency,
            int maximumFileBytes,
            CancellationToken cancellationToken,
            Func<SkyweaverToolProgressUpdate, CancellationToken, ValueTask>? progressReporter = null)
        {
            var existingExcludedFiles = _excludedFilesStore.Load(mapping.DatabasePath);
            var relativePath = GetRelativePath(mapping.TargetPath, filePath);
            var exclusion = GetModeExclusionForFile(filePath, mapping.TargetPath, indexMode);
            var storedExclusionApplies = indexMode != AerialCityRagIndexMode.Everything &&
                existingExcludedFiles.Mode == indexMode &&
                existingExcludedFiles.Contains(relativePath);
            var updatedExcludedFiles = MergeSingleExcludedFile(
                existingExcludedFiles,
                indexMode,
                relativePath,
                exclusion);

            _excludedFilesStore.Save(mapping.DatabasePath, updatedExcludedFiles);

            return await UpdateSingleFileAsync(
                aerialCityDirectory,
                mapping,
                filePath,
                model,
                embeddingConcurrency,
                maximumFileBytes,
                cancellationToken,
                removeFromIndex: storedExclusionApplies || exclusion.Excluded,
                progressReporter: progressReporter).ConfigureAwait(false);
        }

        private async Task<AerialCityRagUpdateResult> UpdateSingleFileAsync(
            string aerialCityDirectory,
            AerialCityRagFolderMapping mapping,
            string filePath,
            EmbeddingModelDefinition model,
            int embeddingConcurrency,
            int maximumFileBytes,
            CancellationToken cancellationToken,
            bool removeFromIndex = false,
            Func<SkyweaverToolProgressUpdate, CancellationToken, ValueTask>? progressReporter = null)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using var engine = new AerialCityBuilder().Build();
            using var updateContext = await CreateUpdateContextAsync(
                engine,
                aerialCityDirectory,
                mapping,
                model,
                embeddingConcurrency,
                cancellationToken).ConfigureAwait(false);

            await using var database = updateContext.Database;
            var snapshots = mapping.FileSnapshots.ToDictionary(
                snapshot => snapshot.RelativePath,
                StringComparer.OrdinalIgnoreCase);
            var updatedSnapshots = new Dictionary<string, AerialCityRagFileSnapshot>(snapshots, StringComparer.OrdinalIgnoreCase);
            var statistics = new AerialCityRagUpdateStatistics();
            var relativePath = GetRelativePath(mapping.TargetPath, filePath);
            var excludedFiles = _excludedFilesStore.Load(mapping.DatabasePath);

            await ReportToolProgressAsync(
                progressReporter,
                "AerialCity 嵌入",
                removeFromIndex || !File.Exists(filePath) ? "正在从索引中移除文件。" : "正在嵌入文件。",
                completedItems: 0,
                totalItems: 1,
                activeItems: [relativePath],
                isCompleted: false,
                cancellationToken).ConfigureAwait(false);

            if (removeFromIndex || !File.Exists(filePath))
            {
                var deletedSegments = await DeleteSegmentsForSourceAsync(
                    updateContext,
                    filePath,
                    cancellationToken).ConfigureAwait(false);

                updatedSnapshots.Remove(relativePath);
                statistics.FilesRemoved++;
                statistics.SegmentsDeleted += deletedSegments;
                if (removeFromIndex)
                {
                    statistics.FilesExcludedByMode++;
                }
            }
            else
            {
                statistics.FilesVisited++;
                var refreshedSnapshot = await RefreshExistingFileAsync(
                    updateContext,
                    filePath,
                    maximumFileBytes,
                    statistics,
                    cancellationToken).ConfigureAwait(false);
                updatedSnapshots[relativePath] = refreshedSnapshot;
                statistics.FilesReembedded++;
            }

            await ReportToolProgressAsync(
                progressReporter,
                "AerialCity 嵌入",
                "文件嵌入完成。",
                completedItems: 1,
                totalItems: 1,
                activeItems: Array.Empty<string>(),
                isCompleted: true,
                cancellationToken).ConfigureAwait(false);

            var updatedMapping = CloneMappingWithSnapshots(
                mapping,
                updatedSnapshots.Values
                    .OrderBy(snapshot => snapshot.RelativePath, StringComparer.OrdinalIgnoreCase)
                    .ToArray(),
                DateTimeOffset.UtcNow);

            _registry.Upsert(aerialCityDirectory, updatedMapping);

            return new AerialCityRagUpdateResult
            {
                RequestedDirectory = filePath,
                TargetDirectory = mapping.TargetPath,
                AerialCityDirectory = aerialCityDirectory,
                RegistryFilePath = _registry.GetRegistryFilePath(aerialCityDirectory),
                DatabasePath = mapping.DatabasePath,
                DatabaseFolderName = mapping.DatabaseFolderName,
                Mode = excludedFiles.Mode,
                ExcludedFilesPath = _excludedFilesStore.GetExcludedFilesPath(mapping.DatabasePath),
                ExcludedFileCount = excludedFiles.Files.Count,
                EmbeddingModelDisplayName = mapping.EmbeddingModelDisplayName,
                EmbeddingModelId = mapping.EmbeddingModelId,
                SupportsMultimodalEmbedding = mapping.SupportsMultimodalEmbedding,
                Statistics = statistics
            };
        }

        private async Task<AerialCityRagDatabaseUpdateContext> CreateUpdateContextAsync(
            AerialCityEngine engine,
            string aerialCityDirectory,
            AerialCityRagFolderMapping mapping,
            EmbeddingModelDefinition model,
            int embeddingConcurrency,
            CancellationToken cancellationToken)
        {
            var createDatabase = engine.CreateDatabase();
            var database = await createDatabase(new DatabaseOptions
            {
                Name = mapping.DatabaseFolderName,
                Storage = new StorageOptions
                {
                    BasePath = aerialCityDirectory
                }
            }, cancellationToken).ConfigureAwait(false);

            return new AerialCityRagDatabaseUpdateContext(
                mapping,
                model,
                database,
                CreateEmbeddingTemplate(model, EmbeddingInput.FromText("AerialCity RAG update")),
                engine.EmbedContent(),
                engine.Insert(),
                engine.Update(),
                engine.Delete(),
                NormalizeEmbeddingConcurrency(embeddingConcurrency));
        }

        private static async Task<AerialCityRagFileSnapshot> RefreshExistingFileAsync(
            AerialCityRagDatabaseUpdateContext context,
            string filePath,
            int maximumFileBytes,
            AerialCityRagUpdateStatistics statistics,
            CancellationToken cancellationToken)
        {
            if (IsFileTooLarge(filePath, maximumFileBytes))
            {
                statistics.FilesSkippedTooLarge++;
                var deleted = await DeleteSegmentsForSourceAsync(
                    context,
                    filePath,
                    cancellationToken).ConfigureAwait(false);
                statistics.SegmentsDeleted += deleted;

                return await CreateFileSnapshotAsync(
                    filePath,
                    context.Mapping.TargetPath,
                    embedded: false,
                    segmentCount: 0,
                    sourceType: "too-large",
                    cancellationToken).ConfigureAwait(false);
            }

            var extension = Path.GetExtension(filePath);
            var probe = await ProbeFileAsync(filePath, cancellationToken).ConfigureAwait(false);
            if (probe.IsBinary)
            {
                if (!context.Model.SupportsMultimodalEmbedding || !s_imageExtensions.Contains(extension))
                {
                    statistics.BinaryFilesSkipped++;
                    var deleted = await DeleteSegmentsForSourceAsync(
                        context,
                        filePath,
                        cancellationToken).ConfigureAwait(false);
                    statistics.SegmentsDeleted += deleted;

                    return await CreateFileSnapshotAsync(
                        filePath,
                        context.Mapping.TargetPath,
                        embedded: false,
                        segmentCount: 0,
                        sourceType: "binary",
                        cancellationToken).ConfigureAwait(false);
                }

                var oldDeleted = await DeleteSegmentsForSourceAsync(
                    context,
                    filePath,
                    cancellationToken).ConfigureAwait(false);
                statistics.SegmentsDeleted += oldDeleted;

                var inserted = await EmbedAndInsertImageAsync(
                    context,
                    filePath,
                    cancellationToken).ConfigureAwait(false);

                statistics.ImageFilesEmbedded++;
                statistics.SegmentsInserted += inserted;

                return await CreateFileSnapshotAsync(
                    filePath,
                    context.Mapping.TargetPath,
                    embedded: inserted > 0,
                    segmentCount: inserted,
                    sourceType: "image",
                    cancellationToken).ConfigureAwait(false);
            }

            var sourceType = s_codeExtensions.Contains(extension) ? "code" : "text";
            try
            {
                var segmentCount = await RefreshTextOrCodeSegmentsAsync(
                    context,
                    filePath,
                    probe.Encoding,
                    sourceType,
                    statistics,
                    cancellationToken).ConfigureAwait(false);

                if (sourceType == "code")
                {
                    statistics.CodeFilesEmbedded++;
                }
                else
                {
                    statistics.TextFilesEmbedded++;
                }

                return await CreateFileSnapshotAsync(
                    filePath,
                    context.Mapping.TargetPath,
                    embedded: segmentCount > 0,
                    segmentCount: segmentCount,
                    sourceType: sourceType,
                    cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex) when (sourceType == "code" &&
                ex is InvalidOperationException or ArgumentException or NotSupportedException or SegmentationException)
            {
                statistics.CodeFilesFellBackToText++;
                var segmentCount = await RefreshTextOrCodeSegmentsAsync(
                    context,
                    filePath,
                    probe.Encoding,
                    "text",
                    statistics,
                    cancellationToken).ConfigureAwait(false);

                statistics.TextFilesEmbedded++;
                return await CreateFileSnapshotAsync(
                    filePath,
                    context.Mapping.TargetPath,
                    embedded: segmentCount > 0,
                    segmentCount: segmentCount,
                    sourceType: "text",
                    cancellationToken).ConfigureAwait(false);
            }
        }

        private static async Task<int> RefreshTextOrCodeSegmentsAsync(
            AerialCityRagDatabaseUpdateContext context,
            string filePath,
            Encoding? encoding,
            string sourceType,
            AerialCityRagUpdateStatistics statistics,
            CancellationToken cancellationToken)
        {
            var segments = await SegmentFileAsync(
                filePath,
                context.Mapping.TargetPath,
                encoding,
                sourceType,
                context.Model.MaxInputTokens,
                cancellationToken).ConfigureAwait(false);

            var oldSegments = await LoadIndexedSegmentsForSourceAsync(
                context.Mapping.DatabasePath,
                filePath,
                cancellationToken).ConfigureAwait(false);

            var oldByKey = new Dictionary<string, Queue<IndexedSegment>>(StringComparer.Ordinal);
            foreach (var oldSegment in oldSegments.OrderBy(segment => segment.StartOffset).ThenBy(segment => segment.EndOffset))
            {
                var key = CreateSegmentMatchKey(oldSegment.Kind, oldSegment.Content);
                if (!oldByKey.TryGetValue(key, out var queue))
                {
                    queue = new Queue<IndexedSegment>();
                    oldByKey[key] = queue;
                }

                queue.Enqueue(oldSegment);
            }

            var matchedOldIds = new HashSet<AerialId>();
            var pendingChanges = new List<PendingSegmentChange>();
            foreach (var segment in segments)
            {
                cancellationToken.ThrowIfCancellationRequested();
                EnrichSegment(segment, filePath, context.Mapping.TargetPath, sourceType);

                var key = CreateSegmentMatchKey(segment.Kind, segment.Content);
                var matched = TryDequeueMatchingSegment(oldByKey, key);
                if (matched == null)
                {
                    pendingChanges.Add(new PendingSegmentChange(segment, ExistingSegment: null, NeedsEmbedding: true));
                    continue;
                }

                matchedOldIds.Add(matched.Id);
                if (matched.Embedding.HasValue)
                {
                    segment.Embedding = matched.Embedding.Value;
                    if (IsStoredSegmentEquivalent(matched, segment))
                    {
                        statistics.SegmentsReused++;
                        continue;
                    }

                    pendingChanges.Add(new PendingSegmentChange(segment, matched, NeedsEmbedding: false));
                    continue;
                }

                pendingChanges.Add(new PendingSegmentChange(segment, matched, NeedsEmbedding: true));
            }

            await EmbedPendingSegmentsAsync(
                context,
                pendingChanges
                    .Where(change => change.NeedsEmbedding)
                    .Select(change => change.Segment)
                    .ToArray(),
                cancellationToken).ConfigureAwait(false);

            foreach (var change in pendingChanges)
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (change.ExistingSegment == null)
                {
                    await InsertSegmentSafelyAsync(context, change.Segment, cancellationToken).ConfigureAwait(false);
                    statistics.SegmentsInserted++;
                    continue;
                }

                await UpdateSegmentSafelyAsync(context, change.ExistingSegment.Id, change.Segment, cancellationToken).ConfigureAwait(false);
                statistics.SegmentsUpdated++;
            }

            foreach (var oldSegment in oldSegments)
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (matchedOldIds.Contains(oldSegment.Id))
                {
                    continue;
                }

                await DeleteSegmentSafelyAsync(context, oldSegment.Id, cancellationToken).ConfigureAwait(false);
                statistics.SegmentsDeleted++;
            }

            return segments.Count;
        }

        private static async Task<IReadOnlyList<Segment>> SegmentFileAsync(
            string filePath,
            string targetDirectory,
            Encoding? encoding,
            string sourceType,
            int maxInputTokens,
            CancellationToken cancellationToken)
        {
            var text = encoding is null
                ? await File.ReadAllTextAsync(filePath, cancellationToken).ConfigureAwait(false)
                : await File.ReadAllTextAsync(filePath, encoding, cancellationToken).ConfigureAwait(false);
            if (text.Length == 0)
            {
                return [];
            }

            var segmentationBudget = GetSegmentationTokenBudget(maxInputTokens > 0 ? maxInputTokens : 8192);
            var metadata = CreateFileMetadata(filePath, targetDirectory, sourceType);

            return sourceType == "code"
                ? TreeSitterCodeSegmenter.SegmentCode(
                    text,
                    ResolveLanguageHint(filePath),
                    filePath,
                    segmentationBudget,
                    metadata)
                : TextFileSegmenter.SegmentText(
                    text,
                    filePath,
                    segmentationBudget,
                    TextFileSegmenter.DefaultOverlapRatio,
                    metadata);
        }

        private static async Task EmbedSegmentAsync(
            EmbedContentDelegate embedContent,
            ApiEmbeddingRequest template,
            Segment segment,
            CancellationToken cancellationToken)
        {
            var request = new ApiEmbeddingRequest
            {
                ApiKey = template.ApiKey,
                BaseUrl = template.BaseUrl,
                ApiType = template.ApiType,
                Model = template.Model,
                Segment = segment,
                Parameters = new Dictionary<string, object?>(template.Parameters, StringComparer.Ordinal),
                Dimensions = template.Dimensions,
                Normalize = template.Normalize,
                IncludeBinaryDataInTextProjection = template.IncludeBinaryDataInTextProjection
            };

            await embedContent(request, cancellationToken).ConfigureAwait(false);
        }

        private static async Task EmbedPendingSegmentsAsync(
            AerialCityRagDatabaseUpdateContext context,
            IReadOnlyList<Segment> segments,
            CancellationToken cancellationToken)
        {
            if (segments.Count == 0)
            {
                return;
            }

            if (context.EmbeddingConcurrency <= 1)
            {
                foreach (var segment in segments)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    await EmbedSegmentAsync(
                        context.EmbedContent,
                        context.Template,
                        segment,
                        cancellationToken).ConfigureAwait(false);
                }

                return;
            }

            var tasks = segments.Select(EmbedOneAsync).ToArray();
            await Task.WhenAll(tasks).ConfigureAwait(false);

            async Task EmbedOneAsync(Segment segment)
            {
                await context.EmbeddingSemaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
                try
                {
                    await EmbedSegmentAsync(
                        context.EmbedContent,
                        context.Template,
                        segment,
                        cancellationToken).ConfigureAwait(false);
                }
                finally
                {
                    context.EmbeddingSemaphore.Release();
                }
            }
        }

        private static async Task InsertSegmentSafelyAsync(
            AerialCityRagDatabaseUpdateContext context,
            Segment segment,
            CancellationToken cancellationToken)
        {
            await context.DatabaseMutationLock.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                await context.Insert(context.Database, segment, cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                context.DatabaseMutationLock.Release();
            }
        }

        private static async Task UpdateSegmentSafelyAsync(
            AerialCityRagDatabaseUpdateContext context,
            AerialId id,
            Segment segment,
            CancellationToken cancellationToken)
        {
            await context.DatabaseMutationLock.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                await context.Update(context.Database, id, segment, cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                context.DatabaseMutationLock.Release();
            }
        }

        private static async Task DeleteSegmentSafelyAsync(
            AerialCityRagDatabaseUpdateContext context,
            AerialId id,
            CancellationToken cancellationToken)
        {
            await context.DatabaseMutationLock.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                await context.Delete(context.Database, id, cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                context.DatabaseMutationLock.Release();
            }
        }

        private AerialCityRagConfiguration RequireEnabledConfiguration()
        {
            var configuration = _configurationRepository.Load();
            if (!configuration.IsEnabled)
            {
                throw new InvalidOperationException("AerialCity RAG is disabled. Enable it in Preferences > Semantic Search first.");
            }

            var aerialCityDirectory = SkyweaverDirectoryRuntime.Instance.AerialCityDirectoryPath;
            if (string.IsNullOrWhiteSpace(aerialCityDirectory))
            {
                throw new InvalidOperationException("AerialCity directory is not configured. Set it in Preferences > Directory Locations first.");
            }

            return configuration;
        }

        private EmbeddingModelDefinition ResolveSelectedEmbeddingModel(AerialCityRagConfiguration configuration)
        {
            if (string.IsNullOrWhiteSpace(configuration.SelectedEmbeddingModelKey))
            {
                throw new InvalidOperationException("No embedding model is selected for AerialCity RAG.");
            }

            var model = _embeddingModelRepository.Load().FirstOrDefault(candidate =>
                string.Equals(candidate.Key, configuration.SelectedEmbeddingModelKey, StringComparison.Ordinal));
            if (model == null)
            {
                throw new InvalidOperationException("The selected embedding model no longer exists.");
            }

            if (!model.IsFullyConfigured)
            {
                throw new InvalidOperationException($"Embedding model '{model.DisplayName}' is not fully configured.");
            }

            return model;
        }

        private EmbeddingModelDefinition ResolveEmbeddingModelForMapping(AerialCityRagFolderMapping mapping)
        {
            var model = _embeddingModelRepository.Load().FirstOrDefault(candidate =>
                string.Equals(candidate.Key, mapping.EmbeddingModelKey, StringComparison.Ordinal));
            if (model == null)
            {
                throw new InvalidOperationException(
                    $"Embedding model used for this AerialCity database is no longer configured: {mapping.EmbeddingModelDisplayName}");
            }

            if (!model.IsFullyConfigured)
            {
                throw new InvalidOperationException($"Embedding model '{model.DisplayName}' is not fully configured.");
            }

            return model;
        }

        private ApiEmbeddingRequest CreateEmbeddingTemplate(EmbeddingModelDefinition model, EmbeddingInput input)
        {
            return _embeddingModelService.CreateRequest(model, input);
        }

        private ApiRetrievalRequest CreateRetrievalRequest(
            AerialCityRagFolderMapping mapping,
            EmbeddingModelDefinition? model,
            RetrievalMethod method,
            string query,
            int candidateLimit,
            float minScore)
        {
            if (method == RetrievalMethod.BM25)
            {
                return new ApiRetrievalRequest
                {
                    DatabasePath = mapping.DatabasePath,
                    Method = method,
                    TextQuery = query,
                    TopK = candidateLimit,
                    MinScore = minScore
                };
            }

            var template = CreateEmbeddingTemplate(model!, EmbeddingInput.FromText(query));
            return new ApiRetrievalRequest
            {
                ApiKey = template.ApiKey,
                BaseUrl = template.BaseUrl,
                ApiType = template.ApiType,
                Model = template.Model,
                Content = template.Content,
                Parameters = new Dictionary<string, object?>(template.Parameters, StringComparer.Ordinal),
                Dimensions = template.Dimensions,
                Normalize = template.Normalize,
                IncludeBinaryDataInTextProjection = template.IncludeBinaryDataInTextProjection,
                DatabasePath = mapping.DatabasePath,
                Method = method,
                TextQuery = query,
                TopK = candidateLimit,
                MinScore = minScore
            };
        }

        private static async Task<int> EmbedAndInsertCodeFileAsync(
            AerialCity.Delegates.EmbedCodeFileDelegate embedCodeFile,
            AerialCity.Delegates.EmbedTextFileDelegate embedTextFile,
            AerialCity.Delegates.InsertSegmentDelegate insert,
            AerialDatabase database,
            ApiEmbeddingRequest template,
            string filePath,
            string targetDirectory,
            Encoding? encoding,
            int maxInputTokens,
            AerialCityRagInitializationStatistics statistics,
            CancellationToken cancellationToken)
        {
            try
            {
                var request = new ApiCodeFileEmbeddingRequest
                {
                    ApiKey = template.ApiKey,
                    BaseUrl = template.BaseUrl,
                    ApiType = template.ApiType,
                    Model = template.Model,
                    FilePath = filePath,
                    SourceUri = filePath,
                    Language = ResolveLanguageHint(filePath),
                    FileEncoding = encoding,
                    MaxInputTokens = maxInputTokens > 0 ? maxInputTokens : 8192,
                    Parameters = new Dictionary<string, object?>(template.Parameters, StringComparer.Ordinal),
                    Dimensions = template.Dimensions,
                    Normalize = template.Normalize,
                    IncludeBinaryDataInTextProjection = template.IncludeBinaryDataInTextProjection,
                    Metadata = CreateFileMetadata(filePath, targetDirectory, "code")
                };

                var results = await embedCodeFile(request, cancellationToken).ConfigureAwait(false);
                return await InsertEmbeddingResultsAsync(insert, database, results, filePath, targetDirectory, "code", cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex) when (ex is InvalidOperationException or ArgumentException or NotSupportedException or SegmentationException)
            {
                statistics.CodeFilesFellBackToText++;
                return await EmbedAndInsertTextFileAsync(
                    embedTextFile,
                    insert,
                    database,
                    template,
                    filePath,
                    targetDirectory,
                    encoding,
                    maxInputTokens,
                    cancellationToken).ConfigureAwait(false);
            }
        }

        private static async Task<int> EmbedAndInsertTextFileAsync(
            AerialCity.Delegates.EmbedTextFileDelegate embedTextFile,
            AerialCity.Delegates.InsertSegmentDelegate insert,
            AerialDatabase database,
            ApiEmbeddingRequest template,
            string filePath,
            string targetDirectory,
            Encoding? encoding,
            int maxInputTokens,
            CancellationToken cancellationToken)
        {
            var request = new ApiTextFileEmbeddingRequest
            {
                ApiKey = template.ApiKey,
                BaseUrl = template.BaseUrl,
                ApiType = template.ApiType,
                Model = template.Model,
                FilePath = filePath,
                SourceUri = filePath,
                FileEncoding = encoding,
                MaxInputTokens = maxInputTokens > 0 ? maxInputTokens : 8192,
                Parameters = new Dictionary<string, object?>(template.Parameters, StringComparer.Ordinal),
                Dimensions = template.Dimensions,
                Normalize = template.Normalize,
                IncludeBinaryDataInTextProjection = template.IncludeBinaryDataInTextProjection,
                Metadata = CreateFileMetadata(filePath, targetDirectory, "text")
            };

            var results = await embedTextFile(request, cancellationToken).ConfigureAwait(false);
            return await InsertEmbeddingResultsAsync(insert, database, results, filePath, targetDirectory, "text", cancellationToken).ConfigureAwait(false);
        }

        private static async Task<int> EmbedAndInsertImageAsync(
            AerialCityRagDatabaseUpdateContext context,
            string filePath,
            CancellationToken cancellationToken)
        {
            var bytes = await File.ReadAllBytesAsync(filePath, cancellationToken).ConfigureAwait(false);
            var segment = new Segment(SegmentKind.Image, $"Image file: {Path.GetFileName(filePath)}\nPath: {filePath}")
            {
                BinaryContent = bytes,
                SourceUri = filePath,
                Metadata =
                {
                    ["sourceKind"] = "Image",
                    ["path"] = filePath,
                    ["relativePath"] = GetRelativePath(context.Mapping.TargetPath, filePath),
                    ["fileName"] = Path.GetFileName(filePath),
                    ["mimeType"] = ResolveImageMimeType(filePath)
                }
            };

            var request = new ApiEmbeddingRequest
            {
                ApiKey = context.Template.ApiKey,
                BaseUrl = context.Template.BaseUrl,
                ApiType = context.Template.ApiType,
                Model = context.Template.Model,
                Segment = segment,
                Parameters = new Dictionary<string, object?>(context.Template.Parameters, StringComparer.Ordinal),
                Dimensions = context.Template.Dimensions,
                Normalize = context.Template.Normalize,
                IncludeBinaryDataInTextProjection = context.Template.IncludeBinaryDataInTextProjection
            };

            await context.EmbeddingSemaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                await context.EmbedContent(request, cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                context.EmbeddingSemaphore.Release();
            }

            await InsertSegmentSafelyAsync(context, segment, cancellationToken).ConfigureAwait(false);
            return 1;
        }

        private static async Task<int> InsertEmbeddingResultsAsync(
            AerialCity.Delegates.InsertSegmentDelegate insert,
            AerialDatabase database,
            IReadOnlyList<EmbeddingResult> results,
            string filePath,
            string targetDirectory,
            string sourceType,
            CancellationToken cancellationToken)
        {
            var inserted = 0;
            foreach (var result in results)
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (result.Segment == null)
                {
                    continue;
                }

                EnrichSegment(result.Segment, filePath, targetDirectory, sourceType);
                await insert(database, result.Segment, cancellationToken).ConfigureAwait(false);
                inserted++;
            }

            return inserted;
        }

        private static void EnrichSegment(Segment segment, string filePath, string targetDirectory, string sourceType)
        {
            segment.Metadata["sourceContent"] = segment.Content;
            segment.Metadata["path"] = filePath;
            segment.Metadata["relativePath"] = GetRelativePath(targetDirectory, filePath);
            segment.Metadata["fileName"] = Path.GetFileName(filePath);
            segment.Metadata["ragSourceType"] = sourceType;
            segment.Metadata["ragSegmentHash"] = ComputeFastTextHash(segment.Content);
        }

        private static Dictionary<string, object> CreateFileMetadata(
            string filePath,
            string targetDirectory,
            string sourceType)
        {
            return new Dictionary<string, object>(StringComparer.Ordinal)
            {
                ["path"] = filePath,
                ["relativePath"] = GetRelativePath(targetDirectory, filePath),
                ["fileName"] = Path.GetFileName(filePath),
                ["ragSourceType"] = sourceType
            };
        }

        private static async Task<int> DeleteSegmentsForSourceAsync(
            AerialCityRagDatabaseUpdateContext context,
            string filePath,
            CancellationToken cancellationToken)
        {
            var oldSegments = await LoadIndexedSegmentsForSourceAsync(
                context.Mapping.DatabasePath,
                filePath,
                cancellationToken).ConfigureAwait(false);

            var deleted = 0;
            foreach (var segment in oldSegments)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await DeleteSegmentSafelyAsync(context, segment.Id, cancellationToken).ConfigureAwait(false);
                deleted++;
            }

            return deleted;
        }

        private static async Task<List<IndexedSegment>> LoadIndexedSegmentsForSourceAsync(
            string databasePath,
            string filePath,
            CancellationToken cancellationToken)
        {
            var normalizedFilePath = AerialCityRagRegistry.NormalizePath(filePath);
            var segments = new List<IndexedSegment>();
            var segmentsPath = Path.Combine(databasePath, "segments");
            if (!Directory.Exists(segmentsPath))
            {
                return segments;
            }

            foreach (var segmentFilePath in Directory.EnumerateFiles(segmentsPath, "*.seg"))
            {
                cancellationToken.ThrowIfCancellationRequested();
                try
                {
                    await using var stream = File.OpenRead(segmentFilePath);
                    using var document = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken).ConfigureAwait(false);
                    var root = document.RootElement;
                    var sourceUri = TryReadString(root, "sourceUri");
                    if (string.IsNullOrWhiteSpace(sourceUri) ||
                        !TryNormalizePath(sourceUri, out var normalizedSourceUri) ||
                        !string.Equals(normalizedSourceUri, normalizedFilePath, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    var idText = TryReadString(root, "id");
                    if (!AerialId.TryParse(idText, out var id))
                    {
                        continue;
                    }

                    var kind = root.TryGetProperty("kind", out var kindElement) && kindElement.ValueKind == JsonValueKind.Number
                        ? (SegmentKind)kindElement.GetInt32()
                        : SegmentKind.TextPassage;
                    var content = TryReadString(root, "content") ?? string.Empty;
                    var startOffset = TryReadInt(root, "startOffset");
                    var endOffset = TryReadInt(root, "endOffset");
                    EmbeddingVector? embedding = null;
                    if (root.TryGetProperty("embedding", out var embeddingElement) &&
                        embeddingElement.ValueKind == JsonValueKind.Array)
                    {
                        var values = embeddingElement.EnumerateArray().Select(value => value.GetSingle()).ToArray();
                        if (values.Length > 0)
                        {
                            embedding = new EmbeddingVector(values);
                        }
                    }

                    var metadata = new Dictionary<string, object>(StringComparer.Ordinal);
                    if (root.TryGetProperty("metadata", out var metadataElement) &&
                        metadataElement.ValueKind == JsonValueKind.Object)
                    {
                        foreach (var property in metadataElement.EnumerateObject())
                        {
                            metadata[property.Name] = ReadMetadataValue(property.Value);
                        }
                    }

                    segments.Add(new IndexedSegment(
                        id,
                        kind,
                        content,
                        sourceUri,
                        startOffset,
                        endOffset,
                        embedding,
                        metadata));
                }
                catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or JsonException or ArgumentException or NotSupportedException)
                {
                }
            }

            return segments;
        }

        private static async Task<AerialCityRagFileSnapshot> CreateFileSnapshotAsync(
            string filePath,
            string targetDirectory,
            bool embedded,
            int segmentCount,
            string sourceType,
            CancellationToken cancellationToken)
        {
            var info = new FileInfo(filePath);
            return new AerialCityRagFileSnapshot
            {
                RelativePath = GetRelativePath(targetDirectory, filePath),
                Hash = await ComputeFastFileHashAsync(filePath, cancellationToken).ConfigureAwait(false),
                HashAlgorithm = FileHashAlgorithm,
                Length = info.Exists ? info.Length : 0L,
                LastWriteTimeUtc = info.Exists ? info.LastWriteTimeUtc : DateTimeOffset.MinValue,
                UpdatedAtUtc = DateTimeOffset.UtcNow,
                Embedded = embedded,
                SegmentCount = segmentCount,
                SourceType = sourceType
            };
        }

        private static async Task<string> ComputeFastFileHashAsync(string filePath, CancellationToken cancellationToken)
        {
            const ulong offsetBasis = 14695981039346656037UL;
            const ulong prime = 1099511628211UL;

            var hash = offsetBasis;
            var buffer = new byte[128 * 1024];
            await using var stream = new FileStream(
                filePath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.ReadWrite | FileShare.Delete,
                buffer.Length,
                useAsync: true);

            while (true)
            {
                var read = await stream.ReadAsync(buffer, cancellationToken).ConfigureAwait(false);
                if (read == 0)
                {
                    break;
                }

                for (var index = 0; index < read; index++)
                {
                    hash ^= buffer[index];
                    hash *= prime;
                }
            }

            return hash.ToString("x16", CultureInfo.InvariantCulture);
        }

        private static bool IsSnapshotCurrent(AerialCityRagFileSnapshot? previous, AerialCityRagFileSnapshot current)
        {
            return previous != null &&
                string.Equals(previous.HashAlgorithm, current.HashAlgorithm, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(previous.Hash, current.Hash, StringComparison.OrdinalIgnoreCase) &&
                previous.Length == current.Length;
        }

        private static AerialCityRagFileSnapshot CarryForwardEmbeddingState(
            AerialCityRagFileSnapshot current,
            AerialCityRagFileSnapshot previous)
        {
            return new AerialCityRagFileSnapshot
            {
                RelativePath = current.RelativePath,
                Hash = current.Hash,
                HashAlgorithm = current.HashAlgorithm,
                Length = current.Length,
                LastWriteTimeUtc = current.LastWriteTimeUtc,
                UpdatedAtUtc = DateTimeOffset.UtcNow,
                Embedded = previous.Embedded,
                SegmentCount = previous.SegmentCount,
                SourceType = previous.SourceType
            };
        }

        private static bool IsSnapshotInsideScope(string targetDirectory, string relativePath, string scopeDirectory)
        {
            var absolutePath = Path.Combine(targetDirectory, relativePath.Replace('/', Path.DirectorySeparatorChar));
            return AerialCityRagRegistry.IsSubPathOrEqual(scopeDirectory, absolutePath);
        }

        private static AerialCityRagFolderMapping CloneMappingWithSnapshots(
            AerialCityRagFolderMapping mapping,
            IReadOnlyList<AerialCityRagFileSnapshot> snapshots,
            DateTimeOffset updatedAtUtc)
        {
            return new AerialCityRagFolderMapping
            {
                TargetPath = mapping.TargetPath,
                DatabaseFolderName = mapping.DatabaseFolderName,
                DatabasePath = mapping.DatabasePath,
                EmbeddingModelKey = mapping.EmbeddingModelKey,
                EmbeddingModelDisplayName = mapping.EmbeddingModelDisplayName,
                EmbeddingModelId = mapping.EmbeddingModelId,
                EmbeddingInterfaceType = mapping.EmbeddingInterfaceType,
                EmbeddingDimensions = mapping.EmbeddingDimensions,
                SupportsMultimodalEmbedding = mapping.SupportsMultimodalEmbedding,
                InitializedAtUtc = mapping.InitializedAtUtc,
                UpdatedAtUtc = updatedAtUtc,
                FileSnapshots = snapshots
            };
        }

        private static IndexedSegment? TryDequeueMatchingSegment(
            Dictionary<string, Queue<IndexedSegment>> oldByKey,
            string key)
        {
            if (!oldByKey.TryGetValue(key, out var queue) || queue.Count == 0)
            {
                return null;
            }

            return queue.Dequeue();
        }

        private static string CreateSegmentMatchKey(SegmentKind kind, string content)
        {
            return $"{(int)kind}:{ComputeFastTextHash(content)}";
        }

        private static string ComputeFastTextHash(string text)
        {
            const ulong offsetBasis = 14695981039346656037UL;
            const ulong prime = 1099511628211UL;

            var hash = offsetBasis;
            foreach (var ch in text)
            {
                hash ^= ch;
                hash *= prime;
            }

            return hash.ToString("x16", CultureInfo.InvariantCulture);
        }

        private static bool IsStoredSegmentEquivalent(IndexedSegment existing, Segment current)
        {
            return existing.Kind == current.Kind &&
                string.Equals(existing.Content, current.Content, StringComparison.Ordinal) &&
                string.Equals(existing.SourceUri, current.SourceUri, StringComparison.OrdinalIgnoreCase) &&
                existing.StartOffset == current.StartOffset &&
                existing.EndOffset == current.EndOffset &&
                MetadataContainsEquivalentValues(existing.Metadata, current.Metadata);
        }

        private static bool MetadataContainsEquivalentValues(
            IReadOnlyDictionary<string, object> existing,
            IReadOnlyDictionary<string, object> current)
        {
            foreach (var (key, value) in current)
            {
                if (!existing.TryGetValue(key, out var existingValue))
                {
                    return false;
                }

                if (!string.Equals(
                        Convert.ToString(existingValue, CultureInfo.InvariantCulture),
                        Convert.ToString(value, CultureInfo.InvariantCulture),
                        StringComparison.Ordinal))
                {
                    return false;
                }
            }

            return true;
        }

        private static string? TryReadString(JsonElement root, string propertyName)
        {
            return root.TryGetProperty(propertyName, out var element) && element.ValueKind == JsonValueKind.String
                ? element.GetString()
                : null;
        }

        private static int TryReadInt(JsonElement root, string propertyName)
        {
            return root.TryGetProperty(propertyName, out var element) && element.ValueKind == JsonValueKind.Number
                ? element.GetInt32()
                : 0;
        }

        private static object ReadMetadataValue(JsonElement element)
        {
            return element.ValueKind switch
            {
                JsonValueKind.String => element.GetString() ?? string.Empty,
                JsonValueKind.Number when element.TryGetInt64(out var longValue) => longValue,
                JsonValueKind.Number when element.TryGetDouble(out var doubleValue) => doubleValue,
                JsonValueKind.True => true,
                JsonValueKind.False => false,
                JsonValueKind.Array => element.EnumerateArray().Select(ReadMetadataValue).ToArray(),
                JsonValueKind.Null => string.Empty,
                _ => element.GetRawText()
            };
        }

        private static int GetSegmentationTokenBudget(int maxInputTokens)
        {
            if (maxInputTokens <= 1)
            {
                return maxInputTokens;
            }

            return Math.Max(1, (int)Math.Floor(maxInputTokens * DefaultSegmentationBudgetRatio));
        }

        private static int NormalizeEmbeddingConcurrency(int embeddingConcurrency)
        {
            return Math.Clamp(
                embeddingConcurrency,
                AerialCityRagConfiguration.MinimumEmbeddingConcurrency,
                AerialCityRagConfiguration.MaximumEmbeddingConcurrency);
        }

        private static async ValueTask ReportToolProgressAsync(
            Func<SkyweaverToolProgressUpdate, CancellationToken, ValueTask>? progressReporter,
            string phase,
            string statusText,
            int completedItems,
            int totalItems,
            IReadOnlyList<string> activeItems,
            bool isCompleted,
            CancellationToken cancellationToken)
        {
            if (progressReporter == null)
            {
                return;
            }

            var boundedTotal = Math.Max(0, totalItems);
            var boundedCompleted = Math.Clamp(completedItems, 0, boundedTotal == 0 ? completedItems : boundedTotal);
            await progressReporter(
                new SkyweaverToolProgressUpdate
                {
                    Phase = phase,
                    StatusText = statusText,
                    CompletedItems = boundedTotal > 0 ? boundedCompleted : null,
                    TotalItems = boundedTotal > 0 ? boundedTotal : null,
                    ProgressFraction = boundedTotal > 0
                        ? boundedCompleted / (double)boundedTotal
                        : null,
                    IsCompleted = isCompleted,
                    ActiveItems = activeItems
                        .Where(item => !string.IsNullOrWhiteSpace(item))
                        .Take(MaximumProgressActiveItems)
                        .ToArray()
                },
                cancellationToken).ConfigureAwait(false);
        }

        private static void MergeUpdateStatistics(
            AerialCityRagUpdateStatistics target,
            AerialCityRagUpdateStatistics source)
        {
            target.FilesVisited += source.FilesVisited;
            target.FilesReembedded += source.FilesReembedded;
            target.FilesRemoved += source.FilesRemoved;
            target.CodeFilesEmbedded += source.CodeFilesEmbedded;
            target.TextFilesEmbedded += source.TextFilesEmbedded;
            target.ImageFilesEmbedded += source.ImageFilesEmbedded;
            target.SegmentsInserted += source.SegmentsInserted;
            target.SegmentsUpdated += source.SegmentsUpdated;
            target.SegmentsReused += source.SegmentsReused;
            target.SegmentsDeleted += source.SegmentsDeleted;
            target.FilesSkippedAlreadyIndexed += source.FilesSkippedAlreadyIndexed;
            target.BinaryFilesSkipped += source.BinaryFilesSkipped;
            target.FilesSkippedTooLarge += source.FilesSkippedTooLarge;
            target.FilesExcludedByMode += source.FilesExcludedByMode;
            target.FilesFailed += source.FilesFailed;
            target.CodeFilesFellBackToText += source.CodeFilesFellBackToText;

            foreach (var failure in source.Failures)
            {
                if (target.Failures.Count >= MaximumRecordedFailures)
                {
                    break;
                }

                target.Failures.Add(failure);
            }
        }

        private static async Task<HashSet<string>> LoadIndexedSourceUrisAsync(string databasePath, CancellationToken cancellationToken)
        {
            var indexedSources = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var segmentsPath = Path.Combine(databasePath, "segments");
            if (!Directory.Exists(segmentsPath))
            {
                return indexedSources;
            }

            foreach (var segmentFilePath in Directory.EnumerateFiles(segmentsPath, "*.seg"))
            {
                cancellationToken.ThrowIfCancellationRequested();
                try
                {
                    await using var stream = File.OpenRead(segmentFilePath);
                    using var document = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken).ConfigureAwait(false);
                    if (document.RootElement.TryGetProperty("sourceUri", out var sourceUriElement) &&
                        sourceUriElement.ValueKind == JsonValueKind.String)
                    {
                        var sourceUri = sourceUriElement.GetString();
                        if (!string.IsNullOrWhiteSpace(sourceUri) && TryNormalizePath(sourceUri, out var normalizedSourceUri))
                        {
                            indexedSources.Add(normalizedSourceUri);
                        }
                    }
                }
                catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or JsonException or ArgumentException or NotSupportedException)
                {
                }
            }

            return indexedSources;
        }

        private static AerialCityRagFileEnumerationResult EnumerateFiles(
            string rootDirectory,
            string targetDirectory,
            AerialCityRagIndexMode mode,
            CancellationToken cancellationToken)
        {
            var includedFiles = new List<string>();
            var excludedFiles = new List<AerialCityRagExcludedFile>();
            var pending = new Stack<string>();
            pending.Push(rootDirectory);

            while (pending.Count > 0)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var directory = pending.Pop();

                IEnumerable<string> files;
                try
                {
                    files = Directory.EnumerateFiles(directory).OrderBy(item => item, StringComparer.OrdinalIgnoreCase).ToArray();
                }
                catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or ArgumentException or NotSupportedException)
                {
                    continue;
                }

                foreach (var file in files)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    if (TryNormalizePath(file, out var normalizedFile))
                    {
                        var exclusion = GetModeExclusionForFile(normalizedFile, targetDirectory, mode);
                        if (exclusion.Excluded)
                        {
                            excludedFiles.Add(new AerialCityRagExcludedFile
                            {
                                RelativePath = GetRelativePath(targetDirectory, normalizedFile),
                                Reason = exclusion.Reason
                            });
                            continue;
                        }

                        includedFiles.Add(normalizedFile);
                    }
                }

                IEnumerable<string> childDirectories;
                try
                {
                    childDirectories = Directory.EnumerateDirectories(directory)
                        .OrderByDescending(item => item, StringComparer.OrdinalIgnoreCase)
                        .ToArray();
                }
                catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or ArgumentException or NotSupportedException)
                {
                    continue;
                }

                foreach (var childDirectory in childDirectories)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    try
                    {
                        if ((File.GetAttributes(childDirectory) & FileAttributes.ReparsePoint) != 0)
                        {
                            continue;
                        }
                    }
                    catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or ArgumentException or NotSupportedException)
                    {
                        continue;
                    }

                    pending.Push(childDirectory);
                }
            }

            return new AerialCityRagFileEnumerationResult(
                includedFiles,
                excludedFiles
                    .GroupBy(file => file.RelativePath, StringComparer.OrdinalIgnoreCase)
                    .Select(group => group.First())
                    .OrderBy(file => file.RelativePath, StringComparer.OrdinalIgnoreCase)
                    .ToArray());
        }

        private static AerialCityRagFileExclusionDecision GetModeExclusionForFile(
            string filePath,
            string targetDirectory,
            AerialCityRagIndexMode mode)
        {
            if (mode == AerialCityRagIndexMode.Everything)
            {
                return AerialCityRagFileExclusionDecision.Include;
            }

            var relativePath = GetRelativePath(targetDirectory, filePath);
            var segments = relativePath.Split(
                ['/', '\\'],
                StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            for (var index = 0; index < Math.Max(0, segments.Length - 1); index++)
            {
                var segment = segments[index];
                if (IsCodeModeExcludedDirectoryName(segment, out var reason))
                {
                    return new AerialCityRagFileExclusionDecision(true, reason);
                }
            }

            var fileName = Path.GetFileName(filePath);
            if (fileName.StartsWith("~$", StringComparison.Ordinal) ||
                fileName.EndsWith("~", StringComparison.Ordinal))
            {
                return new AerialCityRagFileExclusionDecision(true, "temporary-file");
            }

            if (s_codeModeExcludedFileNames.Contains(fileName))
            {
                return new AerialCityRagFileExclusionDecision(true, "noisy-file");
            }

            var extension = Path.GetExtension(fileName);
            if (s_codeModeExcludedFileExtensions.Contains(extension))
            {
                return new AerialCityRagFileExclusionDecision(true, "generated-or-binary-file");
            }

            return AerialCityRagFileExclusionDecision.Include;
        }

        private static bool IsCodeModeExcludedDirectoryName(string directoryName, out string reason)
        {
            reason = string.Empty;
            if (directoryName.StartsWith(".", StringComparison.Ordinal))
            {
                reason = "dot-directory";
                return true;
            }

            if (s_codeModeExcludedDirectoryNames.Contains(directoryName))
            {
                reason = "generated-or-dependency-directory";
                return true;
            }

            return false;
        }

        private static AerialCityRagExcludedFiles MergeExcludedFiles(
            AerialCityRagExcludedFiles existing,
            AerialCityRagIndexMode mode,
            string targetDirectory,
            string scopeDirectory,
            IReadOnlyList<AerialCityRagExcludedFile> scopedExcludedFiles)
        {
            if (mode == AerialCityRagIndexMode.Everything)
            {
                return AerialCityRagExcludedFiles.Empty(mode);
            }

            IReadOnlyList<AerialCityRagExcludedFile> carriedExistingFiles = existing.Mode == mode
                ? existing.Files
                : [];

            var merged = carriedExistingFiles
                .Where(file =>
                    !IsSnapshotInsideScope(targetDirectory, file.RelativePath, scopeDirectory) ||
                    File.Exists(Path.Combine(targetDirectory, file.RelativePath.Replace('/', Path.DirectorySeparatorChar))))
                .Concat(scopedExcludedFiles)
                .GroupBy(file => file.RelativePath, StringComparer.OrdinalIgnoreCase)
                .Select(group => group.First())
                .OrderBy(file => file.RelativePath, StringComparer.OrdinalIgnoreCase)
                .ToArray();

            return new AerialCityRagExcludedFiles
            {
                Mode = mode,
                GeneratedAtUtc = DateTimeOffset.UtcNow,
                Files = merged
            };
        }

        private static AerialCityRagExcludedFiles MergeSingleExcludedFile(
            AerialCityRagExcludedFiles existing,
            AerialCityRagIndexMode mode,
            string relativePath,
            AerialCityRagFileExclusionDecision exclusion)
        {
            if (mode == AerialCityRagIndexMode.Everything)
            {
                return AerialCityRagExcludedFiles.Empty(mode);
            }

            var normalizedRelativePath = relativePath.Replace('\\', '/');
            IReadOnlyList<AerialCityRagExcludedFile> carriedExistingFiles = existing.Mode == mode
                ? existing.Files
                : [];

            var existingEntry = carriedExistingFiles.FirstOrDefault(file =>
                string.Equals(file.RelativePath, normalizedRelativePath, StringComparison.OrdinalIgnoreCase));
            var files = carriedExistingFiles
                .Where(file => !string.Equals(file.RelativePath, normalizedRelativePath, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (exclusion.Excluded)
            {
                files.Add(new AerialCityRagExcludedFile
                {
                    RelativePath = normalizedRelativePath,
                    Reason = exclusion.Reason
                });
            }
            else if (existingEntry != null)
            {
                files.Add(existingEntry);
            }

            return new AerialCityRagExcludedFiles
            {
                Mode = mode,
                GeneratedAtUtc = DateTimeOffset.UtcNow,
                Files = files
                    .OrderBy(file => file.RelativePath, StringComparer.OrdinalIgnoreCase)
                    .ToArray()
            };
        }

        private static AerialCityRagExcludedFiles AddExcludedFile(
            AerialCityRagExcludedFiles existing,
            string relativePath,
            string reason)
        {
            return new AerialCityRagExcludedFiles
            {
                Mode = existing.Mode,
                GeneratedAtUtc = DateTimeOffset.UtcNow,
                Files = existing.Files
                    .Where(file => !string.Equals(file.RelativePath, relativePath, StringComparison.OrdinalIgnoreCase))
                    .Append(new AerialCityRagExcludedFile
                    {
                        RelativePath = relativePath.Replace('\\', '/'),
                        Reason = reason
                    })
                    .OrderBy(file => file.RelativePath, StringComparer.OrdinalIgnoreCase)
                    .ToArray()
            };
        }

        private static async Task<FileProbe> ProbeFileAsync(string filePath, CancellationToken cancellationToken)
        {
            var buffer = new byte[4096];
            await using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete, buffer.Length, useAsync: true);
            var bytesRead = await stream.ReadAsync(buffer, cancellationToken).ConfigureAwait(false);

            if (TryDetectUtfEncoding(buffer, bytesRead, out var encoding))
            {
                return new FileProbe(false, encoding);
            }

            for (var index = 0; index < bytesRead; index++)
            {
                if (buffer[index] == 0)
                {
                    return new FileProbe(true, null);
                }
            }

            return new FileProbe(false, null);
        }

        private static bool TryDetectUtfEncoding(byte[] sample, int sampleLength, out Encoding? encoding)
        {
            encoding = null;
            if (sampleLength >= 3 && sample[0] == 0xEF && sample[1] == 0xBB && sample[2] == 0xBF)
            {
                encoding = Encoding.UTF8;
                return true;
            }

            if (sampleLength >= 2 && sample[0] == 0xFF && sample[1] == 0xFE)
            {
                encoding = Encoding.Unicode;
                return true;
            }

            if (sampleLength >= 2 && sample[0] == 0xFE && sample[1] == 0xFF)
            {
                encoding = Encoding.BigEndianUnicode;
                return true;
            }

            var evenZeros = 0;
            var oddZeros = 0;
            var pairs = sampleLength / 2;
            if (pairs < 8)
            {
                return false;
            }

            for (var index = 0; index + 1 < sampleLength; index += 2)
            {
                if (sample[index] == 0 && sample[index + 1] != 0)
                {
                    evenZeros++;
                }
                else if (sample[index] != 0 && sample[index + 1] == 0)
                {
                    oddZeros++;
                }
            }

            if (oddZeros >= pairs * 0.6 && evenZeros <= pairs * 0.1)
            {
                encoding = Encoding.Unicode;
                return true;
            }

            if (evenZeros >= pairs * 0.6 && oddZeros <= pairs * 0.1)
            {
                encoding = Encoding.BigEndianUnicode;
                return true;
            }

            return false;
        }

        private static bool IsFileTooLarge(string filePath, int maximumFileBytes)
        {
            try
            {
                return new FileInfo(filePath).Length > maximumFileBytes;
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or ArgumentException or NotSupportedException)
            {
                return true;
            }
        }

        private static bool IsResultInsideSearchPath(RetrievalResult result, string searchPath)
        {
            var sourcePath = ResolveResultSourcePath(result);
            if (string.IsNullOrWhiteSpace(sourcePath) || !TryNormalizePath(sourcePath, out var normalizedSourcePath))
            {
                return false;
            }

            if (File.Exists(searchPath))
            {
                return string.Equals(normalizedSourcePath, searchPath, StringComparison.OrdinalIgnoreCase);
            }

            return AerialCityRagRegistry.IsSubPathOrEqual(searchPath, normalizedSourcePath);
        }

        private static string? ResolveResultSourcePath(RetrievalResult result)
        {
            if (!string.IsNullOrWhiteSpace(result.Segment.SourceUri))
            {
                return result.Segment.SourceUri;
            }

            foreach (var key in new[] { "path", "Path", "filePath", "FilePath" })
            {
                if (result.Segment.Metadata.TryGetValue(key, out var value))
                {
                    var text = value?.ToString();
                    if (!string.IsNullOrWhiteSpace(text))
                    {
                        return text;
                    }
                }
            }

            return null;
        }

        private static string ResolveExistingDirectory(string requestedDirectory, string? workspacePath)
        {
            var resolvedPath = ResolvePath(requestedDirectory, workspacePath);
            if (!Directory.Exists(resolvedPath))
            {
                throw new DirectoryNotFoundException($"Target directory not found: {resolvedPath}");
            }

            return AerialCityRagRegistry.NormalizePath(resolvedPath);
        }

        private static string ResolveExistingPath(string requestedPath, string? workspacePath)
        {
            var resolvedPath = ResolvePath(requestedPath, workspacePath);
            if (!Directory.Exists(resolvedPath) && !File.Exists(resolvedPath))
            {
                throw new FileNotFoundException($"Search path not found: {resolvedPath}", resolvedPath);
            }

            return AerialCityRagRegistry.NormalizePath(resolvedPath);
        }

        private static string ResolvePath(string requestedPath, string? workspacePath)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(requestedPath);
            var trimmedPath = requestedPath.Trim();
            if (Path.IsPathRooted(trimmedPath))
            {
                return Path.GetFullPath(trimmedPath);
            }

            var basePath = string.IsNullOrWhiteSpace(workspacePath)
                ? Environment.CurrentDirectory
                : workspacePath.Trim();
            return Path.GetFullPath(Path.Combine(basePath, trimmedPath));
        }

        private static bool TryNormalizePath(string path, out string normalizedPath)
        {
            try
            {
                normalizedPath = AerialCityRagRegistry.NormalizePath(path);
                return true;
            }
            catch (Exception ex) when (ex is ArgumentException or IOException or NotSupportedException)
            {
                normalizedPath = string.Empty;
                return false;
            }
        }

        private static string GetRelativePath(string rootPath, string filePath)
        {
            try
            {
                return Path.GetRelativePath(rootPath, filePath).Replace('\\', '/');
            }
            catch (Exception ex) when (ex is ArgumentException or NotSupportedException)
            {
                return filePath;
            }
        }

        private static string? ResolveLanguageHint(string filePath)
        {
            var extension = Path.GetExtension(filePath).TrimStart('.');
            return string.IsNullOrWhiteSpace(extension) ? null : extension;
        }

        private static string ResolveImageMimeType(string filePath)
        {
            return Path.GetExtension(filePath).ToLowerInvariant() switch
            {
                ".bmp" => "image/bmp",
                ".gif" => "image/gif",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".tif" or ".tiff" => "image/tiff",
                ".webp" => "image/webp",
                _ => "application/octet-stream"
            };
        }

        public static string FormatInitializationResult(AerialCityRagInitializationResult result)
        {
            var stats = result.Statistics;
            var builder = new StringBuilder();
            builder.AppendLine("AerialCity RAG initialized.");
            builder.AppendLine($"TargetDirectory: {result.TargetDirectory}");
            builder.AppendLine($"AerialCityDirectory: {result.AerialCityDirectory}");
            builder.AppendLine($"RegistryFile: {result.RegistryFilePath}");
            builder.AppendLine($"DatabasePath: {result.DatabasePath}");
            builder.AppendLine($"Mode: {result.Mode}");
            builder.AppendLine($"ExcludedFiles: {result.ExcludedFileCount}");
            builder.AppendLine($"ExcludedFilesFile: {result.ExcludedFilesPath}");
            builder.AppendLine($"EmbeddingModel: {result.EmbeddingModelDisplayName} ({result.EmbeddingModelId})");
            builder.AppendLine($"SupportsMultimodalEmbedding: {result.SupportsMultimodalEmbedding}");
            builder.AppendLine();
            builder.AppendLine($"FilesVisited: {stats.FilesVisited}");
            builder.AppendLine($"CodeFilesEmbedded: {stats.CodeFilesEmbedded}");
            builder.AppendLine($"TextFilesEmbedded: {stats.TextFilesEmbedded}");
            builder.AppendLine($"ImageFilesEmbedded: {stats.ImageFilesEmbedded}");
            builder.AppendLine($"SegmentsInserted: {stats.SegmentsInserted}");
            builder.AppendLine($"AlreadyIndexedSkipped: {stats.FilesSkippedAlreadyIndexed}");
            builder.AppendLine($"BinaryFilesSkipped: {stats.BinaryFilesSkipped}");
            builder.AppendLine($"TooLargeSkipped: {stats.FilesSkippedTooLarge}");
            builder.AppendLine($"ModeExcludedFiles: {stats.FilesExcludedByMode}");
            builder.AppendLine($"FailedFiles: {stats.FilesFailed}");
            builder.AppendLine($"CodeFallbackToText: {stats.CodeFilesFellBackToText}");

            if (stats.Failures.Count > 0)
            {
                builder.AppendLine();
                builder.AppendLine("Failures:");
                foreach (var failure in stats.Failures)
                {
                    builder.AppendLine($"- {failure}");
                }
            }

            return builder.ToString().TrimEnd();
        }

        public static string FormatSearchResult(AerialCityRagSearchResult result)
        {
            var builder = new StringBuilder();
            builder.AppendLine($"{result.Method} search completed.");
            builder.AppendLine($"SearchPath: {result.SearchPath}");
            builder.AppendLine($"InitializedRoot: {result.TargetDirectory}");
            builder.AppendLine($"DatabasePath: {result.DatabasePath}");
            builder.AppendLine($"Query: {result.Query}");
            builder.AppendLine($"Results: {result.Results.Count}");
            builder.AppendLine();

            if (result.Results.Count == 0)
            {
                builder.AppendLine("No matching AerialCity segments found.");
                return builder.ToString().TrimEnd();
            }

            for (var index = 0; index < result.Results.Count; index++)
            {
                var item = result.Results[index];
                var path = ResolveResultSourcePath(item) ?? "(unknown source)";
                builder.AppendLine($"{index + 1}. Score: {item.Score.ToString("0.####", CultureInfo.InvariantCulture)}");
                builder.AppendLine($"Path: {path}");
                builder.AppendLine($"Kind: {item.Segment.Kind}");
                if (item.Segment.EndOffset > item.Segment.StartOffset)
                {
                    builder.AppendLine($"Offsets: {item.Segment.StartOffset}-{item.Segment.EndOffset}");
                }

                builder.AppendLine("Content:");
                builder.AppendLine(TrimContent(item.Segment.Content));
                builder.AppendLine();
            }

            return builder.ToString().TrimEnd();
        }

        private static string TrimContent(string content)
        {
            const int maximumLength = 1600;
            var text = content.Trim();
            return text.Length <= maximumLength ? text : $"{text[..maximumLength]}...";
        }

        public static string FormatUpdateResult(AerialCityRagUpdateResult result)
        {
            var stats = result.Statistics;
            var builder = new StringBuilder();
            builder.AppendLine("AerialCity DB updated.");
            builder.AppendLine($"RequestedDirectory: {result.RequestedDirectory}");
            builder.AppendLine($"InitializedRoot: {result.TargetDirectory}");
            builder.AppendLine($"DatabasePath: {result.DatabasePath}");
            builder.AppendLine($"Mode: {result.Mode}");
            builder.AppendLine($"ExcludedFiles: {result.ExcludedFileCount}");
            builder.AppendLine($"ExcludedFilesFile: {result.ExcludedFilesPath}");
            builder.AppendLine($"EmbeddingModel: {result.EmbeddingModelDisplayName} ({result.EmbeddingModelId})");
            builder.AppendLine();
            builder.AppendLine($"FilesVisited: {stats.FilesVisited}");
            builder.AppendLine($"FilesReembedded: {stats.FilesReembedded}");
            builder.AppendLine($"FilesUnchanged: {stats.FilesSkippedAlreadyIndexed}");
            builder.AppendLine($"FilesRemoved: {stats.FilesRemoved}");
            builder.AppendLine($"CodeFilesEmbedded: {stats.CodeFilesEmbedded}");
            builder.AppendLine($"TextFilesEmbedded: {stats.TextFilesEmbedded}");
            builder.AppendLine($"ImageFilesEmbedded: {stats.ImageFilesEmbedded}");
            builder.AppendLine($"SegmentsInserted: {stats.SegmentsInserted}");
            builder.AppendLine($"SegmentsUpdated: {stats.SegmentsUpdated}");
            builder.AppendLine($"SegmentsReused: {stats.SegmentsReused}");
            builder.AppendLine($"SegmentsDeleted: {stats.SegmentsDeleted}");
            builder.AppendLine($"BinaryFilesSkipped: {stats.BinaryFilesSkipped}");
            builder.AppendLine($"TooLargeSkipped: {stats.FilesSkippedTooLarge}");
            builder.AppendLine($"ModeExcludedFiles: {stats.FilesExcludedByMode}");
            builder.AppendLine($"FailedFiles: {stats.FilesFailed}");
            builder.AppendLine($"CodeFallbackToText: {stats.CodeFilesFellBackToText}");

            if (stats.Failures.Count > 0)
            {
                builder.AppendLine();
                builder.AppendLine("Failures:");
                foreach (var failure in stats.Failures)
                {
                    builder.AppendLine($"- {failure}");
                }
            }

            return builder.ToString().TrimEnd();
        }

        private sealed record AerialCityRagDatabaseUpdateContext(
            AerialCityRagFolderMapping Mapping,
            EmbeddingModelDefinition Model,
            AerialDatabase Database,
            ApiEmbeddingRequest Template,
            EmbedContentDelegate EmbedContent,
            InsertSegmentDelegate Insert,
            UpdateSegmentDelegate Update,
            DeleteSegmentDelegate Delete,
            int EmbeddingConcurrency) : IDisposable
        {
            public SemaphoreSlim EmbeddingSemaphore { get; } = new(EmbeddingConcurrency, EmbeddingConcurrency);

            public SemaphoreSlim DatabaseMutationLock { get; } = new(1, 1);

            public void Dispose()
            {
                EmbeddingSemaphore.Dispose();
                DatabaseMutationLock.Dispose();
            }
        }

        private sealed record PendingSegmentChange(
            Segment Segment,
            IndexedSegment? ExistingSegment,
            bool NeedsEmbedding);

        private sealed record AerialCityRagFileUpdateOutcome(
            string RelativePath,
            AerialCityRagFileSnapshot? Snapshot,
            AerialCityRagUpdateStatistics Statistics);

        private sealed record AerialCityRagFileEnumerationResult(
            IReadOnlyList<string> IncludedFilePaths,
            IReadOnlyList<AerialCityRagExcludedFile> ExcludedFiles);

        private sealed record AerialCityRagFileExclusionDecision(
            bool Excluded,
            string Reason)
        {
            public static AerialCityRagFileExclusionDecision Include { get; } = new(false, string.Empty);
        }

        private sealed record IndexedSegment(
            AerialId Id,
            SegmentKind Kind,
            string Content,
            string? SourceUri,
            int StartOffset,
            int EndOffset,
            EmbeddingVector? Embedding,
            IReadOnlyDictionary<string, object> Metadata);

        private sealed record FileProbe(bool IsBinary, Encoding? Encoding);
    }

    public sealed class AerialCityRagInitializationResult
    {
        public required string TargetDirectory { get; init; }

        public required string AerialCityDirectory { get; init; }

        public required string RegistryFilePath { get; init; }

        public required string DatabasePath { get; init; }

        public required string DatabaseFolderName { get; init; }

        public AerialCityRagIndexMode Mode { get; init; }

        public required string ExcludedFilesPath { get; init; }

        public int ExcludedFileCount { get; init; }

        public required string EmbeddingModelDisplayName { get; init; }

        public required string EmbeddingModelId { get; init; }

        public bool SupportsMultimodalEmbedding { get; init; }

        public required AerialCityRagInitializationStatistics Statistics { get; init; }
    }

    public sealed class AerialCityRagInitializationStatistics
    {
        public int FilesVisited { get; set; }

        public int CodeFilesEmbedded { get; set; }

        public int TextFilesEmbedded { get; set; }

        public int ImageFilesEmbedded { get; set; }

        public int SegmentsInserted { get; set; }

        public int FilesSkippedAlreadyIndexed { get; set; }

        public int BinaryFilesSkipped { get; set; }

        public int FilesSkippedTooLarge { get; set; }

        public int FilesExcludedByMode { get; set; }

        public int FilesFailed { get; set; }

        public int CodeFilesFellBackToText { get; set; }

        public List<string> Failures { get; } = [];

        public static AerialCityRagInitializationStatistics FromUpdateStatistics(AerialCityRagUpdateStatistics statistics)
        {
            var result = new AerialCityRagInitializationStatistics
            {
                FilesVisited = statistics.FilesVisited,
                CodeFilesEmbedded = statistics.CodeFilesEmbedded,
                TextFilesEmbedded = statistics.TextFilesEmbedded,
                ImageFilesEmbedded = statistics.ImageFilesEmbedded,
                SegmentsInserted = statistics.SegmentsInserted + statistics.SegmentsUpdated,
                FilesSkippedAlreadyIndexed = statistics.FilesSkippedAlreadyIndexed,
                BinaryFilesSkipped = statistics.BinaryFilesSkipped,
                FilesSkippedTooLarge = statistics.FilesSkippedTooLarge,
                FilesExcludedByMode = statistics.FilesExcludedByMode,
                FilesFailed = statistics.FilesFailed,
                CodeFilesFellBackToText = statistics.CodeFilesFellBackToText
            };

            result.Failures.AddRange(statistics.Failures);
            return result;
        }
    }

    public sealed class AerialCityRagUpdateResult
    {
        public required string RequestedDirectory { get; init; }

        public required string TargetDirectory { get; init; }

        public required string AerialCityDirectory { get; init; }

        public required string RegistryFilePath { get; init; }

        public required string DatabasePath { get; init; }

        public required string DatabaseFolderName { get; init; }

        public AerialCityRagIndexMode Mode { get; init; }

        public required string ExcludedFilesPath { get; init; }

        public int ExcludedFileCount { get; init; }

        public required string EmbeddingModelDisplayName { get; init; }

        public required string EmbeddingModelId { get; init; }

        public bool SupportsMultimodalEmbedding { get; init; }

        public required AerialCityRagUpdateStatistics Statistics { get; init; }
    }

    public sealed class AerialCityRagUpdateStatistics
    {
        public int FilesVisited { get; set; }

        public int FilesReembedded { get; set; }

        public int FilesRemoved { get; set; }

        public int CodeFilesEmbedded { get; set; }

        public int TextFilesEmbedded { get; set; }

        public int ImageFilesEmbedded { get; set; }

        public int SegmentsInserted { get; set; }

        public int SegmentsUpdated { get; set; }

        public int SegmentsReused { get; set; }

        public int SegmentsDeleted { get; set; }

        public int FilesSkippedAlreadyIndexed { get; set; }

        public int BinaryFilesSkipped { get; set; }

        public int FilesSkippedTooLarge { get; set; }

        public int FilesExcludedByMode { get; set; }

        public int FilesFailed { get; set; }

        public int CodeFilesFellBackToText { get; set; }

        public List<string> Failures { get; } = [];
    }

    public sealed class AerialCityRagFileSyncResult
    {
        public bool IsNoOp { get; init; }

        public bool Succeeded { get; init; }

        public string Message { get; init; } = string.Empty;

        public string? FilePath { get; init; }

        public string? DatabasePath { get; init; }

        public AerialCityRagUpdateStatistics? Statistics { get; init; }

        public static AerialCityRagFileSyncResult NoOp(string message) =>
            new()
            {
                IsNoOp = true,
                Succeeded = true,
                Message = message
            };

        public static AerialCityRagFileSyncResult Failed(string filePath, string databasePath, string message) =>
            new()
            {
                IsNoOp = false,
                Succeeded = false,
                Message = message,
                FilePath = filePath,
                DatabasePath = databasePath
            };

        public static AerialCityRagFileSyncResult FromUpdate(string filePath, AerialCityRagUpdateResult result) =>
            new()
            {
                IsNoOp = false,
                Succeeded = true,
                Message = "AerialCity RAG index synchronized.",
                FilePath = filePath,
                DatabasePath = result.DatabasePath,
                Statistics = result.Statistics
            };
    }

    public sealed class AerialCityRagSearchResult
    {
        public required string SearchPath { get; init; }

        public required string TargetDirectory { get; init; }

        public required string DatabasePath { get; init; }

        public required RetrievalMethod Method { get; init; }

        public required string Query { get; init; }

        public int TopK { get; init; }

        public required IReadOnlyList<RetrievalResult> Results { get; init; }
    }
}
