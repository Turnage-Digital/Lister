export interface Auth {
  login: (username: string) => void;
  logout: () => void;
  status: "loggedOut" | "loggedIn";
  username?: string;
}

const getStoredAuth = (): {
  status: "loggedOut" | "loggedIn";
  username?: string;
} => {
  try {
    const stored = sessionStorage.getItem("auth");
    if (stored) {
      const parsed = JSON.parse(stored);
      return {
        status: parsed.status === "loggedIn" ? "loggedIn" : "loggedOut",
        username: parsed.username,
      };
    }
  } catch {
    // Invalid stored data, fall back to logged out
  }
  return { status: "loggedOut", username: undefined };
};

const setStoredAuth = (status: "loggedOut" | "loggedIn", username?: string) => {
  try {
    sessionStorage.setItem("auth", JSON.stringify({ status, username }));
  } catch {
    // Storage not available, continue without persistence
  }
};

const initialAuth = getStoredAuth();

export const auth: Auth = {
  status: initialAuth.status,
  username: initialAuth.username,
  login: (username: string) => {
    auth.status = "loggedIn";
    auth.username = username;
    setStoredAuth("loggedIn", username);
  },
  logout: () => {
    auth.status = "loggedOut";
    auth.username = undefined;
    setStoredAuth("loggedOut");
  },
};
