import React from "react";
import { Paper, Stack } from "@mui/material";
import { DataGrid, GridPaginationModel, GridSortModel } from "@mui/x-data-grid";
import { AddCircle } from "@mui/icons-material";
import { createFileRoute } from "@tanstack/react-router";
import { useSuspenseQuery } from "@tanstack/react-query";

import { getGridColDefs, Titlebar } from "../components";
import { Item, ListSearch } from "../models";
import { listDefinitionQueryOptions, pagedItemsQueryOptions } from "../query-options";

const RouteComponent = () => {
  const { listId } = Route.useParams();
  const navigate = Route.useNavigate();
  const search = Route.useSearch();

  const listDefinitionQuery = useSuspenseQuery(
    listDefinitionQueryOptions(listId)
  );

  const pagedItemsQuery = useSuspenseQuery(
    pagedItemsQueryOptions(search, listId)
  );

  const handlePaginationChange = (gridPaginationModel: GridPaginationModel) => {
    navigate({
      search: (prev) => ({
        ...prev,
        page: gridPaginationModel.page,
        pageSize: gridPaginationModel.pageSize
      })
    });
  };

  const handleSortChange = (gridSortModel: GridSortModel) => {
    if (gridSortModel.length === 0) {
      navigate({
        search: (prev) => ({ ...prev, field: undefined, sort: undefined })
      });
    } else {
      const field = gridSortModel[0].field;
      const sort = gridSortModel[0].sort === "desc" ? "desc" : "asc";

      navigate({
        search: (prev) => ({ ...prev, field, sort })
      });
    }
  };

  const handleItemClicked = (listId: string, itemId: string) => {
    navigate({ to: "/$listId/$itemId", params: { listId, itemId } });
  };

  if (!listDefinitionQuery.isSuccess || !pagedItemsQuery.isSuccess) {
    return null;
  }

  const gridColDefs = getGridColDefs(
    listDefinitionQuery.data,
    handleItemClicked
  );

  const pagination: GridPaginationModel = {
    page: search.page,
    pageSize: search.pageSize
  };

  const sort: GridSortModel = [];
  if (search.field && search.sort) {
    sort.push({
      field: search.field,
      sort: search.sort === "desc" ? "desc" : "asc"
    });
  }

  const rows = pagedItemsQuery.data.items.map((item: Item) => ({
    id: item.id,
    ...item.bag
  }));

  const actions = [
    {
      title: "Add an Item",
      icon: <AddCircle />,
      onClick: () => navigate({ to: "/$listId/create", params: { listId } })
    }
  ];

  const breadcrumbs = [
    {
      title: "Lists",
      onClick: () => navigate({ to: "/" })
    }
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
    const field = search?.field as string | undefined;
    const sort = search?.sort as string | undefined;

    return { page, pageSize, field, sort };
  },
  loaderDeps: ({ search }) => search,
  loader: (options) => {
    options.context.queryClient.ensureQueryData(
      pagedItemsQueryOptions(options.deps, options.params.listId)
    );
  }
});
