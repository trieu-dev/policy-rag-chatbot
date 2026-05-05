# Week 10 — Frontend and Production Hardening

**Goal:** Full chat UI, guardrails, and everything running in Docker Compose.

## Daily Commit Plan

| Day | Commit | What you build |
|---|---|---|
| Mon | `feat(week10): scaffold Next.js 14 chat UI` | App Router, basic chat page |
| Tue | `feat(week10): implement SSE streaming in React` | EventSource hook, typing animation |
| Wed | `feat(week10): add source citation panel` | Show which chunk answered the question |
| Thu | `feat(week10): add guardrail middleware` | Reject non-policy queries before LLM call |
| Fri | `feat(week10): add full Docker Compose` | API + Qdrant + Redis + Next.js frontend |
| Sat | `feat(week10): add document upload UI` | Drag-drop PDF upload with progress bar |
| Sun | `docs(week10): final architecture diagram` | README update + architecture.md |

## Deliverable
`docker compose up` → http://localhost:3000 → fully working policy chatbot.
