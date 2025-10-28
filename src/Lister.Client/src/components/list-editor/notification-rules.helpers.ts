import { NotificationRule } from "../../models";

import type {
  NotificationRuleFormValue,
  NotificationRuleSubmission,
} from "./list-editor.types";

const createClientId = () => {
  const cryptoApi = globalThis.crypto;
  const hasRandomUuid =
    typeof cryptoApi !== "undefined" &&
    typeof cryptoApi.randomUUID === "function";

  if (hasRandomUuid) {
    return cryptoApi.randomUUID();
  }
  return Math.random().toString(36).slice(2);
};

export const stripClientFields = (
  rule: NotificationRuleFormValue,
): NotificationRuleSubmission => ({
  id: rule.id,
  listId: rule.listId,
  trigger: rule.trigger,
  channels: rule.channels,
  schedule: rule.schedule,
  isActive: rule.isActive,
});

export const toNotificationRuleFormValue = (
  rule: NotificationRule,
): NotificationRuleFormValue => ({
  id: rule.id ?? undefined,
  listId: rule.listId,
  trigger: rule.trigger,
  channels: rule.channels,
  schedule: rule.schedule,
  isActive: rule.isActive ?? true,
  clientId: createClientId(),
});

export const createEmptyRuleFormValue = (): NotificationRuleFormValue => ({
  clientId: createClientId(),
  trigger: { type: "ItemCreated" },
  channels: [],
  schedule: { type: "Immediate" },
  isActive: true,
});
