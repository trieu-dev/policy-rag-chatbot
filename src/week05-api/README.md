# Week 5 — ASP.NET Core API

**Goal:** Expose the RAG pipeline as a REST API with SSE streaming and background ingestion.

## Daily Commit Plan

| Day | Commit | What you build |
|---|---|---|
| Mon | `feat(week05): scaffold ASP.NET Core 10 minimal API` | Program.cs, DI setup, health endpoint |
| Tue | `feat(week05): add POST /ingest with file upload` | Accept PDF, queue background job |
| Wed | `feat(week05): add IngestionWorker background service` | Process queue: extract→chunk→embed→upsert |
| Thu | `feat(week05): add POST /chat with SSE streaming` | Stream Claude tokens via text/event-stream |
| Fri | `feat(week05): add GET /chat/history and DELETE /chat` | Persist + retrieve conversation history |
| Sat | `test(week05): add integration tests for all endpoints` | WebApplicationFactory-based tests |
| Sun | `docs(week05): add OpenAPI/Swagger annotations` | Document all endpoints with examples |

## Deliverable
Fully working API tested via Postman: upload a PDF, ask questions, get streamed answers.
