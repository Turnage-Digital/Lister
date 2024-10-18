import { ContentPaste, Save } from "@mui/icons-material";
import { Box, Button, Divider, Stack } from "@mui/material";
import { useSuspenseQuery } from "@tanstack/react-query";
import { createFileRoute } from "@tanstack/react-router";
import React, { FormEvent, useEffect, useState } from "react";

import {
  EditListItemColumnContent,
  EditListItemStatusesContent,
  FormBlock,
  SmartPasteDialog,
  Titlebar,
  useSideDrawer,
} from "../components";
import { Item } from "../models";
import {
  listDefinitionQueryOptions,
  useAddListItemMutation,
} from "../query-options";

const RouteComponent = () => {
  const { openDrawer, closeDrawer } = useSideDrawer();
  const { listId } = Route.useParams();
  const navigate = Route.useNavigate();
  const mutation = useAddListItemMutation(listId);

  const listDefinitionQuery = useSuspenseQuery(
    listDefinitionQueryOptions(listId),
  );

  const defaultListItem: Item = {
    id: null,
    listId,
    bag: {},
  };

  const [updated, setUpdated] = useState<Item>(() => {
    const item = window.sessionStorage.getItem(
      listDefinitionQuery.data?.id ?? "updated_item",
    );
    return item ? JSON.parse(item) : defaultListItem;
  });

  useEffect(() => {
    window.sessionStorage.setItem(
      listDefinitionQuery.data?.id ?? "updated_item",
      JSON.stringify(updated),
    );
  }, [listDefinitionQuery, updated]);

  const handleUpdate = (key: string, value: any) => {
    const newBag = { ...updated.bag, [key]: value };
    setUpdated({ ...updated, bag: newBag });
  };

  const handlePaste = async (text: string) => {
    const body = {
      listId: listDefinitionQuery.data?.id,
      text,
    };

    const postRequest = new Request(`/api/lists/convert-text-to-list-item`, {
      headers: {
        "Content-Type": "application/json",
      },
      method: "POST",
      body: JSON.stringify(body),
    });

    const response = await fetch(postRequest);
    const json = await response.json();

    setUpdated({ ...updated, bag: json.bag });
    closeDrawer();
  };

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();
    const mutated = await mutation.mutateAsync(updated);
    window.sessionStorage.removeItem("updated_item");
    navigate({ to: `/${listId}/${mutated.id}` });
  };

  if (!listDefinitionQuery.isSuccess) {
    return null;
  }

  const actions = [
    {
      title: "Smart Paste",
      icon: <ContentPaste />,
      onClick: () =>
        openDrawer("Smart Paste", <SmartPasteDialog onPaste={handlePaste} />),
    },
  ];

  const breadcrumbs = [
    {
      title: "Lists",
      onClick: () => navigate({ to: "/" }),
    },
    {
      title: listDefinitionQuery.data.name ?? "",
      onClick: () => navigate({ to: `/${listId}` }),
    },
  ];

  return (
    <Stack
      component="form"
      spacing={4}
      divider={<Divider />}
      onSubmit={handleSubmit}
      sx={{ px: 2, py: 4 }}
    >
      <Titlebar
        title="Create an Item"
        actions={actions}
        breadcrumbs={breadcrumbs}
      />

      <FormBlock
        title="Columns"
        blurb="Blurb about columns for an item."
        content={
          <EditListItemColumnContent
            listItemDefinition={listDefinitionQuery.data}
            item={updated}
            onItemUpdated={handleUpdate}
          />
        }
      />

      <FormBlock
        title="Status"
        blurb="Blurb about a status for an item."
        content={
          <EditListItemStatusesContent
            listItemDefinition={listDefinitionQuery.data}
            item={updated}
            onItemUpdated={handleUpdate}
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
          startIcon={<Save />}
          sx={{ width: { xs: "100%", md: "auto" } }}
        >
          Submit
        </Button>
      </Box>
    </Stack>
  );
};

export const Route = createFileRoute("/_auth/$listId/create")({
  component: RouteComponent,
});
