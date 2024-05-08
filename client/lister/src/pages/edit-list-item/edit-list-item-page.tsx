import { ContentPaste, Save } from "@mui/icons-material";
import { Box, Button, Container, Divider, Stack } from "@mui/material";
import React, { FormEvent, useEffect, useState } from "react";
import {
  ActionFunctionArgs,
  redirect,
  useNavigate,
  useParams,
  useSubmit,
} from "react-router-dom";

import { IListsApi, Item, ListItemDefinition, ListsApi } from "../../api";
import {
  FormBlock,
  Loading,
  Titlebar,
  useAuth,
  useSideDrawer,
} from "../../components";

import ColumnContent from "./column-content";
import SmartPasteDialog from "./smart-paste-dialog";
import StatusesContent from "./statuses-content";

const listsApi: IListsApi = new ListsApi(`/api/lists`);

export const editListItemPageAction = async ({
  params,
  request,
}: ActionFunctionArgs) => {
  const data = await request.formData();
  const serialized = data.get("serialized") as string;

  const postRequest = new Request(`/api/lists/${params.listId}/items/create`, {
    headers: {
      "Content-Type": "application/json",
    },
    method: "POST",
    body: serialized,
  });

  await fetch(postRequest);
  return redirect(`/${params.listId}`);
};

const EditListItemPage = () => {
  const { signedIn } = useAuth();
  const navigate = useNavigate();
  const params = useParams();
  const submit = useSubmit();
  const { openDrawer, closeDrawer } = useSideDrawer();

  const [listItemDefinition, setListItemDefinition] =
    useState<ListItemDefinition>();
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!signedIn) {
      return;
    }

    const fetchData = async () => {
      try {
        setLoading(true);
        const listItemDefinition = await listsApi.getListItemDefinition(
          params.listId!
        );
        setListItemDefinition(listItemDefinition);
      } catch (e: any) {
        setError(e.message);
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, [params.listId, signedIn]);

  const defaultListItem: Item = {
    id: null,
    listId: null,
    bag: {},
  };

  const [updated, setUpdated] = useState<Item>(() => {
    const item = window.sessionStorage.getItem(
      listItemDefinition?.id ?? "updated_item"
    );
    return item ? JSON.parse(item) : defaultListItem;
  });

  useEffect(() => {
    window.sessionStorage.setItem(
      listItemDefinition?.id ?? "updated_item",
      JSON.stringify(updated)
    );
  }, [listItemDefinition, updated]);

  const update = (key: string, value: any) => {
    const newBag = { ...updated.bag, [key]: value };
    setUpdated({ ...updated, bag: newBag });
  };

  const handlePaste = async (text: string) => {
    const body = {
      listId: listItemDefinition?.id,
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

    const data = {
      serialized: JSON.stringify(updated),
    };

    submit(data, {
      method: "post",
    });

    window.sessionStorage.removeItem("updated_item");
  };

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
      title: listItemDefinition?.name ?? "",
      onClick: () => navigate(`/${params.listId}`),
    },
  ];

  return loading ? (
    <Loading />
  ) : (
    <Container maxWidth="xl" component="form" onSubmit={handleSubmit}>
      <Stack spacing={4} divider={<Divider />} sx={{ px: 2, py: 4 }}>
        <Titlebar
          title="Edit Item"
          actions={actions}
          breadcrumbs={breadcrumbs}
        />

        <FormBlock
          title="Columns"
          blurb="Blurb about columns for an item."
          content={
            <ColumnContent
              listItemDefinition={listItemDefinition!}
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
              listItemDefinition={listItemDefinition!}
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
  );
};

export default EditListItemPage;
