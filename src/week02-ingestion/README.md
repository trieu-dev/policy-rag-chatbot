# Week 2 — Document Ingestion

**Goal:** Turn real policy PDFs into clean, well-structured chunks ready for embedding.

## Daily Commit Plan

| Day | Commit | What you build |
|---|---|---|
| Mon | `feat(week02): add PdfExtractor using iTextSharp` | Extract raw text from a PDF |
| Tue | `feat(week02): add FixedSizeChunker with overlap` | Split text into fixed token windows |
| Wed | `feat(week02): add SemanticChunker splitting on headings` | Detect Section X.X patterns, split there |
| Thu | `feat(week02): add ChunkMetadata record (policy, section, date)` | Attach structured metadata to each chunk |
| Fri | `feat(week02): wire full ingestion pipeline PDF→chunks` | End-to-end pipeline, log chunk count |
| Sat | `test(week02): add tests for chunker edge cases` | Empty text, single word, very long section |
| Sun | `docs(week02): notes on chunking strategy tradeoffs` | Your learning notes |

## Deliverable

`dotnet run sample-policy.pdf` → prints structured chunk list with metadata to console.

## Files

```
week02-ingestion/
├── Week02.csproj
├── Program.cs
├── PdfExtractor.cs          # iTextSharp PDF → raw string
├── Chunkers/
│   ├── IChunker.cs          # interface: Chunk[] ChunkText(string text)
│   ├── FixedSizeChunker.cs  # split by token count + overlap
│   └── SemanticChunker.cs   # split on headings/section markers
├── Models/
│   └── PolicyChunk.cs       # Id, Text, Metadata, EmbeddingVector?
└── README.md
```
