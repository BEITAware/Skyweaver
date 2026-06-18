using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using AerialCity;
using AerialCity.Core.Primitives;
using AerialCity.Core.Storage;
using AerialCity.Database;
using AerialCity.Embedding;
using AerialCity.GraphStore.Model;
using AerialCity.Retrieval;
using Ferrita.Controls.EmbeddingModelConfigurationControl.Models;
using Ferrita.Controls.EmbeddingModelConfigurationControl.Services;
using Ferrita.Services.AerialCityRag;

namespace Ferrita.Services.KnowledgeWorld
{
    public sealed class KnowledgeWorldService
    {
        private const int DefaultTopK = 10;
        private const int MaximumTopK = 100;
        private const int DefaultLinkTopResults = 5;
        private const int MaximumGraphDepth = 8;
        private const string KnowledgeCollectionName = "KnowledgeWorld.Knowledge";
        private const string QueryCollectionName = "KnowledgeWorld.Queries";
        private const string QueryResultRelation = "KnowledgeWorld.QueryResult";
        private const string QuerySimilarityRelation = "KnowledgeWorld.QuerySimilarity";

        private static readonly Encoding Utf8NoBom = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

        private readonly AerialCityRagConfigurationRepository _ragConfigurationRepository;
        private readonly EmbeddingModelConfigurationRepository _embeddingModelRepository;
        private readonly EmbeddingModelService _embeddingModelService;
        private readonly Func<KnowledgeWorldLayout> _layoutFactory;

        public KnowledgeWorldService()
            : this(
                new AerialCityRagConfigurationRepository(),
                new EmbeddingModelConfigurationRepository(new EmbeddingModelConfigurationPathProvider()),
                new EmbeddingModelService(),
                KnowledgeWorldLayout.CreateDefault)
        {
        }

        internal KnowledgeWorldService(
            AerialCityRagConfigurationRepository ragConfigurationRepository,
            EmbeddingModelConfigurationRepository embeddingModelRepository,
            EmbeddingModelService embeddingModelService,
            Func<KnowledgeWorldLayout> layoutFactory)
        {
            _ragConfigurationRepository = ragConfigurationRepository ?? throw new ArgumentNullException(nameof(ragConfigurationRepository));
            _embeddingModelRepository = embeddingModelRepository ?? throw new ArgumentNullException(nameof(embeddingModelRepository));
            _embeddingModelService = embeddingModelService ?? throw new ArgumentNullException(nameof(embeddingModelService));
            _layoutFactory = layoutFactory ?? throw new ArgumentNullException(nameof(layoutFactory));
        }

        public KnowledgeWorldLayout GetLayout()
        {
            var layout = _layoutFactory();
            layout.EnsureCreated();
            return layout;
        }

        public Task<KnowledgeWorldAddResult> AddKnowledgeTextAsync(
            string text,
            string? suggestedFileName = null,
            CancellationToken cancellationToken = default)
        {
            return AddKnowledgeAsync(new KnowledgeWorldAddRequest
            {
                Text = text,
                SuggestedFileName = suggestedFileName
            }, cancellationToken);
        }

        public Task<KnowledgeWorldAddResult> AddKnowledgeFileAsync(
            string filePath,
            CancellationToken cancellationToken = default)
        {
            return AddKnowledgeAsync(new KnowledgeWorldAddRequest
            {
                FilePath = filePath
            }, cancellationToken);
        }

