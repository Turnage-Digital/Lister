import React from "react";
import { Paper } from "@mui/material";
import {
  LoaderFunctionArgs,
  useLoaderData,
  useSearchParams,
} from "react-router-dom";
import { DataGrid, GridColDef } from "@mui/x-data-grid";

import { List } from "../../models";

export const idPageLoader = async ({ params }: LoaderFunctionArgs) => {
  if (!params.listId) {
    return null;
  }

  const getRequest = new Request(
    `${process.env.PUBLIC_URL}/api/lists/${params.listId}`,
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
  const [searchParams, setSearchParams] = useSearchParams();
  const loaded = useLoaderData() as List;

  const pagination = {
    page: Number(searchParams.get("page") ?? "1"),
    pageSize: Number(searchParams.get("pageSize") ?? "10"),
  };

  const columns: GridColDef[] = loaded.columns.map((column) => ({
    field: column.property!,
    headerName: column.name,
    width: 150,
  }));
  columns.push({
    field: "status",
    headerName: "Status",
    width: 150,
  });

  const rows = loaded.items.map((item) => ({
    id: item.id,
    ...item.bag,
  }));

  return (
    <Paper>
      <DataGrid
        columns={columns}
        rows={rows}
        getRowId={(row) => row.id}
        // rowCount={count}
        paginationMode="server"
        paginationModel={pagination}
        pageSizeOptions={[10, 25, 50]}
        // onPaginationModelChange={setPagination}
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
