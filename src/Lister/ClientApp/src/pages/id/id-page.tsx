import React from "react";
import {
  LoaderFunctionArgs,
  useLoaderData,
  useSearchParams,
} from "react-router-dom";
import {
  DataGrid,
  GridColDef,
  GridPaginationModel,
  GridSortModel,
} from "@mui/x-data-grid";

import { List, Status } from "../../models";
import { StatusChip } from "../../components";

export const idPageLoader = async ({ request, params }: LoaderFunctionArgs) => {
  if (!params.listId) {
    return null;
  }

  const url = new URL(request.url);
  const page = Number(url.searchParams.get("page") ?? "0");
  const pageSize = Number(url.searchParams.get("pageSize") ?? "10");
  const getRequest = new Request(
    `${process.env.PUBLIC_URL}/api/lists/${params.listId}?page=${page}&pageSize=${pageSize}`,
    {
      method: "GET",
    }
  );

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

const IdPage = () => {
  const loaded = useLoaderData() as List;
  const [searchParams, setSearchParams] = useSearchParams();

  const rows = loaded.items.map((item) => ({
    id: item.id,
    ...item.bag,
  }));

  const gridColDefs: GridColDef[] = loaded.columns.map((column) => ({
    field: column.property!,
    headerName: column.name,
    flex: 1,
  }));
  gridColDefs.push({
    field: "status",
    headerName: "Status",
    width: 150,
    disableColumnMenu: true,
    sortable: false,
    renderCell: (params) => (
      <StatusChip status={getStatusFromName(loaded.statuses, params.value)} />
    ),
  });

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

  return (
    <DataGrid
      columns={gridColDefs}
      rows={rows}
      getRowId={(row) => row.id}
      rowCount={loaded.count}
      paginationMode="server"
      paginationModel={getPaginationFromSearchParams(searchParams)}
      pageSizeOptions={[10, 25, 50]}
      onPaginationModelChange={handlePaginationChange}
      sortingMode="server"
      sortModel={getSortFromSearchParams(searchParams)}
      onSortModelChange={handleSortChange}
      disableColumnFilter
      disableColumnSelector
      disableRowSelectionOnClick
    />
  );
};

const getStatusFromName = (statuses: Status[], name: string): Status => {
  const retval = statuses.find((status) => status.name === name);
  if (!retval) {
    throw new Error(`Status with name ${name} not found`);
  }
  return retval;
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

  const field = searchParams.get("field") ?? "id";
  const sort = searchParams.get("sort") === "desc" ? "desc" : "asc";

  return [{ field, sort }];
};

export default IdPage;
