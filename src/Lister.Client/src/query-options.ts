import { infiniteQueryOptions, queryOptions } from "@tanstack/react-query";

import {
  HistoryPage,
  ItemDetails,
  ListItemDefinition,
  ListName,
  ListSearch,
  MigrationProgressRecord,
  NotificationDetails,
  NotificationListPage,
  NotificationRule,
  NotificationsSearch,
  PagedList,
} from "./models";

const throwIfNotOk = async (response: Response, message: string) => {
  if (response.ok) {
    return response;
  }

  const text = await response.text().catch(() => undefined);
  throw new Error(text || message);
};

const fetchNotificationsPage = async (
  search: NotificationsSearch,
  pageParam: number,
): Promise<NotificationListPage> => {
  const params = new URLSearchParams();
  if (search.since) params.set("since", search.since);
  if (typeof search.unread === "boolean")
    params.set("unread", String(search.unread));
  if (search.listId) params.set("listId", search.listId);
  params.set("pageSize", String(search.pageSize ?? 20));
  params.set("page", String(pageParam));

  const response = await fetch(`/api/notifications?${params.toString()}`);
  await throwIfNotOk(response, "Failed to load notifications");
  return (await response.json()) as NotificationListPage;
};

export const notificationsInfiniteQueryOptions = (
  search: NotificationsSearch = {},
) =>
  infiniteQueryOptions<
    NotificationListPage,
    Error,
    NotificationListPage,
    readonly ["notifications", NotificationsSearch],
    number
  >({
    queryKey: ["notifications", search],
    initialPageParam: search.page ?? 0,
    queryFn: ({ pageParam }) => fetchNotificationsPage(search, pageParam),
    getNextPageParam: (lastPage, allPages) =>
      lastPage.hasMore ? allPages.length : undefined,
  });

const fetchHistoryPage = async (url: string): Promise<HistoryPage> => {
  const response = await fetch(url);
  await throwIfNotOk(response, "Failed to load history");
  return (await response.json()) as HistoryPage;
};

const nextHistoryPageParam = (lastPage: HistoryPage) => {
  const totalPages = Math.ceil(lastPage.total / lastPage.pageSize);
  const next = lastPage.page + 1;
  return next < totalPages ? next : undefined;
};

export const listHistoryInfiniteQueryOptions = (
  listId: string,
  pageSize = 20,
) =>
  infiniteQueryOptions<
    HistoryPage,
    Error,
    HistoryPage,
    readonly ["list-history", string, number],
    number
  >({
    queryKey: ["list-history", listId, pageSize],
    initialPageParam: 0,
    enabled: Boolean(listId),
    queryFn: ({ pageParam }) =>
      fetchHistoryPage(
        `/api/lists/${listId}/history?page=${typeof pageParam === "number" ? pageParam : 0}&pageSize=${pageSize}`,
      ),
    getNextPageParam: nextHistoryPageParam,
  });

export const itemHistoryInfiniteQueryOptions = (
  listId: string,
  itemId: number,
  pageSize = 20,
) =>
  infiniteQueryOptions<
    HistoryPage,
    Error,
    HistoryPage,
    readonly ["item-history", string, number, number],
    number
  >({
    queryKey: ["item-history", listId, itemId, pageSize],
    initialPageParam: 0,
    enabled: Boolean(listId) && Number.isInteger(itemId),
    queryFn: ({ pageParam }) =>
      fetchHistoryPage(
        `/api/lists/${listId}/items/${itemId}/history?page=${
          typeof pageParam === "number" ? pageParam : 0
        }&pageSize=${pageSize}`,
      ),
    getNextPageParam: nextHistoryPageParam,
  });

export const listNamesQueryOptions = () =>
  queryOptions({
    queryKey: ["list-names"],
    queryFn: async () => {
      const response = await fetch("/api/lists/names", { method: "GET" });
      await throwIfNotOk(response, "Failed to load list names");
      return (await response.json()) as ListName[];
    },
  });

export const listItemDefinitionQueryOptions = (listId?: string) =>
  queryOptions({
    queryKey: ["list-definition", listId],
    queryFn: async () => {
      const response = await fetch(`/api/lists/${listId}/itemDefinition`, {
        method: "GET",
      });
      await throwIfNotOk(response, "Failed to load list definition");
      return (await response.json()) as ListItemDefinition;
    },
    enabled: Boolean(listId),
  });

