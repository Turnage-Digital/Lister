import { queryOptions } from "@tanstack/react-query";

import {
  fetchNotificationDetails,
  fetchNotifications,
} from "./api/notifications";
import {
  ItemDetails,
  ListItemDefinition,
  ListName,
  ListSearch,
  NotificationRule,
  NotificationsSearch,
  PagedList,
} from "./models";

export const listNamesQueryOptions = () =>
  queryOptions({
    queryKey: ["list-names"],
    queryFn: async () => {
      const request = new Request("/api/lists/names", {
        method: "GET",
      });
      const response = await fetch(request);
      if (!response.ok) {
        const message = await response
          .text()
          .catch(() => "Failed to load list names");
        throw new Error(message || "Failed to load list names");
      }
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
    queryFn: () => fetchNotifications(search),
  });

export const notificationDetailsQueryOptions = (notificationId?: string) =>
  queryOptions({
    queryKey: ["notification", notificationId],
    queryFn: () => fetchNotificationDetails(notificationId as string),
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
      if (!response.ok) {
        throw new Error("Failed to load notification rules");
      }
      const retval: NotificationRule[] = await response.json();
      return retval;
    },
  });
