import { AddCircle } from "@mui/icons-material";
import { Paper, Stack } from "@mui/material";
import { DataGrid, GridPaginationModel, GridSortModel } from "@mui/x-data-grid";
import React, { useEffect, useState } from "react";
import { useNavigate, useParams, useSearchParams } from "react-router-dom";

import { IListsApi, Item, ListsApi } from "../../api";
import { Titlebar, useListDefinition, useLoad } from "../../components";

const listsApi: IListsApi = new ListsApi(`/api/lists`);

const ListPage = () => {
  const { listItemDefinition, getGridColDefs } = useListDefinition();
  const { loading, setLoading } = useLoad();

  const navigate = useNavigate();
  const { listId } = useParams();
  const [searchParams, setSearchParams] = useSearchParams();

  const [pagedItems, setPagedItems] = useState<{
    items: Item[];
    count: number;
  }>({
    items: [],
    count: 0,
  });

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
          listId!,
          page,
          pageSize,
          field,
          sort
        );
        setPagedItems({ items, count });
      } catch (e: any) {
        // setError(e.message);
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, [listId, listItemDefinition, searchParams, setLoading]);

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

  const handleItemClicked = (listId: string, itemId: string) => {
    navigate(`/${listId}/items/${itemId}`);
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

  const rows = pagedItems.items.map((item: Item) => ({
    id: item.id,
    ...item.bag,
  }));
  const gridColDefs = getGridColDefs(handleItemClicked);
  const pagination = getPaginationFromSearchParams(searchParams);
  const sort = getSortFromSearchParams(searchParams);

  const actions = [
    {
      title: "Create an Item",
      icon: <AddCircle />,
      onClick: () => navigate(`/${listId}/items/create`),
    },
  ];

  const breadcrumbs = [
    {
      title: "Lists",
      onClick: () => navigate("/"),
    },
  ];

  return loading || listItemDefinition === null ? null : (
    <Stack sx={{ px: 2, py: 4 }}>
      <Titlebar
        title={listItemDefinition.name ?? ""}
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
  );
};

export default ListPage;
