using System;
using System.Threading.Tasks;
using Microsoft.KernelMemory;
using Week04.Plugins;

namespace Week04;

public static class RagIntegrationTests
{
    public static async Task RunAsync(IKernelMemory memory)
    {
        Console.WriteLine("\n--- Starting SK Pipeline Integration Test ---");
        
        var plugin = new RagPlugin(memory);
        string query = "What is the Transformer architecture?";
        
        Console.WriteLine($"Query: {query}");
        string result = await plugin.SearchMemoryAsync(query);
        
        Console.WriteLine("Search Result:");
        Console.WriteLine(result);

        if (result.Contains("Source:", StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine("\n[PASS] Pipeline returned citations.");
        }
        else
        {
            Console.WriteLine("\n[FAIL] No citations found in response.");
        }
        
        Console.WriteLine("--- Integration Test Complete ---\n");
    }
}
