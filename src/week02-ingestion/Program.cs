using Week02;
using Week02.Chunkers;
using Week02.Models;

// ---------------------------------------------------------------
// Week 2 Deliverable: Document Ingestion Pipeline
// PDF -> Pages -> Chunks -> Structured JSON/Console Output
// ---------------------------------------------------------------

if (args.Length == 0)
{
    Console.WriteLine("Usage: dotnet run <path-to-pdf>");
    return;
}

var pdfPath = args[0];
if (!File.Exists(pdfPath))
{
    Console.WriteLine($"Error: File not found at {pdfPath}");
    return;
}

Console.WriteLine($"=== Policy Ingestion Pipeline — Week 2 ===");
Console.WriteLine($"Source: {pdfPath}\n");

// 1. Extract
Console.WriteLine("Step 1: Extracting text from PDF...");
var extractor = new PdfExtractor();
var pages = extractor.ExtractText(pdfPath);
Console.WriteLine($"Extracted {pages.Count} pages.\n");

// 2. Chunk
Console.WriteLine("Step 2: Chunking text (using Semantic Chunker)...");
IChunker chunker = new SemanticChunker(); // You can swap this with FixedSizeChunker
var allChunks = new List<PolicyChunk>();

foreach (var page in pages)
{
    var chunks = chunker.Chunk(pdfPath, page.PageNumber, page.Text);
    allChunks.AddRange(chunks);
}

Console.WriteLine($"Created {allChunks.Count} chunks total.\n");

// 3. Output/Preview
Console.WriteLine("Step 3: Previewing first 3 chunks...");
foreach (var chunk in allChunks.Take(3))
{
    Console.WriteLine("------------------------------------------------");
    Console.WriteLine($"ID: {chunk.Id}");
    Console.WriteLine($"Section: {chunk.Metadata.SectionTitle ?? "N/A"} (Page {chunk.Metadata.PageNumber})");
    Console.WriteLine($"Content Preview: {chunk.Text.Replace("\n", " ").Substring(0, Math.Min(150, chunk.Text.Length))}...");
}

Console.WriteLine("\n--- Ingestion Complete. Week 3: Store these in Qdrant! ---");
