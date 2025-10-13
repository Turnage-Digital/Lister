import {
  NotificationDetails,
  NotificationListPage,
  NotificationsSearch,
} from "../models";

const throwIfNotOk = async (response: Response, message: string) => {
  if (response.ok) {
    return response;
  }

  const text = await response.text().catch(() => undefined);
  throw new Error(text || message);
};

export const fetchNotifications = async (
  search: NotificationsSearch = {},
): Promise<NotificationListPage> => {
  const params = new URLSearchParams();
  if (search.since) params.set("since", search.since);
  if (typeof search.unread === "boolean")
    params.set("unread", String(search.unread));
  if (search.listId) params.set("listId", search.listId);
  params.set("pageSize", String(search.pageSize ?? 20));
  params.set("page", String(search.page ?? 0));

  const response = await fetch(`/api/notifications?${params.toString()}`);
  await throwIfNotOk(response, "Failed to load notifications");
  const retval = (await response.json()) as NotificationListPage;
  return retval;
};

export const fetchNotificationDetails = async (
  notificationId: string,
): Promise<NotificationDetails> => {
  const response = await fetch(`/api/notifications/${notificationId}`);
  await throwIfNotOk(response, "Failed to load notification details");
  return (await response.json()) as NotificationDetails;
};

export const markNotificationRead = async (notificationId: string) => {
  const response = await fetch(`/api/notifications/${notificationId}/read`, {
    method: "POST",
  });
  await throwIfNotOk(response, "Failed to mark notification as read");
};

export const markAllNotificationsRead = async () => {
  const response = await fetch(`/api/notifications/readAll`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify({}),
  });
  await throwIfNotOk(response, "Failed to mark notifications as read");
};
