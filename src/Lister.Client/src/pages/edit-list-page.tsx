import * as React from "react";

import { Stack } from "@mui/material";
import {
  useMutation,
  useQueryClient,
  useSuspenseQuery,
} from "@tanstack/react-query";
import { useNavigate, useParams } from "react-router-dom";

import {
  createNotificationRule,
  deleteNotificationRule,
  updateNotificationRule,
} from "../api/notification-rules";
import {
  ListEditor,
  Titlebar,
  toNotificationRuleFormValue,
  type ListEditorInitialValue,
  type ListEditorSubmitResult,
  type NotificationRuleSubmission,
} from "../components";
import {
  listItemDefinitionQueryOptions,
  notificationRulesQueryOptions,
} from "../query-options";

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
              templateId: rule.templateId,
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
              templateId: rule.templateId,
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
      navigate(`/${listId}?page=0&pageSize=10`);
    },
  });

  const breadcrumbs = React.useMemo(
    () => [
      {
        title: "Lists",
        onClick: () => navigate("/"),
      },
      {
        title: listDefinitionQuery.data.name,
        onClick: () => navigate(`/${listId}?page=0&pageSize=10`),
      },
    ],
    [listDefinitionQuery.data.name, listId, navigate],
  );

  const handleSubmit = async (result: ListEditorSubmitResult) => {
    await updateListMutation.mutateAsync(result);
  };

  const handleCancel = () => {
    navigate(-1);
  };

  return (
    <Stack sx={{ px: 2, py: 4 }} spacing={4}>
      <Titlebar
        title={`Edit ${listDefinitionQuery.data.name}`}
        breadcrumbs={breadcrumbs}
      />
      <ListEditor
        key={listId}
        mode="edit"
        disableNameField
        initialValue={initialValue}
        onSubmit={handleSubmit}
        onCancel={handleCancel}
        isSubmitting={updateListMutation.isPending}
        submitLabel="Save changes"
      />
    </Stack>
  );
};

export default EditListPage;
