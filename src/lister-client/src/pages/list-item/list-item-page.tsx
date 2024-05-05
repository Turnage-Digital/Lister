import React, { useEffect, useState } from "react";
import { Container, Stack, Typography } from "@mui/material";
import Grid from "@mui/material/Unstable_Grid2";
import { useNavigate, useParams } from "react-router-dom";

import { IListsApi, Item, ListItemDefinition, ListsApi } from "../../api";
import { Loading, useAuth } from "../../components";

const listsApi: IListsApi = new ListsApi(`${process.env.PUBLIC_URL}/api/lists`);

const ListItemPage = () => {
  const { signedIn } = useAuth();
  const params = useParams();

  const [listItemDefinition, setListItemDefinition] =
    useState<ListItemDefinition>();
  const [item, setItem] = useState<Item>();
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!signedIn) {
      return;
    }

    const fetchData = async () => {
      try {
        setLoading(true);
        const listItemDefinition = await listsApi.getListItemDefinition(
          params.listId!,
        );
        setListItemDefinition(listItemDefinition);
      } catch (e: any) {
        setError(e.message);
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, [params.listId, signedIn]);

  useEffect(() => {
    if (!listItemDefinition) {
      return;
    }

    const fetchData = async () => {
      try {
        setLoading(true);
        const item = await listsApi.getItem(params.listId!, params.itemId!);
        setItem(item);
      } catch (e: any) {
        setError(e.message);
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, [params.listId, params.itemId, listItemDefinition]);

  return loading ? (
    <Loading />
  ) : (
    <Container maxWidth="xl">
      <Stack sx={{ px: 2, py: 4 }}>
        <Grid container sx={{ pb: 4 }}>
          <Grid xs={12} md={9}>
            <Typography
              color="primary"
              fontWeight="bold"
              variant="h5"
              component="h1"
            >
              {`Item # ${item?.id}`}
            </Typography>
          </Grid>

          <Grid xs={12} md={3} display="flex" justifyContent="flex-end" />
        </Grid>
      </Stack>
    </Container>
  );
};

export default ListItemPage;
