import * as React from "react";

import { AddCircle } from "@mui/icons-material";
import { Stack, useMediaQuery, useTheme } from "@mui/material";
import { GridPaginationModel, GridSortModel } from "@mui/x-data-grid";
import { useMutation, useSuspenseQuery } from "@tanstack/react-query";
import { createFileRoute } from "@tanstack/react-router";

import { Titlebar, ItemsDesktopView, ItemsMobileView } from "../components";
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
  const theme = useTheme();
  const isMobile = useMediaQuery(theme.breakpoints.down("lg"));

  // Get list definition for titlebar
  const listItemDefinitionQuery = useSuspenseQuery(
    listItemDefinitionQueryOptions(listId),
  );

  // Both desktop and mobile now use single page query
  const pagedItemsQuery = useSuspenseQuery(
    pagedItemsQueryOptions(search, listId),
  );

  // Delete item mutation
  const deleteItemMutation = useMutation({
    mutationFn: async ({
      listId,
      itemId,
    }: {
      listId: string;
      itemId: number;
    }) => {
      const request = new Request(`/api/lists/${listId}/items/${itemId}`, {
        method: "DELETE",
      });
      await fetch(request);
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries();
    },
  });

  // Event handlers
  const handlePaginationChange = async (
    gridPaginationModel: GridPaginationModel,
  ) => {
    await navigate({
      search: (prev: ListSearch) => ({
        ...prev,
        page: gridPaginationModel.page,
        pageSize: gridPaginationModel.pageSize,
      }),
    });
  };

  const handleSortChange = async (gridSortModel: GridSortModel) => {
    if (gridSortModel.length === 0) {
      await navigate({
        search: (prev: ListSearch) => ({
          ...prev,
          field: undefined,
          sort: undefined,
        }),
      });
    } else {
      const field = gridSortModel[0].field;
      const sort = gridSortModel[0].sort === "desc" ? "desc" : "asc";

      await navigate({
        search: (prev: ListSearch) => ({ ...prev, field, sort }),
      });
    }
  };

  const handleViewItem = async (listId: string, itemId: number) => {
    await navigate({ to: "/$listId/$itemId", params: { listId, itemId } });
  };

  const handleDeleteItem = async (listId: string, itemId: number) => {
    await deleteItemMutation.mutateAsync({ listId, itemId });
  };

  const handleMobilePageChange = async (newPage: number) => {
    await navigate({
      search: (prev: ListSearch) => ({
        ...prev,
        page: newPage,
      }),
    });
  };

  // Prepare data for rendering
  if (!listItemDefinitionQuery.isSuccess || !pagedItemsQuery.isSuccess) {
    return null;
  }

  const definition = listItemDefinitionQuery.data;

  const paginationModel: GridPaginationModel = {
    page: search.page,
    pageSize: search.pageSize,
  };

  let sortModel: GridSortModel;
  if (search.field && search.sort) {
    sortModel = [
      {
        field: search.field,
        sort: search.sort === "desc" ? "desc" : "asc",
      },
    ];
  } else {
    sortModel = [];
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

  // Render appropriate view
  const itemsView = isMobile ? (
    <ItemsMobileView
      items={pagedItemsQuery.data.items}
      definition={definition}
      totalCount={pagedItemsQuery.data.count}
      currentPage={search.page}
      pageSize={search.pageSize}
      onPageChange={handleMobilePageChange}
      onViewItem={handleViewItem}
      onDeleteItem={handleDeleteItem}
    />
  ) : (
    <ItemsDesktopView
      data={pagedItemsQuery.data}
      definition={definition}
      paginationModel={paginationModel}
      sortModel={sortModel}
      onPaginationChange={handlePaginationChange}
      onSortChange={handleSortChange}
      onViewItem={handleViewItem}
      onDeleteItem={handleDeleteItem}
    />
  );

  return (
    <Stack sx={{ px: 2, py: 4 }} spacing={4}>
      <Titlebar
        title={listItemDefinitionQuery.data.name}
        actions={actions}
        breadcrumbs={breadcrumbs}
      />
      {itemsView}
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
