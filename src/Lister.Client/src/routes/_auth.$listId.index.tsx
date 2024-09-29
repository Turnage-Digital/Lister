import React from "react";
import { Paper, Stack } from "@mui/material";
import { AddCircle, MoreVert, Visibility } from "@mui/icons-material";
import {
  DataGrid,
  GridActionsCellItem,
  GridColDef,
  GridPaginationModel,
  GridSortModel,
} from "@mui/x-data-grid";
import { useQuery, useSuspenseQuery } from "@tanstack/react-query";
import { createFileRoute } from "@tanstack/react-router";

import { Column, getStatusFromName, Item, ListItemDefinition } from "../models";
import { StatusChip, Titlebar } from "../components";
import { listDefinitionQueryOptions } from "../queryOptions";

const RouteComponent = () => {
  const { listId } = Route.useParams();
  const navigate = Route.useNavigate();
  const search = Route.useSearch();

  const listDefinitionQuery = useSuspenseQuery(
    listDefinitionQueryOptions(listId)
  );

  const pagedItemsQuery = useQuery<{ items: Item[]; count: number }>({
    queryKey: ["list-items", listId, search.toString()],
    queryFn: async () => {
      let url = `/api/lists/${listId}/items?page=${search.page}&pageSize=${search.pageSize}`;
      if (search.field && search.sort) {
        url += `&field=${search.field}&sort=${search.sort}`;
      }
      const request = new Request(url, {
        method: "GET",
      });
      const response = await fetch(request);
      const retval = await response.json();
      return retval;
    },
    enabled: !!listId,
  });

  const handlePaginationChange = (gridPaginationModel: GridPaginationModel) => {
    navigate({
      search: {
        page: gridPaginationModel.page,
        pageSize: gridPaginationModel.pageSize,
      },
    });
  };

  const handleSortChange = (gridSortModel: GridSortModel) => {
    if (gridSortModel.length === 0) {
      navigate({
        search: (prev) => ({ ...prev, field: undefined, sort: undefined }),
      });
    } else {
      const field = gridSortModel[0].field;
      const sort = gridSortModel[0].sort === "desc" ? "desc" : "asc";

      navigate({
        search: (prev) => ({ ...prev, field, sort }),
      });
    }
  };

  const handleItemClicked = (listId: string, itemId: string) => {
    navigate({ to: "/$listId/$itemId", params: { listId, itemId } });
  };

  if (!listDefinitionQuery.isSuccess || !pagedItemsQuery.isSuccess) {
    return null;
  }

  const getGridColDefs = (
    listItemDefinition: ListItemDefinition,
    handleItemClicked: (listId: string, itemId: string) => void
  ): GridColDef[] => {
    const retval: GridColDef[] = [];

    retval.push({
      field: "id",
      headerName: "ID",
      width: 100,
      sortable: false,
      disableColumnMenu: true,
    });

    const mapped = listItemDefinition.columns.map((column: Column) => {
      const retval: GridColDef = {
        field: column.property!,
        headerName: column.name,
        flex: 1,
      };

      if (column.type === "Date") {
        retval.valueFormatter = (params) => {
          const date = new Date(params.value);
          const retval = date.toLocaleDateString();
          return retval;
        };
      }
      return retval;
    });

    retval.push(...mapped);

    retval.push({
      field: "status",
      headerName: "Status",
      width: 150,
      renderCell: (params) => (
        <StatusChip
          status={getStatusFromName(listItemDefinition.statuses, params.value)}
        />
      ),
    });

    retval.push({
      field: "actions",
      type: "actions",
      headerName: "",
      width: 100,
      cellClassName: "actions",
      getActions: ({ id }) => {
        return [
          <GridActionsCellItem
            key={`${id}-view`}
            icon={<Visibility />}
            label="View"
            color="primary"
            onClick={() => handleItemClicked(listId!, id as string)}
          />,
          <GridActionsCellItem
            key={`${id}-delete`}
            icon={<MoreVert />}
            label="More"
            color="primary"
          />,
        ];
      },
    });

    return retval;
  };

  const gridColDefs = getGridColDefs(
    listDefinitionQuery.data,
    handleItemClicked
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

  const rows = pagedItemsQuery.data.items.map((item: Item) => ({
    id: item.id,
    ...item.bag,
  }));

  const actions = [
    {
      title: "Add an Item",
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

export interface ListIdSearch {
  page: number;
  pageSize: number;
  field?: string;
  sort?: string;
}

export const Route = createFileRoute("/_auth/$listId/")({
  component: RouteComponent,
  validateSearch: (search): ListIdSearch => {
    const page = Number(search.page ?? "0");
    const pageSize = Number(search.pageSize ?? "10");
    const field = search?.field as string | undefined;
    const sort = search?.sort as string | undefined;

    return { page, pageSize, field, sort };
  },
});
