using Week02.Models;

namespace Week02.Chunkers;

public interface IChunker
{
    IEnumerable<PolicyChunk> Chunk(string sourceFile, int pageNumber, string text);
}
