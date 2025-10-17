import * as React from "react";
import { useEffect, useMemo, useState } from "react";

import { ContentPaste } from "@mui/icons-material";
import {
  useMutation,
  useQueryClient,
  useSuspenseQuery,
} from "@tanstack/react-query";
import { useNavigate, useParams } from "react-router-dom";

import {
  EditorPageLayout,
  ListItemEditor,
  SmartPasteDialog,
  Titlebar,
} from "../components";
import { ListItem } from "../models";
import { listItemDefinitionQueryOptions } from "../query-options";

const CreateListItemPage = () => {
  const { listId } = useParams<{ listId: string }>();
  if (!listId) {
    throw new Error("List id is required");
  }

  const [smartPasteOpen, setSmartPasteOpen] = useState(false);

  const navigate = useNavigate();
  const queryClient = useQueryClient();

  const createItemMutation = useMutation({
    mutationFn: async (item: ListItem) => {
      const request = new Request(`/api/lists/${listId}/items`, {
        headers: {
          "Content-Type": "application/json",
        },
        method: "POST",
        body: JSON.stringify({ bag: item.bag }),
      });
      const response = await fetch(request);
      const retval: ListItem = await response.json();
      return retval;
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries();
    },
  });

  const listItemDefinitionQuery = useSuspenseQuery(
    listItemDefinitionQueryOptions(listId),
  );

  const definition = listItemDefinitionQuery.data;

  const [formState, setFormState] = useState<ListItem>({
    id: null,
    listId,
    bag: {},
  });

  const initialStatus = useMemo(
    () => definition.statuses[0]?.name,
    [definition.statuses],
  );

  useEffect(() => {
    setFormState({
      id: null,
      listId,
      bag: initialStatus ? { status: initialStatus } : {},
    });
  }, [initialStatus, listId, definition.id]);

  const handleBagChange = (nextBag: Record<string, unknown>) => {
    setFormState((prev) => ({ ...prev, bag: nextBag }));
  };

  const handlePaste = async (text: string) => {
    const command = { text };

    const postRequest = new Request(
      `/api/lists/${listItemDefinitionQuery.data.id}/items/convert-text-to-list-item`,
      {
        headers: {
          "Content-Type": "application/json",
        },
        method: "POST",
        body: JSON.stringify(command),
      },
    );

    const response = await fetch(postRequest);
    const json: ListItem = await response.json();

    setFormState((prev) => ({ ...prev, bag: { ...prev.bag, ...json.bag } }));
    setSmartPasteOpen(false);
  };

  const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    const mutated = await createItemMutation.mutateAsync(formState);
    if (mutated.id === null) {
      throw new Error("Item was not created.");
    }
    navigate(`/${listId}/${mutated.id}`);
  };

  const actions = [
    {
      title: "Smart Paste",
      icon: <ContentPaste />,
      onClick: () => setSmartPasteOpen(true),
    },
  ];

  const breadcrumbs = [
    {
      title: "Lists",
      onClick: () => navigate(`/`),
    },
    {
      title: listItemDefinitionQuery.data.name,
      onClick: () => navigate(`/${listId}`),
    },
  ];

  const isSubmitting = createItemMutation.isPending;

  return (
    <>
      <EditorPageLayout>
        <Titlebar
          title="Create an Item"
          actions={actions}
          breadcrumbs={breadcrumbs}
        />
        <ListItemEditor
          definition={definition}
          bag={formState.bag}
          onBagChange={handleBagChange}
          onSubmit={handleSubmit}
          isSubmitting={isSubmitting}
        />
      </EditorPageLayout>
      <SmartPasteDialog
        open={smartPasteOpen}
        onClose={() => setSmartPasteOpen(false)}
        onPaste={handlePaste}
      />
    </>
  );
};

export default CreateListItemPage;
