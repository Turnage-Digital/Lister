import { createContext } from "react";

import { Info } from "../../api";

interface Props {
  info: Info | null;
  logout: () => Promise<void>;
}

const defaultValue: Props = {
  info: null,
  logout: async () => {},
};

const AuthContext = createContext<Props>(defaultValue);

export default AuthContext;
