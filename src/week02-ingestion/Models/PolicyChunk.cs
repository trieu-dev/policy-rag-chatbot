namespace Week02.Models;

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
