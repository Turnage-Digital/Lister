import { List, ListName } from "./models";

export interface IListsApi {
  create(list: List): Promise<List>;

  get(): Promise<List[]>;

  getNames(): Promise<ListName>;

  getById(id: string): Promise<List>;
}

export class ListsApi implements IListsApi {
  private readonly baseUrl: string;

  constructor(baseUrl: string) {
    this.baseUrl = baseUrl;
  }

  public async create(list: List): Promise<List> {
    const request = new Request(`${this.baseUrl}/create`, {
      headers: {
        "Content-Type": "application/json",
      },
      method: "POST",
      body: JSON.stringify(list),
    });
    const response = await fetch(request);
    const retval = await response.json();
    return retval;
  }

  public async get(): Promise<List[]> {
    const request = new Request(`${this.baseUrl}`, {
      method: "GET",
    });
    const response = await fetch(request);
    const retval = await response.json();
    return retval;
  }

  public async getNames(): Promise<ListName> {
    const request = new Request(`${this.baseUrl}/names`, {
      method: "GET",
    });
    const response = await fetch(request);
    const retval = await response.json();
    return retval;
  }

  public async getById(id: string): Promise<List> {
    const request = new Request(`${this.baseUrl}/${id}`, {
      method: "GET",
    });
    const response = await fetch(request);
    const retval = await response.json();
    return retval;
  }
}
