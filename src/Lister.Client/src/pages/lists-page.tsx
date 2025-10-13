import * as React from "react";

import { PlaylistAdd } from "@mui/icons-material";
import { Grid, Stack } from "@mui/material";
import {
  useMutation,
  useQueryClient,
  useSuspenseQuery,
} from "@tanstack/react-query";
import { useNavigate } from "react-router-dom";

import { ListCard, Titlebar } from "../components";
import { listNamesQueryOptions } from "../query-options";

const ListsPage = () => {
  const navigate = useNavigate();
  const queryClient = useQueryClient();

  const deleteListMutation = useMutation({
    mutationFn: async (listId: string) => {
      const request = new Request(`/api/lists/${listId}`, {
        method: "DELETE",
      });
      await fetch(request);
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries();
    },
  });

  const handleDeleteClicked = async (listId: string) => {
    await deleteListMutation.mutateAsync(listId);
  };

  const listNamesQuery = useSuspenseQuery(listNamesQueryOptions());
  if (!listNamesQuery.isSuccess) {
    return null;
  }

  const actions = [
    {
      title: "Create a List",
      icon: <PlaylistAdd />,
      onClick: () => navigate("/create"),
    },
  ];

  return (
    <Stack
      sx={{
        maxWidth: 1400,
        mx: "auto",
        px: { xs: 3, md: 8 },
        py: { xs: 4, md: 6 },
      }}
      spacing={{ xs: 6, md: 7 }}
    >
      <Titlebar title="Lists" actions={actions} />

      <Grid container spacing={3}>
        {listNamesQuery.data.map((listName) => (
          <Grid key={listName.id} size={{ xs: 12, sm: 6, md: 4 }}>
            <ListCard
              listName={listName}
              onDeleteClick={() => handleDeleteClicked(listName.id)}
            />
          </Grid>
        ))}
      </Grid>
    </Stack>
  );
};

export default ListsPage;
