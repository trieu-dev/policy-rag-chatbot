using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using Microsoft.KernelMemory;
using Microsoft.SemanticKernel;

namespace Week04.Plugins;

public class RagPlugin
{
    private readonly IKernelMemory _memory;

    public RagPlugin(IKernelMemory memory)
    {
        _memory = memory;
    }

    [KernelFunction("SearchMemoryAsync")]
    [Description("Searches the vector database for relevant context based on the user's query.")]
    public async Task<string> SearchMemoryAsync(
        [Description("The query or question to search context for")] string query)
    {
        // Search memory
        SearchResult result = await _memory.SearchAsync(query, limit: 3);

        if (result.Results.Count == 0)
        {
            return "No relevant context found in the memory.";
        }

        // Format results with citations
        var sb = new StringBuilder();
        sb.AppendLine("Relevant Context:");
        foreach (var citation in result.Results)
        {
            foreach (var partition in citation.Partitions)
            {
                sb.AppendLine($"[Source: {citation.SourceName}]");
                sb.AppendLine(partition.Text);
                sb.AppendLine("---");
            }
        }

        return sb.ToString();
    }
}
