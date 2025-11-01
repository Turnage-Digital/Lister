import * as React from "react";

import {
  useMutation,
  useQuery,
  useQueryClient,
  useSuspenseQuery,
} from "@tanstack/react-query";
import { useNavigate, useParams } from "react-router-dom";

import {
  ListEditor,
  type ListEditorInitialValue,
  type ListEditorSubmitResult,
  ListMigrationRequiredError,
  type NotificationRuleSubmission,
  Titlebar,
  toNotificationRuleFormValue,
} from "../components";
import {
  listItemDefinitionQueryOptions,
  migrationProgressQueryOptions,
  notificationRulesQueryOptions,
} from "../query-options";

import type { MigrationPlan, MigrationProgressRecord } from "../models";

type NotificationRuleMutationInput = Pick<
  NotificationRuleSubmission,
  "trigger" | "channels" | "schedule" | "isActive"
>;

const buildNotificationRulePayload = (
  input: NotificationRuleMutationInput,
) => ({
  trigger: input.trigger,
  channels: input.channels,
  schedule: input.schedule,
  isActive: input.isActive,
});

const createNotificationRule = async (
  listId: string,
  input: NotificationRuleMutationInput,
) => {
  const payload = {
    listId,
    ...buildNotificationRulePayload(input),
  };

  const response = await fetch("/api/notifications/rules", {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(payload),
  });

  if (!response.ok) {
    const message = await response
      .text()
      .catch(() => "Failed to create notification rule");
    throw new Error(message);
  }

  await response.json();
};

const updateNotificationRule = async (
  ruleId: string,
  input: NotificationRuleMutationInput,
) => {
  const response = await fetch(`/api/notifications/rules/${ruleId}`, {
    method: "PUT",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(buildNotificationRulePayload(input)),
  });

  if (!response.ok) {
    const message = await response
      .text()
      .catch(() => "Failed to update notification rule");
    throw new Error(message);
  }
};

const deleteNotificationRule = async (ruleId: string) => {
  const response = await fetch(`/api/notifications/rules/${ruleId}`, {
    method: "DELETE",
  });

  if (!response.ok) {
    const message = await response
      .text()
      .catch(() => "Failed to delete notification rule");
    throw new Error(message);
  }
};

