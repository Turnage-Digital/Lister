import * as React from "react";
import { FormEvent, useEffect, useState } from "react";

import { ContentPaste, Save } from "@mui/icons-material";
import { LoadingButton } from "@mui/lab";
import { Box, Divider, Stack } from "@mui/material";
import { useMutation, useSuspenseQuery } from "@tanstack/react-query";
import { createFileRoute } from "@tanstack/react-router";

import {
  EditListItemColumnContent,
  EditListItemStatusesContent,
  FormBlock,
  SmartPasteDialog,
  Titlebar,
  useSideDrawer,
} from "../components";
import { ListItem } from "../models";
import { listDefinitionQueryOptions } from "../query-options";

const RouteComponent = () => {
  const { openDrawer, closeDrawer } = useSideDrawer();
  const { listId } = Route.useParams();
  const navigate = Route.useNavigate();
  const { queryClient } = Route.useRouteContext();

  const createItemMutation = useMutation({
    mutationFn: async (item: ListItem) => {
      const request = new Request(`/api/lists/${listId}/items`, {
        headers: {
          "Content-Type": "application/json",
        },
        method: "POST",
        body: JSON.stringify(item),
      });
      const response = await fetch(request);
      const retval: ListItem = await response.json();
      return retval;
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries();
    },
  });

  const listDefinitionQuery = useSuspenseQuery(
    listDefinitionQueryOptions(listId),
  );

  const defaultListItem: ListItem = {
    id: null,
    listId,
    bag: {},
  };

  const [updated, setUpdated] = useState<ListItem>(() => {
    const item = window.sessionStorage.getItem(
      listDefinitionQuery.data.id ?? "updated_item",
    );
    return item ? JSON.parse(item) : defaultListItem;
  });

  useEffect(() => {
    window.sessionStorage.setItem(
      listDefinitionQuery.data.id ?? "updated_item",
      JSON.stringify(updated),
    );
  }, [listDefinitionQuery, updated]);

  const handleUpdate = (key: string, value: any) => {
    const newBag = { ...updated.bag, [key]: value };
    setUpdated({ ...updated, bag: newBag });
  };

  const handlePaste = async (text: string) => {
    const command = { text };

    const postRequest = new Request(
      `/api/lists/${listDefinitionQuery.data.id}/items/convert-text-to-list-item`,
      {
        headers: {
          "Content-Type": "application/json",
        },
        method: "POST",
        body: JSON.stringify(command),
      },
    );

    const response = await fetch(postRequest);
    const json: ListItem = await response.json();

    setUpdated({ ...updated, bag: json.bag });
    closeDrawer();
  };

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();
    const mutated = await createItemMutation.mutateAsync(updated);
    if (mutated.id === null) {
      throw new Error("Item was not created.");
    }
    window.sessionStorage.removeItem(
      listDefinitionQuery.data.id ?? "updated_item",
    );
    await navigate({
      to: "/$listId/$itemId",
      params: { listId, itemId: mutated.id },
    });
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
      title: listDefinitionQuery.data.name,
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
        <LoadingButton
          type="submit"
          variant="contained"
          startIcon={<Save />}
          loading={createItemMutation.isPending}
          sx={{ width: { xs: "100%", md: "auto" } }}
        >
          Submit
        </LoadingButton>
      </Box>
    </Stack>
  );
};

export const Route = createFileRoute("/_auth/$listId/create")({
  component: RouteComponent,
});
