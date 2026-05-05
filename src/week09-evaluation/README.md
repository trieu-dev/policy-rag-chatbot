# Week 9 — Evaluation and Observability

**Goal:** Measure RAG answer quality. Add distributed tracing.

## Daily Commit Plan

| Day | Commit | What you build |
|---|---|---|
| Mon | `feat(week09): add golden eval dataset (20 Q&A pairs)` | Hand-crafted questions with known answers |
| Tue | `feat(week09): add FaithfulnessScorer using Claude as judge` | Claude scores 1-5: grounded in context? |
| Wed | `feat(week09): add ContextRecallScorer` | Were the right chunks retrieved? |
| Thu | `feat(week09): add OpenTelemetry to ASP.NET Core` | Traces, spans, token counts |
| Fri | `feat(week09): add Langfuse trace exporter` | Send RAG traces to Langfuse dashboard |
| Sat | `feat(week09): add EvalRunner CLI tool` | Run full eval suite, output markdown report |
| Sun | `docs(week09): analysis of eval results` | Honest notes on where RAG fails |

## Deliverable
`dotnet run --project EvalRunner` → faithfulness + recall report across 20 test questions.
