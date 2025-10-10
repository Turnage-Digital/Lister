export type NotificationChannel =
  | "InApp"
  | "Email"
  | "Sms"
  | "Push"
  | "Webhook";

export type DeliveryStatus =
  | "Delivered"
  | "Failed"
  | "Pending"
  | "Processing"
  | "Skipped";

export interface DeliveryAttemptView {
  channel: NotificationChannel;
  // ISO timestamp
  attemptedOn: string;
  status: DeliveryStatus;
  failureReason?: string | null;
  attemptNumber: number;
}

export interface NotificationHistoryEntry {
  // e.g., Created | Delivered | Read
  type: string;
  // ISO timestamp
  on: string;
  by?: string | null;
  bag?: Record<string, unknown> | null;
}

export interface NotificationDetails {
  id: string;
  userId: string;
  listId?: string | null;
  itemId?: number | null;
  title: string;
  body: string;
  metadata?: Record<string, unknown> | null;
  isRead: boolean;
  history: NotificationHistoryEntry[];
  deliveryAttempts: DeliveryAttemptView[];
}

export interface NotificationListItem {
  id: string;
  title: string;
  body: string;
  isRead: boolean;
  // ISO timestamp
  occurredOn: string;
  listId?: string | null;
  itemId?: number | null;
  metadata?: Record<string, unknown> | null;
}

export interface NotificationListPage {
  items: NotificationListItem[];
  page: number;
  pageSize: number;
  total: number;
}

export interface NotificationsSearch {
  since?: string;
  unread?: boolean;
  listId?: string;
  pageSize?: number;
  page?: number;
}
