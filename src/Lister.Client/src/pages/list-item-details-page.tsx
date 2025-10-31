import * as React from "react";

import { History } from "@mui/icons-material";
import { useSuspenseQuery } from "@tanstack/react-query";
import { useNavigate, useParams } from "react-router-dom";

import {
  DisplayPageLayout,
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

  const definition = listItemDefinitionQuery.data;
  const item = itemQuery.data;

  const handleNavigateToLists = () => {
    navigate("/");
  };

  const handleNavigateToList = () => {
    navigate(`/${listId}`);
  };

  const handleShowHistory = () => {
    openDrawer(
      "Item history",
      <ItemHistoryDrawer listId={listId} itemId={Number(itemId)} />,
    );
  };

  const handleEditItem = (currentListId: string, currentItemId: number) => {
    navigate(`/${currentListId}/${currentItemId}/edit`);
  };

  const handleViewItem = (currentListId: string, currentItemId: number) => {
    navigate(`/${currentListId}/${currentItemId}`);
  };

  const breadcrumbs = [
    {
      title: "Lists",
      onClick: handleNavigateToLists,
    },
    {
      title: definition.name || "",
      onClick: handleNavigateToList,
    },
  ];

  const actions = [
    {
      title: "Show history",
      icon: <History />,
      variant: "outlined" as const,
      color: "secondary" as const,
      onClick: handleShowHistory,
    },
  ];

  return (
    <>
      <Titlebar
        title={`ID ${item.id}`}
        breadcrumbs={breadcrumbs}
        actions={actions}
      />

      <ItemCard
        item={item}
        definition={definition}
        onEditItem={handleEditItem}
        onViewItem={handleViewItem}
      />
    </>
  );
};

export default ListItemDetailsPage;
