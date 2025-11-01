import * as React from "react";

import {
  useMutation,
  useQueryClient,
  useSuspenseQuery,
} from "@tanstack/react-query";
import { useNavigate, useParams } from "react-router-dom";

import { ListItemEditor, Titlebar } from "../components";
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
      handleNavigateToItemDetails();
    },
  });

  const handleNavigateToLists = () => {
    navigate("/");
  };

  const handleNavigateToList = () => {
    navigate(`/${listId}`);
  };

  const handleNavigateToItemDetails = () => {
    navigate(`/${listId}/${itemId}`);
  };

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
      onClick: handleNavigateToLists,
    },
    {
      title: definition.name,
      onClick: handleNavigateToList,
    },
    {
      title: `ID ${item.id}`,
      onClick: handleNavigateToItemDetails,
    },
  ];

  const isSubmitting = updateItemMutation.isPending;

  return (
    <>
      <Titlebar title={`Edit Item ${item.id}`} breadcrumbs={breadcrumbs} />
      <ListItemEditor
        definition={definition}
        bag={formState.bag}
        onBagChange={handleBagChange}
        onSubmit={handleSubmit}
        isSubmitting={isSubmitting}
        onCancel={handleCancel}
      />
    </>
  );
};

export default EditListItemPage;
