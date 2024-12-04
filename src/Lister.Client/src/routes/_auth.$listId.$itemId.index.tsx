import * as React from "react";

import { Stack } from "@mui/material";
import { useSuspenseQuery } from "@tanstack/react-query";
import { createFileRoute } from "@tanstack/react-router";

import { Titlebar } from "../components";
import { itemQueryOptions, listDefinitionQueryOptions } from "../query-options";

const RouteComponent = () => {
  const { listId, itemId } = Route.useParams();
  const navigate = Route.useNavigate();

  const listDefinitionQuery = useSuspenseQuery(
    listDefinitionQueryOptions(listId),
  );

  const itemQuery = useSuspenseQuery(itemQueryOptions(listId, itemId));

  if (!listDefinitionQuery.isSuccess || !itemQuery.isSuccess) {
    return null;
  }

  const breadcrumbs = [
    {
      title: "Lists",
      onClick: () => navigate({ to: "/" }),
    },
    {
      title: listDefinitionQuery.data.name || "",
      onClick: () => navigate({ to: `/${listId}` }),
    },
  ];

  return (
    <Stack sx={{ px: 2, py: 4 }}>
      <Titlebar title={`ID ${itemQuery.data.id}`} breadcrumbs={breadcrumbs} />
    </Stack>
  );
};

export const Route = createFileRoute("/_auth/$listId/$itemId/")({
  component: RouteComponent,
});
