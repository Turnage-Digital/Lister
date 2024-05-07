import React, {
  PropsWithChildren,
  ReactElement,
  useEffect,
  useMemo,
  useState,
} from "react";

import { Claim, IUsersApi, UsersApi } from "../../api";
import Loading from "../loading";

import AuthContext from "./auth-context";
import SignInForm from "./sign-in-form";

type Props = PropsWithChildren;

const userApi: IUsersApi = new UsersApi(`/api/users`);

const AuthProvider = ({ children }: Props) => {
  const [signedIn, setSignedIn] = useState<boolean>(true);
  const [claims, setClaims] = useState<Claim[]>([]);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchData = async () => {
      setLoading(true);

      try {
        const result = await userApi.getClaims();

        if (result.succeeded) {
          setSignedIn(true);
          setClaims(result.claims);
        }
      } catch (e: any) {
        setError(e.message);
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, [signedIn]);

  const signIn = async (username: string, password: string) => {
    setLoading(true);
    const result = await userApi.signIn(username, password);
    if (result.succeeded) {
      setSignedIn(result.succeeded);
    } else {
      setError("Invalid username or password.");
    }
    setLoading(false);
  };

  const signOut = async () => {
    setLoading(true);
    await userApi.signOut();
    setSignedIn(false);
    setLoading(false);
  };

  let content: ReactElement;
  if (signedIn) {
    content = <>{children}</>;
  } else if (loading) {
    content = <Loading />;
  } else {
    content = <SignInForm signIn={signIn} error={error} />;
  }

  const authContextValue = useMemo(() => {
    return {
      signedIn,
      setSignedIn,
      claims,
      signOut,
    };
  }, [signedIn, claims]);

  return (
    <AuthContext.Provider value={authContextValue}>
      {content}
    </AuthContext.Provider>
  );
};

export default AuthProvider;
