import React, { PropsWithChildren, useEffect, useMemo, useState } from "react";
import { useLocation, useNavigate } from "react-router-dom";

import { Claim } from "../models";

import AuthContext from "./auth-context";

type Props = PropsWithChildren;

const AuthProvider = ({ children }: Props) => {
  const navigate = useNavigate();
  const location = useLocation();

  // const [signedIn, setSignedIn] = useState<boolean>(false);
  const [claims, setClaims] = useState<Claim[]>([]);

  useEffect(() => {
    if (claims.length > 0) {
      return;
    } else if (claims.length === 0) {
      navigate("/sign-in", { state: { redirectTo: location.pathname } });
    }

    const postRequest = new Request(`${process.env.PUBLIC_URL}/claims`, {
      method: "GET",
    });
    // eslint-disable-next-line promise/catch-or-return
    fetch(postRequest)
      .then((response) => {
        return response.json();
      })
      .then((claims: Claim[]) => {
        setClaims(claims);
      });
  }, [claims, location, navigate]);

  const signOut = async () => {
    setClaims([]);
  };

  const authContextValue = useMemo(() => {
    return {
      claims,
      signOut,
    };
  }, [claims]);

  return (
    <AuthContext.Provider value={authContextValue}>
      {children}
    </AuthContext.Provider>
  );
};

export default AuthProvider;
