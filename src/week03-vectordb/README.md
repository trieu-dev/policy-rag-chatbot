# Week 3 — Vector Database (Qdrant)

**Goal:** Store embedded chunks in Qdrant, query by semantic similarity.

## Daily Commit Plan

| Day | Commit | What you build |
|---|---|---|
| Mon | `feat(week03): add docker-compose for local Qdrant` | Qdrant on :6333 via Docker |
| Tue | `feat(week03): add QdrantRepository with collection setup` | Create collection, configure vector size |
| Wed | `feat(week03): implement UpsertChunksAsync` | Batch insert embedded chunks as Qdrant points |
| Thu | `feat(week03): implement SearchAsync with metadata filter` | Query by vector + filter by policy name |
| Fri | `feat(week03): wire week02 pipeline into Qdrant` | PDF → chunks → embeddings → Qdrant |
| Sat | `test(week03): add integration test hitting local Qdrant` | Upsert 5 chunks, search, assert top result |
| Sun | `docs(week03): notes on Qdrant payload schema design` | Why you chose this metadata structure |

## Deliverable
Run ingestion → open Qdrant dashboard at http://localhost:6333/dashboard → see your chunks.
