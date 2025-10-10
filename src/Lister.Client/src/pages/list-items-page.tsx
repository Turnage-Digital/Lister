import * as React from "react";

import { AddCircle } from "@mui/icons-material";
import { Stack, useMediaQuery, useTheme } from "@mui/material";
import { GridPaginationModel, GridSortModel } from "@mui/x-data-grid";
import {
  useMutation,
  useQueryClient,
  useSuspenseQuery,
} from "@tanstack/react-query";
import { useNavigate, useParams, useSearchParams } from "react-router-dom";

import { ItemsDesktopView, ItemsMobileView, Titlebar } from "../components";
import { ListSearch } from "../models";
import {
  listItemDefinitionQueryOptions,
  pagedItemsQueryOptions,
} from "../query-options";

export const getListSearch = (params: URLSearchParams): ListSearch => {
  const page = Number(params.get("page") ?? "0");
  const pageSize = Number(params.get("pageSize") ?? "10");
  const field = params.get("field") ?? undefined;
  const sort = params.get("sort") ?? undefined;
  return { page, pageSize, field: field ?? undefined, sort: sort ?? undefined };
};

const setListSearch = (
  updater: (current: ListSearch) => ListSearch,
  setParams: (nextInit: URLSearchParams) => void,
  currentParams: URLSearchParams,
) => {
  const nextSearch = updater(getListSearch(currentParams));
  const nextParams = new URLSearchParams();
  nextParams.set("page", nextSearch.page.toString());
  nextParams.set("pageSize", nextSearch.pageSize.toString());
  if (nextSearch.field) {
    nextParams.set("field", nextSearch.field);
  }
  if (nextSearch.sort) {
    nextParams.set("sort", nextSearch.sort);
  }
  setParams(nextParams);
};

const ListItemsPage = () => {
  const { listId } = useParams<{ listId: string }>();
  if (!listId) {
    throw new Error("List id is required");
  }

  const navigate = useNavigate();
  const [searchParams, setSearchParams] = useSearchParams();
  const queryClient = useQueryClient();
  const search = getListSearch(searchParams);
  const theme = useTheme();
  const isMobile = useMediaQuery(theme.breakpoints.down("lg"));

  const listItemDefinitionQuery = useSuspenseQuery(
    listItemDefinitionQueryOptions(listId),
  );

  const pagedItemsQuery = useSuspenseQuery(
    pagedItemsQueryOptions(search, listId),
  );

  const deleteItemMutation = useMutation({
    mutationFn: async ({
      listId: currentListId,
      itemId,
    }: {
      listId: string;
      itemId: number;
    }) => {
      const request = new Request(
        `/api/lists/${currentListId}/items/${itemId}`,
        {
          method: "DELETE",
        },
      );
      await fetch(request);
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries();
    },
  });

  const handlePaginationChange = (gridPaginationModel: GridPaginationModel) => {
    setListSearch(
      (prev) => ({
        ...prev,
        page: gridPaginationModel.page,
        pageSize: gridPaginationModel.pageSize,
      }),
      (next) => setSearchParams(next),
      searchParams,
    );
  };

  const handleSortChange = (gridSortModel: GridSortModel) => {
    setListSearch(
      (prev) => {
        if (gridSortModel.length === 0) {
          return { ...prev, field: undefined, sort: undefined };
        }
        const field = gridSortModel[0].field;
        const sort = gridSortModel[0].sort === "desc" ? "desc" : "asc";
        return { ...prev, field, sort };
      },
      (next) => setSearchParams(next),
      searchParams,
    );
  };

  const handleViewItem = (currentListId: string, itemId: number) => {
    navigate(`/${currentListId}/${itemId}`);
  };

  const handleDeleteItem = async (currentListId: string, itemId: number) => {
    await deleteItemMutation.mutateAsync({ listId: currentListId, itemId });
  };

  const handleMobilePageChange = (newPage: number) => {
    setListSearch(
      (prev) => ({ ...prev, page: newPage }),
      (next) => setSearchParams(next),
      searchParams,
    );
  };

  if (!listItemDefinitionQuery.isSuccess || !pagedItemsQuery.isSuccess) {
    return null;
  }

  const definition = listItemDefinitionQuery.data;

  const paginationModel: GridPaginationModel = {
    page: search.page,
    pageSize: search.pageSize,
  };

  const sortModel: GridSortModel =
    search.field && search.sort
      ? [
          {
            field: search.field,
            sort: search.sort === "desc" ? "desc" : "asc",
          },
        ]
      : [];

  const actions = [
    {
      title: "Create an Item",
      icon: <AddCircle />,
      onClick: () => navigate(`/${listId}/create`),
    },
  ];

  const breadcrumbs = [
    {
      title: "Lists",
      onClick: () => navigate(`/`),
    },
  ];

  const itemsView = isMobile ? (
    <ItemsMobileView
      items={pagedItemsQuery.data.items}
      definition={definition}
      totalCount={pagedItemsQuery.data.count}
      currentPage={search.page}
      pageSize={search.pageSize}
      onPageChange={handleMobilePageChange}
      onViewItem={handleViewItem}
      onDeleteItem={handleDeleteItem}
    />
  ) : (
    <ItemsDesktopView
      data={pagedItemsQuery.data}
      definition={definition}
      paginationModel={paginationModel}
      sortModel={sortModel}
      onPaginationChange={handlePaginationChange}
      onSortChange={handleSortChange}
      onViewItem={handleViewItem}
      onDeleteItem={handleDeleteItem}
    />
  );

  return (
    <Stack sx={{ px: 2, py: 4 }} spacing={4}>
      <Titlebar
        title={listItemDefinitionQuery.data.name}
        actions={actions}
        breadcrumbs={breadcrumbs}
      />
      {itemsView}
    </Stack>
  );
};

export default ListItemsPage;
