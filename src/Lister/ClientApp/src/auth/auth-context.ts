import { createContext } from "react";

import { Claim } from "../api";

interface Props {
  signedIn: boolean;
  claims: Claim[];
  signOut: () => Promise<void>;
}

const defaultValue: Props = {
  signedIn: false,
  claims: [],
  signOut: async () => {},
};

const AuthContext = createContext<Props>(defaultValue);

export default AuthContext;