export const pagedItemsQueryOptions = (search: ListSearch, listId?: string) =>
  queryOptions({
    queryKey: [
      "list-items",
      listId,
      search.page,
      search.pageSize,
      search.field ?? "id",
      search.sort ?? "asc",
    ],
    queryFn: async () => {
      let url = `/api/lists/${listId}/items?page=${search.page}&pageSize=${search.pageSize}`;
      if (search.field && search.sort) {
        url += `&field=${search.field}&sort=${search.sort}`;
      }
      const response = await fetch(url, { method: "GET" });
      await throwIfNotOk(response, "Failed to load list items");
      return (await response.json()) as PagedList;
    },
    enabled: Boolean(listId),
  });

export const itemQueryOptions = (listId?: string, itemId?: number) =>
  queryOptions({
    queryKey: ["list-item", listId, itemId],
    queryFn: async () => {
      const response = await fetch(`/api/lists/${listId}/items/${itemId}`, {
        method: "GET",
      });
      await throwIfNotOk(response, "Failed to load item");
      return (await response.json()) as ItemDetails;
    },
    enabled: Boolean(listId) && Boolean(itemId),
  });

export const notificationDetailsQueryOptions = (notificationId?: string) =>
  queryOptions({
    queryKey: ["notification", notificationId],
    queryFn: async () => {
      const response = await fetch(`/api/notifications/${notificationId}`, {
        method: "GET",
      });
      await throwIfNotOk(response, "Failed to load notification details");
      return (await response.json()) as NotificationDetails;
    },
    enabled: Boolean(notificationId),
  });

export const unreadCountQueryOptions = (listId?: string) =>
  queryOptions({
    queryKey: ["notifications-unread-count", listId ?? null],
    queryFn: async () => {
      const url = listId
        ? `/api/notifications/unreadCount?listId=${encodeURIComponent(listId)}`
        : "/api/notifications/unreadCount";
      const response = await fetch(url, { method: "GET" });
      await throwIfNotOk(response, "Failed to load unread notification count");
      return (await response.json()) as number;
    },
  });

export const notificationRulesQueryOptions = (listId?: string) =>
  queryOptions({
    queryKey: ["notification-rules", listId ?? null],
    enabled: Boolean(listId),
    queryFn: async () => {
      if (!listId) {
        return [] as NotificationRule[];
      }

      const params = new URLSearchParams();
      params.set("listId", listId);
      const request = new Request(
        `/api/notifications/rules?${params.toString()}`,
        { method: "GET" },
      );
      const response = await fetch(request);
      if (response.status === 204) {
        return [] as NotificationRule[];
      }
      await throwIfNotOk(response, "Failed to load notification rules");
      return (await response.json()) as NotificationRule[];
    },
  });

interface MigrationJobStatusResponse {
  jobId: string;
  sourceListId: string;
  correlationId: string;
  stage: string;
  requestedBy?: string;
  createdOn?: string;
  startedOn?: string;
  completedOn?: string;
  backupExpiresOn?: string;
  backupRemovedOn?: string;
  attempts?: number;
  lastError?: string;
}

export const migrationProgressQueryOptions = (
  listId: string,
  correlationId: string,
) =>
  queryOptions<MigrationProgressRecord | null>({
    queryKey: ["list-migration-progress", listId, correlationId],
    enabled: Boolean(listId) && Boolean(correlationId),
    queryFn: async () => {
      const response = await fetch(
        `/api/lists/${listId}/migrations/${correlationId}`,
        { method: "GET" },
      );

      if (response.status === 404) {
        return null;
      }

      await throwIfNotOk(response, "Failed to load migration status");
      const dto = (await response.json()) as MigrationJobStatusResponse;
      const stage = dto.stage as MigrationProgressRecord["stage"];

      let percent: number | undefined;
      if (stage === "Completed" || stage === "Archived") {
        percent = 100;
      } else if (stage === "Failed") {
        percent = 0;
      }

      return {
        listId: dto.sourceListId,
        correlationId: dto.correlationId,
        stage,
        requestedBy: dto.requestedBy,
        createdOn: dto.createdOn,
        startedOn: dto.startedOn,
        completedOn: dto.completedOn,
        backupExpiresOn: dto.backupExpiresOn,
        backupRemovedOn: dto.backupRemovedOn,
        attempts: dto.attempts,
        lastError: dto.lastError,
        updatedAt: dto.completedOn ?? dto.startedOn ?? dto.createdOn,
        percent,
      } satisfies MigrationProgressRecord;
    },
  });