const EditListPage = () => {
  const { listId } = useParams<{ listId: string }>();
  if (!listId) {
    throw new Error("List id is required");
  }

  const navigate = useNavigate();
  const queryClient = useQueryClient();
  const [activeMigration, setActiveMigration] = React.useState<{
    correlationId: string;
  } | null>(null);

  const migrationProgressQuery = useQuery(
    migrationProgressQueryOptions(listId, activeMigration?.correlationId ?? ""),
  );
  const migrationProgress = migrationProgressQuery.data ?? null;

  const migrationStatus = React.useMemo(() => {
    if (!migrationProgress) {
      return null;
    }

    const stage = migrationProgress.stage;
    let percent = migrationProgress.percent;
    if (typeof percent !== "number") {
      if (stage === "Completed" || stage === "Archived") {
        percent = 100;
      } else if (stage === "Failed") {
        percent = 0;
      }
    }

    let message = migrationProgress.lastMessage;
    if (stage === "Completed") {
      if (typeof migrationProgress.itemsProcessed === "number") {
        message = `Migration completed — ${migrationProgress.itemsProcessed} item${
          migrationProgress.itemsProcessed === 1 ? "" : "s"
        } processed.`;
      } else {
        message = message ?? "Migration completed.";
      }
    } else if (stage === "Failed") {
      message = migrationProgress.lastError ?? message ?? "Migration failed.";
    } else if (stage === "Archived") {
      message = message ?? "Backup removed after retention window.";
    }

    if (!message) {
      if (stage === "Running") {
        message = "Migration in progress.";
      } else if (stage === "Pending") {
        message = "Migration queued.";
      } else {
        message = `Migration ${stage.toLowerCase()}.`;
      }
    }

    return {
      stage,
      message,
      percent,
    };
  }, [migrationProgress]);

  const listDefinitionQuery = useSuspenseQuery(
    listItemDefinitionQueryOptions(listId),
  );

  const notificationRulesQuery = useSuspenseQuery(
    notificationRulesQueryOptions(listId),
  );

  const definition = listDefinitionQuery.data;

  const initialValue = React.useMemo<ListEditorInitialValue>(() => {
    const definition = listDefinitionQuery.data;
    const rules = notificationRulesQuery.data.map(toNotificationRuleFormValue);
    const transitions = Array.isArray(definition.transitions)
      ? definition.transitions
      : [];
    const definitionId = definition.id ?? listId;

    return {
      id: definitionId,
      name: definition.name,
      columns: definition.columns,
      statuses: definition.statuses,
      transitions,
      notificationRules: rules,
    };
  }, [listDefinitionQuery.data, notificationRulesQuery.data, listId]);

  const updateListMutation = useMutation({
    mutationFn: async (result: ListEditorSubmitResult) => {
      const { definition, notificationRules } = result;
      const updatePayload = {
        columns: definition.columns,
        statuses: definition.statuses,
        transitions: definition.transitions,
      };

      const response = await fetch(`/api/lists/${listId}`, {
        method: "PUT",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(updatePayload),
      });

      if (!response.ok) {
        const clone = response.clone();
        const contentType = response.headers.get("content-type") ?? "";

        let parsedBody: unknown = null;
        if (contentType.includes("application/json")) {
          parsedBody = await response.json().catch(() => null);
        }

        let message: string | undefined;
        let reasons: string[] | undefined;
        let plan: MigrationPlan | undefined;

        if (parsedBody && typeof parsedBody === "object") {
          const bodyRecord = parsedBody as Record<string, unknown>;
          if (
            typeof bodyRecord.message === "string" &&
            bodyRecord.message.length > 0
          ) {
            message = bodyRecord.message;
          }

          if (Array.isArray(bodyRecord.reasons)) {
            const filtered = bodyRecord.reasons.filter(
              (reason): reason is string =>
                typeof reason === "string" && reason.trim().length > 0,
            );
            if (filtered.length > 0) {
              reasons = filtered;
            }
          }

          if (bodyRecord.plan && typeof bodyRecord.plan === "object") {
            plan = bodyRecord.plan as MigrationPlan;
          }
        }

        if (!message) {
          const text = await clone.text().catch(() => "");
          if (text && text.length > 0) {
            message = text;
          }
        }

        if (response.status === 409 && reasons && reasons.length > 0) {
          throw new ListMigrationRequiredError(
            message ?? "This update requires a list migration.",
            reasons,
            plan,
          );
        }

        throw new Error(
          message || "Failed to update list. Check for validation errors.",
        );
      }

      const upserts = notificationRules.upserts.filter(
        (rule) => rule.channels.length > 0,
      );
      const creates = upserts.filter((rule) => !rule.id);
      const updates = upserts.filter(
        (rule): rule is NotificationRuleSubmission & { id: string } =>
          typeof rule.id === "string" && rule.id.length > 0,
      );

      if (creates.length > 0) {
        await Promise.all(
          creates.map((rule) =>
            createNotificationRule(listId, {
              trigger: rule.trigger,
              channels: rule.channels,
              schedule: rule.schedule,
              isActive: rule.isActive,
            }),
          ),
        );
      }

      if (updates.length > 0) {
        await Promise.all(
          updates.map((rule) =>
            updateNotificationRule(rule.id, {
              trigger: rule.trigger,
              channels: rule.channels,
              schedule: rule.schedule,
              isActive: rule.isActive,
            }),
          ),
        );
      }

      if (notificationRules.deletes.length > 0) {
        await Promise.all(
          notificationRules.deletes.map((ruleId) =>
            deleteNotificationRule(ruleId),
          ),
        );
      }
    },
    onSuccess: async () => {
      setActiveMigration(null);
      await Promise.all([
        queryClient.invalidateQueries({
          queryKey: ["list-definition", listId],
        }),
        queryClient.invalidateQueries({
          queryKey: ["notification-rules", listId],
        }),
        queryClient.invalidateQueries({
          queryKey: ["list-names"],
        }),
      ]);
      handleNavigateToList();
    },
  });

  interface MigrationRequestResponse {
    messages?: string[];
    correlationId?: string;
  }

  const requestMigrationMutation = useMutation<
    MigrationRequestResponse,
    Error,
    MigrationPlan
  >({
    mutationFn: async (plan: MigrationPlan) => {
      const response = await fetch(`/api/lists/${listId}/migrations`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({ plan, mode: "execute" }),
      });

      if (!response.ok) {
        const message = await response
          .text()
          .catch(() => "Failed to queue migration.");
        throw new Error(message);
      }

      return (await response.json()) as MigrationRequestResponse;
    },
    onSuccess: async (result) => {
      const messages = Array.isArray(result.messages) ? result.messages : [];
      const message =
        messages.length > 0 ? messages[0] : "Migration queued successfully.";
      const correlationId =
        typeof result.correlationId === "string"
          ? result.correlationId
          : undefined;

      if (correlationId) {
        setActiveMigration({ correlationId });
        queryClient.setQueryData<MigrationProgressRecord | null>(
          ["list-migration-progress", listId, correlationId],
          (previous) => {
            const base: MigrationProgressRecord = previous ?? {
              listId,
              correlationId,
              stage: "Pending",
              createdOn: new Date().toISOString(),
              updatedAt: new Date().toISOString(),
            };

            return {
              ...base,
              stage: "Pending",
              lastMessage: message,
              percent: 0,
              updatedAt: new Date().toISOString(),
            };
          },
        );
      }

      await Promise.all([
        queryClient.invalidateQueries({
          queryKey: ["list-definition", listId],
        }),
        queryClient.invalidateQueries({
          queryKey: ["notification-rules", listId],
        }),
      ]);
    },
    onError: () => {
      setActiveMigration(null);
    },
  });

  const handleNavigateToLists = () => {
    navigate("/");
  };

  const handleNavigateToList = () => {
    navigate(`/${listId}?page=0&pageSize=10`);
  };

  const handleSubmit = async (result: ListEditorSubmitResult) => {
    setActiveMigration(null);
    await updateListMutation.mutateAsync(result);
  };

  const handleRequestMigration = async (plan: MigrationPlan) => {
    await requestMigrationMutation.mutateAsync(plan);
  };

  const handleCancel = () => {
    navigate(-1);
  };

  const breadcrumbs = [
    {
      title: "Lists",
      onClick: handleNavigateToLists,
    },
    {
      title: definition.name,
      onClick: handleNavigateToList,
    },
  ];

  return (
    <>
      <Titlebar title={`Edit ${definition.name}`} breadcrumbs={breadcrumbs} />
      <ListEditor
        key={listId}
        disableNameField
        initialValue={initialValue}
        onSubmit={handleSubmit}
        onCancel={handleCancel}
        isSubmitting={updateListMutation.isPending}
        isMigrationPending={requestMigrationMutation.isPending}
        migrationStatus={migrationStatus}
        onRequestMigration={handleRequestMigration}
      />
    </>
  );
};

export default EditListPage;
