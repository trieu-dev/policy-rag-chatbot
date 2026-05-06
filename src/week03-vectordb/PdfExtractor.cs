using UglyToad.PdfPig;
using Week03.Models;

namespace Week03;

/// <summary>
/// Extracts text page-by-page from a PDF file using PdfPig.
/// Reuses the same approach as week02.
/// </summary>
public class PdfExtractor
{
    public List<(int PageNumber, string Text)> ExtractText(string filePath)
    {
        var results = new List<(int PageNumber, string Text)>();

        using var document = PdfDocument.Open(filePath);
        foreach (var page in document.GetPages())
        {
            var text = page.Text.Trim();
            if (!string.IsNullOrWhiteSpace(text))
                results.Add((page.Number, text));
        }

        return results;
    }

    /// <summary>
    /// Converts raw pages into PolicyChunk objects (one chunk per page).
    /// Week03 keeps chunking simple — each page is one chunk.
    /// Week04+ will use semantic chunking from week02.
    /// </summary>
    public List<PolicyChunk> ExtractChunks(string filePath)
    {
        var pages = ExtractText(filePath);
        var fileName = Path.GetFileName(filePath);

        return pages.Select((p, i) => new PolicyChunk(
            Id: $"{fileName}-p{p.PageNumber}",
            Text: p.Text,
            Metadata: new ChunkMetadata(
                SourceFile: fileName,
                PageNumber: p.PageNumber,
                SectionTitle: $"Page {p.PageNumber}",
                IngestedAt: DateTime.UtcNow
            )
        )).ToList();
    }
}
