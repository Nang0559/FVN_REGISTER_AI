# FVN_REGISTER AI Platform — Phase Status

> **Status convention:** `Implementation-complete / exit gate pending` means the code foundation for the phase is present, but the phase is **not** considered production-complete until its end-to-end authorization, integration, deployment and operational tests pass.

## Current branch

- Branch: `feature/ai-platform-phase1-6`
- PR: `#1` — draft, not merged to `master`
- `master` remains the baseline branch.

## Phase status

| Phase | Status | What is implemented | Exit-gate / production work remaining |
|---|---|---|---|
| 1 — Universal Data Ingestion | Implementation-complete / exit gate pending | Tenant-scoped ingestion, SHA-256 duplicate protection, quarantine, source metadata, folder connector, leased queue, retention metadata/purge | ZIP-bomb/file-corruption tests, multi-replica nonce/lease verification, encrypted/backup storage verification, SQL/runtime verification |
| 2 — Document Understanding | Implementation-complete / exit gate pending | Provider-neutral representation, page/paragraph/table/row/cell/region locators, confidence and OCR provenance | Provider/OCR end-to-end tests, malformed document coverage, operational verification |
| 3 — Extraction & Validation | Implementation-complete / exit gate pending | Versioned schema registry, mappings, normalization, deterministic validation, evidence/provenance, review contracts | Persist validation runs/issues with provenance, connect ingestion worker to published schema, review queue integration and tests |
| 4 — RAG & Knowledge Base | Implementation-complete / exit gate pending | Human-approved publication boundary, bounded chunks, embeddings, tenant/ACL/approved-review retrieval, hybrid ranking, citation validation | Knowledge policy lifecycle (effective/expiry/replacement), admin UI, RAG UI, retrieval-quality evaluation, prompt-injection red-team tests |
| 5 — Active Learning & Controlled Release | Implementation-complete / exit gate pending | Golden datasets, evaluation summary, human feedback gate, candidate/active/retired/rollback lifecycle, one-active SQL constraint | Field-level precision/recall/calibration, drift/cost monitoring, PII/training-data governance, real authorization adapter, production evaluation workflow |
| 6 — FVN_REGISTER AI Applications | Pilot foundation only / integration pending | HR Assistant adapter boundary, authorization-aware HR fact contract, deterministic OT/attendance/leave comparison, human-review workspace | Real FVN_REGISTER gateway/authorization adapter, production HR data integration, audit/operational verification, UI/API end-to-end pilot |

## Authorization boundary

The following operations intentionally fail closed until FVN_REGISTER supplies a trusted adapter:

- Human document review authorization.
- Schema administration authorization.
- RAG knowledge ACL authorization.
- Phase 5 AI-platform administration authorization.
- HR facts/review authorization for Phase 6.

Therefore these capabilities are **implemented as secured application boundaries**, not production-ready business functions until the real FVN_REGISTER adapters are connected and tested.

## SQL migration order

The Phase 1–6 branch does **not** start at `007` on an empty database. The dedicated AI database must already have migrations `001` through `006` applied.

Apply all migrations in this exact order:

1. `SQL/AI/001_CreateAiIngestion.sql`
2. `SQL/AI/002_ReliableIngestionQueue.sql`
3. `SQL/AI/003_Review_Knowledge_Feedback.sql`
4. `SQL/AI/004_HR_Reconciliation_Cases.sql`
5. `SQL/AI/005_Universal_AI_Core.sql`
6. `SQL/AI/006_Document_Schema_Routing.sql`
7. `SQL/AI/007_UniversalDocumentLifecycle.sql`
8. `SQL/AI/008_Phase1_3_Retention_Representation.sql`
9. `SQL/AI/009_Phase4_5_Knowledge_ActiveLearning.sql`

`007` requires the structures created by `001–006`; `008` explicitly requires `007`; `009` explicitly requires the knowledge structures created before it.

All migrations are intended to be repeatable (`IF NOT EXISTS` / `COL_LENGTH` / guarded index or procedure creation). Runtime verification against a real SQL Server is still required before treating repeatability as an operational guarantee.

## Exit-gate test coverage

Current automated coverage includes:

- Phase 1–3 representation, mapping, validation and retention idempotency tests.
- Phase 4 RAG citation/authorization behavior tests.
- Phase 5 golden dataset and release-gate tests.
- Phase 6 HR reconciliation tests and authorization fail-closed tests.

The tests currently prove important application invariants; they do **not** replace SQL Server concurrency tests, real provider/OCR tests, adapter integration tests, or production deployment verification.

## CI status

The validation workflow is configured for both:

- `push` to `master`
- `pull_request` targeting `master`

The latest PR validation for head `156e61ab4a8ce9ff540438275ddf1afc0883cac0` did run on the feature branch, but it **failed during the solution build**, so there is currently no green CI result for PR #1. The AI test step was skipped because the build failed.

Local user verification that the solution builds successfully is useful, but it is not equivalent to a green CI run.

## Verification boundary

Until the SQL migrations, real AI provider, OCR dependencies, FVN_REGISTER authorization/gateway adapters, worker processing and end-to-end flows have been exercised, the overall platform must be described as:

**Implementation-complete foundation — exit gate pending; not production-complete.**
