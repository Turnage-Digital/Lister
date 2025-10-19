import * as React from "react";

import { AddCircle, History } from "@mui/icons-material";
import {
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  DialogContentText,
  DialogTitle,
  useMediaQuery,
  useTheme,
} from "@mui/material";
import { GridPaginationModel, GridSortModel } from "@mui/x-data-grid";
import {
  useMutation,
  useQueryClient,
  useSuspenseQuery,
} from "@tanstack/react-query";
import { useNavigate, useParams, useSearchParams } from "react-router-dom";

import {
  DisplayPageLayout,
  ItemsDesktopView,
  ItemsMobileView,
  ListHistoryDrawer,
  Titlebar,
  useSideDrawer,
} from "../components";
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
  const { openDrawer } = useSideDrawer();
  const [searchParams, setSearchParams] = useSearchParams();
  const queryClient = useQueryClient();
  const search = getListSearch(searchParams);
  const theme = useTheme();
  const isMobile = useMediaQuery(theme.breakpoints.down("lg"));
  const [itemToDelete, setItemToDelete] = React.useState<{
    listId: string;
    itemId: number;
  } | null>(null);

  const deleteItemDialogMessage = itemToDelete
    ? `Are you sure you want to delete item #${itemToDelete.itemId}? This action cannot be undone.`
    : "Are you sure you want to delete this item? This action cannot be undone.";

  const listItemDefinitionQuery = useSuspenseQuery(
    listItemDefinitionQueryOptions(listId),
  );

  const pagedItemsQuery = useSuspenseQuery(
    pagedItemsQueryOptions(search, listId),
  );

  const definition = listItemDefinitionQuery.data;

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
      const response = await fetch(request);
      if (!response.ok) {
        const message = await response
          .text()
          .catch(() => "Failed to delete item");
        throw new Error(message);
      }
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

  const handleEditItem = (currentListId: string, itemId: number) => {
    navigate(`/${currentListId}/${itemId}/edit`);
  };

  const handleDeleteItem = (currentListId: string, itemId: number) => {
    setItemToDelete({ listId: currentListId, itemId });
  };

  const handleConfirmDeleteItem = async () => {
    if (!itemToDelete) {
      return;
    }

    try {
      await deleteItemMutation.mutateAsync(itemToDelete);
    } finally {
      setItemToDelete(null);
    }
  };

  const handleCancelDeleteItem = () => {
    setItemToDelete(null);
  };

  const handleMobilePageChange = (newPage: number) => {
    setListSearch(
      (prev) => ({ ...prev, page: newPage }),
      (next) => setSearchParams(next),
      searchParams,
    );
  };

  const handleCreateItem = () => {
    navigate(`/${listId}/create`);
  };

  const handleShowHistory = () => {
    openDrawer("List history", <ListHistoryDrawer listId={listId} />);
  };

  const handleNavigateToLists = () => {
    navigate("/");
  };
  
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
      onClick: handleCreateItem,
    },
    {
      title: "Show history",
      icon: <History />,
      variant: "outlined" as const,
      color: "secondary" as const,
      onClick: handleShowHistory,
    },
  ];

  const breadcrumbs = [
    {
      title: "Lists",
      onClick: handleNavigateToLists,
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
      onEditItem={handleEditItem}
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
      onEditItem={handleEditItem}
      onDeleteItem={handleDeleteItem}
    />
  );

  return (
    <DisplayPageLayout>
      <Titlebar
        title={listItemDefinitionQuery.data.name}
        actions={actions}
        breadcrumbs={breadcrumbs}
      />
      {itemsView}
      <Dialog
        open={Boolean(itemToDelete)}
        onClose={handleCancelDeleteItem}
        maxWidth="xs"
        fullWidth
      >
        <DialogTitle>Delete item</DialogTitle>
        <DialogContent>
          <DialogContentText>{deleteItemDialogMessage}</DialogContentText>
        </DialogContent>
        <DialogActions>
          <Button onClick={handleCancelDeleteItem}>Cancel</Button>
          <Button
            onClick={handleConfirmDeleteItem}
            color="error"
            variant="contained"
            disabled={deleteItemMutation.isPending}
          >
            Delete
          </Button>
        </DialogActions>
      </Dialog>
    </DisplayPageLayout>
  );
};

export default ListItemsPage;
