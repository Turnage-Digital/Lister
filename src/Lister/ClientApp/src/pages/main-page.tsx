import React from "react";
import { redirect, useLoaderData } from "react-router-dom";
import { Container } from "@mui/material";

import { List } from "../api";

export const mainPageLoader = async () => {
  const request = new Request(`${process.env.PUBLIC_URL}/api/lists/names`, {
    method: "GET",
  });
  const response = await fetch(request);
  if (response.status === 401) {
    return redirect("/sign-in?callbackUrl=/");
  }
  const retval = await response.json();
  return retval;
};

const MainPage = () => {
  const loaded = useLoaderData() as List[];

  return <Container />;
};

export default MainPage;
