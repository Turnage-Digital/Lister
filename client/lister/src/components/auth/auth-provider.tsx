import React, { PropsWithChildren, useEffect, useMemo, useState } from "react";

import { Info, IUsersApi, UsersApi } from "../../api";
import { useLoad } from "../load";

import AuthContext from "./auth-context";

type Props = PropsWithChildren;

const userApi: IUsersApi = new UsersApi(`/identity`);

const AuthProvider = ({ children }: Props) => {
  const { setLoading } = useLoad();

  const [loggedIn, setLoggedIn] = useState<boolean>(false);
  const [info, setInfo] = useState<Info | null>(null);

  useEffect(() => {
    const fetchData = async () => {
      try {
        setLoading(true);
        const result = await userApi.getInfo();
        if (result.succeeded) {
          setLoggedIn(true);
          setInfo(result.info);
        } else {
          setLoggedIn(false);
          setInfo(null);
          // setError(result.errorMessage || "An error occurred.");
        }
      } catch (e: any) {
        // setError(e.message);
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, [loggedIn, setLoading]);

  const authContextValue = useMemo(() => {
    const login = async (username: string, password: string) => {
      setLoading(true);
      const result = await userApi.login(username, password);
      if (result.succeeded) {
        setLoggedIn(result.succeeded);
      } else {
        // setError("Invalid username or password.");
      }
      setLoading(false);
    };

    const logout = async () => {
      setLoading(true);
      await userApi.logout();
      setLoggedIn(false);
      setLoading(false);
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
