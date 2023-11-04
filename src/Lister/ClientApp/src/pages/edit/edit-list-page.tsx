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
  LoaderFunctionArgs,
  redirect,
  useLoaderData,
  useSubmit,
} from "react-router-dom";

import { EditBlock } from "../../components";
import { IListDefsApi, ListDef, ListDefsApi } from "../../api";

import NameBlock from "./name-block";
import StatusesBlock from "./statuses-block";
import ColumnsBlock from "./columns-block";

const listDefsApi: IListDefsApi = new ListDefsApi(
  `${process.env.PUBLIC_URL}/api/list-defs`
);

const defaultListDef: ListDef = {
  id: null,
  userId: "",
  name: "",
  statusDefs: [],
  columnDefs: [],
};

export const editListPageLoader = async ({ params }: LoaderFunctionArgs) => {
  let retval: ListDef;

  if (params.id) {
    retval = await listDefsApi.getById(params.id);
  } else {
    retval = defaultListDef;
  }

  return retval;
};

// https://github.com/remix-run/react-router/discussions/9858#discussioncomment-4638753
export const editListPageAction = async ({ request }: ActionFunctionArgs) => {
  const data = await request.formData();
  const serialized = data.get("serialized") as string;
  const thingDef = JSON.parse(serialized) as ListDef;

  await listDefsApi.create(thingDef);

  return redirect(`/lists`);
};

const EditListPage = () => {
  const submit = useSubmit();
  const loaded = useLoaderData() as ListDef;
  const [updated, setUpdated] = useState<ListDef>(loaded);

  const update = (key: keyof ListDef, value: any) => {
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

        <EditBlock
          title="Name"
          blurb="Blurb about naming a list."
          content={
            <NameBlock
              name={updated.name}
              updateName={(name) => update("name", name)}
            />
          }
        />

        <EditBlock
          title="Columns"
          blurb="Blurb about columns for a list."
          content={
            <ColumnsBlock
              columnDefs={updated.columnDefs}
              updateColumnDefs={(columnDefs) =>
                update("columnDefs", columnDefs)
              }
            />
          }
        />

        <EditBlock
          title="Statuses"
          blurb="Blurb about statuses for a list item."
          content={
            <StatusesBlock
              statusDefs={updated.statusDefs}
              updateStatusDefs={(statusDefs) =>
                update("statusDefs", statusDefs)
              }
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

export default EditListPage;
