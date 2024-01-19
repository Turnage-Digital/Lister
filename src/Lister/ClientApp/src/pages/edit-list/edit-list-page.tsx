import React, { FormEvent, useState } from "react";
import {
  Box,
  Button,
  Container,
  Divider,
  Stack,
  Typography,
} from "@mui/material";
import { Save } from "@mui/icons-material";
import {
  ActionFunctionArgs,
  redirect,
  useLoaderData,
  useSubmit,
} from "react-router-dom";

import { FormBlock } from "../../components";
import { Column, Item, List, Status } from "../../models";

import NameBlock from "./name-block";
import StatusesBlock from "./statuses-block";
import ColumnsBlock from "./columns-block";

const defaultList = {
  name: "",
  columns: [],
  statuses: [],
};

export const editListPageLoader = async () => {
  const retval = defaultList;
  return retval;
};

export const editListPageAction = async ({ request }: ActionFunctionArgs) => {
  const data = await request.formData();
  const serialized = data.get("serialized") as string;

  const postRequest = new Request(
    `${process.env.PUBLIC_URL}/api/lists/create`,
    {
      headers: {
        "Content-Type": "application/json",
      },
      method: "POST",
      body: serialized,
    }
  );
  const response = await fetch(postRequest);
  const json = await response.json();
  const listId = json.id;

  return redirect(`/${listId}`);
};

const EditListPage = () => {
  const loaded = useLoaderData() as List;
  const submit = useSubmit();
  const [updated, setUpdated] = useState<List>(loaded);

  const update = (key: keyof List, value: any) => {
    setUpdated((prev) => ({ ...prev, [key]: value }));
  };

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();

    const data = {
      serialized: JSON.stringify(updated),
    };

    submit(data, {
      method: "post",
    });
  };

  return (
    <Container component="form" onSubmit={handleSubmit}>
      <Stack spacing={4} divider={<Divider />} sx={{ px: 2, py: 4 }}>
        <Box alignItems="center" sx={{ display: "flex" }}>
          <Box sx={{ flexGrow: 1 }}>
            <Typography variant="h5" component="h1" gutterBottom>
              Create a List
            </Typography>
          </Box>
        </Box>

        <FormBlock
          title="Name"
          blurb="Blurb about naming a list."
          content={
            <NameBlock
              name={updated.name}
              onNameChanged={(name) => update("name", name)}
            />
          }
        />

        <FormBlock
          title="Columns"
          blurb="Blurb about columns for a list."
          content={
            <ColumnsBlock
              columns={updated.columns}
              onColumnsChanged={(columns) => update("columns", columns)}
            />
          }
        />

        <FormBlock
          title="Statuses"
          blurb="Blurb about statuses for an item."
          content={
            <StatusesBlock
              statuses={updated.statuses}
              onStatusesChanged={(statuses) => update("statuses", statuses)}
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
    </Container>
  );
};

export default EditListPage;
