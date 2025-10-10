# Client Architecture

This document describes how the React client is structured, how it talks to the API (TanStack Query + Router), and how it reacts to server events (SSE) while keeping domain separation clear.

## Tech Stack

- React + TypeScript, Material UI for components
- TanStack Router for routing; TanStack Query for data fetching/caching
- Server‑Sent Events (SSE) for real‑time invalidation
- Minimal global state — prefer query cache and local component state

## Folder Structure (high level)

```
src/Lister.Client/src
  /components          # Reusable UI (grids, editors, drawers)
  /models              # Type definitions for views/value objects
  /routes              # TanStack Router route files
  /lib                 # SSE utilities and small helpers
  query-options.ts     # All queryOptions builders in one place
  auth.tsx             # Light auth helpers/components

### Barrel Exports

- Every leaf folder uses an `index.ts` to re-export its contents (e.g., `components/side-drawer/index.ts`).
- The top-level `components/index.ts` re-exports all subfolders and single-file components.
- This enables clean imports like:
  - `import { SideDrawer, UserMenu, NotificationsBell } from "../components";`
  - `import { ItemsDesktopView } from "../components";`
```

## App Shell & Routing

- Root route (`routes/__root.tsx`) provides:
  - AppBar + layout
  - QueryClient via Router context
  - SSE bootstrap (see SSE section) and event → cache invalidation mapping
- Auth gating: anonymous users render auth routes; logged‑in users render main app via nested routes

## Data Fetching & Caching (TanStack Query)

- All reads go through `query-options.ts` using `queryOptions({ queryKey, queryFn })`
- Query keys are namespaced (e.g., `['list-items', listId, page, pageSize, sortField, sortDir]`)
- Mutations:
  - POST/PUT/DELETE call `fetch` with JSON body
  - On success, invalidate relevant keys (or rely on SSE invalidation)
  - Keep mutations simple; prefer server as source of truth over complex optimistic updates

## SSE (Change Feed)

- Utility: `lib/sse.ts` exposes `connectChangeFeed(handler)` and `createChangeFeedRouter(routes)`
- Root route subscribes once and routes typed envelopes `{ type, data, occurredOn }` to cache invalidations
- Current mappings include:
  - List item created/deleted → invalidate `list-items` and `list-item`
  - List deleted → invalidate `list-names`, `list-items`
  - List updated → invalidate `list-definition`
  - Notifications created/read/processed → invalidate `notifications` and `notifications-unread-count`
  - Migrations progress (optional UI hooks) → may trigger toasts or progress bars later

## Views & API Contracts

- Client types live under `models/` and mirror server views/value objects
- Treat server views as the source of truth; client augments with presentational helpers only
- Do not import domain/persistence types into the client; use only the API view shapes

## Feature Boundaries

- Lists
  - List pages (paged items), item details, create/update item
  - Definition reads (`GET /api/lists/{listId}/itemDefinition`)
  - Updates to definition use unified `PUT /api/lists/{listId}` (columns, statuses, transitions)
  - Create list includes transitions in the same payload (no separate transitions route)

- Notifications
  - Drawer/bell UI, list/detail pages
  - Mark read / mark all read APIs

## Forms & Validation

- Client editors prefer simple validation (formatting/required) derived from `ListItemDefinition`
- Always enforce authoritative validation on the server; surface errors inline
  - Example: status transition violations show server message; dropdown pre‑filters allowed transitions if known

## Error Handling & Auth

- 401s: prompt sign‑in; advise clearing stale cookies after DB resets (data protection keys)
- Show concise error toasts/messages; avoid leaking server stack traces

## Adding New Features

1) Add queryOptions (read) and route(s)
2) Add mutations (write) and wire invalidation on success
3) Add SSE mappings for any new integration events
4) Keep UI local state minimal; lean on query cache for shared data

## Performance & UX Notes

- Render skeletons/placeholders while queries are `isLoading`
- Prefer pagination over infinite large lists
- Avoid heavy client validation libraries that duplicate server logic; keep it light

## Testing Approach

- Component tests around critical flows (e.g., item edit form) using MSW or simple fetch mocks
- Avoid Storybook/zod per domain separation guidance; rely on server views and lightweight fixtures
