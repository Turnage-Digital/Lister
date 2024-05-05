import { Claim } from "./models";

export interface IUsersApi {
  signIn(username: string, password: string): Promise<{ succeeded: boolean }>;

  getClaims(): Promise<{ succeeded: boolean; claims: Claim[] }>;

  signOut(): Promise<void>;
}

export class UsersApi implements IUsersApi {
  private readonly baseUrl: string;

  constructor(baseUrl: string) {
    this.baseUrl = baseUrl;
  }

  public async signIn(
    username: string,
    password: string,
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
    if (!response.ok) {
      throw new Error("Failed to sign in.");
    }
    const retval = await response.json();
    return retval;
  }

  public async getClaims(): Promise<{ succeeded: boolean; claims: Claim[] }> {
    const request = new Request(`${this.baseUrl}/claims`, {
      method: "GET",
    });
    const response = await fetch(request);
    if (response.status === 401) {
      return { succeeded: false, claims: [] };
    } else if (!response.ok) {
      throw new Error("Failed to get claims.");
    }
    const claims = await response.json();
    const retval = { succeeded: true, claims };
    return retval;
  }

  public async signOut(): Promise<void> {
    const request = new Request(`${this.baseUrl}/sign-out`, {
      method: "POST",
    });
    const response = await fetch(request);
    if (!response.ok) {
      throw new Error("Failed to sign out.");
    }
  }
}
