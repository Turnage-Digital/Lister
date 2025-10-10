import { NotificationRule, NotificationRuleInput } from "../models";

const buildRequestInit = (body: unknown, method: string): RequestInit => ({
  method,
  headers: {
    "Content-Type": "application/json",
  },
  body: JSON.stringify(body),
});

export const createNotificationRule = async (
  listId: string,
  input: NotificationRuleInput,
): Promise<NotificationRule> => {
  const payload = {
    listId,
    trigger: input.trigger,
    channels: input.channels,
    schedule: input.schedule,
    templateId: input.templateId,
  };

  const response = await fetch(
    "/api/notifications/rules",
    buildRequestInit(payload, "POST"),
  );

  if (!response.ok) {
    throw new Error("Failed to create notification rule");
  }

  const result: NotificationRule = await response.json();
  return result;
};

export const updateNotificationRule = async (
  ruleId: string,
  input: NotificationRuleInput,
): Promise<void> => {
  const payload = {
    trigger: input.trigger,
    channels: input.channels,
    schedule: input.schedule,
    templateId: input.templateId,
  };

  const response = await fetch(
    `/api/notifications/rules/${ruleId}`,
    buildRequestInit(payload, "PUT"),
  );

  if (!response.ok) {
    throw new Error("Failed to update notification rule");
  }
};

export const deleteNotificationRule = async (ruleId: string): Promise<void> => {
  const response = await fetch(`/api/notifications/rules/${ruleId}`, {
    method: "DELETE",
  });

  if (!response.ok) {
    throw new Error("Failed to delete notification rule");
  }
};
