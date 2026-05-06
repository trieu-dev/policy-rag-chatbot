using Qdrant.Client;
using Qdrant.Client.Grpc;
using Week03.Models;

namespace Week03;

/// <summary>
/// Wraps the Qdrant gRPC client to:
///   1. Create/ensure a vector collection exists
///   2. Upsert PolicyChunks (with embeddings) as Qdrant Points
///   3. Search by semantic similarity, optionally filtering by source file
/// </summary>
public class QdrantRepository
{
    private readonly QdrantClient _client;
    private readonly string _collectionName;
    private readonly uint _vectorSize;

    public QdrantRepository(string qdrantUrl, string collectionName, uint vectorSize)
    {
        var uri = new Uri(qdrantUrl);
        // Qdrant .NET client is a gRPC client, so it must connect to the gRPC port (6334).
        // The URL in .env is usually the HTTP dashboard URL (6333).
        _client = new QdrantClient(uri.Host, 6334);
        _collectionName = collectionName;
        _vectorSize = vectorSize;
    }

    /// <summary>
    /// Creates the collection if it does not already exist.
    /// Uses Cosine distance — the standard for text embeddings.
    /// </summary>
    public async Task EnsureCollectionAsync()
    {
        var exists = await _client.CollectionExistsAsync(_collectionName);
        if (exists)
        {
            Console.WriteLine($"  Collection '{_collectionName}' already exists. Skipping creation.");
            return;
        }

        await _client.CreateCollectionAsync(_collectionName, new VectorParams
        {
            Size = _vectorSize,
            Distance = Distance.Cosine
        });
        Console.WriteLine($"  Collection '{_collectionName}' created (dim={_vectorSize}, cosine).");
    }

    /// <summary>
    /// Upserts a batch of chunks with their embedding vectors.
    /// Each Qdrant point carries the full chunk metadata as a payload.
    /// </summary>
    public async Task UpsertChunksAsync(List<PolicyChunk> chunks, float[][] embeddings)
    {
        if (chunks.Count != embeddings.Length)
            throw new ArgumentException("Chunks and embeddings counts must match.");

        var points = chunks.Select((chunk, i) => new PointStruct
        {
            Id = new PointId { Uuid = ToUuid(chunk.Id) },
            Vectors = embeddings[i],
            Payload =
            {
                ["id"]           = chunk.Id,
                ["text"]         = chunk.Text,
                ["source_file"]  = chunk.Metadata.SourceFile,
                ["page_number"]  = chunk.Metadata.PageNumber,
                ["section_title"]= chunk.Metadata.SectionTitle ?? "",
                ["ingested_at"]  = chunk.Metadata.IngestedAt?.ToString("o") ?? ""
            }
        }).ToList();

        await _client.UpsertAsync(_collectionName, points);
        Console.WriteLine($"  Upserted {points.Count} points into '{_collectionName}'.");
    }

    /// <summary>
    /// Searches the collection by a query embedding vector.
    /// Optionally filters results to a specific source PDF file.
    /// </summary>
    public async Task<List<SearchResult>> SearchAsync(
        float[] queryVector,
        int topK = 5,
        string? sourceFileFilter = null)
    {
        Filter? filter = null;
        if (sourceFileFilter is not null)
        {
            filter = new Filter
            {
                Must =
                {
                    new Condition
                    {
                        Field = new FieldCondition
                        {
                            Key = "source_file",
                            Match = new Match { Text = sourceFileFilter }
                        }
                    }
                }
            };
        }

        var results = await _client.SearchAsync(
            collectionName: _collectionName,
            vector: queryVector,
            limit: (ulong)topK,
            filter: filter,
            payloadSelector: true);

        return results.Select(r => new SearchResult(
            Id: r.Payload["id"].StringValue,
            Score: r.Score,
            Text: r.Payload["text"].StringValue,
            SourceFile: r.Payload["source_file"].StringValue,
            PageNumber: (int)r.Payload["page_number"].IntegerValue,
            SectionTitle: r.Payload["section_title"].StringValue
        )).ToList();
    }

    /// <summary>Deterministic UUID from an arbitrary string ID (UUID v5 namespace-based).</summary>
    private static string ToUuid(string id)
    {
        // Simple approach: use a hash-derived UUID so the same chunk ID always maps to the same UUID
        using var sha = System.Security.Cryptography.SHA1.Create();
        var hash = sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(id));
        // Format first 16 bytes as a UUID (version 5 style)
        hash[6] = (byte)((hash[6] & 0x0F) | 0x50); // version 5
        hash[8] = (byte)((hash[8] & 0x3F) | 0x80); // variant
        return new Guid(hash.Take(16).ToArray()).ToString();
    }
}

public record SearchResult(
    string Id,
    float Score,
    string Text,
    string SourceFile,
    int PageNumber,
    string SectionTitle
);
