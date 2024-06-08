import { Info } from "./models";

export interface IUsersApi {
  login(
    username: string,
    password: string
  ): Promise<{ succeeded: boolean; errorMessage?: string }>;

  getInfo(): Promise<{
    succeeded: boolean;
    info: Info | null;
    errorMessage?: string;
  }>;

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
  ): Promise<{ succeeded: boolean; errorMessage?: string }> {
    let retval: { succeeded: boolean; errorMessage?: string };

    try {
      const input = { email, password };
      const request = new Request(`${this.baseUrl}/login?useCookies=true`, {
        headers: {
          "Content-Type": "application/json",
        },
        method: "POST",
        body: JSON.stringify(input),
      });
      const response = await fetch(request);
      if (response.status === 401) {
        retval = { succeeded: false };
      } else {
        retval = { succeeded: true };
      }
    } catch (e: any) {
      retval = { succeeded: false, errorMessage: e.message };
    }

    return retval;
  }

  public async getInfo(): Promise<{
    succeeded: boolean;
    info: Info | null;
    errorMessage?: string;
  }> {
    let retval: {
      succeeded: boolean;
      info: Info | null;
      errorMessage?: string;
    };

    try {
      const request = new Request(`${this.baseUrl}/manage/info`, {
        method: "GET",
      });
      const response = await fetch(request);
      if (response.status === 401) {
        retval = { succeeded: false, info: null };
      } else {
        const info = await response.json();
        retval = { succeeded: true, info };
      }
    } catch (e: any) {
      retval = { succeeded: false, info: null, errorMessage: e.message };
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
