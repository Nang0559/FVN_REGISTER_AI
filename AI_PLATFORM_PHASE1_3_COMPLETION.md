# FVN_REGISTER AI — Phase 1-6 completion baseline

This document is the implementation exit-gate for the universal AI platform branch.

## Phase 1 — Universal Data Ingestion

- [x] Tenant-scoped ingestion and SHA-256 duplicate detection.
- [x] Quarantine + malware scan gate.
- [x] Upload/source metadata (`SourceType`, `SourceReference`).
- [x] Config-driven folder connector with stable-file detection.
- [x] SQL-backed leased queue with recovery/retry.
- [x] Retention deadline and scheduled binary purge.
- [x] Archive/tombstone state is idempotent and preserves audit metadata.
- [x] Exit-gate tests for retention idempotency.

## Phase 2 — Document Understanding

- [x] Canonical `DocumentRepresentation` v1.0.
- [x] Page/paragraph/table/row/cell/region objects.
- [x] Stable provider-neutral locators.
- [x] Confidence attached to normalized units.
- [x] OCR/scanned-PDF marker and lower default OCR confidence.
- [x] PDF page markers, DOCX paragraphs, XLSX/CSV rows/cells normalized.
- [x] Representation persisted with extraction and returned by extraction query.
- [x] Exit-gate test for page/table/cell/region normalization.

## Phase 3 — Extraction & Validation

- [x] Tenant/module/schema registry with immutable version records.
- [x] Draft → Published → Retired lifecycle.
- [x] Declarative field mappings, source hints and deterministic normalization.
- [x] Schema-driven extraction restricted to declared field paths.
- [x] Evidence/source locator captured from exact source text where available.
- [x] Deterministic validation before candidate is exposed as valid.
- [x] Standardized result with schema/representation/hash/model/parser/prompt provenance.
- [x] Human review and schema administration are fail-closed behind trusted FVN_REGISTER authorization.
- [x] Exit-gate tests for mapping and validation.

## Phase 4 — RAG & Knowledge Base

- [x] Human approval is the publication boundary; ingestion/extraction alone cannot enter the knowledge base.
- [x] Chunking preserves page and section metadata and is bounded to protect SQL/embedding workloads.
- [x] Embeddings are persisted with model provenance and tenant isolation.
- [x] SQL vector retrieval applies tenant, active-review and ACL intersection before returning text.
- [x] Hybrid reranking combines semantic similarity with lexical token overlap using configurable weights.
- [x] Knowledge publication is idempotent per document/chunk ordinal; repeated publish no longer violates the unique ordinal constraint.
- [x] Unpublish disables active chunks without deleting audit metadata.
- [x] RAG answers are grounded only in retrieved evidence and citations are validated against returned chunk IDs.
- [x] Knowledge lifecycle query enforces the caller's allowed scopes.

## Phase 5 — Active Learning & Controlled Release

- [x] Tenant-scoped golden dataset cases with JSON input/expected/rubric validation.
- [x] Latest-evaluation-per-case summary and pass-rate metric.
- [x] Human-feedback acceptance/rejection/correction metrics remain the release signal.
- [x] Candidate model/prompt/schema releases are persisted with explicit status.
- [x] Promotion is fail-closed behind a metric gate: minimum human decisions, acceptance rate and complete passing golden set are required.
- [x] Only one release is active per tenant; promotion retires the previous active release.
- [x] Rollback marks the active release as rolled back and restores the most recently promoted retired release.
- [x] Phase 5 administration has a dedicated trusted authorization port and is fail-closed until FVN_REGISTER supplies the adapter.

## Phase 6 — FVN_REGISTER AI Applications

- [x] HR Assistant remains a business adapter over the AI core rather than embedding FVN_REGISTER business rules in infrastructure.
- [x] HR facts are supplied through the trusted authorization-aware gateway contract.
- [x] HR case types are schema-like typed contracts (OT, attendance, leave) with deterministic comparison rules and explicit human-review outcome.
- [x] HR case review/query/audit persistence remains in the AI application boundary; no AI operation writes payroll/attendance master data directly.
- [x] `IHRCaseAssistantAdapter` provides the explicit application adapter boundary for future OT/Leave/Trip/Equipment/Excel adapters.
- [x] Existing `AIHRCaseWorkspace` is the HR-first UI entry point and remains human-in-the-loop.

## SQL migrations

Apply to the dedicated AI database in order:

1. `SQL/AI/007_UniversalDocumentLifecycle.sql`
2. `SQL/AI/008_Phase1_3_Retention_Representation.sql`
3. `SQL/AI/009_Phase4_5_Knowledge_ActiveLearning.sql`

## Verification boundary

The user has verified the current branch with a local build after the Phase 1-3 compile fix. This environment can inspect and commit repository changes but cannot execute the user's Windows/.NET build or SQL Server runtime. The branch therefore should still be treated as **implementation-complete / locally build-verified by the user**, while SQL migration execution and end-to-end provider/gateway tests remain deployment verification steps.

Branch: `feature/ai-platform-phase1-6`
PR: `#1` (draft, not merged to `master`).
