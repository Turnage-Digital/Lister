import { queryOptions } from "@tanstack/react-query";

import { ListName } from "./models";

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
      const request = new Request(`/api/lists/${listId}`, {
        method: "GET",
      });
      const response = await fetch(request);
      const retval = await response.json();
      return retval;
    },
    enabled: !!listId,
  });
