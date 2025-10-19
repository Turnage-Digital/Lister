import * as React from "react";

import { PlaylistAdd } from "@mui/icons-material";
import {
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  DialogContentText,
  DialogTitle,
  Grid,
} from "@mui/material";
import {
  useMutation,
  useQueryClient,
  useSuspenseQuery,
} from "@tanstack/react-query";
import { useNavigate } from "react-router-dom";

import { DisplayPageLayout, ListCard, Titlebar } from "../components";
import { ListName } from "../models";
import { listNamesQueryOptions } from "../query-options";

const ListsPage = () => {
  const navigate = useNavigate();
  const [listToDelete, setListToDelete] = React.useState<ListName | null>(null);
  const queryClient = useQueryClient();

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

  const listNamesQuery = useSuspenseQuery(listNamesQueryOptions());

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
    <DisplayPageLayout>
      <Titlebar title="Lists" actions={actions} />

      <Grid container spacing={3}>
        {listNamesQuery.data.map((listName) => (
          <Grid key={listName.id} size={{ xs: 12, sm: 6, md: 4 }}>
            <ListCard listName={listName} onDeleteClick={handleDeleteClicked} />
          </Grid>
        ))}
      </Grid>

      <Dialog
        open={Boolean(listToDelete)}
        onClose={handleCancelDelete}
        maxWidth="xs"
        fullWidth
      >
        <DialogTitle>Delete list</DialogTitle>
        <DialogContent>
          <DialogContentText>{deleteListDialogMessage}</DialogContentText>
        </DialogContent>
        <DialogActions>
          <Button onClick={handleCancelDelete}>Cancel</Button>
          <Button
            onClick={handleConfirmDelete}
            color="error"
            variant="contained"
            disabled={deleteListMutation.isPending}
          >
            Delete
          </Button>
        </DialogActions>
      </Dialog>
    </DisplayPageLayout>
  );
};

export default ListsPage;
