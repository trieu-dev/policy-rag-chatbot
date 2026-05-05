# Daily Commit Guide

## Commit Message Convention

Use this format every day — it keeps your git log readable as a learning diary.

```
<type>(<scope>): <short description>
```

### Types
| Type | When to use |
|---|---|
| `feat` | New feature or working code |
| `fix` | Bug fix |
| `test` | Adding or fixing tests |
| `docs` | README, notes, architecture |
| `refactor` | Restructuring without behaviour change |
| `chore` | Config, deps, tooling |
| `wip` | Work in progress — use when you must commit unfinished code |

### Scopes
Use the week folder name: `week01`, `week02`, ... `week10`, `infra`, `frontend`, `docs`

### Examples
```
feat(week01): add ClaudeClient with raw HttpClient
feat(week02): implement SemanticChunker splitting on section headings
fix(week03): correct Qdrant payload field name from 'section' to 'sectionId'
test(week04): add SK pipeline integration test
docs(week05): add OpenAPI annotations to /chat endpoint
refactor(week06): extract IReranker interface for testability
chore(infra): add Langfuse keys to .env.example
wip(week07): semantic cache partially working, threshold needs tuning
```

---

## When You're Stuck

It's okay to commit `wip:` commits. Something is always better than nothing. Even:

```
wip(week03): Qdrant upsert working, search returning wrong scores
```

is a valid daily commit. Write a TODO comment in the code, commit, and pick it up the next day.

---

## Sunday = Docs Day

Every Sunday commit should be documentation:
- Add learning notes to `docs/week-notes/weekXX.md`
- Update `docs/architecture.md` with any design decisions you made
- Update the progress table in the root `README.md`

---

## Week Completion Checklist

Before moving to the next week, make sure:

- [ ] Deliverable runs without errors
- [ ] At least one test covers the core behaviour
- [ ] README for the week is up to date
- [ ] Architecture doc reflects any new decisions
- [ ] Progress table in root README updated (⬜ → ✅)

---

## Git Workflow

```bash
# Start each day
git pull origin main

# During the day — commit early, commit often
git add .
git commit -m "feat(weekXX): describe what you built"

# End of day — push
git push origin main
```

You don't need branches for solo learning. Just commit to `main` daily. When you start Week 5 and the project gets API-shaped, consider a `develop` branch — but don't over-engineer the workflow.
