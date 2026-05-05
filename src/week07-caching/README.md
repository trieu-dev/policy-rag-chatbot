# Week 7 — Redis Caching (Three Layers)

**Goal:** Add semantic, embedding, and response caching to cut LLM costs.

## Daily Commit Plan

| Day | Commit | What you build |
|---|---|---|
| Mon | `feat(week07): add Redis to docker-compose` | Redis on :6379, StackExchange.Redis |
| Tue | `feat(week07): add EmbeddingCache (SHA256 key → vector)` | Cache embedding results |
| Wed | `feat(week07): add SemanticCache with cosine threshold` | Cache Q+A pairs, match by similarity |
| Thu | `feat(week07): add ResponseCache for identical queries` | IDistributedCache exact-match cache |
| Fri | `feat(week07): add cache metrics logging` | Log hit/miss ratio per layer |
| Sat | `test(week07): add cache integration tests` | Assert cache hit on second identical query |
| Sun | `docs(week07): notes on TTL strategy` | Why embedding TTL > semantic TTL |

## Deliverable
Benchmarks: 0ms LLM latency on repeated semantically similar questions (cache hit).
