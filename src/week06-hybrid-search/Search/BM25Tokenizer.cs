using System.Text.RegularExpressions;

namespace Week06.Search;

public class BM25Tokenizer
{
    private static readonly HashSet<string> StopWords = new()
    {
        "a", "an", "the", "and", "or", "but", "if", "then", "else", "when", 
        "at", "by", "from", "for", "in", "out", "on", "off", "over", "under",
        "is", "was", "are", "were", "be", "been", "being", "to", "of"
    };

    public static Dictionary<uint, float> Tokenize(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return new();

        // Basic tokenization: lowercase, remove punctuation, split by space
        var cleanText = Regex.Replace(text.ToLower(), @"[^\w\s]", "");
        var words = cleanText.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        var termCounts = new Dictionary<string, int>();
        foreach (var word in words)
        {
            if (word.Length < 2 || StopWords.Contains(word)) continue;
            
            if (!termCounts.ContainsKey(word)) termCounts[word] = 0;
            termCounts[word]++;
        }

        // Convert word strings to uint IDs (simple hash for demonstration)
        // In a real scenario, you'd use a stable vocabulary ID
        var sparseVector = new Dictionary<uint, float>();
        foreach (var (word, count) in termCounts)
        {
            var id = (uint)word.GetHashCode();
            sparseVector[id] = (float)count;
        }

        return sparseVector;
    }
}
