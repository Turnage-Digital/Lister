import { queryOptions } from "@tanstack/react-query";

import {
  ItemDetails,
  ListItemDefinition,
  ListName,
  ListSearch,
  PagedList,
  NotificationDetails,
  NotificationListPage,
} from "./models";

export const listNamesQueryOptions = () =>
  queryOptions({
    queryKey: ["list-names"],
    queryFn: async () => {
      const request = new Request("/api/lists/names", {
        method: "GET",
      });
      const response = await fetch(request);
      const retval: ListName[] = await response.json();
      return retval;
    },
  });

export const listItemDefinitionQueryOptions = (listId?: string) =>
  queryOptions({
    queryKey: ["list-definition", listId],
    queryFn: async () => {
      const request = new Request(`/api/lists/${listId}/itemDefinition`, {
        method: "GET",
      });
      const response = await fetch(request);
      const retval: ListItemDefinition = await response.json();
      return retval;
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
      const request = new Request(url, {
        method: "GET",
      });
      const response = await fetch(request);
      const retval: PagedList = await response.json();
      return retval;
    },
    enabled: Boolean(listId),
  });

export const itemQueryOptions = (listId?: string, itemId?: number) =>
  queryOptions({
    queryKey: ["list-item", listId, itemId],
    queryFn: async () => {
      const request = new Request(`/api/lists/${listId}/items/${itemId}`, {
        method: "GET",
      });
      const response = await fetch(request);
      const retval: ItemDetails = await response.json();
      return retval;
    },
    enabled: Boolean(listId) && Boolean(itemId),
  });

// Notifications
export interface NotificationsSearch {
  since?: string;
  unread?: boolean;
  listId?: string;
  pageSize?: number;
  page?: number;
}

export const notificationsListQueryOptions = (
  search: NotificationsSearch = {},
) =>
  queryOptions({
    queryKey: [
      "notifications",
      search.since ?? null,
      search.unread ?? null,
      search.listId ?? null,
      search.pageSize ?? 20,
      search.page ?? 0,
    ],
    queryFn: async () => {
      const params = new URLSearchParams();
      if (search.since) params.set("since", search.since);
      if (typeof search.unread === "boolean")
        params.set("unread", String(search.unread));
      if (search.listId) params.set("listId", search.listId);
      params.set("pageSize", String(search.pageSize ?? 20));
      params.set("page", String(search.page ?? 0));

      const request = new Request(`/api/notifications?${params.toString()}`, {
        method: "GET",
      });
      const response = await fetch(request);
      const retval: NotificationListPage = await response.json();
      return retval;
    },
  });

export const notificationDetailsQueryOptions = (notificationId?: string) =>
  queryOptions({
    queryKey: ["notification", notificationId],
    queryFn: async () => {
      const request = new Request(`/api/notifications/${notificationId}`, {
        method: "GET",
      });
      const response = await fetch(request);
      const retval: NotificationDetails = await response.json();
      return retval;
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
      const retval: number = await response.json();
      return retval;
    },
  });
