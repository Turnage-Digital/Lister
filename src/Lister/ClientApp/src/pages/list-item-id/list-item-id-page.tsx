import React from "react";
import { LoaderFunctionArgs, useLoaderData } from "react-router-dom";
import { Container, Divider, Stack } from "@mui/material";

import { FormHeader } from "../../components";
import { Item, ListItemDefinition } from "../../models";

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
  const { listItemDefinition, item } = useLoaderData() as {
    listItemDefinition: ListItemDefinition;
    item: Item;
  };

  return (
    <Container>
      <Stack spacing={4} divider={<Divider />} sx={{ px: 2, py: 4 }}>
        <FormHeader
          header={`Item # ${item.id}`}
          currentRoute={item.id ?? ""}
          previousRoute="Home"
        />
      </Stack>
    </Container>
  );
};

export default ListItemIdPage;
