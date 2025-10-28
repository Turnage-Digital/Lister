# Lister Architecture

This document captures the intentional architecture boundaries and composition patterns for the Lister monorepo.

## Table of Contents

1. Solution Structure
2. Layers & Dependencies
3. Host Composition & Module Registration
4. Conventions
5. Persistence & Mutation Flow
6. MediatR
7. Behaviors & Controllers
8. Integration Events
9. Event Phases & Dispatch
10. Change Feed (SSE)
11. Entity Design
12. Value Objects & Serialization
13. Read Path & Views
14. List Schema Validation & Indexing (Bag)
15. Endpoints Overview
16. List Schema Migrations (Outbox + SSE)
17. Testing
18. Config
19. Performance considerations
20. Routing (React Router v6)

## Solution Structure

```
/src
  /Lister.App.Server           # Host / Composition Root (DI, HTTP, SSE, Outbox dispatcher)
  /Lister.App.Server.Tests     # Host tests
  /Lister.Client               # React SPA
  /Lister.Mcp.Server           # Dev-only MCP sidecar (tools over HTTP)
  /Modules
    /Core                      # Shared kernel
      /Lister.Core.Domain
      /Lister.Core.Application
      /Lister.Core.Infrastructure.Sql
      /Lister.Core.Infrastructure.OpenAi
      /Lister.Core.Tests
    /Lists                     # Lists module
      /Lister.Lists.Domain
      /Lister.Lists.Application      # Endpoints (incl. UpdateList, Migrations), handlers
      /Lister.Lists.Infrastructure.Sql
      /Lister.Lists.Tests
    /Notifications             # Notifications module
      /Lister.Notifications.Domain
      /Lister.Notifications.Application
      /Lister.Notifications.Infrastructure.Sql
      /Lister.Notifications.Tests
    /Users                     # Users / Identity module
      /Lister.Users.Domain
      /Lister.Users.Application
      /Lister.Users.Infrastructure.Sql
```

## Layers & Dependencies

- Domain: Pure business logic and models. May raise domain/integration events. No framework/runtime dependencies beyond
  core BCL and MediatR abstractions if desired.
- Application: Use-cases (MediatR handlers), controllers, pipeline behaviors. Depends on Domain. Does not reference
  Infrastructure projects.
- Infrastructure: Technology-specific implementations. Depends on Domain. Not referenced by Application.
    - **Infrastructure.Sql**: EF Core DbContexts, stores, query services, migrations
    - **Infrastructure.OpenAi**: External service integrations (e.g., AI text processing)
    - Pattern: `Infrastructure.{Technology}` for technology-specific implementations
- Host (Lister.App.Server): Composition root. References Infrastructure.* and Application. Wires DbContexts, stores,
  aggregates, query services, closes generic handlers, configures middleware and hosted services.
- MCP Server: Dev-only sidecar. Talks to Host over HTTP.
- Client: React SPA, served by Host.

## Host Composition & Module Registration

- Program.cs remains the composition root: it closes open generics, wires aggregates to concrete EF types, and keeps
  Application projects free of DI adapters.
- Modules expose `AddInfrastructure()`, `AddDomain()`, and `AddApplication()` extension methods; call them in that order
  so stores and aggregates are available before handlers.
- Set the EF migration assembly for each DbContext when registering infrastructure services.

Example registrations:

```csharp
// Lists
services.AddScoped(typeof(IRequestHandler<CreateListItemCommand, ListItem>),
    typeof(CreateListItemCommandHandler<ListDb, ItemDb>));

// Notifications
services.AddScoped(typeof(IRequestHandler<CreateNotificationRuleCommand, CreateNotificationRuleResponse>),
    typeof(CreateNotificationRuleCommandHandler<NotificationRuleDb, NotificationDb>));
```

## Conventions

- Application must not reference Infrastructure projects.
- Host is the only place that closes generics or binds module abstractions to concrete infrastructure (see Host
  Composition & Module Registration).
- New modules follow: Domain + Application + Infrastructure.Sql; wire in Host.
- Access stores exclusively via their unit of work abstractions; do not inject `IListsStore`, `IItemsStore`, etc.
  directly into handlers or services.

## Persistence & Mutation Flow

### DbContexts

- One DbContext per module; migrations live in the module's Infrastructure.Sql project.
- Host registers DbContexts via `AddInfrastructure(connectionString)` which auto-detects each context's migration
  assembly.

