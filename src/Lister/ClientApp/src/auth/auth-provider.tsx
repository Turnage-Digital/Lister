import React, { PropsWithChildren, useMemo, useState } from "react";
import { useLocation, useNavigate } from "react-router-dom";

import { Claim, IUsersApi, UsersApi } from "../api";

import AuthContext from "./auth-context";

type Props = PropsWithChildren;

const userApi: IUsersApi = new UsersApi(`${process.env.PUBLIC_URL}/api/users`);

const AuthProvider = ({ children }: Props) => {
  const [loading, setLoading] = useState<boolean>(false);
  const [signedIn, setSignedIn] = useState<boolean>(false);
  const [claims, setClaims] = useState<Claim[]>([]);
  const [error, setError] = useState<string | null>(null);

  const signIn = async (username: string, password: string) => {
    setLoading(true);

    const { succeeded } = await userApi.signIn(username, password);
    if (succeeded) {
      setSignedIn(true);

      const claimsResult = await userApi.getClaims();
      setClaims(claimsResult.claims);
    } else {
      setError("Invalid username or password.");
    }

    setLoading(false);
  };

  const signOut = async () => {
    setLoading(true);
    await userApi.signOut();
    setSignedIn(false);
    setClaims([]);
    setLoading(false);
  };

  const authContextValue = useMemo(() => {
    return {
      loading,
      error,
      signedIn,
      claims,
      signIn,
      signOut,
    };
  }, [loading, error, signedIn, claims]);

  return (
    <AuthContext.Provider value={authContextValue}>
      {children}
    </AuthContext.Provider>
  );
};

export default AuthProvider;
