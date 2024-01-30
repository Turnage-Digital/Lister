import React from "react";
import { Box, Paper } from "@mui/material";
import {
  LoaderFunctionArgs,
  useLoaderData,
  useSearchParams,
} from "react-router-dom";
import { DataGrid, GridColDef, GridPaginationModel } from "@mui/x-data-grid";

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
  const [searchParams, setSearchParams] = useSearchParams({
    page: "0",
    pageSize: "10",
  });
  const loaded = useLoaderData() as List;

  const columns: GridColDef[] = loaded.columns.map((column) => ({
    field: column.property!,
    headerName: column.name,
    flex: 1,
  }));
  columns.push({
    field: "status",
    headerName: "Status",
    width: 150,
    disableColumnMenu: true,
    sortable: false,
    renderCell: (params) => (
      <StatusChip status={getStatusFromName(params.value)} />
    ),
  });

  const rows = loaded.items.map((item) => ({
    id: item.id,
    ...item.bag,
  }));

  const pagination = {
    page: Number(searchParams.get("page")),
    pageSize: Number(searchParams.get("pageSize")),
  };

  const getStatusFromName = (name: string): Status => {
    const retval = loaded.statuses.find((status) => status.name === name);
    if (!retval) {
      throw new Error(`Status with name ${name} not found`);
    }
    return retval;
  };

  const handlePaginationChange = (model: GridPaginationModel) => {
    const page = model.page;
    const pageSize = model.pageSize;

    searchParams.set("page", page.toString());
    searchParams.set("pageSize", pageSize.toString());

    setSearchParams(searchParams);
  };

  return (
    <Paper>
      <DataGrid
        columns={columns}
        rows={rows}
        getRowId={(row) => row.id}
        rowCount={loaded.count}
        paginationMode="server"
        paginationModel={pagination}
        pageSizeOptions={[10, 25, 50]}
        onPaginationModelChange={handlePaginationChange}
        // sortingMode="server"
        // sortModel={sort}
        // onSortModelChange={setSort}
        disableColumnFilter
        disableColumnSelector
        disableRowSelectionOnClick
      />
    </Paper>
  );
};

export default IdPage;
