import React, { FormEvent, useEffect, useState } from "react";
import {
  Box,
  Button,
  Container,
  Divider,
  FormControl,
  InputLabel,
  MenuItem,
  Select,
  Stack,
  TextField,
} from "@mui/material";
import { Save } from "@mui/icons-material";
import {
  ActionFunctionArgs,
  LoaderFunctionArgs,
  redirect,
  useLoaderData,
  useSubmit,
} from "react-router-dom";

import { Item, ListItemDefinition } from "../../models";
import { FormBlock, FormHeader, StatusChip } from "../../components";
import { getStatusFromName } from "../../status-fns";

const defaultListItem = {
  bag: {},
};

export const editListItemPageLoader = async ({
  params,
}: LoaderFunctionArgs) => {
  if (!params.listId) {
    return null;
  }

  const getRequest = new Request(
    `${process.env.PUBLIC_URL}/api/lists/${params.listId}/itemDefinition`,
    {
      method: "GET",
    }
  );
  const response = await fetch(getRequest);
  if (response.status === 401) {
    return null;
  }
  if (response.status === 404) {
    return null;
  }
  const listItemDefinition = await response.json();
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

  const columnContent = (
    <Stack spacing={2}>
      {loaded.listItemDefinition.columns.map((column) => {
        return (
          <TextField
            key={column.name}
            label={column.name}
            value={updated.bag[column.property!] ?? ""}
            onChange={(e) => update(column.property!, e.target.value)}
          />
        );
      })}
    </Stack>
  );

  const statusesContent = (
    <Stack spacing={2}>
      <FormControl variant="outlined" margin="normal" fullWidth>
        <InputLabel htmlFor="status">Status</InputLabel>
        <Select
          name="status"
          id="status"
          label="Status"
          renderValue={(value) => (
            <StatusChip
              status={getStatusFromName(
                loaded.listItemDefinition.statuses,
                value
              )}
            />
          )}
          value={updated.bag.status ?? ""}
          onChange={(event) => update("status", event.target.value)}
        >
          {loaded.listItemDefinition.statuses.map((status) => (
            <MenuItem key={status.name} value={status.name}>
              {status.name}
            </MenuItem>
          ))}
        </Select>
      </FormControl>
    </Stack>
  );

  return (
    <Container component="form" onSubmit={handleSubmit}>
      <Stack spacing={4} divider={<Divider />} sx={{ px: 2, py: 4 }}>
        <FormHeader
          currentHeader="Create an Item"
          previousHeader={loaded.listItemDefinition.name}
        />

        <FormBlock
          title="Columns"
          blurb="Blurb about columns for an item."
          content={<Stack spacing={2}>{columnContent}</Stack>}
        />

        <FormBlock
          title="Status"
          blurb="Blurb about a status for an item."
          content={statusesContent}
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
  );
};

export default EditListItemPage;
