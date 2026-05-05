# Week 1 — Fundamentals

**Goal:** Understand embeddings, cosine similarity, and make your first Claude API call from C#.

## Daily Commit Plan

| Day | Commit | What you build |
|---|---|---|
| Mon | `feat(week01): project scaffold and week1 readme` | Folder structure, this file |
| Tue | `feat(week01): add ClaudeClient with raw HttpClient` | POST to /v1/messages, parse response |
| Wed | `feat(week01): add EmbeddingClient returning float[]` | Call embedding API, deserialize vector |
| Thu | `feat(week01): implement cosine similarity helper` | `VectorMath.CosineSimilarity(float[], float[])` |
| Fri | `feat(week01): wire first RAG demo — question answered from hardcoded context` | Full W1 deliverable |
| Sat | `test(week01): add unit tests for CosineSimilarity` | Edge cases: zero vector, identical vectors |
| Sun | `docs(week01): add notes on what embeddings represent` | Your own learning notes in /docs |

## Deliverable

Console app that:
1. Takes a hardcoded policy text
2. Embeds it
3. Takes a hardcoded question
4. Embeds the question
5. Computes cosine similarity
6. Sends both to Claude with a grounding prompt
7. Prints the grounded answer

## Key Concepts to Learn This Week

- What a float[] embedding vector represents geometrically
- Why cosine similarity (not Euclidean distance) is used for text
- How Claude's /v1/messages API works — system prompt, user turn, assistant turn
- What a context window limit means in practice

## Running

```bash
cd src/week01-fundamentals
dotnet run
```

## Files

```
week01-fundamentals/
├── Week01.csproj
├── Program.cs              # Entry point — runs the demo
├── ClaudeClient.cs         # Raw HttpClient wrapper for Claude API
├── EmbeddingClient.cs      # Calls embedding API, returns float[]
└── VectorMath.cs           # CosineSimilarity() and helpers
```
