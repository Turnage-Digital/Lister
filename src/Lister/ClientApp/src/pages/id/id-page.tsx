import React from "react";
import { Paper } from "@mui/material";
import { LoaderFunctionArgs, useLoaderData } from "react-router-dom";
import { DataGrid, GridColDef } from "@mui/x-data-grid";

import { List } from "../../models";

export const idPageLoader = async ({ params }: LoaderFunctionArgs) => {
  if (!params.id) {
    return null;
  }

  const getRequest = new Request(
    `${process.env.PUBLIC_URL}/api/lists/${params.id}`,
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

  const columns: GridColDef[] = loaded.columns.map((column) => ({
    field: column.name,
    headerName: column.name,
    width: 150,
  }));
  columns.push({
    field: "status",
    headerName: "Status",
    width: 150,
  });

  return (
    <Paper>
      <DataGrid
        columns={columns}
        rows={[]}
        getRowId={(row) => row.id}
        // rowCount={count}
        // paginationMode="server"
        // paginationModel={pagination}
        // pageSizeOptions={[10, 25, 50]}
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
