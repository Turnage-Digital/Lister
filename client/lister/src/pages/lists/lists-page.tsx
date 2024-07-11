import { PlaylistAdd } from "@mui/icons-material";
import {
  Button,
  Card,
  CardActions,
  CardContent,
  Stack,
  Typography,
} from "@mui/material";
import Grid from "@mui/material/Unstable_Grid2";
import React, { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";

import { IListsApi, ListName, ListsApi } from "../../api";
import { ListCard, Titlebar, useLoad } from "../../components";

const listsApi: IListsApi = new ListsApi(`/api/lists`);

const ListsPage = () => {
  const { loading, setLoading } = useLoad();

  const navigate = useNavigate();

  const [listNames, setListNames] = useState<ListName[]>([]);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchData = async () => {
      try {
        setLoading(true);
        const listNames = await listsApi.getListNames();
        setListNames(listNames);
      } catch (e: any) {
        setError(e.message);
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, [setLoading]);

  const actions = [
    {
      title: "Create a List",
      icon: <PlaylistAdd />,
      onClick: () => navigate(`/create`),
    },
  ];

  return loading ? null : (
    <Stack sx={{ px: 2, py: 4 }}>
      <Titlebar title="Lists" actions={actions} />

      <Grid container spacing={2} sx={{ my: 2 }}>
        {listNames.map((listName) => (
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
