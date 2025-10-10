import * as React from "react";

import { History } from "@mui/icons-material";
import { Grid, Stack } from "@mui/material";
import { useSuspenseQuery } from "@tanstack/react-query";
import { useNavigate, useParams } from "react-router-dom";

import {
  ItemCard,
  ItemHistoryDrawer,
  Titlebar,
  useSideDrawer,
} from "../components";
import {
  itemQueryOptions,
  listItemDefinitionQueryOptions,
} from "../query-options";

const ListItemDetailsPage = () => {
  const { listId, itemId } = useParams<{ listId: string; itemId: string }>();
  if (!listId || !itemId) {
    throw new Error("List id and item id are required");
  }

  const navigate = useNavigate();
  const { openDrawer } = useSideDrawer();

  const listItemDefinitionQuery = useSuspenseQuery(
    listItemDefinitionQueryOptions(listId),
  );

  const itemQuery = useSuspenseQuery(itemQueryOptions(listId, Number(itemId)));

  if (!listItemDefinitionQuery.isSuccess || !itemQuery.isSuccess) {
    return null;
  }

  const breadcrumbs = [
    {
      title: "Lists",
      onClick: () => navigate(`/`),
    },
    {
      title: listItemDefinitionQuery.data.name || "",
      onClick: () => navigate(`/${listId}`),
    },
  ];

  const actions = [
    {
      title: "Show history",
      icon: <History />,
      onClick: () =>
        openDrawer(
          "Item history",
          <ItemHistoryDrawer listId={listId} itemId={Number(itemId)} />,
        ),
    },
  ];

  return (
    <Stack sx={{ px: 2, py: 4 }} spacing={4}>
      <Titlebar
        title={`ID ${itemQuery.data.id}`}
        breadcrumbs={breadcrumbs}
        actions={actions}
      />

      <Grid container spacing={2}>
        <Grid size={{ xs: 12, md: 6 }}>
          <ItemCard
            item={itemQuery.data}
            definition={listItemDefinitionQuery.data}
            onEditItem={(currentListId, currentItemId) =>
              navigate(`/${currentListId}/${currentItemId}/edit`)
            }
            onViewItem={(currentListId, currentItemId) =>
              navigate(`/${currentListId}/${currentItemId}`)
            }
          />
        </Grid>
      </Grid>
    </Stack>
  );
};

export default ListItemDetailsPage;
