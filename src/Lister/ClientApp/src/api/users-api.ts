import { Claim } from "./models";

export interface IUsersApi {
  signIn(username: string, password: string): Promise<{ succeeded: boolean }>;

  getClaims(): Promise<{ claims: Claim[] }>;

  signOut(): Promise<void>;
}

export class UsersApi implements IUsersApi {
  private readonly baseUrl: string;

  constructor(baseUrl: string) {
    this.baseUrl = baseUrl;
  }

  public async signIn(
    username: string,
    password: string
  ): Promise<{ succeeded: boolean }> {
    const input = { username, password };
    const request = new Request(`${this.baseUrl}/sign-in`, {
      headers: {
        "Content-Type": "application/json",
      },
      method: "POST",
      body: JSON.stringify(input),
    });
    const response = await fetch(request);
    const retval = await response.json();
    return retval;
  }

  public async getClaims(): Promise<{ claims: Claim[] }> {
    const request = new Request(`${this.baseUrl}/claims`, {
      method: "GET",
    });
    const response = await fetch(request);
    const claims = await response.json();
    return { claims };
  }

  public async signOut(): Promise<void> {
    const request = new Request(`${this.baseUrl}/sign-out`, {
      method: "POST",
    });
    await fetch(request);
  }
}
