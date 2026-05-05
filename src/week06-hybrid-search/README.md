# Week 6 — Hybrid Search and Reranking

**Goal:** Improve retrieval with BM25 + dense hybrid search and cross-encoder reranking.

## Daily Commit Plan

| Day | Commit | What you build |
|---|---|---|
| Mon | `feat(week06): enable sparse vectors in Qdrant` | BM25 sparse vector alongside dense |
| Tue | `feat(week06): add BM25Tokenizer for sparse vectors` | Tokenize + score terms |
| Wed | `feat(week06): implement hybrid search with RRF` | Combine dense + sparse via Reciprocal Rank Fusion |
| Thu | `feat(week06): add CohereReranker` | Rerank top-20 results to top-5 |
| Fri | `feat(week06): add query rewriting with HyDE` | Generate hypothetical answer → embed → search |
| Sat | `test(week06): compare retrieval quality before vs after` | Run 10 questions, log top-3 chunks each way |
| Sun | `docs(week06): notes on hybrid search tradeoffs` | When BM25 helps vs pure vector search |

## Deliverable
Side-by-side comparison script: hybrid+reranking vs pure vector search on 10 test queries.
