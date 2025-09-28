# Lister

Another list app... 🤦‍♂️

codex
Here’s a clear, end-to-end workflow plan so we can verify what’s done, what’s partial, and what we still need to build. I’ve grouped each workflow with goals, main UI, API/SSE backing, current status, and next steps.

Auth

- Sign In/Out: user logs in and sees AppBar + lists; log out returns to sign-in.
    - UI: sign-in.tsx, UserMenu → logout
    - API: /identity/login, /identity/logout
    - Status: Implemented
    - Next: None
- Sign Up / Reset Password: optional flows for adding new users and recovering access.
    - UI: sign-up.tsx, reset-password.tsx
    - API: Identity endpoints (in server)
    - Status: Implemented
    - Next: None

Lists: Management

- View Available Lists: see list cards with counts.
    - UI: Home routed list (already present)
    - API: GET /api/lists/names
    - Status: Implemented
    - Next: None
- Create List: define name, columns (type + constraints), statuses (name + color).
    - UI: /_auth.create.tsx, EditListColumnsContent, EditListStatusesContent
    - API: POST /api/lists
    - Status: Implemented
    - Next: Confirm constraints UI (allowedValues/min/max/regex) are fully exposed in column editor.
- Edit List (Name/Columns/Statuses): update name, adjust schema, adjust statuses.
    - UI: EditListNameContent, EditListColumnsContent, EditListStatusesContent
    - API: existing endpoints used by current components (verify; if missing, add)
    - Status: Likely Implemented (components exist)
    - Next: Confirm columns editor surfaces new constraints (allowedValues/min/max/regex).
- Delete List: remove list (soft delete).
    - UI: list screen action
    - API: DELETE /api/lists/{listId}
    - SSE: ListDeletedIntegrationEvent invalidates caches
    - Status: Implemented
    - Next: Add route guard to redirect if user is on deleted list when event arrives.

Lists: Status Transitions

- Manage Transitions: set allowed “from → to” matrix for statuses.
    - UI: Transitions editor at /_auth/$listId/transitions (new)
    - API: GET/PUT /api/lists/{listId}/statusTransitions
    - Status: Implemented (screen added)
    - Next: None
- Enforce Transitions on Item Update: block disallowed transitions.
    - UI: Item edit experience should constrain status dropdown to allowed next statuses.
    - API: PUT /api/lists/{listId}/items/{itemId} (server enforces)
    - Status: Partial (server-enforced; client must pre-filter status options)
    - Next: Add transitions-aware status dropdown to item edit.

Items

- Browse Items: paged list with sort (and optional search).
    - UI: /_auth.$listId.index.tsx with desktop/mobile views
    - API: GET /api/lists/{listId}/items?page&pageSize&field&sort
    - Status: Implemented
    - Next: None
- View Item Details: show bag values and status.
    - UI: /_auth.$listId.$itemId.index.tsx, ItemCard
    - API: GET /api/lists/{listId}/items/{itemId}
    - Status: Implemented
    - Next: None
- Create Item (Form-driven): honor required/allowed/min/max/regex.
    - UI: /_auth.$listId.create.tsx
    - API: POST /api/lists/{listId}/items
    - Status: Implemented (server-enforced); client-side checks to polish
    - Next: Add inline client checks for constraints before submit.
- Update Item: edit fields; status changes constrained by transitions.
    - UI: /_auth.$listId.$itemId.index.tsx (edit interaction)
    - API: PUT /api/lists/{listId}/items/{itemId}
    - Status: Partial (server-enforced; UI needs transitions-aware dropdown)
    - Next: Implement dropdown + error surface for failed transitions.
- Delete Item: remove an item from list.
    - UI: delete action in list/table
    - API: DELETE /api/lists/{listId}/items/{itemId}
    - Status: Implemented
    - Next: None
- Smart Paste / Convert Text: bulk create helper from pasted text.
    - UI: SmartPasteDialog (present)
    - API: POST /api/lists/{listId}/items/convert-text-to-list-item
    - Status: Implemented
    - Next: None

Validation (Enforced)

- Per Column: required, allowedValues, min/max (number), regex (text)
    - Server: enforced in ListsAggregate.ValidateBagAsync
    - Client: should pre-validate and show friendly errors
    - Status: Server done; client partial
    - Next: Add light client checks to create/edit forms.

Notifications

- View Notifications (Drawer/List): filters for unread, list, since; pagination.
    - UI: Notifications bell + drawer (new), details view
    - API: GET /api/notifications?since&unread&listId&pageSize&page
    - Status: Implemented (basic), filters/paging partial
    - Next: Wire filters and real paging; cache append for “Load more”.
- View Details: attempts and history.
    - UI: details panel in drawer (new)
    - API: GET /api/notifications/{id}
    - Status: Implemented
    - Next: None
- Unread Count: header badge shows count.
    - UI: bell badge (new)
    - API: GET /api/notifications/unreadCount?listId?
    - SSE: updates via events
    - Status: Implemented
    - Next: None
- Mark One as Read: update individual notification.
    - UI: button in drawer/details (new)
    - API: POST /api/notifications/{id}/read
    - Status: Implemented
    - Next: None
- Mark All as Read: bulk mark by cutoff.
    - UI: add “Mark all read” in drawer header
    - API: POST /api/notifications/readAll (optional { before })
    - Status: Pending (API implemented, UI pending)
    - Next: Add this control with optimistic update.
- Notifications Rule CRUD: create/update/delete rules.
    - UI: Not in main UI; available via MCP server tools
    - API: /api/notifications/rules (POST/PUT/DELETE)
    - Status: Available via MCP; UI pending (if desired)
    - Next: Defer unless you want rules UI now.

Real-time (SSE)

- Change Feed: keep UI in sync without manual refresh.
    - UI: SSE wired in __root.tsx, event→cache mappings
    - API: GET /api/changes/stream
    - Status: Implemented (cache invalidations for Lists and Notifications)
    - Next: Add a route guard redirect on ListDeletedIntegrationEvent.

Errors and UX Polish

- Uniform Errors: map server messages to toasts/inline errors.
    - Status: Partial
    - Next: Add centralized error mapping in query-options.ts + Snackbar toasts.
- Skeletons/Empty States: for loading and empty lists.
    - Status: Partially present
    - Next: Ensure for notifications drawer and transitions editor.

Now / Next / Later (to implement)

- Now
    - Transitions-aware status editing in item details (client pre-filter + server fallback).
    - Notifications drawer: add Mark-all-read with optimistic updates.
    - Add mutations to src/query-options.ts with consistent error mapping.
- Next
    - Client-side validation (required/allowed/min/max/regex) in create/edit item forms.
    - Notifications drawer filters (unread/list/since) and real pagination.
    - Route guard for ListDeleted event.
- Later
    - Toast system for success/failure + SSE disconnect notice.
    - Optional UI for Notifications rule management.

If this looks right, I’ll start with the “Now” work: item status dropdown (transitions-aware), mark-all-read in drawer, and the four mutations with cache updates.

▌ Summarize recent commits

> _It keeps you runnin', yeah it keeps you runnin'_ - The Doobie Brothers