import React from "react";
import { Stack } from "@mui/material";
import Grid from "@mui/material/Unstable_Grid2";
import { PlaylistAdd } from "@mui/icons-material";
import { useQuery } from "@tanstack/react-query";
import { useNavigate } from "react-router-dom";

import { IListsApi, ListsApi } from "../../api";
import { ListCard, Titlebar } from "../../components";

const listsApi: IListsApi = new ListsApi(`/api/lists`);

const ListsPage = () => {
  const navigate = useNavigate();

  const listNamesQuery = useQuery({
    queryKey: ["list-names"],
    queryFn: () => listsApi.getListNames(),
  });

  if (!listNamesQuery.isSuccess) {
    return null;
  }

  const actions = [
    {
      title: "Create a List",
      icon: <PlaylistAdd />,
      onClick: () => navigate(`/create`),
    },
  ];

  return (
    <Stack sx={{ px: 2, py: 4 }}>
      <Titlebar title="Lists" actions={actions} />

      <Grid container spacing={2} sx={{ my: 2 }}>
        {listNamesQuery.data.map((listName) => (
          <ListCard
            key={listName.id}
            listName={listName}
            onViewClick={(id) => navigate(`/${id}`)}
          />
        ))}
      </Grid>
    </Stack>
  );
};

export default ListsPage;
