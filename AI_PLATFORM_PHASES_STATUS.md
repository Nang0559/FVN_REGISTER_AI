# FVN_REGISTER AI Platform — Phase Status

> **Status convention:** `Foundation / partial` means some contracts and infrastructure exist, but core end-to-end behavior is not yet implemented or verified. No Phase 1–5 phase is production-complete.

## Current branch

- Branch: `feature/ai-platform-phase1-6`
- PR: `#1` — draft, not merged to `master`
- `master` remains the baseline branch.

## Phase status

| Phase | Status | What is implemented | Exit-gate / production work remaining |
|---|---|---|---|
| 1 — Universal Data Ingestion | Foundation / partial — exit gate pending | Tenant-scoped ingestion, SHA-256 duplicate protection scoped by (Tenant, SHA-256, Module, Schema), quarantine, source metadata, folder connector, leased queue, retention metadata/purge | ZIP-bomb/file-corruption tests, verify SQL-backed nonce replay protection (migration 011), multi-replica lease behavior, encrypted/backup storage verification, SQL/runtime verification |
| 2 — Document Understanding | Foundation / partial — exit gate pending | Provider-neutral representation, page/paragraph/table/row/cell/region locators, confidence and OCR provenance | Provider/OCR end-to-end tests, malformed document coverage, operational verification |
| 3 — Extraction & Validation | Foundation / partial — exit gate pending | Versioned schema registry, mappings, normalization, deterministic validation, evidence/provenance, review contracts | Persist validation runs/issues with schema/model/prompt and evidence provenance; bind worker to the published schema; complete review-queue integration and end-to-end tests |
| 4 — RAG & Knowledge Base | Foundation / partial — exit gate pending | Human-approved publication boundary, bounded chunks, embeddings, tenant/ACL/approved-review retrieval, cosine ranking, citation validation | Knowledge policy lifecycle (effective/expiry/replacement), real authorization adapter, admin UI, RAG UI, retrieval-quality evaluation, prompt-injection red-team tests |
| 5 — Active Learning & Controlled Release | Foundation / partial — exit gate pending | Golden dataset/evaluation contracts and summary, human feedback gate, candidate/active/retired/rollback lifecycle, one-active SQL constraint | Real Phase 5 authorization adapter, field-level precision/recall/calibration, drift/cost monitoring, PII/training-data governance, production evaluation workflow |
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

For an empty dedicated `FVN_AI` database, run migrations `001` through `013` in this one sequence:

1. `SQL/AI/001_CreateAiIngestion.sql`
2. `SQL/AI/002_ReliableIngestionQueue.sql`
3. `SQL/AI/003_Review_Knowledge_Feedback.sql`
4. `SQL/AI/004_HR_Reconciliation_Cases.sql`
5. `SQL/AI/005_Universal_AI_Core.sql`
6. `SQL/AI/006_Document_Schema_Routing.sql`
7. `SQL/AI/007_UniversalDocumentLifecycle.sql`
8. `SQL/AI/008_Phase1_3_Retention_Representation.sql`
9. `SQL/AI/009_Phase4_5_Knowledge_ActiveLearning.sql`
10. `SQL/AI/010_Extraction_Deduplication_And_MigrationLedger.sql`
11. `SQL/AI/011_Persistent_Request_Replay_Protection.sql`
12. `SQL/AI/012_KnowledgeChunkConstraint_And_ChecksumLedger.sql`
13. `SQL/AI/013_HRCaseTextColumnSizeRepair.sql`

`007` requires the structures created by `001–006`; `008` explicitly requires `007`; `009` explicitly requires the knowledge structures created before it. `010` requires `001–009`, adds a unique filtered index for versioned extractions, and initializes a migration ledger for subsequent tracking.

All migrations are intended to be repeatable (`IF NOT EXISTS` / `COL_LENGTH` / guarded index or procedure creation). The knowledge-chunk ordinal unique index is introduced by migration `012`, not retrofitted into historical migrations. Existing duplicate ordinals cause `012` to stop with a clear error; reconcile them before retrying. Migration `012` extends the ledger with filename and SHA-256 checksum fields. A NULL checksum means the older migration is an unverified legacy baseline. Runtime verification against a real SQL Server is still required before treating repeatability as an operational guarantee.

## Exit-gate test coverage

Automated coverage is incomplete until the SQL Server workflow exercises fresh-database migrations, repeat application, duplicate-ordinal failure, concurrent lease claims and tenant-scoped smoke tests. Unit tests alone do not prove SQL locking semantics.

- Phase 1–3 representation, mapping, validation and retention idempotency tests.
- Phase 4 RAG citation/authorization behavior tests.
- Phase 5 golden dataset and release-gate tests.
- Phase 6 HR reconciliation tests and authorization fail-closed tests.

The tests currently prove important application invariants; they do **not** replace SQL Server concurrency tests, real provider/OCR tests, adapter integration tests, or production deployment verification.

## CI status

The validation workflow is configured for both:

- `push` to `master`
- `pull_request` targeting `master`

Earlier PR runs were cancelled while Restore/Build was in progress, so those runs do **not** establish a code failure or a green CI result. The workflow has now been changed to `cancel-in-progress: false` so an in-flight PR validation is allowed to finish and produce a meaningful Restore → Build → AI Tests result.

Local user verification that the solution builds successfully is useful, but it is not equivalent to a green CI run.

## Verification boundary

Until the SQL migrations, real AI provider, OCR dependencies, FVN_REGISTER authorization/gateway adapters, worker processing and end-to-end flows have been exercised, the overall platform must be described as:

**Implementation-complete foundation — exit gate pending; not production-complete.**
