import React, { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import {
  Button,
  Card,
  CardActions,
  CardContent,
  Container,
  Divider,
  Stack,
  Typography,
} from "@mui/material";
import Grid from "@mui/material/Unstable_Grid2";
import { AddCircle, PlaylistAdd } from "@mui/icons-material";

import { IListsApi, ListName, ListsApi } from "../../api";
import { useAuth } from "../../auth";
import { Loading } from "../../components";

const listsApi: IListsApi = new ListsApi(`${process.env.PUBLIC_URL}/api/lists`);

const ListsPage = () => {
  const { signedIn } = useAuth();
  const navigate = useNavigate();

  const [listNames, setListNames] = useState<ListName[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!signedIn) {
      return;
    }

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
  }, [signedIn]);

  const content = loading ? (
    <Loading />
  ) : (
    <Grid container>
      {listNames.map((listName) => (
        <Grid key={listName.id} xs={4}>
          <Card>
            <CardContent>
              <Typography gutterBottom variant="h5" component="div">
                {listName.name}
              </Typography>
            </CardContent>
            <CardActions>
              <Button size="small" onClick={() => navigate(`/${listName.id}`)}>
                View
              </Button>
            </CardActions>
          </Card>
        </Grid>
      ))}
    </Grid>
  );

  return (
    <Container maxWidth="xl">
      <Stack spacing={4} sx={{ px: 2, py: 4 }}>
        <Grid container>
          <Grid xs={12} md={9}>
            <Grid>
              <Typography
                color="primary"
                fontWeight="medium"
                variant="h4"
                component="h1"
              >
                Lists
              </Typography>
            </Grid>
          </Grid>

          <Grid xs={12} md={3} display="flex" justifyContent="flex-end">
            <Button
              variant="contained"
              startIcon={<PlaylistAdd />}
              onClick={() => navigate(`/create`)}
            >
              Create a List
            </Button>
          </Grid>
        </Grid>

        {content}
      </Stack>
    </Container>
  );
};

export default ListsPage;
