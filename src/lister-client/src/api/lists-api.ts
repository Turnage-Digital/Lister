import {Item, ListItemDefinition, ListName} from "./models";

export interface IListsApi {
    getListNames(): Promise<ListName[]>;

    getListItemDefinition(listId: string): Promise<ListItemDefinition>;

    getItems(
        listId: string,
        page: number,
        pageSize: number,
        field: string | null,
        sort: string | null
    ): Promise<{ items: Item[]; count: number }>;

    getItem(listId: string, itemId: string): Promise<Item>;
}

export class ListsApi implements IListsApi {
    private readonly baseUrl: string;

    constructor(baseUrl: string) {
        this.baseUrl = baseUrl;
    }

    public async getListNames(): Promise<ListName[]> {
        const request = new Request(`${this.baseUrl}/names`, {
            method: "GET",
        });
        const response = await fetch(request);
        const retval = await response.json();
        return retval;
    }

    public async getListItemDefinition(
        listId: string
    ): Promise<ListItemDefinition> {
        const request = new Request(`${this.baseUrl}/${listId}/itemDefinition`, {
            method: "GET",
        });
        const response = await fetch(request);
        const retval = await response.json();
        return retval;
    }

    public async getItems(
        listId: string,
        page: number,
        pageSize: number,
        field: string | null,
        sort: string | null
    ): Promise<{ items: Item[]; count: number }> {
        let url = `${this.baseUrl}/${listId}/items?page=${page}&pageSize=${pageSize}`;
        if (field && sort) {
            url += `&field=${field}&sort=${sort}`;
        }

        const request = new Request(url, {
            method: "GET",
        });
        const response = await fetch(request);
        const retval = await response.json();
        return retval;
    }

    public async getItem(listId: string, itemId: string): Promise<Item> {
        const request = new Request(`${this.baseUrl}/${listId}/items/${itemId}`, {
            method: "GET",
        });
        const response = await fetch(request);
        const retval = await response.json();
        return retval;
    }
}
