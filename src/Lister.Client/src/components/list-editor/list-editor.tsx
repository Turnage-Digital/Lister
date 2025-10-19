import * as React from "react";

import { Save, Undo } from "@mui/icons-material";
import { Box, Button, Divider, Stack } from "@mui/material";

import {
  Column,
  ListItemDefinition,
  NotificationRule,
  Status,
  StatusTransition,
} from "../../models";
import EditListColumnsContent from "../edit-list-columns-content";
import EditListNameContent from "../edit-list-name-content";
import EditListStatusesContent from "../edit-list-statuses-content";
import FormBlock from "../form-block";
import NotificationRulesEditor from "./notification-rules-editor";
import StatusTransitionsEditor from "./status-transitions-editor";

import type {
  ListEditorInitialValue,
  ListEditorSubmitResult,
  NotificationRuleFormValue,
  NotificationRuleSubmission,
} from "./types";

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

interface Props {
  initialValue: ListEditorInitialValue;
  onSubmit: (result: ListEditorSubmitResult) => Promise<void> | void;
  isSubmitting?: boolean;
  onCancel?: () => void;
  disableNameField?: boolean;
}

interface InternalState {
  id?: string | null;
  name: string;
  columns: Column[];
  statuses: Status[];
  transitions: StatusTransition[];
  notificationRules: NotificationRuleFormValue[];
}

const sanitizeTransitions = (
  statuses: Status[],
  transitions: StatusTransition[],
): StatusTransition[] => {
  const allowedNames = new Set(statuses.map((status) => status.name));
  return transitions
    .filter((transition) => allowedNames.has(transition.from))
    .map((transition) => ({
      from: transition.from,
      allowedNext: transition.allowedNext.filter((name) =>
        allowedNames.has(name),
      ),
    }))
    .filter((transition) => transition.allowedNext.length > 0);
};

const stripClientFields = (
  rule: NotificationRuleFormValue,
): NotificationRuleSubmission => ({
  id: rule.id,
  listId: rule.listId,
  trigger: rule.trigger,
  channels: rule.channels,
  schedule: rule.schedule,
  templateId: rule.templateId,
});

export const toNotificationRuleFormValue = (
  rule: NotificationRule,
): NotificationRuleFormValue => ({
  id: rule.id ?? undefined,
  listId: rule.listId,
  trigger: rule.trigger,
  channels: rule.channels,
  schedule: rule.schedule,
  templateId: rule.templateId ?? undefined,
  clientId: createClientId(),
});

export const createEmptyRuleFormValue = (): NotificationRuleFormValue => ({
  clientId: createClientId(),
  trigger: { type: "ItemCreated" },
  channels: [{ type: "InApp" }],
  schedule: { type: "Immediate" },
});

