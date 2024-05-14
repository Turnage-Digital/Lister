import { Info } from "./models";

export interface IUsersApi {
  login(username: string, password: string): Promise<{ succeeded: boolean }>;

  getInfo(): Promise<{ succeeded: boolean; info: Info | null }>;

  logout(): Promise<void>;
}

export class UsersApi implements IUsersApi {
  private readonly baseUrl: string;

  constructor(baseUrl: string) {
    this.baseUrl = baseUrl;
  }

  public async login(
    email: string,
    password: string
  ): Promise<{ succeeded: boolean }> {
    const input = { email, password };
    const request = new Request(`${this.baseUrl}/login?useCookies=true`, {
      headers: {
        "Content-Type": "application/json",
      },
      method: "POST",
      body: JSON.stringify(input),
    });
    const response = await fetch(request);
    if (!response.ok) {
      throw new Error("Failed to log in.");
    }
    const retval = await response.json();
    return retval;
  }

  public async getInfo(): Promise<{ succeeded: boolean; info: Info | null }> {
    const request = new Request(`${this.baseUrl}/manage/info`, {
      method: "GET",
    });
    const response = await fetch(request);
    if (response.status === 401) {
      return { succeeded: false, info: null };
    } else if (!response.ok) {
      throw new Error("Failed to get info.");
    }
    const info = await response.json();
    const retval = { succeeded: true, info };
    return retval;
  }

  public async logout(): Promise<void> {
    const request = new Request(`${this.baseUrl}/logout`, {
      method: "POST",
    });
    const response = await fetch(request);
    if (!response.ok) {
      throw new Error("Failed to log out.");
    }
  }
}
