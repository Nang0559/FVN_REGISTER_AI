# FVN_REGISTER AI — Phase 1-3 completion baseline

This document defines the implementation exit gate before Phase 4 starts.

## Phase 1 — Universal Data Ingestion

- [x] Tenant-scoped ingestion and SHA-256 duplicate detection.
- [x] Quarantine + malware scan gate.
- [x] Upload/source metadata (`SourceType`, `SourceReference`).
- [x] Config-driven folder connector with stable-file detection.
- [x] SQL-backed leased queue with recovery/retry already present.
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
- [x] Declarative field mappings stored inside versioned schema definition.
- [x] Source hints and deterministic normalization (`Trim`, `Upper`, `Lower`, `DigitsOnly`, `IsoDate`).
- [x] Schema-driven extraction restricted to declared field paths.
- [x] Evidence/source locator captured from exact source text where available.
- [x] Deterministic validation before candidate is exposed as valid.
- [x] Standardized result with schema/representation/hash/model/parser/prompt provenance.
- [x] Human review is fail-closed behind trusted FVN_REGISTER authorization.
- [x] Schema draft/publish operations are fail-closed behind trusted authorization.
- [x] Exit-gate tests for mapping and validation.

## SQL migrations

Apply to the dedicated AI database in order:

1. `SQL/AI/007_UniversalDocumentLifecycle.sql`
2. `SQL/AI/008_Phase1_3_Retention_Representation.sql`

## Verification boundary

The repository is updated through Phase 3 on `feature/ai-platform-phase1-6`. Local execution was not available in this environment, and GitHub Actions has not returned a workflow/status for the current branch head. Therefore the implementation is intentionally **not labelled build-verified** until CI/build and the two SQL migrations have been executed.

Phase 4, 5 and 6 are intentionally not included in this exit gate.
