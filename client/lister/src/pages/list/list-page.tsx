import { AddCircle, MoreVert, Visibility } from "@mui/icons-material";
import { Container, Paper, Stack } from "@mui/material";
import {
  DataGrid,
  GridActionsCellItem,
  GridColDef,
  GridPaginationModel,
  GridSortModel,
} from "@mui/x-data-grid";
import React, { useEffect, useMemo, useState } from "react";
import { useNavigate, useParams, useSearchParams } from "react-router-dom";

import {
  Column,
  IListsApi,
  Item,
  ListItemDefinition,
  ListsApi,
} from "../../api";
import { Loading, StatusChip, Titlebar, useAuth } from "../../components";
import { getStatusFromName } from "../../status-fns";

const listsApi: IListsApi = new ListsApi(`/api/lists`);

const ListPage = () => {
  const { signedIn } = useAuth();
  const navigate = useNavigate();
  const params = useParams();
  const [searchParams, setSearchParams] = useSearchParams();

  const [listItemDefinition, setListItemDefinition] =
    useState<ListItemDefinition>();
  const [pagedItems, setPagedItems] = useState<{
    items: Item[];
    count: number;
  }>({
    items: [],
    count: 0,
  });
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!signedIn) {
      return;
    }

    const fetchData = async () => {
      try {
        setLoading(true);
        const listItemDefinition = await listsApi.getListItemDefinition(
          params.listId!
        );
        setListItemDefinition(listItemDefinition);
      } catch (e: any) {
        setError(e.message);
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, [params.listId, signedIn]);

  useEffect(() => {
    if (!listItemDefinition) {
      return;
    }

    const page = Number(searchParams.get("page") ?? "0");
    const pageSize = Number(searchParams.get("pageSize") ?? "10");
    const field = searchParams.get("field");
    const sort = searchParams.get("sort");

    const fetchData = async () => {
      try {
        if (!loading) setLoading(true);
        const { items, count } = await listsApi.getItems(
          params.listId!,
          page,
          pageSize,
          field,
          sort
        );
        setPagedItems({ items, count });
      } catch (e: any) {
        setError(e.message);
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, [params.listId, listItemDefinition, searchParams]);

  const gridColDefs: GridColDef[] = useMemo(() => {
    if (!listItemDefinition) {
      return [];
    }

    const retval: GridColDef[] = [];

    retval.push({
      field: "id",
      headerName: "Item #",
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
            onClick={() => navigate(`/${params.listId}/items/${id}`)}
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
  }, [listItemDefinition, navigate, params]);

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

  const rows = pagedItems.items.map((item: Item) => ({
    id: item.id,
    ...item.bag,
  }));
  const pagination = getPaginationFromSearchParams(searchParams);
  const sort = getSortFromSearchParams(searchParams);

  const actions = [
    {
      title: "Create an Item",
      icon: <AddCircle />,
      onClick: () => navigate(`/${params.listId}/items/create`),
    },
  ];

  const breadcrumbs = [
    {
      title: "Lists",
      onClick: () => navigate("/"),
    },
  ];

  return loading ? (
    <Loading />
  ) : (
    <Container maxWidth="xl">
      <Stack sx={{ px: 2, py: 4 }}>
        <Titlebar
          title={listItemDefinition?.name ?? ""}
          actions={actions}
          breadcrumbs={breadcrumbs}
        />

        <Paper sx={{ my: 4 }}>
          <DataGrid
            columns={gridColDefs}
            rows={rows}
            getRowId={(row) => row.id}
            rowCount={pagedItems.count}
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
    </Container>
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

export default ListPage;
