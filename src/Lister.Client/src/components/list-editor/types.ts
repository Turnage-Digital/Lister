import {
  Column,
  ListItemDefinition,
  NotificationRuleInput,
  Status,
  StatusTransition,
} from "../../models";

export interface NotificationRuleFormValue extends NotificationRuleInput {
  id?: string;
  listId?: string;
  clientId: string;
}

export interface NotificationRuleSubmission extends NotificationRuleInput {
  id?: string;
  listId?: string;
}

export interface ListEditorInitialValue {
  id?: string | null;
  name: string;
  columns: Column[];
  statuses: Status[];
  transitions: StatusTransition[];
  notificationRules: NotificationRuleFormValue[];
}

export interface ListEditorSubmitResult {
  definition: ListItemDefinition;
  notificationRules: {
    upserts: NotificationRuleSubmission[];
    deletes: string[];
  };
}
