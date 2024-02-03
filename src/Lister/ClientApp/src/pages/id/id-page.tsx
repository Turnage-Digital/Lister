import React, { useEffect, useMemo, useState } from "react";
import {
  LoaderFunctionArgs,
  useLoaderData,
  useParams,
  useSearchParams,
} from "react-router-dom";
import {
  DataGrid,
  GridColDef,
  GridPaginationModel,
  GridSortModel,
} from "@mui/x-data-grid";
import { Paper } from "@mui/material";

import { Column, Item, ListItemDefinition, Status } from "../../models";
import { Loading, StatusChip } from "../../components";
import { getStatusFromName } from "../../status-fns";

export const idPageLoader = async ({ request, params }: LoaderFunctionArgs) => {
  if (!params.listId) {
    return null;
  }

  const url = new URL(request.url);
  const page = Number(url.searchParams.get("page") ?? "0");
  const pageSize = Number(url.searchParams.get("pageSize") ?? "10");
  const getRequest = new Request(
    `/api/lists/${params.listId}/items?page=${page}&pageSize=${pageSize}`,
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
  const loaded = useLoaderData() as { items: Item[]; count: number };
  const params = useParams();
  const [searchParams, setSearchParams] = useSearchParams();
  const [listItemDefinition, setListItemDefinition] =
    useState<ListItemDefinition | null>(null);

  useEffect(() => {
    const request = new Request(`/api/lists/${params.listId}/itemDefinition`, {
      method: "GET",
    });

    const fetchData = async () => {
      const response = await fetch(request);
      const data = await response.json();

      setListItemDefinition(data);
    };

    fetchData();
  }, [params.listId]);

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
      disableColumnMenu: true,
      sortable: false,
      renderCell: (params) => (
        <StatusChip
          status={getStatusFromName(listItemDefinition.statuses, params.value)}
        />
      ),
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

  return listItemDefinition ? (
    <Paper>
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
    </Paper>
  ) : (
    <Loading />
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

export default IdPage;
