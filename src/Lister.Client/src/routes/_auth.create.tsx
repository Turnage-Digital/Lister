import * as React from "react";
import { FormEvent, useEffect, useState } from "react";

import { Save } from "@mui/icons-material";
import { LoadingButton } from "@mui/lab";
import { Box, Divider, Stack } from "@mui/material";
import { useMutation } from "@tanstack/react-query";
import { createFileRoute } from "@tanstack/react-router";

import {
  EditListColumnsContent,
  EditListNameContent,
  EditListStatusesContent,
  FormBlock,
  Titlebar,
} from "../components";
import { ListItemDefinition } from "../models";

const RouteComponent = () => {
  const navigate = Route.useNavigate();
  const { queryClient } = Route.useRouteContext();

  const createListMutation = useMutation({
    mutationFn: async (list: ListItemDefinition) => {
      const request = new Request("/api/lists", {
        headers: {
          "Content-Type": "application/json",
        },
        method: "POST",
        body: JSON.stringify(list),
      });
      const response = await fetch(request);
      const retval: ListItemDefinition = await response.json();
      return retval;
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries();
    },
  });

  const defaultListItemDefinition: ListItemDefinition = {
    id: null,
    name: "",
    columns: [],
    statuses: [],
  };

  const [updated, setUpdated] = useState<ListItemDefinition>(() => {
    const item = window.sessionStorage.getItem("updated_list");
    return item ? JSON.parse(item) : defaultListItemDefinition;
  });

  useEffect(() => {
    window.sessionStorage.setItem("updated_list", JSON.stringify(updated));
  }, [updated]);

  const update = (key: keyof ListItemDefinition, value: any) => {
    setUpdated((prev) => ({ ...prev, [key]: value }));
  };

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();

    const mutated = await createListMutation.mutateAsync(updated);
    if (mutated.id === null) {
      throw new Error("List was not created.");
    }

    window.sessionStorage.removeItem("updated_list");
    await navigate({
      to: "/$listId",
      params: { listId: mutated.id },
      search: { page: 0, pageSize: 10 },
    });
  };

  const breadcrumbs = [
    {
      title: "Lists",
      onClick: () => navigate({ to: "/" }),
    },
  ];

  return (
    <Stack
      component="form"
      divider={<Divider />}
      onSubmit={handleSubmit}
      sx={{ px: 2, py: 4 }}
      spacing={4}
    >
      <Titlebar title="Create a List" breadcrumbs={breadcrumbs} />

      <FormBlock
        title="Name"
        content={
          <EditListNameContent
            name={updated.name}
            onNameChanged={(name) => update("name", name)}
          />
        }
      />

      <FormBlock
        title="Columns"
        content={
          <EditListColumnsContent
            columns={updated.columns}
            onColumnsChanged={(columns) => update("columns", columns)}
          />
        }
      />

      <FormBlock
        title="Statuses"
        content={
          <EditListStatusesContent
            statuses={updated.statuses}
            onStatusesChanged={(statuses) => update("statuses", statuses)}
          />
        }
      />

      <Box
        sx={{
          display: "flex",
          justifyContent: { xs: "center", md: "flex-end" },
        }}
      >
        <LoadingButton
          type="submit"
          variant="contained"
          startIcon={<Save />}
          loading={createListMutation.isPending}
          sx={{ width: { xs: "100%", md: "auto" } }}
        >
          Submit
        </LoadingButton>
      </Box>
    </Stack>
  );
};

export const Route = createFileRoute("/_auth/create")({
  component: RouteComponent,
});