const ListEditor = ({
  initialValue,
  onSubmit,
  isSubmitting,
  onCancel,
  disableNameField = false,
}: Props) => {
  const [state, setState] = React.useState<InternalState>(() => ({
    id: initialValue.id ?? null,
    name: initialValue.name,
    columns: [...initialValue.columns],
    statuses: [...initialValue.statuses],
    transitions: sanitizeTransitions(
      initialValue.statuses,
      initialValue.transitions,
    ),
    notificationRules: initialValue.notificationRules.map((rule) => ({
      ...rule,
    })),
  }));

  const [deletedRuleIds, setDeletedRuleIds] = React.useState<string[]>([]);
  const [error, setError] = React.useState<string | null>(null);

  const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    const trimmedName = state.name.trim();
    if (!trimmedName) {
      setError("List name is required.");
      return;
    }

    const sanitizedTransitions = sanitizeTransitions(
      state.statuses,
      state.transitions,
    );

    const definition: ListItemDefinition = {
      id: state.id ?? null,
      name: trimmedName,
      columns: state.columns,
      statuses: state.statuses,
      transitions: sanitizedTransitions,
    };

    const result: ListEditorSubmitResult = {
      definition,
      notificationRules: {
        upserts: state.notificationRules.map(stripClientFields),
        deletes: deletedRuleIds,
      },
    };

    setError(null);
    await onSubmit(result);
  };

  const handleNameChange = (name: string) => {
    setState((prev) => ({ ...prev, name }));
  };

  const handleColumnsChange = (columns: Column[]) => {
    setState((prev) => ({ ...prev, columns }));
  };

  const handleStatusesChange = (statuses: Status[]) => {
    setState((prev) => ({
      ...prev,
      statuses,
      transitions: sanitizeTransitions(statuses, prev.transitions),
    }));
  };

  const handleTransitionsChange = (transitions: StatusTransition[]) => {
    setState((prev) => ({ ...prev, transitions }));
  };

  const handleAddRule = () => {
    setState((prev) => ({
      ...prev,
      notificationRules: [
        ...prev.notificationRules,
        createEmptyRuleFormValue(),
      ],
    }));
  };

  const handleUpdateRule = (
    clientId: string,
    updater: (rule: NotificationRuleFormValue) => NotificationRuleFormValue,
  ) => {
    setState((prev) => ({
      ...prev,
      notificationRules: prev.notificationRules.map((rule) =>
        rule.clientId === clientId ? updater(rule) : rule,
      ),
    }));
  };

  const handleRemoveRule = (rule: NotificationRuleFormValue) => {
    setState((prev) => ({
      ...prev,
      notificationRules: prev.notificationRules.filter(
        (existing) => existing.clientId !== rule.clientId,
      ),
    }));
    if (rule.id) {
      const id = rule.id;
      setDeletedRuleIds((prev) => (prev.includes(id) ? prev : [...prev, id]));
    }
  };

  const errorBanner = error ? (
    <Box sx={{ mr: "auto", color: "error.main" }}>{error}</Box>
  ) : null;

  return (
    <Stack
      component="form"
      onSubmit={handleSubmit}
      divider={<Divider sx={{ my: { xs: 5, md: 6 } }} />}
      spacing={{ xs: 6, md: 7 }}
    >
      <FormBlock
        title="Name"
        subtitle="Give this list a clear, human-friendly name so everyone knows what lives here."
        content={
          <EditListNameContent
            name={state.name}
            onNameChanged={handleNameChange}
            disabled={disableNameField}
          />
        }
      />

      <FormBlock
        title="Columns"
        subtitle="Define the fields each item should capture — text, numbers, dates, or yes/no toggles."
        content={
          <EditListColumnsContent
            columns={state.columns}
            onColumnsChanged={handleColumnsChange}
          />
        }
      />

      <FormBlock
        title="Statuses"
        subtitle="List the steps in your workflow so items move from start to finish in a predictable way."
        content={
          <EditListStatusesContent
            statuses={state.statuses}
            onStatusesChanged={handleStatusesChange}
          />
        }
      />

      <FormBlock
        title="Status transitions"
        subtitle="Choose which status changes are allowed to keep items flowing through the process safely."
        content={
          <StatusTransitionsEditor
            statuses={state.statuses}
            transitions={state.transitions}
            onChange={handleTransitionsChange}
          />
        }
      />

      <FormBlock
        title="Notification rules"
        subtitle="Send alerts to teammates when important list or item changes occur."
        content={
          <NotificationRulesEditor
            rules={state.notificationRules}
            statuses={state.statuses}
            onAddRule={handleAddRule}
            onUpdateRule={handleUpdateRule}
            onRemoveRule={handleRemoveRule}
          />
        }
      />

      <Box
        sx={{
          display: "flex",
          flexWrap: "wrap",
          gap: 2,
          justifyContent: { xs: "center", md: "flex-end" },
          alignItems: "center",
        }}
      >
        {errorBanner}
        {onCancel && (
          <Button
            type="button"
            onClick={onCancel}
            startIcon={<Undo />}
            variant="text"
          >
            Cancel
          </Button>
        )}
        <Button
          type="submit"
          variant="contained"
          startIcon={<Save />}
          disabled={isSubmitting}
        >
          Submit
        </Button>
      </Box>
    </Stack>
  );
};

export default ListEditor;
