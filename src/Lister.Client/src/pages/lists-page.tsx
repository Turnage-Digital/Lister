import * as React from "react";

import { PlaylistAdd } from "@mui/icons-material";
import { Grid } from "@mui/material";
import {
  useMutation,
  useQueryClient,
  useSuspenseQuery,
} from "@tanstack/react-query";
import { useNavigate } from "react-router-dom";

import {
  ConfirmDeleteDialog,
  DisplayPageLayout,
  ListCard,
  Titlebar,
} from "../components";
import { ListName } from "../models";
import { listNamesQueryOptions } from "../query-options";

const ListsPage = () => {
  const navigate = useNavigate();
  const [listToDelete, setListToDelete] = React.useState<ListName | null>(null);
  const queryClient = useQueryClient();

  const listNamesQuery = useSuspenseQuery(listNamesQueryOptions());

  const deleteListDialogMessage = listToDelete
    ? `Are you sure you want to delete "${listToDelete.name}"? This action cannot be undone.`
    : "Are you sure you want to delete this list? This action cannot be undone.";

  const deleteListMutation = useMutation({
    mutationFn: async (listId: string) => {
      const request = new Request(`/api/lists/${listId}`, {
        method: "DELETE",
      });
      const response = await fetch(request);
      if (!response.ok) {
        const message = await response
          .text()
          .catch(() => "Failed to delete list");
        throw new Error(message);
      }
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries();
    },
  });

  const handleDeleteClicked = (list: ListName) => {
    setListToDelete(list);
  };

  const handleConfirmDelete = async () => {
    if (!listToDelete) {
      return;
    }
    try {
      await deleteListMutation.mutateAsync(listToDelete.id);
    } finally {
      setListToDelete(null);
    }
  };

  const handleCancelDelete = () => {
    setListToDelete(null);
  };

  const handleCreateList = () => {
    navigate("/create");
  };

  const actions = [
    {
      title: "Create a List",
      icon: <PlaylistAdd />,
      onClick: handleCreateList,
    },
  ];

  return (
    <>
      <Titlebar title="Lists" actions={actions} />

      <Grid container spacing={3}>
        {listNamesQuery.data.map((listName) => (
          <Grid key={listName.id} size={{ xs: 12, sm: 6, md: 4 }}>
            <ListCard listName={listName} onDeleteClick={handleDeleteClicked} />
          </Grid>
        ))}
      </Grid>

      <ConfirmDeleteDialog
        open={Boolean(listToDelete)}
        title="Delete list"
        description={deleteListDialogMessage}
        confirmDisabled={deleteListMutation.isPending}
        onCancel={handleCancelDelete}
        onConfirm={handleConfirmDelete}
      />
    </>
  );
};

export default ListsPage;
