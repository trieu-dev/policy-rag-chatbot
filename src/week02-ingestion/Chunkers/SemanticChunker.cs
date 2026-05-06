using System.Text.RegularExpressions;
using Week02.Models;

namespace Week02.Chunkers;

public class SemanticChunker : IChunker
{
    // Regex to find "Section X.X" or "Article X" patterns
    private static readonly Regex SectionRegex = new Regex(@"(Section\s+\d+(\.\d+)?|Article\s+\d+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public IEnumerable<PolicyChunk> Chunk(string sourceFile, int pageNumber, string text)
    {
        var matches = SectionRegex.Matches(text);
        if (matches.Count == 0)
        {
            // Fallback: treat whole page as one chunk if no sections found
            yield return CreateChunk(sourceFile, pageNumber, 0, text, "Full Page");
            yield break;
        }

        for (int i = 0; i < matches.Count; i++)
        {
            var currentMatch = matches[i];
            int start = currentMatch.Index;
            int end = (i + 1 < matches.Count) ? matches[i + 1].Index : text.Length;
            
            var chunkText = text.Substring(start, end - start).Trim();
            var sectionTitle = currentMatch.Value;

            yield return CreateChunk(sourceFile, pageNumber, i, chunkText, sectionTitle);
        }
    }

    private PolicyChunk CreateChunk(string sourceFile, int pageNumber, int idx, string text, string section)
    {
        var id = $"{Path.GetFileName(sourceFile)}-p{pageNumber}-s{idx}";
        var metadata = new ChunkMetadata(sourceFile, pageNumber, section, DateTime.UtcNow);
        return new PolicyChunk(id, text, metadata);
    }
}