        public async Task<KnowledgeWorldAddResult> AddKnowledgeAsync(
            KnowledgeWorldAddRequest request,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(request);
            cancellationToken.ThrowIfCancellationRequested();

            var hasText = !string.IsNullOrWhiteSpace(request.Text);
            var hasFile = !string.IsNullOrWhiteSpace(request.FilePath);
            if (hasText == hasFile)
                throw new InvalidOperationException("KnowledgeWorld requires exactly one knowledge source: FilePath or Text.");

            var layout = GetLayout();
            var model = ResolveSelectedEmbeddingModel();
            var now = DateTimeOffset.UtcNow;
            string rawPath;
            string? originalSourcePath = null;

            if (hasText)
            {
                rawPath = CreateUniqueFilePath(
                    layout.RawPath,
                    request.SuggestedFileName,
                    "knowledge.txt",
                    ".txt",
                    now);
                await File.WriteAllTextAsync(rawPath, request.Text!, Utf8NoBom, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                originalSourcePath = Path.GetFullPath(request.FilePath!.Trim());
                if (!File.Exists(originalSourcePath))
                    throw new FileNotFoundException($"Knowledge source file not found: {originalSourcePath}", originalSourcePath);

                rawPath = CreateUniqueFilePath(
                    layout.RawPath,
                    request.SuggestedFileName ?? Path.GetFileName(originalSourcePath),
                    "knowledge.txt",
                    Path.GetExtension(originalSourcePath),
                    now);
                await CopyFileAsync(originalSourcePath, rawPath, cancellationToken).ConfigureAwait(false);
            }

            using var engine = new AerialCityBuilder().Build();
            await using var knowledgeDatabase = await OpenDatabaseAsync(
                engine,
                layout.DatabasePath,
                KnowledgeWorldLayout.KnowledgeDatabaseName,
                cancellationToken).ConfigureAwait(false);

            var template = CreateEmbeddingTemplate(model, EmbeddingInput.FromText("KnowledgeWorld knowledge"));
            var requestMetadata = CreateKnowledgeMetadata(rawPath, originalSourcePath, now);
            var embedRequest = new ApiTextFileEmbeddingRequest
            {
                ApiKey = template.ApiKey,
                BaseUrl = template.BaseUrl,
                ApiType = template.ApiType,
                Model = template.Model,
                FilePath = rawPath,
                SourceUri = rawPath,
                MaxInputTokens = model.MaxInputTokens,
                Parameters = new Dictionary<string, object?>(template.Parameters, StringComparer.Ordinal),
                Dimensions = template.Dimensions,
                Normalize = template.Normalize,
                IncludeBinaryDataInTextProjection = template.IncludeBinaryDataInTextProjection,
                Metadata = requestMetadata
            };

            var embedTextFile = engine.EmbedTextFile();
            var insert = engine.Insert();
            var results = await embedTextFile(embedRequest, cancellationToken).ConfigureAwait(false);
            var segmentIds = new List<AerialId>();

            foreach (var result in results)
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (result.Segment is not { } segment)
                    continue;

                EnrichKnowledgeSegment(segment, rawPath, originalSourcePath, now);
                await insert(knowledgeDatabase, segment, cancellationToken).ConfigureAwait(false);
                segmentIds.Add(segment.Id);
            }

            return new KnowledgeWorldAddResult
            {
                RootPath = layout.RootPath,
                RawPath = rawPath,
                KnowledgeDatabasePath = layout.KnowledgeDatabasePath,
                OriginalSourcePath = originalSourcePath,
                IngestedAtUtc = now,
                SegmentCount = segmentIds.Count,
                SegmentIds = segmentIds
            };
        }

        public Task<KnowledgeWorldQueryResult> QueryKnowledgeByBm25Async(
            string query,
            int topK = DefaultTopK,
            CancellationToken cancellationToken = default)
        {
            return QueryKnowledgeAsync(new KnowledgeWorldQueryRequest
            {
                Query = query,
                Method = KnowledgeWorldRetrievalMethod.BM25,
                TopK = topK
            }, cancellationToken);
        }

        public Task<KnowledgeWorldQueryResult> QueryKnowledgeByCosineAsync(
            string query,
            int topK = DefaultTopK,
            CancellationToken cancellationToken = default)
        {
            return QueryKnowledgeAsync(new KnowledgeWorldQueryRequest
            {
                Query = query,
                Method = KnowledgeWorldRetrievalMethod.Cosine,
                TopK = topK
            }, cancellationToken);
        }

