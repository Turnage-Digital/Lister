import React, { useState } from "react";
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
import { IListsApi, List, ListsApi } from "../../api";

import NameBlock from "./name-block";
import StatusesBlock from "./statuses-block";
import ColumnsBlock from "./columns-block";

const listsApi: IListsApi = new ListsApi(`${process.env.PUBLIC_URL}/api/lists`);

const defaultList: List = {
  id: null,
  userId: "",
  name: "",
  statuses: [],
  columns: [],
};

export const createListPageLoader = async () => {
  const retval = defaultList;
  return retval;
};

// https://github.com/remix-run/react-router/discussions/9858#discussioncomment-4638753
export const createListPageAction = async ({ request }: ActionFunctionArgs) => {
  const data = await request.formData();
  const serialized = data.get("serialized") as string;
  const parsed = JSON.parse(serialized) as List;

  await listsApi.create(parsed);

  return redirect(`/lists`);
};

const CreateListPage = () => {
  const submit = useSubmit();
  const loaded = useLoaderData() as List;
  const [updated, setUpdated] = useState<List>(loaded);

  const update = (key: keyof List, value: any) => {
    setUpdated((prev) => ({ ...prev, [key]: value }));
  };

  const handleSubmit = async () => {
    const data = {
      serialized: JSON.stringify(updated),
    };

    submit(data, {
      method: "post",
    });
  };

  return (
    <Container>
      <Stack spacing={4} divider={<Divider />} sx={{ p: 4 }}>
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
          blurb="Blurb about statuses for a list item."
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
            variant="contained"
            color="primary"
            startIcon={<Save />}
            sx={{ width: { xs: "100%", md: "auto" } }}
            onClick={handleSubmit}
          >
            Submit
          </Button>
        </Box>
      </Stack>
    </Container>
  );
};

export default CreateListPage;
