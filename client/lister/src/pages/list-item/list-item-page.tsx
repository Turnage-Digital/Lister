import { Container, Stack } from "@mui/material";
import React, { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";

import { IListsApi, Item, ListsApi } from "../../api";
import { Loading, Titlebar, useListDefinition } from "../../components";

const listsApi: IListsApi = new ListsApi(`/api/lists`);

const ListItemPage = () => {
  const { listItemDefinition } = useListDefinition();
  const navigate = useNavigate();
  const { listId, itemId } = useParams();

  const [item, setItem] = useState<Item>();
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!listItemDefinition) {
      return;
    }

    const fetchData = async () => {
      try {
        setLoading(true);
        const item = await listsApi.getItem(listId!, itemId!);
        setItem(item);
      } catch (e: any) {
        setError(e.message);
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, [listId, itemId, listItemDefinition]);

  const breadcrumbs = [
    {
      title: "Lists",
      onClick: () => navigate("/"),
    },
    {
      title: listItemDefinition?.name || "",
      onClick: () => navigate(`/${listId}`),
    },
  ];

  return loading ? (
    <Loading />
  ) : (
    <Container maxWidth="xl">
      <Stack sx={{ px: 2, py: 4 }}>
        <Titlebar title={`Id ${item?.id}`} breadcrumbs={breadcrumbs} />
      </Stack>
    </Container>
  );
};

export default ListItemPage;