        public async Task<KnowledgeWorldQueryResult> QueryKnowledgeAsync(
            KnowledgeWorldQueryRequest request,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(request);
            cancellationToken.ThrowIfCancellationRequested();

            var query = (request.Query ?? string.Empty).Trim();
            if (query.Length == 0)
                throw new InvalidOperationException("KnowledgeWorld query cannot be empty.");

            var layout = GetLayout();
            var model = ResolveSelectedEmbeddingModel();
            var topK = Math.Clamp(request.TopK <= 0 ? DefaultTopK : request.TopK, 1, MaximumTopK);
            var linkTopResults = Math.Clamp(request.LinkTopResults < 0 ? DefaultLinkTopResults : request.LinkTopResults, 0, topK);
            var relatedDepth = Math.Clamp(request.RelatedQueryMaxDepth, 0, MaximumGraphDepth);
            var similarThreshold = request.SimilarQueryThreshold <= 0 ? 0.8f : request.SimilarQueryThreshold;
            var now = DateTimeOffset.UtcNow;

            using var engine = new AerialCityBuilder().Build();
            await using var knowledgeDatabase = await OpenDatabaseAsync(
                engine,
                layout.DatabasePath,
                KnowledgeWorldLayout.KnowledgeDatabaseName,
                cancellationToken).ConfigureAwait(false);
            await using var queriesDatabase = await OpenDatabaseAsync(
                engine,
                layout.DatabasePath,
                KnowledgeWorldLayout.QueriesDatabaseName,
                cancellationToken).ConfigureAwait(false);

            var queryPath = CreateUniqueFilePath(
                layout.QueriesPath,
                "query.txt",
                "query.txt",
                ".txt",
                now);
            await File.WriteAllTextAsync(queryPath, query, Utf8NoBom, cancellationToken).ConfigureAwait(false);

            var querySegment = CreateQuerySegment(query, queryPath, request.Method, topK, now);
            var template = CreateEmbeddingTemplate(model, EmbeddingInput.FromText(query, queryPath));
            await EmbedSegmentAsync(engine, template, querySegment, cancellationToken).ConfigureAwait(false);

            var insert = engine.Insert();
            await insert(queriesDatabase, querySegment, cancellationToken).ConfigureAwait(false);

            var retrievalResults = await RetrieveKnowledgeAsync(
                engine,
                knowledgeDatabase,
                query,
                querySegment,
                request.Method,
                topK,
                cancellationToken).ConfigureAwait(false);

            var similarQueries = await LinkSimilarQueriesAsync(
                queriesDatabase,
                querySegment,
                similarThreshold,
                now,
                cancellationToken).ConfigureAwait(false);

            await LinkTopKnowledgeResultsAsync(
                queriesDatabase,
                querySegment,
                retrievalResults,
                request.Method,
                linkTopResults,
                now,
                cancellationToken).ConfigureAwait(false);

            var relatedQueryResults = await CollectRelatedQueryResultsAsync(
                queriesDatabase,
                knowledgeDatabase,
                querySegment.Id,
                relatedDepth,
                cancellationToken).ConfigureAwait(false);

            return new KnowledgeWorldQueryResult
            {
                RootPath = layout.RootPath,
                QueryPath = queryPath,
                KnowledgeDatabasePath = layout.KnowledgeDatabasePath,
                QueriesDatabasePath = layout.QueriesDatabasePath,
                QueryId = querySegment.Id,
                Query = query,
                Method = request.Method,
                QueriedAtUtc = now,
                Results = retrievalResults.Select(result => CreateSearchHit(result, request.Method)).ToArray(),
                SimilarQueries = similarQueries,
                RelatedQueryResults = relatedQueryResults
            };
        }

