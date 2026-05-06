using Week02.Models;

namespace Week02.Chunkers;

public class FixedSizeChunker : IChunker
{
    private readonly int _chunkSize;
    private readonly int _overlap;

    public FixedSizeChunker(int chunkSize = 1000, int overlap = 200)
    {
        _chunkSize = chunkSize;
        _overlap = overlap;
    }

    public IEnumerable<PolicyChunk> Chunk(string sourceFile, int pageNumber, string text)
    {
        if (string.IsNullOrWhiteSpace(text)) yield break;

        int start = 0;
        int chunkIdx = 0;

        while (start < text.Length)
        {
            int length = Math.Min(_chunkSize, text.Length - start);
            var chunkText = text.Substring(start, length);
            
            var id = $"{Path.GetFileName(sourceFile)}-p{pageNumber}-c{chunkIdx++}";
            var metadata = new ChunkMetadata(sourceFile, pageNumber, IngestedAt: DateTime.UtcNow);
            
            yield return new PolicyChunk(id, chunkText, metadata);

            if (start + length >= text.Length) break;
            
            start += (_chunkSize - _overlap);
        }
    }
}
