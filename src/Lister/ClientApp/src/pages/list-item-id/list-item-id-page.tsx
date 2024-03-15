import React from "react";
import { LoaderFunctionArgs } from "react-router-dom";

export const listItemIdPageLoader = async ({ params }: LoaderFunctionArgs) => {
  if (!params.listId || !params.itemId) {
    return null;
  }

  const [getListItemDefinitionResponse, getItemResponse] = await Promise.all([
    fetch(
      `${process.env.PUBLIC_URL}/api/lists/${params.listId}/itemDefinition`
    ),
    fetch(`/api/lists/${params.listId}/items/${params.itemId}`),
  ]);

  const listItemDefinition = await getListItemDefinitionResponse.json();
  const item = await getItemResponse.json();

  return { listItemDefinition, item };
};

const ListItemIdPage = () => {
  return <div>ListItemIdPage</div>;
};

export default ListItemIdPage;
