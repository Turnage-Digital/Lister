import React, { FormEvent, useEffect, useState } from "react";
import { Box, Button, Divider, Stack } from "@mui/material";
import { ContentPaste, Save } from "@mui/icons-material";
import { useQuery } from "@tanstack/react-query";
import { useNavigate, useParams, useSubmit } from "react-router-dom";

import { Item, type ListItemDefinition } from "../../api";
import { FormBlock, Titlebar, useSideDrawer } from "../../components";

import ColumnContent from "./column-content";
import SmartPasteDialog from "./smart-paste-dialog";
import StatusesContent from "./statuses-content";

const EditListItemPage = () => {
  const { openDrawer, closeDrawer } = useSideDrawer();
  const navigate = useNavigate();
  const { listId } = useParams();
  const submit = useSubmit();

  const listDefinitionQuery = useQuery<ListItemDefinition>({
    queryKey: ["list-definition", listId],
    queryFn: async () => {
      const request = new Request(`/api/lists/${listId}/itemDefinition`, {
        method: "GET",
      });
      const response = await fetch(request);
      const retval = await response.json();
      return retval;
    },
  });

  const defaultListItem: Item = {
    id: null,
    listId: null,
    bag: {},
  };

  const [updated, setUpdated] = useState<Item>(() => {
    const item = window.sessionStorage.getItem(
      listDefinitionQuery.data?.id ?? "updated_item"
    );
    return item ? JSON.parse(item) : defaultListItem;
  });

  useEffect(() => {
    window.sessionStorage.setItem(
      listDefinitionQuery.data?.id ?? "updated_item",
      JSON.stringify(updated)
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

    window.sessionStorage.clear();

    const data = {
      serialized: JSON.stringify(updated),
    };

    submit(data, {
      method: "post",
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
      onClick: () => navigate("/"),
    },
    {
      title: listDefinitionQuery.data.name ?? "",
      onClick: () => navigate(`/${listId}`),
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
          <ColumnContent
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
          <StatusesContent
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

export default EditListItemPage;
