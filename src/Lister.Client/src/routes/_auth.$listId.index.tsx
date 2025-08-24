import * as React from "react";

import { AddCircle } from "@mui/icons-material";
import { Stack } from "@mui/material";
import { useSuspenseQuery } from "@tanstack/react-query";
import { createFileRoute } from "@tanstack/react-router";

import { Titlebar, ItemsContainer } from "../components";
import { ListSearch } from "../models";
import {
  listItemDefinitionQueryOptions,
  pagedItemsQueryOptions,
} from "../query-options";

const RouteComponent = () => {
  const { listId } = Route.useParams();
  const navigate = Route.useNavigate();
  const { queryClient } = Route.useRouteContext();
  const search = Route.useSearch();

  // Get list definition for titlebar
  const listItemDefinitionQuery = useSuspenseQuery(
    listItemDefinitionQueryOptions(listId),
  );

  if (!listItemDefinitionQuery.isSuccess) {
    return null;
  }

  const actions = [
    {
      title: "Create an Item",
      icon: <AddCircle />,
      onClick: () => navigate({ to: "/$listId/create", params: { listId } }),
    },
  ];

  const breadcrumbs = [
    {
      title: "Lists",
      onClick: () => navigate({ to: "/" }),
    },
  ];

  return (
    <Stack sx={{ px: 2, py: 4 }} spacing={4}>
      <Titlebar
        title={listItemDefinitionQuery.data.name}
        actions={actions}
        breadcrumbs={breadcrumbs}
      />
      <ItemsContainer
        search={search}
        listId={listId}
        queryClient={queryClient}
        navigate={navigate}
      />
    </Stack>
  );
};

export const Route = createFileRoute("/_auth/$listId/")({
  component: RouteComponent,
  validateSearch: (search): ListSearch => {
    const page = Number(search.page ?? "0");
    const pageSize = Number(search.pageSize ?? "10");
    const field = search.field as string | undefined;
    const sort = search.sort as string | undefined;

    return { page, pageSize, field, sort };
  },
  loaderDeps: ({ search }) => search,
  loader: (options) => {
    options.context.queryClient.ensureQueryData(
      pagedItemsQueryOptions(options.deps, options.params.listId),
    );
  },
});
