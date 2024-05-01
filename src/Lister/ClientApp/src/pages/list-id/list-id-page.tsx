import React, { useEffect, useMemo, useState } from "react";
import { useNavigate, useParams, useSearchParams } from "react-router-dom";
import {
  DataGrid,
  GridActionsCellItem,
  GridColDef,
  GridPaginationModel,
  GridSortModel,
} from "@mui/x-data-grid";
import {
  Button,
  Container,
  Divider,
  Paper,
  Stack,
  Typography,
} from "@mui/material";
import { AddCircle, MoreVert, Visibility } from "@mui/icons-material";
import Grid from "@mui/material/Unstable_Grid2";

import {
  Column,
  IListsApi,
  Item,
  ListItemDefinition,
  ListsApi,
} from "../../api";
import { Loading, StatusChip } from "../../components";
import { getStatusFromName } from "../../status-fns";
import { useAuth } from "../../auth";

const listsApi: IListsApi = new ListsApi(`${process.env.PUBLIC_URL}/api/lists`);

const ListIdPage = () => {
  const { signedIn } = useAuth();
  const navigate = useNavigate();
  const params = useParams();
  const [searchParams, setSearchParams] = useSearchParams();

  const [listItemDefinition, setListItemDefinition] =
    useState<ListItemDefinition | null>(null);
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
        setLoading(true);
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

  return loading ? (
    <Loading />
  ) : (
    <Container maxWidth="xl">
      <Stack spacing={4} divider={<Divider />} sx={{ px: 2, py: 4 }}>
        <Grid container>
          <Grid xs={12} md={9}>
            <Grid>
              <Typography
                color="primary"
                fontWeight="medium"
                variant="h4"
                component="h1"
              >
                {listItemDefinition?.name}
              </Typography>
            </Grid>
          </Grid>

          <Grid xs={12} md={3} display="flex" justifyContent="flex-end">
            <Button
              variant="contained"
              startIcon={<AddCircle />}
              onClick={() =>
                navigate(`/${listItemDefinition?.id}/items/create`)
              }
            >
              Create an Item
            </Button>
          </Grid>
        </Grid>

        <Paper>
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

export default ListIdPage;
