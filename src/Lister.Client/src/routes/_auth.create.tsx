import { Save } from "@mui/icons-material";
import { Box, Button, Divider, Stack } from "@mui/material";
import { useMutation } from "@tanstack/react-query";
import { createFileRoute } from "@tanstack/react-router";
import React, { FormEvent, useEffect, useState } from "react";

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

  const defaultListDefinition: ListItemDefinition = {
    id: null,
    name: "",
    columns: [],
    statuses: [],
  };

  const [updated, setUpdated] = useState<ListItemDefinition>(() => {
    const item = window.sessionStorage.getItem("updated_list");
    return item ? JSON.parse(item) : defaultListDefinition;
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
    window.sessionStorage.removeItem("updated_list");
    navigate({ to: `/${mutated.id}` });
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
      spacing={4}
      divider={<Divider />}
      onSubmit={handleSubmit}
      sx={{ px: 2, py: 4 }}
    >
      <Titlebar title="Create a List" breadcrumbs={breadcrumbs} />

      <FormBlock
        title="Name"
        blurb="Blurb about naming a list."
        content={
          <EditListNameContent
            name={updated.name}
            onNameChanged={(name) => update("name", name)}
          />
        }
      />

      <FormBlock
        title="Columns"
        blurb="Blurb about columns for a list."
        content={
          <EditListColumnsContent
            columns={updated.columns}
            onColumnsChanged={(columns) => update("columns", columns)}
          />
        }
      />

      <FormBlock
        title="Statuses"
        blurb="Blurb about statuses for an item."
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
        <Button
          type="submit"
          variant="contained"
          startIcon={<Save />}
          sx={{ width: { xs: "100%", md: "auto" } }}
        >
          Submit
        </Button>
      </Box>
    </Stack>
  );
};

export const Route = createFileRoute("/_auth/create")({
  component: RouteComponent,
});
