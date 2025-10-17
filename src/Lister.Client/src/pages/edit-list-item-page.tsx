import * as React from "react";

import { Save } from "@mui/icons-material";
import { LoadingButton } from "@mui/lab";
import { Box, Button, Divider, Stack } from "@mui/material";
import {
  useMutation,
  useQueryClient,
  useSuspenseQuery,
} from "@tanstack/react-query";
import { useNavigate, useParams } from "react-router-dom";

import { FormBlock, ListItemEditor, Titlebar } from "../components";
import { ListItem } from "../models";
import {
  itemQueryOptions,
  listItemDefinitionQueryOptions,
} from "../query-options";

const EditListItemPage = () => {
  const { listId, itemId } = useParams<{ listId: string; itemId: string }>();
  if (!listId || !itemId) {
    throw new Error("List id and item id are required");
  }

  const navigate = useNavigate();
  const queryClient = useQueryClient();

  const definitionQuery = useSuspenseQuery(
    listItemDefinitionQueryOptions(listId),
  );
  const itemQuery = useSuspenseQuery(itemQueryOptions(listId, Number(itemId)));

  const definition = definitionQuery.data;
  const item = itemQuery.data;

  const [formState, setFormState] = React.useState<ListItem>({
    id: item.id,
    listId,
    bag: item.bag ?? {},
  });

  React.useEffect(() => {
    setFormState({
      id: item.id,
      listId,
      bag: { ...item.bag },
    });
  }, [item.id, item.bag, listId]);

  const updateItemMutation = useMutation({
    mutationFn: async (payload: ListItem) => {
      const request = new Request(`/api/lists/${listId}/items/${itemId}`, {
        headers: {
          "Content-Type": "application/json",
        },
        method: "PUT",
        body: JSON.stringify(payload.bag),
      });
      const response = await fetch(request);
      if (!response.ok) {
        throw new Error("Failed to update item");
      }
    },
    onSuccess: async () => {
      await Promise.all([
        queryClient.invalidateQueries({
          queryKey: ["list-items"],
          exact: false,
        }),
        queryClient.invalidateQueries({
          queryKey: ["list-item", listId, Number(itemId)],
        }),
      ]);
      navigate(`/${listId}/${itemId}`);
    },
  });

  const handleBagChange = (nextBag: Record<string, unknown>) => {
    setFormState((prev) => ({ ...prev, bag: nextBag }));
  };

  const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    await updateItemMutation.mutateAsync(formState);
  };

  const handleCancel = () => {
    navigate(-1);
  };

  const breadcrumbs = [
    {
      title: "Lists",
      onClick: () => navigate("/"),
    },
    {
      title: definition.name,
      onClick: () => navigate(`/${listId}`),
    },
    {
      title: `ID ${item.id}`,
      onClick: () => navigate(`/${listId}/${item.id}`),
    },
  ];

  return (
    <Stack
      component="form"
      divider={<Divider sx={{ my: { xs: 5, md: 6 } }} />}
      onSubmit={handleSubmit}
      sx={{
        maxWidth: 1180,
        width: "100%",
        mx: "auto",
        px: { xs: 3, md: 7 },
        py: { xs: 4, md: 6 },
      }}
      spacing={{ xs: 6, md: 7 }}
    >
      <Titlebar title={`Edit Item ${item.id}`} breadcrumbs={breadcrumbs} />

      <FormBlock
        title="Item details"
        subtitle="Update fields and status. Changes will be validated before saving."
        content={
          <ListItemEditor
            definition={definition}
            bag={formState.bag}
            onBagChange={handleBagChange}
          />
        }
      />

      <Box
        sx={{
          display: "flex",
          justifyContent: { xs: "center", md: "flex-end" },
          gap: 2,
        }}
      >
        <Button variant="text" onClick={handleCancel}>
          Cancel
        </Button>
        <LoadingButton
          type="submit"
          variant="contained"
          startIcon={<Save />}
          loading={updateItemMutation.isPending}
          sx={{ width: { xs: "100%", md: "auto" } }}
        >
          Save changes
        </LoadingButton>
      </Box>
    </Stack>
  );
};

export default EditListItemPage;
