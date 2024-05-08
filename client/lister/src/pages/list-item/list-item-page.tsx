import { Container, Stack } from "@mui/material";
import React, { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";

import { IListsApi, Item, ListItemDefinition, ListsApi } from "../../api";
import { Loading, Titlebar, useAuth } from "../../components";

const listsApi: IListsApi = new ListsApi(`/api/lists`);

const ListItemPage = () => {
  const { signedIn } = useAuth();
  const navigate = useNavigate();
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
          params.listId!
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

  const breadcrumbs = [
    {
      title: "Lists",
      onClick: () => navigate("/"),
    },
    {
      title: listItemDefinition?.name || "",
      onClick: () => navigate(`/${params.listId}`),
    },
  ];

  return loading ? (
    <Loading />
  ) : (
    <Container maxWidth="xl">
      <Stack sx={{ px: 2, py: 4 }}>
        <Titlebar title={`Item # ${item?.id}`} breadcrumbs={breadcrumbs} />
      </Stack>
    </Container>
  );
};

export default ListItemPage;
