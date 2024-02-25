import React, { useMemo } from "react";
import {
  LoaderFunctionArgs,
  useLoaderData,
  useParams,
  useSearchParams,
} from "react-router-dom";
import {
  DataGrid,
  GridActionsCellItem,
  GridColDef,
  GridPaginationModel,
  GridSortModel,
} from "@mui/x-data-grid";
import { Paper } from "@mui/material";
import { MoreVert, Visibility } from "@mui/icons-material";

import { Column, Item } from "../../models";
import { Loading, StatusChip } from "../../components";
import { getStatusFromName } from "../../status-fns";
import { useListItemDefinition } from "../../hooks";

export const listIdPageLoader = async ({
  request,
  params,
}: LoaderFunctionArgs) => {
  if (!params.listId) {
    return null;
  }

  const searchParams = new URL(request.url).searchParams;
  const page = Number(searchParams.get("page") ?? "0");
  const pageSize = Number(searchParams.get("pageSize") ?? "10");
  const field = searchParams.get("field");
  const sort = searchParams.get("sort");

  let url = `/api/lists/${params.listId}/items?page=${page}&pageSize=${pageSize}`;
  if (field && sort) {
    url += `&field=${field}&sort=${sort}`;
  }
  const getRequest = new Request(url, {
    method: "GET",
  });

  const response = await fetch(getRequest);
  if (response.status === 401) {
    return null;
  }
  if (response.status === 404) {
    return null;
  }
  const retval = await response.json();
  return retval;
};

const ListIdPage = () => {
  const loaded = useLoaderData() as { items: Item[]; count: number };
  const [searchParams, setSearchParams] = useSearchParams();

  const params = useParams();
  const { listItemDefinition, loading } = useListItemDefinition(params.listId);

  const gridColDefs: GridColDef[] = useMemo(() => {
    if (!listItemDefinition) {
      return [];
    }

    const retval: GridColDef[] = listItemDefinition.columns.map(
      (column: Column) => ({
        field: column.property!,
        headerName: column.name,
        flex: 1,
      })
    );

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
          />,
          <GridActionsCellItem
            key={`${id}-delete`}
            icon={<MoreVert />}
            label="Delete"
          />,
        ];
      },
    });

    return retval;
  }, [listItemDefinition]);

  const handlePaginationChange = (gridPaginationModel: GridPaginationModel) => {
    const page = gridPaginationModel.page;
    const pageSize = gridPaginationModel.pageSize;

    searchParams.set("page", page.toString());
    searchParams.set("pageSize", pageSize.toString());

    setSearchParams(searchParams);
  };

  const handleSortChange = (gridSortModel: GridSortModel) => {
    if (gridSortModel.length === 0) {
      searchParams.delete("field");
      searchParams.delete("sort");
    } else {
      const field = gridSortModel[0].field;
      const sort = gridSortModel[0].sort === "desc" ? "desc" : "asc";

      searchParams.set("field", field);
      searchParams.set("sort", sort);
    }

    setSearchParams(searchParams);
  };

  const rows = loaded.items.map((item) => ({
    id: item.id,
    ...item.bag,
  }));

  const pagination = getPaginationFromSearchParams(searchParams);
  const sort = getSortFromSearchParams(searchParams);

  return loading ? (
    <Loading />
  ) : (
    <Paper>
      <DataGrid
        columns={gridColDefs}
        rows={rows}
        getRowId={(row) => row.id}
        rowCount={loaded.count}
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
  );
};

const getPaginationFromSearchParams = (
  searchParams: URLSearchParams
): GridPaginationModel => {
  const page = Number(searchParams.get("page") ?? "0");
  const pageSize = Number(searchParams.get("pageSize") ?? "10");

  return { page, pageSize };
};

const getSortFromSearchParams = (
  searchParams: URLSearchParams
): GridSortModel => {
  if (!searchParams.has("field")) {
    return [];
  }

  const field = searchParams.get("field")!;
  const sort = searchParams.get("sort") === "desc" ? "desc" : "asc";

  return [{ field, sort }];
};

export default ListIdPage;
