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
import { Titlebar, useLoad } from "../../components";

const listsApi: IListsApi = new ListsApi(`/api/lists`);

const ListsPage = () => {
  const { loading, setLoading } = useLoad();

  const navigate = useNavigate();

  const [listNames, setListNames] = useState<ListName[]>([]);

  useEffect(() => {
    const fetchData = async () => {
      try {
        setLoading(true);
        const listNames = await listsApi.getListNames();
        setListNames(listNames);
      } catch (e: any) {
        // setError(e.message);
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
          <Grid key={listName.id} xs={12} sm={6} md={4}>
            <Card>
              <CardContent>
                <Typography gutterBottom variant="h5" component="div">
                  {listName.name}
                </Typography>
              </CardContent>
              <CardActions>
                <Button
                  size="small"
                  onClick={() => navigate(`/${listName.id}`)}
                >
                  View
                </Button>
              </CardActions>
            </Card>
          </Grid>
        ))}
      </Grid>
    </Stack>
  );
};

export default ListsPage;
