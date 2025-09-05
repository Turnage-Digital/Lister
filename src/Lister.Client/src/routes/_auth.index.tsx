import * as React from "react";

import { PlaylistAdd } from "@mui/icons-material";
import { Grid, Stack } from "@mui/material";
import { useMutation, useSuspenseQuery } from "@tanstack/react-query";
import { createFileRoute } from "@tanstack/react-router";

import { ListCard, Titlebar } from "../components";
import { listNamesQueryOptions } from "../query-options";

const RouteComponent = () => {
  const navigate = Route.useNavigate();
  const { queryClient } = Route.useRouteContext();

  const deleteListMutation = useMutation({
    mutationFn: async (listId: string) => {
      const request = new Request(`/api/lists/${listId}`, {
        method: "DELETE",
      });
      await fetch(request);
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries();
    },
  });

  const handleDeleteClicked = async (listId: string) => {
    await deleteListMutation.mutateAsync(listId);
  };

  const listNamesQuery = useSuspenseQuery(listNamesQueryOptions());
  if (!listNamesQuery.isSuccess) {
    return null;
  }

  const actions = [
    {
      title: "Create a List",
      icon: <PlaylistAdd />,
      onClick: () => navigate({ to: "/create" }),
    },
  ];

  return (
    <Stack sx={{ px: 2, py: 4 }} spacing={4}>
      <Titlebar title="Lists" actions={actions} />

      <Grid container spacing={3}>
        {listNamesQuery.data.map((listName) => (
          <Grid key={listName.id} size={{ xs: 12, sm: 6, md: 4 }}>
            <ListCard
              listName={listName}
              onDeleteClick={() => handleDeleteClicked(listName.id)}
            />
          </Grid>
        ))}
      </Grid>
    </Stack>
  );
};

export const Route = createFileRoute("/_auth/")({
  component: RouteComponent,
  loader: (options) =>
    options.context.queryClient.ensureQueryData(listNamesQueryOptions()),
});
