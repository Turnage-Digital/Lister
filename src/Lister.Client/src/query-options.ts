import { queryOptions, useMutation } from "@tanstack/react-query";

import { ListSearch, ListName, ListItemDefinition } from "./models";

export const listNamesQueryOptions = () =>
  queryOptions({
    queryKey: ["list-names"],
    queryFn: async () => {
      const request = new Request("/api/lists/names", {
        method: "GET",
      });
      const response = await fetch(request);
      const retval: ListName[] = await response.json();
      return retval;
    },
  });

export const listDefinitionQueryOptions = (listId?: string) =>
  queryOptions({
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

export const pagedItemsQueryOptions = (search: ListSearch, listId?: string) =>
  queryOptions({
    queryKey: [
      "list-items",
      listId,
      search.page,
      search.pageSize,
      search.field ?? "id",
      search.sort ?? "asc",
    ],
    queryFn: async () => {
      let url = `/api/lists/${listId}?page=${search.page}&pageSize=${search.pageSize}`;
      if (search.field && search.sort) {
        url += `&field=${search.field}&sort=${search.sort}`;
      }
      const request = new Request(url, {
        method: "GET",
      });
      const response = await fetch(request);
      const retval = await response.json();
      return retval;
    },
    enabled: !!listId,
  });

export const useCreateListMutation = () =>
  useMutation({
    mutationFn: async (list: ListItemDefinition) => {
      const request = new Request("/api/lists/create", {
        headers: {
          "Content-Type": "application/json",
        },
        method: "POST",
        body: JSON.stringify(list),
      });
      const response = await fetch(request);
      const retval: ListItemDefinition = await response.json();
      return retval;
    },
  });
