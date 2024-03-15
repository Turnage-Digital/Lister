import React, { FormEvent, useEffect, useState } from "react";
import { Box, Button, Container, Divider, Stack } from "@mui/material";
import { Save } from "@mui/icons-material";
import {
  ActionFunctionArgs,
  LoaderFunctionArgs,
  redirect,
  useLoaderData,
  useSubmit,
} from "react-router-dom";

import { Item, ListItemDefinition } from "../../models";
import { FormBlock, FormHeader } from "../../components";

import StatusesContent from "./statuses-content";
import ColumnContent from "./column-content";

const defaultListItem: Item = {
  id: null,
  listId: null,
  serialNumber: null,
  bag: {},
};

export const editListItemPageLoader = async ({
  params,
}: LoaderFunctionArgs) => {
  if (!params.listId) {
    return null;
  }

  const getListItemDefinitionResponse = await fetch(
    `${process.env.PUBLIC_URL}/api/lists/${params.listId}/itemDefinition`
  );
  const listItemDefinition = await getListItemDefinitionResponse.json();
  return { listItemDefinition, defaultListItem };
};

export const editListItemPageAction = async ({
  params,
  request,
}: ActionFunctionArgs) => {
  const data = await request.formData();
  const serialized = data.get("serialized") as string;

  const postRequest = new Request(
    `${process.env.PUBLIC_URL}/api/lists/${params.listId}/items/create`,
    {
      headers: {
        "Content-Type": "application/json",
      },
      method: "POST",
      body: serialized,
    }
  );

  await fetch(postRequest);
  return redirect(`/${params.listId}`);
};

const EditListItemPage = () => {
  const loaded = useLoaderData() as {
    listItemDefinition: ListItemDefinition;
    defaultListItem: Item;
  };
  const submit = useSubmit();

  const [updated, setUpdated] = useState<Item>(() => {
    const item = window.sessionStorage.getItem("updated_item");
    return item ? JSON.parse(item) : loaded.defaultListItem;
  });

  useEffect(() => {
    window.sessionStorage.setItem("updated_item", JSON.stringify(updated));
  }, [updated]);

  const update = (key: string, value: any) => {
    const newBag = { ...updated.bag, [key]: value };
    setUpdated({ ...updated, bag: newBag });
  };

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();

    const data = {
      serialized: JSON.stringify(updated),
    };

    submit(data, {
      method: "post",
    });

    window.sessionStorage.removeItem("updated_item");
  };

  return loaded.listItemDefinition ? (
    <Container component="form" onSubmit={handleSubmit}>
      <Stack spacing={4} divider={<Divider />} sx={{ px: 2, py: 4 }}>
        <FormHeader
          currentHeader="Create an Item"
          previousHeader={loaded.listItemDefinition.name}
        />

        <FormBlock
          title="Columns"
          blurb="Blurb about columns for an item."
          content={
            <ColumnContent
              listItemDefinition={loaded.listItemDefinition}
              item={updated}
              onItemUpdated={update}
            />
          }
        />

        <FormBlock
          title="Status"
          blurb="Blurb about a status for an item."
          content={
            <StatusesContent
              listItemDefinition={loaded.listItemDefinition}
              item={updated}
              onItemUpdated={update}
            />
          }
        />

        <Box
          sx={{
            display: "flex",
            justifyContent: { xs: "center", md: "flex-end" },
          }}
        >
          <Button
            type="submit"
            variant="contained"
            color="primary"
            startIcon={<Save />}
            sx={{ width: { xs: "100%", md: "auto" } }}
          >
            Submit
          </Button>
        </Box>
      </Stack>
    </Container>
  ) : null;
};

export default EditListItemPage;
