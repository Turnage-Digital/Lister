import {queryOptions} from "@tanstack/react-query";

import {ItemDetails, ListItemDefinition, ListName, ListSearch, PagedList,} from "./models";

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
            const retval: ListItemDefinition = await response.json();
            return retval;
        },
        enabled: Boolean(listId),
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
            let url = `/api/lists/${listId}/items?page=${search.page}&pageSize=${search.pageSize}`;
            if (search.field && search.sort) {
                url += `&field=${search.field}&sort=${search.sort}`;
            }
            const request = new Request(url, {
                method: "GET",
            });
            const response = await fetch(request);
            const retval: PagedList = await response.json();
            return retval;
        },
        enabled: Boolean(listId),
    });

export const itemQueryOptions = (listId?: string, itemId?: number) =>
    queryOptions({
        queryKey: ["list-item", listId, itemId],
        queryFn: async () => {
            const request = new Request(`/api/lists/${listId}/items/${itemId}`, {
                method: "GET",
            });
            const response = await fetch(request);
            const retval: ItemDetails = await response.json();
            return retval;
        },
        enabled: Boolean(listId) && Boolean(itemId),
    });
