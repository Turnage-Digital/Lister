import React, { PropsWithChildren, useEffect, useMemo, useState } from "react";

import { Info } from "../../models";

import AuthContext from "./auth-context";

type Props = PropsWithChildren;

const AuthProvider = ({ children }: Props) => {
  const [loggedIn, setLoggedIn] = useState<boolean>(false);
  const [info, setInfo] = useState<Info | null>(null);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchData = async () => {
      try {
        setLoading(true);
        const info = await userApi.getInfo();
        if (info === null) {
          setLoggedIn(false);
          setInfo(null);
        } else {
          setLoggedIn(true);
          setInfo(info);
        }
      } catch (e: any) {
        setError(e.message);
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, [loggedIn, setLoading]);

  const authContextValue = useMemo(() => {
    const login = async (username: string, password: string) => {
      try {
        setLoading(true);
        const succeeded = await userApi.login(username, password);
        if (succeeded) {
          setLoggedIn(succeeded);
        } else {
          setError("Invalid username or password.");
        }
      } catch (e: any) {
        setError(e.message);
      } finally {
        setLoading(false);
      }
    };

    const logout = async () => {
      try {
        setLoading(true);
        await userApi.logout();
        setLoggedIn(false);
      } catch (e: any) {
        setError(e.message);
      } finally {
        setLoading(false);
      }
    };

    return {
      loggedIn,
      info,
      login,
      logout,
    };
  }, [loggedIn, info, setLoading]);

  return (
    <AuthContext.Provider value={authContextValue}>
      {children}
    </AuthContext.Provider>
  );
};

export default AuthProvider;
