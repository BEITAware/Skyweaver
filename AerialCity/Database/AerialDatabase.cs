using AerialCity.Core.Exceptions;
using AerialCity.Core.Primitives;
using AerialCity.Core.Storage;
using AerialCity.Database.Schema;
using AerialCity.GraphStore.Index;
using AerialCity.GraphStore.Model;
using AerialCity.Retrieval.Scoring;
using AerialCity.VectorStore.Index;
using AerialCity.VectorStore.Similarity;
using Microsoft.Extensions.Logging;

namespace AerialCity.Database;

/// <summary>
/// Configuration for creating a new AerialCity database.
/// </summary>
public sealed class DatabaseOptions
{
    /// <summary>Database name (used as directory name for file-backed storage).</summary>
    public required string Name { get; init; }

    /// <summary>Storage configuration.</summary>
    public StorageOptions Storage { get; init; } = new();

    /// <summary>Similarity metric for vector indexing. Default: Cosine.</summary>
    public ISimilarityMetric SimilarityMetric { get; init; } = new CosineSimilarity();

    /// <summary>HNSW index configuration.</summary>
    public HnswOptions HnswOptions { get; init; } = new();
}

/// <summary>
/// Represents an open AerialCity database instance, encapsulating the vector index,
/// graph index, storage engine, and BM25 corpus.
/// </summary>
public sealed class AerialDatabase : IAsyncDisposable
{
    /// <summary>Database name.</summary>
    public string Name { get; }

    internal IStorageEngine Storage { get; }
    internal HnswGraph VectorIndex { get; }
    internal AdjacencyListIndex GraphIndex { get; }
    internal Bm25Scorer Bm25 { get; }

    private readonly Dictionary<string, CollectionSchema> _collections = [];
    private readonly ILogger _logger;

    internal AerialDatabase(
        string name,
        IStorageEngine storage,
        HnswGraph vectorIndex,
        AdjacencyListIndex graphIndex,
        ILogger logger)
    {
        Name = name;
        Storage = storage;
        VectorIndex = vectorIndex;
        GraphIndex = graphIndex;
        Bm25 = new Bm25Scorer();
        _logger = logger;
    }

    /// <summary>Reads a segment by ID from this database.</summary>
    public Task<Segment?> ReadSegmentAsync(AerialId id, CancellationToken ct = default) =>
        Storage.ReadSegmentAsync(id, ct);

    /// <summary>Enumerates all segment IDs stored in this database.</summary>
    public IAsyncEnumerable<AerialId> ListSegmentIdsAsync(CancellationToken ct = default) =>
        Storage.ListSegmentIdsAsync(ct);

    /// <summary>Returns outgoing graph edges for a stored segment ID.</summary>
    public IReadOnlyList<GraphEdge> GetOutgoingEdges(AerialId segmentId, EdgeKind? kind = null) =>
        kind.HasValue ? GraphIndex.GetOutgoing(segmentId, kind.Value) : GraphIndex.GetOutgoing(segmentId);

    /// <summary>Returns incoming graph edges for a stored segment ID.</summary>
    public IReadOnlyList<GraphEdge> GetIncomingEdges(AerialId segmentId, EdgeKind? kind = null) =>
        kind.HasValue ? GraphIndex.GetIncoming(segmentId, kind.Value) : GraphIndex.GetIncoming(segmentId);

    /// <summary>
    /// Adds a relationship edge and persists it when source and target vectors are available.
    /// </summary>
    public async Task AddEdgeAsync(
        AerialId sourceSegmentId,
        AerialId targetSegmentId,
        EdgeKind kind,
        float weight = 1.0f,
        IReadOnlyDictionary<string, object>? metadata = null,
        EmbeddingVector? sourceVector = null,
        EmbeddingVector? targetVector = null,
        CancellationToken ct = default)
    {
        var edge = CreateGraphEdge(sourceSegmentId, targetSegmentId, kind, weight, metadata);
        GraphIndex.AddEdge(edge);

        var resolvedSourceVector = sourceVector;
        if (!resolvedSourceVector.HasValue)
            resolvedSourceVector = (await Storage.ReadSegmentAsync(sourceSegmentId, ct).ConfigureAwait(false))?.Embedding;

        var resolvedTargetVector = targetVector;
        if (!resolvedTargetVector.HasValue)
            resolvedTargetVector = (await Storage.ReadSegmentAsync(targetSegmentId, ct).ConfigureAwait(false))?.Embedding;

        if (resolvedSourceVector.HasValue && resolvedTargetVector.HasValue)
        {
            await Storage.WriteGraphEdgeAsync(
                edge,
                resolvedSourceVector.Value,
                resolvedTargetVector.Value,
                ct).ConfigureAwait(false);
        }
    }