### Stores

- Stores (`INotificationsStore<T>`, `INotificationRulesStore<T>`, etc.) handle low-level persistence operations and
  remain thin on business logic.
- Stores are registered in the Host and injected into aggregates via their module unit of work abstractions.
- Store `Get*` methods eagerly hydrate the full aggregate root (navigation properties, history, metadata); mutation
  flows assume they will not issue additional queries. When you add new invariant-sensitive data, update the store
  hydration accordingly.
- Store `Get*` methods exist strictly for mutation paths—use read services for projections or listings.
- Stores append history entries for their entities (create/update/delete/etc.) so aggregates stay focused on invariants
  while the persistence layer maintains audit trails consistently.

### Aggregates

- Aggregates compose stores to add business logic, domain events, and invariant enforcement; they do not access
  EF/DbContexts directly.
- All persistence flows through the module unit of work and its stores; aggregates call store methods such as
  `GetListByIdAsync`, `GetItemByIdAsync`, `SetBagAsync`, `SetStatusesAsync`, etc.
- Aggregates never partially load state—hydrate once via store APIs and pass the result through the workflow rather
  than building ad hoc projections.
- Aggregates orchestrate multiple stores via their module unit of work (e.g., `IListsUnitOfWork`), keeping work inside
  a single transaction and enabling `UnitOfWork.SaveChangesAsync` to dispatch queued domain events in the correct
  phase.
- When aggregates need supporting data (status transitions, notification rules, etc.) they request it from store APIs
  so hydration responsibilities stay centralized.

## MediatR

- Use assembly scanning for non-generic handlers.
- Use closed DI bindings (above) for generic handlers and integration event handlers.

## Behaviors & Controllers

- `AssignUserBehavior` populates `UserId` on `RequestBase{,T}`.
- `LoggingBehavior` logs request/response boundaries for RequestBase types.
- Controllers live in Application (as done here); Host maps controllers.

## Integration Events

- Domain events that cross module boundaries (e.g., `ListItemCreatedIntegrationEvent`)
- Handlers registered in Host using MediatR's `INotificationHandler<T>`
- Enable loose coupling between modules (Lists → Notifications)
- Published via MediatR after successful aggregate operations

## Event Phases & Dispatch

- Purpose: Make the timing of domain/integration events explicit while preserving transactional guarantees.
- Phases:
    - `BeforeSave`: raised inside the transaction prior to persistence; use sparingly (e.g., validation that needs
      observers).
    - `AfterSave`: raised after `SaveChanges` commits; IDs exist; side-effects safe.
- Mechanism:
    - Domain enqueues events into `IDomainEventQueue` with a phase.
    - `UnitOfWork.SaveChangesAsync` publishes `BeforeSave` events, saves/commits, then publishes `AfterSave` events.
    - MediatR is used for dispatch; domain retains explicit control over when events fire (by phase and enqueue
      location).

## Change Feed (SSE)

- Route: `GET /api/changes/stream`
- Envelope: `{ type, data, occurredOn }`
- Producers: Integration event handlers publish to a ChangeFeed service; Outbox dispatcher replays persisted messages.
- Consumers: The client registers handlers by `type` and invalidates relevant caches (e.g., list-items,
  list-definition).

## Entity Design

- Interfaces define identifiers used for selection and hydration.
    - `IItem`: `int? Id`, `Guid? ListId` — the composite identifiers used by stores and aggregates to load a specific
      item within a list.
    - `IList`: `Guid? Id`, `string Name` — lists are addressed by Id; Name is also treated as an identifier for
      convenience queries (e.g., `GetByNameAsync`).
- Writable vs read-only contracts separate mutation from projection.
    - `IWritableItem : IItem`, `IWritableList : IList` — implemented by persistence entities for write paths.
    - Read models (e.g., `IReadOnlyList`, DTOs) are returned by query services without mutation concerns.
- Persistence entities implement writable interfaces and add storage-specific shape.
    - Lists: `ListDb : IWritableList` with navigation collections and flags (e.g., `IsDeleted`).
    - Items: `ItemDb : IWritableItem` with `Bag` object stored as a MySQL `JSON` column; configured with a camelCase
      serializer.
- Identifier usage rules in repositories/aggregates.
    - Lists: selection by `Id` or by `Name` (`GetListByIdAsync`, `GetListByNameAsync`).
    - Items: selection by `(itemId, listId)` pair to ensure list scoping.
    - Aggregates and stores should use only identifier properties in `Where` expressions to hydrate entities from the
      database.
