import { queryOptions } from "@tanstack/react-query";

import { ListIdSearch, ListName } from "./models";

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

export const pagedItemsQueryOptions = (search: ListIdSearch, listId?: string) =>
  queryOptions({
    queryKey: [
      "list-items",
      listId,
      `${listId}-${search.page}-${search.pageSize}-${search.field}-${search.sort}`,
    ],
    queryFn: async () => {
      const lol = search.toString();
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
