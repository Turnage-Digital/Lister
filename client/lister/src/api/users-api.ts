import { Info } from "./models";

export interface IUsersApi {
  login(username: string, password: string): Promise<boolean>;

  getInfo(): Promise<Info | null>;

  logout(): Promise<void>;
}

export class UsersApi implements IUsersApi {
  private readonly baseUrl: string;

  constructor(baseUrl: string) {
    this.baseUrl = baseUrl;
  }

  public async login(email: string, password: string): Promise<boolean> {
    const input = { email, password };
    const request = new Request(`${this.baseUrl}/login?useCookies=true`, {
      headers: {
        "Content-Type": "application/json",
      },
      method: "POST",
      body: JSON.stringify(input),
    });
    const response = await fetch(request);
    const retval = response.status !== 401;
    return retval;
  }

  public async getInfo(): Promise<Info | null> {
    const request = new Request(`${this.baseUrl}/manage/info`, {
      method: "GET",
    });
    const response = await fetch(request);
    let retval: Info | null;
    if (response.status === 401) {
      retval = null;
    } else {
      retval = await response.json();
    }
    return retval;
  }

  public async logout(): Promise<void> {
    const request = new Request(`${this.baseUrl}/logout`, {
      method: "POST",
    });
    await fetch(request);
  }
}
