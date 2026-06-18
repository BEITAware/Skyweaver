using AerialCity.Core.Primitives;

namespace Ferrita.Services.KnowledgeWorld
{
    public enum KnowledgeWorldRetrievalMethod
    {
        BM25 = 0,
        Cosine = 1
    }

    public sealed class KnowledgeWorldAddRequest
    {
        public string? FilePath { get; init; }

        public string? Text { get; init; }

        public string? SuggestedFileName { get; init; }
    }

    public sealed class KnowledgeWorldAddResult
    {
        public required string RootPath { get; init; }

        public required string RawPath { get; init; }

        public required string KnowledgeDatabasePath { get; init; }

        public string? OriginalSourcePath { get; init; }

        public DateTimeOffset IngestedAtUtc { get; init; }

        public int SegmentCount { get; init; }

        public IReadOnlyList<AerialId> SegmentIds { get; init; } = [];
    }

    public sealed class KnowledgeWorldQueryRequest
    {
        public required string Query { get; init; }

        public KnowledgeWorldRetrievalMethod Method { get; init; } = KnowledgeWorldRetrievalMethod.Cosine;

        public int TopK { get; init; } = 10;

        public int LinkTopResults { get; init; } = 5;

        public int RelatedQueryMaxDepth { get; init; } = 3;

        public float SimilarQueryThreshold { get; init; } = 0.8f;
    }

    public sealed class KnowledgeWorldQueryResult
    {
        public required string RootPath { get; init; }

        public required string QueryPath { get; init; }

        public required string KnowledgeDatabasePath { get; init; }

        public required string QueriesDatabasePath { get; init; }

        public required AerialId QueryId { get; init; }

        public required string Query { get; init; }

        public KnowledgeWorldRetrievalMethod Method { get; init; }

        public DateTimeOffset QueriedAtUtc { get; init; }

        public IReadOnlyList<KnowledgeWorldSearchHit> Results { get; init; } = [];

        public IReadOnlyList<KnowledgeWorldSimilarQuery> SimilarQueries { get; init; } = [];

        public IReadOnlyList<KnowledgeWorldRelatedQueryResult> RelatedQueryResults { get; init; } = [];
    }

    public sealed class KnowledgeWorldSearchHit
    {
        public required AerialId SegmentId { get; init; }

        public required string Content { get; init; }

        public string? SourceUri { get; init; }

        public int StartOffset { get; init; }

        public int EndOffset { get; init; }

        public DateTimeOffset CreatedAt { get; init; }

        public DateTimeOffset? UpdatedAt { get; init; }

        public float Score { get; init; }

        public float EdgeWeight { get; init; }

        public IReadOnlyDictionary<string, float> ScoreBreakdown { get; init; } =
            new Dictionary<string, float>(StringComparer.Ordinal);

        public IReadOnlyDictionary<string, object> Metadata { get; init; } =
            new Dictionary<string, object>(StringComparer.Ordinal);
    }

    public sealed class KnowledgeWorldSimilarQuery
    {
        public required AerialId QueryId { get; init; }

        public required string Query { get; init; }

        public float Similarity { get; init; }

        public DateTimeOffset CreatedAt { get; init; }

        public string? QueryPath { get; init; }
    }

    public sealed class KnowledgeWorldRelatedQueryResult
    {
        public required AerialId QueryId { get; init; }

        public required string Query { get; init; }

        public int Depth { get; init; }

        public DateTimeOffset CreatedAt { get; init; }

        public string? QueryPath { get; init; }

        public IReadOnlyList<KnowledgeWorldSearchHit> Results { get; init; } = [];
    }
}
