import { createContext } from "react";

import { Claim } from "../models";

interface Props {
  claims: Claim[];
  signOut: () => Promise<void>;
}

const defaultValue: Props = {
  claims: [],
  signOut: async () => {},
};

const AuthContext = createContext<Props>(defaultValue);

export default AuthContext;
