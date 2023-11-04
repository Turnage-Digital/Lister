import { ListDef } from "./models";

export interface IListDefsApi {
  create(listDef: ListDef): Promise<ListDef>;

  update(listDef: ListDef): Promise<ListDef>;

  get(): Promise<ListDef[]>;

  getById(id: string): Promise<ListDef>;
}

export class ListDefsApi implements IListDefsApi {
  private readonly baseUrl: string;

  constructor(baseUrl: string) {
    this.baseUrl = baseUrl;
  }

  public async create(listDef: ListDef): Promise<ListDef> {
    const request = new Request(`${this.baseUrl}/create`, {
      headers: {
        "Content-Type": "application/json",
      },
      method: "POST",
      body: JSON.stringify(listDef),
    });
    const response = await fetch(request);
    const retval = await response.json();
    return retval;
  }

  public async update(listDef: ListDef): Promise<ListDef> {
    const request = new Request(`${this.baseUrl}/update`, {
      headers: {
        "Content-Type": "application/json",
      },
      method: "PUT",
      body: JSON.stringify(listDef),
    });
    await fetch(request);
    return listDef;
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
