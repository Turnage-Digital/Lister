import * as React from "react";

export type AuthStatus = "loggedOut" | "loggedIn";

export interface Auth {
  status: AuthStatus;
  username?: string;
  login: (username: string) => void;
  logout: () => void;
}

interface StoredAuth {
  status: AuthStatus;
  username?: string;
}

export const getStoredAuth = (): StoredAuth => {
  try {
    const stored = sessionStorage.getItem("auth");
    if (stored) {
      const parsed = JSON.parse(stored) as Partial<StoredAuth>;
      return {
        status: parsed.status === "loggedIn" ? "loggedIn" : "loggedOut",
        username:
          typeof parsed.username === "string" ? parsed.username : undefined,
      };
    }
  } catch {
    // Invalid stored data, fall back to logged out
  }
  return { status: "loggedOut", username: undefined };
};

const setStoredAuth = (status: AuthStatus, username?: string) => {
  try {
    sessionStorage.setItem("auth", JSON.stringify({ status, username }));
  } catch {
    // Storage not available, continue without persistence
  }
};

const AuthContext = React.createContext<Auth | undefined>(undefined);

export const AuthProvider = ({ children }: { children: React.ReactNode }) => {
  const [state, setState] = React.useState<StoredAuth>(() => getStoredAuth());

  const login = React.useCallback((username: string) => {
    setState({ status: "loggedIn", username });
    setStoredAuth("loggedIn", username);
  }, []);

  const logout = React.useCallback(() => {
    setState({ status: "loggedOut", username: undefined });
    setStoredAuth("loggedOut");
  }, []);

  const value = React.useMemo<Auth>(
    () => ({ status: state.status, username: state.username, login, logout }),
    [state.status, state.username, login, logout],
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
};

export const useAuth = (): Auth => {
  const context = React.useContext(AuthContext);
  if (!context) {
    throw new Error("useAuth must be used within an AuthProvider");
  }
  return context;
};
