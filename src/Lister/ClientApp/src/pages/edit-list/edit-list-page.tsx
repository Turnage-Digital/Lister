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
  LoaderFunctionArgs,
  redirect,
  useLoaderData,
  useSubmit,
} from "react-router-dom";

import { FormBlock } from "../../components";
import { List } from "../../models";

import NameBlock from "./name-block";
import StatusesBlock from "./statuses-block";
import ColumnsBlock from "./columns-block";

const defaultList: List = {
  id: null,
  userId: "",
  name: "",
  statuses: [],
  columns: [],
};

// https://github.com/Turnage-Digital/Lister/commit/39ea265921d6b1ee47aebaf9f338b752990d7981#diff-3f13125f89541809d55b48493178ab0570736a71c6dc6e7b73f0f9aa6a22b40c
export const editListPageLoader = async ({ params }: LoaderFunctionArgs) => {
  if (params.id) {
    //
  }

  const retval = defaultList;
  return retval;
};

// https://github.com/remix-run/react-router/discussions/9858#discussioncomment-4638753
export const editListPageAction = async ({
  params,
  request,
}: ActionFunctionArgs) => {
  if (params.id) {
    //
  }

  const data = await request.formData();
  const serialized = data.get("serialized") as string;
  const parsed = JSON.parse(serialized) as List;

  const postRequest = new Request(
    `${process.env.PUBLIC_URL}/api/lists/create`,
    {
      headers: {
        "Content-Type": "application/json",
      },
      method: "POST",
      body: JSON.stringify(parsed),
    }
  );
  const response = await fetch(postRequest);
  const json = await response.json();
  const id = json.id;

  return redirect(`/${id}`);
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

export default EditListPage;