- History/audit pattern for state transitions.
    - Each aggregate maintains a history collection (`ListHistoryEntryDb`, `ItemHistoryEntryDb`,
      `NotificationHistoryEntryDb`, `NotificationRuleHistoryEntryDb`).
    - Write operations append history entries (Created, Updated, Deleted/Cancelled, Delivered, Read, etc.).
- Soft-delete and invariants.
    - Entities include flags like `IsDeleted` (e.g., `ListDb`, `NotificationRuleDb`); query paths filter them out by
      default.
    - Required fields are enforced in EF configuration (e.g., `Name`, foreign keys), with sensible max lengths.
- Value objects capture domain-specific fields separate from identifiers.
    - Lists: `Status`, `Column` define schema; mapped via separate tables and relationships.
    - Notifications: `NotificationTrigger`, `NotificationChannel`, `NotificationSchedule`, `NotificationContent`;
      persisted as JSON blobs for flexibility, with denormalized `TriggerType` for efficient filtering.
- Mapping between write models and views.
    - Application-layer context maps project aggregates and value objects into DTO records; infrastructure query
      services return tailored projections for reads.
    - Commands continue to operate on `IWritable*` abstractions via aggregates to keep write logic inside the domain.

## Value Objects & Serialization

- Value objects are record types intended to cross service/app boundaries.
    - Author all ValueObjects as C# `record` types in Domain.
    - Annotate every serializable property with `JsonPropertyName("{propName}")` to lock the wire contract explicitly.
    - Keep them small and composable; use them inside commands, events, and views.

- Views (read models) follow the same rule.
    - Views are records and should annotate all properties with `JsonPropertyName`.
    - Treat view types as API contracts; avoid leaking persistence-only fields.

- What NOT to annotate
    - Storage-only types (e.g., EF Core DB records like `ColumnDb`, `StatusDb`, history entry DB records) are not
      serialized over the wire; do not add JSON attributes.
    - Persistence entities exist for storage and mapping, not as API shapes.

- Naming and JSON policy
    - The app uses a camelCase JSON policy in controllers; `JsonPropertyName` ensures stability even if naming policies
      change.
    - Prefer explicit property names to prevent accidental wire breaks during refactors.

- Example pattern
    - ValueObject: `NotificationChannel`, `NotificationTrigger`, `Status`, `Column` — records with `JsonPropertyName` on
      each field.
    - View: `ListItem`, `ItemDetails`, `ListItemDefinition` — records with `JsonPropertyName` on each field.

## Read Path & Views

- Views are read-only projections tailored to specific API needs.
    - Multiple views can represent the same entity for different use-cases.
        - Example (Lists): `ListItem` (lightweight for listings) and `ItemDetails` (includes `History`) both represent
          an
          item.
            - Views live under `*.Domain.Views` and typically implement an `IReadOnly*` interface that extends the base
              identifier interface (`IReadOnlyItem : IItem`).

- Query services compose views efficiently without loading aggregates.
    - Application handlers call small interfaces (e.g., `IGetItemDetails`, `IGetListItemDefinition`, `IGetPagedList`,
      `IGetListNames`).
    - For Notifications, equivalent domain services exist (e.g., `IGetUserNotifications`, `IGetNotificationDetails`,
      `IGetUserNotificationRules`, `IGetUnreadNotificationCount`).
    - Infrastructure.Sql implements these with EF Core projections that select only required columns and navigations.
        - Avoid N+1 by selecting from the correct root with explicit `Select(...)` into the view shape and scoped
          `Include(...)` only when needed.

- Shape examples (Lists)
    - `ListItem`: `{ id, listId, bag }` — used for list pages and paginated results.
    - `ItemDetails`: `{ id, listId, bag, history[] }` — used for item detail page; adds typed history entries.
    - `ListItemDefinition`: list schema `{ columns[], statuses[] }` returned when building editors/validators.

- Paging, sorting, searching
    - Paged reads use a `PagedList` view: `{ items[], count }` with server-side `Skip/Take`.
    - Sorting and filtering are applied at the DB via query parameters (e.g., `sortBy`, `ascending`, `searchTerm`).
    - JSON `bag` remains opaque in DB; index on frequently filtered scalar columns (e.g., timestamps, foreign keys).

