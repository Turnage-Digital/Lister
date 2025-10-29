import {
  Column,
  ListItemDefinition,
  MigrationPlan,
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

export class ListMigrationRequiredError extends Error {
  public readonly reasons: string[];
  public readonly plan?: MigrationPlan;

  constructor(message: string, reasons: string[], plan?: MigrationPlan) {
    super(message);
    this.name = "ListMigrationRequiredError";
    this.reasons = reasons;
    this.plan = plan;
  }
}
