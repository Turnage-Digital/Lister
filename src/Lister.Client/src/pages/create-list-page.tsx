import * as React from "react";

import { Stack } from "@mui/material";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { createSearchParams, useNavigate } from "react-router-dom";

import {
  ListEditor,
  type ListEditorInitialValue,
  type ListEditorSubmitResult,
  Titlebar,
} from "../components";
import { ListItemDefinition, NotificationRuleInput } from "../models";

const initialValue: ListEditorInitialValue = {
  id: null,
  name: "",
  columns: [],
  statuses: [],
  transitions: [],
  notificationRules: [],
};

const createNotificationRule = async (
  listId: string,
  input: NotificationRuleInput,
) => {
  const payload = {
    listId,
    trigger: input.trigger,
    channels: input.channels,
    schedule: input.schedule,
    templateId: input.templateId,
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

const CreateListPage = () => {
  const navigate = useNavigate();
  const queryClient = useQueryClient();

  const createListMutation = useMutation({
    mutationFn: async (result: ListEditorSubmitResult) => {
      const { definition, notificationRules } = result;
      const request = new Request("/api/lists", {
        headers: {
          "Content-Type": "application/json",
        },
        method: "POST",
        body: JSON.stringify({
          name: definition.name,
          columns: definition.columns,
          statuses: definition.statuses,
          transitions: definition.transitions,
        }),
      });
      const response = await fetch(request);
      if (!response.ok) {
        throw new Error("Failed to create list");
      }

      const created: ListItemDefinition = await response.json();
      if (!created.id) {
        throw new Error("List was created without an identifier");
      }

      if (notificationRules.upserts.length > 0) {
        await Promise.all(
          notificationRules.upserts
            .filter((rule) => rule.channels.length > 0)
            .map((rule) =>
              createNotificationRule(created.id as string, {
                trigger: rule.trigger,
                channels: rule.channels,
                schedule: rule.schedule,
                templateId: rule.templateId,
              }),
            ),
        );
      }

      return created;
    },
    onSuccess: async (created) => {
      await queryClient.invalidateQueries();
      const search = createSearchParams({
        page: "0",
        pageSize: "10",
      }).toString();
      navigate(`/${created.id}?${search}`);
    },
  });

  const breadcrumbs = React.useMemo(
    () => [
      {
        title: "Lists",
        onClick: () => navigate("/"),
      },
    ],
    [navigate],
  );

  const handleSubmit = async (result: ListEditorSubmitResult) => {
    await createListMutation.mutateAsync(result);
  };

  return (
    <Stack
      sx={{
        maxWidth: 1180,
        width: "100%",
        mx: "auto",
        px: { xs: 3, md: 7 },
        py: { xs: 4, md: 6 },
      }}
      spacing={{ xs: 6, md: 7 }}
    >
      <Titlebar title="Create a List" breadcrumbs={breadcrumbs} />
      <ListEditor
        mode="create"
        initialValue={initialValue}
        onSubmit={handleSubmit}
        isSubmitting={createListMutation.isPending}
      />
    </Stack>
  );
};

export default CreateListPage;
