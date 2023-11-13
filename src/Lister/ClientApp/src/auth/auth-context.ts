import { createContext } from "react";

import { Claim } from "../api";

interface Props {
  loading: boolean;
  error: string | null;
  signedIn: boolean;
  claims: Claim[];
  signIn: (username: string, password: string) => Promise<void>;
  signOut: () => Promise<void>;
}

const defaultValue: Props = {
  loading: false,
  error: null,
  signedIn: false,
  claims: [],
  signIn: async () => {},
  signOut: async () => {},
};

const AuthContext = createContext<Props>(defaultValue);

export default AuthContext;
