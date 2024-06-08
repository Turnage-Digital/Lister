import { createContext } from "react";

import { Info } from "../../api";

export interface AuthValue {
  loggedIn: boolean;
  info: Info | null;
  login: (username: string, password: string) => Promise<void>;
  logout: () => Promise<void>;
}

const defaultValue: AuthValue = {
  loggedIn: false,
  info: null,
  login: async () => {},
  logout: async () => {},
};

const AuthContext = createContext<AuthValue>(defaultValue);

export default AuthContext;
