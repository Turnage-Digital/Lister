# Backlog

This file tracks near-term ideas and deferred plans that are agreed but not yet implemented.

## Migrations: Background Jobs (stashed)

Goal: run List Migrations as durable, resumable background jobs with progress over SSE and outbox durability.

- Job entity: `ListMigrationJob` (Id, ListId, PlanJson, RequestedBy, Status, Progress, ItemsProcessed, LastMessage, Attempts, NextAttempt, CreatedOn, StartedOn, FinishedOn, LastProcessedItemId, CorrelationId). Enforce one active job per list.
- Worker: `ListMigrationWorker : BackgroundService` picks queued jobs, runs in chunks (configurable batch size), commits per chunk, updates job row, emits Started/Progress/Completed/Failed integration events.
- Executor: reuse current `MigrationExecutor<TList,TItem>`; add chunking/resume (startFromItemId, maxBatchSize). Apply metadata first, then item transforms in batches.
- API:
  - `POST /api/lists/{listId}/migrations` → dryRun (validate) or execute (enqueue + 202 with `{ jobId, correlationId }`).
  - `GET /api/lists/{listId}/migrations/{jobId}` → status snapshot.
  - `POST /api/lists/{listId}/migrations/{jobId}:cancel` → cooperative cancellation.
- Concurrency: one job per list; global parallelism cap. Optionally 409 Busy for list updates while migration running.
- Reliability: retry with backoff; stale lease detection; idempotent chunk processing.
- Observability: SSE events include `correlationId`; client shows badges/progress.
- Config: batch size, max parallel jobs, retry/backoff limits, busy policy.

