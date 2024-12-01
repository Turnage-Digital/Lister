import * as React from "react";

import { AddCircle } from "@mui/icons-material";
import { Paper, Stack } from "@mui/material";
import { DataGrid, GridPaginationModel, GridSortModel } from "@mui/x-data-grid";
import { useMutation, useSuspenseQuery } from "@tanstack/react-query";
import { createFileRoute } from "@tanstack/react-router";

import { getGridColDefs, Titlebar } from "../components";
import { ListItem, ListSearch } from "../models";
import {
  listDefinitionQueryOptions,
  pagedItemsQueryOptions,
} from "../query-options";

const RouteComponent = () => {
  const { listId } = Route.useParams();
  const navigate = Route.useNavigate();
  const { queryClient } = Route.useRouteContext();
  const search = Route.useSearch();

  const listDefinitionQuery = useSuspenseQuery(
    listDefinitionQueryOptions(listId),
  );

  const pagedItemsQuery = useSuspenseQuery(
    pagedItemsQueryOptions(search, listId),
  );

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

  const handlePaginationChange = async (
    gridPaginationModel: GridPaginationModel,
  ) => {
    await navigate({
      search: (prev) => ({
        ...prev,
        page: gridPaginationModel.page,
        pageSize: gridPaginationModel.pageSize,
      }),
    });
  };

  const handleSortChange = async (gridSortModel: GridSortModel) => {
    if (gridSortModel.length === 0) {
      await navigate({
        search: (prev) => ({ ...prev, field: undefined, sort: undefined }),
      });
    } else {
      const field = gridSortModel[0].field;
      const sort = gridSortModel[0].sort === "desc" ? "desc" : "asc";

      await navigate({
        search: (prev) => ({ ...prev, field, sort }),
      });
    }
  };

  const handleViewClicked = async (listId: string, itemId: number) => {
    await navigate({ to: "/$listId/$itemId", params: { listId, itemId } });
  };

  const handleDeleteClicked = async (listId: string, itemId: number) => {
    await deleteItemMutation.mutateAsync({ listId, itemId });
  };

  if (!listDefinitionQuery.isSuccess || !pagedItemsQuery.isSuccess) {
    return null;
  }

  const gridColDefs = getGridColDefs(
    listDefinitionQuery.data,
    handleViewClicked,
    handleDeleteClicked,
  );

  const pagination: GridPaginationModel = {
    page: search.page,
    pageSize: search.pageSize,
  };

  const sort: GridSortModel = [];
  if (search.field && search.sort) {
    sort.push({
      field: search.field,
      sort: search.sort === "desc" ? "desc" : "asc",
    });
  }

  const rows = pagedItemsQuery.data.items.map((item: ListItem) => ({
    id: item.id,
    ...item.bag,
  }));

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
    <Stack sx={{ px: 2, py: 4 }}>
      <Titlebar
        title={listDefinitionQuery.data.name}
        actions={actions}
        breadcrumbs={breadcrumbs}
      />

      <Paper sx={{ my: 4 }}>
        <DataGrid
          columns={gridColDefs}
          rows={rows}
          getRowId={(row) => row.id}
          rowCount={pagedItemsQuery.data.count}
          paginationMode="server"
          paginationModel={pagination}
          pageSizeOptions={[10, 25, 50]}
          onPaginationModelChange={handlePaginationChange}
          sortingMode="server"
          sortModel={sort}
          onSortModelChange={handleSortChange}
          disableColumnFilter
          disableColumnSelector
          disableRowSelectionOnClick
        />
      </Paper>
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
