import React, { useMemo } from "react";
import {
  LoaderFunctionArgs,
  useLoaderData,
  useNavigate,
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

import { Column, Item, ListItemDefinition } from "../../models";
import { StatusChip } from "../../components";
import { getStatusFromName } from "../../status-fns";

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

  let getItemsUrl = `/api/lists/${params.listId}/items?page=${page}&pageSize=${pageSize}`;
  if (field && sort) {
    getItemsUrl += `&field=${field}&sort=${sort}`;
  }

  const [listItemDefinitionResponse, itemsResponse] = await Promise.all([
    fetch(`/api/lists/${params.listId}/itemDefinition`),
    fetch(getItemsUrl),
  ]);

  const listItemDefinition = await listItemDefinitionResponse.json();
  const data = await itemsResponse.json();

  return { listItemDefinition, data };
};

const ListIdPage = () => {
  const loaded = useLoaderData() as {
    listItemDefinition: ListItemDefinition;
    data: { items: Item[]; count: number };
  };

  const params = useParams();
  const navigate = useNavigate();

  const gridColDefs: GridColDef[] = useMemo(() => {
    if (!loaded.listItemDefinition) {
      return [];
    }

    const retval: GridColDef[] = loaded.listItemDefinition.columns.map(
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
          status={getStatusFromName(
            loaded.listItemDefinition.statuses,
            params.value
          )}
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
            onClick={() => navigate(`/${params.listId}/items/${id}`)}
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
  }, [loaded.listItemDefinition, navigate, params]);

  const [searchParams, setSearchParams] = useSearchParams();
  const pagination = getPaginationFromSearchParams(searchParams);
  const sort = getSortFromSearchParams(searchParams);

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

  const rows = loaded.data.items.map((item: Item) => ({
    id: item.id,
    ...item.bag,
  }));

  return (
    <Paper>
      <DataGrid
        columns={gridColDefs}
        rows={rows}
        getRowId={(row) => row.id}
        rowCount={loaded.data.count}
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
