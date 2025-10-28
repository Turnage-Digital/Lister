import * as React from "react";

import {
  useMutation,
  useQueryClient,
  useSuspenseQuery,
} from "@tanstack/react-query";
import { useNavigate, useParams } from "react-router-dom";

import {
  EditorPageLayout,
  ListEditor,
  type ListEditorInitialValue,
  type ListEditorSubmitResult,
  type NotificationRuleSubmission,
  Titlebar,
  toNotificationRuleFormValue,
} from "../components";
import {
  listItemDefinitionQueryOptions,
  notificationRulesQueryOptions,
} from "../query-options";

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
        const message = await response.text();
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

  const handleNavigateToLists = () => {
    navigate("/");
  };

  const handleNavigateToList = () => {
    navigate(`/${listId}?page=0&pageSize=10`);
  };

  const handleSubmit = async (result: ListEditorSubmitResult) => {
    await updateListMutation.mutateAsync(result);
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
    <EditorPageLayout>
      <Titlebar title={`Edit ${definition.name}`} breadcrumbs={breadcrumbs} />
      <ListEditor
        key={listId}
        disableNameField
        initialValue={initialValue}
        onSubmit={handleSubmit}
        onCancel={handleCancel}
        isSubmitting={updateListMutation.isPending}
      />
    </EditorPageLayout>
  );
};

export default EditListPage;
