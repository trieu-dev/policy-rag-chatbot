using UglyToad.PdfPig;
using Week02.Models;

namespace Week02;

public class PdfExtractor
{
    public List<(int PageNumber, string Text)> ExtractText(string filePath)
    {
        var results = new List<(int PageNumber, string Text)>();
        
        using var document = PdfDocument.Open(filePath);
        foreach (var page in document.GetPages())
        {
            // Extract text and perform basic normalization
            var text = page.Text.Trim();
            if (!string.IsNullOrWhiteSpace(text))
            {
                results.Add((page.Number, text));
            }
        }
        
        return results;
    }
}
