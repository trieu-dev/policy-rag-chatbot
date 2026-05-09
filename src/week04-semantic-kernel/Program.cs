using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.KernelMemory;
using Microsoft.KernelMemory.AI.Ollama;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Week04.Plugins;

namespace Week04;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Week 4 - Semantic Kernel Orchestration\n");

        Config.Load();

        string chatModel = Config.GetEnv("ANTHROPIC_MODEL", "llama3");
        string ollamaEndpoint = Config.GetEnv("OLLAMA_ENDPOINT", "http://localhost:11434");

        string embedModel = Config.GetEnv("OPENAI_MODEL", "nomic-embed-text");
        string qdrantUrl = Config.GetEnv("QDRANT_URL", "http://localhost:6333");

        // 1. Setup Kernel Memory with Qdrant and local embeddings
        var memoryBuilder = new KernelMemoryBuilder()
            .WithOllamaTextGeneration(new OllamaConfig
            {
                Endpoint = ollamaEndpoint,
                TextModel = new OllamaModelConfig(chatModel)
            })
            .WithOllamaTextEmbeddingGeneration(new OllamaConfig
            {
                Endpoint = ollamaEndpoint,
                EmbeddingModel = new OllamaModelConfig(embedModel)
            })
            .WithQdrantMemoryDb(qdrantUrl)
            .WithSimpleFileStorage("data");

        var memory = memoryBuilder.Build<MemoryServerless>();

        // 2. Setup Semantic Kernel
        var builder = Kernel.CreateBuilder();
        builder.AddOpenAIChatCompletion(
            modelId: chatModel,
            apiKey: "dummy",
            endpoint: new Uri($"{ollamaEndpoint}/v1")
        );

        var kernel = builder.Build();

        // 3. Import RAG Plugin
        var ragPlugin = new RagPlugin(memory);
        kernel.Plugins.AddFromObject(ragPlugin, "RagPlugin");

        // 4. Setup Conversation History
        var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
        var chatHistory = new ChatHistory("You are a helpful assistant answering policy questions based on provided context. Always cite your sources.");

        // Integration test mode check
        if (args.Length > 0 && args[0] == "--test")
        {
            await RagIntegrationTests.RunAsync(memory);
            return;
        }

        Console.WriteLine("Interactive chat started. Type 'exit' or 'quit' to stop.");
        
        while (true)
        {
            Console.Write("\n> ");
            var userInput = Console.ReadLine();

            if (userInput == null) break;
            if (string.IsNullOrWhiteSpace(userInput)) continue;
            if (userInput.ToLower() == "exit" || userInput.ToLower() == "quit") break;

            // Search memory directly or let SK call it? 
            // The prompt asks to "implement RAG plugin using SK functions: SearchMemory -> BuildPrompt -> Ask".
            // Let's call the plugin directly to get context, then inject it into the prompt.
            Console.WriteLine("Searching memory...");
            string context = await ragPlugin.SearchMemoryAsync(userInput);

            // Construct turn context
            string systemPrompt = $@"Use the following context to answer the user's question. 
Context:
{context}

If the answer is not contained in the context, say so.";

            chatHistory.AddUserMessage(userInput);

            // Create a temporary history with context for this turn
            var turnHistory = new ChatHistory(systemPrompt);
            turnHistory.AddRange(chatHistory);

            Console.WriteLine("Generating answer...");
            
            var responseStream = chatCompletionService.GetStreamingChatMessageContentsAsync(
                chatHistory: turnHistory,
                kernel: kernel
            );

            string fullResponse = "";
            await foreach (var chunk in responseStream)
            {
                Console.Write(chunk.Content);
                fullResponse += chunk.Content;
            }
            Console.WriteLine();

            // Add the assistant's response to the actual history (without the context injection to save tokens)
            chatHistory.AddAssistantMessage(fullResponse);
        }
    }
}
