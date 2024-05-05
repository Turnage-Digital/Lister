import React, { FormEvent, useEffect, useState } from "react";
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
  useNavigate,
  useSubmit,
} from "react-router-dom";
import Grid from "@mui/material/Unstable_Grid2";

import { FormBlock } from "../../components";
import { ListItemDefinition } from "../../api";

import NameBlock from "./name-block";
import StatusesContent from "./statuses-content";
import ColumnsContent from "./columns-content";

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
  const submit = useSubmit();

  const defaultListDefinition: ListItemDefinition = {
    id: null,
    name: "",
    columns: [],
    statuses: [],
  };

  const [updated, setUpdated] = useState<ListItemDefinition>(() => {
    const item = window.sessionStorage.getItem("updated_list");
    return item ? JSON.parse(item) : defaultListDefinition;
  });

  useEffect(() => {
    window.sessionStorage.setItem("updated_list", JSON.stringify(updated));
  }, [updated]);

  const update = (key: keyof ListItemDefinition, value: any) => {
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

    window.sessionStorage.removeItem("updated_list");
  };

  return (
    <Container component="form" onSubmit={handleSubmit}>
      <Stack spacing={4} divider={<Divider />} sx={{ px: 2, py: 4 }}>
        <Grid container>
          <Grid xs={12} md={9}>
            <Typography
              color="primary"
              fontWeight="bold"
              variant="h5"
              component="h1"
            >
              Create a List
            </Typography>
          </Grid>
        </Grid>

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
            <ColumnsContent
              columns={updated.columns}
              onColumnsChanged={(columns) => update("columns", columns)}
            />
          }
        />

        <FormBlock
          title="Statuses"
          blurb="Blurb about statuses for an item."
          content={
            <StatusesContent
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
