import { createContext } from "react";

import { Claim } from "../../api";

export interface AuthContextProps {
  signedIn: boolean;
  setSignedIn: (value: boolean) => void;
  claims: Claim[];
  signOut: () => Promise<void>;
}

const defaultValue: AuthContextProps = {
  signedIn: false,
  setSignedIn: () => {},
  claims: [],
  signOut: async () => {},
};

const AuthContext = createContext<AuthContextProps>(defaultValue);

export default AuthContext;
