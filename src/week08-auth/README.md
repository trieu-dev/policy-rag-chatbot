# Week 8 — Auth, RBAC and Multi-Tenancy

**Goal:** Secure the API with JWT, RBAC, and per-company data isolation.

## Daily Commit Plan

| Day | Commit | What you build |
|---|---|---|
| Mon | `feat(week08): add JWT bearer auth middleware` | Validate token on every request |
| Tue | `feat(week08): add POST /auth/login and /auth/register` | Issue JWT with tenantId + role claims |
| Wed | `feat(week08): add tenant-scoped Qdrant namespacing` | Collections named {tenantId}-policies |
| Thu | `feat(week08): restrict /ingest to Admin role` | Policy-based authorization on upload |
| Fri | `feat(week08): add tenant isolation middleware` | Extract tenantId from JWT, scope all queries |
| Sat | `test(week08): add cross-tenant isolation test` | Tenant A cannot read Tenant B documents |
| Sun | `docs(week08): notes on multi-tenancy design` | Namespace vs separate collection tradeoffs |

## Deliverable
Two-tenant demo: Company A and B each upload different policies, cannot see each other's data.