        private EmbeddingModelDefinition ResolveSelectedEmbeddingModel()
        {
            var configuration = _ragConfigurationRepository.Load();
            if (string.IsNullOrWhiteSpace(configuration.SelectedEmbeddingModelKey))
                throw new InvalidOperationException("KnowledgeWorld requires a selected embedding model in Semantic Search preferences.");

            var model = _embeddingModelRepository.Load().FirstOrDefault(item =>
                string.Equals(item.Key, configuration.SelectedEmbeddingModelKey, StringComparison.Ordinal));
            if (model == null)
                throw new InvalidOperationException("KnowledgeWorld selected embedding model was not found.");

            if (!model.IsFullyConfigured)
                throw new InvalidOperationException($"KnowledgeWorld embedding model '{model.DisplayName}' is not fully configured.");

            return model;
        }

        private ApiEmbeddingRequest CreateEmbeddingTemplate(EmbeddingModelDefinition model, EmbeddingInput input)
        {
            return _embeddingModelService.CreateRequest(model, input);
        }

        private static async Task<AerialDatabase> OpenDatabaseAsync(
            AerialCityEngine engine,
            string databaseRootPath,
            string databaseName,
            CancellationToken cancellationToken)
        {
            var createDatabase = engine.CreateDatabase();
            return await createDatabase(new DatabaseOptions
            {
                Name = databaseName,
                Storage = new StorageOptions
                {
                    BasePath = databaseRootPath
                }
            }, cancellationToken).ConfigureAwait(false);
        }

        private static async Task EmbedSegmentAsync(
            AerialCityEngine engine,
            ApiEmbeddingRequest template,
            Segment segment,
            CancellationToken cancellationToken)
        {
            var embed = engine.EmbedContent();
            await embed(new ApiEmbeddingRequest
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
            }, cancellationToken).ConfigureAwait(false);
        }

        private static async Task<IReadOnlyList<RetrievalResult>> RetrieveKnowledgeAsync(
            AerialCityEngine engine,
            AerialDatabase knowledgeDatabase,
            string query,
            Segment querySegment,
            KnowledgeWorldRetrievalMethod method,
            int topK,
            CancellationToken cancellationToken)
        {
            var retrieve = engine.Retrieve();
            var retrievalQuery = new RetrievalQuery
            {
                TextQuery = method == KnowledgeWorldRetrievalMethod.BM25 ? query : null,
                QueryVector = method == KnowledgeWorldRetrievalMethod.Cosine ? querySegment.Embedding : null,
                TopK = topK,
                CollectionFilter = KnowledgeCollectionName
            };

            return await retrieve(knowledgeDatabase, retrievalQuery, cancellationToken).ConfigureAwait(false);
        }

        private static async Task LinkTopKnowledgeResultsAsync(
            AerialDatabase queriesDatabase,
            Segment querySegment,
            IReadOnlyList<RetrievalResult> retrievalResults,
            KnowledgeWorldRetrievalMethod method,
            int linkTopResults,
            DateTimeOffset linkedAtUtc,
            CancellationToken cancellationToken)
        {
            if (linkTopResults <= 0 || querySegment.Embedding is not { } queryVector)
                return;

            for (var index = 0; index < Math.Min(linkTopResults, retrievalResults.Count); index++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var result = retrievalResults[index];
                if (result.Segment.Embedding is not { } targetVector)
                    continue;

                var metadata = new Dictionary<string, object>(StringComparer.Ordinal)
                {
                    ["knowledgeWorldRelation"] = QueryResultRelation,
                    ["knowledgeWorldRank"] = index + 1,
                    ["knowledgeWorldScore"] = result.Score.ToString(CultureInfo.InvariantCulture),
                    ["knowledgeWorldRetrievalMethod"] = method.ToString(),
                    ["knowledgeWorldLinkedAtUtc"] = linkedAtUtc.ToString("O", CultureInfo.InvariantCulture)
                };

                await queriesDatabase.AddEdgeAsync(
                    querySegment.Id,
                    result.Segment.Id,
                    EdgeKind.Custom,
                    NormalizeResultWeight(result.Score, method),
                    metadata,
                    queryVector,
                    targetVector,
                    cancellationToken).ConfigureAwait(false);
            }
        }

