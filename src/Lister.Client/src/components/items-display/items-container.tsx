import * as React from "react";

import { useMediaQuery, useTheme } from "@mui/material";
import { GridPaginationModel, GridSortModel } from "@mui/x-data-grid";
import { useMutation, useSuspenseQuery } from "@tanstack/react-query";

import ItemsDesktopView from "./items-desktop-view";
import ItemsMobileView from "./items-mobile-view";
import { useAccumulatedItems } from "../../hooks";
import { ListSearch } from "../../models";
import {
  listItemDefinitionQueryOptions,
  pagedItemsQueryOptions,
} from "../../query-options";

interface Props {
  search: ListSearch;
  listId: string;
  queryClient: any;
  navigate: any;
}

const ItemsContainer = ({
  search,
  listId,
  queryClient,
  navigate,
}: Props) => {
  const theme = useTheme();
  const isMobile = useMediaQuery(theme.breakpoints.down("md"));

  // Get list definition
  const listItemDefinitionQuery = useSuspenseQuery(
    listItemDefinitionQueryOptions(listId),
  );

  // Desktop: Use single page query
  // Mobile: Use accumulated items from all pages up to current
  const desktopQuery = useSuspenseQuery(pagedItemsQueryOptions(search, listId));

  const mobileData = useAccumulatedItems(search, listId);

  // Delete item mutation
  const deleteItemMutation = useMutation({
    mutationFn: async ({
      listId,
      itemId,
    }: {
      listId: string;
      itemId: number;
    }) => {
      const request = new Request(`/api/lists/${listId}/items/${itemId}`, {
        method: "DELETE",
      });
      await fetch(request);
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries();
    },
  });

  // Event handlers
  const handlePaginationChange = async (
    gridPaginationModel: GridPaginationModel,
  ) => {
    await navigate({
      search: (prev: ListSearch) => ({
        ...prev,
        page: gridPaginationModel.page,
        pageSize: gridPaginationModel.pageSize,
      }),
    });
  };

  const handleSortChange = async (gridSortModel: GridSortModel) => {
    if (gridSortModel.length === 0) {
      await navigate({
        search: (prev: ListSearch) => ({
          ...prev,
          field: undefined,
          sort: undefined,
        }),
      });
    } else {
      const field = gridSortModel[0].field;
      const sort = gridSortModel[0].sort === "desc" ? "desc" : "asc";

      await navigate({
        search: (prev: ListSearch) => ({ ...prev, field, sort }),
      });
    }
  };

  const handleViewItem = async (listId: string, itemId: number) => {
    await navigate({ to: "/$listId/$itemId", params: { listId, itemId } });
  };

  const handleDeleteItem = async (listId: string, itemId: number) => {
    await deleteItemMutation.mutateAsync({ listId, itemId });
  };

  const handleLoadMore = async () => {
    if (mobileData.isLoading) return;

    await navigate({
      search: (prev: ListSearch) => ({
        ...prev,
        page: (prev.page || 0) + 1,
      }),
    });
  };

  // Prepare data for rendering
  if (
    !listItemDefinitionQuery.isSuccess ||
    (!isMobile && !desktopQuery.isSuccess) ||
    (isMobile && !mobileData.allQueriesSuccessful)
  ) {
    return null;
  }

  const definition = listItemDefinitionQuery.data;

  const paginationModel: GridPaginationModel = {
    page: search.page,
    pageSize: search.pageSize,
  };

  let sortModel: GridSortModel;
  if (search.field && search.sort) {
    sortModel = [
      {
        field: search.field,
        sort: search.sort === "desc" ? "desc" : "asc",
      },
    ];
  } else {
    sortModel = [];
  }

  // Render appropriate view
  if (isMobile) {
    return (
      <ItemsMobileView
        items={mobileData.accumulatedItems}
        definition={definition}
        totalCount={mobileData.totalCount}
        hasMoreItems={mobileData.hasMoreItems}
        isLoadingMore={mobileData.isLoading}
        onLoadMore={handleLoadMore}
      />
    );
  }

  return (
    <ItemsDesktopView
      data={desktopQuery.data}
      definition={definition}
      paginationModel={paginationModel}
      sortModel={sortModel}
      onPaginationChange={handlePaginationChange}
      onSortChange={handleSortChange}
      onViewItem={handleViewItem}
      onDeleteItem={handleDeleteItem}
    />
  );
};

export default ItemsContainer;
