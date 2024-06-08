import { Stack } from "@mui/material";
import React, { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";

import { IListsApi, Item, ListsApi } from "../../api";
import { Titlebar, useListDefinition, useLoad } from "../../components";

const listsApi: IListsApi = new ListsApi(`/api/lists`);

const ListItemPage = () => {
  const { listItemDefinition } = useListDefinition();
  const { loading, setLoading } = useLoad();

  const navigate = useNavigate();
  const { listId, itemId } = useParams();

  const [item, setItem] = useState<Item>();

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
        // setError(e.message);
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, [listId, itemId, listItemDefinition, setLoading]);

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

  return loading || listItemDefinition === null ? null : (
    <Stack sx={{ px: 2, py: 4 }}>
      <Titlebar title={`Id ${item?.id}`} breadcrumbs={breadcrumbs} />
    </Stack>
  );
};

export default ListItemPage;