    /// <summary>Creates a new collection with the given schema.</summary>
    internal void CreateCollection(CollectionSchema schema)
    {
        if (_collections.ContainsKey(schema.Name))
            throw new SchemaException($"Collection '{schema.Name}' already exists.");
        _collections[schema.Name] = schema;
        _logger.LogInformation("Created collection '{Name}'", schema.Name);
    }

    /// <summary>Gets a collection schema by name.</summary>
    internal CollectionSchema? GetCollection(string name) =>
        _collections.GetValueOrDefault(name);

    /// <summary>Lists all collection names.</summary>
    internal IReadOnlyList<string> ListCollections() => [.. _collections.Keys];

    /// <summary>Inserts a segment into the database.</summary>
    internal async Task InsertAsync(Segment segment, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(segment);

        await Storage.WriteSegmentAsync(segment, ct);
        IndexStoredSegment(segment);

        _logger.LogDebug("Inserted segment {Id}", segment.Id);
    }

    /// <summary>Updates an existing segment.</summary>
    internal async Task UpdateAsync(AerialId id, Segment updated, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(updated);

        var existing = await Storage.ReadSegmentAsync(id, ct)
            ?? throw new NotFoundException($"Segment {id} not found.", id.ToString());

        // Remove old indexes
        VectorIndex.Remove(id);
        Bm25.RemoveDocument(existing);

        // Preserve the addressed entity identity even when the caller supplies
        // a newly-created Segment instance as the updated data.
        var replacement = CreateReplacementSegment(id, existing, updated);
        await Storage.WriteSegmentAsync(replacement, ct);

        // Re-index
        IndexStoredSegment(replacement);

        _logger.LogDebug("Updated segment {Id}", id);
    }

    /// <summary>Deletes a segment by ID.</summary>
    internal async Task DeleteAsync(AerialId id, CancellationToken ct = default)
    {
        var existing = await Storage.ReadSegmentAsync(id, ct);
        if (existing is null) return;

        VectorIndex.Remove(id);
        Bm25.RemoveDocument(existing);
        GraphIndex.RemoveNode(id);
        await Storage.DeleteSegmentAsync(id, ct);

        _logger.LogDebug("Deleted segment {Id}", id);
    }

    /// <summary>Adds a relationship edge between two segments.</summary>
    internal void AddEdge(AerialId sourceSegmentId, AerialId targetSegmentId, EdgeKind kind, float weight = 1.0f)
    {
        AddEdgeAsync(sourceSegmentId, targetSegmentId, kind, weight).GetAwaiter().GetResult();
    }

    private static string GetLabel(Segment s) =>
        s.Metadata.TryGetValue("symbolName", out var name) ? name?.ToString() ?? "" : "";

    internal void IndexStoredSegment(Segment segment)
    {
        if (segment.Embedding is { } embeddingVec)
            VectorIndex.Add(segment.Id, in embeddingVec);

        Bm25.AddDocument(segment);

        var node = new GraphNode(segment.Id, segment.Id) { Label = GetLabel(segment) };
        GraphIndex.AddNode(node);
    }

    internal void IndexGraphEdge(GraphEdge edge)
    {
        GraphIndex.AddEdge(edge);
    }

    private static GraphEdge CreateGraphEdge(
        AerialId sourceSegmentId,
        AerialId targetSegmentId,
        EdgeKind kind,
        float weight,
        IReadOnlyDictionary<string, object>? metadata)
    {
        var edge = new GraphEdge(sourceSegmentId, targetSegmentId, kind)
        {
            Weight = Math.Clamp(weight, 0.0f, 1.0f)
        };

        if (metadata is not null)
        {
            foreach (var (key, value) in metadata)
                edge.Properties[key] = value;
        }

        return edge;
    }

    private static Segment CreateReplacementSegment(AerialId id, Segment existing, Segment updated) =>
        new(id, updated.Kind, updated.Content, existing.CreatedAt)
        {
            BinaryContent = updated.BinaryContent,
            Embedding = updated.Embedding,
            SourceUri = updated.SourceUri,
            StartOffset = updated.StartOffset,
            EndOffset = updated.EndOffset,
            CollectionName = updated.CollectionName,
            Metadata = new Dictionary<string, object>(updated.Metadata),
            UpdatedAt = DateTimeOffset.UtcNow
        };

    public async ValueTask DisposeAsync()
    {
        VectorIndex.Dispose();
        GraphIndex.Dispose();
        await Storage.DisposeAsync();
    }
}
