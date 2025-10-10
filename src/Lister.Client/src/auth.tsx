import * as React from "react";

export type AuthStatus = "checking" | "loggedOut" | "loggedIn";

export interface UserInfo {
  userName?: string;
  email?: string;
  name?: string;
  [key: string]: unknown;
}

interface AuthState {
  status: AuthStatus;
  username?: string;
  user: UserInfo | null;
}

export interface Auth {
  status: AuthStatus;
  username?: string;
  user: UserInfo | null;
  login: (username?: string) => void;
  logout: () => void;
  refresh: () => void;
}

const AuthContext = React.createContext<Auth | undefined>(undefined);

const getUsernameFromInfo = (info: UserInfo | null | undefined) => {
  if (!info) {
    return undefined;
  }

  if (typeof info.userName === "string" && info.userName.length > 0) {
    return info.userName;
  }
  if (typeof info.email === "string" && info.email.length > 0) {
    return info.email;
  }
  if (typeof info.name === "string" && info.name.length > 0) {
    return info.name;
  }
  return undefined;
};

const createLoggedOutState = (): AuthState => ({
  status: "loggedOut",
  username: undefined,
  user: null,
});

const createCheckingState = (username?: string): AuthState => ({
  status: "checking",
  username,
  user: null,
});

export const AuthProvider = ({ children }: { children: React.ReactNode }) => {
  const [state, setState] = React.useState<AuthState>(() =>
    createCheckingState(),
  );
  const abortControllerRef = React.useRef<AbortController | null>(null);

  const refresh = React.useCallback(() => {
    abortControllerRef.current?.abort();
    const controller = new AbortController();
    abortControllerRef.current = controller;

    setState((prev) => createCheckingState(prev.username));

    const load = async () => {
      try {
        const response = await fetch("/identity/manage/info", {
          credentials: "include",
          signal: controller.signal,
        });

        if (response.status === 204) {
          setState(createLoggedOutState());
          return;
        }

        if (!response.ok) {
          if (response.status === 401) {
            setState(createLoggedOutState());
            return;
          }

          throw new Error("Failed to load user info");
        }

        const info = (await response.json()) as UserInfo | null;
        const username = getUsernameFromInfo(info);
        setState({ status: "loggedIn", username, user: info ?? null });
      } catch (error) {
        if (error instanceof DOMException && error.name === "AbortError") {
          return;
        }

        setState(createLoggedOutState());
      }
    };

    load().catch(() => {
      // load() already handles its own error paths; this catch silences
      // the unhandled rejection warning some browsers emit when an async
      // function throws after the caller intentionally ignores the promise.
    });
  }, []);

  React.useEffect(() => {
    refresh();
    return () => {
      abortControllerRef.current?.abort();
    };
  }, [refresh]);

  const login = React.useCallback(
    (username?: string) => {
      setState(createCheckingState(username));
      refresh();
    },
    [refresh],
  );

  const logout = React.useCallback(() => {
    abortControllerRef.current?.abort();
    setState(createLoggedOutState());
  }, []);

  const value = React.useMemo<Auth>(
    () => ({
      status: state.status,
      username: state.username,
      user: state.user,
      login,
      logout,
      refresh,
    }),
    [state.status, state.username, state.user, login, logout, refresh],
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
