import { ListDef } from "./models";

export interface IListDefsApi {
  create(thingDef: ListDef): Promise<ListDef>;

  get(): Promise<ListDef[]>;

  getById(id: string): Promise<ListDef>;
}

export class ListDefsApi implements IListDefsApi {
  private readonly baseUrl: string;

  constructor(baseUrl: string) {
    this.baseUrl = baseUrl;
  }

  public async create(thingDef: ListDef): Promise<ListDef> {
    const request = new Request(`${this.baseUrl}/create`, {
      headers: {
        "Content-Type": "application/json",
      },
      method: "POST",
      body: JSON.stringify(thingDef),
    });
    const response = await fetch(request);
    const retval = await response.json();
    return retval;
  }

  public async get(): Promise<ListDef[]> {
    const request = new Request(`${this.baseUrl}`, {
      method: "GET",
    });
    const response = await fetch(request);
    const retval = await response.json();
    return retval;
  }

  public async getById(id: string): Promise<ListDef> {
    const request = new Request(`${this.baseUrl}/${id}`, {
      method: "GET",
    });
    const response = await fetch(request);
    const retval = await response.json();
    return retval;
  }
}
