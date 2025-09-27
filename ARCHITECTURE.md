# Lister Architecture

This document captures the intentional architecture boundaries and composition patterns for the Lister monorepo.

## Solution Structure

```
/src
  /Lister.App.Server         # Host/Composition Root
  /Lister.Client             # React SPA
  /Lister.Mcp.Server         # Dev-only MCP sidecar
  /Modules
    /Core                    # Shared kernel
      /*.Domain
      /*.Application  
      /*.Infrastructure.Sql
      /*.Infrastructure.OpenAi
    /Lists                   # Lists module
      /*.Domain
      /*.Application
      /*.Infrastructure.Sql
      /*.Tests
    /Notifications           # Notifications module
      /*.Domain
      /*.Application
      /*.Infrastructure.Sql
    /Users                   # Users/Identity module
      /*.Domain
      /*.Application
      /*.Infrastructure.Sql
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

## Composition Root (Host)

- Close open-generic handlers via DI. Do not add adapters in Application.
- Register module services and aggregates per concrete EF types.
- Set EF migration assemblies for each DbContext.

Example registrations:

```csharp
// Lists
services.AddScoped(typeof(IRequestHandler<CreateListItemCommand, ListItem>),
    typeof(CreateListItemCommandHandler<ListDb, ItemDb>));

// Notifications
services.AddScoped(typeof(IRequestHandler<CreateNotificationRuleCommand, CreateNotificationRuleResponse>),
    typeof(CreateNotificationRuleCommandHandler<NotificationRuleDb, NotificationDb>));

// Integration events
services.AddScoped<INotificationHandler<ListItemCreatedIntegrationEvent>,
    NotifyEventHandler<NotificationRuleDb, NotificationDb>>();
```

## Module Registration

Each module provides extension methods for registration:

- `AddInfrastructure()`: Registers DbContexts, stores, query services
- `AddDomain()`: Registers aggregates, domain services, domain event handlers
- `AddApplication()`: Registers MediatR handlers, pipeline behaviors, controllers

Host calls these in order: Infrastructure → Domain → Application

## MediatR

- Use assembly scanning for non-generic handlers.
- Use closed DI bindings (above) for generic handlers and integration event handlers.

## Integration Events

- Domain events that cross module boundaries (e.g., `ListItemCreatedIntegrationEvent`)
- Handlers registered in Host using MediatR's `INotificationHandler<T>`
- Enable loose coupling between modules (Lists → Notifications)
- Published via MediatR after successful aggregate operations

## Persistence

- One DbContext per module; migrations live in the module's Infrastructure.Sql project.
- Host provides `DatabaseOptions.*MigrationAssemblyName` and connection string.

## Store Pattern

- Stores (`INotificationsStore<T>`, `INotificationRulesStore<T>`, etc.) handle low-level persistence operations
- Aggregates compose stores and add business logic, domain events, and invariant enforcement
- Stores are registered in the Host and injected into aggregates via `IUnitOfWork`

## Behaviors & Controllers

- `AssignUserBehavior` populates `UserId` on `RequestBase{,T}`.
- `LoggingBehavior` logs request/response boundaries for RequestBase types.
- Controllers live in Application (as done here); Host maps controllers.

## Config

- Greenfield dev may keep simple credentials checked in; production uses secrets/env vars.

## Testing

- Unit tests live alongside modules: `{Module}.Tests`
- Test projects reference Domain and Application layers
- Infrastructure mocked via interfaces defined in Domain

## Conventions

- Application must not reference Infrastructure projects.
- Only Host closes generics and wires concrete EF implementations.
- New modules follow: Domain + Application + Infrastructure.Sql; wire in Host.

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
    - AutoMapper profiles translate persistence entities to view models (e.g., `ListsMappingProfile`).
    - Query services return projection models tailored for reads; commands operate on `IWritable*` via aggregates.

## Read Path & Views

- Views are read-only projections tailored to specific API needs.
    - Multiple views can represent the same entity for different use-cases.
      - Example (Lists): `ListItem` (lightweight for listings) and `ItemDetails` (includes `History`) both represent an
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

## List Schema Validation & Indexing (Bag)

- Validation: On write, the aggregate validates the `bag` against the list's `Columns` (type checks per `ColumnType`;
  optional required rules future).
- Status: `status` remains a conventional field; transitions can be validated as needed.
- Indexing (future guidance): Frequently queried bag fields should be denormalized (computed/materialized columns) or
  indexed via JSON-path when supported; paginate server-side.

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
