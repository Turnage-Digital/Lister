import React from "react";
import { Grid2, Stack } from "@mui/material";
import { PlaylistAdd } from "@mui/icons-material";
import { createFileRoute } from "@tanstack/react-router";
import { useSuspenseQuery } from "@tanstack/react-query";

import { listNamesQueryOptions } from "../query-options";
import { ListCard, Titlebar } from "../components";

const RouteComponent = () => {
  const navigate = Route.useNavigate();

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
            onViewClick={() =>
              navigate({
                to: "/$listId",
                params: { listId: listName.id },
                search: { page: 0, pageSize: 10 },
              })
            }
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
