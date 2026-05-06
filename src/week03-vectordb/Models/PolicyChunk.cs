namespace Week03.Models;

/// <summary>
/// Represents a chunk of text extracted from a policy PDF,
/// ready to be embedded and stored in Qdrant.
/// </summary>
public record PolicyChunk(
    string Id,
    string Text,
    ChunkMetadata Metadata
);

public record ChunkMetadata(
    string SourceFile,
    int PageNumber,
    string? SectionTitle = null,
    DateTime? IngestedAt = null
);
