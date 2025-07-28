import * as React from "react";

import { Grid, Stack } from "@mui/material";
import { useSuspenseQuery } from "@tanstack/react-query";
import { createFileRoute } from "@tanstack/react-router";

import { ItemCard, Titlebar } from "../components";
import {
  itemQueryOptions,
  listItemDefinitionQueryOptions,
} from "../query-options";

const RouteComponent = () => {
  const { listId, itemId } = Route.useParams();
  const navigate = Route.useNavigate();

  const listItemDefinitionQuery = useSuspenseQuery(
    listItemDefinitionQueryOptions(listId),
  );

  const itemQuery = useSuspenseQuery(itemQueryOptions(listId, itemId));

  if (!listItemDefinitionQuery.isSuccess || !itemQuery.isSuccess) {
    return null;
  }

  const breadcrumbs = [
    {
      title: "Lists",
      onClick: () => navigate({ to: "/" }),
    },
    {
      title: listItemDefinitionQuery.data.name || "",
      onClick: () => navigate({ to: `/${listId}` }),
    },
  ];

  return (
    <Stack sx={{ px: 2, py: 4 }} spacing={4}>
      <Titlebar title={`ID ${itemQuery.data.id}`} breadcrumbs={breadcrumbs} />

      <Grid container spacing={2}>
        <Grid size={{ xs: 12, md: 6 }}>
          <ItemCard
            item={itemQuery.data}
            definition={listItemDefinitionQuery.data}
          />
        </Grid>
      </Grid>
    </Stack>
  );
};

export const Route = createFileRoute("/_auth/$listId/$itemId/")({
  component: RouteComponent,
});