        private static async Task<IReadOnlyList<KnowledgeWorldSimilarQuery>> LinkSimilarQueriesAsync(
            AerialDatabase queriesDatabase,
            Segment querySegment,
            float threshold,
            DateTimeOffset linkedAtUtc,
            CancellationToken cancellationToken)
        {
            if (querySegment.Embedding is not { } queryVector)
                return [];

            var similarQueries = new List<KnowledgeWorldSimilarQuery>();
            await foreach (var id in queriesDatabase.ListSegmentIdsAsync(cancellationToken))
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (id == querySegment.Id)
                    continue;

                var oldQuery = await queriesDatabase.ReadSegmentAsync(id, cancellationToken).ConfigureAwait(false);
                if (oldQuery?.Embedding is not { } oldVector || oldVector.Dimensions != queryVector.Dimensions)
                    continue;

                if (!IsKnowledgeWorldQuery(oldQuery))
                    continue;

                var similarity = queryVector.CosineSimilarity(oldVector);
                if (similarity < threshold)
                    continue;

                var weight = Math.Clamp(similarity, 0.0f, 1.0f);
                var metadata = new Dictionary<string, object>(StringComparer.Ordinal)
                {
                    ["knowledgeWorldRelation"] = QuerySimilarityRelation,
                    ["knowledgeWorldSimilarity"] = similarity.ToString(CultureInfo.InvariantCulture),
                    ["knowledgeWorldLinkedAtUtc"] = linkedAtUtc.ToString("O", CultureInfo.InvariantCulture)
                };

                await queriesDatabase.AddEdgeAsync(
                    querySegment.Id,
                    oldQuery.Id,
                    EdgeKind.SimilarTo,
                    weight,
                    metadata,
                    queryVector,
                    oldVector,
                    cancellationToken).ConfigureAwait(false);

                await queriesDatabase.AddEdgeAsync(
                    oldQuery.Id,
                    querySegment.Id,
                    EdgeKind.SimilarTo,
                    weight,
                    metadata,
                    oldVector,
                    queryVector,
                    cancellationToken).ConfigureAwait(false);

                similarQueries.Add(new KnowledgeWorldSimilarQuery
                {
                    QueryId = oldQuery.Id,
                    Query = oldQuery.Content,
                    Similarity = similarity,
                    CreatedAt = oldQuery.CreatedAt,
                    QueryPath = oldQuery.SourceUri
                });
            }

