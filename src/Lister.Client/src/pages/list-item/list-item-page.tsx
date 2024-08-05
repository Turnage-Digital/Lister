import React from "react";
import { Stack } from "@mui/material";
import { useQuery } from "@tanstack/react-query";
import { useNavigate, useParams } from "react-router-dom";

import { Titlebar } from "../../components";

const ListItemPage = () => {
  const navigate = useNavigate();
  const { listId, itemId } = useParams();

  const listDefinitionQuery = useQuery({
    queryKey: ["list-definition", listId],
    queryFn: async () => {
      const request = new Request(`/api/lists/${listId}/itemDefinition`, {
        method: "GET",
      });
      const response = await fetch(request);
      const retval = await response.json();
      return retval;
    },
    enabled: !!listId,
  });

  const itemQuery = useQuery({
    queryKey: ["list-item", listId, itemId],
    queryFn: async () => {
      const request = new Request(`/api/lists/${listId}/items/${itemId}`, {
        method: "GET",
      });
      const response = await fetch(request);
      const retval = await response.json();
      return retval;
    },
    enabled: !!listId && !!itemId,
  });

  if (!listDefinitionQuery.isSuccess || !itemQuery.isSuccess) {
    return null;
  }

  const breadcrumbs = [
    {
      title: "Lists",
      onClick: () => navigate("/"),
    },
    {
      title: listDefinitionQuery.data.name || "",
      onClick: () => navigate(`/${listId}`),
    },
  ];

  return (
    <Stack sx={{ px: 2, py: 4 }}>
      <Titlebar title={`ID ${itemQuery.data.id}`} breadcrumbs={breadcrumbs} />
    </Stack>
  );
};

export default ListItemPage;
