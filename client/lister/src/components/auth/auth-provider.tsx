import React, {
  PropsWithChildren,
  ReactElement,
  useEffect,
  useMemo,
  useState,
} from "react";

import { Info, IUsersApi, UsersApi } from "../../api";
import Loading from "../loading";

import AuthContext from "./auth-context";
import SignInForm from "./sign-in-form";

type Props = PropsWithChildren;

const userApi: IUsersApi = new UsersApi(`/identity`);

const AuthProvider = ({ children }: Props) => {
  const [loggedIn, setLoggedIn] = useState<boolean>(false);
  const [info, setInfo] = useState<Info | null>(null);
  const [loading, setLoading] = useState<boolean>(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchData = async () => {
      setLoading(true);

      try {
        const result = await userApi.getInfo();

        if (result.succeeded) {
          setLoggedIn(true);
          setInfo(result.info);
        }
      } catch (e: any) {
        setError(e.message);
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, [loggedIn]);

  const login = async (username: string, password: string) => {
    setLoading(true);
    const result = await userApi.login(username, password);
    if (result.succeeded) {
      setLoggedIn(result.succeeded);
    } else {
      setError("Invalid username or password.");
    }
    setLoading(false);
  };

  const logout = async () => {
    setLoading(true);
    await userApi.logout();
    setLoggedIn(false);
    setLoading(false);
  };

  let content: ReactElement;
  if (loggedIn) {
    content = <>{children}</>;
  } else if (loading) {
    content = <Loading />;
  } else {
    content = <SignInForm signIn={login} error={error} />;
  }

  const authContextValue = useMemo(() => {
    return {
      info,
      logout,
    };
  }, [info]);

  return (
    <AuthContext.Provider value={authContextValue}>
      {content}
    </AuthContext.Provider>
  );
};

export default AuthProvider;
