# Policy RAG Chatbot

A production-ready Retrieval-Augmented Generation (RAG) chatbot for company and insurance policy Q&A, built with **C# / ASP.NET Core 10**, **Semantic Kernel**, **Qdrant**, and **Claude API**.

Built week-by-week over 10 weeks. Each `src/weekXX-*` folder is a self-contained milestone you can run independently.

---

## Tech Stack

| Layer | Technology |
|---|---|
| LLM | Claude API (Anthropic) |
| Orchestration | Semantic Kernel + Kernel Memory |
| Vector DB | Qdrant |
| Search | Hybrid (BM25 + dense vectors) + Reranking |
| Backend | ASP.NET Core 10 Minimal API |
| Cache | Redis (semantic + embedding + response) |
| Auth | JWT Bearer + ASP.NET Core Identity |
| Observability | OpenTelemetry + Langfuse |
| Frontend | Next.js 14 + SSE streaming |
| Infra | Docker Compose |

---

## Project Structure

```
policy-rag-chatbot/
├── src/
│   ├── week01-fundamentals/       # Embeddings, cosine similarity, first Claude call
│   ├── week02-ingestion/          # PDF parsing, chunking, metadata
│   ├── week03-vectordb/           # Qdrant setup, upsert, search
│   ├── week04-semantic-kernel/    # SK orchestration, streaming
│   ├── week05-api/                # ASP.NET Core API, SSE, Worker Service
│   ├── week06-hybrid-search/      # BM25 + dense hybrid, reranking
│   ├── week07-caching/            # Redis semantic + embedding cache
│   ├── week08-auth/               # JWT, RBAC, multi-tenancy
│   ├── week09-evaluation/         # Eval metrics, Langfuse, OpenTelemetry
│   └── week10-production/         # Full stack, Docker Compose, frontend
├── frontend/                      # Next.js chat UI
├── infra/                         # Docker Compose, env templates
├── docs/                          # Architecture notes, decisions
└── scripts/                       # Utility scripts (seed data, eval runner)
```

---

## Quick Start (Week 10 — Full Stack)

```bash
# 1. Clone
git clone https://github.com/YOUR_USERNAME/policy-rag-chatbot.git
cd policy-rag-chatbot

# 2. Copy env file and fill in your keys
cp infra/.env.example infra/.env

# 3. Run everything
docker compose -f infra/docker-compose.yml up

# API:      http://localhost:5000
# Frontend: http://localhost:3000
# Qdrant:   http://localhost:6333
```

---

## Free & Local Development (Ollama)

You can run the entire pipeline for **free** using local models with [Ollama](https://ollama.com).

1. **Start Ollama**: Use the dev infrastructure:
   ```bash
   docker compose -f infra/docker-compose.dev.yml up -d
   ```

2. **Pull Models**:
   ```bash
   docker exec -it policy-rag-ollama ollama pull llama3
   docker exec -it policy-rag-ollama ollama pull nomic-embed-text
   ```

3. **Configure .env**:
   Set the following in `infra/.env`:
   ```env
   ANTHROPIC_ENDPOINT=http://localhost:11434/v1/chat/completions
   ANTHROPIC_MODEL=llama3
   OPENAI_ENDPOINT=http://localhost:11434/v1/embeddings
   OPENAI_MODEL=nomic-embed-text
   ```

---

---

## Week-by-Week Progress

| Week | Focus | Status |
|---|---|---|
| Week 1 | Fundamentals — embeddings, Claude API | 🔄 In progress |
| Week 2 | Document ingestion — PDF, chunking | ⬜ Not started |
| Week 3 | Vector DB — Qdrant setup | ⬜ Not started |
| Week 4 | Semantic Kernel orchestration | ⬜ Not started |
| Week 5 | ASP.NET Core API + SSE streaming | ⬜ Not started |
| Week 6 | Hybrid search + reranking | ⬜ Not started |
| Week 7 | Redis caching (semantic + embedding) | ⬜ Not started |
| Week 8 | Auth, RBAC, multi-tenancy | ⬜ Not started |
| Week 9 | Evaluation + observability | ⬜ Not started |
| Week 10 | Frontend + Docker + production | ⬜ Not started |

Update each row as you progress: ⬜ → 🔄 → ✅

---

## Daily Commit Convention

```
feat(week01): add cosine similarity helper
feat(week02): implement semantic chunker
fix(week03): correct Qdrant collection payload schema
docs(week04): add SK orchestration notes
test(week05): add integration test for /chat endpoint
refactor(week06): extract reranker into its own service
chore: update .env.example with Langfuse keys
```

---

## Environment Variables

See `infra/.env.example` for all required keys.

| Variable | Description |
|---|---|
| `ANTHROPIC_API_KEY` | Claude API key |
| `EMBEDDING_MODEL` | e.g. `text-embedding-3-small` |
| `QDRANT_URL` | e.g. `http://localhost:6333` |
| `QDRANT_API_KEY` | Qdrant cloud key (empty for local) |
| `REDIS_CONNECTION` | e.g. `localhost:6379` |
| `JWT_SECRET` | Random 256-bit secret |
| `LANGFUSE_SECRET_KEY` | Langfuse observability key |
| `LANGFUSE_PUBLIC_KEY` | Langfuse public key |

---

## License

MIT