            return similarQueries
                .OrderByDescending(item => item.Similarity)
                .ToArray();
        }

        private static async Task<IReadOnlyList<KnowledgeWorldRelatedQueryResult>> CollectRelatedQueryResultsAsync(
            AerialDatabase queriesDatabase,
            AerialDatabase knowledgeDatabase,
            AerialId queryId,
            int maxDepth,
            CancellationToken cancellationToken)
        {
            if (maxDepth <= 0)
                return [];

            var relatedQueries = TraverseRelatedQueries(queriesDatabase, queryId, maxDepth);
            var results = new List<KnowledgeWorldRelatedQueryResult>();

            foreach (var (relatedQueryId, depth) in relatedQueries.OrderBy(item => item.Value).Select(item => (item.Key, item.Value)))
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (relatedQueryId == queryId)
                    continue;

                var relatedQuery = await queriesDatabase.ReadSegmentAsync(relatedQueryId, cancellationToken).ConfigureAwait(false);
                if (relatedQuery == null)
                    continue;

                var hits = new List<KnowledgeWorldSearchHit>();
                foreach (var edge in queriesDatabase.GetOutgoingEdges(relatedQueryId, EdgeKind.Custom))
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    if (!IsKnowledgeResultEdge(edge))
                        continue;

                    var knowledgeSegment = await knowledgeDatabase.ReadSegmentAsync(edge.TargetId, cancellationToken).ConfigureAwait(false);
                    if (knowledgeSegment == null)
                        continue;

                    hits.Add(CreateSearchHit(knowledgeSegment, ReadEdgeScore(edge), edge.Weight));
                }

                if (hits.Count == 0)
                    continue;

                results.Add(new KnowledgeWorldRelatedQueryResult
                {
                    QueryId = relatedQuery.Id,
                    Query = relatedQuery.Content,
                    Depth = depth,
                    CreatedAt = relatedQuery.CreatedAt,
                    QueryPath = relatedQuery.SourceUri,
                    Results = hits
                });
            }

            return results;
        }

        private static Dictionary<AerialId, int> TraverseRelatedQueries(
            AerialDatabase queriesDatabase,
            AerialId startQueryId,
            int maxDepth)
        {
            var visited = new Dictionary<AerialId, int> { [startQueryId] = 0 };
            var queue = new Queue<(AerialId QueryId, int Depth)>();
            queue.Enqueue((startQueryId, 0));

            while (queue.Count > 0)
            {
                var (current, depth) = queue.Dequeue();
                if (depth >= maxDepth)
                    continue;

                foreach (var edge in EnumerateSimilarityEdges(queriesDatabase, current))
                {
                    var next = edge.SourceId == current ? edge.TargetId : edge.SourceId;
                    if (visited.ContainsKey(next))
                        continue;

                    visited[next] = depth + 1;
                    queue.Enqueue((next, depth + 1));
                }
            }

            return visited;
        }

        private static IEnumerable<GraphEdge> EnumerateSimilarityEdges(
            AerialDatabase queriesDatabase,
            AerialId queryId)
        {
            foreach (var edge in queriesDatabase.GetOutgoingEdges(queryId, EdgeKind.SimilarTo))
            {
                if (IsQuerySimilarityEdge(edge))
                    yield return edge;
            }

            foreach (var edge in queriesDatabase.GetIncomingEdges(queryId, EdgeKind.SimilarTo))
            {
                if (IsQuerySimilarityEdge(edge))
                    yield return edge;
            }
        }

        private static Segment CreateQuerySegment(
            string query,
            string queryPath,
            KnowledgeWorldRetrievalMethod method,
            int topK,
            DateTimeOffset queriedAtUtc)
        {
            return new Segment(SegmentKind.TextPassage, query)
            {
                SourceUri = queryPath,
                CollectionName = QueryCollectionName,
                Metadata =
                {
                    ["knowledgeWorldKind"] = "Query",
                    ["knowledgeWorldQueryPath"] = queryPath,
                    ["knowledgeWorldQueriedAtUtc"] = queriedAtUtc.ToString("O", CultureInfo.InvariantCulture),
                    ["knowledgeWorldRetrievalMethod"] = method.ToString(),
                    ["knowledgeWorldTopK"] = topK,
                    ["sourceKind"] = "knowledge-world-query",
                    ["sourceContent"] = query
                }
            };
        }

        private static Dictionary<string, object> CreateKnowledgeMetadata(
            string rawPath,
            string? originalSourcePath,
            DateTimeOffset ingestedAtUtc)
        {
            var metadata = new Dictionary<string, object>(StringComparer.Ordinal)
            {
                ["knowledgeWorldKind"] = "Knowledge",
                ["knowledgeWorldRawPath"] = rawPath,
                ["knowledgeWorldIngestedAtUtc"] = ingestedAtUtc.ToString("O", CultureInfo.InvariantCulture),
                ["knowledgeWorldRawFileName"] = Path.GetFileName(rawPath),
                ["sourceKind"] = "knowledge-world-raw",
                ["path"] = rawPath,
                ["fileName"] = Path.GetFileName(rawPath)
            };

            if (!string.IsNullOrWhiteSpace(originalSourcePath))
            {
                metadata["knowledgeWorldOriginalSourcePath"] = originalSourcePath;
                metadata["knowledgeWorldOriginalSourceFileName"] = Path.GetFileName(originalSourcePath);
            }

            return metadata;
        }

        private static void EnrichKnowledgeSegment(
            Segment segment,
            string rawPath,
            string? originalSourcePath,
            DateTimeOffset ingestedAtUtc)
        {
            segment.Metadata["knowledgeWorldKind"] = "Knowledge";
            segment.Metadata["knowledgeWorldRawPath"] = rawPath;
            segment.Metadata["knowledgeWorldRawFileName"] = Path.GetFileName(rawPath);
            segment.Metadata["knowledgeWorldIngestedAtUtc"] = ingestedAtUtc.ToString("O", CultureInfo.InvariantCulture);
            segment.Metadata["sourceContent"] = segment.Content;
            segment.Metadata["path"] = rawPath;
            segment.Metadata["fileName"] = Path.GetFileName(rawPath);
            segment.CollectionName = KnowledgeCollectionName;

            if (!string.IsNullOrWhiteSpace(originalSourcePath))
            {
                segment.Metadata["knowledgeWorldOriginalSourcePath"] = originalSourcePath;
                segment.Metadata["knowledgeWorldOriginalSourceFileName"] = Path.GetFileName(originalSourcePath);
            }

            segment.Metadata["knowledgeWorldSegmentHash"] = ComputeSha256(segment.Content);
        }

        private static KnowledgeWorldSearchHit CreateSearchHit(
            RetrievalResult result,
            KnowledgeWorldRetrievalMethod method)
        {
            return CreateSearchHit(result.Segment, result.Score, NormalizeResultWeight(result.Score, method), result.ScoreBreakdown);
        }

        private static KnowledgeWorldSearchHit CreateSearchHit(Segment segment, float score, float edgeWeight)
        {
            return CreateSearchHit(segment, score, edgeWeight, new Dictionary<string, float>(StringComparer.Ordinal));
        }

        private static KnowledgeWorldSearchHit CreateSearchHit(
            Segment segment,
            float score,
            float edgeWeight,
            IReadOnlyDictionary<string, float> scoreBreakdown)
        {
            return new KnowledgeWorldSearchHit
            {
                SegmentId = segment.Id,
                Content = segment.Content,
                SourceUri = segment.SourceUri,
                StartOffset = segment.StartOffset,
                EndOffset = segment.EndOffset,
                CreatedAt = segment.CreatedAt,
                UpdatedAt = segment.UpdatedAt,
                Score = score,
                EdgeWeight = edgeWeight,
                ScoreBreakdown = new Dictionary<string, float>(scoreBreakdown, StringComparer.Ordinal),
                Metadata = new Dictionary<string, object>(segment.Metadata, StringComparer.Ordinal)
            };
        }

        private static bool IsKnowledgeWorldQuery(Segment segment)
        {
            return TryGetMetadataString(segment.Metadata, "knowledgeWorldKind", out var kind) &&
                string.Equals(kind, "Query", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsKnowledgeResultEdge(GraphEdge edge)
        {
            return TryGetMetadataString(edge.Properties, "knowledgeWorldRelation", out var relation) &&
                string.Equals(relation, QueryResultRelation, StringComparison.Ordinal);
        }

        private static bool IsQuerySimilarityEdge(GraphEdge edge)
        {
            return TryGetMetadataString(edge.Properties, "knowledgeWorldRelation", out var relation) &&
                string.Equals(relation, QuerySimilarityRelation, StringComparison.Ordinal);
        }

        private static bool TryGetMetadataString(
            IReadOnlyDictionary<string, object> metadata,
            string key,
            out string value)
        {
            value = string.Empty;

            if (!metadata.TryGetValue(key, out var raw))
            {
                foreach (var (candidateKey, candidateValue) in metadata)
                {
                    if (string.Equals(candidateKey, key, StringComparison.OrdinalIgnoreCase))
                    {
                        raw = candidateValue;
                        break;
                    }
                }
            }

            switch (raw)
            {
                case null:
                    return false;
                case string text when !string.IsNullOrWhiteSpace(text):
                    value = text;
                    return true;
                case string[] array when array.Length > 0 && !string.IsNullOrWhiteSpace(array[0]):
                    value = array[0];
                    return true;
                default:
                    var scalarText = raw.ToString();
                    if (string.IsNullOrWhiteSpace(scalarText))
                        return false;

                    value = scalarText;
                    return true;
            }
        }

        private static float ReadEdgeScore(GraphEdge edge)
        {
            if (TryGetMetadataString(edge.Properties, "knowledgeWorldScore", out var scoreText) &&
                float.TryParse(scoreText, NumberStyles.Float, CultureInfo.InvariantCulture, out var score))
            {
                return score;
            }

            return edge.Weight;
        }

        private static float NormalizeResultWeight(float score, KnowledgeWorldRetrievalMethod method)
        {
            if (float.IsNaN(score) || float.IsInfinity(score))
                return 0.0f;

            return method == KnowledgeWorldRetrievalMethod.BM25
                ? Math.Clamp(score / (score + 1.0f), 0.0f, 1.0f)
                : Math.Clamp((score + 1.0f) / 2.0f, 0.0f, 1.0f);
        }

        private static string CreateUniqueFilePath(
            string directoryPath,
            string? suggestedFileName,
            string fallbackFileName,
            string fallbackExtension,
            DateTimeOffset timestamp)
        {
            Directory.CreateDirectory(directoryPath);

            var fileName = string.IsNullOrWhiteSpace(suggestedFileName)
                ? fallbackFileName
                : Path.GetFileName(suggestedFileName.Trim());
            if (string.IsNullOrWhiteSpace(fileName))
                fileName = fallbackFileName;

            var stem = SanitizeFileName(Path.GetFileNameWithoutExtension(fileName));
            var extension = Path.GetExtension(fileName);
            if (string.IsNullOrWhiteSpace(extension))
                extension = string.IsNullOrWhiteSpace(fallbackExtension) ? ".txt" : fallbackExtension;
            if (!extension.StartsWith(".", StringComparison.Ordinal))
                extension = "." + extension;

            var timestampText = timestamp.ToString("yyyyMMdd_HHmmss_fff", CultureInfo.InvariantCulture);
            for (var attempt = 0; attempt < 1000; attempt++)
            {
                var suffix = attempt == 0 ? string.Empty : "_" + attempt.ToString(CultureInfo.InvariantCulture);
                var path = Path.Combine(directoryPath, $"{timestampText}_{stem}{suffix}{extension}");
                if (!File.Exists(path))
                    return path;
            }

            return Path.Combine(directoryPath, $"{timestampText}_{stem}_{Guid.NewGuid():N}{extension}");
        }

        private static string SanitizeFileName(string value)
        {
            var text = string.IsNullOrWhiteSpace(value) ? "knowledge" : value.Trim();
            var invalid = Path.GetInvalidFileNameChars();
            var builder = new StringBuilder(text.Length);
            foreach (var ch in text)
                builder.Append(invalid.Contains(ch) ? '_' : ch);

            var sanitized = builder.ToString();
            return sanitized.Length <= 80 ? sanitized : sanitized[..80];
        }

        private static async Task CopyFileAsync(
            string sourcePath,
            string destinationPath,
            CancellationToken cancellationToken)
        {
            await using var source = new FileStream(
                sourcePath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read);
            await using var destination = new FileStream(
                destinationPath,
                FileMode.CreateNew,
                FileAccess.Write,
                FileShare.None);

            await source.CopyToAsync(destination, cancellationToken).ConfigureAwait(false);
        }

        private static string ComputeSha256(string text)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(text));
            return Convert.ToHexString(bytes).ToLowerInvariant();
        }
    }
}