- JSON “bag” handling
    - Stored as MySQL `JSON` with camelCase serialization for consistent client shape.
    - Read projections return the `bag` object as-is; endpoints that need specific fields can extract them in the client
      or add targeted projections when required.

- Performance considerations
    - Views must only return what the endpoint needs; do not over-fetch.
    - Prefer server-side projection (`Select(...)`) into view types instead of materializing entities and mapping
      in-memory.
    - Use denormalized fields (e.g., `TriggerType`) to support efficient queries when JSON filtering would be expensive.

- Contracts and evolution
    - Views are the API contracts — safe to add new optional fields, but removing/renaming is breaking.
    - Add new views when a use-case diverges materially instead of overloading an existing one.
- Do not use DTO classes; endpoints return View types from `*.Domain.Views` directly.
- Requests return views: Commands/Queries are defined as `RequestBase<ViewType>` (or `RequestBase` for void) — avoid
  `*Response` classes.

### Timepoints via History, not fields

- API models (Views) never expose direct `...On` timestamp properties (e.g., `CreatedOn`, `DeliveredOn`).
- Timepoints are represented as history entries (`Entry<HistoryEnum>`); include `History` in detailed views when needed
  and derive convenience flags (e.g., `isRead`) from history/state.

## List Schema Validation & Indexing (Bag)

- Validation: On write, the `ListItemBagValidator` validates the `bag` against the list's `Columns` (type checks per
  `ColumnType`; domain service injected into the aggregate);
  optional required rules future).
- Status: `status` remains a conventional field; transitions can be validated as needed.
- Indexing (future guidance): Frequently queried bag fields should be denormalized (computed/materialized columns) or
  indexed via JSON-path when supported; paginate server-side.

## Endpoints Overview

- Lists
    - `POST /api/lists/` → Create list (returns `ListItemDefinition`)
    - `PUT /api/lists/{listId}` → Update list definition (Columns/Statuses/Transitions)
    - `DELETE /api/lists/{listId}` → Delete list
    - `GET /api/lists/names` → List names (for pickers/navigation)
    - `GET /api/lists/{listId}/itemDefinition` → Read list definition (view)
    - `GET /api/lists/{listId}/items` → Paged items (`PagedList`)
    - `GET /api/lists/{listId}/history` → Paginated list history (`HistoryPage<ListHistoryType>`)
    - `POST /api/lists/{listId}/items` → Create item
    - `PUT /api/lists/{listId}/items/{itemId}` → Update item
    - `DELETE /api/lists/{listId}/items/{itemId}` → Delete item
    - `GET /api/lists/{listId}/items/{itemId}/history` → Paginated item history (`HistoryPage<ItemHistoryType>`)
    - `POST /api/lists/{listId}/migrations` → Run migrations (`mode: dryRun|execute`), progress via SSE
    - `GET /api/lists/{listId}/statusTransitions` → Read-only transitions (for editors)

- Notifications
    - `GET /api/notifications` → User notifications (filters: since, unread, listId, paging)
    - `GET /api/notifications/{notificationId}` → Notification details
    - `POST /api/notifications/{notificationId}/read` → Mark as read
    - `POST /api/notifications/readAll` → Mark all as read
    - Rules:
        - `GET /api/notifications/rules` → User rules
        - `POST /api/notifications/rules` → Create rule
        - `PUT /api/notifications/rules` → Update rule
        - `DELETE /api/notifications/rules` → Delete rule

- Change Feed (SSE)
    - `GET /api/changes/stream` → Server-Sent Events (typed envelopes `{ type, data, occurredOn }`)
    - Event types include list item changes, list lifecycle (created/deleted/updated), notifications, and migration
      progress.

## List Schema Migrations (Outbox + SSE)

Goals

- Safely evolve list schemas without breaking items or client contracts.
- Provide dry-run validation, explicit execution, durable eventing (Outbox), and real-time progress (SSE).

Key Concepts

- Stable Column Keys: Columns have immutable `StorageKey` values (e.g., `prop1`, `prop2`) decoupled from display `Name`.
    - Keys uniquely identify fields in item `bag` payloads and support safe renames.
    - Aggregates assign missing keys during list creation; persistence stores round-trip keys.

- MigrationPlan: Declarative set of operations the server validates and executes.
    - ChangeColumnType(key, targetType, converter)
    - RemoveColumn(key, policy)
    - TightenConstraints(key, required?, allowedValues?, min?, max?, regex?)
    - RenameStorageKey(from, to)
    - RemoveStatus(name, mapTo?)

