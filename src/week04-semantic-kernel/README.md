# Week 4 — Semantic Kernel Orchestration

**Goal:** Replace manual RAG wiring with Semantic Kernel. Add streaming.

## Daily Commit Plan

| Day | Commit | What you build |
|---|---|---|
| Mon | `feat(week04): add SK kernel builder with Claude connector` | Wire SK to Claude via HttpClient connector |
| Tue | `feat(week04): add KernelMemory with Qdrant store` | Replace manual Qdrant with SK memory |
| Wed | `feat(week04): implement RAG plugin using SK functions` | SK Plugin: SearchMemory → BuildPrompt → Ask |
| Thu | `feat(week04): add streaming via IAsyncEnumerable` | Stream tokens to console as they arrive |
| Fri | `feat(week04): add conversation history context` | Pass last N turns into the prompt |
| Sat | `test(week04): add SK pipeline integration test` | Ask a question, assert answer contains citation |
| Sun | `docs(week04): notes on SK plugin architecture` | Your understanding of SK plugins |

## Deliverable
Interactive terminal chat: type questions, get streamed grounded answers with source citations.
