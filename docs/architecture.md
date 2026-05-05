# Architecture

> Update this file as you build. It is your living design document.

## System Overview

```
User
 │
 ▼
Next.js Frontend (port 3000)
 │  SSE stream of tokens
 ▼
ASP.NET Core 8 API (port 5000)
 │
 ├─► Redis ──────────────────── Semantic cache / embedding cache
 │
 ├─► Semantic Kernel
 │    ├─► Claude API ────────── LLM generation
 │    └─► Kernel Memory
 │         └─► Qdrant ──────── Vector storage + hybrid search
 │
 └─► PostgreSQL (Week 8+) ───── Chat history, user accounts
```

## Key Design Decisions

### Chunking strategy
- **Chosen:** Semantic chunking on section headings (Section X.X pattern)
- **Why:** Policy documents have clear hierarchical structure. Splitting on headings preserves logical units.
- **Alternative considered:** Fixed 512-token chunks — rejected because they split sections mid-sentence.

### Vector database
- **Chosen:** Qdrant
- **Why:** Best .NET SDK quality, supports hybrid search (sparse + dense), easy Docker setup.
- **Alternative considered:** pgvector — simpler if already using Postgres but less performant at scale.

### Multi-tenancy
- **Chosen:** Separate Qdrant collection per tenant (`{tenantId}-policies`)
- **Why:** Complete data isolation with no risk of cross-tenant leakage in queries.
- **Alternative considered:** Single collection with metadata filter — simpler but isolation is softer.

### Caching
Three layers (add notes as you implement each):
1. **Embedding cache** — SHA256(text) → float[] — TTL: 7 days
2. **Semantic cache** — question embedding → cached answer, cosine threshold 0.95 — TTL: 24h
3. **Response cache** — exact query string → response — TTL: 5 min

## Data Flow — Ingestion

```
PDF upload → PdfExtractor → SemanticChunker → EmbeddingClient
    → [EmbeddingCache check] → Qdrant.UpsertAsync
```

## Data Flow — Query

```
User question → [SemanticCache check]
    → EmbeddingClient → HybridSearch(Qdrant)
    → CohereReranker → top 5 chunks
    → Semantic Kernel prompt builder
    → Claude API (streaming)
    → SSE to frontend
    → [SemanticCache write]
```

## Sequence Diagram — Chat Request

```
Client          API             SK              Qdrant      Claude
  │──POST /chat──►│             │                │           │
  │               │─cache hit?─►│                │           │
  │               │◄─miss───────│                │           │
  │               │─embed query─►SK              │           │
  │               │             │─hybrid search──►Qdrant     │
  │               │             │◄─top 20 chunks─│           │
  │               │             │─rerank──────────────────── │  (Cohere)
  │               │             │─build prompt               │
  │               │             │─────────────────────────────►Claude
  │◄─SSE stream───│◄────────────────────────────────tokens───│
```
