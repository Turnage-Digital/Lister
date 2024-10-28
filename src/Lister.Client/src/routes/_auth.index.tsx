import { PlaylistAdd } from "@mui/icons-material";
import { Grid2, Stack } from "@mui/material";
import { useMutation, useSuspenseQuery } from "@tanstack/react-query";
import { createFileRoute } from "@tanstack/react-router";
import React from "react";

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

  const handleViewClicked = (listId: string) => {
    navigate({
      to: "/$listId",
      params: { listId },
      search: { page: 0, pageSize: 10 },
    });
  };

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
    <Stack sx={{ px: 2, py: 4 }}>
      <Titlebar title="Lists" actions={actions} />

      <Grid2 container spacing={2} sx={{ my: 2 }}>
        {listNamesQuery.data.map((listName) => (
          <ListCard
            key={listName.id}
            listName={listName}
            onViewClick={() => handleViewClicked(listName.id)}
            onDeleteClick={() => handleDeleteClicked(listName.id)}
          />
        ))}
      </Grid2>
    </Stack>
  );
};

export const Route = createFileRoute("/_auth/")({
  component: RouteComponent,
  loader: (options) =>
    options.context.queryClient.ensureQueryData(listNamesQueryOptions()),
});
