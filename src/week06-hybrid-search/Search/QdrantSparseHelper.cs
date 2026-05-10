using Qdrant.Client;
using Qdrant.Client.Grpc;

namespace Week06.Search;

public static class QdrantSparseHelper
{
    public static async Task EnsureSparseVectorEnabledAsync(string endpoint, string collectionName)
    {
        var uri = new Uri(endpoint);
        var client = new QdrantClient(uri.Host, uri.Port, https: uri.Scheme == "https");

        var collections = await client.ListCollectionsAsync();
        bool exists = collections.Contains(collectionName);

        if (!exists)
        {
            Console.WriteLine($"Creating collection '{collectionName}' with sparse vector support...");
            await client.CreateCollectionAsync(collectionName, 
                new VectorParams { Size = 768, Distance = Distance.Cosine }, // Dense (nomic-embed-text)
                sparseVectorsConfig: new SparseVectorConfig
                {
                    Map = 
                    {
                        { "bm25", new SparseVectorParams() }
                    }
                });
            Console.WriteLine("Collection created.");
        }
        else
        {
            Console.WriteLine($"Collection '{collectionName}' already exists.");
            // Note: In a production scenario, you might need to update the collection 
            // if sparse vectors weren't already enabled.
        }
    }
}
