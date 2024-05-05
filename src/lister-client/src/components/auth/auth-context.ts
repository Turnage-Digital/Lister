import { createContext } from "react";

import { Claim } from "../../api";

interface Props {
  signedIn: boolean;
  setSignedIn: (value: boolean) => void;
  claims: Claim[];
  signOut: () => Promise<void>;
}

const defaultValue: Props = {
  signedIn: false,
  setSignedIn: () => {},
  claims: [],
  signOut: async () => {},
};

const AuthContext = createContext<Props>(defaultValue);

export default AuthContext;