- Validation (Dry-Run):
    - Checks existence, collisions, converter presence, and constraint tightening safety.
    - Returns `MigrationDryRunResult { IsSafe, Messages[] }` without modifying data.

- Execution (with SSE):
    - Applies metadata changes and iterates items for data transformations (e.g., type conversion, field removal, status
      mapping).
    - Publishes progress via integration events routed through the Outbox and SSE feed (see Migration Events).

Migration Events

- `ListMigrationStartedIntegrationEvent`: marks the migration request as executing.
- `ListMigrationProgressIntegrationEvent`: emits progress percentage and contextual messages.
- `ListMigrationCompletedIntegrationEvent`: signals all operations succeeded.
- `ListMigrationFailedIntegrationEvent`: signals execution stopped with errors; payload should guide remediation.

Contracts & Endpoints

- POST `/api/lists/{listId}/migrations`
    - Body: `{ plan, mode }` where mode is `dryRun` or `execute`.
    - Returns `MigrationDryRunResult` (for both dry-run and execute initiation).
    - UI subscribes to SSE to reflect live progress.

Durability & Delivery

- Outbox: All migration events are persisted and dispatched independently with exponential backoff and retention
  cleanup.
- SSE: Host streams typed envelopes `{ type, data, occurredOn }`; client routes by `type` (including the migration
  events above) and invalidates caches accordingly.

CQRS Separation

- Aggregates: Hydration-first for updates; schema updates still flow through aggregate guardrails outside explicit
  migrations.
- Read Side: `IGetListItemDefinition` composes columns/statuses/transitions for validation and editors.

Safety & Guardrails

- The default update path rejects breaking changes (removals, type changes, constraint tightening) — use MigrationPlan
  instead.
- Migration validation surfaces potential breakage; execution can include converter logic and mapping policies.

Future Enhancements

- Persist `StorageKey` in the database with uniqueness per list and indexes to enforce the stable key contract
  consistently across read models.
- Converter Registry: Named converters with strict/lenient modes and rich diagnostics.
- Abort Thresholds: Stop execution if error rate exceeds a configurable limit; emit `Failed` event with details.
- Rollback Strategy: Archive removed columns or write to a shadow field; expose recovery scripts.

## Testing

- Unit tests live alongside modules: `{Module}.Tests`.
- Test projects reference Domain and Application layers.
- Infrastructure mocked via interfaces defined in Domain.

## Config

- Greenfield dev may keep simple credentials checked in; production uses secrets/env vars.

## Performance considerations

- Views must only return what the endpoint needs; do not over-fetch.
- Prefer server-side projection (`Select(...)`) into view types instead of materializing entities and mapping in-memory.
- Use denormalized fields (e.g., `TriggerType`) to support efficient queries when JSON filtering would be expensive.

## Routing (React Router v6)

- **Framework:** React 19
- **Build Tool:** Vite (`@vitejs/plugin-react`)
- **UI:** MUI (Core + X DataGrid / Date Pickers)
- **Data Fetching / Cache:** React Query v5 (`@tanstack/react-query`)
- **Routing:** React Router v6 (`react-router-dom`)

### Structure

Routes are defined in `src/Lister.Client/src/router.tsx` using `createBrowserRouter`.  
Loaders are used for **auth-gated** and **data-prefetched** routes.

**Example:**

```tsx
export const createAppRouter = (queryClient: QueryClient) =>
  createBrowserRouter([
    {
      path: '/',
      element: <Shell />,
      loader: async () => {
        const auth = await ensureAuthenticated();
        if (!auth) throw redirect('/sign-in');
        return null;
      },
      children: [
        { index: true, element: <Dashboard /> },
        { path: 'lists/:listId', loader: ensureListPrefetch(queryClient), element: <ListItems /> },
      ],
    },
  ]);
```

### History UX

- Shared drawers live under `src/Lister.Client/src/components/history` and wrap a generic `HistoryDrawer` (React Query +
  `SideDrawerContent`).
- `ListItemsPage` surfaces a “Show history” action beside “Create an Item”; `ListItemDetailsPage` mirrors the
  affordance.
- SSE change-feed handlers (`shell.tsx`) invalidate `list-history` and `item-history` query keys so drawers stay current
  after mutations or background events.
